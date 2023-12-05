using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Himsa.Noah.Modules;
using System.Windows.Forms;
using System.Xml;
using EarTechnicNoahModule;
using EarTechnicNoahModule.Entity;
using EarTechnicNoahModule.Global;
using EarTechnicNoahModuleTest.Registration;
using Microsoft.Win32;
using DataType = Himsa.Noah.Modules.DataType;

namespace EarTechnicNoahModuleTest
{
    public class NoahConnectionHandler : MarshalByRefObject, ICallbackHandler
    {
        private readonly ModuleAPI _moduleApi = new ModuleAPI();
        private readonly LaunchInfo _launchInfo;
        private bool _moduleConnected;
        private Form _parentGui;
        private bool _printing;
        private ModulePatient _modulePatient;

        private void DoNoahRequest(NotifyEventArgs eventArgs)
        {
            switch (eventArgs.Notification)
            {
                case NotificationType.LanguageChanged:
                    SetLanguage();
                    break;
                
                case NotificationType.ModuleDisconnect:
                    DisconnectModule();
                    break;
                
            }
        }
        private void NoahEventRequestHandler(object sender, EventArgs e)
        {
            var eventArgs = e as NotifyEventArgs;

            if (eventArgs == null)
                return;

            DoNoahRequest(eventArgs);
        }
        private bool Initialize(Form parentGui)
        {
            _parentGui = parentGui;

            if (_printing)
            {
                // As this is a test example we choose to connect as fitting
                _moduleApi.Connect(Resources.FittingModuleId, this, true);
                _moduleConnected = true;
                // Connect will throw an exception if connect fail. In that case it is illegal to call disconnect

                var launchInfo = _moduleApi.GetLaunchInfo();
                if (launchInfo.Print) //The user has started the module with an action to print
                {
                    MessageBox.Show("You need to print action (connecting to Noah as fitting)" + launchInfo.Action.Id + " now!");
                }
                else if (launchInfo.ActionGroup.HasValue) //The user has started the module with an action group to print
                {
                    var actionGroup = launchInfo.ActionGroup.Value;
                    MessageBox.Show("You need to print action group (connecting to Noah as fitting)" + actionGroup + " now!");
                }

                return false;
            }
            return true;
        }

        private static bool IsUnsavedData()
        {
            throw new WarningException("Not Implemented Yet");
        }

        private void SetLanguage()
        {
            var currentLang = _moduleApi.LanguageId;

            Enumerators.Language lng;

            switch (currentLang)
            {
                case 1033:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    lng = Enumerators.Language.English;
                    break;
                case 1055:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    lng = Enumerators.Language.Turkish;
                    break;
                case 1031:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
                    lng = Enumerators.Language.German;
                    break;
                case 1036:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
                    lng = Enumerators.Language.French;
                    break;
                case 1049:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                    lng = Enumerators.Language.Russian;
                    break;
                case 3082:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
                    lng = Enumerators.Language.Spanish;
                    break;
                case 1051:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("sk-SK");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("sk-SK");
                    lng = Enumerators.Language.Slovak;
                    break;
                case 2070:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-PT");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-PT");
                    lng = Enumerators.Language.Portugal;
                    break;
                default:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    lng = Enumerators.Language.English;
                    break;
            }
            
            /*Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Application.Current.MainWindow.Activate();
                        parentGui.SetLanguage(lng);
                    }); */
        }
        private static XmlDocument GetAudioGramXml(byte[] data)
        {
            var xml = new XmlDocument();
            xml.LoadXml(System.Text.Encoding.UTF8.GetString(data));

            return xml;
        }
        private bool IsCorrectModule()
        {
            return _launchInfo.ModuleId == Resources.ManufacturerModuleIdFitting;
        }

        private bool IsNoahAlive()
        {
            return _moduleApi.IsNoahAlive();
        }
        private byte[] GetAudioGramActionData() // Set Patient On Noah Before Call The Function, if its not handled then returns null
        {
            byte[] data = null;

            if (_moduleApi.CurrentPatient == null)
                return null;
            
            foreach (var currentSessionAction in _moduleApi.CurrentPatient.CurrentSession.Actions)
            {
                if (currentSessionAction.DataType.Code == Resources.AudioGram && 
                    currentSessionAction.DataType.Format == 502)
                    data = currentSessionAction.GetPublicData();
                
                break;
            }
            
            return data;
        }

        public NoahConnectionHandler()
        {
            _moduleApi.EventPublisher.Notify += NoahEventRequestHandler;
            _launchInfo = _moduleApi.GetLaunchInfo();
            _moduleConnected = false;
        }
        public static bool IsNoahInstalled()
        {
            var regKey = Environment.Is64BitOperatingSystem ? Registry.LocalMachine.OpenSubKey(Resources.Os64Bit)
                : Registry.LocalMachine.OpenSubKey(Resources.Os32Bit);
         
            var obj = regKey.GetValue("Installed");

            return obj != null && obj is int num && num != 0;
        }
        public bool CanSwitchPatient()
        {
            return false;
        }
        public bool AcceptToDisconnect()
        {
            if (IsUnsavedData())
                if (MessageBox.Show("You have unsaved data on your process, Do you want to continue?", "Warning",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DisconnectModule();
                    return true;
                }

            return false;
        }

        public void ConnectWithNoah()
        {
            var res = ConnectRes.ModuleNotRegistered;
            
            while (res != ConnectRes.Ok)
            {
                if (res == ConnectRes.ModuleNotRegistered)
                {
                    RegisterModule.HandleModuleRegistration("1.0.0");
                    res = _moduleApi.Connect(Resources.ManufacturerID, this, _launchInfo.Print, true);
                }

                if (res == ConnectRes.ModuleAlreadyRunning)
                    break;
            }

            _moduleConnected = true;
        }
        public bool CanModuleLaunch()
        {
            if (_moduleApi.CurrentPatient == null || !IsNoahAlive() || !IsCorrectModule() || !_moduleConnected)
                return false;

            return true;

        }
        public void DisconnectModule()
        {
            _moduleApi.Disconnect();
            _moduleConnected = false;
        }

        public ModulePatient GetNoahPatient()
        {
            return new ModulePatient().GetInfoFromNoah(_moduleApi);
        }
        public void AddFittingRecordAction(Himsa.Noah.Modules.Action action, short deviceType, EarType earType)
        {
            action.DeviceType = deviceType;

            action.DataType = earType.ToString() == Resources.LeftEar
                ? new DataType { Code = Resources.HIFitting_L, Format = 500 }
                : new DataType { Code = Resources.HIFitting_R, Format = 500 };
            
            _moduleApi.CurrentSession.Actions.Add(action);
        }
        public void AddAudioGramAction(Himsa.Noah.Modules.Action action, short deviceType, string desc)
        {
            action.DeviceType = deviceType;

            action.DataType = new DataType { Code = Resources.AudioGram, Format = 502 };
            action.Description = desc;
            action.ActionGroup = DateTime.Now;

            _moduleApi.CurrentSession.Actions.Add(action);
            
        }
        public void UnregisterModule()
        {
            if (_moduleConnected)
                RegisterModule.HandleModuleUnregistration();
        }
        public XmlDocument GetCurrentPatientAudioGramXml()
        {
            return GetAudioGramXml(GetAudioGramActionData());
        }
    }
}
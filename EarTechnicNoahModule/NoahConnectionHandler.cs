Ü﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
        private bool _printing;

        private XmlDocument ConvertDataToFormat500(Himsa.Noah.Modules.Action action)
        {
            var byteArray = action.GetPublicData();
            var xml = new XmlDocument();
            try
            {
                if (byteArray.Length == 19472 || byteArray.Length == 36128)
                {
                    Himsa.Noah.Modules.Action convertedAction = action.ConvertData(500);
                    var convertedXML = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
                        "ConvertedData.xml");
                    using (BinaryWriter binWriter = new BinaryWriter(File.Open(convertedXML, FileMode.Create)))
                    {
                        binWriter.Write(convertedAction.GetPublicData(), 0, convertedAction.GetPublicData().Length);
                    }
                    
                    xml.LoadXml(convertedXML);
                    
                }
                return xml;
            }
            catch (Exception ignore)
            {
                return null;
            }
        }
        private void DoNoahRequest(NotifyEventArgs eventArgs)
        // You can add more events from list for integrated usage of the developed module, 
        {
            switch (eventArgs.Notification)
            {
                case NotificationType.LanguageChanged:
                    SetLanguage();
                    break;
                
                case NotificationType.ModuleDisconnect:
                    DisconnectModule();
                    break;
                case NotificationType.PatientChanged:
                    CanSwitchPatient();
                    break;
                case NotificationType.MustDisconnect:
                    _moduleApi.Dispose();
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
        private bool Initialize() // you can call this function from the app for init the necessaries
        {
            if (_printing)
            {
                //you can set which moduleType will connect to NOAH with constants as first argument
                _moduleApi.Connect(Resources.FittingModuleId, this, true); // give the app context to callback argument
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
            // The app must ask the user for saving unsaved data when user attempts to close the app
        }

        private void SetLanguage() // An event must listen Noah's language change action, when its triggered call that function
        {
            var currentLang = _moduleApi.LanguageId;

            Enumerators.Language lng;

            switch (currentLang)
            {
                case 1033:
Ü                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
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
            // Set the application context in here
            /*Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Application.Current.MainWindow.Activate();
                        mainForm.SetLanguage(lng);
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
        private XmlDocument GetFittingActionData(EarType earType) // Set Patient On Noah Before Call The Function, if its not handled then returns null
        {
            var xml = new XmlDocument();

            if (_moduleApi.CurrentPatient == null)
                return null;
            
            foreach (var currentSessionAction in _moduleApi.CurrentPatient.CurrentSession.Actions)
            {
                if (currentSessionAction.DataType.Code == (earType == EarType.Left ? Resources.HIFitting_L : Resources.HIFitting_R))
                    if (currentSessionAction.DataType.Format <= 500)
                        xml.LoadXml(System.Text.Encoding.UTF8.GetString(currentSessionAction.GetPublicData()));
                    else 
                        xml = ConvertDataToFormat500(currentSessionAction);
                break;
            }
            
            return xml;
        }
        public NoahConnectionHandler()
        {
            _moduleApi.EventPublisher.Notify += NoahEventRequestHandler;
            _launchInfo = _moduleApi.GetLaunchInfo();
            _moduleConnected = false;
initialize();
        }
        public static bool IsNoahInstalled() // Checks the pc for NOAH, use the function on install process
        {
            var regKey = Environment.Is64BitOperatingSystem ? Registry.LocalMachine.OpenSubKey(Resources.Os64Bit)
                : Registry.LocalMachine.OpenSubKey(Resources.Os32Bit);
         
            var obj = regKey.GetValue("Installed");

            return obj != null && obj is int num && num != 0;
        }
        
        public bool AcceptToDisconnect() //RELEVANT
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
        public void AddAudioGramAction(Himsa.Noah.Modules.Action action, short deviceType, string desc)
            // Saves a new AudioGram Record to NOAH Server in NOAH Format 502
        {
            action.DeviceType = deviceType;

            action.DataType = new DataType { Code = Resources.AudioGram, Format = 502 };
            action.Description = desc;
            action.ActionGroup = DateTime.Now;

            _moduleApi.CurrentSession.Actions.Add(action);
        }
        public void AddFittingRecordAction(Himsa.Noah.Modules.Action action, short deviceType, EarType earType)
            // Saves a new Fitting Record to NOAH Server in NOAH Format 500
        {
            action.DeviceType = deviceType;

            action.DataType = earType.ToString() == Resources.LeftEar
                ? new DataType { Code = Resources.HIFitting_L, Format = 500 }
                : new DataType { Code = Resources.HIFitting_R, Format = 500 };
            
            _moduleApi.CurrentSession.Actions.Add(action);
        }
        public bool CanModuleLaunch()
        {
            if (_moduleApi.CurrentPatient == null || !IsNoahAlive() || !IsCorrectModule() || !_moduleConnected)
                return false;

            return true;
        }
        public bool CanSwitchPatient() // you can implement this function that lets the Noah can switch current patient. RELEVANT
        {
            return false;
        }
        public void ConnectWithNoah() // If the app will install with NOAH, you have to call this function, handles the registration
        {
            var res = ConnectRes.ModuleNotRegistered;
            
            while (res != ConnectRes.Ok)
            {
                if (res == ConnectRes.ModuleNotRegistered)
                {
                    RegisterModule.HandleModuleRegistration("1.0.0");
                    //give the app context to callback argument
                    res = _moduleApi.Connect(Resources.ManufacturerID, this, _launchInfo.Print, true);
                }

                if (res == ConnectRes.ModuleAlreadyRunning)
                    break;
            }

            _moduleConnected = true;
        }
        public void DisconnectModule() // If the app started with Noah, this function must be called when Noah close event triggered
        {
            _moduleApi.Disconnect();
            _moduleConnected = false;
        }
        public XmlDocument GetCurrentPatientAudioGramXml()
        {
            return GetAudioGramXml(GetAudioGramActionData());
        }
        public XmlDocument GetCurrentPatientLeftEarFittingData()
        {
            return GetFittingActionData(EarType.Left);
        }
        public XmlDocument GetCurrentPatientRightEarFittingData()
        {
            return GetFittingActionData(EarType.Right);
        }
        public ModulePatient GetNoahCurrentPatient() // Returns the Noah Side current patient info
        {
            return new ModulePatient().GetInfoFromNoah(_moduleApi);
        }
        public ModuleUser GetNoahCurrentUser() // Returns the Noah Side current patient info
        {
            return new ModuleUser().GetInfoFromNoah(_moduleApi);
        }
        public void UnregisterModule() // Deletes the app configuration that handles being a NOAH module
        {
            if (_moduleConnected)
                RegisterModule.HandleModuleUnregistration();
        }
    }
}
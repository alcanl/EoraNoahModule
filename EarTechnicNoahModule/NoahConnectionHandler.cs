using System;
using System.ComponentModel;
using Himsa.Noah.Modules;
using System.Windows.Forms;
using System.Xml;
using EarTechnicNoahModule;
using EarTechnicNoahModule.Entity;
using EarTechnicNoahModule.Global;
using EarTechnicNoahModule.Registration;
using Microsoft.Win32;
using DataType = Himsa.Noah.Modules.DataType;

namespace EarTechnicNoahModuleTest
{
    public class NoahConnectionHandler : MarshalByRefObject, ICallbackHandler
    {
        private readonly ModuleAPI _moduleApi = new ModuleAPI();
        private readonly LaunchInfo _launchInfo;
        private bool _moduleConnected;

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
        private static bool IsUnsavedData()
        {
            throw new WarningException("Not Implemented Yet");
        }

        private void SetLanguage()
        {
            var currentLang = _moduleApi.LanguageId;
            
            //
        }
        private static XmlDocument GetAudioGramXml(byte[] data)
        {
            var xml = new XmlDocument();
            xml.LoadXml(System.Text.Encoding.UTF8.GetString(data));

            return xml;
        }
        private bool IsCorrectModule()
        {
            return _launchInfo.ModuleId == Resources.ManufacturerModuleId;
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
                    RegisterModule.handleModuleRegistration("1.0.0");
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
using System;
using System.ComponentModel;
using Himsa.Noah.Modules;
using System.Windows.Forms;
using EarTechnicNoahModule.Entity;
using EarTechnicNoahModule.Global;
using Microsoft.Win32;

namespace EarTechnicNoahModule
{
    public class NoahConnectionHandler : MarshalByRefObject, ICallbackHandler
    {
        private readonly ModuleAPI _moduleApi = new ModuleAPI();
        public static bool IsNoahInstalled()
        {
            var regKey = Environment.Is64BitOperatingSystem ? Registry.LocalMachine.OpenSubKey(Resources.Os64Bit)
                : Registry.LocalMachine.OpenSubKey(Resources.Os32Bit);
         
            var obj = regKey.GetValue("Installed");

            return obj != null && obj is int num && num != 0;
        }
        
        public bool CanSwitchPatient()
        {
            return AcceptToDisconnect();
        }
        public bool AcceptToDisconnect()
        {
            if (!CheckForSavedData())
                return (MessageBox.Show("You have unsaved Data, Do you want to continue?", "Warning",
                    MessageBoxButtons.YesNo) == DialogResult.Yes);
            
            return true;
        }
        private bool CheckForSavedData()
        {
            return false;
        }

        private void DisableUserInputs()
        {
            throw new WarningException("Not Implemented Yet");
        }
        private void EnableUserInputs()
        {
            throw new WarningException("Not Implemented Yet");
        }
        private void UpdatePatient()
        {
            throw new WarningException("Not Implemented Yet");
        }

        private void PatientSwitchCheck()
        {
            if (CanSwitchPatient())
            {
                DisableUserInputs();
                UpdatePatient();
                EnableUserInputs();
            }
        }
        
        public bool LaunchModule()
        {
            try {
                _moduleApi.Connect(Resources.ManufacturerID, this, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public void CheckForNoah()
        {
            throw new WarningException("Not Implemented Yet");
        }

        public bool IsCorrectModule()
        {
            return _moduleApi.GetLaunchInfo().ModuleId == Resources.ManufacturerModuleId;
        }

        public bool IsNoahAlive()
        {
            return _moduleApi.IsNoahAlive();
        }

        public void DısconnectModule()
        {
            _moduleApi.Disconnect();
        }

        public ModulePatient GetNoahPatient()
        {
            return new ModulePatient().GetInfoFromNoah(_moduleApi);
        }

        public void AddFittingRecordAction(Himsa.Noah.Modules.Action action, short deviceType, EarType earType)
        {
            action.DeviceType = deviceType;

            action.DataType = earType.ToString() == Resources.LeftEar
                ? new DataType { Code = Resources.HIFitting_L, Format = 100 }
                : new DataType { Code = Resources.HIFitting_R, Format = 100 };
            
            _moduleApi.CurrentSession.Actions.Add(action);
        }

        public void AddAudioGramAction(Himsa.Noah.Modules.Action action, short deviceType)
        {
            action.DeviceType = deviceType;

            action.DataType = new DataType { Code = Resources.Audiogram, Format = 100 };

            _moduleApi.CurrentSession.Actions.Add(action);
        }
        
    }
}
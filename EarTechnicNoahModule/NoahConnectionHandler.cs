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
        private readonly ModuleAPI _moduleApı = new ModuleAPI();

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
            return true;
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

        private void initializeModule()
        {
            _moduleApı.Connect(Resources.ManufacturerModuleId, this);
        }
        
        private void setNoahConnectedUser(ModuleUser moduleUser)
        {

        }

        public bool LaunchModule()
        {
            try {
                _moduleApı.Connect(Resources.ManufacturerID, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
        
    }
}
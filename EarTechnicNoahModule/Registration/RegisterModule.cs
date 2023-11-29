using System;
using System.Collections.Generic;
using System.IO;
using EarTechnicNoahModule.Global;
using Himsa.Noah.Modules;

namespace EarTechnicNoahModuleTest.Registration
{
    public static class RegisterModule
    {
        public static void HandleModuleRegistration(string version)
        {
            var str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            
            using (var regData = new RegistrationData()
                   {
                       ModuleName = Resources.ModuleName,
                       ModuleCategory = 0,
                       ExePath = str,
                       ManufacturerId = Resources.ManufacturerID,
                       ManufacturerModuleId = 4,
                       ButtonDLLPath = str,
                       HelpPath = "",
                       Show = true,
                       IMCServer = "",
                       UninstallCmd = "",
                       Version = version,
                       ActionMake = new List<Himsa.Noah.Modules.Registration.DataType>
                       {
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.AudioGram, DataFmtStd = 502},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_L, DataFmtStd = 500},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_R, DataFmtStd = 500},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.REM_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.REM_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HISelection_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HISelection_R, DataFmtStd = 100}
                       },
                       ActionShow = new List<Himsa.Noah.Modules.Registration.DataType>
                       {
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.AudioGram, DataFmtStd = 502},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_L, DataFmtStd = 500},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_R, DataFmtStd = 500},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.REM_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.REM_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HISelection_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HISelection_R, DataFmtStd = 100}
                       },
                       Protocols = null,
                       ModuleAlias = new List<Himsa.Noah.Modules.Registration.ModuleAlias>
                       {
                           new Himsa.Noah.Modules.Registration.ModuleAlias {ManufacturerId = Resources.ManufacturerID, ModuleId = 4}
                       }

                   }) SetRegistration(regData);
        }

        public static void HandleModuleUnregistration()
        {
            using (Himsa.Noah.Modules.Registration reg = new Himsa.Noah.Modules.Registration())
                reg.UnRegisterModule(Resources.ManufacturerID, Resources.ManufacturerModuleId);
        }

        private static void SetRegistration(RegistrationData registrationData)
        {
            using (var registration = new Himsa.Noah.Modules.Registration())
            {
                registration.RegisterModule(registrationData);
            }
        }
    }
}
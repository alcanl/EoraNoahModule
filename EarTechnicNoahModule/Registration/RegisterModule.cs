using System;
using System.Collections.Generic;
using System.IO;
using EarTechnicNoahModule.Global;
using Himsa.Noah.Modules;

namespace EarTechnicNoahModule.Registration
{
    public static class RegisterModule
    {
        public static void handleModuleRegistration(string version)
        {
            var str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            
            using (RegistrationData regdata = new RegistrationData()
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
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.Audiogram, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.REM_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.REM_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HISelection_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HISelection_R, DataFmtStd = 100}
                       },
                       ActionShow = new List<Himsa.Noah.Modules.Registration.DataType>
                       {
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.Audiogram, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HITest_R, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_L, DataFmtStd = 100},
                           new Himsa.Noah.Modules.Registration.DataType {DataTypeCode = Resources.HIFitting_R, DataFmtStd = 100},
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

                   }) setRegistration(regdata);
        }

        public static void HandleModuleUnregistration()
        {
            using (Himsa.Noah.Modules.Registration reg = new Himsa.Noah.Modules.Registration())
                reg.UnRegisterModule(Resources.ManufacturerID, Resources.ManufacturerModuleId);
        }

        private static void setRegistration(RegistrationData registrationData)
        {
            using (Himsa.Noah.Modules.Registration registration = new Himsa.Noah.Modules.Registration())
            {
                registration.RegisterModule(registrationData);
            }
        }
    }
}
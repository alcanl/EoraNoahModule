using EarTechnicNoahModule.Global;
using Himsa.Noah.Modules;

namespace EarTechnicNoahModule.Registration
{
    public class RegisterModule
    {
        public void handleModuleRegistration(string version)
        {
            using (RegistrationData regdata = new RegistrationData()
                   {
                       ModuleName = Resources.ModuleName,
                       ModuleCategory = 0,
                       ExePath = Resources.ModuleExePath,
                       ManufacturerId = Resources.ManufacturerID,
                       ManufacturerModuleId = 4,
                       ButtonDLLPath = Resources.ModuleLogoDllPath,
                       HelpPath = "",
                       Show = true,
                       IMCServer = "",
                       UninstallCmd = "",
                       Version = version

                   }) setRegistration(regdata);
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
using System;
using Himsa.Noah.Modules;

namespace EarTechnicNoahModule.Entity
{
    public class ModuleUser
    {
        private Guid _userGuid;
        private string _name;

        public ModuleUser GetInfoFromNoah(ModuleAPI _moduleApı)
        {
            _name = _moduleApı.CurrentUser.Name;
            _userGuid = _moduleApı.CurrentUser.UserGUID;
            return this;
        }
    }
}
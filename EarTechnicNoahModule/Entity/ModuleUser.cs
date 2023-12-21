using System;
using Himsa.Noah.Modules;

namespace EarTechnicNoahModule.Entity
{
    public class ModuleUser // If the app gonna hold the user info then you can use this class for getting noah user info
    {
        private Guid _userGuid;
        private string _name;

        public ModuleUser GetInfoFromNoah(ModuleAPI moduleApı)
        {
            _name = moduleApı.CurrentUser.Name;
            _userGuid = moduleApı.CurrentUser.UserGUID;
            return this;
        }

        public void SetUserGuid(Guid guid)
        {
            _userGuid = guid;
        }

        public Guid GetUserGuid()
        {
            return _userGuid;
        }
        public void SetUserName(string name)
        {
            _name = name;
        }

        public string GetUserName()
        {
            return _name;
        }
    }
}
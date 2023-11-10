using System;
using Himsa.Noah.Modules;

namespace EarTechnicNoahModule.Entity
{
    public class ModulePatient
    {
        private string _firstName;
        private string _lastName;
        private DateTime _birthDate;
        private string _gender;
        private string _homePhoneNumber;
        private string _workPhoneNumber;
        private string _mobilePhoneNumber;
        private string _address;
        private string _city;
        private string _eMailAddress;
        private string _idNumber;

        public ModulePatient GetInfoFromNoah(ModuleAPI _moduleApı)
        {
            _idNumber = _moduleApı.CurrentPatient.Id.ToString();
            _firstName = _moduleApı.CurrentPatient.FirstName;
            _lastName = _moduleApı.CurrentPatient.LastName;
            _address = _moduleApı.CurrentPatient.Address1;
            _birthDate = _moduleApı.CurrentPatient.BirthDate;
            _gender = _moduleApı.CurrentPatient.Gender.ToString();
            _eMailAddress = _moduleApı.CurrentPatient.Email;
            _workPhoneNumber = _moduleApı.CurrentPatient.WorkTelephone;
            _homePhoneNumber = _moduleApı.CurrentPatient.HomeTelephone;
            _mobilePhoneNumber = _moduleApı.CurrentPatient.MobileTelephone;
            _city = _moduleApı.CurrentPatient.City;

            return this;
        }
    }
}
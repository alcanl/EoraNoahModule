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
            _mobilePhoneNumber = _moduleApı.CurrentPatient.MobileTelephone;
            _city = _moduleApı.CurrentPatient.City;

            return this;
        }

        public void SetFirstName(string firstName)
        {
            _firstName = firstName;
        }

        public string GetFirstName()
        {
            return _firstName;
        }
        public void SetLastName(string lastName)
        {
            _lastName = lastName;
        }

        public string GetLastName()
        {
            return _lastName;
        }
        public void SetBirthDate(DateTime birthDate)
        {
            _birthDate = birthDate;
        }

        public DateTime GetBirthDate()
        {
            return _birthDate;
        }
        public void SetGender(string gender)
        {
            _gender = gender;
        }

        public string GetGender()
        {
            return _gender;
        }
        public void SetPhoneNumber(string phoneNumber)
        {
            _mobilePhoneNumber = phoneNumber;
        }

        public string GetPhoneNumber()
        {
            return _mobilePhoneNumber;
        }
        public void SetAddress(string address)
        {
            _address = address;
        }

        public string GetAddress()
        {
            return _address;
        }
        public void SetEMail(string eMail)
        {
            _eMailAddress = eMail;
        }

        public string GetEMail()
        {
            return _eMailAddress;
        }
        public void SetIdNumber(string idNumber)
        {
            _idNumber = idNumber;
        }

        public string GetIdNumber()
        {
            return _idNumber;
        }
        public void SetCity(string city)
        {
            _city = city;
        }

        public string GetCity()
        {
            return _city;
        }

        public string GetFullName()
        {
            return _firstName + _lastName;
        }

        public int GetAge()
        {
            return DateTime.Now.Year - _birthDate.Year;
        }
    }
}
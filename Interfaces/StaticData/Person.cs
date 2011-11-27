using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.StaticData
{
    public enum Gender
    {
        Unknown = 0,
        Male,
        Female
    }

    public class Person
    {
        public Person() { }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }        

        public Gender Gender
        {
            get 
            {
                if (genderChar != null && genderChar.Length > 0)
                {
                    switch (genderChar)
                    {
                        case "M":
                            return Gender.Male;
                        case "V":
                            return Gender.Female;
                        default:
                            return Gender.Unknown;
                    }
                }
                else
                    return gender; 
            }
            set 
            { 
                gender = value;
                switch (value)
                {
                    case Gender.Male:
                        genderChar = "M";
                        break;
                    case Gender.Female:
                        genderChar = "V";
                        break;
                    default:
                        genderChar = null;
                        break;
                }
            }
        }       

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }        

        public string Initials
        {
            get { return initials; }
            set { initials = value; }
        }
        
        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }        

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }        

        public Address ResidentialAddress
        {
            get { return residentialAddress; }
            set { residentialAddress = value; }
        }       

        public Address PostalAddress
        {
            get { return postalAddress; }
            set { postalAddress = value; }
        }        

        public TelephoneNumber Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }        

        public TelephoneNumber TelephoneAH
        {
            get { return telephoneAH; }
            set { telephoneAH = value; }
        }        

        public TelephoneNumber Mobile
        {
            get { return mobile; }
            set { mobile = value; }
        }        

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string FullName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                if (!string.IsNullOrEmpty(Title))
                    sb.Append(" " + Title);
                if (!string.IsNullOrEmpty(FirstName))
                    sb.Append(" " + FirstName);
                else if (!string.IsNullOrEmpty(Initials))
                    sb.Append(" " + Initials.ToUpper());
                if (!string.IsNullOrEmpty(MiddleName))
                    sb.Append(" " + MiddleName);
                if (!string.IsNullOrEmpty(LastName))
                    sb.Append(" " + LastName);
                
                return sb.ToString().Trim();
            }
        }

        public string FullNameLastNameFirst
        {
            get
            {
                string lastName = !string.IsNullOrEmpty(LastName) ? LastName : "";
                string firstAndMiddleName = FirstAndMiddleName;
                string separator = lastName != "" && firstAndMiddleName != "" ? ", " : "";

                return (lastName + separator + firstAndMiddleName).Trim();
            }
        }

        public string FirstAndMiddleName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                if (!string.IsNullOrEmpty(Initials))
                    sb.Append(" " + Initials.ToUpper());
                else if (!string.IsNullOrEmpty(FirstName))
                    sb.Append(" " + FirstName);
                if (!string.IsNullOrEmpty(MiddleName))
                    sb.Append(" " + MiddleName);
                
                return sb.ToString().Trim();
            }
        }

        #region Overrides

        /// <summary>
        /// Overriding composition of name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullName;
        }

        public override int GetHashCode()
        {
            string details = FullName;

            if (ResidentialAddress != null)
                details += ResidentialAddress.AddressLine1;
            else
            {
                if (PostalAddress != null)
                    details += PostalAddress.AddressLine1;
            }
            return details.GetHashCode();
        }

        #endregion

        #region Private Variables

        private string title;
        private Gender gender = Gender.Unknown;
        private string genderChar;
        private string firstName;
        private string initials;
        private string middleName;
        private string lastName;
        private Address residentialAddress;
        private Address postalAddress;
        private TelephoneNumber telephone = new TelephoneNumber();
        private TelephoneNumber telephoneAH = new TelephoneNumber();
        private TelephoneNumber mobile = new TelephoneNumber();
        private string email;

        #endregion

    }
}

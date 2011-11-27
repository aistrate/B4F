using System;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Stichting.Remisier
{
    public class RemisierEmployee : IRemisierEmployee
    {
        protected RemisierEmployee() { }

        public RemisierEmployee(IRemisier remisier, Person person) 
        {
            if (remisier == null)
                throw new ApplicationException("Remisier is mandatory");
            if (person == null || string.IsNullOrEmpty(person.LastName))
                throw new ApplicationException("Person / last name is mandatory");
            this.Remisier = remisier;
            this.Employee = person;
        }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }        

        public Person Employee
        {
            get { return employee; }
            set { employee = value; }
        }        

        public IRemisier Remisier
        {
            get { return remisier; }
            set { remisier = value; }
        }

        public EmployeeRoles Role
        {
            get
            {
                EmployeeRoles role = EmployeeRoles.Unknown;
                if (!string.IsNullOrEmpty(roleDescription))
                {
                    switch (roleDescription)
                    {
                        case "Beheerder":
                            role = EmployeeRoles.Administrator;
                            break;
                        case "Medewerker":
                            role = EmployeeRoles.Employee;
                            break;
                    }
                }
                return role;
            }
            set
            {
                EmployeeRoles role = value;
                switch (role)
                {
                    case EmployeeRoles.Administrator:
                        roleDescription = "Beheerder";
                        break;
                    case EmployeeRoles.Employee:
                        roleDescription = "Medewerker";
                        break;
                    default:
                        roleDescription = null;
                        break;
                }
            }
        }

        public virtual IRemisierEmployeeLogin Login { get; set; }

        public RemisierEmployeeLoginPerson LoginPerson
        {
            get
            {
                if (loginPerson == null)
                    loginPerson = new RemisierEmployeeLoginPerson(this);
                return loginPerson;
            }
        }
        private RemisierEmployeeLoginPerson loginPerson;

        public bool IsLocalAdministrator
        {
            get { return (Login != null && Login.IsLocalAdministrator); }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; deleted = !value; }
        }

        public bool IsDefault { get; set; }

        #region Private Variables

        private int key;
        private Person employee;
        private IRemisier remisier;
        private bool isActive;
        private bool deleted;
        private string roleDescription;

        #endregion
    }
}

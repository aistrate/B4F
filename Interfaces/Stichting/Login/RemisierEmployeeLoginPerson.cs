using System;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.Stichting.Login
{
    public class RemisierEmployeeLoginPerson : LoginPerson
    {
        public RemisierEmployeeLoginPerson(IRemisierEmployee remisierEmployee)
        {
            RemisierEmployee = remisierEmployee;
        }

        public IRemisierEmployee RemisierEmployee { get; private set; }

        public IRemisier Remisier
        {
            get
            {
                if (RemisierEmployee.Remisier != null)
                    return RemisierEmployee.Remisier;
                else
                    throw new ApplicationException("Remisier Employee does not have an associated remisier company.");
            }
        }

        public override IExternalLogin Login
        {
            get
            {
                if (HasLogin)
                    return RemisierEmployee.Login;
                else
                    throw new ApplicationException("Remisier Employee does not have an associated login.");
            }
            set
            {
                IRemisierEmployeeLogin remisierEmployeeLogin = (IRemisierEmployeeLogin)value;
                remisierEmployeeLogin.RemisierEmployee = RemisierEmployee;
                RemisierEmployee.Login = remisierEmployeeLogin;
            }
        }

        public override int PersonKey
        {
            get { return RemisierEmployee.Key; }
        }

        public override string PersonType
        {
            get { return "Remisier Employee"; }
        }

        public override string PdfSubfolder
        {
            get { return "Remisiers"; }
        }

        public override bool IsActive
        {
            get { return RemisierEmployee.IsActive; }
        }

        public override bool HasLogin
        {
            get { return RemisierEmployee.Login != null && !string.IsNullOrEmpty(RemisierEmployee.Login.UserName); }
        }

        public override string Email
        {
            get { return RemisierEmployee.Employee.Email; }
        }

        public override string ShortName
        {
            get { return RemisierEmployee.Employee.LastName; }
        }

        public override string FullName
        {
            get
            {
                //if (RemisierEmployee.Role == EmployeeRoles.Administrator)
                //    return RemisierEmployee.Employee.LastName;
                //else
                return ContactsFormatter.GetFullPoliteForm(RemisierEmployee.Employee.Gender,
                                                           FirstName,
                                                           RemisierEmployee.Employee.MiddleName,
                                                           RemisierEmployee.Employee.LastName);
            }
        }

        public override string DearSirForm
        {
            get
            {
                return ContactsFormatter.GetDearSirForm(RemisierEmployee.Employee.Gender, 
                                                        RemisierEmployee.Employee.MiddleName,
                                                        RemisierEmployee.Employee.LastName);
            }
        }

        public override string AddressFirstLine
        {
            get { return Remisier.Name; }
        }

        public override string AddressSecondLine
        {
            get
            {
                return ContactsFormatter.GetAttentionOfForm(RemisierEmployee.Employee.Gender,
                                                            FirstName,
                                                            RemisierEmployee.Employee.MiddleName,
                                                            RemisierEmployee.Employee.LastName);
            }
        }

        public string FirstName
        {
            get { return string.Format("{0} {1}", RemisierEmployee.Employee.FirstName, RemisierEmployee.Employee.Initials).Trim(); }
        }

        public override Address Address
        {
            get { return Remisier.PostalAddress != null ? Remisier.PostalAddress : Remisier.OfficeAddress; }
        }

        public override string FullAddress
        {
            get { return ContactsFormatter.GetFullAddress(AddressFirstLine, AddressSecondLine, Address); }
        }

        public override IAssetManager AssetManager
        {
            get { return Remisier.AssetManager; }
        }

        public override void AssertAddressIsComplete()
        {
            if (Address == null)
                throw new ApplicationException(
                                string.Format("Neither postal nor office address could be found for remisier '{0}'.", Remisier.Name));

            Address.AssertIsComplete();
        }

        public override string[] InitialClientUserRoleList
        {
            get
            {
                // TODO: this should come from the database
                return new string[] { "Remisier Employee: Basic" };
            }
        }
    }
}

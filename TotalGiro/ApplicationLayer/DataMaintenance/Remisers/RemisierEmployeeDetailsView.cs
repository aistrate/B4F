using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.ApplicationLayer.Remisers
{
    public class RemisierEmployeeDetailsView
    {
        public RemisierEmployeeDetailsView()
        {
            this.EmployeeID = int.MinValue;
            this.RemisierID = int.MinValue;
            this.Role = EmployeeRoles.Unknown;
        }

        public RemisierEmployeeDetailsView(IRemisierEmployee employee)
            : this()
        {
            this.EmployeeID = employee.Key;
            this.Title = employee.Employee.Title;
            this.Gender = employee.Employee.Gender;
            this.Initials = employee.Employee.Initials;
            this.MiddleName = employee.Employee.MiddleName;
            this.LastName = employee.Employee.LastName;
            if (employee.Employee.Telephone != null)
                this.Telephone = employee.Employee.Telephone.Number;
            if (employee.Employee.TelephoneAH != null)
                this.TelephoneAH = employee.Employee.TelephoneAH.Number;
            if (employee.Employee.Mobile != null)
                this.Mobile = employee.Employee.Mobile.Number;
            this.Email = employee.Employee.Email;
            this.Role = employee.Role;
        }

        public int EmployeeID { get; set; }
        public int RemisierID { get; set; }
        public string Title { get; set; }
        public Gender Gender { get; set; }
        public string Initials { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string TelephoneAH { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public EmployeeRoles Role { get; set; }
    }
}

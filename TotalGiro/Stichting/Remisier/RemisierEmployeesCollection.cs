using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting.Login;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Stichting.Remisier
{
    public class RemisierEmployeesCollection : TransientDomainCollection<IRemisierEmployee>, IRemisierEmployeesCollection
    {
        public RemisierEmployeesCollection()
            : base() { }

        public RemisierEmployeesCollection(IRemisier parentRemisier)
            : base()
        {
            this.ParentRemisier = parentRemisier;
        }

        public IRemisier ParentRemisier
        {
            get { return parentRemisier; }
            internal set { parentRemisier = value; }
        }

        #region Private Variables

        private IRemisier parentRemisier;

        #endregion
    }
}

using System;
using System.Collections.Generic;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
namespace B4F.TotalGiro.Stichting
{
    public interface IRemisierEmployeesCollection : IList<IRemisierEmployee>
    {
        IRemisier ParentRemisier { get; }
    }
}

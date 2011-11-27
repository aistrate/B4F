using System;
using B4F.TotalGiro.StaticData;
using System.Collections.Generic;

namespace B4F.TotalGiro.Stichting.Remisier
{
    public enum RemisierTypes
    {
        Remisier,
        Inkooporganisatie,
        InternalVermogensbeheer,
        InternalBeheer
    }
    
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.Remisier">Remisier</see> class
    /// </summary>
    public interface IRemisier
    {
        int Key { get; set; }
        string Name { get; set; }
        RemisierTypes RemisierType { get; set; }
        bool IsActive { get; set; }
        bool IsInternal { get; }
        string DisplayNameAndRefNumber { get; }
        string DisplayName { get; }
        string InternalRef { get; set; }
        Address OfficeAddress { get; set; }
        Address PostalAddress { get; set; }
        Person ContactPerson { get; set; }
        TelephoneNumber Telephone { get; set; }
        TelephoneNumber Fax { get; set; }
        string Email { get; set; }

        IBankDetails BankDetails { get; set; }
        IAssetManager AssetManager { get; }
        //IAssetManagerCollection AssetManagers { get;}
        IRemisierEmployeesCollection Employees { get; }

        IRemisier ParentRemisier { get; set; }
        decimal ParentRemisierKickBackPercentage { get; set; }
        IRemisierCollection ChildRemisiers { get; }

        string ProvisieAfspraak { get; set; }
        string DatumOvereenkomst { get; set; }
        string NummerOvereenkomst { get; set; }
        string NummerAFM { get; set; }
        string NummerKasbank { get; set; }	 
    }
}

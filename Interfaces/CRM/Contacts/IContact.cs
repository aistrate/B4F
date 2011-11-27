using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.CRM
{
    public enum InternetEnabled
    {
        Unknown = -1,
        No = 0,
        Yes = 1
    }

    public enum ContactTypeEnum
    {
        Person = 0,
        Company = 1,
        Delegate = 2
    }

    public enum EnumStatusNAR
    {
        Incomplete = 0,
        Complete
    }

    public enum ResidentStatus
    {
        Resident = 1,
        NonResident
    }

    public interface IContact : ITotalGiroBase<IContact>, IAuditable
    {
        int Key { get; }
        IContactsIntroducer CurrentIntroducer { get; set; }
        IContactsNAW CurrentNAW { get; set; }
        string FullName { get; }
        string FullAddress { get; }
        ContactTypeEnum ContactType { get; }
        string GetBSN { get; }
        DateTime GetBirthFounding { get; }
        EnumStatusNAR StatusNAR { get; set; }
        IAssetManager AssetManager { get; set; }
        InternetEnabled InternetEnabled { get; set; }
        IContactDetails ContactDetails { get; set; }
        string Email { get; }
        IContactsNAWCollection ContactsNAWs { get; }
        IContactsIntroducersCollection ContactsIntroducers { get; }
        IContactAccountHoldersCollection AccountHolders { get; }
        List<ICustomerAccount> GetAccounts(bool activeOnly);
        List<ICustomerAccount> ActiveAccounts { get; }
        ICounterAccountCollection CounterAccounts { get; }
        DateTime CreationDate { get; set; }
        DateTime LastUpdated { get; }
        bool IsActive { get; set; }
        ICustomerLogin Login { get; set; }
        CustomerLoginPerson LoginPerson { get; }
        ResidentStatus ResidentialState { get; set; }
        IContactNotificationsCollection Notifications { get; }
        IContactSendingOptionCollection ContactSendingOptions { get; }
        bool SendByPost { get; }
        bool SendByEmail { get; }
        bool SendNewsItem { get; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts;
using System.Text.RegularExpressions;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public enum AccountContactSelectedTypes
    {
        Contact,
        Account
    }

    [Serializable]
    public class AccountContactSelectedDetails
    {
        public AccountContactSelectedDetails(int key, AccountContactSelectedTypes selectedType, string description)
        {
            this.EntityKey = key;
            this.SelectedType = selectedType;
            this.Description = description;
            Key = (SelectedType == AccountContactSelectedTypes.Contact ? "C" : "A") + EntityKey.ToString();
        }

        public string Key { get; set; }
        public int EntityKey { get; set; }
        public AccountContactSelectedTypes SelectedType { get; set; }
        public string Description { get; set; }
    }    
    
    public static class AccountsContactsSelectorAdapter
    {
        public static DataSet GetContacts(string filter, int[] excludedKeys)
        {
            if (string.IsNullOrEmpty(filter))
                return null;

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();
                Hashtable parameterLists = new Hashtable();

                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (!company.IsStichting)
                    parameters.Add("assetManagerId", company.Key);

                if (!string.IsNullOrEmpty(filter))
                {
                    if (Util.IsNumeric(filter) || (filter.Length > 2 && Util.IsNumeric(filter.Substring(2))))
                        parameters.Add("number", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                    else
                        parameters.Add("name", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                }
                if (excludedKeys != null && excludedKeys.Length > 0)
                    parameterLists.Add("excludedKeys", excludedKeys);

                List<IContact> contacts = session.GetTypedListByNamedQuery<IContact>(
                    "B4F.TotalGiro.ApplicationLayer.UC.ContactsToSelect", "",
                    parameters, parameterLists, 100, null);

                return contacts
                    .Select(c => new
                    {
                        c.Key,
                        Name = c.FullName,
                        BSN = c.GetBSN
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetAccounts(string filter, int[] excludedKeys)
        {
            if (string.IsNullOrEmpty(filter))
                return null;

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();
                Hashtable parameterLists = new Hashtable();

                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (!company.IsStichting)
                    parameters.Add("assetManagerId", company.Key);

                if (!string.IsNullOrEmpty(filter))
                {
                    // filter contains a number
                    if (Regex.IsMatch(filter, @"[0-9]+$"))
                        parameters.Add("number", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                    else
                        parameters.Add("name", Util.PrepareNamedParameterWithWildcard(filter, MatchModes.Anywhere));
                }
                if (excludedKeys != null && excludedKeys.Length > 0)
                    parameterLists.Add("excludedKeys", excludedKeys);

                List<IAccountTypeCustomer> contacts = session.GetTypedListByNamedQuery<IAccountTypeCustomer>(
                    "B4F.TotalGiro.ApplicationLayer.UC.AccountsToSelect", "",
                    parameters, parameterLists, 100, null);

                return contacts
                    .Select(c => new
                    {
                        c.Key,
                        c.Number,
                        c.ShortName
                    })
                    .ToDataSet();
            }
        }
    }
}

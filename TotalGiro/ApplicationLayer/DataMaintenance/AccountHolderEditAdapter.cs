using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class AccountHolderEditAdapter
    {
        public static void AddAccountHolder(int newContactID, int accountNrID)
        {
            AccountEditAdapter.AddAccountHolder(newContactID, accountNrID);
        }

        public static void DetachAccountHolder(int accountKey, int contactKey)
        {
            AccountEditAdapter.DetachAccountHolder(accountKey, contactKey);
        }

        public static DataSet GetAccountAccountHolders(int accountNrID)
        {
            return AccountEditAdapter.GetAccountAccountHolders(accountNrID);
        }

        public static string GetContactName(int contactID)
        {
            return AccountEditAdapter.GetContactName(contactID);
        }

        
    }
}

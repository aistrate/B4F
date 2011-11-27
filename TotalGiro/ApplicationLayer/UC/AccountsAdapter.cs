using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using System.Collections;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class AccountsAdapter
    {
        public static DataSet GetAccountHolders(int accountID)
        {
            IAccountHolder[] listAH = null;
            IDalSession session = NHSessionFactory.CreateSession();

            IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountID);
            if (account != null)
            {
                //IRemisierCollection remisierColl = assetManager.Remisiers;
                //    IRemisier[] remList = new IRemisier[remisierColl.Count];
                //    remisierColl.CopyTo(remList, 0);

                IAccountHolderCollection collAH = (IAccountHolderCollection)account.AccountHolders;
                listAH = new IAccountHolder[collAH.Count];
                collAH.CopyTo(listAH, 0);

            }
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(listAH,
                "Contact.ContactsNAWs.Current.Name, Contact.Key");

            session.Close();
            return ds;
        }
    }
}

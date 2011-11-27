using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using System.Data.SqlClient;
using System.Data;

namespace B4F.TotalGiro.Accounts
{
    public class AccountFamilyMapper
    {


        public static IList<IAccountFamily> GetAccountFamilies(IDalSession session, IAssetManager assetManager)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (!assetManager.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", assetManager.Key));
            return session.GetTypedList<AccountFamily,IAccountFamily>(expressions);
        }

        public static IAccountFamily GetAccountFamily(IDalSession session, int accountFamilyID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", accountFamilyID));
            IList result = session.GetList(typeof(IAccountFamily), expressions);
            if ((result != null) && (result.Count > 0))
                return (IAccountFamily)result[0];
            else
                return null;
        }

        public static IAccountFamily GetAccountFamily(IDalSession session, string prefix)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("AccountPrefix", prefix));
            IList result = session.GetList(typeof(IAccountFamily), expressions);
            if ((result != null) && (result.Count > 0))
                return (IAccountFamily)result[0];
            else
                return null;
        }

        public static string GetNewAccountNumber(IDalSession session, IAccountFamily accountFamily)
        {
            string prefix = accountFamily.AccountPrefix;
            //string accountNumber = "";

            SqlConnection sa = (SqlConnection)session.Connection;

            SqlCommand comm = new SqlCommand("TG_CreateAccountNumber", sa);
            comm.CommandType = CommandType.StoredProcedure;

            SqlParameter cmdParamIn = new SqlParameter( "AccountPrefix", prefix);
            comm.Parameters.Add(cmdParamIn);

            SqlParameter cmdParamOut = new SqlParameter("AccountNumber", SqlDbType.VarChar, 30);
            cmdParamOut.Direction = ParameterDirection.Output;
            comm.Parameters.Add(cmdParamOut);

            SqlDataReader myReader = comm.ExecuteReader();
            return cmdParamOut.Value.ToString();
        }
    }
}

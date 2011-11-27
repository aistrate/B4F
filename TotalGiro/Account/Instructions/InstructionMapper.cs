using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate;
using NHibernate.Criterion;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// Enumeration that specifies which type of Instruction you want.
    /// </summary>
    public enum InstructionReturnClass
    {
        /// <summary>
        /// Rebalance
        /// </summary>
        Rebalance = 1,
        /// <summary>
        /// Buy Model
        /// </summary>
        BuyModel = 2,
        /// <summary>
        /// Cash Withdrawal
        /// </summary>
        CashWithdrawal = 3,
        /// <summary>
        /// TypeOfRebalance
        /// </summary>
        TypeOfRebalance = 666,
        /// <summary>
        /// All Types
        /// </summary>
        All = 999
    }

    /// <summary>
    /// This class is used to instantiate Instruction objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class InstructionMapper
    {
        #region Retrieval Methods

        /// <summary>
        /// This method retrieves an instruction instance by its unique identifier. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="id">The unique identifier of the instruction</param>
        /// <returns>An Instance of an account class</returns>
        public static IInstruction GetInstruction(IDalSession session, Int32 id)
		{
			return (IInstruction)session.GetObjectInstance(typeof(Instruction), id);
		}

        /// <overloads>
        /// This method has 5 overloads.
        /// </overloads>
        /// <summary>
        /// This method retrieves the list of instruction instances
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<IInstruction> GetInstructions(IDalSession session)
		{
			return session.GetTypedList<Instruction, IInstruction>();
		}

        /// <summary>
        /// This method retrieves the list of instruction instances
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="instructionIDs">An array of uniquely identifiers of the instructions</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<IInstruction> GetInstructions(IDalSession session, int[] instructionIDs)
		{
			List<ICriterion> expressions = new List<ICriterion>();
			expressions.Add(Expression.In("Key", instructionIDs));
			return session.GetTypedList<Instruction, IInstruction>(expressions);
		}

        /// <summary>
        /// This method retrieves the list of instruction instances
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="account">The account that owns the instructions</param>
        /// <param name="activeOnly">A flag indicating whether only active instructions should be returned</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<IInstruction> GetInstructions(IDalSession session, IAccount account, bool activeOnly)
        {
            return GetInstructions<IInstruction>(session, account, InstructionReturnClass.All, activeOnly);
        }

        /// <summary>
        /// This method retrieves the list of instruction instances
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="account">The account that owns the instructions</param>
        /// <param name="retClass">The type of class that should be returned</param>
        /// <param name="activeOnly">A flag indicating whether only active instructions should be returned</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<I> GetInstructions<I>(IDalSession session, IAccount account, InstructionReturnClass retClass, bool activeOnly)
            where I : IInstruction
		{
			ISession nhsession;
			IList list = null;

			try
			{
				nhsession = ((NHSession)session).Session;
                ICriteria crit = nhsession.CreateCriteria(getRealType(retClass));
                crit.Add(Expression.Eq("Account.Key", account.Key));
				if (activeOnly)
				{
					crit.Add(Expression.Eq("IsActive", true));
				}
				list = crit.List();
			}
			catch (Exception ex)
			{
				throw new ApplicationException(string.Format("Could not retrieve account instruction objects."), ex);
			}
			finally
			{
			}
			return NHSession.ToList<I>(list);
		}

        /// <summary>
        /// This method retrieves the list of instruction instances
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">The type of class that should be returned</param>
        /// <param name="activeOnly">A flag indicating whether only active instructions should be returned</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<I> GetInstructions<I>(IDalSession session, InstructionReturnClass retClass, bool activeOnly)
            where I : IInstruction
		{
            return GetInstructions<I>(session, retClass, activeOnly, DateTime.Now);
		}

        /// <summary>
        /// This method retrieves the list of instruction instances
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="retClass">The type of class that should be returned</param>
        /// <param name="activeOnly">A flag indicating whether only active instructions should be returned</param>
        /// <param name="date">The date that filters the execution dates of the instructions. The Execution date should be before the date passed in.</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<I> GetInstructions<I>(IDalSession session, InstructionReturnClass retClass, bool activeOnly, DateTime date)
            where I : IInstruction
        {
            return GetInstructions<I>(session, retClass, activeOnly, date, 0, 0, string.Empty, string.Empty, false);
		}

        public static IList<I> GetInstructions<I>(IDalSession session, InstructionReturnClass retClass, 
            bool activeOnly, DateTime date, int assetManagerId, int modelPortfolioId, 
            string accountNumber, string accountName, bool showClosedToday)
            where I : IInstruction
        {
            Hashtable parameters = new Hashtable();

            string hql = "";
            if (retClass != InstructionReturnClass.All)
                hql = string.Format("and I.Key in (select X.Key from {0} X)", getType(retClass));

            if (activeOnly)
            {
                parameters.Add("isActive", true);
                if (activeOnly && date != DateTime.MinValue && showClosedToday)
                    parameters.Add("executionDateShowClosedToday", date);
            }
            if (date != DateTime.MinValue)
                parameters.Add("executionDate", date);
            if (assetManagerId > 0)
                parameters.Add("assetManagerId", assetManagerId);
            if (modelPortfolioId > 0)
                parameters.Add("modelPortfolioId", modelPortfolioId);
            if (!string.IsNullOrEmpty(accountNumber))
                parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber, MatchModes.Anywhere));
            if (!string.IsNullOrEmpty(accountName))
                parameters.Add("accountName", Util.PrepareNamedParameterWithWildcard(accountName, MatchModes.Anywhere));

            return session.GetTypedListByNamedQuery<I>(
                "B4F.TotalGiro.Accounts.Instructions.GetInstructions",
                hql,
                parameters);
        }

        /// <summary>
        /// This method retrieves the list of Cash Withdrawal instruction instances that belong to a withdrawal rule
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="withdrawalRuleID">The rule</param>
        /// <returns>A list of instruction instances</returns>
        public static IList<ICashWithdrawalInstruction> GetWithdrawalInstructions(IDalSession session, int withdrawalRuleID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Rule.Key", withdrawalRuleID));
            return session.GetTypedList<CashWithdrawalInstruction, ICashWithdrawalInstruction>(expressions);
        }

        #endregion

        #region CRUD

        /// <summary>
        /// This method is used to insert or update an instruction
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">The instruction that is inserted or updated</param>
        /// <returns>Returns True when succesfull</returns>
        public static bool Update(IDalSession session, IInstruction obj)
        {
            return session.InsertOrUpdate(obj);
        }

        /// <summary>
        /// This method is used to insert or update a list of instructions
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="list">The instructions that need insertion or updating</param>
        /// <returns>Returns True when succesfull</returns>
        public static bool Update(IDalSession session, IList list)
        {
            return session.InsertOrUpdate(list);
        }

        #endregion

        #region Helpers

        private static string getType(InstructionReturnClass retClass)
        {
            string type;
            switch (retClass)
            {
                case InstructionReturnClass.Rebalance:
                    type = "RebalanceInstruction";
                    break;
                case InstructionReturnClass.BuyModel:
                    type = "BuyModelInstruction";
                    break;
                case InstructionReturnClass.CashWithdrawal:
                    type = "CashWithdrawalInstruction";
                    break;
                case InstructionReturnClass.TypeOfRebalance:
                    type = "InstructionTypeRebalance";
                    break;
                default:
                    type = "Instruction";
                    break;
            }
            return type;
        }

        private static Type getRealType(InstructionReturnClass retClass)
        {
            switch (retClass)
            {
                case InstructionReturnClass.Rebalance:
                    return typeof(RebalanceInstruction);
                case InstructionReturnClass.BuyModel:
                    return typeof(BuyModelInstruction);
                case InstructionReturnClass.CashWithdrawal:
                    return typeof(CashWithdrawalInstruction);
                case InstructionReturnClass.TypeOfRebalance:
                    return typeof(InstructionTypeRebalance);
                default:
                    return typeof(Instruction);
            }
        }

        #endregion
    }
}

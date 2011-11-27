using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class LifecycleMaintenanceAdapter
    {
        #region Lifecycles

        public static DataSet GetLifecycles(string lifecycleName, B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus activeStatus, bool isInsert)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();
                parameters.Add("managementCompanyID", ModelMaintenanceAdapter.getAssetManager().GetV(x => x.Key));
                if (!string.IsNullOrEmpty(lifecycleName))
                    parameters.Add("name", Util.PrepareNamedParameterWithWildcard(lifecycleName));
                if (activeStatus != B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.All)
                    parameters.Add("isActive", activeStatus == B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.Active);

                List<ILifecycle> list = session.GetTypedListByNamedQuery<ILifecycle>(
                    "B4F.TotalGiro.Instruments.Lifecycles",
                    parameters);

                if (isInsert)
                    list.Add(new Lifecycle("Lifecycle", ModelMaintenanceAdapter.getAssetManager()));

                return list.Select(c => new
                    {
                        c.Key,
                        c.Name,
                        c.CreatedBy,
                        c.CreationDate,
                        c.LastUpdated,
                        c.IsActive
                    })
                    .ToDataSet();
            }
        }

        public static void UpdateLifecycle(int original_Key, string Name, bool isActive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILifecycle lifeCycle = null;
                if (original_Key != 0)
                {
                    lifeCycle = LifecycleMapper.GetLifecycle(session, original_Key);

                    if (lifeCycle == null)
                        throw new ApplicationException(string.Format("Could not find lifecycle {0}", original_Key));
                    if (!lifeCycle.IsActive)
                        lifeCycle.IsActive = isActive;
                    lifeCycle.Name = Name;
                    lifeCycle.LastUpdated = DateTime.Now;
                }
                else
                    lifeCycle = new Lifecycle(Name, ModelMaintenanceAdapter.getAssetManager());
                LifecycleMapper.Update(session, lifeCycle);
            }
        }

        public static void DeleteLifecycle(int lifecycleId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILifecycle lifeCycle = LifecycleMapper.GetLifecycle(session, lifecycleId);
                if (lifeCycle == null)
                    throw new ApplicationException(string.Format("Could not find lifecycle {0}", lifecycleId));

                isLifecycleOnActiveCustomers(lifeCycle, session);

                IList<ILifecycleLine> lines = lifeCycle.Lines.ToList();
                for (int i = lines.Count; i > 0; i--)
                    lifeCycle.Lines.RemoveLine(lines[i - 1]);

                session.BeginTransaction();
                if (lines.Count > 0)
                    session.Delete(lines);
                session.Delete(lifeCycle);
                session.CommitTransaction();
            }
        }

        public static bool DeActivateLifecycle(int lifecycleId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILifecycle lifeCycle = LifecycleMapper.GetLifecycle(session, lifecycleId);
                if (lifeCycle == null)
                    throw new ApplicationException(string.Format("Could not find lifecycle {0}", lifecycleId));

                isLifecycleOnActiveCustomers(lifeCycle, session);
                lifeCycle.IsActive = false;
                lifeCycle.LastUpdated = DateTime.Now;

               return LifecycleMapper.Update(session, lifeCycle);
            }
        }

        #endregion

        #region LifecycleLines

        public static DataSet GetLifecycleLines(int lifecycleId, bool isInsert)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILifecycle lifecycle = LifecycleMapper.GetLifecycle(session, lifecycleId);
                if (isInsert)
                    lifecycle.Lines.AddEmptyLine();
                List<ILifecycleLine> list = lifecycle.Lines.ToList<ILifecycleLine>();


                return list.Select(c => new
                {
                    c.Key,
                    c.AgeFrom,
                    ModelID = 
                        c.Model != null ? c.Model.Key : int.MinValue,
                    ModelName = 
                        c.Model != null ? c.Model.ModelName : "",
                    c.CreatedBy,
                    c.CreationDate,
                    c.LastUpdated
                })
                .ToDataSet();
            }
        }

        public static DataSet GetModelPortfolios()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = ModelMapper.GetModels(session, true, ActivityReturnFilter.Active)
                    .Select(c => new
                    {
                        c.Key,
                        c.ModelName
                    })
                    .ToDataSet();                    

                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static void UpdateLifecycleLine(int original_Key, int lifecycleId, int ageFrom, int modelId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILifecycleLine line = null;
                ILifecycle lifeCycle = LifecycleMapper.GetLifecycle(session, lifecycleId);
                if (lifeCycle == null)
                    throw new ApplicationException(string.Format("Could not find lifecycle {0}", lifecycleId));
                
                if (original_Key != 0)
                {
                    line = lifeCycle.Lines.Where(x => x.Key == original_Key).FirstOrDefault();
                    if (line == null)
                        throw new ApplicationException(string.Format("Could not find lifecycle line {0}", original_Key));

                    line.AgeFrom = ageFrom;
                    line.Model = ModelMapper.GetModel(session, modelId);
                    line.LastUpdated = DateTime.Now;
                }
                else
                    lifeCycle.Lines.AddLine(ageFrom, ModelMapper.GetModel(session, modelId));
                LifecycleMapper.Update(session, lifeCycle);
            }
        }

        public static void DeleteLifecycleLine(int lifecycleId, int lifecycleLineId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILifecycle lifeCycle = LifecycleMapper.GetLifecycle(session, lifecycleId);
                if (lifeCycle == null)
                    throw new ApplicationException(string.Format("Could not find lifecycle {0}", lifecycleId));
                ILifecycleLine line = lifeCycle.Lines.Where(x => x.Key == lifecycleLineId).FirstOrDefault();
                if (line == null)
                    throw new ApplicationException(string.Format("Could not find lifecycle line {0}", lifecycleLineId));
                lifeCycle.Lines.RemoveLine(line);
                session.BeginTransaction();
                session.Delete(line);
                session.Update(lifeCycle);
                session.CommitTransaction();
            }
        }

        #endregion

        #region Methods

        public static void UpdateLifecycleModelToAge(BatchExecutionResults results)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                try
                {
                    IManagementCompany comp = LoginMapper.GetCurrentManagmentCompany(session);
                    IInternalEmployeeLogin employee = (IInternalEmployeeLogin)LoginMapper.GetCurrentLogin(session);

                    Hashtable parameters = new Hashtable();
                    if (!comp.IsStichting)
                    {
                        IAssetManager am = (IAssetManager)comp;
                        if (!am.SupportLifecycles)
                            throw new ApplicationException("This asset manager does not support lifecycles.");
                        else
                            parameters.Add("managementCompanyID", am.Key);
                    }

                    List<ILifecycle> cycles = session.GetTypedListByNamedQuery<ILifecycle>(
                        "B4F.TotalGiro.Instruments.ActiveLifecycles",
                        parameters);

                    List<ILifecycleLine> lines = cycles.SelectMany(x => x.Lines).ToList();

                    List<Object> accountWithLifecycleData = session.GetTypedListByNamedQuery<Object>(
                        "B4F.TotalGiro.Instruments.AccountWithLifecycleData",
                        parameters);

                    var acctData = from a in accountWithLifecycleData.Cast<object[]>()
                                   where (DateTime)(a[4] ?? a[5] ?? DateTime.MinValue) != DateTime.MinValue
                                   select new
                                   {
                                       AccountID = (int)a[0],
                                       ModelID = (int)(a[2] ?? 0),
                                       LifecycleID = (int)a[3],
                                       Age = Util.CalculateCurrentAge((DateTime)(a[4] ?? a[5]))
                                   };

                    var acctNoBirthDate = from a in accountWithLifecycleData.Cast<object[]>()
                                          where (DateTime)(a[4] ?? a[5] ?? DateTime.MinValue) == DateTime.MinValue
                                          select (string)a[1];

                    if (acctNoBirthDate.Count() > 0)
                        results.MarkWarning(string.Format("{0} accounts have a primary account holder without a proper birth date: {1}",
                            acctNoBirthDate.Count(),
                            acctNoBirthDate.Take(25).JoinStrings(",")));

                    var acctIds = from x in acctData
                                  join y in lines on x.LifecycleID equals y.Parent.Key
                                  where x.Age >= y.AgeFrom && x.Age < y.AgeTo && x.ModelID != y.Model.Key
                                  select new { x.AccountID, x.Age }; //, x.Age, x.ModelID, y.AgeFrom, y.AgeTo, y.Model.ModelName };

                    foreach (var acctId in acctIds)
                    {
                        ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, acctId.AccountID);
                        if (acc.IsDeparting)
                            results.MarkWarning(string.Format("Account {0} is departing so the model will not be updated", acc.DisplayNumberWithName));
                        else if (acc.IsUnderRebalance)
                            results.MarkWarning(string.Format("Account {0} is under rebalance so the model will not be updated", acc.DisplayNumberWithName));
                        else
                        {
                            ILifecycle lc = acc.Lifecycle;
                            IPortfolioModel model = null;
                            if (lc != null)
                            {
                                model = lc.GetRelevantModel(acctId.Age);
                                if (!acc.ModelPortfolio.Equals(model))
                                    acc.SetModelPortfolio(acc.Lifecycle, model, acc.IsExecOnlyCustomer, acc.EmployerRelationship, employee, DateTime.Now);
                                if (AccountMapper.Update(session, acc))
                                    results.MarkSuccess();
                            }
                            else
                                results.MarkError(
                                    new ApplicationException(string.Format("Account {0} did not have a lifecycle.", acc.DisplayNumberWithName)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    results.MarkError(
                        new ApplicationException("Error in UpdateLifecycleModelToAge.", ex));
                }
            }
        }

        private static void isLifecycleOnActiveCustomers(ILifecycle lifecycle, IDalSession session)
        {
            // check if lifecycle is on active accounts
            long activeAccounts = session.Session.GetNamedQuery(
                "B4F.TotalGiro.ApplicationLayer.DataMaintenance.LifecycleOnActiveCustomers")
                .SetParameter("lifecycleId", lifecycle.Key)
                .UniqueResult<long>();
            if (activeAccounts > 0)
                throw new ApplicationException(string.Format("It is not possible to inactivate/delete this lifecycle since {0} active accounts are attached to it.", activeAccounts));
        }

        #endregion

        #region Display Results

        public static string FormatErrorsForUpdateLifecycleModelToAge(BatchExecutionResults results)
        {
            const int MAX_ERRORS_DISPLAYED = 25;

            string message = "<br/>";

            if (results.SuccessCount == 0 && results.ErrorCount == 0)
                message += "No accounts needed to be updated.<br/><br/>";
            else
                message += string.Format("{0} accounts have been updated to a new model according with their age.<br/><br/><br/>", results.SuccessCount);

            if (results.WarningCount > 0)
            {
                string tooManyWarningsMessage = (results.WarningCount > MAX_ERRORS_DISPLAYED ?
                                                    string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                message += string.Format("{0} warnings occured while updating Lifecycle Models to the account's age{1}:<br/><br/><br/>",
                                         results.WarningCount, tooManyWarningsMessage);

                int warnings = 0;
                foreach (string wr in results.Warnings)
                {
                    if (++warnings > MAX_ERRORS_DISPLAYED)
                        break;
                    message += string.Format("{0}<br/>", wr);
                }
            }
            
            if (results.ErrorCount > 0)
            {
                string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                    string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                message += string.Format("{0} errors occured while updating Lifecycle Models to the account's age{1}:<br/><br/><br/>",
                                         results.ErrorCount, tooManyErrorsMessage);

                int errors = 0;
                foreach (Exception ex in results.Errors)
                {
                    if (++errors > MAX_ERRORS_DISPLAYED)
                        break;
                    message += Utility.GetCompleteExceptionMessage(ex) + "<br/>";
                }
            }

            return message;
        }

        #endregion
    
    
    }
}

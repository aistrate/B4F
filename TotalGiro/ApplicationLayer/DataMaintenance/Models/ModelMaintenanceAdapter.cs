using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using System.Collections;
using System.Globalization;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Fees.FeeCalculations;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    #region Helper Classes

    public class ModelVersionHelper
    {
        public ModelVersionHelper()
        {
            ModelDetailID = int.MinValue;
            CashFundAlternativeID = int.MinValue;
        }
        
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        
        public string ModelShortName 
        { 
            get 
            {
                if (this.modelShortName.Equals(ModelName))
                    return null;
                else if (ModelName.Length > 15 && this.modelShortName.Equals(ModelName.Substring(0,15)))
                    return null;
                else
                {
                    if (this.modelShortName.Length > 15)
                        return this.modelShortName.Substring(0, 15);
                    else
                        return this.modelShortName;
                }
            }
            set { this.modelShortName = value; }
        }

        public string Description { get; set; }
        public string Notes { get; set; }
        public int AssetManagerId { get; set; }
        public int NextVersion { get; set; }
        public decimal ExpectedReturn { get; set; }
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public bool IsSubModel { get; set; }
        public ExecutionOnlyOptions ExecutionOptions { get; set; }
        public int ModelDetailID { get; set; }
        public int CashFundAlternativeID { get; set; }
        public Login Editor { get; set; }
        public List<ModelComponentHelper> Components { get; set; }

        public override bool Equals(object obj)
        {
            IPortfolioModel model = obj as IPortfolioModel;
            if (model == null)
                return base.Equals(obj);
            else
            {
                bool isEqual = false;
                if (model.LatestVersion.ModelComponents.Count == this.Components.Count)
                {
                    isEqual = true;
                    IModelComponentCollection components = model.LatestVersion.ModelComponents;
                    foreach (ModelComponentHelper mch in Components)
                    {
                        if (components.Where(c => c.ModelComponentType == mch.ModelComponentType &&
                                                  c.ModelComponentKey == mch.ComponentID &&
                                                  c.Allocation == mch.Allocation)
                                      .Count() != 1)
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }
                return isEqual;
            }
        }

        private string modelShortName;
    }

    public class ModelComponentHelper
    {
        public ModelComponentHelper() 
        {
            this.Instruments = new Dictionary<int, string>();
        }

        public ModelComponentHelper(IModelComponent component, int index)
            : this()
        {
            this.Key = index;
            this.ComponentID = component.ModelComponentKey;
            this.ComponentName = component.ComponentName;
            this.ModelComponentType = component.ModelComponentType;
            this.Allocation = component.Allocation;
            if (component.ModelComponentType == ModelComponentType.Instrument)
            {
                IModelInstrument mi = (IModelInstrument)component;
                SetInstrumentDetails(mi.Component);
            }
            else
            {
                IModelModel mi = (IModelModel)component;
                SetInstrumentDetails(mi.Version.ModelInstruments.Instruments);
            }
        }

        public void SetInstrumentDetails(IInstrument instrument)
        {
            this.IsCashManagementFund = instrument.IsCashManagementFund;
            Instruments.Add(instrument.Key, instrument.Name);
        }

        public void SetInstrumentDetails(List<IInstrument> instruments)
        {
            foreach (IInstrument instrument in instruments)
            {
                Instruments.Add(instrument.Key, instrument.Name);
                if (instrument.IsCashManagementFund)
                    this.IsCashManagementFund = instrument.IsCashManagementFund;
            }
        }

        public int Key { get; set; }
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public ModelComponentType ModelComponentType { get; set; }
        public decimal Allocation { get; set; }
        public bool IsCashManagementFund { get; set; }
        public Dictionary<int, string> Instruments { get; set; }
        public string DisplayAllocation
        {
            get
            {
                return this.Allocation.ToString("P5", CultureInfo.CreateSpecificCulture("nl-NL"));
            }
        }

        public static List<ModelComponentHelper> modelComponents;

        public static List<ModelComponentHelper> GetModelComponents()
        {
            return modelComponents;
        }

        public static void UpdateModelComponent(decimal allocation, int original_Key)
        {
            modelComponents.Where(c => c.Key == original_Key).First().Allocation = allocation;
        }
    }

    #endregion

    public static class ModelMaintenanceAdapter
    {
        public static DataSet GetExecutionOptions()
        {
            DataSet ReturnValue = new DataSet();
            DataTable Statuses = new DataTable();
            ReturnValue.Tables.Add(Statuses);

            DataColumn dc1 = new DataColumn("ID", System.Type.GetType("System.Int32"));
            Statuses.Columns.Add(dc1);

            DataColumn dc2 = new DataColumn("Option", System.Type.GetType("System.String"));
            Statuses.Columns.Add(dc2);

            DataRow dr1 = Statuses.NewRow();
            dr1["ID"] = 0;
            dr1["Option"] = "NotAllowed";
            Statuses.Rows.Add(dr1);

            DataRow dr2 = Statuses.NewRow();
            dr2["ID"] = 1;
            dr2["Option"] = "Allowed";
            Statuses.Rows.Add(dr2);

            DataRow dr3 = Statuses.NewRow();
            dr3["ID"] = 2;
            dr3["Option"] = "Always";
            Statuses.Rows.Add(dr3);

            return ReturnValue;

        }

        public static IAssetManager getAssetManager()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);
        }

        public static bool IsLoggedInAsAssetManager()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return LoginMapper.IsLoggedInAsAssetManager(session);
        }

        public static ILogin GetLoginDetails()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return LoginMapper.GetCurrentLogin(session);
        }

        public static DataSet GetModels(string modelName, B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus activeStatus)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ActivityReturnFilter activityFilter = ActivityReturnFilter.All;
                if (activeStatus != B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.All)
                    activityFilter = (activeStatus == B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.Active ? ActivityReturnFilter.Active : ActivityReturnFilter.InActive);

                // "Key, ModelName, LatestVersion.VersionNumber, LatestVersion.LatestVersionDate, AssetManager.CompanyName, LatestVersion.CreatedBy.UserName, IsPublic, IsActive"
                return ModelMapper.GetModelsSorted(session, getAssetManager(), true, true, activityFilter, modelName)
                    .Select(c => new
                    {
                        c.Key,
                        c.ModelName,
                        LatestVersion_VersionNumber = c.LatestVersion.VersionNumber,
                        LatestVersion_LatestVersionDate = c.LatestVersion.LatestVersionDate,
                        AssetManager_CompanyName = c.AssetManager.CompanyName,
                        LatestVersion_CreatedBy_UserName = c.LatestVersion.CreatedBy != null ? c.LatestVersion.CreatedBy.UserName : "",
                        c.IsPublic,
                        c.IsActive,
                        c.IsSubModel
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetModelsExcludedCurrentModel(int modelID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            IList list = ModelMapper.GetModelsSorted(session, true, true);

            if (list != null && list.Count > 0)
            {
                for (int i = list.Count; i > 0; i--)
                {
                    IModelBase model = (IModelBase)list[i - 1];
                    if (model.Key == modelID)
                    {
                        list.RemoveAt(i-1);
                        break;
                    }
                }

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list,
                    "Key, ModelName");
            }
            session.Close();
            return ds;
        }

        public static IModelBase GetModel(int modelID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return ModelMapper.GetModel(session, modelID);
        }

        public static int GetNextVersionforModel(int modelID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            int returnValue = GetNextVersionforModel(session, modelID);
            session.Close();
            return returnValue;
        }

        public static int GetNextVersionforModel(IDalSession session, int modelID)
        {
            int returnvalue;

            if (modelID == 0)
                return 1;
            else
            {
                IModelBase mb = ModelMapper.GetModel(session, modelID);
                if (mb != null)
                    returnvalue = mb.LatestVersion.VersionNumber + 1;
                else
                    returnvalue = 1;

            }
            return returnvalue;
        }

        public static DataSet GetModelVersions(int modelID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                // "Key, VersionNumber, LatestVersionDate, CreatedBy.UserName"
                return ModelMapper.GetModelVersions(session, modelID)
                    .Select(c => new
                    {
                        c.Key,
                        c.VersionNumber,
                        c.LatestVersionDate,
                        CreatedBy_UserName = c.CreatedBy != null ? c.CreatedBy.UserName : ""
                    })
                    .ToDataSet();
            }
        }

        public static ModelComponentHelper GetModelComponentHelper(int parentModelID,
            ModelComponentType modelComponentType, int componentID, int newIndex, decimal allocation)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ModelComponentHelper dr = new ModelComponentHelper();
                dr.Key = newIndex;
                dr.ModelComponentType = modelComponentType;
                dr.Allocation = allocation;

                switch (modelComponentType)
                {
                    case ModelComponentType.Model:
                        IModelBase model = ModelMapper.GetModel(session, componentID);

                        // check if parent model is not embedded in the new child
                        Hashtable parameters = new Hashtable();
                        long embedCount = session.Session.GetNamedQuery(
                            "B4F.TotalGiro.Instruments.ModelEmbeddedInParentModel")
                            .SetParameter("topParentModelID", componentID)
                            .SetParameter("childModelID", parentModelID)
                            .UniqueResult<long>();
                        if (embedCount > 0)
                            throw new ApplicationException(string.Format("It is not possible to embed this model since it already embeds model {0}.", model.ModelName));

                        IModelVersion version = model.LatestVersion;
                        dr.SetInstrumentDetails(version.ModelInstruments.Instruments);
                        dr.ComponentID = version.Key;
                        dr.ComponentName = version.ToString();
                        break;
                    case ModelComponentType.Instrument:
                        ITradeableInstrument instrument = InstrumentMapper.GetTradeableInstrument(session, componentID);
                        dr.SetInstrumentDetails(instrument);
                        dr.ComponentID = instrument.Key;
                        dr.ComponentName = instrument.Name;
                        break;
                }
                return dr;
            }
        }

        public static DataSet GetModelComponents(int modelVersionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                // "Key, ComponentName, ModelComponentType, DisplayAllocation, Allocation"
                return ModelMapper.GetModelComponents(session, modelVersionId)
                    .Select(c => new
                    {
                        c.Key,
                        c.ComponentName,
                        c.ModelComponentType,
                        c.DisplayAllocation,
                        c.Allocation
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetModelInstruments(int modelVersionId)
        {
            if (modelVersionId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IModelVersion mv = ModelMapper.GetModelVersion(session, modelVersionId);
                    IModelInstrument[] instruments = new IModelInstrument[mv.ModelInstruments.Count];
                    mv.ModelInstruments.CopyTo(instruments, 0);

                    if (instruments != null)
                    {
                        // "Key, Component.Name, Component.DisplayIsin, DisplayAllocation, Allocation"
                        return instruments
                            .Select(c => new
                            {
                                c.Key,
                                Component_Name = c.Component.Name,
                                Component_DisplayIsin = c.Component.DisplayIsin,
                                c.DisplayAllocation,
                                c.Allocation
                            })
                            .ToDataSet();
                    }
                }
            }
            return null;
        }

        public static List<ModelComponentHelper> GetModelInstrumentsforLatestModel(int modelID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                List<ModelComponentHelper> returnValue = new List<ModelComponentHelper>();
                IList<IModelComponent> components = ModelMapper.GetModelComponentsLatestVersion(session, modelID);
                int i = 0;

                if ((components != null) && (components.Count > 0))
                {
                    foreach (IModelComponent component in components)
                    {
                        returnValue.Add(new ModelComponentHelper(component, i));
                        i++;
                    }
                }
                return returnValue;
            }
        }

        public static DataSet GetInstruments()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetTradeableInstruments(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Name, 
                        c.Isin
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetModelDetailData()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                session.GetList(typeof(ModelDetail)),
                "Key, Description");
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();
            return ds;
        }

        public static bool IsModelDetailInclCashManagementFund(int modelDetailId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IModelDetail md = (IModelDetail)session.GetObjectInstance(typeof(ModelDetail), modelDetailId);
            bool inclCashFund = md.IncludeCashManagementFund;
            session.Close();
            return inclCashFund;
        }


        public static DataSet GetModelPerformances(int modelID, int quarter, int yyyy)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return ModelPerformanceMapper.GetModelBenchmarkPerformances(session, modelID, quarter, yyyy)
                    .OrderByDescending(c => c.PerformanceYear)
                    .ThenByDescending(c => c.Quarter)
                    .Select(c => new
                    {
                        c.Key,
                        c.IBoxxTarget,
                        c.MSCIWorldTarget,
                        c.CompositeTarget,
                        c.BenchMarkValue,
                        c.Quarter,
                        c.PerformanceYear,
                        c.DisplayBenchMarkValue
                    })
                    .ToDataSet();
            }
        }

        public static void UpdateModelPerformance(decimal benchmarkvalue, int original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IModelPerformance modelperformance = ModelPerformanceMapper.GetModelBenchmarkPerformance(session, original_Key);
            if (modelperformance != null)
            {
                modelperformance.BenchMarkValue = benchmarkvalue;
                session.Update(modelperformance);
            }

            session.Close();
        }

        public static bool CreateModelBenchMarkPerformance(int modelid, decimal iboxxtarget, decimal msciworldtarget, decimal compositetarget, decimal benchmarkperformance, int quarter, int yyyy)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IPortfolioModel model = ModelMapper.GetModel(session, modelid);
            ModelPerformance modelperformance = new ModelPerformance(model, iboxxtarget, msciworldtarget, compositetarget, benchmarkperformance, quarter, yyyy);
            bool success = ModelPerformanceMapper.Insert(session, modelperformance);

            session.Close();
            return success;
        }

        public static bool DeleteModelBenchMarkPerformance(string original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IModelPerformance modelbenchmarkperformance = ModelPerformanceMapper.GetModelBenchmarkPerformance(session, int.Parse(original_Key));

            bool success = ModelPerformanceMapper.Delete(session, modelbenchmarkperformance);

            session.Close();
            return success;
        }

        public static DataSet GetModelFeeRules(int modelID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                FeeRuleMapper.GetModelFeeRules(session, modelID),
                "Key, FeeCalculation.Name, ExecutionOnly, SendByPost, StartPeriod, EndPeriod");
            session.Close();
            return ds;
        }

        public static void UpdateModelFeeRule(int endPeriod, int original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IFeeRule rule = FeeRuleMapper.GetFeeRule(session, original_Key);
            if (rule != null)
            {
                rule.EndPeriod = endPeriod;
                session.Update(rule);
            }

            session.Close();
        }

        public static bool CreateModelFeeRule(int modelId, int feeCalculationId,
            bool executionOnly, bool sendByPost, int startPeriod, int endPeriod)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            
            IPortfolioModel model = ModelMapper.GetModel(session, modelId);
            IFeeCalc calc = FeeCalcMapper.GetFeeCalculation(session, feeCalculationId);

            FeeRule rule = new FeeRule(calc, model, null, false, executionOnly, false, sendByPost, startPeriod);
            if (endPeriod > 0)
                rule.EndPeriod = endPeriod;
            bool success = FeeRuleMapper.Insert(session, rule);
            session.Close();
            return success;
        }

        public static DataSet GetActiveFeeCalculations(int modelID)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();

            IModelBase model = ModelMapper.GetModel(session, modelID);
            if (model != null)
            {
                if (model.AssetManager != null)
                    parameters.Add("managementCompanyID", model.AssetManager.Key);
                IList list = session.GetListByNamedQuery(
                    "B4F.TotalGiro.Fees.FeeCalculations.ActiveFeeCalculations",
                    parameters);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list,
                    "Key, Name");
                Utility.AddEmptyFirstRow(ds.Tables[0]);
            }

            session.Close();
            return ds;
        }

        //public static DataSet GetPossibleCashFundAlternativeInstruments(int modelID)
        //{
        //    DataSet ds = null;
        //    IDalSession session = NHSessionFactory.CreateSession();

        //    if (modelID != 0)
        //    {
        //        IModelBase model = ModelMapper.GetModel(session, modelID);
        //        IModelVersion mv = model.LatestVersion;
        //        if (mv != null && mv.ModelInstruments != null && mv.ModelInstruments.Count > 1 && !mv.ContainsCashManagementFund)
        //        {
        //            IModelInstrument[] instruments = new IModelInstrument[mv.ModelInstruments.Count];
        //            mv.ModelInstruments.CopyTo(instruments, 0);

        //            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
        //                instruments,
        //                "Component.Key, Component.DisplayNameWithIsin");
        //        }
        //    }
        //    session.Close();

        //    if (ds == null)
        //    {
        //        ds = new DataSet();
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("Component_Key", typeof(int));
        //        dt.Columns.Add("Component_DisplayNameWithIsin", typeof(string));
        //        ds.Tables.Add(dt);
        //    }
        //    Utility.AddEmptyFirstRow(ds.Tables[0], "Component_Key");
        //    return ds;
        //}

        public static void UpdateVersion(ModelVersionHelper modelVersionHelper)
        {
            if (modelVersionHelper == null)
                return;

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILogin login = LoginMapper.GetCurrentLogin(session);
                IModelBase model;
                IModelVersion modelVersion = null;
                List<ModelHierarchy> parentModels = null;
                
                bool createNewVersion = true;
                bool createComponents = true;

                if (modelVersionHelper.ModelID == 0)   //First Create the Model
                {
                    model = new PortfolioModel();
                    model.AssetManager = getAssetManager();
                    model.CreatedBy = login.UserName;
                }
                else
                {
                    model = ModelMapper.GetModel(session, modelVersionHelper.ModelID);
                    if (modelVersionHelper.Equals(model))
                    {
                        createNewVersion = false;
                        createComponents = false;
                    }
                    else if (model.LatestVersion.LatestVersionDate.Date.Equals(DateTime.Today))
                        createNewVersion = false;
                }

                model.ModelName = modelVersionHelper.ModelName;
                model.IsSubModel = modelVersionHelper.IsSubModel;
                model.ShortName = modelVersionHelper.ModelShortName;
                model.Description = modelVersionHelper.Description;
                model.ModelNotes = modelVersionHelper.Notes;
                model.IsPublic = modelVersionHelper.IsPublic;
                model.ExpectedReturn = modelVersionHelper.ExpectedReturn;

                // Get parent models
                if (model.Key > 0)
                {
                    Hashtable parameters = new Hashtable();
                    parameters.Add("childModelID", model.Key);
                    parentModels = session.GetTypedListByNamedQuery<ModelHierarchy>(
                        "B4F.TotalGiro.Instruments.ParentModels",
                        parameters);
                }

                // check IsActive
                if (model.Key > 0 && model.IsActive && !modelVersionHelper.IsActive)
                {
                    // check if model is on active accounts
                    long activeAccounts = session.Session.GetNamedQuery(
                        "B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelOnActiveCustomers")
                        .SetParameter("modelId", model.Key)
                        .UniqueResult<long>();
                    if (activeAccounts > 0)
                        throw new ApplicationException(string.Format("It is not possible to inactivate this model since {0} active accounts are attached to it.", activeAccounts));

                    // check if model is nested in active models
                    if (parentModels != null && parentModels.Count > 0)
                        throw new ApplicationException(string.Format("It is not possible to inactivate this model since {0} active models embed this model.", parentModels.Count));
                }
                model.IsActive = modelVersionHelper.IsActive;

                if (model.ModelType == ModelType.PortfiolioModel)
                {
                    IPortfolioModel portfolioModel = (IPortfolioModel)model;
                    
                    portfolioModel.ExecutionOptions = modelVersionHelper.ExecutionOptions;
                    if (modelVersionHelper.ModelDetailID != int.MinValue)
                        portfolioModel.Details = (IModelDetail)session.GetObjectInstance(typeof(ModelDetail), modelVersionHelper.ModelDetailID);
                    else
                        portfolioModel.Details = null;
                    if (modelVersionHelper.CashFundAlternativeID != int.MinValue)
                        portfolioModel.CashFundAlternative = InstrumentMapper.GetTradeableInstrument(session, modelVersionHelper.CashFundAlternativeID);
                    else
                        portfolioModel.CashFundAlternative = null;
                }

                if (createNewVersion)
                {
                    modelVersion = new ModelVersion();
                    modelVersion.ParentModel = model;
                    if (model.Key == 0)
                        modelVersion.VersionNumber = 1;
                    else
                        modelVersion.VersionNumber = GetNextVersionforModel(session, model.Key);
                    modelVersion.LatestVersionDate = DateTime.Now;
                    modelVersion.CreatedBy = login;
                }
                else
                {
                    modelVersion = model.LatestVersion;
                }

                if (createComponents && modelVersion != null)
                {
                    modelVersion.ModelComponents.Clear();
                    foreach (ModelComponentHelper modelComponentHelper in modelVersionHelper.Components)
                    {
                        IModelComponent modelComponent = null;
                        switch (modelComponentHelper.ModelComponentType)
                        {
                            case ModelComponentType.Model:
                                modelComponent = new ModelModel(ModelMapper.GetModelVersion(session, modelComponentHelper.ComponentID), 
                                                                modelComponentHelper.Allocation);
                                break;
                            case ModelComponentType.Instrument:
                                IInstrument inst = InstrumentMapper.GetInstrument(session, modelComponentHelper.ComponentID);
                                modelComponent = new ModelInstrument(modelVersion, inst, modelComponentHelper.Allocation);
                                break;
                            default:
                                break;
                        }
                        modelVersion.ModelComponents.AddComponent(modelComponent);
                    }
                    model.LatestVersion = modelVersion;
                }

                ModelMapper.Update(session, model);

                if (createNewVersion && parentModels != null && parentModels.Count > 0)
                {
                    List<IModelBase> modelsToUpdate = new List<IModelBase>();
                    foreach (IModelHierarchy modelHierarchy in parentModels)
                    {
                        if (updateVersionOnParentModels(session, login, (IPortfolioModel)modelHierarchy.TopParentModel, modelVersion))
                            modelsToUpdate.Add(modelHierarchy.TopParentModel);
                    }
                    if (modelsToUpdate.Count > 0)
                        ModelMapper.Update(session, modelsToUpdate);
                }
            }
        }

        private static bool updateVersionOnParentModels(IDalSession session, ILogin login, IPortfolioModel parent, IModelVersion newVersion)
        {
            bool success = false;
            IModelVersion parentVersion = parent.LatestVersion;
            if (parentVersion != null && parentVersion.ModelComponents != null)
            {
                IModelComponent oldModelComponent = (from mc in parentVersion.ModelComponents
                                                     where mc.ModelComponentType == ModelComponentType.Model && 
                                                           ((IModelModel)mc).Component.Key == newVersion.ParentModel.Key
                                                     select mc)
                                                    .First();

                if (oldModelComponent != null)
                {
                    // same day -> no new version -> just replace with new version
                    if (parentVersion.LatestVersionDate.Date.Equals(DateTime.Today))
                    {
                        parentVersion.ModelComponents.RemoveComponent(oldModelComponent);
                        parentVersion.ModelComponents.AddComponent(new ModelModel(newVersion, oldModelComponent.Allocation));
                        success = true;
                    }
                    else // create new version
                    {
                        ModelVersion modelVersion = new ModelVersion();
                        modelVersion.ParentModel = parent;
                        modelVersion.VersionNumber = parentVersion.VersionNumber + 1;
                        modelVersion.LatestVersionDate = DateTime.Now;
                        modelVersion.CreatedBy = login;

                        foreach (IModelComponent modelComponent in parentVersion.ModelComponents)
                        {
                            if (modelComponent.Key == oldModelComponent.Key)
                                modelVersion.ModelComponents.AddComponent(new ModelModel(newVersion, oldModelComponent.Allocation));
                            else
                            {
                                switch (modelComponent.ModelComponentType)
                                {
                                    case ModelComponentType.Model:
                                        modelVersion.ModelComponents.AddComponent(new ModelModel(((IModelModel)modelComponent).Version, 
                                                                                        modelComponent.Allocation));
                                        break;
                                    case ModelComponentType.Instrument:
                                        modelVersion.ModelComponents.AddComponent(new ModelInstrument(modelVersion, 
                                                                                             ((IModelInstrument)modelComponent).Component,
                                                                                             modelComponent.Allocation));
                                        break;
                                }
                            }
                        }

                        parent.LatestVersion = modelVersion;
                        success = true;
                    }
                }
            }
            return success;
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class is used to instantiate Model objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public abstract class ModelMapper
    {
        /// <summary>
        /// Get model by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="id">Identifier</param>
        /// <returns>Model object</returns>
        public static IPortfolioModel GetModel(IDalSession session, int id)
		{
            IPortfolioModel model = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            List<IPortfolioModel> list = session.GetTypedList<PortfolioModel, IPortfolioModel>(expressions);
            if (list != null && list.Count > 0)
                model = list[0];
            return model;
		}

        public static IModelBase GetModelBase(IDalSession session, int id)
        {
            IModelBase model = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            List<IModelBase> list = session.GetTypedList<ModelBase, IModelBase>(expressions);
            if (list != null && list.Count > 0)
                model = list[0];
            return model;
        }

        /// <summary>
        /// Get Model version by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="versionID">Identifier</param>
        /// <returns>ModelVersion object</returns>
		public static IModelVersion GetModelVersion(IDalSession session, int versionID)
		{
			List<ICriterion> expressions = new List<ICriterion>();
			expressions.Add(Expression.Eq("Key", versionID));
            List<IModelVersion> versions = session.GetTypedList<ModelVersion, IModelVersion>(expressions);
			if (versions != null && versions.Count == 1)
				return versions[0];
			else
				return null;
		}

        /// <summary>
        /// Get collection of modelversions of a model
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="modelID">Identifier</param>
        /// <returns>Collection of modelversions</returns>
		public static IList<IModelVersion> GetModelVersions(IDalSession session, int modelID)
		{
            Hashtable parameters = new Hashtable();
            parameters.Add("modelID", modelID);
            return session.GetTypedListByNamedQuery<IModelVersion>(
                "B4F.TotalGiro.Instruments.ModelVersion.GetModelVersion",
                parameters);
		}

        public static IList<IModelVersion> GetModelVersionsForDate(IDalSession session, int[] modelIds, DateTime date)
        {
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("date", date.Date);
            parameterLists.Add("modelIds", modelIds);
            IList<IModelVersion> versions = session.GetTypedListByNamedQuery<IModelVersion>(
                "B4F.TotalGiro.Instruments.ModelVersion.GetModelVersionsForModelsAndDate",
                parameters, parameterLists);

            return (from a in versions
                    group a by a.ParentModel.Key into g
                    select g.OrderByDescending(x => x.LatestVersionDate).First() 
                    )
                    .OrderBy(x => x.ParentModel.ModelName)
                    .ToList();
        }

        public static IList<IModelComponent> GetModelComponents(IDalSession session, int modelVersionID)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ParentVersion.Key", modelVersionID));
            return session.GetTypedList<ModelComponent, IModelComponent>(expressions);
        }

        public static IList<IModelComponent> GetModelComponentsLatestVersion(IDalSession session, int modelID)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("modelID", modelID);
            return session.GetTypedListByNamedQuery<IModelComponent>(
                "B4F.TotalGiro.Instruments.ModelComponent.GetModelComponentsLatestVersion",
                parameters);
        }

        /// <summary>
        /// Get all system models
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="includePublic">Include Public Models</param>
        /// <param name="activityFilter">Active Filter</param>
        /// <returns>Collection of models</returns>
        public static List<IPortfolioModel> GetModels(IDalSession session, bool includePublic, ActivityReturnFilter activityFilter)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Asc("ModelName"));

            addAssetManagerCriterion(expressions, getForcedAssetManagerId(session, null), includePublic);

            if (activityFilter != ActivityReturnFilter.All)
                expressions.Add(Expression.Eq("IsActive", (activityFilter == ActivityReturnFilter.Active ? true : false)));
            
            return session.GetTypedList<PortfolioModel, IPortfolioModel>(expressions, orderings, null, null);
        }

        /// <summary>
        /// Get all system models, sorted alphabetically
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="includePublic">Include Public Models</param>
        /// <returns>Collection of models</returns>
        public static List<IPortfolioModel> GetModelsSorted(IDalSession session, bool includePublic, bool includeSubModels)
        {
            return GetModelsSorted(session, null, includePublic, includeSubModels, ActivityReturnFilter.Active);
        }

        /// <summary>
        /// Get all system models, sorted alphabetically
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="assetManager">The assetManager that owns the model</param>
        /// <param name="includePublic">Include Public Models</param>
        /// <returns>Collection of models</returns>
        public static List<IPortfolioModel> GetModelsSorted(IDalSession session, IAssetManager assetManager, bool includePublic, bool includeSubModels)
        {
            return GetModelsSorted(session, assetManager, includePublic, includeSubModels, ActivityReturnFilter.Active, null);
        }

        /// <summary>
        /// Get all system models, sorted alphabetically
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="assetManager">The assetManager that owns the model</param>
        /// <param name="includePublic">Include Public Models</param>
        /// <param name="activityFilter">Active Filter</param>
        /// <returns>Collection of models</returns>
        public static List<IPortfolioModel> GetModelsSorted(IDalSession session, IAssetManager assetManager, bool includePublic, bool includeSubModels, ActivityReturnFilter activityFilter)
        {
            return GetModelsSorted(session, assetManager, includePublic, includeSubModels, activityFilter, null);
        }

        /// <summary>
        /// Get all system models, sorted alphabetically
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="assetManager">The assetManager that owns the model</param>
        /// <param name="includePublic">Include Public Models</param>
        /// <param name="activityFilter">Active Filter</param>
        /// <param name="modelName">Model Name Filter</param>
        /// <returns>Collection of models</returns>
        public static List<IPortfolioModel> GetModelsSorted(IDalSession session, IAssetManager assetManager, bool includePublic, bool includeSubModels,
                                                            ActivityReturnFilter activityFilter, string modelName)
        {
            if (assetManager != null || !LoginMapper.IsLoggedInAsStichting(session))
                return GetModelsSorted(session, getForcedAssetManagerId(session, assetManager), includePublic, includeSubModels, activityFilter, modelName);
            else
                return null;
        }

        /// <summary>
        /// This method does NOT filter by the current asset manager.
        /// </summary>
        public static List<IPortfolioModel> GetModelsSorted(IDalSession session, int assetManagerId, bool includePublic, bool includeSubModels,
                                                            ActivityReturnFilter activityFilter, string modelName)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();

            addAssetManagerCriterion(expressions, assetManagerId, includePublic);

            if (!includeSubModels)
                expressions.Add(Expression.Or(Expression.Eq("IsSubModel", false), Expression.IsNull("IsSubModel")));
            if (activityFilter != ActivityReturnFilter.All)
                expressions.Add(Expression.Eq("IsActive", (activityFilter == ActivityReturnFilter.Active ? true : false)));
            if (!string.IsNullOrEmpty(modelName))
                expressions.Add(Expression.Like("ModelName", modelName, MatchMode.Anywhere));
            orderings.Add(Order.Asc("ModelName"));

            return session.GetTypedList<PortfolioModel, IPortfolioModel>(expressions, orderings, null, null);
        }

        private static int getForcedAssetManagerId(IDalSession session, IAssetManager assetManager)
        {
            IManagementCompany currentCompany = LoginMapper.GetCurrentManagmentCompany(session);
            if (currentCompany != null)
            {
                if (!currentCompany.IsStichting)
                    assetManager = (IAssetManager)currentCompany;

                return assetManager != null ? assetManager.Key : 0;
            }
            else
                throw new ApplicationException("Current management company could not be found.");
        }

        private static void addAssetManagerCriterion(List<ICriterion> expressions, int assetManagerId, bool IncludePublic)
        {
            if (assetManagerId != 0)
            {
                if (IncludePublic)
                    expressions.Add(Expression.Or(Expression.Eq("AssetManager.Key", assetManagerId), Expression.Eq("IsPublic", true)));
                else
                    expressions.Add(Expression.Eq("AssetManager.Key", assetManagerId));
            }
        }

        public static bool Update(IDalSession session, IModelBase obj)
        {
            return session.InsertOrUpdate(obj);
        }

        public static bool Update(IDalSession session, IList list)
        {
            return session.InsertOrUpdate(list);
        }
    }
}

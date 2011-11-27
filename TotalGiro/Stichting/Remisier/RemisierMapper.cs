using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Stichting.Remisier
{
    public enum RemisierFilterTypes
    {
        All = 0,
        External = 1,
        Internal = 2
    }
    
    /// <summary>
    /// This class is used to instantiate Remisier objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public static class RemisierMapper
    {
        /// <summary>
        /// Gets Remisier by unique identifier
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="RemisierID">Unique identifier</param>
        /// <returns>Object of Remisier</returns>
        public static IRemisier GetRemisier(IDalSession session, int remisierId)
        {
            return session.GetTypedList<Remisier, IRemisier>(remisierId)
                          .FirstOrDefault();
       }

        /// <summary>
        /// Get all Remisiers
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <returns>Collection of all Remisiers</returns>
        public static List<IRemisier> GetRemisiers(IDalSession session)
        {
            return session.GetTypedList<Remisier, IRemisier>()
                          .OrderBy(r => r.Name)
                          .ToList();
        }

        public static List<IRemisier> GetRemisiers(IDalSession session, IAssetManager assetManager)
        {
            return GetRemisiers(session, assetManager, null, ActivityReturnFilter.Active);
        }

        public static List<IRemisier> GetRemisiers(IDalSession session, IAssetManager assetManager, RemisierFilterTypes remisierFilterType)
        {
            List<IRemisier> list = GetRemisiers(session, assetManager, null, ActivityReturnFilter.Active);

            if (remisierFilterType != RemisierFilterTypes.All)
                return list.Where(r => r.IsInternal == (remisierFilterType == RemisierFilterTypes.Internal))
                           .ToList();
            else
                return list;
        }

        public static List<IRemisier> GetRemisiers(IDalSession session, IAssetManager assetManager, string remisierName,
                                                   ActivityReturnFilter activityFilter)
        {
            bool loggedInAsStichting = LoginMapper.IsLoggedInAsStichting(session);
            if (assetManager == null && !loggedInAsStichting)
                assetManager = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);

            if (assetManager != null || loggedInAsStichting)
            {
                int assetManagerId = assetManager != null ? assetManager.Key : 0;
                return GetRemisiers(session, assetManagerId, remisierName, activityFilter);
            }
            else
                return null;
        }

        public static List<IRemisier> GetRemisiers(IDalSession session, int assetManagerId)
        {
            return GetRemisiers(session, assetManagerId, null, ActivityReturnFilter.Active);
        }

        /// <summary>
        /// This method does NOT filter by the current asset manager.
        /// </summary>
        public static List<IRemisier> GetRemisiers(IDalSession session, int assetManagerId, string remisierName,
                                                   ActivityReturnFilter activityFilter)
        {
            Hashtable parameters = new Hashtable();

            if (assetManagerId != 0)
                parameters.Add("assetManagerId", assetManagerId);
            if (!string.IsNullOrEmpty(remisierName))
                parameters.Add("remisierName", Util.PrepareNamedParameterWithWildcard(remisierName));
            if (activityFilter != ActivityReturnFilter.All)
                parameters.Add("deleted", activityFilter != ActivityReturnFilter.Active);

            return session.GetTypedListByNamedQuery<IRemisier>(
                                "B4F.TotalGiro.Stichting.Remisier.Remisiers",
                                parameters);
        }

        #region CRUD

        public static bool Insert(IDalSession session, IRemisier obj)
        {
            return session.Insert(obj);
        }

        public static bool Update(IDalSession session, IRemisier obj)
        {
            return session.Update(obj);
        }

        #endregion
    }
}

using System;
using System.Linq;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    /// <summary>
    /// Class that keeps a (static) table of interface types, each with an ID-property getter and a name.
    /// Its role is to:
    ///     1. check objects for nullity, and/or
    ///     2. check whether an object's ID is the same as a specified ID,
    /// and throw an exception if they fail.
    /// </summary>
    internal class ObjectChecker
    {
        private ObjectChecker(Type objectType, Func<object, int> objectIdGetter, string objectName)
        {
            ObjectType = objectType;
            ObjectIdGetter = objectIdGetter;
            ObjectName = objectName;
        }

        private Type ObjectType { get; set; }
        private Func<object, int> ObjectIdGetter { get; set; }
        private string ObjectName { get; set; }


        public static T GetNotNull<T>(T obj)
        {
            if (obj == null)
                throw new SecurityLayerException(string.Format("No {0} is associated with the current login.",
                                                               findChecker<T>(obj).ObjectName));
            return obj;
        }

        public static int GetForcedKey<T>(T obj, int requestedId)
        {
            GetNotNull<T>(obj);

            ObjectChecker objectChecker = findChecker<T>(obj);
            int objectId = objectChecker.ObjectIdGetter(obj);

            if (requestedId != 0 && requestedId != objectId)
                throw new SecurityLayerException(string.Format("User not authorized to access data belonging to specified {0}.",
                                                               objectChecker.ObjectName));
            return objectId;
        }

        private static ObjectChecker findChecker<T>(T obj)
        {
            ObjectChecker objectChecker = objectCheckers.FirstOrDefault(oc => oc.ObjectType == typeof(T));
            if (objectChecker != null)
                return objectChecker;
            else
                throw new ApplicationException(string.Format("Could not find ObjectChecker for type {0}.", typeof(T).Name));
        }

        private static ObjectChecker[] objectCheckers = new [] { 
            new ObjectChecker(typeof(IAssetManager),      obj => ((IAssetManager)obj).Key,      "asset manager"),
            new ObjectChecker(typeof(IManagementCompany), obj => ((IManagementCompany)obj).Key, "management company"),
            new ObjectChecker(typeof(IRemisier),          obj => ((IRemisier)obj).Key,          "remisier"),
            new ObjectChecker(typeof(IRemisierEmployee),  obj => ((IRemisierEmployee)obj).Key,  "remisier employee"),
            new ObjectChecker(typeof(IContact),           obj => ((IContact)obj).Key,           "contact")
        };
    }
}

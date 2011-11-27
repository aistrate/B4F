using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace B4F.TotalGiro.ApplicationLayer
{
    public static class Utility
    {
        internal static void AddEmptyFirstRow(DataSet ds)
        {
            if (ds != null)
                AddEmptyFirstRow(ds.Tables[0], "Key");
        }
        
        internal static void AddEmptyFirstRow(DataTable dt)
        {
            if (dt != null)
                AddEmptyFirstRow(dt, "Key");
        }

        public static void AddEmptyFirstRow(DataTable dt, string keyFieldName)
        {
            AddEmptyFirstRow(dt, keyFieldName, int.MinValue);
        }

        internal static void AddEmptyFirstRow(DataSet ds, string valueFieldName, string value)
        {
            if (ds != null)
                AddEmptyFirstRow(ds.Tables[0], "Key", int.MinValue, valueFieldName, value);
        }

        public static void AddEmptyFirstRow(DataSet ds, string keyFieldName, int keyValue, string valueFieldName, string value)
        {
            if (ds != null)
                AddEmptyFirstRow(ds.Tables[0], keyFieldName, keyValue, valueFieldName, value);
        }

        public static void AddEmptyFirstRow(DataTable dt, string keyFieldName, int keyValue)
        {
            AddEmptyFirstRow(dt, keyFieldName, keyValue, null, null);
        }

        public static void AddEmptyFirstRow(DataTable dt, string keyFieldName, int keyValue, string valueFieldName, string value)
        {
            DataRow row = dt.NewRow();
            int keyFieldIndex = dt.Columns.IndexOf(keyFieldName);

            row[keyFieldIndex] = keyValue;
            if (!string.IsNullOrEmpty(valueFieldName) && !string.IsNullOrEmpty(value))
                row[valueFieldName] = value;
            dt.Rows.InsertAt(row, 0);
        }

        public static void RemoveRow(DataTable dt, int value)
        {
            RemoveRow(dt, "Key", value);
        }

        public static void RemoveRow(DataTable dt, string keyFieldName, int value)
        {
            int keyFieldIndex = dt.Columns.IndexOf(keyFieldName);
            foreach (DataRow row in dt.Rows)
            {
                if ((int)row[keyFieldIndex] == value)
                {
                    dt.Rows.Remove(row);
                    return;
                }
            }
        }

        public static string[] SubtractLists(string[] listToSubtractFrom, string[] listToSubtract)
        {
            ArrayList differenceList = new ArrayList();
            foreach (string name in listToSubtractFrom)
                if (!IsNameInList(name, listToSubtract))
                    differenceList.Add(name);

            string[] difference = new string[differenceList.Count];
            differenceList.CopyTo(difference);

            return difference;
        }

        public static bool IsNameInList(string userName, string[] userNameList)
        {
            foreach (string user in userNameList)
                if (user == userName)
                    return true;

            return false;
        }

        public static string GetPathFromConfigFile(string configFileEntry)
        {
            string path = ConfigurationManager.AppSettings.Get(configFileEntry);
            if (path != null)
            {
                if (Directory.Exists(path))
                    return path + (path.Substring(path.Length - 1) == "\\" ? "" : "\\");
                else
                    throw new DirectoryNotFoundException(
                        string.Format("Could not find folder {0}. Please create this folder or change entry '{1}' in the config file to point to a valid folder.",
                                      path, configFileEntry));
            }
            else
                throw new ConfigurationErrorsException(string.Format("Could not find entry '{0}' in config file.", configFileEntry));
        }

        public static string GetCompleteExceptionMessage(Exception ex)
        {
            if (ex is System.Reflection.TargetInvocationException && ex.InnerException != null)
                return GetCompleteExceptionMessage(ex.InnerException);
            else
                return ex.Message + "<br/>" + (ex.InnerException != null ? GetCompleteExceptionMessage(ex.InnerException) : "");
        }

        public static string ReadResource(Assembly assembly, string resource)
        {
            try
            {
                Stream resourceStream = assembly.GetManifestResourceStream(resource);
                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Could not read resource '{0}'.", resource), ex);
            }
        }

        /// <summary>
        /// Makes an XML tag visible/hidden, depending on the 'visible' parameter.
        /// The tag 'tagName' should not be embedded inside another tag with the same name.
        /// </summary>
        public static string ShowOptionalTag(string text, string tagName, bool visible)
        {
            Regex re = new Regex(string.Format(@"<{0}>(.*?)</{0}>", tagName),
                                 RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            return re.Replace(text, visible ? "$1" : "");
        }
    }
}

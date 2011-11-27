using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

public class QueryParameterCollection : Dictionary<string, string[]>
{
    public QueryParameterCollection(HttpContext context) 
                : this(context.Request.RawUrl) { }

    public QueryParameterCollection(string rawUrl)
        : base(toDictionary(rawUrl)) { }

    private static Dictionary<string, string[]> toDictionary(string rawUrl)
    {
        int indexQuery = rawUrl.IndexOf('?');
        if (indexQuery >= 0 && indexQuery < rawUrl.Length - 1)
        {
            NameValueCollection nameValueColl = HttpUtility.ParseQueryString(rawUrl.Substring(indexQuery + 1));
            
            return nameValueColl.Keys.Cast<string>()
                                     .Where(k => k != null)
                                     .ToDictionary(k => k.ToLower(), k => nameValueColl[k].Split(','));
        }
        else
            return new Dictionary<string, string[]>();
    }

    public T GetValue<T>(string key, T defaultValue, Func<string, T> valueConverter)
    {
        string[] values;
        if (TryGetValue(key.ToLower(), out values))
            return valueConverter(values[values.Length - 1]);
        else
            return defaultValue;
    }

    public string GetStringValue(string key, string defaultValue)
    {
        return GetValue<string>(key, defaultValue, val => val);
    }

    public bool GetBoolValue(string key, bool defaultValue)
    {
        return GetValue<bool>(key, defaultValue, val =>
        {
            bool converted;
            return bool.TryParse(val, out converted) ? converted : defaultValue;
        });
    }

    public int GetIntValue(string key, int defaultValue)
    {
        return GetValue<int>(key, defaultValue, val =>
        {
            int converted;
            return int.TryParse(val, out converted) ? converted : defaultValue;
        });
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Utils
{
    public static class ConfigSettingsInfo
    {
        public static string GetInfo(string key)
        {
            string info = "";
            try
            {
                // Get the appSettings.
                info = (string)(System.Configuration.ConfigurationSettings.AppSettings.Get(key));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return info;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ClientApplicationLayer.Common
{
    public static class CommonAdapter
    {
        public static string GetDatabaseName()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return session.Session.Connection.Database;
            }
        }

        public static string GetUserName()
        {
            return SecurityManager.CurrentUser;
        }

        public static bool IsCurrentUserInRole(string roleName)
        {
            return SecurityManager.IsCurrentUserInRole(roleName);
        }
        
        public static DateTime GetLastLoginDate(string userName)
        {
            return SecurityManager.GetLastLoginDate(userName);
        }

        public static void CreateUserLogEntry(string userName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                UserLogEntry userLogEntry = new UserLogEntry(userName);
                session.Insert(userLogEntry);
            }
        }

        public static bool RunningInDebugMode()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public static LoginTypes GetCurrentLoginType()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetCurrentLoginType(session);
            }
        }
        
        public static DataSet GetContactAccounts(int contactId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedContactAccounts(session, contactId, true)
                                           .Select(a => new { a.Key, a.DisplayNumberWithName })
                                           .ToDataSet();
            }
        }

        public static Color GetTrafficLightColor(decimal chanceOfMeetingTarget)
        {
            return lowerBounds.Last(p => p.Key <= chanceOfMeetingTarget).Value;
        }

        public static Color[] TrafficLightColors
        {
            get { return lowerBounds.Values.ToArray(); }
        }

        private static SortedList<decimal, Color> lowerBounds = new SortedList<decimal, Color>()
        {
            { 0.00m, Color.Red },
            { 0.50m, Color.Yellow },
            { 0.80m, Color.Green  }
        };
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Security;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ApplicationLayer.Admin
{
    public static class RoleOverviewAdapter
    {
        public static DataSet GetRoles()
        {
            string[] roles = Roles.GetAllRoles();
            object[] roleObjects = new object[roles.Length];
            int i = 0;
            foreach (string role in roles)
                roleObjects[i++] = new object[] { role, Roles.GetUsersInRole(role).Length };
            return DataSetBuilder.CreateDataSetFromHibernateList(
                roleObjects, "RoleName, NumberOfUsers");
        }

        public static DataSet GetUsersInRole(string roleName)
        {
            DataSet ds = (DataSet)HttpContext.Current.Session["dsUsersInRole"];

            if (ds == null)
            {
                // Get all users
                MembershipUserCollection userColl = Membership.GetAllUsers();
                MembershipUser[] users = new MembershipUser[userColl.Count];
                userColl.CopyTo(users, 0);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    users, "UserName");

                // Add check-boxes
                string[] usersInRole = Roles.GetUsersInRole(roleName);
                DataColumn col = ds.Tables[0].Columns.Add("IsInRole", typeof(bool));
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string userName = (string)row["UserName"];
                    row["IsInRole"] = Utility.IsNameInList(userName, usersInRole);
                }

                HttpContext.Current.Session["dsUsersInRole"] = ds;
            }

            return ds;
        }

        public static void DeleteRole(string roleName)
        {
            Roles.DeleteRole(roleName, false);
        }

        public static void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        public static void SaveRole(string roleName, string[] newUserList)
        {
            string[] oldUserList = Roles.GetUsersInRole(roleName);

            string[] usersToAdd = newUserList.Except(oldUserList).ToArray();
            string[] usersToRemove = oldUserList.Except(newUserList).ToArray();
            string[] usersUnchanged = oldUserList.Except(usersToRemove).ToArray();

            if (usersToAdd.Length > 0)
                Roles.AddUsersToRole(usersToAdd, roleName);
            if (usersToRemove.Length > 0)
                Roles.RemoveUsersFromRole(usersToRemove, roleName);

            if (usersToAdd.Length > 0 || usersToRemove.Length > 0)
                SendUpdatedPermissionsEmail(roleName, usersToAdd, usersToRemove, usersUnchanged, true);
        }

        internal static void SendUpdatedPermissionsEmail(string parent, string[] childrenToAdd, string[] childrenToRemove, string[] childrenUnchanged,
                                                         bool parentIsRole)
        {
            try
            {
                string testRecipients = ConfigurationManager.AppSettings.Get("UpdatedPermissionsEmailRecipients");
                if (testRecipients != null && testRecipients.Trim() != "")
                {
                    string body = updatedPermissionsEmailTemplate;

                    body = Utility.ShowOptionalTag(body, "option-role-with-users", parentIsRole);
                    body = Utility.ShowOptionalTag(body, "option-user-with-roles", !parentIsRole);

                    body = body.Replace("<%PermissionParent%>", parent)
                               .Replace("<%FormattedAdd%>", formatAsAdded("this"))
                               .Replace("<%FormattedRemove%>", formatAsRemoved("this"));

                    string[] childrenAllInvolved = childrenToAdd.Union(childrenToRemove).Union(childrenUnchanged).OrderBy(child => child).ToArray();

                    string[] formattedChildNames = childrenAllInvolved.Select(
                                                        child => childrenToAdd.Contains(child) ? formatAsAdded(child) :
                                                                 childrenToRemove.Contains(child) ? formatAsRemoved(child) :
                                                                 child).ToArray();

                    body = body.Replace("<%PermissionChildren%>", string.Join("<br />", formattedChildNames));

                    MailMessage message = new MailMessage();

                    message.To.Add(testRecipients);
                    message.Subject = string.Format("{0} permissions updated", parentIsRole ? "Role" : "User");
                    message.Body = body;
                    message.IsBodyHtml = true;

                    SmtpClient client = new SmtpClient();
                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new SmtpException("Could not send 'Permissions updated' email.", ex);
            }
        }

        private static string formatAsAdded(string text)
        {
            return string.Format("<span style=\"font-weight: bold; color: Blue\"><ins>{0}</ins></span>", text);
        }

        private static string formatAsRemoved(string text)
        {
            return string.Format("<span style=\"font-style: italic; color: Red\"><del>{0}</del></span>", text);
        }

        private static string updatedPermissionsEmailTemplate = Utility.ReadResource(Assembly.GetAssembly(typeof(RoleOverviewAdapter)),
                                                                                "B4F.TotalGiro.ApplicationLayer.Admin.UpdatedPermissionsEmail.htm");
    }
}

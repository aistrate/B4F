using System.Linq;
using System.Web.Security;

namespace B4F.TotalGiro.ApplicationLayer.Admin
{
    public static class UserEditAdapter
    {
        public static void GetUserData(string userName, out string email, out string comment, out bool isApproved)
        {
            MembershipUser user = Membership.GetUser(userName);

            email = user.Email;
            comment = user.Comment;
            isApproved = user.IsApproved;
        }

        public static void SaveUser(string userName, string email, string comment, bool isApproved)
        {
            MembershipUser user = Membership.GetUser(userName);

            user.Email = email;
            user.Comment = comment;
            user.IsApproved = isApproved;

            Membership.UpdateUser(user);
        }

        public static void GetUserRoles(string userName, out string[] allRoles, out bool[] activeRoles)
        {
            allRoles = Roles.GetAllRoles();
            activeRoles = new bool[allRoles.Length];

            string[] rolesOfThisUser = Roles.GetRolesForUser(userName);

            for (int i = 0; i < allRoles.Length; i++)
                activeRoles[i] = Utility.IsNameInList(allRoles[i], rolesOfThisUser);
        }

        public static void SaveUserRoles(string userName, string[] newRoleList)
        {
            string[] oldRoleList = Roles.GetRolesForUser(userName);

            string[] rolesToAdd = newRoleList.Except(oldRoleList).ToArray();
            string[] rolesToRemove = oldRoleList.Except(newRoleList).ToArray();
            string[] rolesUnchanged = oldRoleList.Except(rolesToRemove).ToArray();
            
            if (rolesToAdd.Length > 0)
                Roles.AddUserToRoles(userName, rolesToAdd);
            if (rolesToRemove.Length > 0)
                Roles.RemoveUserFromRoles(userName, rolesToRemove);

            if (rolesToAdd.Length > 0 || rolesToRemove.Length > 0)
                RoleOverviewAdapter.SendUpdatedPermissionsEmail(userName, rolesToAdd, rolesToRemove, rolesUnchanged, false);
        }
    }
}

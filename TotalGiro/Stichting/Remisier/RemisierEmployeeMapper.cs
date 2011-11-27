using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using NHibernate.Linq;

namespace B4F.TotalGiro.Stichting.Remisier
{
    public static class RemisierEmployeeMapper
    {
        public static IRemisierEmployee GetRemisierEmployee(IDalSession session, int remisierEmployeeId)
        {
            //return (IRemisierEmployee)session.Session.Linq<RemisierEmployee>()
            //                                         .FirstOrDefault(re => re.Key == remisierEmployeeId);
            var step1 = session.Session.Linq<IRemisierEmployee>()
                    .Where(r => r.Key == remisierEmployeeId)
                    .AsQueryable();

            if (step1.Count() == 1)
                return step1.First();
            else
                return null;
        }

        public static List<IRemisierEmployee> GetRemisierEmployees(IDalSession session)
        {
            return GetRemisierEmployees(session, 0);
        }

        public static List<IRemisierEmployee> GetRemisierEmployees(IDalSession session, int remisierId)
        {
            return GetRemisierEmployees(session, remisierId, ActivityReturnFilter.Active);
        }

        public static List<IRemisierEmployee> GetRemisierEmployees(IDalSession session, int remisierId, ActivityReturnFilter activityFilter)
        {
            IQueryable<RemisierEmployee> remisierEmployees = session.Session.Linq<RemisierEmployee>()
                                                                            .Where(re => re.Remisier.IsActive);

            if (remisierId != 0)
                remisierEmployees = remisierEmployees.Where(re => re.Remisier.Key == remisierId);

            if (activityFilter == ActivityReturnFilter.Active)
                remisierEmployees = remisierEmployees.Where(re => re.IsActive);
            else if (activityFilter == ActivityReturnFilter.InActive)
                remisierEmployees = remisierEmployees.Where(re => !re.IsActive);

            return remisierEmployees.OrderBy(re => re.Employee.LastName)
                                    .Cast<IRemisierEmployee>()
                                    .ToList();
        }

        public static List<IRemisierEmployee> GetRemisierEmployees(IDalSession session, int[] remisierEmployeeIds)
        {
            IQueryable<RemisierEmployee> remisierEmployees = session.Session.Linq<RemisierEmployee>()
                                                                            .Where(re => re.IsActive && re.Remisier.IsActive);

            remisierEmployees = remisierEmployees.Where(re => remisierEmployeeIds.Contains(re.Key));

            return remisierEmployees.OrderBy(re => re.Employee.LastName)
                                    .Cast<IRemisierEmployee>()
                                    .ToList();
        }

        public static List<IRemisierEmployee> GetRemisierEmployees(
            IDalSession session, int assetManagerId, int remisierId, string remisierEmployeeName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            IQueryable<RemisierEmployee> remisierEmployees = session.Session.Linq<RemisierEmployee>()
                                                                            .Where(re => re.IsActive && re.Remisier.IsActive);

            if (assetManagerId != 0)
                remisierEmployees = remisierEmployees.Where(re => re.Remisier.AssetManager.Key == assetManagerId);

            if (remisierId != 0)
                remisierEmployees = remisierEmployees.Where(re => re.Remisier.Key == remisierId);

            if (!string.IsNullOrEmpty(remisierEmployeeName))
                remisierEmployees = remisierEmployees.Where(re => re.Employee.LastName.Contains(remisierEmployeeName));

            if (emailStatusYes && !emailStatusNo)
                remisierEmployees = remisierEmployees.Where(re => re.Employee.Email != null && re.Employee.Email != "");
            else if (!emailStatusYes && emailStatusNo)
                remisierEmployees = remisierEmployees.Where(re => re.Employee.Email == null || re.Employee.Email == "");

            if (passwordSent != null)
                remisierEmployees = remisierEmployees.Where(re => re.Login.PasswordSent == passwordSent);

            if (isLoginActive != null)
                remisierEmployees = remisierEmployees.Where(re => re.Login.IsActive == isLoginActive);

            IEnumerable<IRemisierEmployee> result = remisierEmployees.OrderBy(re => re.Employee.LastName)
                                                                     .Cast<IRemisierEmployee>()
                                                                     .ToList();

            // This must happen here, AFTER the IQueryable methods are evaluated (because before that, re.Login is always non-null)
            if (hasLogin == true)
                result = result.Where(re => re.Login != null);
            else if (hasLogin == false)
                result = result.Where(re => re.Login == null);

            return result.ToList();
        }
    }
}

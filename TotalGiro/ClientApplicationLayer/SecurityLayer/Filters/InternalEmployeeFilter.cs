using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    internal abstract class InternalEmployeeFilter : SecurityFilter
    {
        protected IInternalEmployeeLogin CurrentEmployeeLogin
        {
            get { return (IInternalEmployeeLogin)CurrentLogin; }
        }

        protected IManagementCompany CurrentManagementCompany
        {
            get { return GetNotNull(CurrentEmployeeLogin.Employer); }
        }

        public override IRemisier CurrentRemisier
        {
            get { throw new SecurityLayerException("Could not determine current remisier."); }
        }

        public override IRemisierEmployee CurrentRemisierEmployee
        {
            get { throw new SecurityLayerException("Could not determine current remisier employee."); }
        }

        protected IRemisierEmployee GetRemisierEmployee(int remisierEmployeeId)
        {
            IRemisierEmployee remisierEmployee = RemisierEmployeeMapper.GetRemisierEmployee(Session, remisierEmployeeId);
            if (remisierEmployee != null)
                return remisierEmployee;
            else
                throw new SecurityLayerException("Remisier Employee not authorized or not found.");
        }
    }
}

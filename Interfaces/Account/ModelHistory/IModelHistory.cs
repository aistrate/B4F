using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Accounts.ModelHistory
{
    public interface IModelHistory
    {
        int Key { get; set; }
        IAccountTypeCustomer Account { get; }
        ILifecycle Lifecycle { get; set; }
        IPortfolioModel ModelPortfolio { get; set; }
        int ModelPortfolioKey { get; }
        bool IsExecOnlyCustomer { get; set; }
        AccountEmployerRelationship EmployerRelationship { get; set; }
        DateTime ChangeDate { get; set; }
        DateTime EndDate { get; set;  }
        IInternalEmployeeLogin Employee { get; }
        bool Edit(IPortfolioModel model, bool isExecOnlyCustomer, AccountEmployerRelationship employerRelationship, IInternalEmployeeLogin employee, DateTime changeDate);
    }
}

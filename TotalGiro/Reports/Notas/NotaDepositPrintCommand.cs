using System;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Reports.Notas
{
    public class NotaDepositPrintCommand : NotaPrintCommand
    {
        public NotaDepositPrintCommand()
            : base()
        {
            NotaType = NotaReturnClass.NotaDeposit;
        }

        public override object GetNotaFields(INota nota)
        {
            INotaDeposit n = (INotaDeposit)nota;

            return new
            {
                n.Key,
                n.NotaNumber,
                n.TransactionDate,
                Account_Number =
                    n.Account.Number,
                Account_AccountOwner_CompanyName =
                    n.Account.AccountOwner.CompanyName,
                n.ModelPortfolioName,
                n.IsStorno,

                n.Formatter.DearSirForm,
                TotalValueAbs_DisplayString =
                    n.GrandTotalValue.Abs().DisplayString,
                n.TegenrekeningNumber,
                n.IsWithdrawalOneOff,
                n.IsWithdrawalPeriodic,
                n.IsWithdrawalTermination,
                n.IsDeposit,
                TransferFeeAbs_Quantity =
                    n.TransferFee != null ? n.TransferFee.Abs().Quantity : 0m,
                TransferFeeAbs_DisplayString =
                    n.TransferFee != null ? n.TransferFee.Abs().DisplayString : ""
            };
        }

        protected override string GetFileSuffix(INota[] notaGroup)
        {
            return (((INotaDeposit)notaGroup[0]).IsDeposit ? "Deposit" : "Withdrawal");
        }

        protected override void BeforeDataSetBuild(IDalSession session, INota[] notaGroup)
        {
            foreach (INotaDeposit nota in notaGroup)
                if (nota.IsWithdrawalPeriodic && (nota.TegenrekeningNumber == null || nota.TegenrekeningNumber == string.Empty))
                    throw new ApplicationException(
                        "Periodic Withdrawal Nota cannot be printed because Tegenrekening Number is missing on the Cash Transfer.");
        }
    }
}

using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Notas
{
    public class NotaDeposit : NotaGeneralOperationsBooking, INotaDeposit
    {
        #region Constructor

        private NotaDeposit() { }

        public NotaDeposit(ICashTransfer underlyingCashTransfer)
            : base(underlyingCashTransfer)
        {
        }

        #endregion

        #region Props

        private IJournalEntryLine mainTransferLine
        {
            get { return ((ICashTransfer)UnderlyingBooking).MainTransferLine ; }
        }

        public virtual string TegenrekeningNumber
        {
            get { return mainTransferLine.TegenrekeningNumber; }
        }

        public virtual CashTransferDetailTypes CashTransferDetailType
        {
            get 
            {
                IJournalEntryLine line = mainTransferLine;
                CashTransferDetailTypes detailType = line.CashTransferDetailType;
                if (detailType == CashTransferDetailTypes.None && line.IsCashTransfer)
                {
                    if (line.GLAccount.CashTransferType == CashTransferTypes.Deposit)
                        detailType = CashTransferDetailTypes.Deposit;
                    else
                        detailType = CashTransferDetailTypes.WithdrawalOneOff;
                }
                return detailType;
            }
        }

        public virtual bool IsWithdrawalOneOff
        {
            get
            {
                return (CashTransferDetailType == CashTransferDetailTypes.WithdrawalOneOff);
            }
        }

        public virtual bool IsWithdrawalPeriodic
        {
            get
            {
                return (CashTransferDetailType == CashTransferDetailTypes.WithdrawalPeriodic);
            }
        }

        public virtual bool IsWithdrawalTermination
        {
            get
            {
                return (CashTransferDetailType == CashTransferDetailTypes.WithdrawalTermination);
            }
        }

        public virtual bool IsDeposit
        {
            get { return (mainTransferLine.GLAccount.CashTransferType == CashTransferTypes.Deposit); }
        }

        public virtual Money TransferFee
        {
            get { return ((ICashTransfer)UnderlyingBooking).TransferFee; }
        }

        #endregion

        #region Overriden Props

        public override string Title
        {
            get 
            {
                string title = "";
                if (mainTransferLine != null)
                {
                    if (mainTransferLine.GLAccount.IsDefaultWithdrawal)
                        title = "Onttrekking";
                    else if (mainTransferLine.GLAccount.IsDefaultDeposit)
                        title = "Storting";
                }
                return title;
            }
        }

        public override Money GrandTotalValue
        {
            get { return mainTransferLine.Balance; }
        }

        #endregion

    }
}

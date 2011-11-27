using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Orders.Transfers
{


    public class PositionTransfer : IPositionTransfer
    {
        public PositionTransfer()
        {

            this.transferDetails = new PositionTransferDetailCollection(this);
            //this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
        }

        public PositionTransfer(bool aIsInternal, IAccountTypeInternal accountA, bool bIsInternal,
            IAccountTypeInternal accountB, TransferType typeOfTransfer, Money transferAmount,
            DateTime transferDate)
            : this()
        {
            this.AIsInternal = aIsInternal;
            if (aIsInternal) this.AccountA = accountA;
            this.BIsInternal = bIsInternal;
            if (bIsInternal) this.AccountB = accountB;
            this.TypeOfTransfer = typeOfTransfer;
            this.TransferAmount = transferAmount;
            this.TransferDate = transferDate;
            this.TransferStatus = TransferStatus.New;
        }

        public int Key { get; set; }
        public IAccountTypeInternal AccountA { get; set; }
        public IAccountTypeInternal AccountB { get; set; }
        public bool AIsInternal { get; set; }
        public bool BIsInternal { get; set; }
        public bool Executed { get; set; }
        public TransferType TypeOfTransfer { get; set; }
        public Money TransferAmount { get; set; }
        public IPositionTransferPortfolio APortfolioBefore { get; set; }
        public IPositionTransferPortfolio BPortfolioBefore { get; set; }
        public IPositionTransferPortfolio APortfolioAfter { get; set; }
        public IPositionTransferPortfolio BPortfolioAfter { get; set; }
        public bool CanBeBiDirectional { get { return AIsInternal && BIsInternal; } }
        public bool PriceCanBeAltered { get { return !(AIsInternal && BIsInternal); } }
        public bool IsInitialised { get; set; }
        public ICurrency BaseCurrency
        {
            get
            {
                if (AIsInternal)
                    return this.AccountA.BaseCurrency;
                else
                    return this.AccountB.BaseCurrency;
            }
        }

        public string Reason { get; set; }
        public IInternalEmployeeLogin CreatedBy { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public string AccountNumberA { get { return (AIsInternal && AccountA != null) ? AccountA.Number : "External"; } }
        public string AccountNumberB { get { return (BIsInternal && AccountB != null) ? AccountB.Number : "External"; } }
        public string DescriptionAccountA { get { return string.Format("Overboeking naar {0}", this.AccountNumberB); } }
        public string DescriptionAccountB { get { return string.Format("Overboeking van {0}", this.AccountNumberA); } }
        public DateTime TransferDate
        {
            get
            {
                return this.transferDate.HasValue ? this.transferDate.Value : DateTime.MinValue;
            }
            set
            {
                this.transferDate = value;
            }
        }
        public bool IsEditable
        {
            get
            {
                return !((this.TransferStatus == TransferStatus.Executed) || (this.TransferStatus == TransferStatus.Cancelled));
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue;
            }
        }

        public IPositionTransferDetailCollection TransferDetails
        {
            get
            {
                IPositionTransferDetailCollection temp = (IPositionTransferDetailCollection)transferDetails.AsList();
                if (temp.ParentTransfer == null) temp.ParentTransfer = this;
                return temp;
            }
        }



        //public virtual ITransactionNTMCollection Transactions
        //{
        //    get
        //    {
        //        ITransactionNTMCollection temp = (ITransactionNTMCollection)transactions.AsList();
        //        if (temp.ParentTransfer == null) temp.ParentTransfer = this;
        //        return temp;
        //    }
        //}


        #region Privates

        private DateTime? creationDate;
        private DateTime? transferDate;

        private IDomainCollection<IPositionTransferDetail> transferDetails;

        #endregion
    }
}

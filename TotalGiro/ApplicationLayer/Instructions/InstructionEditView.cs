using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;

namespace B4F.TotalGiro.ApplicationLayer.Instructions
{
    #region helper classes

    [Serializable]
    public class RebalanceExclusionDetails : ISerializable
    {
        public RebalanceExclusionDetails(int componentKey, string componentName, ModelComponentType componentType)
        {
            this.ComponentKey = componentKey;
            this.ComponentName = componentName;
            this.ComponentType = componentType;
            Key = (ComponentType == ModelComponentType.Instrument ? "I" : "M") + ComponentKey.ToString();
        }

        public RebalanceExclusionDetails(IRebalanceExclusion exclusion)
        {
            Key = (exclusion.ComponentType == ModelComponentType.Instrument ? "I" : "M") + exclusion.ComponentKey.ToString();
            ComponentKey = exclusion.ComponentKey;
            ComponentName = exclusion.ComponentName;
            ComponentType = exclusion.ComponentType;
        }

        public RebalanceExclusionDetails(SerializationInfo info, StreamingContext ctxt)
        {
            //Get the values from info and assign them to the appropriate properties
            Key = (String)info.GetValue("Key", typeof(string));
            ComponentKey = (int)info.GetValue("ComponentKey", typeof(int));
            ComponentName = (String)info.GetValue("ComponentName", typeof(string));
            ComponentType = (ModelComponentType)info.GetValue("ComponentType", typeof(int));
        }

        public string Key { get; internal set; }
        public int ComponentKey { get; set; }
        public string ComponentName { get; set; }
        public ModelComponentType ComponentType { get; set; }

        internal IPortfolioModel Model { get; set; }
        internal ITradeableInstrument Instrument { get; set; }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Key", Key);
            info.AddValue("ComponentKey", ComponentKey);
            info.AddValue("ComponentName", ComponentName);
            info.AddValue("ComponentType", (int)ComponentType);
        }
    }

    #endregion
    
    public class InstructionEditView
    {
        public InstructionEditView()
        {
        }

        public InstructionEditView(int instructionId, int orderActionTypeID, DateTime executionDate, bool doNotChargeCommission, bool isEditable)
        {
            this.InstructionID = instructionId;
            this.OrderActionType = orderActionTypeID;
            this.ExecutionDate = executionDate;
            this.DoNotChargeCommission = doNotChargeCommission;
            this.IsEditable = isEditable;
            this.Exclusions = new List<RebalanceExclusionDetails>();
        }

        public InstructionEditView(int instructionId, DateTime withdrawalDate, DateTime executionDate, decimal withdrawalAmount, int counterAccountID, bool isEditable, string transferDescription, bool doNotChargeCommission)
        {
            this.InstructionID = instructionId;
            this.WithdrawalDate = withdrawalDate;
            this.ExecutionDate = executionDate;
            this.WithdrawalAmount = withdrawalAmount;
            this.CounterAccountID = counterAccountID;
            this.IsEditable = isEditable;
            this.TransferDescription = transferDescription;
            this.DoNotChargeCommission = doNotChargeCommission;
        }
        public int InstructionID
        {
            get { return instructionId; }
            set { instructionId = value; }
        }

        public int OrderActionType
        {
            get { return orderActionTypeID; }
            set { orderActionTypeID = value; }
        }

        public bool DoNotChargeCommission
        {
            get { return doNotChargeCommission; }
            set { doNotChargeCommission = value; }
        }

        public DateTime ExecutionDate
        {
            get { return executionDate; }
            set { executionDate = value; }
        }

        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; }
        }

        public DateTime WithdrawalDate
        {
            get { return withdrawalDate; }
            set { withdrawalDate = value; }
        }

        public decimal WithdrawalAmount
        {
            get { return withdrawalAmount; }
            set { withdrawalAmount = value; }
        }

        public int CounterAccountID
        {
            get { return counterAccountID; }
            set { counterAccountID = value; }
        }

        public List<RebalanceExclusionDetails> Exclusions { get; set; }

        public void AddExclusions(IRebalanceExclusionCollection exclusions)
        {
            if (exclusions != null && exclusions.Count > 0)
            {
                if (Exclusions == null)
                    Exclusions = new List<RebalanceExclusionDetails>();
                foreach (IRebalanceExclusion x in exclusions)
                    Exclusions.Add(new RebalanceExclusionDetails(x));
            }
        }

        public string TransferDescription { get; set; }

        private int instructionId;
        private int orderActionTypeID;
        private DateTime executionDate;
        private bool doNotChargeCommission;
        private bool isEditable;

        private DateTime withdrawalDate;
        private decimal withdrawalAmount;
        private int counterAccountID;

    }
}

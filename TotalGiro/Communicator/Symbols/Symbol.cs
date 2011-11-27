using System;
using System.Collections.Generic;
using B4F.TotalGiro.Communicator.ExternalInterfaces;

namespace B4F.TotalGiro.Communicator
{
	abstract public class Symbol : ISymbol
	{
		protected Symbol() {}

        public Symbol(IExternalInterface externalInterface, string externalSymbol)
		{
            this.ExternalInterface = externalInterface;
            this.ExternalSymbol = externalSymbol;
		}

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }
        
        public virtual string ExternalSymbol
		{
			get { return externalSymbol; }
			set { externalSymbol = value; }
		}

        public virtual IExternalInterface ExternalInterface
		{
            get { return externalInterface; }
            set { externalInterface = value; }
		}

		public override string ToString()
		{
			return this.ExternalSymbol.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is ISymbol)
                return this.ExternalInterface.Equals(((ISymbol)obj).ExternalInterface);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return this.Key.GetHashCode();
		}

		#region Private Variables

		private int key;
		private string externalSymbol;
        private IExternalInterface externalInterface;

		#endregion

	}
}

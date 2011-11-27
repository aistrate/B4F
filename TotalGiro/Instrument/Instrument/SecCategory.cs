using System;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Dal;
using System.Collections;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class holds the type of a security
    /// </summary>
	public class SecCategory : ISecCategory
	{
		public SecCategory() { }

        /// <summary>
        /// Get identifier
        /// </summary>
		public virtual SecCategories Key
		{
			get { return (SecCategories)key; }
			//set { key = value; }
		}

        /// <summary>
        /// Get/set Name
        /// </summary>
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

        /// <summary>
        /// Get/set description
        /// </summary>
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

        /// <summary>
        /// Get/set cash flag to distinguish Cash Funds from rest
        /// </summary>
		public virtual bool IsCash
		{
			get { return isCash; }
			set { isCash = value; }
		}

        /// <summary>
        /// Get/set default ordering route
        /// </summary>
		public virtual IRoute DefaultRoute
		{
			get { return route;	}
			set { route = value; }
		}

        public virtual bool IsSupported { get; set; }

        public virtual SecCategoryTypes SecCategoryType { get; set; }
        public virtual bool IsSecurity 
        {
            get { return SecCategoryType == SecCategoryTypes.Security; }
        }
        public virtual bool IsDerivative
        {
            get { return SecCategoryType == SecCategoryTypes.Derivative; }
        }

		#region OverRides


        /// <summary>
        /// Overridden method for convenience purpose
        /// </summary>
        /// <returns>Description of type</returns>
		public override string ToString()
		{
			return Description;
		}


        /// <summary>
        /// Overridden composition of hashcode 
        /// </summary>
        /// <returns>Hashcode</returns>
		public override int GetHashCode()
		{
			return this.Key.GetHashCode();
		}

		#endregion

		#region Equality


        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="lhs">First type</param>
        /// <param name="rhs">Second type</param>
        /// <returns>Flag</returns>
		public static bool operator ==(SecCategory lhs, SecCategory rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (lhs.Key == rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Overridden unequality operator
        /// </summary>
        /// <param name="lhs">First type</param>
        /// <param name="rhs">Secong type</param>
        /// <returns>Flag</returns>
		public static bool operator !=(SecCategory lhs, SecCategory rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				if (lhs.Key != rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Overridden equality method
        /// </summary>
        /// <param name="obj">Obect to compare with</param>
        /// <returns>Flag</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SecCategory))
			{
				return false;
			}
			return this == (SecCategory)obj;
		}

		#endregion

		#region Private Variables

		private int key;
		private string name;
		private string description;
		private bool isCash;
		private IRoute route;
        //private IDal dataAccess;

		#endregion


	}
}

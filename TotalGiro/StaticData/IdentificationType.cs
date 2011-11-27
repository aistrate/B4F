using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// Class representing Client identification types
    /// </summary>
	public class IdentificationType : IIdentificationType
	{
		public IdentificationType() {	}

        /// <summary>
        /// Initializing new Type Identification
        /// </summary>
        /// <param name="idType">Name</param>
		public IdentificationType(string idType)
		{
			this.idType = idType;
		}

        /// <summary>
        /// Get/set unique idtentifier
        /// </summary>
		public virtual int Key
		{
			get { return key; }
			set { key = value; }
		}

        /// <summary>
        /// Get/set ID Name
        /// </summary>
		public virtual string IdType
		{
			get { return idType; }
			set { idType = value; }
		}	

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
		public override string ToString()
		{
			return this.IdType != null ? this.IdType.ToString() : "";
		}


		#region PrivateVariables

		private int key;
		private string idType;

		#endregion

	}
}

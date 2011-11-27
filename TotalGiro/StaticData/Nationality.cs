using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// Class representing nationality
    /// </summary>
	public class Nationality: INationality
	{

		public Nationality() 	{	}

        /// <summary>
        /// Initializing nationality object
        /// </summary>
        /// <param name="description">Description of nationality</param>
		public Nationality(string description) 
		{
			this.description = description;
		}

        /// <summary>
        /// Get/set unique identifier
        /// </summary>
		public virtual int Key
		{
			get { return key; }
			set { key = value; }
		}

        /// <summary>
        /// Get/set description of nationality
        /// </summary>
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
		public override string ToString()
		{
			return Description.ToString();
		}


		#region PrivateVariables

		private int key;
		private string description;

		#endregion

		
    }
}

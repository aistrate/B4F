using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// Class representing nationality
    /// </summary>
	public class PandHouder : IPandHouder
	{

		public PandHouder() 	{	}


        public PandHouder(string name) 
		{
            this.name = name;
		}

		public virtual int Key
		{
			get { return key; }
			set { key = value; }
		}

        /// <summary>
        /// Get/set description of nationality
        /// </summary>
		public virtual string Name
		{
            get { return name; }
            set { name = value; }
		}

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
		public override string ToString()
		{
			return Name.ToString();
		}


		#region PrivateVariables

		private int key;
		private string name;

		#endregion

		
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Class holding identification information
    /// </summary>
	public class Identification : IIdentification
	{

		public Identification() {	}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.Identification">Identification</see> class.
        /// </summary>
        public Identification(IIdentificationType identificationType, DateTime validityPeriod, string number)
		{
			this.identificationType = identificationType;
			this.validityPeriod = validityPeriod;
			this.number = number;
		}

        /// <summary>
        /// Get/set identification object number
        /// </summary>
		public string Number
		{
			get { return number; }
			set { number = value; }
		}	

        /// <summary>
        /// Get/set time period identification is valid
        /// </summary>
		public DateTime ValidityPeriod
		{
			get {
                    if (validityPeriod.Year <= 1800)
                    {
                        validityPeriod = DateTime.MinValue;
                    }
                    return validityPeriod; 
                }
			set { validityPeriod = value; }
		}	

        /// <summary>
        /// Get/set identification type
        /// </summary>
		public IIdentificationType IdentificationType
		{
			get { return identificationType; }
			set { identificationType = value; }
		}

		private IIdentificationType identificationType;
		private DateTime validityPeriod = DateTime.MinValue;
		private string number;
	
	

	}
}

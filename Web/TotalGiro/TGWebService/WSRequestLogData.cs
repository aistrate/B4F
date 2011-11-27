using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.Web.WebService
{
    public class WSRequestLogData
    {
        private long key = 0;
        private string wsrequest = null;

        public WSRequestLogData(string request)
        {
            this.wsrequest = request;
        }

        #region Properties

        /// <summary>
        /// Key of the this entry in the requestdata log
        /// </summary>
        public long Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// The data that formed the request
        /// </summary>
        public string RequestData
        {
            get { return wsrequest; }
            set { wsrequest = value; }
        }

        #endregion
    }
}
        #endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace B4F.Web.WebControls
{
    public class DownloadAsXML : CompositeControl, INamingContainer
    {
        private void init()
        {
            if (!IsInitialized)
            {
                this.objectDataSource = new ObjectDataSource();
                IsInitialized = true;
            }
        }
        private ObjectDataSource objectDataSource
        {
            get
            {
                init();
                object obj = ViewState["objectDataSource"];
                return (obj == null) ? null : (ObjectDataSource)obj;
            }
            set
            {
                ViewState["objectDataSource"] = value;
            }
        }


        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public string SelectMethod
        {
            get
            {
                return this.objectDataSource.SelectMethod;
            }
            set
            {
                this.objectDataSource.SelectMethod = value;
            }
        }

        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public string TypeName
        {
            get
            {
                return this.objectDataSource.TypeName;
            }
            set
            {
                this.objectDataSource.TypeName = value;
            }
        }

        //[Browsable(true)]
        //[Themeable(true)]
        //[DefaultValue("")]
        //[Category("Behavior")]
        //public ParameterCollection SelectParameters
        //{
        //    get
        //    {
        //        return this.objectDataSource.SelectParameters;
        //    }
        //    set
        //    {
        //        this.objectDataSource.SelectParameters = value;
        //    }
        //}


        public bool IsInitialized
        {
            get
            {
                object obj = ViewState["IsInitialized"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["IsInitialized"] = value;
            }
        }


    }
}

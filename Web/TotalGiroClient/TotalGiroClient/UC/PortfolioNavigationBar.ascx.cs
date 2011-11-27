using System;
using System.ComponentModel;
using B4F.TotalGiro.ClientApplicationLayer.UC;

namespace B4F.TotalGiro.Client.Web.UC
{
    public partial class PortfolioNavigationBar : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Visible)
            {
                // This will prevent the ContactId value in Session state from expiring (becoming null)
                ContactId = ContactId;

                if (!IsDataBound)
                    DataBind();
            }
        }

        public override void DataBind()
        {
            lblContactFullName.Text = PortfolioNavigationBarAdapter.GetContactFullName(ContactId);
            IsDataBound = true;
        }

        protected bool IsDataBound
        {
            get
            {
                object b = ViewState["IsDataBound"];
                return (b == null ? false : (bool)b);
            }
            set { ViewState["IsDataBound"] = value; }
        }

        protected int ContactId
        {
            get
            {
                object i = Session["ContactId"];
                return (i == null ? 0 : (int)i);
            }
            set { Session["ContactId"] = value; }
        }

        [DefaultValue(true), Category("Behavior")]
        public bool ShowPortfolio
        {
            get { return viewStateBool("ShowPortfolio", true); }
            set
            {
                ViewState["ShowPortfolio"] = value;
                phdPortfolio.Visible = value;
            }
        }

        [DefaultValue(true), Category("Behavior")]
        public bool ShowCharts
        {
            get { return viewStateBool("ShowCharts", true); }
            set
            {
                ViewState["ShowCharts"] = value;
                phdCharts.Visible = value;
            }
        }

        [DefaultValue(true), Category("Behavior")]
        public bool ShowPlanner
        {
            get { return viewStateBool("ShowPlanner", true); }
            set
            {
                ViewState["ShowPlanner"] = value;
                phdPlanner.Visible = value;
            }
        }

        private bool viewStateBool(string key, bool defaultValue)
        {
            object b = ViewState[key];
            return (b == null) ? defaultValue : (bool)b;
        }
    } 
}

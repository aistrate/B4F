using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class DataMaintenance_AttachCommRuleToAccount : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
         base.OnInit(e);
         gvAccounts.DataBind();
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        gvAccounts.DataSourceID = "odsAccounts";
        gvAccounts.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Add an commission rule to an account";
            int contactID = 0;
            if (Session["contactid"] != null)
            {
                contactID = (int)Session["contactid"];
            }

            lblPerson.Text = "EGVL010016";
            if (Request.UrlReferrer != null && Session["ReferrerUrlSearchContact"] == null)
            {
                Session["ReferrerUrlSearchContact"] = Request.UrlReferrer.AbsolutePath;
            }
        }
    }

    protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int contactID = (int)gvAccounts.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "ADDCONTACT":
                        gvAccounts.SelectedIndex = -1;
                        Session["addcontactid"] = contactID;
                        if (Session["ReferrerUrlSearchContact"] != null)
                        {
                            string qStr = Session["ReferrerUrlSearchContact"].ToString();
                            Session["ReferrerUrlSearchContact"] = null;
                            Response.Redirect(qStr);
                        }
                        break;
                }

            }
        }
        gvAccounts.SelectedIndex = -1;
    }
    
}

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
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Security;
using System.Text;

public partial class DataMaintenance_ContactOverview : System.Web.UI.Page
{
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Contacts Overview";
            gvContactOverview.Sort("CurrentNAW_Name", SortDirection.Ascending);

            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                pnlButtons.Visible = true;

            //if (Request.QueryString.Count > 0)
            //{
            //    if (Request.QueryString["AccountName"] != null)
            //        ctlAccountFinder.AccountName = Request.QueryString["AccountName"];
            //    if (Request.QueryString["AccountNumber"] != null)
            //        ctlAccountFinder.AccountNumber = Request.QueryString["AccountNumber"];
            //}
        }
    }

    protected void gvContactOverview_DataBinding(object sender, EventArgs e)
    {
        UserHasEditRights = SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit");
    }
    
    protected void gvContactOverview_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvContactOverview.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int key = (int)gvContactOverview.SelectedDataKey.Value;
                gvContactOverview.SelectedIndex = -1;
                string contactType = ContactOverviewAdapter.GetContactType(key);

                switch (e.CommandName.ToUpper())
                {
                    case "EDIT":
                        Session["contactid"] = key;

                        if ((contactType.ToUpper()).Equals(ContactTypeEnum.Person.ToString().ToUpper()))
                        {                           
                            Response.Redirect("Person.aspx");
                        }
                        else if ((contactType.ToUpper()).Equals(ContactTypeEnum.Company.ToString().ToUpper()))
                        {
                            Response.Redirect("Company.aspx");
                        }
                        break;
                    
                }
            }
        }

        gvContactOverview.SelectedIndex = -1;
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        pnlContacts.Visible = true;
       
        gvContactOverview.EditIndex = -1;
        gvContactOverview.DataBind();
    }
    
    protected void btnAddPerson_Click(object sender, EventArgs e)
    {
        // Response.Redirect("Person.aspx?id=0");
        Session["contactid"] = 0;
        Response.Redirect("Person.aspx");
    }

    protected void btnAddCompany_Click(object sender, EventArgs e)
    {
        Session["contactid"] = 0;
        Response.Redirect("Company.aspx");
    }
    
    protected void odsContactOverview_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        GridViewRow tableRow = gvContactOverview.Rows[gvContactOverview.EditIndex];
        DropDownList ddlIsActive = (DropDownList)tableRow.FindControl("ddlIsActive");
        e.InputParameters.Add("status", Convert.ToBoolean(ddlIsActive.SelectedValue));

        gvContactOverview.SelectedIndex = tableRow.RowIndex+1;
        int key = (int)gvContactOverview.SelectedDataKey.Value;
        e.InputParameters["Key"] = key;
    }

    protected bool UserHasEditRights = false;

}

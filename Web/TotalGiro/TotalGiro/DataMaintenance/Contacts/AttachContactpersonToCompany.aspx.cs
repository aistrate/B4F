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

public partial class DataMaintenance_AttachContactpersonToCompany : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
         base.OnInit(e);
         ctlContactFinder.Search += new EventHandler(ctlContactFinder_Search);
    }

    protected void ctlContactFinder_Search(object sender, EventArgs e)
    {
        pnlContactOverview.Visible = true;
        gvContactOverview.EditIndex = -1;
        gvContactOverview.Sort("CurrentNAW_Name", SortDirection.Ascending);

        gvContactOverview.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Attach contactperson to company";

            Utility.CreatePrevPageSession();

            int contactID = 0;
            if (Session["contactid"] != null)
            {
                contactID = (int)Session["contactid"];
            }

            lblCompany.Text = CompanyEditAdapter.GetCompanyName(contactID);
        }
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

                int persID = (int)gvContactOverview.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "ADDCONTACT":
                        gvContactOverview.SelectedIndex = -1;
                        CompanyContactPersonEditAdapter.AddContactPerson(int.Parse(Session["contactid"].ToString()), persID);
                        Utility.NavigateToPrevPageSessionIfAny();
                        break;
                }

            }
        }
        gvContactOverview.SelectedIndex = -1;
    }

    protected void btnAddNewPerson_Click(object sender, EventArgs e)
    {
        string companyId = Session["contactid"].ToString();
        Session["contactid"] = 0;
        string qStr = QueryStringModule.Encrypt(string.Format("IsCompanyContact=1&CompanyID={0}", companyId));
        Response.Redirect(string.Format("Person.aspx{0}", qStr));
    }
}

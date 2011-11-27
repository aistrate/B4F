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
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class DataMaintenance_AttachCompanyToContactperson : System.Web.UI.Page
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
        gvContactOverview.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Attach company to (contact)person";

            Utility.CreatePrevPageSession();
            int contactID = 0;
            if (Session["contactid"] != null)
            {
                contactID = (int)Session["contactid"];
            }
            
            
            lblPerson.Text = PersonEditAdapter.GetPersonName(contactID);

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

                int companyID = (int)gvContactOverview.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "ADDCONTACT":
                        gvContactOverview.SelectedIndex = -1;
                        CompanyContactPersonEditAdapter.AddContactPerson(companyID, int.Parse(Session["contactid"].ToString()));
                        Utility.NavigateToPrevPageSessionIfAny();
                        break;
                }

            }
        }
        gvContactOverview.SelectedIndex = -1;
    }
    
}

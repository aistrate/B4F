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
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Security;
using System.Drawing;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.Remisers;

public partial class RemisierOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Remisiers Overview";
            gvRemisiers.Sort("Name", SortDirection.Ascending);


            if (!RemisierAdapter.IsLoggedInAsAssetManager())
            {
                mvwAssetManager.ActiveViewIndex = 1;
                ddlAssetManager.DataBind();
                ddlAssetManager.SelectedIndex = -1;
            }
            else
            {
                int managementCompanyID = 0;
                string companyName = "";

                RemisierAdapter.GetCurrentManagmentCompany(ref companyName, ref managementCompanyID);
                hfAssetMAnagerID.Value = Convert.ToString(managementCompanyID);
                lblAssetManager.Text = companyName;
                mvwAssetManager.ActiveViewIndex = 0;
                lblAssetManager.Text = companyName;
            }
        }
        lblErrorMessage.Text = "";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvRemisiers.Visible = true;
    }

    protected void gvRemisiers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvRemisiers.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int remisierId = (int)gvRemisiers.SelectedDataKey.Value;
                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITREMISIER":
                            Session["RemisierID"] = remisierId;
                            Response.Redirect("Remisier.aspx");
                            break;
                        case "DELETEREMISIER":
                            if (RemisierAdapter.DeleteRemisier(remisierId))
                                gvRemisiers.DataBind();
                            break;
                        case "VIEWEMPLOYEES":
                            gvEmployees.Visible = true;
                            btnCreateEmployee.Visible = true;
                            btnHideEmployees.Visible = true;
                            btnHideEmployees.Focus();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvEmployees.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int employeeID = (int)gvEmployees.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITEMPLOYEE":
                            // encrypt the contents of token 
                            string qStr = QueryStringModule.Encrypt(string.Format("RemisierID={0}&RemisierEmployeeID={1}", Utility.GetCurrentRowKey(gvRemisiers), employeeID));
                            Response.Redirect(string.Format("RemisierEmployee.aspx{0}", qStr));
                            break;
                        case "DELETEEMPLOYEE":
                            if (RemisierAdapter.DeleteEmployee(employeeID))
                                gvEmployees.DataBind();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnHideEmployees_Click(object sender, EventArgs e)
    {
        try
        {
            gvEmployees.Visible = false;
            btnCreateEmployee.Visible = false;
            btnHideEmployees.Visible = false;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("Remisier.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateEmployee_Click(object sender, EventArgs e)
    {
        try
        {
            if (gvRemisiers.SelectedIndex > -1)
            {
                string qStr = QueryStringModule.Encrypt(string.Format("RemisierID={0}", Utility.GetCurrentRowKey(gvRemisiers)));
                Response.Redirect(string.Format("RemisierEmployee.aspx{0}", qStr));
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

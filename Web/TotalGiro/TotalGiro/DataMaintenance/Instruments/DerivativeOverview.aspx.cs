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

public partial class DerivativeOverview : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Derivatives Overview";
            gvInstruments.Sort("Name", SortDirection.Ascending);
            gvOptionSeries.Sort("SortOrder", SortDirection.Descending);
            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
            {
                pnlCreation.Visible = true;
                btnCreateTurbo.Visible = true;
            }
        }
        lblErrorMessage.Text = "";
    }

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        gvInstruments.EditIndex = -1;
        gvInstruments.DataBind();
        gvInstruments.Visible = true;
    }

    protected void gvInstruments_DataBinding(object sender, EventArgs e)
    {
        UserHasEditRights = SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit");
    }
    
    protected void gvInstruments_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string commName = e.CommandName.ToUpper();
        GridView gvView = (GridView)sender;
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            int key = Utility.GetCurrentRowKey((LinkButton)e.CommandSource, false);
            if (key > int.MinValue)
            {
                switch (commName)
                {
	                case "EDITINSTRUMENT":
                        Session["DerivativeMasterID"] = key;
                        Response.Redirect("DerivativeMaster.aspx");
                        break;
	                case "EDITTURBO":
                        Session["InstrumentID"] = key;
                        Response.Redirect("Turbo.aspx");
                        break;
	                case "EDITOPTION":
                        Session["InstrumentID"] = key;
                        Response.Redirect("Option.aspx");
                        break;
                    case "VIEWSERIES":
                        SecCategories secCategory = (SecCategories)int.Parse((string)e.CommandArgument);
                        switch (secCategory)
                        {
                            case SecCategories.Option:
                                mlvSeries.ActiveViewIndex = 0;
                                break;
                            case SecCategories.Turbo:
                                mlvSeries.ActiveViewIndex = 1;
                                break;
                        }
                        mlvSeries.Visible = true;
                        break;
                }
            }
        }
    }

    protected void btnCreateDerivativeMaster_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlSecCategory.SelectedIndex == 0)
                lblErrorMessage.Text = "Please select a seccategory first.";
            else
            {
                string qStr = QueryStringModule.Encrypt(string.Format("SecCategory={0}", ddlSecCategory.SelectedValue));
                Response.Redirect(string.Format("DerivativeMaster.aspx{0}", qStr));
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateTurbo_Click(object sender, EventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("DerivativeMasterID={0}", gvInstruments.SelectedValue));
            Response.Redirect(string.Format("Turbo.aspx{0}", qStr));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateOption_Click(object sender, EventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("DerivativeMasterID={0}", gvInstruments.SelectedValue));
            Response.Redirect(string.Format("Option.aspx{0}", qStr));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected bool UserHasEditRights = false;

}

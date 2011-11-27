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

public partial class SecurityOverview : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Instruments Overview";
            gvInstruments.Sort("DisplayName", SortDirection.Ascending);

            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                pnlCreation.Visible = true;
        }
        lblErrorMessage.Text = "";
    }

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        gvInstruments.EditIndex = -1;
        gvInstruments.DataBind();
        gvInstruments.Visible = true;
        gvStockDividendInstruments.Visible = false;
    }

    protected void gvInstruments_DataBinding(object sender, EventArgs e)
    {
        UserHasEditRights = SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit");
    }

    protected void gvInstruments_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                gvInstruments.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                string commName = e.CommandName.ToUpper();
                int instrumentid = (int)gvInstruments.SelectedDataKey.Value;
                switch (commName)
                {
                    case "EDITINSTRUMENT":
                        gvStockDividendInstruments.Visible = false;
                        Session["instrumentid"] = instrumentid;
                        Response.Redirect("Security.aspx");
                        break;
                    case "VIEWPRICES":
                        Session["instrumentid"] = instrumentid;
                        Response.Redirect("~/DataMaintenance/Prices/InstrumentPriceUpdate.aspx");
                        break;
                    case "SHOWSTOCKDIV":
                        gvStockDividendInstruments.Visible = true;
                        gvStockDividendInstruments.DataBind();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    protected void lbtnEditStockDiv_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int instrumentId = int.Parse((string)e.CommandArgument);
            gvStockDividendInstruments.EditIndex = Utility.FindGridRowIndex(gvStockDividendInstruments, instrumentId);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvStockDividendInstruments_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        if (e.Exception != null)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(e.Exception);
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlSecCategory.SelectedIndex == 0)
                lblErrorMessage.Text = "Please select a seccategory first.";
            else
            {
                string qStr = QueryStringModule.Encrypt(string.Format("SecCategory={0}", ddlSecCategory.SelectedValue));
                Response.Redirect(string.Format("Security.aspx{0}", qStr));
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected bool UserHasEditRights = false;

}

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
using System.IO;
using B4F.TotalGiro.ApplicationLayer.Orders.AssetManager;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments;

public partial class AssetManagerInstrumentSelection : System.Web.UI.Page
{
    public bool IsLoggedInAsAssetManager
    {
        get
        {
            object b = ViewState["IsLoggedInAsAssetManager"];
            if (b == null)
                b = AccountFinderAdapter.IsLoggedInAsAssetManager();
            return ((b == null) ? true : (bool)b);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
		{
			((EG)this.Master).setHeaderText = "Asset Manager - Instruments Mapping";

            if (!IsLoggedInAsAssetManager)
            {
                pnlAssetManager.Visible = true;
                mvwAssetManager.ActiveViewIndex = 1;
                ddlAssetManager.DataBind();
            }
            //else
            //{
            //    mvwAssetManager.ActiveViewIndex = 0;
            //    lblAssetManager.Text = AccountFinderAdapter.GetCurrentManagmentCompanyName();
            //}

            gvUnMappedInstruments.Sort("DisplayName", SortDirection.Ascending);
            gvMappedInstruments.Sort("Instrument_DisplayName", SortDirection.Ascending);
        }
        lblErrorMessage.Text = string.Empty;
    }

    protected void gvMappedInstruments_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            GridViewRow editingRow = (gvMappedInstruments.EditIndex >= 0 ? gvMappedInstruments.Rows[gvMappedInstruments.EditIndex] : null);
            if (editingRow != null)
            {
                DropDownList ddlAssetClass = (DropDownList)Utility.FindControl(editingRow, "ddlAssetClass");
                DropDownList ddlRegionClass = (DropDownList)Utility.FindControl(editingRow, "ddlRegionClass");
                DropDownList ddlInstrumentsCategories = (DropDownList)Utility.FindControl(editingRow, "ddlInstrumentsCategories");
                DropDownList ddlSectorClass = (DropDownList)Utility.FindControl(editingRow, "ddlSectorClass");
                DecimalBox dbMaxWithdrawalAmountPercentage = (DecimalBox)Utility.FindControl(editingRow, "dbMaxWithdrawalAmountPercentage");

                e.NewValues["assetClassId"] = Convert.ToInt32(ddlAssetClass.SelectedValue);
                e.NewValues["regionClassId"] = Convert.ToInt32(ddlRegionClass.SelectedValue);
                e.NewValues["instrumentsCategoryId"] = Convert.ToInt32(ddlInstrumentsCategories.SelectedValue);
                e.NewValues["sectorClassId"] = Convert.ToInt32(ddlSectorClass.SelectedValue);
                e.NewValues["maxWithdrawalAmountPercentage"] = dbMaxWithdrawalAmountPercentage.Value;
            }
        }
        catch (Exception ex)
        {
            e.Cancel = true;
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

	//Map
    protected void btnMapInstruments_Click(object sender, EventArgs e)
	{
        try
        {
            int assetManagerId = 0;
            if (!IsLoggedInAsAssetManager)
                assetManagerId = Convert.ToInt32(ddlAssetManager.SelectedValue);
            AssetManagerInstrumentSelectionAdapter.CreateInstrumentMapping(assetManagerId, gvUnMappedInstruments.GetSelectedIds());

            gvUnMappedInstruments.ClearSelection();
            gvUnMappedInstruments.DataBind();
            gvMappedInstruments.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
	}

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            gvMappedInstruments.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void rblActivityFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnRefresh_Click(sender, e);
    }
}

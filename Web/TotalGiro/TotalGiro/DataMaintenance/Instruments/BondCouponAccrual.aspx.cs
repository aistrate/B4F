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
using B4F.TotalGiro.ApplicationLayer.BackOffice;

public partial class BondCouponAccrual : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
        if (!IsPostBack)
		{
            ((EG)this.Master).setHeaderText = "Bonds Coupon Accrual Overview";

            gvCoupons.Sort("InstrumentName", SortDirection.Ascending);
        }
        lblErrorMessage.Text = string.Empty;
    }

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        gvCoupons.EditIndex = -1;
        gvCoupons.DataBind();
        gvCoupons.Visible = true;
    }

    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        try
        {
            BondCouponAccrualAdapter.ProcessBondPositions(DateTime.Today);
            if (gvCoupons.Visible)
                gvCoupons.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCoupons_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            gvCouponPaymentDetails.Visible = true;
            Utility.ScrollToBottom(hdnScrollToBottom);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCouponPaymentDetails_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            gvCalculations.Visible = true;
            Utility.ScrollToBottom(hdnScrollToBottom);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

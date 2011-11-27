using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Security;
using System.Text;
using B4F.TotalGiro.Instruments;

public partial class DataMaintenance_Turbo : System.Web.UI.Page
{
    public int InstrumentID
    {
        get
        {
            object b = ViewState["InstrumentID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["InstrumentID"] = value;
        }
    }

    public int DerivativeMasterID
    {
        get
        {
            object b = ViewState["DerivativeMasterID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["DerivativeMasterID"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            elbErrorMessage.Text = "";
            if (!IsPostBack)
            {
                Utility.DisablePageCaching();
                Utility.AlertSaveMessage();
                ((EG)this.Master).setHeaderText = "Edit Turbo Serie";

                if (SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                    bntSave.Enabled = true;

                DerivativeMasterID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "DerivativeMasterID");
                if (DerivativeMasterID != 0)
                {
                    ((EG)this.Master).setHeaderText = "Create New Turbo Serie";
                    setDefault();
                }

                if (Session["instrumentid"] != null)
                {
                    InstrumentID = (int)Session["instrumentid"];
                    Session["instrumentid"] = null;
                    loadRecord(InstrumentID);
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    private void setDefault()
    {
        rblSign.SelectedValue = ((int)B4F.TotalGiro.Accounts.Portfolios.IsLong.Long).ToString();
    }

    private void loadRecord(int id)
    {
        if (id > 0)
        {
            TurboDetails turbo = InstrumentEditAdapter.GetTurboDetails(id);
            if (turbo != null)
            {
                tbDerivativeName.Text = turbo.InstrumentName;
                tbISIN.Text = turbo.ISIN;
                rblSign.SelectedValue = ((int)turbo.Sign).ToString();
                dbStopLoss.Value = turbo.StopLoss;
                dbLeverage.Value = turbo.Leverage;
                dbFinanceLevel.Value = turbo.FinanceLevel;
                dbRatio.Value = turbo.Ratio;
            }
        }
    }

    protected void bntSave_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            bool valid = Page.IsValid;
            int instrumentID = InstrumentID;
            bool blnSaveSuccess = false;

            if (valid)
            {
                TurboDetails turboDetails = new TurboDetails();

                turboDetails.DerivativeMasterID = DerivativeMasterID;
                turboDetails.InstrumentName = tbDerivativeName.Text;
                turboDetails.ISIN = tbISIN.Text;
                turboDetails.Sign = (B4F.TotalGiro.Accounts.Portfolios.IsLong)Convert.ToInt32(rblSign.SelectedValue);
                turboDetails.StopLoss = dbStopLoss.Value;
                turboDetails.Leverage = dbLeverage.Value;
                turboDetails.FinanceLevel = dbFinanceLevel.Value;
                turboDetails.Ratio = Convert.ToInt16(dbRatio.Value);

                InstrumentEditAdapter.SaveTurbo(ref instrumentID, ref blnSaveSuccess, turboDetails);
                Session["instrumentid"] = instrumentID;
                Response.Redirect("Turbo.aspx");
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }
}
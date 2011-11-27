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

public partial class DataMaintenance_Option : System.Web.UI.Page
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
                ((EG)this.Master).setHeaderText = "Edit Option Serie";

                if (SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                    bntSave.Enabled = true;

                DerivativeMasterID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "DerivativeMasterID");
                if (DerivativeMasterID != 0)
                {
                    ((EG)this.Master).setHeaderText = "Create New Option Serie";
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
        rblOptionType.SelectedValue = ((int)OptionTypes.Call).ToString();
    }

    private void loadRecord(int id)
    {
        if (id > 0)
        {
            OptionDetails option = InstrumentEditAdapter.GetOptionDetails(id);
            if (option != null)
            {
                tbDerivativeName.Text = option.InstrumentName;
                //tbISIN.Text = option.ISIN;
                rblOptionType.SelectedValue = ((int)option.OptionType).ToString();
                dbStrikePrice.Value = option.StrikePrice;
                //mlvExpiry.ActiveViewIndex = 0;
                //ucExpiryDate.SelectedDate = option.ExpiryDate;
                ppExpiry.SelectedYear = option.ExpiryDate.Year;
                ppExpiry.SelectedMonth = option.ExpiryDate.Month;
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
                OptionDetails optionDetails = new OptionDetails();

                optionDetails.DerivativeMasterID = DerivativeMasterID;
                optionDetails.InstrumentName = null;
                //optionDetails.ISIN = tbISIN.Text;
                optionDetails.OptionType = (OptionTypes)Convert.ToInt32(rblOptionType.SelectedValue);
                optionDetails.StrikePrice = dbStrikePrice.Value;
                //if (mlvExpiry.ActiveViewIndex == 0)
                    optionDetails.SetExpiryDate(ppExpiry.SelectedPeriod);
                //else
                //    optionDetails.ExpiryDate = ucExpiryDate.SelectedDate;

                InstrumentEditAdapter.SaveOption(ref instrumentID, ref blnSaveSuccess, optionDetails);
                Session["instrumentid"] = instrumentID;
                Response.Redirect("Option.aspx");
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }
}
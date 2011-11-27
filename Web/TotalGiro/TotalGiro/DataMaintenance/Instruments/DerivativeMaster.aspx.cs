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

public partial class DataMaintenance_DerivativeMaster : System.Web.UI.Page
{
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

    public SecCategories SecCategory
    {
        get
        {
            object b = ViewState["SecCategory"];
            return ((b == null) ? SecCategories.Undefined : (SecCategories)b);
        }
        set
        {
            ViewState["SecCategory"] = value;
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

                if (SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                    bntSave.Enabled = true;

                SecCategory = (SecCategories)QueryStringModule.GetValueFromQueryString(Request.RawUrl, "SecCategory");
                if (SecCategory != SecCategories.Undefined)
                {
                    ((EG)this.Master).setHeaderText = "Create New " + SecCategory.ToString() + " Derivative Master";
                    setDefault();
                }

                if (Session["DerivativeMasterID"] != null)
                {
                    DerivativeMasterID = (int)Session["DerivativeMasterID"];
                    Session["DerivativeMasterID"] = null;
                    loadRecord(DerivativeMasterID);
                    ((EG)this.Master).setHeaderText = "Edit Derivative Master";
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void ddlUnderlyingSecCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlUnderlyingSecCategory.Items.Count > 0 && int.Parse(ddlUnderlyingSecCategory.SelectedValue) >= 0)
            ddlUnderlyingInstrument.DataBind();
    }

    private void setDefault()
    {
        dbDecimalPlaces.Value = 6M;
        dbContractSize.Value = 100M;
        ddCurrencyNominal.SelectedValue = "600";
    }

    private void loadRecord(int id)
    {
        if (id > 0)
        {
            DerivativeMasterDetails master = InstrumentEditAdapter.GetDerivativeMasterDetails(id);
            if (master != null)
            {
                tbDerivativeMasterName.Text = master.Name;
                ddlExchange.SelectedValue = master.ExchangeID.ToString();
                if (ddlUnderlyingSecCategory.Items == null || ddlUnderlyingSecCategory.Items.Count == 0)
                    ddlUnderlyingSecCategory.DataBind();
                ddlUnderlyingSecCategory.SelectedValue = ((int)master.UnderlyingSecCategory).ToString();
                if (master.UnderlyingSecCategory != SecCategories.Undefined)
                    ddlUnderlyingInstrument.DataBind();
                ddlUnderlyingInstrument.SelectedValue = master.UnderlyingID.ToString();
                if (master.NominalCurrencyID != int.MinValue)
                    ddCurrencyNominal.SelectedValue = master.NominalCurrencyID.ToString();
                dbDecimalPlaces.Value = master.DecimalPlaces;
                dbContractSize.Value = master.ContractSize;
                txtSymbol.Text = master.Symbol;
            }
        }
    }

    protected void bntSave_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            bool valid = Page.IsValid;
            int derivativeMasterID = DerivativeMasterID;
            bool blnSaveSuccess = false;

            if (valid)
            {
                DerivativeMasterDetails masterDetails = new DerivativeMasterDetails();

                masterDetails.Key = derivativeMasterID; ;
                masterDetails.SecCategory = SecCategory;
                masterDetails.ContractSize = Convert.ToInt32(dbContractSize.Value);
                masterDetails.DecimalPlaces = Convert.ToInt32(dbDecimalPlaces.Value);
                masterDetails.ExchangeID = Utility.GetKeyFromDropDownList(ddlExchange);
                masterDetails.Name = tbDerivativeMasterName.Text;
                masterDetails.Symbol = txtSymbol.Text;
                masterDetails.NominalCurrencyID = Utility.GetKeyFromDropDownList(ddCurrencyNominal);
                masterDetails.UnderlyingID = Utility.GetKeyFromDropDownList(ddlUnderlyingInstrument);
                masterDetails.UnderlyingSecCategory = (SecCategories)Utility.GetKeyFromDropDownList(ddlUnderlyingSecCategory);

                InstrumentEditAdapter.SaveDerivativeMaster(ref derivativeMasterID, ref blnSaveSuccess, masterDetails);
                Session["DerivativeMasterID"] = derivativeMasterID;
                Response.Redirect("DerivativeMaster.aspx");
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }
}
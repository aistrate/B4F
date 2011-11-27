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
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.CRM;

public partial class DataMaintenance_CounterAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErrorMess.Visible = false;
        lblMess.Text = "";

        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Counter Account";

            try
            {
                //Utility.CreatePrevPageSessionWithPageName("AttachCounterAccountToContact.aspx");

                // Parameter from Person.aspx || AttachCounterAccountToContact.aspx 
                if (Session["counteraccountid"] != null)
                    CounterAccountID = Convert.ToInt32(Session["counteraccountid"]);
                if (Session["contactid"] != null)
                    ContactID = Convert.ToInt32(Session["contactid"]);

                loadCounterAccountRecord();

            }
            catch (Exception ex)
            {
                pnlErrorMess.Visible = true;
                lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }
    }

    public int CounterAccountID
    {
        get
        {
            string id = hfCounterAccountID.Value;
            return ((id.Length > 0 && Util.IsNumeric(id)) ? Convert.ToInt32(id) : int.MinValue);
        }
        set { hfCounterAccountID.Value = value.ToString(); }
    }

    public int ContactID
    {
        get
        {
            string id = hfContactID.Value;
            return ((id.Length > 0 && Util.IsNumeric(id)) ? Convert.ToInt32(id) : int.MinValue);
        }
        set { hfContactID.Value = value.ToString(); }
    }

    private void loadCounterAccountRecord()
    {
        int counterAccountID = CounterAccountID;
        CounterAccountEditAdapter.CounterAccountDetails details;

        if (counterAccountID != int.MinValue)
        {
            CounterAccountEditAdapter.GetCounterAccountDetails(counterAccountID, out details);

            if (!AccountFinderAdapter.IsLoggedInAsStichting() && AccountFinderAdapter.GetCurrentManagmentCompanyId() != details.ManagementCompanyID)
            {
                throw new ApplicationException("This counter account is owned by another company.");
            }

            tbTegenrekNR.Text = details.TegenrekeningNr;
            tbTegenrekNameBank.Text = details.TegenrekNameBank;
            tbTegenrekTNV.Text = details.TegenrekTNV;
            ucBankAddress.Street = details.BankStreet;
            ucBankAddress.HouseNumber = details.BankHouseNumber;
            ucBankAddress.HouseNumberSuffix = details.BankHouseNumberSuffix;
            ucBankAddress.PostCode = details.BankPostalcode;
            ucBankAddress.City = details.BankCity;
            ucBankAddress.Country = details.BankCountryID.ToString();
            ddlBank.SelectedValue = details.BankID.ToString();
            chkUseElfProef.Checked = details.UseElfProef;

            chkIsPublic.Checked = details.IsPublic;
            chkIsPublic_CheckedChanged(null, null);
            ucBeneficiaryAddress.Street = details.BeneficiaryStreet;
            ucBeneficiaryAddress.HouseNumber = details.BeneficiaryHouseNumber;
            ucBeneficiaryAddress.HouseNumberSuffix = details.BeneficiaryHouseNumberSuffix;
            ucBeneficiaryAddress.PostCode = details.BeneficiaryPostalcode;
            ucBeneficiaryAddress.City = details.BeneficiaryCity;
            ucBeneficiaryAddress.Country = details.BeneficiaryCountryID.ToString();

            if (details.BankID == int.MinValue && details.TegenrekNameBank != "")
            {
                this.rblBankChoice.SelectedIndex = 1;
                rblBankChoice_SelectedIndexChanged(rblBankChoice, null);
            }

        }
    }

    protected void rblBankChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        chkUseElfProef.Checked = true;
        ddlBank.SelectedValue = int.MinValue.ToString();
        this.mvwBank.ActiveViewIndex = this.rblBankChoice.SelectedIndex;
        this.mvwBankValidator.ActiveViewIndex = this.rblBankChoice.SelectedIndex;
    }

    protected void btnSaveAccount_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                ContactTypeEnum contactType;
                CounterAccountEditAdapter.CounterAccountDetails details = new CounterAccountEditAdapter.CounterAccountDetails();

                details.TegenrekeningNr = tbTegenrekNR.Text;
                details.TegenrekTNV = tbTegenrekTNV.Text;
                if (!ucBankAddress.IsEmpty)
                {
                    details.BankStreet = ucBankAddress.Street;
                    details.BankHouseNumber = ucBankAddress.HouseNumber;
                    details.BankHouseNumberSuffix = ucBankAddress.HouseNumberSuffix;
                    details.BankPostalcode = ucBankAddress.PostCode;
                    details.BankCity = ucBankAddress.City;
                    details.BankCountryID = Convert.ToInt32(ucBankAddress.Country);
                }
                else
                    details.IsBankAddressEmpty = true;

                details.IsPublic = chkIsPublic.Checked;
                if (!ucBeneficiaryAddress.IsEmpty)
                {
                    details.BeneficiaryStreet = ucBeneficiaryAddress.Street;
                    details.BeneficiaryHouseNumber = ucBeneficiaryAddress.HouseNumber;
                    details.BeneficiaryHouseNumberSuffix = ucBeneficiaryAddress.HouseNumberSuffix;
                    details.BeneficiaryPostalcode = ucBeneficiaryAddress.PostCode;
                    details.BeneficiaryCity = ucBeneficiaryAddress.City;
                    details.BeneficiaryCountryID = Convert.ToInt32(ucBeneficiaryAddress.Country);
                }
                else
                    details.IsBeneficiaryAddressEmpty = true;

                if (mvwBank.ActiveViewIndex == 0)
                    details.BankID = Convert.ToInt32(ddlBank.SelectedValue);
                else
                    details.TegenrekNameBank = tbTegenrekNameBank.Text;

                int id = CounterAccountID;
                CounterAccountEditAdapter.SaveCounterAccount(ref id, ContactID, details, out contactType);

                Session["counteraccountid"] = null;
                //Utility.NavigateToPrevPageSessionIfAnyWithQueryString("?accountnumber=" + accountNumber);

                if (contactType == ContactTypeEnum.Company)
                    Response.Redirect("Company.aspx");
                else
                    Response.Redirect("Person.aspx");
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void chkIsPublic_CheckedChanged(object sender, EventArgs e)
    {
        pnlBeneficiaryAddress.Visible = chkIsPublic.Checked;
    }

    protected void ddlBank_SelectedIndexChanged(object sender, EventArgs e)
    {
        int bankID;
        if (int.TryParse(ddlBank.SelectedValue, out bankID))
        {
            CounterAccountEditAdapter.CounterAccountDetails details;
            if (CounterAccountEditAdapter.GetBankAddressDetails(bankID, out details) && details != null)
            {
                ucBankAddress.Street = details.BankStreet;
                ucBankAddress.HouseNumber = details.BankHouseNumber;
                ucBankAddress.HouseNumberSuffix = details.BankHouseNumberSuffix;
                ucBankAddress.PostCode = details.BankPostalcode;
                ucBankAddress.City = details.BankCity;
                ucBankAddress.Country = details.BankCountryID.ToString();
                chkUseElfProef.Checked = details.UseElfProef;
            }
        }
    }

    protected void cvTegenrekNr_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!(Util.IsNumeric(args.Value) || (args.Value.Substring(0, 1).ToUpper() == "P" && Util.IsNumeric(args.Value.Substring(1)))))
        {
            args.IsValid = false;
            return;
        }

        if (chkUseElfProef.Checked)
            args.IsValid = Util.PerformElfProefCheck(ElfProefCheckType.Bank, args.Value);
        else
            args.IsValid = true;
    }

    //private bool checkfields()
    //{
    //    ValidatorCollection validators = null;
    //    bool isValid = true;
    //    validators = Page.Validators;
    //    foreach (IValidator validator in validators)
    //    {
    //        if (!(validator is RequiredFieldValidator) && !(validator is RangeValidator))
    //        {
    //            validator.Validate();
    //            if (!validator.IsValid)
    //                isValid = false;
    //        }
    //    }
    //    return isValid;
    //}
}


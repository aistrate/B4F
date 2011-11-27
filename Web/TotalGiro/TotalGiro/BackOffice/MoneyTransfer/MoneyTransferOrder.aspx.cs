using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
//using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using B4F.TotalGiro.BackOffice.Orders;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using B4F.TotalGiro.ApplicationLayer.BackOffice;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class MoneyTransferOrder : System.Web.UI.Page
{
    enum BeneficiaryChoice
    {
        Predefined = 0,
        AccountCounterAccount
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        pnlAccounts.Visible = true;

        gvAccounts.EditIndex = -1;
        gvAccounts.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErrorMess.Visible = false;
        lblMess.Text = "";
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);

        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Create/Edit Money Transfer Order";
                DataBind();
                ResetInitialData();
                readUrlParameters();

                if (MoneyTransferOrderID > 0)
                    loadRecord(MoneyTransferOrderID);
                else
                    IsEdit = true;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void readUrlParameters()
    {
        string rawUrl = HttpContext.Current.Request.RawUrl;
        int indexQuery = rawUrl.IndexOf('?');
        if (indexQuery >= 0 && indexQuery < rawUrl.Length - 1)
        {
            NameValueCollection pageParams = QueryStringModule.ParseQueryString(rawUrl);
            foreach (string param in pageParams.AllKeys)
            {
                switch (param.ToLower())
                {
                    case "moneytransferorderid":
                        MoneyTransferOrderID = int.Parse(HttpUtility.UrlDecode(pageParams[param]));
                        break;
                    case "edit":
                        IsEdit = bool.Parse(HttpUtility.UrlDecode(pageParams[param]));
                        break;

                    default:
                        continue;
                }
            }
        }
    }

    private void loadRecord(int moneyTransferOrderID)
    {
        MoneyTransferOrderDetails details = MoneyTransferOrderAdapter.LoadMoneyTransferOrder(moneyTransferOrderID);
        if ((details != null) && (details.MoneyOrderID != null))
        {
            if (details.FromJournal != null) ddlWithdrawJournal.SelectedValue = details.FromJournal.ToString();
            if (details.NarDebet1 != null) tbNarDebet1.Text = details.NarDebet1;
            if (details.NarDebet2 != null) tbNarDebet2.Text = details.NarDebet2;
            if (details.NarDebet3 != null) tbNarDebet3.Text = details.NarDebet3;
            if (details.NarDebet4 != null) tbNarDebet4.Text = details.NarDebet4;
            if (details.AmountID != null) ddlCurrency.SelectedValue = details.AmountID.ToString();
            if (details.AmountQty != null) tbAmount.Text = details.AmountQty.ToString("####.00");

            if (details.SwiftAddress != null) tbSwiftAddress.Text = details.SwiftAddress;
            if (details.BenefBankAcctNr != null) tbBenefBankAcctNr.Text = details.BenefBankAcctNr;
            if (details.NarBenef1 != null) tbNarBenef1.Text = details.NarBenef1;
            if (details.NarBenef2 != null) tbNarBenef2.Text = details.NarBenef2;
            if (details.NarBenef3 != null) tbNarBenef3.Text = details.NarBenef3;
            if (details.NarBenef4 != null) tbNarBenef4.Text = details.NarBenef4;

            if (details.TransferDescription1 != null) tbDescription1.Text = details.TransferDescription1;
            if (details.TransferDescription2 != null) tbDescription2.Text = details.TransferDescription2;
            if (details.TransferDescription3 != null) tbDescription3.Text = details.TransferDescription3;
            if (details.TransferDescription4 != null) tbDescription4.Text = details.TransferDescription4;

            if (details.ProcessDate != null) ucProcessDate.SelectedDate = details.ProcessDate;
            if (details.Reference != null) tbReference.Text = details.Reference;
            this.ddlCostIndication.SelectedValue = ((int)details.CostIndication).ToString();
            btnSave.Enabled = details.IsEditable;
            updPnlUcChooseBeneficiary.Visible = details.IsEditable;
        }
    }

    private bool ResetInitialData()
    {
        StichtingDetails details = MoneyTransferOrderAdapter.GetStichtingDetails();
        ddlWithdrawJournal.SelectedValue = details.DefaultJournalID.ToString();
        tbNarDebet1.Text = details.NarDebet1;
        tbNarDebet2.Text = details.NarDebet2;
        tbNarDebet3.Text = details.NarDebet3;
        tbNarDebet4.Text = details.NarDebet4;
        ddlCurrency.SelectedValue = "600";
        setlblDebetAcctBank();
        return true;
    }

    private void setlblDebetAcctBank()
    {
        string journal = (ddlWithdrawJournal).SelectedValue;
        DataSet ds = MoneyTransferOrderAdapter.GetWithdrawalJournals();
        DataRow dr = (ds.Tables[0].Select("Key = " + journal))[0];
        lblDebetAcctBank.Text = (string)dr["BankAccountDescription"];
    }

    private bool checkValidation()
    {
        ValidatorCollection validators = null;
        bool isValid = true;
        validators = Page.Validators;
        foreach (IValidator validator in validators)
        {
            //if (!(validator is RequiredFieldValidator) && !(validator is RangeValidator))
            //{
            validator.Validate();
            if (!validator.IsValid)
                isValid = false;
            //}
        }
        return isValid;
    }

    protected void ddlWithdrawJournal_SelectedIndexChanged(object sender, EventArgs e)
    {
        setlblDebetAcctBank();
    }

    protected void btnPredefinedBeneficiary_Click(object sender, EventArgs e)
    {
        mvwBeneficiarySelect.ActiveViewIndex = (int)BeneficiaryChoice.Predefined;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkValidation())
            {
                bool existingRecord = (MoneyTransferOrderID != 0);
                if (saveRecord(false) && existingRecord)
                    Response.Redirect("ApproveMoneyTransfers.aspx");
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private bool saveRecord(bool ignoreWarning)
    {
        MoneyTransferOrderDetails mtod = new MoneyTransferOrderDetails();
        if (AccountID != 0) mtod.Accountid = AccountID;
        if (CounterAccountID != 0) mtod.CounterAccountID = CounterAccountID;
        mtod.AmountQty = Decimal.Parse(tbAmount.Text);
        mtod.AmountID = (int.Parse(ddlCurrency.SelectedValue));
        mtod.BenefBankAcctNr = tbBenefBankAcctNr.Text.Trim();
        mtod.FromJournal = (int.Parse(ddlWithdrawJournal.SelectedValue));
        mtod.NarBenef1 = tbNarBenef1.Text;
        mtod.NarBenef2 = tbNarBenef2.Text;
        mtod.NarBenef3 = tbNarBenef3.Text;
        mtod.NarBenef4 = tbNarBenef4.Text;
        mtod.NarDebet1 = tbNarDebet1.Text;
        mtod.NarDebet2 = tbNarDebet2.Text;
        mtod.NarDebet3 = tbNarDebet3.Text;
        mtod.NarDebet4 = tbNarDebet4.Text;
        mtod.SwiftAddress = (tbSwiftAddress.Text != null ? tbSwiftAddress.Text : "");
        mtod.TransferDescription1 = tbDescription1.Text;
        mtod.TransferDescription2 = tbDescription2.Text;
        mtod.TransferDescription3 = tbDescription3.Text;
        mtod.TransferDescription4 = tbDescription4.Text;
        mtod.ProcessDate = ucProcessDate.SelectedDate;
        mtod.MoneyOrderID = MoneyTransferOrderID;
        mtod.CostIndication = (IndicationOfCosts) int.Parse(this.ddlCostIndication.SelectedValue);

        var result = MoneyTransferOrderAdapter.SaveMoneyTransferOrder(mtod, ignoreWarning);
        if (!result.Item2 && !string.IsNullOrEmpty(result.Item3))
        {
            DialogMode = true;
            lblMess2.Text = result.Item3 + "<br/>Do you want to bypass this validation?";
        }
        MoneyTransferOrderID = result.Item1;
        return result.Item2;
    }

    protected void rblDialog_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblDialog.SelectedValue == "1")
        {
            bool existingRecord = (MoneyTransferOrderID != 0);
            if (saveRecord(true) && existingRecord)
                Response.Redirect("ApproveMoneyTransfers.aspx");
        }
        DialogMode = false;
    }

    private bool DialogMode
    {
        get { return (bool)ViewState["DialogMode"]; }
        set
        {
            pnlDialog.Visible = value;
            if (value)
            {
                rblDialog.SelectedIndex = -1;
                lblMess2.Text = "";
                rblDialog.Focus();
                Utility.ScrollToBottom(hdnScrollToBottom);
            }
            ViewState["DialogMode"] = value;
        }
    }

    public int MoneyTransferOrderID
    {
        get
        {
            return ((hfMoneyTransferOrderID.Value.Length == 0) ? 0 : int.Parse(hfMoneyTransferOrderID.Value));
        }
        set { hfMoneyTransferOrderID.Value = value.ToString(); }
    }

    public bool IsEdit
    {
        get { return btnSave.Visible; }
        set { btnSave.Visible = value; }
    }

    public int AccountID
    {
        get
        {
            return ((hfAccountid.Value.Length == 0) ? 0 : int.Parse(hfAccountid.Value));
        }
        set { hfAccountid.Value = value.ToString(); }
    }

    public int CounterAccountID
    {
        get
        {
            return ((hfCounterAccountID.Value.Length == 0) ? 0 : int.Parse(hfCounterAccountID.Value));
        }
        set { hfCounterAccountID.Value = value.ToString(); }
    }

    protected void btnAccountCounterAccount_Click(object sender, EventArgs e)
    {
        mvwBeneficiarySelect.ActiveViewIndex = (int)BeneficiaryChoice.AccountCounterAccount;
    }

    private void FillBeneficiaryDetails(int predefinedKey)
    {
        PredefinedBeneficiary pb = MoneyTransferOrderAdapter.GetPredefinedBeneficiary(predefinedKey);
        if (pb != null)
        {
            if (pb.SwiftAddress != null) this.tbSwiftAddress.Text = pb.SwiftAddress;
            if (pb.BenefBankAcctNr != null) this.tbBenefBankAcctNr.Text = pb.BenefBankAcctNr;
            if (pb.NarBenef1 != null) this.tbNarBenef1.Text = pb.NarBenef1;
            if (pb.NarBenef2 != null) this.tbNarBenef2.Text = pb.NarBenef2;
            if (pb.NarBenef3 != null) this.tbNarBenef3.Text = pb.NarBenef3;
            if (pb.NarBenef4 != null) this.tbNarBenef4.Text = pb.NarBenef4;
            if (pb.Description1 != null) this.tbDescription1.Text = pb.Description1;
            if (pb.Description2 != null) this.tbDescription2.Text = pb.Description2;
            if (pb.Description3 != null) this.tbDescription3.Text = pb.Description3;
            if (pb.Description4 != null) this.tbDescription4.Text = pb.Description4;
            this.ddlCostIndication.SelectedValue = ((int)pb.CostIndication).ToString();
        }

    }
    protected void gvPredefinedBeneficiary_SelectedIndexChanged(object sender, EventArgs e)
    {
        int predefinedKey = (int)(((GridView)sender).SelectedValue);
        pnlErrorMess.Visible = true;
        lblMess.Text = predefinedKey.ToString();
        FillBeneficiaryDetails(predefinedKey);
    }
    protected void customValAccountNumbers_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (ddlWithdrawJournal.SelectedItem.Text != tbBenefBankAcctNr.Text);
    }

    protected void customValBankAccountNumbers_ServerValidate(object source, ServerValidateEventArgs value)
    {
        if (tbSwiftAddress.Text.Length == 0)
            validateDutchAccountNumber((CustomValidator)source, value);
        else
            validateIbanAccountNumber((CustomValidator)source, value);

    }

    private void validateDutchAccountNumber(CustomValidator source, ServerValidateEventArgs value)
    {
        value.IsValid = true;
        //int counter;
        //string bankAcctNr = (string)tbBenefBankAcctNr.Text.Trim();
        //decimal nTotal = 0m;
        //decimal u;

        //if (bankAcctNr[0].ToString().ToUpper() == "P")
        //{
        //    value.IsValid = true; //We cannot validate Postbank accounts yet!!!!
        //    source.ErrorMessage = "We cannot validate Postbank accounts yet!!!!";
        //    return;
        //}

        //if ((bankAcctNr.Length < 9))
        //{
        //    value.IsValid = false;
        //    source.ErrorMessage = "The Beneficiary account number does not appear to be a Valid Dutch Bank account Number";
        //    return;
        //}
        //else if ((bankAcctNr.Length == 9) || (bankAcctNr.Length == 10))
        //{
        //    if (bankAcctNr.Length == 9)
        //    {
        //        bankAcctNr = "0" + bankAcctNr;
        //    }


        //    for (counter = 0; counter < 10; counter++)
        //    {
        //        nTotal = nTotal + Convert.ToDecimal(bankAcctNr.Substring(counter, 1)) * (10 - counter);
        //    }
        //    u = nTotal % 11;
        //    if (u != 0m)
        //    {
        //        value.IsValid = false;
        //        source.ErrorMessage = "The Beneficiary account number failed the 'elfproef' test.";
        //        return;
        //    }
        //    else
        //        value.IsValid = true;
        //}
        //else
        //{
        //    value.IsValid = false;
        //    source.ErrorMessage = "The Beneficiary account number does not appear to be a Valid Dutch Bank account Number";
        //    return;
        //}
    }

    private void validateIbanAccountNumber(CustomValidator source, ServerValidateEventArgs value)
    {
        value.IsValid = true;
        //source.ErrorMessage = "If Swift Address is provided , then a Valid Iban Number is Compulsory.";
        //string input = Regex.Replace(this.tbBenefBankAcctNr.Text.ToUpper(), @"\s", "");
        //if ((input.Length == 20) && char.IsLetter(input[0]) && char.IsLetter(input[1]))
        //{
        //    input = input.Substring(4, (input.Length - 4)) + (Convert.ToInt16(input[0]) - 55).ToString() + (Convert.ToInt16(input[1]) - 55).ToString() + input.Substring(2, 2);

        //    int len = Convert.ToInt32((Math.Ceiling(input.Length / 6m)));
        //    input = input.PadRight((6 * len), ' ');
        //    string[] sRebuild = new string[len];
        //    string runner = "00";

        //    for (int i = 0; i < (6 * len); i += 6)
        //    {
        //        runner = (int.Parse((runner + input.Substring(i, 6))) % 97).ToString();
        //    }

        //    value.IsValid = (int.Parse(runner) == 1);
        //}
        //else
        //    value.IsValid = false;
    }

    protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                int accountID = (int)gvAccounts.SelectedDataKey.Value;
                switch (e.CommandName.ToUpper())
                {
                    case "ADDACCOUNT":
                        gvAccounts.SelectedIndex = -1;
                        break;
                }

            }
        }
        gvAccounts.SelectedIndex = -1;
    }
}

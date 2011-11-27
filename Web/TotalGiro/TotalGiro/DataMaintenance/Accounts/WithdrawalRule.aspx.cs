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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Utils;
using System.Text;
using B4F.TotalGiro.Accounts.Withdrawals;

public partial class DataMaintenance_WithdrawalRule : System.Web.UI.Page
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErr.Visible = false;
        lblErrorMess.Text = "";

        try
        {
            if (!IsPostBack)
            {
                if (AccountId == 0)
                {
                    Utility.CreatePrevPageSession();
                    Utility.NavigateToPrevPageSessionIfAny();
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    string number = WithdrawalRuleEditAdapter.GetAccountNumber(AccountId);
                    lblAccount.Text = number;
                }

                ((EG)this.Master).setHeaderText = "Withdrawal Rule";
                int ruleID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "ID");
                if (ruleID != 0)
                {
                    hfRuleID.Value = ruleID.ToString();
                    WithdrawalRuleDetails details = WithdrawalRuleEditAdapter.GetWithdrawalRule(ruleID);
                    lblAmount.Text = details.Amount.ToString();
                    if (ddlCounterAccount.Items.Count == 0)
                        ddlCounterAccount.DataBind();
                    ddlCounterAccount.SelectedValue = (details.CounterAccountID == 0 ? int.MinValue.ToString() : details.CounterAccountID.ToString());
                    lblRegularity.Text = details.RegularityLabel;
                    hfRegularity.Value = details.RegularityValue.ToString();
                    lblFirstDate.Text = details.FirstDateWithdrawal.ToShortDateString();
                    if (Util.IsNotNullDate(details.EndDateWithdrawal))
                        this.dpEndDate.SelectedDate = details.EndDateWithdrawal;
                    lblPandhouderPermission.Text = details.PandhouderPermission;
                    pnlIsActive.Visible = true;
                    chkIsActive.Checked = details.IsActive;
                    chkNoCharges.Checked = details.DoNotChargeCommission;
                    txtTransferDescription.Text = details.TransferDescription;
                }
                else
                {
                    mvRegularity.ActiveViewIndex = 1;
                    mvAmount.ActiveViewIndex = 1;
                    mvFirstDate.ActiveViewIndex = 1;
                    mvSave.ActiveViewIndex = 1;
                    mvPandhouderPermission.ActiveViewIndex = 1;
                    chkNoCharges.Checked = WithdrawalRuleEditAdapter.GetDefaultCommissionNotCharged();
                }

                bool blnTegenrekening = false;
                bool blnValidTegenrekening = false;
                bool blnVerpand = false;

                WithdrawalRuleEditAdapter.CheckWithdrawalRuleConditions(AccountId,
                                                                        ref blnTegenrekening,
                                                                        ref blnValidTegenrekening,
                                                                        ref blnVerpand);
                if (!blnTegenrekening || !blnValidTegenrekening)
                {
                    pnlErr.Visible = true;
                    lbtnInsert.Visible = false;
                    lbtnUpdate.Visible = false;
                    const string ERR = "It is not possible to create/edit withdrawal rules. ";
                    if (!blnTegenrekening)
                        lblErrorMess.Text = ERR + "There's no counter account.";
                    else if (!blnValidTegenrekening)
                        lblErrorMess.Text = ERR + "The counter account on the account is not valid.";
                }

                if (blnVerpand)
                    pnlPandhouderPermission.Visible = true;
            }
        }
        catch (Exception ex)
        {
            pnlErr.Visible = true;
            lblErrorMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    public int AccountId
    {
        get
        {
            object i = Session["accountnrid"];
            return ((i == null) ? int.MinValue : (int)i);
        }
    }

    protected void lbtnUpdate_Click(object sender, EventArgs e)
    {
        pnlErr.Visible = false;
        lblErrorMess.Text = "";

        try
        {
            if (!dpEndDate.IsEmpty)
                cvEndDate.Enabled = true;
            Page.Validate();
            if (Page.IsValid)
            {
                int ruleID = 0;
                if (hfRuleID.Value.Length > 0)
                    ruleID = int.Parse(hfRuleID.Value);

                WithdrawalRuleDetails details = new WithdrawalRuleDetails();
                details.EndDateWithdrawal = dpEndDate.SelectedDate;
                details.CounterAccountID = int.Parse(ddlCounterAccount.SelectedValue);
                details.AccountID = AccountId;
                details.IsActive = chkIsActive.Checked;
                details.DoNotChargeCommission = chkNoCharges.Checked;
                details.TransferDescription = txtTransferDescription.Text;
                WithdrawalRuleEditAdapter.SaveWithdrawalRule(ref ruleID, details);
                Response.Redirect("Account.aspx");
            }
        }
        catch (Exception ex)
        {
            pnlErr.Visible = true;
            lblErrorMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtnInsert_Click(object sender, EventArgs e)
    {
        pnlErr.Visible = false;
        lblErrorMess.Text = "";

        try
        {
            cvEndDate.Enabled = false;
            if (!dpEndDate.IsEmpty)
            {
                cvEndDate.Enabled = true;
                Page.Validate();
            }

            if (Page.IsValid)
            {
                int ruleID = 0;
                WithdrawalRuleDetails details = new WithdrawalRuleDetails();
                details.RegularityValue = int.Parse(ddlRegularity.SelectedValue);
                details.CounterAccountID = int.Parse(ddlCounterAccount.SelectedValue);
                details.AccountID = AccountId;
                details.FirstDateWithdrawal = dpFirstDate.SelectedDate;
                details.EndDateWithdrawal = dpEndDate.SelectedDate;
                details.Amount = dbAmount.Value;
                if (pnlPandhouderPermission.Visible)
                    details.PandhouderPermissionValue = cbPandhouderPermission.Checked;
                details.TransferDescription = txtTransferDescription.Text;
                details.DoNotChargeCommission = chkNoCharges.Checked;
                WithdrawalRuleEditAdapter.SaveWithdrawalRule(ref ruleID, details);
                //Response.Redirect("WithdrawalRule.aspx?ID=" + Convert.ToString(ruleID));
                Response.Redirect("Account.aspx");
            }
        }
        catch (Exception ex)
        {
            pnlErr.Visible = true;
            lblErrorMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void cvPandhouderPermission_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (cbPandhouderPermission.Checked == true);
    }


    protected void cvEndDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        DateTime firstDate;
        DateTime endDate = dpEndDate.SelectedDate;
        DateTime timeSpan;
        int period;

        if (mvFirstDate.ActiveViewIndex == 0)
        {
            string strFirstDate = lblFirstDate.Text;
            char[] sep = { '-' };
            string[] dateParts = strFirstDate.Split(sep);
            firstDate = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]));
        }
        else
            firstDate = dpFirstDate.SelectedDate;

        if (mvRegularity.ActiveViewIndex == 0)
            period = int.Parse(hfRegularity.Value);
        else
            period = int.Parse(ddlRegularity.SelectedValue);

        //Regularities: Annual = 1, SemiAnnual = 2, Quarterly = 4, Monthly = 12
        if (period == 1 || period == 2)
            timeSpan = firstDate.AddYears(1);
        else if (period == 4)
            timeSpan = firstDate.AddMonths(3);
        else
            timeSpan = firstDate.AddMonths(1);

        if (endDate < timeSpan)
            args.IsValid = false;
    }
}

                            
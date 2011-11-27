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
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.CRM;
using System.ComponentModel;
using B4F.TotalGiro.Instruments;
using B4F.Web.WebControls;

public partial class DataMaintenance_Account : System.Web.UI.Page
{
    //private const int GOOD_COLOUR = 0xCCFFCC;
    //private const int BAD_COLOUR = 0xFF6666;


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Utility.EnableScrollToBottom(this, hdnScrollToBottom);
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "View Account";

                int managementCompanyID = 0;

                Utility.CreatePrevPageSessionWithPageName("AttachAccountToContact.aspx");

                // Parameter from Account.aspx || AccountOverview.aspx || Accounts.ascx
                AccountId = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "AccountID");
                if (AccountId != 0)
                    Session["accountnrid"] = AccountId;
                else if (AccountId == 0 && Session["accountnrid"] != null)
                    AccountId = Convert.ToInt32(Session["accountnrid"]);

                loadAccountRecord(AccountId, managementCompanyID);

                if (IsUserAllowedToEditAccount)
                {
                    // Hide the Edit & Delete functionality for modelhistory
                    gvModelHistory.Columns[4].Visible = true;
                }
                ViewState["InsertMPMode"] = false;
                gvModelHistory.Sort("ChangeDate", SortDirection.Descending);
                gvManagementPeriods.Sort("StartDate", SortDirection.Descending);
                gvWithDrawals.Sort("FirstDateWithdrawal", SortDirection.Descending);
            }
            dpFinalManagementEndDate.SelectionChanged += new EventHandler(dpFinalManagementEndDate_SelectionChanged);
            dpFirstManagementStartDate.Expanded += new EventHandler(DatePicker_Expanded);
            dpFinalManagementEndDate.Expanded += new EventHandler(DatePicker_Expanded);

            if (Session["messagesavingaccount"] != null)
            {
                lblErrorMessage.Text = Session["messagesavingaccount"].ToString();
                Session["messagesavingaccount"] = null;
            }
            pnlErrorMess.Visible = false;
            lblMess.Text = "";
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void DatePicker_Expanded(object sender, EventArgs e)
    {
        try
        {
            DatePicker picker = (DatePicker)sender;
            if (picker != null)
            {
                if (picker.IsExpanded)
                {
                    if (picker.ID == "dpFirstManagementStartDate")
                        dpFinalManagementEndDate.IsExpanded = false;
                    else
                        dpFirstManagementStartDate.IsExpanded = false;
                }
                ddlExitFeePayer.Visible = !picker.IsExpanded;
                ddlRemisier.Visible = !picker.IsExpanded;
                ddlRemisierEmployee.Visible = !picker.IsExpanded;
                ddlEmployerRelationship.Visible = !picker.IsExpanded;
                ddlRelatedEmployee.Visible = !picker.IsExpanded;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    #region Properties

    public int AccountId
    {
        get
        {
            object i = ViewState["AccountId"];
            return ((i == null) ? int.MinValue : (int)i);
        }
        set { ViewState["AccountId"] = value; }
    }

    protected bool StatusIsOpen
    {
        get
        {
            object b = ViewState["StatusIsOpen"];
            return ((b == null) ? true : (bool)b);
        }
        set { ViewState["StatusIsOpen"] = value; }
    }
    #endregion

    protected void btnCreateCR_Click(object sender, EventArgs e)
    {
        string qStr = QueryStringModule.Encrypt(string.Format("AccountID={0}", AccountId));
        Response.Redirect(string.Format("~/DataMaintenance/Commission/RuleEdit.aspx{0}", qStr));
    }

    protected void btnCreateWR_Click(object sender, EventArgs e)
    {
        try
        {
            if (Session["accountnrid"] == null)
            {
                pnlErrorMess.Visible = true;
                lblMess.Text = "There's no account to associate the withdrawal rule with.";
            }
            else
                Response.Redirect("WithdrawalRule.aspx");
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        mvwAccountSave.ActiveViewIndex = 0;
    }

    protected void btnSaveAccount_Click(object sender, EventArgs e)
    {
        saveAccount(false);
    }

    protected void btnShowPortfolio_Click(object sender, EventArgs e)
    {
        try
        {
            if (ctlAccountLabel.AccountNumber != "")
            {
                string qStr = QueryStringModule.Encrypt(string.Format("AccountNumber={0}&Active=true&Inactive=true", HttpUtility.UrlEncode(ctlAccountLabel.AccountNumber)));
                Response.Redirect(string.Format("~/Portfolio/ClientPortfolio.aspx{0}", qStr));
            }
            else
            {
                pnlErrorMess.Visible = true;
                lblMess.Text = "Account Number not found.";
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        saveAccount(true);
    }

    private bool checkfields()
    {
        ValidatorCollection validators = null;
        bool isValid = true;
        validators = Page.Validators;
        foreach (IValidator validator in validators)
        {
            if (!(validator is RequiredFieldValidator) && !(validator is RangeValidator))
            {
                validator.Validate();
                if (!validator.IsValid)
                    isValid = false;
            }
        }
        return isValid;
    }

    protected void ddlModelPortfolio_SelectedIndexChanged(object sender, EventArgs e)
    {
        int modelId = int.Parse(ddlModelPortfolio.SelectedValue);
        ExecutionOnlyOptions execOption = AccountEditAdapter.GetModelExecutionOnlyOptions(modelId);
        setExecOnlyCustomerCheckBox(execOption);
    }

    protected void ddlLifecycle_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            int lifecycleId = Utility.GetKeyFromDropDownList(ddlLifecycle);
            ddlModelPortfolio.Enabled = (lifecycleId == int.MinValue);
            if (lifecycleId != int.MinValue)
                ddlModelPortfolio.SelectedValue = AccountOverviewAdapter.GetRelevantLifecycleModelID(AccountId, lifecycleId).ToString();
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void setExecOnlyCustomerCheckBox(ExecutionOnlyOptions execOption)
    {
        switch (execOption)
        {
            case ExecutionOnlyOptions.NotAllowed:
                chkIsExecOnlyCustomer.Enabled = false;
                chkIsExecOnlyCustomer.Checked = false;
                break;
            case ExecutionOnlyOptions.Allowed:
                chkIsExecOnlyCustomer.Enabled = true;
                break;
            case ExecutionOnlyOptions.Always:
                chkIsExecOnlyCustomer.Enabled = false;
                chkIsExecOnlyCustomer.Checked = true;
                break;
        }
    }


    protected void ddlVerpandSoort_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlVerpandSoort.Items.Count > 0 && int.Parse(ddlVerpandSoort.SelectedValue) >= 0)
            rfvPandhouder.Enabled = true;
        else
            rfvPandhouder.Enabled = false;
    }

    protected void ddlPandhouder_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPandhouder.Items.Count > 0 && int.Parse(ddlPandhouder.SelectedValue) >= 0)
            rfvVerpandSoort.Enabled = true;
        else
            rfvVerpandSoort.Enabled = false;
    }

    protected void dpFinalManagementEndDate_SelectionChanged(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = "";
            if (Util.IsNotNullDate(dpFinalManagementEndDate.SelectedDate))
            {
                if (!pnlExitFeePayer.Visible)
                {
                    DataSet ds = AccountEditAdapter.GetOtherOwnAccounts(AccountId);
                    if (ds != null && ds.Tables[0].Rows.Count > 1)
                        pnlExitFeePayer.Visible = true;
                }
                lblErrorMessage.Text = AccountEditAdapter.CheckAccountsWithdrawals(AccountId, dpFinalManagementEndDate.SelectedDate);
            }
            else
                pnlExitFeePayer.Visible = false;
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    //private void fillDDLModelPortfolio(int managementCompanyID)
    //{
    //    DataSet dsModelPortfolios = AccountFinderAdapter.GetModelPortfolios(managementCompanyID, ActivityReturnFilter.Active);
    //    ddlModelPortfolio.DataSource = dsModelPortfolios;
    //    ddlModelPortfolio.DataBind();
    //}

    protected void ddlRemisier_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlRemisierEmployee.DataBind();
        checkKickbackMode();
    }

    protected void ddlRemisierEmployee_SelectedIndexChanged(object sender, EventArgs e)
    {
        checkKickbackMode();
    }

    protected void ddlEmployerRelationship_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            pnlRelatedEmployee.Visible = (ddlEmployerRelationship.SelectedValue != "0");
            if (!pnlRelatedEmployee.Visible)
            {
                if (ddlRelatedEmployee.Items != null && ddlRelatedEmployee.Items.Count > 0)
                    ddlEmployerRelationship.SelectedIndex = -1;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvAccountholders_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvAccountholders.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int contactKey = (int)gvAccountholders.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "SETPRIMARY":
                            AccountEditAdapter.SetPrimaryAccountHolder(int.Parse(Session["accountnrid"].ToString()), contactKey);
                            Response.Redirect("Account.aspx");
                            break;
                        case "DETACHACCOUNTHOLDER":
                            AccountEditAdapter.DetachAccountHolder(int.Parse(Session["accountnrid"].ToString()), contactKey);
                            Response.Redirect("Account.aspx");
                            break;
                        case "EDITCONTACT":
                            string contactType = ContactOverviewAdapter.GetContactType(contactKey);
                            Session["contactid"] = contactKey;
                            if ((contactType.ToUpper()).Equals(ContactTypeEnum.Person.ToString().ToUpper()))
                            {
                                Response.Redirect("~/DataMaintenance/Contacts/Person.aspx");
                            }
                            else
                            {
                                Response.Redirect("~/DataMaintenance/Contacts/Company.aspx");
                            }
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvAccountholders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && IsUserAllowedToEditAccount)
        {
            LinkButton lbtnDetach = (LinkButton)e.Row.FindControl("lbtnDetach");
            lbtnDetach.Enabled = true;
            LinkButton lbtnEdit = (LinkButton)e.Row.FindControl("lbtnEdit");
            lbtnEdit.Enabled = true;
        }
    }

    protected void gvCommissionRules_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    // Select row
                    gvCommissionRules.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int ruleId = (int)gvCommissionRules.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITRULE":
                            gvCommissionRules.SelectedIndex = -1;
                            string qStr = QueryStringModule.Encrypt(string.Format("id={0}&AccountID={1}", ruleId, AccountId));
                            Response.Redirect(string.Format("~/DataMaintenance/Commission/RuleEdit.aspx{0}", qStr));
                            break;

                    }
                }
            }

            gvCommissionRules.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCommissionRules_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnEditCommissionRule = (LinkButton)e.Row.FindControl("lbtnEditCommissionRule");
            if (!(StatusIsOpen && SecurityManager.IsCurrentUserInRole("Calculation Rules Mtce")))
                lbtnEditCommissionRule.Enabled = false;
        }
    }

    protected void gvModelHistory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvModelHistory.EditIndex = -1;
                    gvModelHistory.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "DELETEHISTORYITEM":
                            int modelHistoryID = Convert.ToInt32(gvModelHistory.SelectedDataKey.Value);
                            AccountEditAdapter.DeleteModelHistoryItem(modelHistoryID, AccountId);
                            gvModelHistory.DataBind();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvModelHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            bool enabled = IsUserAllowedToEditAccount;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
                lbtnDelete.Enabled = enabled;

                e.Row.Cells[gvModelHistory.Columns.Count - 2].Visible = enabled;

                DropDownList ddlModel = (DropDownList)e.Row.FindControl("ddlModel");
                if (ddlModel != null)
                    ddlModel.SelectedValue = ((DataRowView)e.Row.DataItem)["ModelPortfolioKey"].ToString();
                
                DropDownList ddlEmployerRelationship = (DropDownList)e.Row.FindControl("ddlEmployerRelationship");
                if (ddlEmployerRelationship != null)
                    ddlEmployerRelationship.SelectedValue = ((DataRowView)e.Row.DataItem)["EmployerRelationship"].ToString();
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvModelHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            DropDownList ddlModel = (DropDownList)(gvModelHistory.Rows[e.RowIndex].FindControl("ddlModel"));
            e.NewValues["modelID"] = int.Parse(ddlModel.SelectedValue);

            DropDownList ddlEmployerRelationship = (DropDownList)(gvModelHistory.Rows[e.RowIndex].FindControl("ddlEmployerRelationship"));
            e.NewValues["employerRelationship"] = int.Parse(ddlEmployerRelationship.SelectedValue);
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void odsData_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        try
        {
            if (e.Exception != null)
            {
                string errMessage = "";
                if (e.Exception.InnerException != null)
                    errMessage = Utility.GetCompleteExceptionMessage(e.Exception.InnerException);
                else
                    errMessage = Utility.GetCompleteExceptionMessage(e.Exception);
                
                if (errMessage != "")
                {
                    pnlErrorMess.Visible = true;
                    lblMess.Text = errMessage;
                }
                e.ExceptionHandled = true;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvManagementPeriods_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvManagementPeriods.EditIndex = -1;
                    gvManagementPeriods.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "CREATEMANAGEMENTPERIOD":
                            InsertMPMode = true;
                            break;
                        case "EDITMANAGEMENTPERIOD":
                            EditMPMode = true;
                            break;
                        case "DELETEMANAGEMENTPERIOD":
                            int managementPeriodID = Convert.ToInt32(gvManagementPeriods.SelectedDataKey.Value);
                            AccountEditAdapter.DeleteManagementPeriod(managementPeriodID);
                            gvManagementPeriods.DataBind();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvManagementPeriods_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            bool enabled = IsUserAllowedToEditAccount;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool isEditable = (bool)((DataRowView)e.Row.DataItem)["IsEditable"];
                if (!isEditable)
                    enabled = false;

                LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
                lbtnDelete.Enabled = enabled;

                LinkButton lbtnEdit = (LinkButton)e.Row.FindControl("lbtnEdit");
                lbtnEdit.Enabled = enabled;

                //e.Row.Cells[gvManagementPeriods.Columns.Count - 2].Visible = enabled;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateMP_Click(object sender, EventArgs e)
    {
        try
        {
            if (!InsertMPMode)
                InsertMPMode = true;
            else
            {
                int managementType = int.Parse(ddlManagementTypes.SelectedValue);
                AccountEditAdapter.CreateManagementPeriod(AccountId, managementType, dtpMPStartDate.SelectedDate);
                gvManagementPeriods.DataBind();

                InsertMPMode = false;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnEditMP_Click(object sender, EventArgs e)
    {
        try
        {
            if (!EditMPMode)
                EditMPMode = true;
            else
            {
                cvMPEndDate.Validate();
                if (cvMPEndDate.IsValid)
                {
                    int managementPeriodID = Convert.ToInt32(gvManagementPeriods.SelectedDataKey.Value);
                    AccountEditAdapter.UpdateManagementPeriod(dtpMPStartDate.SelectedDate, dtpMPEndDate.SelectedDate, managementPeriodID);
                    gvManagementPeriods.DataBind();

                    EditMPMode = false;
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancelMP_Click(object sender, EventArgs e)
    {
        try
        {
            InsertMPMode = false;
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void chkUseKickBack_CheckedChanged(object sender, EventArgs e)
    {
        checkKickbackMode();
        setKickbackMode();
    }

    private void setKickbackMode()
    {
        dbKickbackPercentage.Enabled = chkUseKickBack.Checked;
    }

    private void checkKickbackMode()
    {
        bool useKickBack = chkUseKickBack.Checked;
        int employeeID = int.MinValue;
        int.TryParse(ddlRemisierEmployee.SelectedValue, out employeeID);

        if (useKickBack && employeeID == int.MinValue)
        {
            this.chkUseKickBack.Checked = false;
            dbKickbackPercentage.Enabled = chkUseKickBack.Checked;
        }
        this.chkUseKickBack.Enabled = (employeeID != int.MinValue);
        string message = AccountEditAdapter.CheckKickBackManagementPeriod(AccountId, chkUseKickBack.Checked);
        lblErrorMessage.Text = message;
    }

    private bool InsertMPMode
    {
        get { return (ViewState["InsertMPMode"] != null ? (bool)ViewState["InsertMPMode"] : false); }
        set
        {
            btnSaveAccount.Enabled = !value;
            btnCreateCR.Enabled = !value;
            gvCommissionRules.Enabled = !value;
            btnCreateFeeRule.Enabled = !value;
            gvFeeRules.Enabled = !value;
            btnCreateWR.Enabled = !value;
            gvWithDrawals.Enabled = !value;
            gvAccountholders.Enabled = !value;
            gvModelHistory.Enabled = !value;
            gvManagementPeriods.Enabled = !value;
            pnlCreateManagementPeriod.Visible = value;
            btnCreateMP.Visible = true;
            btnCancelMP.Visible = value;
            btnEditMP.Visible = false;
            if (!value)
            {
                gvManagementPeriods.SelectedIndex = -1;
                dtpMPStartDate.Clear();
            }
            else
                Utility.ScrollToBottom(hdnScrollToBottom);
            ViewState["InsertMPMode"] = value;

            ddlManagementTypes.Enabled = true;
            pnlMPEndDate.Visible = false;
        }
    }

    private bool EditMPMode
    {
        get { return (ViewState["EditMPMode"] != null ? (bool)ViewState["EditMPMode"] : false); }
        set
        {
            btnSaveAccount.Enabled = !value;
            btnCreateCR.Enabled = !value;
            gvCommissionRules.Enabled = !value;
            btnCreateFeeRule.Enabled = !value;
            gvFeeRules.Enabled = !value;
            btnCreateWR.Enabled = !value;
            gvWithDrawals.Enabled = !value;
            gvAccountholders.Enabled = !value;
            gvModelHistory.Enabled = !value;
            gvManagementPeriods.Enabled = !value;

            pnlCreateManagementPeriod.Visible = value;
            btnCancelMP.Visible = value;
            btnCreateMP.Visible = !value;
            btnEditMP.Visible = value;
            if (!value)
            {
                gvManagementPeriods.SelectedIndex = -1;
                dtpMPStartDate.Clear();
            }
            ViewState["EditMPMode"] = value;

            if (value)
            {
                ddlManagementTypes.Enabled = false;
                pnlMPEndDate.Visible = true;

                int managementTypeId;
                DateTime startDate, endDate;
                int managementPeriodID = Convert.ToInt32(gvManagementPeriods.SelectedDataKey.Value);
                AccountEditAdapter.GetManagementPeriodDetails(managementPeriodID, out managementTypeId, out startDate, out endDate);

                ddlManagementTypes.SelectedValue = managementTypeId.ToString();
                dtpMPStartDate.SelectedDate = startDate;
                dtpMPEndDate.SelectedDate = endDate;

                btnEditMP.Focus();
            }
        }
    }

    private bool InsertFeeRuleMode
    {
        get { return (ViewState["InsertFeeRuleMode"] != null ? (bool)ViewState["InsertFeeRuleMode"] : false); }
        set
        {
            pnlFeeRuleEntry.Visible = value;
            btnCancelCreateFeeRule.Visible = value;

            btnSaveAccount.Enabled = !value;
            gvFeeRules.Enabled = !value;
            btnCreateCR.Enabled = !value;
            gvCommissionRules.Enabled = !value;
            btnCreateWR.Enabled = !value;
            gvWithDrawals.Enabled = !value;
            gvAccountholders.Enabled = !value;
            gvModelHistory.Enabled = !value;
            btnCreateMP.Enabled = !value;
            gvManagementPeriods.Enabled = !value;

            this.btnCreateFeeRule.Focus();
            ViewState["InsertFeeRuleMode"] = value;
        }
    }

    protected void gvWithDrawals_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvWithDrawals.EditIndex = -1;
                    gvWithDrawals.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    string withDrawalKey = gvWithDrawals.SelectedDataKey.Value.ToString();

                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITWITHDRAWALRULE":
                            string qStr = QueryStringModule.Encrypt(string.Format("ID={0}", withDrawalKey));
                            Response.Redirect(string.Format("WithdrawalRule.aspx{0}", qStr));
                            break;
                        case "DELETEWITHDRAWALRULE":
                            int ruleID = int.Parse(withDrawalKey);
                            WithdrawalRuleEditAdapter.DeleteWithDrawalRule(ruleID);
                            gvWithDrawals.DataBind();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvWithDrawals_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lbtnEditWithdrawalRule = (LinkButton)e.Row.FindControl("lbtnEditWithdrawalRule");
                LinkButton lbtnDeleteWithdrawalRule = (LinkButton)e.Row.FindControl("lbtnDeleteWithdrawalRule");

                DateTime endDateWithdrawal = (DateTime)((DataRowView)e.Row.DataItem)["EndDateWithdrawal"];
                if (!(Util.IsNullDate(endDateWithdrawal) || endDateWithdrawal > DateTime.Today) ||
                    !(StatusIsOpen && SecurityManager.IsCurrentUserInRole("Data Mtce: Withdrawal Rule")))
                {
                    lbtnEditWithdrawalRule.Enabled = false;
                    lbtnDeleteWithdrawalRule.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvFeeRules_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            YearMonthPicker ppEndPeriod = (YearMonthPicker)(gvFeeRules.Rows[e.RowIndex].FindControl("ppEndPeriod"));
            e.NewValues["endPeriod"] = ppEndPeriod.SelectedPeriod;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateFeeRule_Click(object sender, EventArgs e)
    {
        try
        {
            if (!InsertFeeRuleMode)
                InsertFeeRuleMode = true;
            else
            {
                rfvFeeCalculation.Validate();
                cvEndPeriod.Validate();
                if (rfvFeeCalculation.IsValid && cvEndPeriod.IsValid)
                {
                    if (AccountEditAdapter.CreateAccountFeeRule(
                        AccountId,
                        Convert.ToInt32(ddlFeeCalculation.SelectedValue),
                        chkExecutionOnly.Checked,
                        chkSendByPost.Checked,
                        ppStartPeriod.SelectedPeriod,
                        ppEndPeriod.SelectedPeriod))
                    {
                        gvFeeRules.DataBind();
                        InsertFeeRuleMode = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancelCreateFeeRule_Click(object sender, EventArgs e)
    {
        InsertFeeRuleMode = false;
    }

    protected void cvEndPeriod_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = !(ppEndPeriod.SelectedPeriod > 0 && (ppStartPeriod.SelectedPeriod == 0 || ppStartPeriod.SelectedPeriod >= ppEndPeriod.SelectedPeriod));
    }

    protected bool IsUserAllowedToEditAccount
    {
        get
        {
            return SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit") && StatusIsOpen;
        }
    }

    /// <summary>
    /// Retrieve/set the asset manager key.
    /// </summary>
    [Description("Retrieve/set the asset manager key.")]
    public int AssetManagerID
    {
        get
        {
            object b = ViewState["AssetManagerID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["AssetManagerID"] = value;
        }
    }

    private void loadAccountRecord(int accountId, int managementCompanyID)
    {
        if (accountId != int.MinValue)
        {
            AccountDetails accDetails = AccountEditAdapter.GetAccountDetails(accountId);
            if (accDetails.AccountNrID != 0)
            {
                lblAssetManager.Text = accDetails.AssetManagerName;
                tbFullName.Text = accDetails.AccountFullName;
                tbShortName.Text = accDetails.AccountShortName;
                ctlAccountLabel.AccountID = accDetails.AccountNrID;
                ctlAccountLabel.AccountNumber = accDetails.AccountNumber;
                ctlAccountLabel.Notification = accDetails.Notification;
                ctlAccountLabel.NotificationType = accDetails.NotificationType;
                ctlAccountLabel.AccountIsActive = (accDetails.Status == AccountStati.Active);
                ctlAccountLabel.AccountIsDeparting = accDetails.IsDeparting;
                ctlAccountLabel.AccountIsUnderRebalance = accDetails.IsUnderRebalance;

                ddlAccountFamily.SelectedValue = accDetails.FamilyID.ToString();
                chkIsJointAccount.Checked = accDetails.IsJointAccount;
                //fillDDLModelPortfolio(accDetails.AssetManagerKey);
                hfAssetMAnagerID.Value = accDetails.AssetManagerKey.ToString();


                pnlLifecycle.Visible = accDetails.AssetManagerSupportLifecycles;
                if (accDetails.LifecycleId != 0 && accDetails.LifecycleId != int.MinValue)
                {
                    pnlLifecycle.Visible = true;
                    ddlLifecycle.SelectedValue = accDetails.LifecycleId.ToString();
                    ddlModelPortfolio.Enabled = (accDetails.LifecycleId == int.MinValue);
                }

                ddlModelPortfolio.DataBind();
                if (accDetails.Modelid != 0)
                    ddlModelPortfolio.SelectedValue = accDetails.Modelid.ToString();

                chkIsExecOnlyCustomer.Enabled = accDetails.ModelAllowExecOnlyCustomers;
                chkIsExecOnlyCustomer.Checked = accDetails.IsExecOnlyCustomer;
                dbFirstDeposit.Value = accDetails.FirstPromisedDeposit;

                if (accDetails.CounterAccountID != 0)
                {
                    string key = accDetails.CounterAccountID.ToString();
                    if (this.ddlCounterAccount.Items == null || this.ddlCounterAccount.Items.Count == 0)
                        this.ddlCounterAccount.DataBind();
                    if (this.ddlCounterAccount.Items.FindByValue(key) != null)
                        this.ddlCounterAccount.SelectedValue = key;
                }

                if (accDetails.VerpandSoortID != 0)
                    ddlVerpandSoort.SelectedValue = accDetails.VerpandSoortID.ToString();

                if (accDetails.PandHouderID != 0)
                    ddlPandhouder.SelectedValue = accDetails.PandHouderID.ToString();

                if (accDetails.ContactContractsValidated)
                {
                    lblNarDetails.Text = "Contact NAR Details are IN Order";
                    lblNarDetails.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblNarDetails.Text = "Contact NAR Details are NOT IN Order";
                    lblNarDetails.ForeColor = System.Drawing.Color.Red; ;
                }

                lblCreationDate.Text = accDetails.CreationDate.ToString("ddd, dd-MMM-yyyy hh:mm");
                lblLastupdated.Text = accDetails.LastUpDated.ToString("ddd, dd-MMM-yyyy hh:mm");

                ddlTradeability.SelectedValue = ((int)(accDetails.TradeableStatus)).ToString();
                if (accDetails.DateTradeabilityStatusChanged != null)
                    lblLastDateTradeabilityChanged.Text = accDetails.DateTradeabilityStatusChanged.ToString("ddd, dd-MMM-yyyy hh:mm");
                else
                {
                    lblLastDateTradeabilityChanged.Visible = false;
                    lblLastDateTradeabilityChangedLabel.Visible = false;
                }

                ddlStatus.SelectedValue = ((int)(accDetails.Status)).ToString();
                if (accDetails.LastDateStatusChanged != null)
                    lblLastDateStatusChanged.Text = accDetails.LastDateStatusChanged.ToString("ddd, dd-MMM-yyyy hh:mm");
                else
                {
                    lblLastDateStatusChanged.Visible = false;
                    lblLastDateStatusChangedLabel.Visible = false;
                }

                if (Util.IsNotNullDate(accDetails.FirstManagementStartDate))
                    dpFirstManagementStartDate.SelectedDate = accDetails.FirstManagementStartDate;
                if (Util.IsNotNullDate(accDetails.FinalManagementEndDate))
                    dpFinalManagementEndDate.SelectedDate = accDetails.FinalManagementEndDate;

                if (accDetails.ExitFeePayingAccountID != int.MinValue)
                {
                    pnlExitFeePayer.Visible = true;
                    ddlExitFeePayer.DataBind();
                    ddlExitFeePayer.SelectedValue = (accDetails.ExitFeePayingAccountID.ToString());
                }

                chkUseManagementFee.Checked = accDetails.UseManagementFee;
                if (accDetails.RemisierID != int.MinValue)
                {
                    ddlRemisier.DataBind();
                    ddlRemisier.SelectedValue = accDetails.RemisierID.ToString();
                    ddlRemisierEmployee.DataBind();
                }
                if (accDetails.RemisierEmployeeID != int.MinValue)
                {
                    ddlRemisierEmployee.SelectedValue = accDetails.RemisierEmployeeID.ToString();
                    chkUseKickBack.Enabled = true;
                }
                chkUseKickBack.Checked = accDetails.UseKickBack;
                setKickbackMode();
                dbKickbackPercentage.Value = accDetails.KickBack;
                dbIntroductionFee.Value = accDetails.IntroductionFee;
                dbIntroductionFeeReduction.Value = accDetails.IntroductionFeeReduction;
                dbSubsequentDepositFee.Value = accDetails.SubsequentDepositFee;
                dbSubsequentDepositFeeReduction.Value = accDetails.SubsequentDepositFeeReduction;

                ddlEmployerRelationship.DataBind();
                ddlEmployerRelationship.SelectedValue = ((int)accDetails.EmployerRelationship).ToString();
                ddlEmployerRelationship_SelectedIndexChanged(null, null);
                try
                {
                    ddlRelatedEmployee.SelectedValue = accDetails.RelatedEmployeeID.ToString();
                }
                catch (Exception)
                {
                    lblMess.Text = string.Format("Employee {0} is no longer Active", accDetails.RelatedEmployeeID);
                }

                StatusIsOpen = accDetails.StatusIsOpen;
                ddlStatus.Enabled = StatusIsOpen;
                btnSaveAccount.Enabled = IsUserAllowedToEditAccount && accDetails.IsCustomer;
                btnCreateFeeRule.Enabled = IsUserAllowedToEditAccount && accDetails.IsCustomer;
                btnCreateMP.Enabled = IsUserAllowedToEditAccount && accDetails.IsCustomer;

                if (!StatusIsOpen)
                    ((EG)this.Master).setHeaderText = "View Account (closed)";
                else if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                    ((EG)this.Master).setHeaderText = (AccountId != 0 ? "Edit Account" : "Create New Account");

                if (!SecurityManager.IsCurrentUserInRole("Portfolio: Client Portfolio"))
                    btnShowPortfolio.Enabled = false;

                btnCreateCR.Enabled = StatusIsOpen && SecurityManager.IsCurrentUserInRole("Calculation Rules Mtce") && accDetails.IsCustomer;
                btnCreateWR.Enabled = StatusIsOpen && SecurityManager.IsCurrentUserInRole("Data Mtce: Withdrawal Rule") && accDetails.IsCustomer;
            }
        }
    }

    private void saveAccount(bool forceClosedStatus)
    {
        try
        {
            string message;
            bool valid = false;
            bool askConfirmation = false;
            AccountDetails saveValue = new AccountDetails()
            {
                AccountNrID = AccountId,
                AccountNumber = String.Empty,
                AssetManagerKey = int.MinValue,
                Modelid = int.MinValue,
                LifecycleId = int.MinValue,
                AccountShortName = string.Empty,
                AccountFullName = string.Empty,
                PandHouderID = int.MinValue,
                CounterAccountID = int.MinValue,
                CreationDate = DateTime.MinValue,
                LastUpDated = DateTime.MinValue,
                LastDateStatusChanged = DateTime.MinValue,
                FirstManagementStartDate = DateTime.MinValue,
                FinalManagementEndDate = DateTime.MinValue,
                DateTradeabilityStatusChanged = DateTime.MinValue,
                ExitFeePayingAccountID = int.MinValue,
                VerpandSoortID = int.MinValue,
                RelatedEmployeeID = int.MinValue
            };

            if (ddlVerpandSoort.Items.Count > 0 && int.Parse(ddlVerpandSoort.SelectedValue) >= 0)
                rfvPandhouder.Enabled = true;

            if (ddlPandhouder.Items.Count > 0 && int.Parse(ddlPandhouder.SelectedValue) >= 0)
                rfvVerpandSoort.Enabled = true;

            Page.Validate();
            valid = Page.IsValid;
            if (valid)
            {
                saveValue.AccountNrID = Convert.ToInt32(AccountId);
                saveValue.AssetManagerKey = Convert.ToInt32(hfAssetMAnagerID.Value);
                saveValue.IsJointAccount = chkIsJointAccount.Checked;
                if (pnlLifecycle.Visible)
                    saveValue.LifecycleId = Utility.GetKeyFromDropDownList(ddlLifecycle);
                saveValue.Modelid = int.Parse(ddlModelPortfolio.SelectedValue);
                saveValue.IsExecOnlyCustomer = chkIsExecOnlyCustomer.Checked;
                saveValue.AccountShortName = tbShortName.Text;
                saveValue.AccountFullName = tbFullName.Text;
                saveValue.FamilyID = Utility.GetKeyFromDropDownList(ddlAccountFamily);
                saveValue.PandHouderID = int.Parse(ddlPandhouder.SelectedValue);

                if (int.Parse(ddlVerpandSoort.SelectedValue) != int.MinValue)
                    saveValue.VerpandSoortID = int.Parse(ddlVerpandSoort.SelectedValue);

                saveValue.CounterAccountID = int.Parse(ddlCounterAccount.SelectedValue);
                saveValue.Status = (AccountStati)int.Parse(ddlStatus.SelectedValue);
                saveValue.TradeableStatus = (Tradeability)int.Parse(ddlTradeability.SelectedValue);

                if (dpFirstManagementStartDate.SelectedDate > DateTime.MinValue)
                    saveValue.FirstManagementStartDate = dpFirstManagementStartDate.SelectedDate;
                saveValue.FinalManagementEndDate = dpFinalManagementEndDate.SelectedDate;
                if (pnlExitFeePayer.Visible)
                    saveValue.ExitFeePayingAccountID = int.Parse(ddlExitFeePayer.SelectedValue);

                if (ddlRemisierEmployee.Items.Count > 0 && ddlRemisierEmployee.SelectedIndex != -1)
                    saveValue.RemisierEmployeeID = Convert.ToInt32(ddlRemisierEmployee.SelectedValue);
                saveValue.UseManagementFee = chkUseManagementFee.Checked;
                saveValue.UseKickBack = chkUseKickBack.Checked;
                saveValue.KickBack = dbKickbackPercentage.Value;
                saveValue.IntroductionFee = dbIntroductionFee.Value;
                saveValue.IntroductionFeeReduction = dbIntroductionFeeReduction.Value;
                saveValue.SubsequentDepositFee = dbSubsequentDepositFee.Value;
                saveValue.SubsequentDepositFeeReduction = dbSubsequentDepositFeeReduction.Value;

                saveValue.EmployerRelationship = (AccountEmployerRelationship)int.Parse(ddlEmployerRelationship.SelectedValue);
                if (pnlRelatedEmployee.Visible)
                    saveValue.RelatedEmployeeID = int.Parse(ddlRelatedEmployee.SelectedValue);
                
                saveValue.FirstPromisedDeposit = dbFirstDeposit.Value;

                AccountEditAdapter.SaveCustomerAccount(saveValue, forceClosedStatus, out askConfirmation, out message);
                if (message != null && message.Length > 0)
                    Session["messagesavingaccount"] = message;

                if (askConfirmation)
                    mvwAccountSave.ActiveViewIndex = 1;
                else
                {
                    //Session["accountnrid"] = AccountId;
                    Utility.NavigateToPrevPageSessionIfAnyWithQueryString("?accountnumber=" + saveValue.AccountNumber);
                    Response.Redirect("Account.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

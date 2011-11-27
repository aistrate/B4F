using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions;
using B4F.TotalGiro.Instruments.CorporateAction;
using System.Data;
using B4F.TotalGiro.Security;

public partial class InstrumentConversionDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Instrument Conversion Details";
            bool? isEdit = QueryStringModule.GetBValueFromQueryString(Request.RawUrl, "Edit");
            int id = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "instrumentConversionID");

            if (isEdit.HasValue && isEdit.Value && id != 0)
            {
                InstrumentConversionID = id;
                this.DataBind();
                displayConversionDetails();
            }
            else
            {
                bool allowSave = false;
                int instrumentId = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "InstrumentToConvertID");
                int newInstrumentId = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "NewInstrumentID");
                if (instrumentId != 0 && newInstrumentId != 0)
                {
                    ddlInstrument.SelectedValue = instrumentId.ToString();
                    ddlNewInstrument.SelectedValue = newInstrumentId.ToString();
                    allowSave = true;
                }

                displayConversionDetails();
                enableControls(false, false);
                btnSaveDetails.Enabled = allowSave;
                btnInitialise.Enabled = false;
            }
        }
    }

    protected int InstrumentConversionID
    {
        get
        {
            object i = ViewState["InstrumentConversionID"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            ViewState["InstrumentConversionID"] = value;
            this.hdnInstrumentConversionID.Value = value.ToString();
        }
    }

    protected bool IsInitialised
    {
        get
        {
            object i = ViewState["isInitialised"];
            return ((i == null) ? false : (bool)i);
        }
        set
        {
            ViewState["isInitialised"] = value;
        }
    }

    protected void gvInstrumentConversionTxDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["AccountID"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            int id = Utility.GetKeyFromDropDownList(ddlInstrument);
            this.btnSaveDetails.Enabled = id != int.MinValue;
            this.btnCreateNewInstrument.Enabled = id != int.MinValue;
            hdnConvertedInstrumentID.Value = id.ToString();
            if (id != int.MinValue)
                ddlNewInstrument.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateNewInstrument_Click(object sender, EventArgs e)
    {
        try
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                lblErrorMessage.Text = "You don't have the right to create new instruments.";
            else
            {
                int instrumentid = Utility.GetKeyFromDropDownList(ddlNewInstrument);
                if (instrumentid > 0)
                    Session["instrumentid"] = instrumentid;
                string qStr = QueryStringModule.Encrypt(string.Format("InstrumentToConvertID={0}", ddlInstrument.SelectedValue));
                Response.Redirect(string.Format("~/DataMaintenance/Instruments/Security.aspx{0}", qStr));
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSaveDetails_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkFields())
            {
                InstrumentConversionAdapter.InstrumentConversionDetails newValue = getInstrumentConversionDetails();
                this.InstrumentConversionID = InstrumentConversionAdapter.CreateOrSaveInstrumentConversion(newValue);
                displayConversionDetails();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnInitialise_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkFields())
            {
                InstrumentConversionAdapter.InstrumentConversionDetails oldValue = getInstrumentConversionDetails();
                int instrumentConversionID = InstrumentConversionAdapter.CreateOrSaveInstrumentConversion(oldValue);
                if (InstrumentConversionAdapter.InitialiseConversion(instrumentConversionID))
                    displayConversionDetails();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnExecute_Click(object sender, EventArgs e)
    {
        try
        {
            if (InstrumentConversionAdapter.ExecuteConversion(this.InstrumentConversionID))
                displayConversionDetails();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private InstrumentConversionAdapter.InstrumentConversionDetails getInstrumentConversionDetails()
    {
        InstrumentConversionAdapter.InstrumentConversionDetails returnValue = new InstrumentConversionAdapter.InstrumentConversionDetails();
        returnValue.Key = this.InstrumentConversionID;
        returnValue.ChangeDate = dpChangeDate.SelectedDate;
        //returnValue.ExecutionDate = dpExecutionDate.SelectedDate;
        returnValue.InstrumentID = Utility.GetKeyFromDropDownList(ddlInstrument);
        returnValue.IsSpinOff = chkIsSpinOff.Checked;
        returnValue.NewInstrumentID = Utility.GetKeyFromDropDownList(ddlNewInstrument);
        returnValue.NewParentRatio = (Byte)dbNewParentRatio.Value;
        returnValue.OldChildRatio = dbOldChildRatio.Value;
        return returnValue;
    }

    private void displayConversionDetails()
    {
        if (this.InstrumentConversionID != 0)
        {
            InstrumentConversionAdapter.InstrumentConversionDetails details = InstrumentConversionAdapter.GetInstrumentConversionDetails(this.InstrumentConversionID);
            this.hdnInstrumentConversionID.Value = details.Key.ToString();
            this.hdnConvertedInstrumentID.Value = details.InstrumentID.ToString();
            this.InstrumentConversionID = details.Key;
            this.ddlInstrument.SelectedValue = details.InstrumentID.ToString();
            this.ddlNewInstrument.SelectedValue = details.NewInstrumentID.ToString();
            this.dpChangeDate.SelectedDate = details.ChangeDate;
            this.dpExecutionDate.SelectedDate = details.ExecutionDate;
            this.chkIsSpinOff.Checked = details.IsSpinOff;
            this.dbOldChildRatio.Value = details.OldChildRatio;
            this.dbNewParentRatio.Value = details.NewParentRatio;

            this.lblTotalOriginalSize.Text = details.TotalOriginalSize.ToString("0.0#####");
            this.lblTotalConvertedSize.Text = details.TotalConvertedSize.ToString("0.0#####");
            this.trTotalOriginalSize.Visible = details.IsInitialised;
            this.trTotalConvertedSize.Visible = details.IsInitialised;
            this.gvInstrumentConversionTxDetails.Visible = details.IsInitialised;
            if (this.gvInstrumentConversionTxDetails.Visible)
                this.gvInstrumentConversionTxDetails.DataBind();
            enableControls(details.IsInitialised, details.IsExecuted);
            btnSaveDetails.Text = "Save Conversion";
            btnSaveDetails.Enabled = !details.IsInitialised;
        }
        else
        {
            btnSaveDetails.Text = "Create New Conversion";
        }
    }

    private void enableControls(bool isInitialised, bool isExecuted)
    {
        this.ddlInstrument.Enabled = !isInitialised;
        this.ddlNewInstrument.Enabled = !isInitialised;
        this.btnCreateNewInstrument.Enabled = !isInitialised;
        this.dpChangeDate.Enabled = !isInitialised;
        this.dpExecutionDate.Enabled = !isInitialised;
        this.chkIsSpinOff.Enabled = !isInitialised;
        this.dbOldChildRatio.Enabled = !isInitialised;
        this.dbNewParentRatio.Enabled = !isInitialised;
        this.btnInitialise.Enabled = !isInitialised;
        this.trExecutionDate.Visible = isExecuted;
        this.btnExecute.Enabled = isInitialised && !isExecuted;
    }

    private bool checkFields()
    {
        ValidatorCollection validators = null;
        bool isValid = true;
        validators = Page.Validators;
        foreach (IValidator validator in validators)
        {
            if ((validator is RequiredFieldValidator) || (validator is RangeValidator) || (validator is CompareValidator))
            {
                validator.Validate();
                if (!validator.IsValid)
                    isValid = false;
            }
        }
        return isValid;
    }
}

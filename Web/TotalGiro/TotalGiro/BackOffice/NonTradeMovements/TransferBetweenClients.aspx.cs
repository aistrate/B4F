using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.Orders.Transfers;
using System.Data;
using B4F.TotalGiro.ApplicationLayer.BackOffice;


public partial class TransferBetweenClients : System.Web.UI.Page
{
    enum VIEWS
    {
        AccountA = 0,
        AccountB
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Non Trade Transfers";
                PositionTransferID = (Session["PositionTransferID"] != null ? (int)Session["PositionTransferID"] : 0);
                ctlTransferPositions.TransferID = PositionTransferID;
                this.chkAIsInternal.Checked = true;
                displayTransferDetails();
            }
            lblErrorMessageMain.Text = "";
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    public TransferType TypeOfTransfer
    {
        get
        {
            TransferType returnValue = TransferType.Manual;

            if (this.rvFull.Checked)
                returnValue = TransferType.Full;
            else if (this.rvManual.Checked)
                returnValue = TransferType.Manual;
            else if (this.rvAmount.Checked)
                returnValue = TransferType.Amount;
            return returnValue;

        }
        set
        {
            switch (value)
            {
                case TransferType.Full:
                    this.rvFull.Checked = true;
                    this.dbAmountToTransfer.Visible = false;
                    break;
                case TransferType.Amount:
                    this.rvAmount.Checked = true;
                    this.dbAmountToTransfer.Visible = true;
                    break;
                case TransferType.Manual:
                    this.rvManual.Checked = true;
                    this.dbAmountToTransfer.Visible = false;
                    break;
                default:
                    this.rvManual.Checked = true;
                    this.dbAmountToTransfer.Visible = false;
                    break;
            }
        }
    }

    protected int PositionTransferID
    {
        get
        {
            object i = ViewState["positionTransferID"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            if ((value != 0)) this.hdnTransferID.Value = value.ToString();
            ViewState["positionTransferID"] = value;
        }
    }

    protected bool IsInitialized
    {
        get
        {
            object i = ViewState["IsInitialized"];
            return ((i == null) ? false : (bool)i);
        }
        set { ViewState["IsInitialized"] = value; }
    }

    protected TransferStatus CurrentStatus
    {
        get
        {
            return (TransferStatus)(int.Parse(this.hdnCurrentStatus.Value));
        }
        set
        {
            this.hdnCurrentStatus.Value = ((int)value).ToString();
            if (CurrentStatus == TransferStatus.New)
                this.lblStatusValue.BackColor = System.Drawing.Color.LightGreen;
            else
                this.lblStatusValue.BackColor = System.Drawing.Color.MediumVioletRed;
            this.lblStatusValue.Text = CurrentStatus.ToString();
        }
    }

    public bool IsEditable
    {
        get
        {
            object i = ViewState["IsEditable"];
            return ((i == null) ? false : (bool)i);
        }
        set { ViewState["IsEditable"] = value; }
    }

    protected void gvPortfolioView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dataRowView = (DataRowView)e.Row.DataItem;

                if ((bool)dataRowView["IsChanged"])
                    e.Row.BackColor = System.Drawing.Color.LightSalmon;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNewLine_Click(object sender, EventArgs e)
    {
        try
        {
            ctlTransferPositions.InsertLine();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void ddlAccountA_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void ddlAccountA_DataBound(object sender, EventArgs e)
    {
    }
    protected void ddlAccountB_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void ddlAccountB_DataBound(object sender, EventArgs e)
    {
    }

    protected void rvAmount_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            bool visible = this.rvAmount.Checked;
            this.dbAmountToTransfer.Visible = this.lblAmountToTransfer.Visible = visible;
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnInitialize_Click(object sender, EventArgs e)
    {
        try
        {
            arrangeTransfer();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void arrangeTransfer()
    {
        TransferAdapter.PositionTransferDetails details = getDetails();
        this.PositionTransferID = TransferAdapter.SetupTransfer(details);
        displayTransferDetails();
    }


    protected void btnExecute_Click(object sender, EventArgs e)
    {
        try
        {
            TransferAdapter.PositionTransferDetails details = getDetails();
            if (TransferAdapter.ExecuteTransfer(details))
            {
                displayTransferDetails();
                ctlTransferPositions.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnFilterAccountA_Click(object sender, EventArgs e)
    {
        try
        {
            setFilterVisible(pnlAccountFinderA, btnFilterAccountA, !pnlAccountFinderA.Visible, VIEWS.AccountA);
            btnFilterAccountA.Focus();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
    protected void btnFilterAccountB_Click(object sender, EventArgs e)
    {
        try
        {
            setFilterVisible(pnlAccountFinderTo, btnFilterAccountB, !pnlAccountFinderTo.Visible, VIEWS.AccountB);
            btnFilterAccountB.Focus();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void setFilterVisible(Panel filterPanel, Button filterButton, bool visible, VIEWS view)
    {
        filterPanel.Visible = visible;
        filterButton.Text = "Filter  " + (visible ? "<<" : ">>");
    }

    protected void btnSaveDetails_Click(object sender, EventArgs e)
    {
        try
        {
            saveDetails();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void chkAIsInternal_OnCheckedChanged(object sender, EventArgs e)
    {
        try
        {
            setupControlsInitialised();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void chkBIsInternal_OnCheckedChanged(object sender, EventArgs e)
    {
        try
        {
            setupControlsInitialised();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private TransferAdapter.PositionTransferDetails getDetails()
    {
        TransferAdapter.PositionTransferDetails details = new TransferAdapter.PositionTransferDetails();
        details.Key = this.PositionTransferID;
        details.AIsInternal = this.chkAIsInternal.Checked;
        if (this.chkAIsInternal.Checked) details.AccountAID = int.Parse(this.ddlAccountA.SelectedValue);
        details.BIsInternal = this.chkBIsInternal.Checked;
        if (this.chkBIsInternal.Checked) details.AccountBID = int.Parse(this.ddlAccountB.SelectedValue);
        details.TransferDate = this.dpDateOfPortfolio.SelectedDate;
        details.TypeOfTransfer = TypeOfTransfer;
        if (details.TypeOfTransfer == TransferType.Amount) details.TransferAmount = this.dbAmountToTransfer.Value;
        return details;
    }

    private void saveDetails()
    {
        TransferAdapter.PositionTransferDetails details = getDetails();
        TransferAdapter.SavePositionTransferDetails(details);
    }

    private void displayTransferDetails()
    {
        TransferAdapter.PositionTransferDetails details = TransferAdapter.GetPositionTransfer(this.PositionTransferID);
        if (details != null)
        {
            this.chkAIsInternal.Checked = details.AIsInternal;
            if (details.AIsInternal) this.ddlAccountA.SelectedValue = details.AccountAID.ToString();

            this.chkBIsInternal.Checked = details.BIsInternal;
            if (details.BIsInternal) this.ddlAccountB.SelectedValue = details.AccountBID.ToString();
            
            this.TypeOfTransfer = details.TypeOfTransfer;
            if (details.TransferAmount != null) this.dbAmountToTransfer.Value = details.TransferAmount;

            //switch (details.TypeOfTransfer)
            //{
            //    case TransferType.Full:
            //        this.rvFull.Checked = true;
            //        break;
            //    case TransferType.Amount:
            //        this.rvAmount.Checked = true;
            //        this.txtAmountToTransfer.Visible = true;
            //        if (details.TransferAmount != null) this.txtAmountToTransfer.Text = details.TransferAmount.ToString();
            //        break;
            //    case TransferType.Manual:
            //        this.rvManual.Checked = true;
            //        break;
            //    default:
            //        this.rvFull.Checked = true;
            //        break;
            //}

            if (details.TransferDate != null) this.dpDateOfPortfolio.SelectedDate = details.TransferDate;
            this.CurrentStatus = details.Status;
            this.ctlTransferPositions.TransferID = PositionTransferID;
            this.IsInitialized = details.IsInitialised;
            this.IsEditable = details.IsEditable;
            setupControlsInitialised();
        }
    }

    private void setupControlsInitialised()
    {
        this.btnInitialize.Enabled = !this.IsInitialized && (this.chkAIsInternal.Checked || this.chkBIsInternal.Checked);
        this.chkAIsInternal.Enabled = !this.IsInitialized;
        this.chkBIsInternal.Enabled = !this.IsInitialized;
        this.btnFilterAccountB.Enabled = !this.IsInitialized;
        this.ddlAccountB.Enabled = !this.IsInitialized;
        this.dpDateOfPortfolio.Enabled = !this.IsInitialized;
        this.ddlAccountA.Enabled = this.btnFilterAccountA.Enabled = (this.chkAIsInternal.Checked && !this.IsInitialized);
        this.ddlAccountB.Enabled = this.btnFilterAccountB.Enabled = (this.chkBIsInternal.Checked && !this.IsInitialized);
        this.rvAmount.Enabled = !this.IsInitialized;
        rvAmount_CheckedChanged(null, null);
        this.rvFull.Enabled = !this.IsInitialized;
        this.rvManual.Enabled = !this.IsInitialized;
        //this.dbAmountToTransfer.ReadOnly = this.IsInitialized;
        this.dbAmountToTransfer.Enabled = !this.IsInitialized;
        this.btnExecute.Enabled = this.IsInitialized && !((this.CurrentStatus == TransferStatus.Executed) || (this.CurrentStatus == TransferStatus.Cancelled));
        this.btnNewLine.Enabled = this.IsInitialized && !((this.CurrentStatus == TransferStatus.Executed) || (this.CurrentStatus == TransferStatus.Cancelled));
        this.btnSaveDetails.Enabled = this.IsEditable;
    }
}

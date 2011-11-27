using System;
using System.Collections;
using System.Collections.Generic;
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
using B4F.TotalGiro.Stichting;
using System.Globalization;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Security;
using B4F.Web.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee;

public partial class FeeCalculationsOverview : System.Web.UI.Page
{
    protected enum EDITMODE
    {
        None = 0,
        Edit,
        Insert,
        Both
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Fee Calculations Overview";
                gvCalculations.Sort("Name", SortDirection.Ascending);
                gvCalcVersions.Sort("VersionNumber", SortDirection.Descending);
                gvCalcLines.Sort("SerialNo", SortDirection.Ascending);
                if (B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.IsLoggedInAsAssetManager())
                {
                    IAssetManager am = B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.getAssetManager();
                    if (am != null)
                        this.gvCalculations.Caption = "Current Calculations available to " + am.CompanyName;
                }
            }
            lblErrorMessage.Text = "";
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void initialSetup()
    {
        this.mlvVersion.ActiveViewIndex = 0;
        this.gvCalculations.DataBind();
        this.gvCalculations.SelectedIndex = -1;
        this.gvCalcVersions.Visible = false;
        this.gvCalcLines.Visible = false;
        this.CurrentEditMode = EDITMODE.None;

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvCalculations.DataBind();
    }

    protected void gvCalculations_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.mlvVersion.ActiveViewIndex = 0;
        this.gvCalcVersions.Visible = true;
        this.gvCalcLines.Visible = false;
    }

    protected void gvCalculations_PageIndexChanged(object sender, EventArgs e)
    {
        this.gvCalcVersions.Visible = false;
        this.gvCalcLines.Visible = false;
    }

    protected void gvCalculations_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvCalculations.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                if (this.CurrentEditMode == EDITMODE.None)
                {
                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITCALC":
                            setupEditMode(false, EDITMODE.Edit);
                            return;
                    }
                }
            }
        }
    }

    protected void gvCalculations_RowEditing(object sender, GridViewEditEventArgs e)
    {
        setCalculationsEditMode(true, e.NewEditIndex);
    }

    protected void gvCalculations_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        setCalculationsEditMode(false, -1);
    }

    protected void gvCalculations_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            YearMonthPicker ppEnd = (YearMonthPicker)(gvCalculations.Rows[e.RowIndex].FindControl("ppEnd"));
            e.NewValues["endPeriod"] = ppEnd.SelectedPeriod;
            setCalculationsEditMode(false, -1);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }

    }

    protected void gvCalculations_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (CalculationsRowIndex > -1 && e.Row.RowIndex == CalculationsRowIndex)
            {
                LinkButton lbtEditCalc = e.Row.FindControl("lbtEditCalc") as LinkButton;
                if (lbtEditCalc != null)
                    lbtEditCalc.Visible = false;
            }

            if (!(bool)((DataRowView)e.Row.DataItem)["IsActive"])
            {
                e.Row.Controls[e.Row.Controls.Count - 1].Visible = false;
            }
        }
    }

    protected int CalculationsRowIndex
    {
        get
        {
            object b = ViewState["CalculationsRowIndex"];
            return ((b == null) ? -1 : (int)b);
        }
        set 
        {
            if (value > -1)
                ViewState["CalculationsRowIndex"] = value;
            else
                ViewState["CalculationsRowIndex"] = null;
        }
    }

    private void setCalculationsEditMode(bool edit, int rowIndex)
    {
        gvCalculations.Columns[7].HeaderText = (edit ? "End Period" : "");
        CalculationsRowIndex = rowIndex;
    }

    protected void gvCalcVersions_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.gvCalcLines.Visible = true;
    }

    protected void gvCalcVersions_PageIndexChanged(object sender, EventArgs e)
    {
        this.gvCalcLines.Visible = false;
    }

    protected void cvEndPeriod_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = !(ppEndPeriod.SelectedPeriod > 0 && (ppStartPeriod.SelectedPeriod == 0 || ppStartPeriod.SelectedPeriod >= ppEndPeriod.SelectedPeriod));
    }

    protected void btnCreateNewCalc_Click(object sender, EventArgs e)
    {
        setupEditMode(true, EDITMODE.Insert);
    }

    protected void btnCancelChanges_Click(object sender, EventArgs e)
    {
        setupEditMode(false, EDITMODE.None);
        initialSetup();
    }

    protected void btnUpdateVersion_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                FeeCalcHelper fch = new FeeCalcHelper();
                fch.CalcID = CalcID;
                fch.CalcName = this.tbCalcName.Text;
                fch.FeeType = (B4F.TotalGiro.Fees.FeeTypes)Utility.GetKeyFromDropDownList(ddlFeeType);
                fch.CurrencyID = Utility.GetKeyFromDropDownList(ddlCurrency);
                fch.IsActive = this.chkIsActive.Checked;
                fch.StartPeriod = ppStartPeriod.SelectedPeriod;
                fch.EndPeriod = ppEndPeriod.SelectedPeriod;
                fch.FeeCalcType = (B4F.TotalGiro.Fees.FeeCalcTypes)Convert.ToInt32(rbSlabFlat.SelectedValue);
                fch.NoFees = chkNoFees.Checked;
                fch.SetUp = dbSetUp.Value;
                fch.MinValue = dbMinValue.Value;
                fch.MaxValue = dbMaxValue.Value;

                int index = 0;
                FlatSlab flatslabcontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}", index++));
                while (flatslabcontrol != null)
                {
                    fch.Lines.Add(createFeeCalcLineHelper(flatslabcontrol));
                    flatslabcontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}", index++));
                }

                FeeCalculationsOverviewAdapter.UpdateCalcVersion(fch);
                setupEditMode(false, EDITMODE.None);
                initialSetup();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    /// <summary>
    /// Creates a CommCalcLineView object based on the FlatSlab control input
    /// </summary>
    /// <param name="uc">The FlatSlab user control</param>
    /// <returns>A CommCalcLineView object</returns>
    private FeeCalcLineHelper createFeeCalcLineHelper(FlatSlab uc)
    {
        FeeCalcLineHelper line = new FeeCalcLineHelper();

        line.FeePercentage = uc.Percentage;
        line.LowerRange = uc.FromRange;
        line.StaticCharge = uc.StaticCharge;
        return line;
    }

    private void setupEditMode(bool newCalc, EDITMODE currentEditMode)
    {
        if (CurrentEditMode == EDITMODE.None)
        {
            this.CalcID = 0;
            this.tbCalcName.Text = "";
            if (ddlFeeType.Items.Count > 0)
                this.ddlFeeType.SelectedIndex = 0;
            this.ddlFeeType.Enabled = true;
            ddlCurrency.SelectedValue = "600";
            this.ddlCurrency.Enabled = true;
            this.chkIsActive.Checked = true;
            this.chkIsActive.Enabled = false;
            this.ppStartPeriod.Clear();
            this.ppEndPeriod.Clear();
            this.rbSlabFlat.SelectedValue = "3";
            rbSlabFlat_SelectedIndexChanged(null, null);
            this.chkNoFees.Checked = false;
            this.dbSetUp.Clear();
            this.dbMinValue.Clear();
            this.dbMaxValue.Clear();

            this.NumberOfRanges = 0;

            if (this.gvCalculations.SelectedValue != null)
                this.CalcID = (int)this.gvCalculations.SelectedValue;
            if (!newCalc)
            {
                this.CalcID = (int)this.gvCalculations.SelectedValue;
                FeeCalcHelper calc = FeeCalculationsOverviewAdapter.GetFeeCalculation(this.CalcID);
                this.tbCalcName.Text = calc.CalcName;
                this.ddlFeeType.SelectedValue = ((int)calc.FeeType).ToString();
                this.ddlFeeType.Enabled = false;
                this.ddlCurrency.SelectedValue = ((int)calc.CurrencyID).ToString();
                this.ddlCurrency.Enabled = false;
                this.chkIsActive.Checked = calc.IsActive;
                this.chkIsActive.Enabled = true;
                this.ppStartPeriod.SelectedPeriod = calc.StartPeriod;
                this.ppEndPeriod.SelectedPeriod = calc.EndPeriod;
                this.rbSlabFlat.SelectedValue = ((int)calc.FeeCalcType).ToString();
                rbSlabFlat_SelectedIndexChanged(null, null);
                this.chkNoFees.Checked = calc.NoFees;
                chkNoFees_CheckedChanged(null, null);
                if (calc.SetUp != 0M)
                    this.dbSetUp.Value = calc.SetUp;
                if (calc.MinValue != 0M)
                    this.dbMinValue.Value = calc.MinValue;
                if (calc.MaxValue != 0M)
                    this.dbMaxValue.Value = calc.MaxValue;

                foreach (FeeCalcLineHelper line in calc.Lines)
                {
                    FlatSlab flatslabcontrol = CreateFlatSlabControl(line.SerialNo);

                    flatslabcontrol.dBoxFrom.Value = line.LowerRange;
                    flatslabcontrol.dBoxPercent.Value = line.FeePercentage;
                    flatslabcontrol.dBoxStaticCharge.Value = line.StaticCharge;

                    this.FlatSlabPlaceHolder.Controls.Add(flatslabcontrol);
                }
                this.NumberOfRanges = calc.Lines.Count;
                btnUpdateVersion.Focus();
            }
        }

        CurrentEditMode = currentEditMode;
        switch (CurrentEditMode)
        {
            case EDITMODE.Edit:
                btnCreateNewCalc.Enabled = false;
                txtCalcName.Enabled = false;
                ddlActiveStatus.Enabled = false;
                btnSearch.Enabled = false;
                break;
            case EDITMODE.Insert:
                btnCreateNewCalc.Enabled = false;
                txtCalcName.Enabled = false;
                ddlActiveStatus.Enabled = false;
                btnSearch.Enabled = false;
                break;
            case EDITMODE.Both:
                break;
            default:
                btnCreateNewCalc.Enabled = true;
                txtCalcName.Enabled = true;
                ddlActiveStatus.Enabled = true;
                btnSearch.Enabled = true;
                break;
        }

        mlvVersion.ActiveViewIndex = 1;
        this.btnUpdateVersion.Enabled = true;
    }

    protected Int32 CalcID
    {
        get
        {
            object i = ViewState["CalcID"];
            return ((i == null) ? 0 : (Int32)i);
        }
        set { ViewState["CalcID"] = value; }
    }

    protected EDITMODE CurrentEditMode
    {
        get
        {
            object i = ViewState["CurrentEditMode"];
            return ((i == null) ? EDITMODE.None : (EDITMODE)i);
        }
        set 
        { 
            ViewState["CurrentEditMode"] = value;
            gvCalculations.Enabled = (value == EDITMODE.None);
        }
    }

    /// <summary>
    /// Custom validation to check the sequence of the flatslab definitions. The numbers in the from field must be filled
    /// and must be greater than the value of the previous range.
    /// </summary>
    /// <param name="source">source object of the calling validation</param>
    /// <param name="args">specific validation arguments</param>
    protected void checkRanges(object source, ServerValidateEventArgs args)
    {
        try
        {
            args.IsValid = true;

            decimal lastvalue = 0M;
            for (int index = 1; index < this.NumberOfRanges; index++)
            {
                FlatSlab fscontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}", index));
                if (fscontrol != null)
                {
                    if (lastvalue >= fscontrol.dBoxFrom.Value)
                    {
                        args.IsValid = false;
                        return;
                    }

                    lastvalue = fscontrol.dBoxFrom.Value;
                }
            }
        }
        catch (Exception)
        {
            args.IsValid = false;
        }

    }

    /// <summary>
    /// Custom validation to check the setup value
    /// </summary>
    /// <param name="source">source object of the calling validation</param>
    /// <param name="args">specific validation arguments</param>
    protected void CustSetUp(object source, ServerValidateEventArgs args)
    {
        bool boolNoSetUp = false;
        if (args.Value.Length == 0)
            boolNoSetUp = true;
        else if (Double.Parse(args.Value) == 0.0)
            boolNoSetUp = true;

        if ((boolNoSetUp && !chkNoFees.Checked) || rbSlabFlat.SelectedValue != "3")
        {
            FlatSlab firstflatslabcontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl("flatslab_0");
            if (firstflatslabcontrol == null || (firstflatslabcontrol != null && firstflatslabcontrol.dBoxPercent.IsEmpty))
            {
                args.IsValid = false;
            }
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (rbSlabFlat.SelectedValue == "3")
        {
            btnAddNewFlatSlabRange.Enabled = false;
            btnDeleteFlatSlabRange.Enabled = false;
        }
        else
        {
            btnAddNewFlatSlabRange.Enabled = true;
            btnDeleteFlatSlabRange.Enabled = true;
        }

        base.OnPreRender(e);
    }
    /// <summary>
    /// Overridden method that takes care of the generation of flatslab controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        if (IsPostBack)
            CreateFlatSlabControls();

        base.CreateChildControls();
    }

    /// <summary>
    /// Creates a FlatSlab User control with a given index number
    /// </summary>
    /// <param name="fsindex">Index number of the flatslab range (0 based)</param>
    /// <returns>A FlatSlab User control</returns>
    protected FlatSlab CreateFlatSlabControl(int fsindex)
    {
        FlatSlab fsctltemplate = (FlatSlab)LoadControl("../../UC/FlatSlab.ascx");
        fsctltemplate.ID = string.Format("flatslab_{0}", fsindex);
        fsctltemplate.dBoxFrom.ID = string.Concat(fsctltemplate.ID + "_From");
        if (fsindex == 0)
            fsctltemplate.dBoxFrom.ReadOnly = true;
        fsctltemplate.ShowStaticCharge = true;
        fsctltemplate.PercentageLabel = "Fee";
        fsctltemplate.dBoxPercent.ID = string.Concat(fsctltemplate.ID + "_Percent");
        fsctltemplate.RangeValidatorFrom.ControlToValidate = fsctltemplate.dBoxFrom.ID + ":tbDecimal";
        fsctltemplate.RangeValidatorPercent.ControlToValidate = fsctltemplate.dBoxPercent.ID + ":tbDecimal";
        return fsctltemplate;
    }

    /// <summary>
    /// Creates a number of flatslab user controls
    /// </summary>
    protected void CreateFlatSlabControls()
    {
        for (int index = 0; index < this.NumberOfRanges; index++)
        {
            FlatSlab flatslabcontrol = CreateFlatSlabControl(index);

            this.FlatSlabPlaceHolder.Controls.Add(flatslabcontrol);
        }
    }

    /// <summary>
    /// The number of flatslab ranges that is displayed
    /// </summary>
    protected int NumberOfRanges
    {
        get
        {
            if (this.Session["nranges"] == null)
                this.Session["nranges"] = 0;
            return (int)this.Session["nranges"];
        }
        set { this.Session["nranges"] = value; }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CalculationsOverview.aspx");
    }


    protected void btnAddNewFlatSlabRange_Click(object sender, EventArgs e)
    {
        FlatSlab flatslabcontrol = CreateFlatSlabControl(this.NumberOfRanges);
        this.FlatSlabPlaceHolder.Controls.Add(flatslabcontrol);
        this.NumberOfRanges = this.NumberOfRanges + 1;
        btnUpdateVersion.Focus();
    }

    protected void btnDeleteFlatSlabRange_Click(object sender, EventArgs e)
    {
        // Remove the last control from the list already built during createchildcontrols
        Control lastfscontrol = this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}", this.NumberOfRanges - 1));
        if (lastfscontrol != null)
            this.FlatSlabPlaceHolder.Controls.Remove(lastfscontrol);
        this.NumberOfRanges = this.NumberOfRanges - 1;
        btnUpdateVersion.Focus();
    }

    protected void rbSlabFlat_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool isSimple = (rbSlabFlat.SelectedItem.Value == "3");
        chkNoFees.Enabled = isSimple;
        chkNoFees.Checked = false;
        chkNoFees_CheckedChanged(null, e);
        btnAddNewFlatSlabRange.Enabled = !isSimple;
        btnDeleteFlatSlabRange.Enabled = !isSimple;
        dbMinValue.Enabled = !isSimple;
        dbMinValue.BackColor = (isSimple ? System.Drawing.Color.WhiteSmoke : txtCalcName.BackColor);
        dbMaxValue.Enabled = !isSimple;
        dbMaxValue.BackColor = (isSimple ? System.Drawing.Color.WhiteSmoke : txtCalcName.BackColor);
        if (isSimple)
        {
            dbMinValue.Clear();
            dbMaxValue.Clear();
            // Delete all slabfalt rules as they are not used for a simple definition
            this.FlatSlabPlaceHolder.Controls.Clear();
            this.NumberOfRanges = 0;
        }
        btnUpdateVersion.Focus();
    }

    protected void chkNoFees_CheckedChanged(object sender, EventArgs e)
    {
        bool noFees = chkNoFees.Checked;
        if (noFees)
            dbSetUp.Clear();
        dbSetUp.Enabled = !noFees;
        dbSetUp.BackColor = (noFees ? System.Drawing.Color.WhiteSmoke : txtCalcName.BackColor);
    }
}

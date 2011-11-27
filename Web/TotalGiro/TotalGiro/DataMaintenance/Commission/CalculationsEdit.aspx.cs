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
using System.ComponentModel;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission;
using System.Drawing;

public partial class Calculations : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

        if (!IsPostBack)
		{
            int id = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "id");
            if (CommCurrencySymbol == "") ;
                CommCurrencySymbol = CalculationsEditAdapter.GetCurrencySymbol();
			loadRecord(id);
			hidIdValue.Value = id.ToString();
		}
	}

    [Description("CommCurrencySymbol."), Category("Behaviour")]
    public string CommCurrencySymbol
    {
        get { return ViewState["CommCurrencySymbol"] != null ? (string)ViewState["CommCurrencySymbol"] : ""; }
        set { ViewState["CommCurrencySymbol"] = value; }
    }

    /// <summary>
    /// Loads a calculation into the controls on screen
    /// </summary>
    /// <param name="id">Identifiaction id of the calculation.</param>
    private void loadRecord(int id)
    {
        if (id != 0)
        {
            ((EG)this.Master).setHeaderText = "Edit Commission Calculation in €";

            CommCalcView commCalcView = CalculationsEditAdapter.LoadRecord(id);

            tbCalcName.Text = commCalcView.Name;

            rbSlabFlat.SelectedValue = commCalcView.CalcType.ToString();
            rbSlabFlat_SelectedIndexChanged(null, null);

            rbSlabFlat.Enabled = false;

            dbMinimum.Value = commCalcView.MinValue;
            if (commCalcView.MaxValue.HasValue)
                dbMaximum.Value = commCalcView.MaxValue.Value;
            dbSetUp.Value = commCalcView.FixedSetup;

            foreach (CommCalcLineView lineView in commCalcView.LineViews)
            {
                FlatSlab flatslabcontrol = CreateFlatSlabControl(lineView.SerialNo);

                flatslabcontrol.dBoxFrom.Value = lineView.LowerRange;
                flatslabcontrol.dBoxPercent.Value = lineView.FeePercentage;
                flatslabcontrol.dBoxStaticCharge.Value = lineView.StaticCharge;
                flatslabcontrol.dBoxTariff.Value = lineView.Tariff;
                flatslabcontrol.IsAmountBased = lineView.IsAmountBased;

                this.FlatSlabPlaceHolder.Controls.Add(flatslabcontrol);

            }
            this.NumberOfRanges = commCalcView.LineViews.Count;

        }
        else
        {
            this.NumberOfRanges = 0;
            ((EG)this.Master).setHeaderText = "Add Commission Calculation in €";
        }
    }

    /// <summary>
    /// Creates a CommCalcLineView object based on the FlatSlab control input
    /// </summary>
    /// <param name="uc">The FlatSlab user control</param>
    /// <returns>A CommCalcLineView object</returns>
    private CommCalcLineView createCommCalcLineView(FlatSlab uc)
    {
        CommCalcLineView lineView = new CommCalcLineView();

        lineView.FeePercentage = uc.dBoxPercent.Value;
        lineView.LowerRange = uc.dBoxFrom.Value;
        lineView.StaticCharge = uc.dBoxStaticCharge.Value;
        lineView.Tariff = uc.dBoxTariff.Value;
        lineView.IsAmountBased = uc.IsAmountBased;

        return lineView;
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
		{
			boolNoSetUp = true;
		}
		else if (Double.Parse(args.Value) == 0.0)
		{
			boolNoSetUp = true;
		}
        FlatSlab firstflatslabcontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl("flatslab_0");
        if (firstflatslabcontrol != null && boolNoSetUp && 
            ((firstflatslabcontrol.IsAmountBased && firstflatslabcontrol.dBoxPercent.IsEmpty) ||
             (!firstflatslabcontrol.IsAmountBased && firstflatslabcontrol.dBoxTariff.IsEmpty)))
		{
			args.IsValid = false;
		}
	}


    protected override void OnPreRender(EventArgs e)
    {
        if (rbSlabFlat.SelectedValue == "3")
        {
            dbMinimum.Enabled = false;
            dbMinimum.Clear();
            dbMinimum.BackColor = Color.LightGray;
            dbMaximum.Enabled = false;
            dbMaximum.Clear();
            dbMaximum.BackColor = Color.LightGray;
            btnAddNewFlatSlabRange.Enabled = false;
            btnDeleteFlatSlabRange.Enabled = false;
        }
        else
        {
            dbMinimum.Enabled = true;
            dbMinimum.BackColor = Color.White;
            dbMaximum.Enabled = true;
            dbMaximum.BackColor = Color.White;
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
        fsctltemplate.IsAmountBased = rbSlabFlat.SelectedValue != "4";
        fsctltemplate.CommCurrencySymbol = CommCurrencySymbol;
        if (fsindex == 0)
            fsctltemplate.dBoxFrom.ReadOnly = true;
        fsctltemplate.dBoxPercent.ID = string.Concat(fsctltemplate.ID + "_Percent");
        fsctltemplate.ShowStaticCharge = true;
        fsctltemplate.RangeValidatorFrom.ControlToValidate = fsctltemplate.dBoxFrom.ID + ":tbDecimal";
        fsctltemplate.RangeValidatorPercent.ControlToValidate = fsctltemplate.dBoxPercent.ID + ":tbDecimal";
        fsctltemplate.RangeValidatorTariff.ControlToValidate = fsctltemplate.dBoxTariff.ID + ":tbDecimal";
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
            return (int) this.Session["nranges"];
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
        flatslabcontrol.IsAmountBased = rbSlabFlat.SelectedValue != "4";

        this.FlatSlabPlaceHolder.Controls.Add(flatslabcontrol);

        this.NumberOfRanges = this.NumberOfRanges + 1;
        checkFlatSlabCount();
    }

    protected void btnDeleteFlatSlabRange_Click(object sender, EventArgs e)
    {
        // Remove the last control from the list already built during createchildcontrols
        Control lastfscontrol = this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}",this.NumberOfRanges - 1));
        if (lastfscontrol != null)
            this.FlatSlabPlaceHolder.Controls.Remove(lastfscontrol);
        this.NumberOfRanges = this.NumberOfRanges - 1;
        checkFlatSlabCount();
    }

    protected void checkFlatSlabCount()
    {
        rbSlabFlat.Enabled = (this.FlatSlabPlaceHolder.Controls.Count == 0);
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {

        if (Page.IsValid)
        {
            try
            {
                int id = int.Parse(hidIdValue.Value);

                CommCalcView commCalcView = new CommCalcView();
                commCalcView.Name = tbCalcName.Text;
                commCalcView.CalcType = int.Parse(rbSlabFlat.SelectedValue);
                commCalcView.MinValue = dbMinimum.Value;
                if (!string.IsNullOrEmpty(dbMaximum.Text))
                    commCalcView.MaxValue = dbMaximum.Value;
                commCalcView.FixedSetup = dbSetUp.Value;

                int index = 0;
                FlatSlab flatslabcontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}", index++));
                while (flatslabcontrol != null)
                {
                    commCalcView.LineViews.Add(createCommCalcLineView(flatslabcontrol));

                    flatslabcontrol = (FlatSlab)this.FlatSlabPlaceHolder.FindControl(string.Format("flatslab_{0}", index++));
                }

                CalculationsEditAdapter.SaveRecord(id, commCalcView);

                Response.Redirect("CalculationsOverview.aspx");
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }


    protected void rbSlabFlat_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnAddNewFlatSlabRange.Enabled = true;
        btnDeleteFlatSlabRange.Enabled = true;

        switch (rbSlabFlat.SelectedItem.Value)
        {
            case "3":
                btnAddNewFlatSlabRange.Enabled = false;
                btnDeleteFlatSlabRange.Enabled = false;

                // Delete all slabfalt rules as they are not used for a simple definition
                this.FlatSlabPlaceHolder.Controls.Clear();
                this.NumberOfRanges = 0;
                break;
        }
    }
}

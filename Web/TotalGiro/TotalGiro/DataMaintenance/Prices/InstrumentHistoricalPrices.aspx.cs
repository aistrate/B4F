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
using B4F.TotalGiro.Security;

public partial class InstrumentHistoricalPrices : System.Web.UI.Page
{
    private int priceColumnIndex;

    protected void Page_Init(object sender, EventArgs e)
    {
        cldpDate.DateChanged += new EventHandler(cldpDate_DateChanged);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Instrument Historical Prices";
            gvInstrumentPrices.Sort("InstrumentName", SortDirection.Ascending);
            this.userInRole = SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Historical Prices Maintenance");

            if (!userInRole)
            {
                gvInstrumentPrices.Columns[gvInstrumentPrices.Columns.Count - 1].Visible = false;
                gvInstrumentPrices.Columns[gvInstrumentPrices.Columns.Count - 2].Visible = false;
                gvCurrencyRates.Columns[gvCurrencyRates.Columns.Count - 1].Visible = false;
                gvCurrencyRates.Columns[gvCurrencyRates.Columns.Count - 2].Visible = false;
                gvBenchMarks.Columns[gvBenchMarks.Columns.Count - 1].Visible = false;
                gvBenchMarks.Columns[gvBenchMarks.Columns.Count - 2].Visible = false;
            }
        }

        lblErrorMessage.Text = "";
        priceColumnIndex = gvInstrumentPrices.Columns.Count - 3;
    }

    public bool UserInRole
    {
        get { return userInRole; }
    }

    protected void btnHideMissingPrices_Click(object sender, EventArgs e)
    {
        btnHideMissingPrices.Visible = false;
        gvMissingPrices.Visible = false;
        gvMissingExRates.Visible = false;
    }

    protected void cldpDate_DateChanged(object sender, EventArgs e)
    {
        this.rblDataSourceChoice.Visible = true;
        rblDataSourceChoice_SelectedIndexChanged(this, new EventArgs());
        this.ctlInstrumentFinder.Visible = true;
    }

    protected void rblDataSourceChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.mlvHistoricalPrices.ActiveViewIndex = this.rblDataSourceChoice.SelectedIndex;
        switch (this.mlvHistoricalPrices.ActiveViewIndex)
        {
            case 0:
                gvInstrumentPrices.DataBind();
                this.gvBenchMarks.EditIndex = -1;
                this.gvCurrencyRates.EditIndex = -1;
                break;
            case 1:
                this.gvCurrencyRates.DataBind();
                this.gvBenchMarks.EditIndex = -1;
                this.gvInstrumentPrices.EditIndex = -1;
                this.gvMissingPrices.Visible = false;
                break;
            case 2:
                this.gvBenchMarks.DataBind();
                this.gvInstrumentPrices.EditIndex = -1;
                this.gvMissingPrices.Visible = false;
                this.gvCurrencyRates.EditIndex = -1;
                break;
            default:
                break;
        }

        switch (this.rblDataSourceChoice.SelectedIndex)
        {
            case (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.Currencies:
                ctlInstrumentFinder.ISINLabel = "Symbol:";
                break;
            default:
                ctlInstrumentFinder.ISINLabel = "ISIN:";
                break;
        }
        this.mlvHistoricalPrices.ActiveViewIndex = this.rblDataSourceChoice.SelectedIndex;
    }

    protected void gvSender_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            string errorMessage = null;
            decimal newQuantity = 0M;
            GridView gvSender = (GridView)sender;
            DecimalBox dbNewQuantity = (DecimalBox)(gvSender.Rows[e.RowIndex].FindControl("dbNewQuantity"));
            if (dbNewQuantity != null)
                newQuantity = dbNewQuantity.Value;

            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Historical Prices Maintenance"))
                errorMessage = "Only Stichting employees are allowed to add/update instrument prices.";
            else if (newQuantity <= 0m)
            {
                switch (gvSender.ID)
                {
                    case "gvInstrumentPrices":
                    case "gvMissingPrices":
                        errorMessage = "Price ";
                        break;
                    case "gvCurrencyRates":
                    case "gvMissingExRates":
                        errorMessage = "Exchange Rate ";
                        break;
                    case "gvBenchMarks":
                        errorMessage = "Benchmark ";
                        break;
                }
                errorMessage += " must be greater than zero.";
            }

            if (errorMessage != null)
            {
                lblErrorMessage.Text = errorMessage;
                DialogMode = true;
                e.Cancel = true;
            }
            else
            {
                e.NewValues["newQuantity"] = newQuantity;
                e.NewValues["ignoreWarning"] = IgnoreWarnings;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvSender_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(e.Exception) + "Are you sure you want to insert this price?";
            DialogMode = true;
        }
        else
        {
            DialogMode = false;
        }
    }

    protected void gvInstrumentPrices_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvInstrumentPrices.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                switch (e.CommandName.ToUpper())
                {
                    case "MISSINGPRICES":
                        gvMissingPrices.Visible = true;
                        btnHideMissingPrices.Visible = true;
                        int instrumentId = (int)gvInstrumentPrices.SelectedDataKey.Value;
                        gvMissingPrices.Caption = "Missing Prices for " + InstrumentHistoricalPricesAdapter.GetInstrumentDescription(instrumentId);
                        return;
                }
            }
        }
        gvInstrumentPrices.SelectedIndex = -1;
    }

    protected void gvCurrencyRates_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvCurrencyRates.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                switch (e.CommandName.ToUpper())
                {
                    case "MISSINGRATES":
                        gvMissingExRates.Visible = true;
                        btnHideMissingPrices.Visible = true;
                        int currencyId = (int)gvCurrencyRates.SelectedDataKey.Value;
                        gvMissingExRates.Caption = "Missing Exchange Rates for " + CurrencyHistoricalRatesAdapter.GetCurrencyDescription(currencyId);
                        return;
                }
            }
        }
        gvCurrencyRates.SelectedIndex = -1;
    }

    protected void rblDialog_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView gvCurrent = ActiveGridview;
        if (gvCurrent != null && gvCurrent.EditIndex > -1)
        {
            if (rblDialog.SelectedValue == "1")
                gvCurrent.UpdateRow(gvCurrent.EditIndex, false);
            else
                gvCurrent.EditIndex = -1;
        }
        DialogMode = false;
        gvCurrent.Enabled = true;
    }

    protected GridView ActiveGridview
    {
        get
        {
            GridView gvCurrent = null;
            switch (this.mlvHistoricalPrices.ActiveViewIndex)
            {
                case 0:
                    if (gvMissingPrices.EditIndex > -1)
                        gvCurrent = gvMissingPrices;
                    else
                        gvCurrent = gvInstrumentPrices;
                    break;
                case 1:
                    if (gvMissingExRates.EditIndex > -1)
                        gvCurrent = gvMissingExRates;
                    else
                        gvCurrent = gvCurrencyRates;
                    break;
                case 2:
                    gvCurrent = gvBenchMarks;
                    break;
            }
            return gvCurrent;
        }
    }

    private bool IgnoreWarnings
    {
        get { return rblDialog.Visible && rblDialog.SelectedValue == "1"; }
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
                rblDialog.Focus();
            }
            GridView gvCurrent = ActiveGridview;
            if (gvCurrent != null)
                gvCurrent.Enabled = !value;
            ViewState["DialogMode"] = value;
        }
    }

    #region Privates

    private bool userInRole;
    private int dataChoice = 0;

    #endregion
}

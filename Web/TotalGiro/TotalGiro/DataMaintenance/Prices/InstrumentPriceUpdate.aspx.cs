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
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using Dundas.Charting.WebControl;
using System.Drawing;
using B4F.TotalGiro.Utils.Tuple;

public partial class InstrumentPriceUpdate : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
        cldDateFrom.DateChanged += new EventHandler(cldDate_DateChanged);
        cldDateTo.DateChanged += new EventHandler(cldDate_DateChanged);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Instrument Price Update";
            gvPriceHistory.Sort("Date", SortDirection.Ascending);
            this.rblDataSourceChoice.SelectedIndex = (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.InstrumentsTradeable;

            if (!IsEditingAllowed)
            {
                gvPriceHistory.Columns[gvPriceHistory.Columns.Count - 1].Visible = false;
                dbNewPrice.Visible = false;
                btnUpdatePriceRange.Visible = false;
                lblNewPrice.Visible = false;
                lblNewPriceCurrencyLabel.Visible = false;
            }

            if (Session["instrumentid"] != null)
            {
                int instrumentId = (int)Session["instrumentid"];
                Session["instrumentid"] = null;
                Tuple<string, DateTime, DateTime, bool> details = InstrumentPriceUpdateAdapter.GetTradeableInstrumentPriceDetails(instrumentId);
                DateTime startDate = DateTime.Today.AddMonths(-1);
                if (details != null)
                {
                    ctlInstrumentFinder.Isin = details.Item1;
                    startDate = details.Item2;
                    if (details.Item3 != DateTime.MinValue)
                        cldDateTo.SelectedDate = details.Item3;
                    ctlInstrumentFinder.ActivityFilter = details.Item4 ? ActivityReturnFilter.Active : ActivityReturnFilter.InActive;
                }
                rblDataSourceChoice.DataBind();
                filterInstrumnents(instrumentId, startDate);
            }
        }
        lblErrorMessage.Text = "";
    }

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        filterInstrumnents(0, DateTime.Today.AddMonths(-1));
    }

    protected void filterInstrumnents(int instrumentId, DateTime startDate)
    {
        pnlSelectedInstrument.Visible = true;
        if (Util.IsNullDate(cldDateFrom.SelectedDate))
            cldDateFrom.SelectedDate = startDate;
        ddlSelectedInstrument.DataBind();
        gvPriceHistory.ClearSelection();

        if (ddlSelectedInstrument.Items.Count != 2)
        {
            if (instrumentId == 0)
                ddlSelectedInstrument.SelectedIndex = 0;
            else
            {
                ddlSelectedInstrument.SelectedValue = instrumentId.ToString();
                ddlSelectedInstrument_SelectedIndexChanged(ddlSelectedInstrument, EventArgs.Empty);
            }
        }
        else
        {
            ddlSelectedInstrument.SelectedIndex = 1;
            ddlSelectedInstrument_SelectedIndexChanged(ddlSelectedInstrument, EventArgs.Empty);
        }
    }

    protected void cldDate_DateChanged(object sender, EventArgs e)
    {
        databind();
    }

    protected void rblDataSourceChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        //this.mlvPricesView.ActiveViewIndex = this.rblDataSourceChoice.SelectedIndex;
        switch (this.rblDataSourceChoice.SelectedIndex)
        {
            case (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.Currencies:
                ctlInstrumentFinder.ISINLabel = "Symbol:";
                break;
            default:
                ctlInstrumentFinder.ISINLabel = "ISIN:";
                break;
        }
        pnlSelectedInstrument.Visible = false;
        tbPriceHistory.Visible = false;
        gvPriceHistory.EditIndex = -1;
        gvPriceHistory.ClearSelection();
        DialogMode = false;
        EditMode = false;
    }

    protected void ddlSelectedInstrument_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            tbPriceHistory.Visible = true;
            gvPriceHistory.EditIndex = -1;
            gvPriceHistory.ClearSelection();
            DialogMode = false;
            EditMode = false;
            databind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void tbPriceHistory_ActiveTabChanged(object sender, EventArgs e)
    {
        try
        {
            DialogMode = false;
            EditMode = false;
            //AjaxControlToolkit.TabPanel[] panels = new AjaxControlToolkit.TabPanel[] { pnlPriceHistory, pnlGraph };
            //for (int i = 0; i < 3; i++)
            //    panels[i].Visible = (tbView.ActiveTabIndex == i);

            switch (tbPriceHistory.ActiveTabIndex)
            {
                case 1:
                    priceHistoryGraphDataBind();
                    break;
                default:
                    //gvPriceHistory.DataBind();
                    break;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    #region helpers

    private void databind()
    {
        if (ddlSelectedInstrument.SelectedIndex == 0)
            return;
        switch (tbPriceHistory.ActiveTabIndex)
        {
            case 1:
                priceHistoryGraphDataBind();
                break;
        }
    }

    protected string getInstrumentType()
    {
        switch (this.rblDataSourceChoice.SelectedIndex)
        {
            case (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.Currencies:
                return "Exchange Rate ";
                break;
            case (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.BenchMarks:
                return "Benchmark ";
                break;
            default:
                return "Price ";
                break;
        }
    }

    protected string getInstrumentName()
    {
        string caption = "History ";
        switch (this.rblDataSourceChoice.SelectedIndex)
        {
            case (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.Currencies:
                caption = "Exchange Rate " + caption;
                break;
            default:
                caption = "Price " + caption;
                break;
        }
        string instrumentDetails = InstrumentPriceUpdateAdapter.GetInstrumentDetails(Utility.GetKeyFromDropDownList(ddlSelectedInstrument));
        return caption + (string.IsNullOrEmpty(instrumentDetails) ? "" : "-> " + instrumentDetails);
    }

    protected bool IsEditingAllowed
    {
        get { return SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Historical Prices Maintenance"); }
    }

    #endregion

    #region Grid

    protected void gvPriceHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            string errorMessage = null;
            decimal newQuantity = 0M;
            GridView gvSender = (GridView)sender;
            DecimalBox dbNewQuantity = (DecimalBox)(gvSender.Rows[e.RowIndex].FindControl("dbNewQuantity"));
            if (dbNewQuantity != null)
                newQuantity = dbNewQuantity.Value;

            if (!IsEditingAllowed)
                errorMessage = "Only Stichting employees are allowed to add/update instrument prices.";
            else if (newQuantity <= 0m)
                errorMessage += getInstrumentType() +"must be greater than zero.";

            if (errorMessage != null)
            {
                lblErrorMessage.Text = errorMessage;
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


    protected void gvPriceHistory_RowUpdated(object sender, GridViewUpdatedEventArgs e)
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
            EditMode = false;
        }
    }

    public void gvPriceHistory_RowEditing(object sender, GridViewEditEventArgs e)
    {
        EditMode = true;
    }

    public void gvPriceHistory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvPriceHistory.EditIndex = -1;
        gvPriceHistory.DataBind();
        DialogMode = false;
        EditMode = false;
    }

    protected void gvPriceHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex == 0)
        {
            int rowCount = ((DataRowView)e.Row.DataItem).Row.Table.Rows.Count;
            gvPriceHistory.Caption = string.Format("{0} ({1} days)", getInstrumentName(), rowCount);
        }
    }

    protected void gvPriceHistory_DataBound(object sender, EventArgs e)
    {
        if (gvPriceHistory.Rows.Count == 0)
            gvPriceHistory.Caption = getInstrumentName();
    }

    protected void rblIgnoreWarning_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (gvPriceHistory.EditIndex > -1)
        {
            if (rblIgnoreWarning.SelectedValue == "1")
                gvPriceHistory.UpdateRow(gvPriceHistory.EditIndex, false);
            else
                gvPriceHistory.EditIndex = -1;
        }
        else if (!EditMode)
        {
            if (rblIgnoreWarning.SelectedValue == "1")
            {
                string errMessage;
                if (!updatePriceRange(true, out errMessage))
                    lblErrorMessage.Text = errMessage;
            }

        }
        DialogMode = false;
        EditMode = false;
    }

    protected void btnUpdatePriceRange_Click(object sender, EventArgs e)
    {
        try
        {
            if (!IsEditingAllowed)
            {
                lblErrorMessage.Text = "Only Stichting employees are allowed to add/update instrument prices.";
                return;
            }

            string errMessage;
            if (!updatePriceRange(false, out errMessage))
            {
                lblErrorMessage.Text = errMessage + "<br/>Are you sure you want to insert this price?";
                DialogMode = true;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected bool updatePriceRange(bool pushItThrough, out string errMessage)
    {
        bool success = false;
        errMessage = "";
        if (InstrumentPriceUpdateAdapter.UpdateInstrumentPriceHistory(
            gvPriceHistory.GetSelectedIds(),
            Utility.GetKeyFromDropDownList(ddlSelectedInstrument),
            dbNewPrice.Value,
            pushItThrough ? true : IgnoreWarnings, out errMessage))
        {
            gvPriceHistory.DataBind();
            gvPriceHistory.ClearSelection();
            success = true;
        }
        return success;
    }


    protected void cvCheckUpdatePriceRange_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (gvPriceHistory.GetSelectedIds().Length > 0);
    }

    private bool IgnoreWarnings
    {
        get { return rblIgnoreWarning.Visible && rblIgnoreWarning.SelectedValue == "1"; }
    }

    private bool DialogMode
    {
        get { return rblIgnoreWarning.Visible; }
        set
        {
            rblIgnoreWarning.Visible = value;
            if (value)
            {
                rblIgnoreWarning.SelectedIndex = -1;
                rblIgnoreWarning.Focus();
            }
        }
    }

    private bool EditMode
    {
        get { return !btnUpdatePriceRange.Enabled; }
        set
        {
            dbNewPrice.Enabled = !value;
            btnUpdatePriceRange.Enabled = !value;
            pnlGraph.Enabled = !value;
        }
    }

    #endregion

    #region Chart

    private void priceHistoryGraphDataBind()
    {
        try
        {
            DateTime startDate = cldDateFrom.SelectedDate;
            DateTime endDate = cldDateTo.SelectedDate;

            DataSet priceHistory = InstrumentPriceUpdateAdapter.GetInstrumentPriceHistory(
                Utility.GetKeyFromDropDownList(ddlSelectedInstrument),
                ref startDate,
                ref endDate);

            chHistoryGraph.Series.Clear();

            DateTime[] dates = Util.GetDatesArray(startDate, endDate);

            if (dates.Length > 0)
            {
                Series series = createSeries(getInstrumentType() + "History", false);
                chHistoryGraph.Series.Add(series);
                priceHistory.Tables[0].DefaultView.RowFilter = "PriceQuantity > 0";
                DataView dataSource = priceHistory.Tables[0].DefaultView;
                if (dataSource.Count > 0)
                {
                    if (dataSource.Count == 1)
                        setChartTypeToPoint(series);
                    string currency = dataSource.Table.Rows[0].ItemArray[dataSource.Table.Columns["Currency"].Ordinal].ToString();
                    chHistoryGraph.ChartAreas[0].AxisY.LabelStyle.Format = string.Format("{0}0.00", currency);
                }
                series.Points.DataBindXY(dataSource, "Date", dataSource, "PriceQuantity");

                priceHistory.Tables[0].DefaultView.RowFilter = "IsNull(PriceQuantity, 0) = 0";
                dataSource = priceHistory.Tables[0].DefaultView;
                bool startFromZero = false;
                if (dataSource.Count > 0)
                {
                    series = createSeries("Missing Values", true);
                    chHistoryGraph.Series.Add(series);
                    if (dataSource.Count == 1)
                        setChartTypeToPoint(series);
                    series.Points.DataBindXY(dataSource, "Date", dataSource, "PriceQuantity");
                    startFromZero = true;
                }
                chHistoryGraph.ChartAreas[0].AxisY.StartFromZero = startFromZero;
            }

            chHistoryGraph.ChartAreas[0].AxisX.LabelStyle.Format = getAxisXDateFormat(startDate, endDate);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private Series createSeries(string seriesName, bool emptyValues)
    {
        Series series = new Series(seriesName);
        if (emptyValues)
        {
            series.Type = SeriesChartType.Point;
            series.Color = Color.Red;
            series.MarkerSize = 6;
        }
        else
            series.Type = SeriesChartType.Line;
        series.LegendText = seriesName;
        series.XValueType = ChartValueTypes.DateTime;
        series.EmptyPointStyle.Color = Color.Transparent;
        series.BorderWidth = 2;
        return series;
    }

    private void setChartTypeToPoint(Series series)
    {
        series.Type = SeriesChartType.Point;
        series.MarkerSize = 8;
    }

    private string getAxisXDateFormat(DateTime minStartDate, DateTime maxEndDate)
    {
        if ((maxEndDate - minStartDate).TotalDays > 180)
            return "MMM\nyyyy";
        else
            return "d MMM\nyyyy";
    }

    #endregion
}

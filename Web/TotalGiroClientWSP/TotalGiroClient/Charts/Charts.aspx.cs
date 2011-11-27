using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Charts;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using Dundas.Charting.WebControl;

public partial class Charts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Grafieken";
            ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx";

            pnlPortfolioNavigationBar.Visible = CommonAdapter.GetCurrentLoginType() != LoginTypes.Customer;
        }

        elbErrorMessage.Text = "";
    }

    protected int ContactId
    {
        get
        {
            // TODO: if customer login, just return 0
            object i = Session["ContactId"];
            return (i == null ? 0 : (int)i);
        }
    }

    protected void tbcChartType_ActiveTabChanged(object sender, EventArgs e)
    {
        try
        {
            Panel[] panels = new Panel[] { pnlPortfolio, pnlPositions, pnlAllocation };
            for (int i = 0; i < 3; i++)
                panels[i].Visible = (tbcChartType.ActiveTabIndex == i);

            switch (tbcChartType.ActiveTabIndex)
            {
                case 0:
                    chPortfolioValuationsDataBind();
                    ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx";
                    break;

                case 1:
                    if (rblPositionsAccounts.Items.Count == 0)
                        rblPositionsAccounts.DataBind();
                    else
                        chPositionsValuationsDataBind();
                    ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx#positions";
                    break;

                case 2:
                    ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx#allocation";
                    break;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    
    #region Portfolio Chart

    protected void cblPortfolioAccounts_DataBound(object sender, EventArgs e)
    {
        foreach (ListItem li in cblPortfolioAccounts.Items)
            li.Selected = true;
        chPortfolioValuationsDataBind();
    }

    protected void cblPortfolioAccounts_SelectedIndexChanged(object sender, EventArgs e)
    {
        chPortfolioValuationsDataBind();
    }

    private void chPortfolioValuationsDataBind()
    {
        try
        {
            int[] selAccountIds = getSelectedAccountIds();

            var accountSeriesInfos = ChartsAdapter.GetAccountSeriesInfoList(ContactId);

            var selAccountSeriesInfos = from si in accountSeriesInfos
                                        where selAccountIds.Contains(si.Key)
                                        select si;

            DateTime minStartDate = getMinStartDate(selAccountSeriesInfos),
                     maxEndDate = getMaxEndDate(selAccountSeriesInfos);
            double dateIncrement = ChartsAdapter.GetDateIncrement(minStartDate, maxEndDate);

            foreach (SeriesInfo accountSeriesInfo in accountSeriesInfos)
            {
                Series series = createSeries(accountSeriesInfo.SeriesName);
                chPortfolioValuations.Series.Add(series);

                if (selAccountIds.Contains(accountSeriesInfo.Key))
                {
                    DateTime[] dates = ChartsAdapter.GenerateDates(accountSeriesInfo.StartDate, accountSeriesInfo.EndDate, dateIncrement);

                    if (dates.Length > 0)
                    {
                        DataView dataSource = ChartsAdapter.GetValuationsTotalPortfolio(accountSeriesInfo.Key, dates).DefaultView;
                        if (dataSource.Count == 1)
                            setChartTypeToPoint(series);

                        series.Points.DataBindXY(dataSource, "Date", dataSource, "TotalValueQuantity");
                    }
                }
            }

            chPortfolioValuations.ChartAreas[0].AxisX.LabelStyle.Format = getAxisXDateFormat(minStartDate, maxEndDate);
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private Series createSeries(string seriesName)
    {
        Series series = new Series(seriesName);
        series.Type = SeriesChartType.Spline;
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

    private int[] getSelectedAccountIds()
    {
        return (from li in cblPortfolioAccounts.Items.Cast<ListItem>()
                where li.Selected
                select int.Parse(li.Value)).ToArray();
    }

    #endregion


    #region Positions Chart

    protected void rblPositionsAccounts_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (rblPositionsAccounts.Items.Count > 0)
            {
                rblPositionsAccounts.Items[0].Selected = true;
                cblPositionInstruments.DataBind();
            }
            else
                cblPositionInstruments.Visible = false;
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void cblPositionInstruments_DataBound(object sender, EventArgs e)
    {
        try
        {
            int accountId = int.Parse(rblPositionsAccounts.SelectedItem.Value);
            int[] highestValueInstrumentIds = ChartsAdapter.GetHighestValueInstrumentIds(accountId, 2);

            foreach (ListItem li in cblPositionInstruments.Items)
                li.Selected = highestValueInstrumentIds.Contains(int.Parse(li.Value));

            chPositionsValuationsDataBind();
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void cblPositionInstruments_SelectedIndexChanged(object sender, EventArgs e)
    {
        chPositionsValuationsDataBind();
    }

    private void chPositionsValuationsDataBind()
    {
        try
        {
            if (rblPositionsAccounts.Items.Count > 0 && chPositionsValuations.Series.Count == 0)
            {
                int accountId = int.Parse(rblPositionsAccounts.SelectedItem.Value);
                int[] selInstrumentIds = getSelectedInstrumentIds();

                // this does NOT set StartDate and EndDate
                var positionSeriesInfos = ChartsAdapter.GetPositionSeriesInfoList(accountId);

                var selPositionSeriesInfos = (from si in positionSeriesInfos
                                              where selInstrumentIds.Contains(si.Key)
                                              select si).ToList();

                ChartsAdapter.RetrieveStartEndDates(selPositionSeriesInfos, accountId);

                DateTime minStartDate = getMinStartDate(selPositionSeriesInfos),
                         maxEndDate = getMaxEndDate(selPositionSeriesInfos);
                double dateIncrement = ChartsAdapter.GetDateIncrement(minStartDate, maxEndDate);

                foreach (SeriesInfo positionSeriesInfo in positionSeriesInfos)
                {
                    Series series = createSeries(positionSeriesInfo.SeriesName);
                    chPositionsValuations.Series.Add(series);

                    if (selInstrumentIds.Contains(positionSeriesInfo.Key))
                    {
                        DateTime[] dates = ChartsAdapter.GenerateDates(positionSeriesInfo.StartDate, positionSeriesInfo.EndDate, dateIncrement);

                        DataView dataSource = ChartsAdapter.GetPositionValuations(accountId, positionSeriesInfo.Key, dates).DefaultView;
                        if (dataSource.Count == 1)
                            setChartTypeToPoint(series);

                        series.Points.DataBindXY(dataSource, "Date", dataSource, "BaseMarketValueQuantity");
                    }
                }

                chPositionsValuations.ChartAreas[0].AxisX.LabelStyle.Format = getAxisXDateFormat(minStartDate, maxEndDate);

                // Resize Legend to fit all instruments on two columns
                chPositionsValuations.Legends[0].Position.Height = 3 + 3 * ((cblPositionInstruments.Items.Count - 1) / 2 + 1);
                chPositionsValuations.ChartAreas[0].Position.Height = 95 - chPositionsValuations.Legends[0].Position.Height;
                chPositionsValuations.ChartAreas[0].Position.Y = 100 - chPositionsValuations.ChartAreas[0].Position.Height;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private int[] getSelectedInstrumentIds()
    {
        return (from li in cblPositionInstruments.Items.Cast<ListItem>()
                where li.Selected
                select int.Parse(li.Value)).ToArray();
    }

    private string getAxisXDateFormat(DateTime minStartDate, DateTime maxEndDate)
    {
        if ((maxEndDate - minStartDate).TotalDays > 180)
            return "MMM\nyyyy";
        else
            return "d MMM\nyyyy";
    }

    private DateTime getMinStartDate(IEnumerable<SeriesInfo> seriesList)
    {
        seriesList = from s in seriesList where Util.IsNotNullDate(s.StartDate) select s;
        return seriesList.Count() > 0 ? (from s in seriesList select s.StartDate).Min() : DateTime.Today;
    }

    private DateTime getMaxEndDate(IEnumerable<SeriesInfo> seriesList)
    {
        seriesList = from s in seriesList where Util.IsNotNullDate(s.EndDate) select s;
        return seriesList.Count() > 0 ? (from s in seriesList select s.EndDate).Max() : DateTime.Today;
    }
    
    #endregion


    #region Allocation by Instrument Chart

    protected void rblAllocationAccounts_DataBound(object sender, EventArgs e)
    {
        if (rblAllocationAccounts.Items.Count > 0)
            rblAllocationAccounts.Items[0].Selected = true;
    }

    protected void odsAllocation_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (rblAllocationAccounts.Items.Count == 0)
            e.Cancel = true;
    }

    #endregion
}

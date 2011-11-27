using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Client.Web.Util;
using B4F.TotalGiro.ClientApplicationLayer.Charts;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.Stichting.Login;
using Dundas.Charting.WebControl;

namespace B4F.TotalGiro.Client.Web.Charts
{
    public partial class Charts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Grafieken";
                ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx";

                pnlPortfolioNavigationBar.Visible = CommonAdapter.GetCurrentLoginType() != LoginTypes.Customer;

                initializeTabs();
            }

            elbErrorMessage.Text = "";
        }

        protected int ContactId
        {
            get
            {
                object i = Session["ContactId"];
                return i == null ? 0 : (int)i;
            }
        }

        protected int AccountId
        {
            get
            {
                object i = Session["AccountId"];
                return i == null ? 0 : (int)i;
            }
            set { Session["AccountId"] = value; }
        }

        protected int ActiveTabIndex
        {
            get
            {
                object i = Session["ActiveTabIndex"];
                return i == null ? 0 : (int)i;
            }
            set { Session["ActiveTabIndex"] = value; }
        }

        private void initializeTabs()
        {
            try
            {
                cblAccounts.DataBind();
                rblPositionAccounts.DataBind();
                rblAllocationAccounts.DataBind();

                if (AccountId != 0)
                {
                    rblPositionAccounts.SelectedValue = AccountId.ToString();
                    rblAllocationAccounts.SelectedValue = AccountId.ToString();
                }
                else if (rblPositionAccounts.Items.Count > 0)
                {
                    rblPositionAccounts.SelectedIndex = 0;
                    rblAllocationAccounts.SelectedIndex = 0;
                }
                else
                    cblPositionInstruments.Visible = false;

                tbcChartType.ActiveTabIndex = ActiveTabIndex;
                tbcChartType_ActiveTabChanged(tbcChartType, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }

        protected void tbcChartType_ActiveTabChanged(object sender, EventArgs e)
        {
            try
            {
                Panel[] panels = new Panel[] { pnlAccountValuations, pnlPositionValuations, pnlAllocation };
                for (int i = 0; i < 3; i++)
                    panels[i].Visible = (tbcChartType.ActiveTabIndex == i);

                switch (tbcChartType.ActiveTabIndex)
                {
                    case 0:
                        chAccountValuationsDataBind();
                        ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx";
                        break;

                    case 1:
                        chPositionValuationsDataBind();
                        ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx#positions";
                        break;

                    case 2:
                        ((TotalGiroClient)Master).HelpUrl = "~/Help/ChartsHelp.aspx#allocation";
                        break;
                }
                
                ActiveTabIndex = tbcChartType.ActiveTabIndex;
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }


        #region Account Valuations Chart

        protected void cblAccounts_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem li in cblAccounts.Items)
                li.Selected = true;
        }

        protected void cblAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            chAccountValuationsDataBind();
        }

        private void chAccountValuationsDataBind()
        {
            try
            {
                int[] selAccountIds = getSelectedAccountIds();

                var accountSeriesInfos = ChartsAdapter.GetAccountSeriesInfoList(ContactId);

                var selAccountSeriesInfos = accountSeriesInfos.Where(s => selAccountIds.Contains(s.Key));

                DateTime minStartDate = getMinStartDate(selAccountSeriesInfos),
                         maxEndDate = getMaxEndDate(selAccountSeriesInfos);
                double dateIncrement = ChartsAdapter.GetDateIncrement(minStartDate, maxEndDate);

                chAccountValuations.Series.Clear();

                foreach (SeriesInfo accountSeriesInfo in accountSeriesInfos)
                {
                    Series series = createSeries(accountSeriesInfo.SeriesName);
                    chAccountValuations.Series.Add(series);

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

                chAccountValuations.ChartAreas[0].AxisX.LabelStyle.Format = getAxisXDateFormat(minStartDate, maxEndDate);
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
            return cblAccounts.Items.Cast<ListItem>()
                                    .Where(i => i.Selected)
                                    .Select(i => int.Parse(i.Value))
                                    .ToArray();
        }

        #endregion


        #region Position Valuations Chart

        protected void rblPositionAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblAllocationAccounts.SelectedIndex = rblPositionAccounts.SelectedIndex;

            AccountId = int.Parse(rblPositionAccounts.SelectedValue);
        }

        protected void cblPositionInstruments_DataBound(object sender, EventArgs e)
        {
            try
            {
                int accountId = int.Parse(rblPositionAccounts.SelectedValue);
                int[] highestValueInstrumentIds = ChartsAdapter.GetHighestValueInstrumentIds(accountId, 2);

                foreach (ListItem li in cblPositionInstruments.Items)
                    li.Selected = highestValueInstrumentIds.Contains(int.Parse(li.Value));

                chPositionValuationsDataBind();
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }

        protected void cblPositionInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            chPositionValuationsDataBind();
        }

        private void chPositionValuationsDataBind()
        {
            try
            {
                chPositionValuations.Series.Clear();

                if (rblPositionAccounts.Items.Count > 0 && cblPositionInstruments.Items.Count > 0)
                {
                    int accountId = int.Parse(rblPositionAccounts.SelectedValue);
                    int[] selInstrumentIds = getSelectedInstrumentIds();

                    // this does NOT set StartDate and EndDate
                    var positionSeriesInfos = ChartsAdapter.GetPositionSeriesInfoList(accountId);

                    var selPositionSeriesInfos = positionSeriesInfos.Where(s => selInstrumentIds.Contains(s.Key))
                                                                    .ToList();

                    ChartsAdapter.RetrieveStartEndDates(selPositionSeriesInfos, accountId);

                    DateTime minStartDate = getMinStartDate(selPositionSeriesInfos),
                             maxEndDate = getMaxEndDate(selPositionSeriesInfos);
                    double dateIncrement = ChartsAdapter.GetDateIncrement(minStartDate, maxEndDate);

                    foreach (SeriesInfo positionSeriesInfo in positionSeriesInfos)
                    {
                        Series series = createSeries(positionSeriesInfo.SeriesName);
                        chPositionValuations.Series.Add(series);

                        if (selInstrumentIds.Contains(positionSeriesInfo.Key))
                        {
                            DateTime[] dates = ChartsAdapter.GenerateDates(positionSeriesInfo.StartDate, positionSeriesInfo.EndDate, dateIncrement);

                            DataView dataSource = ChartsAdapter.GetPositionValuations(accountId, positionSeriesInfo.Key, dates).DefaultView;
                            if (dataSource.Count == 1)
                                setChartTypeToPoint(series);

                            series.Points.DataBindXY(dataSource, "Date", dataSource, "BaseMarketValueQuantity");
                        }
                    }

                    chPositionValuations.ChartAreas[0].AxisX.LabelStyle.Format = getAxisXDateFormat(minStartDate, maxEndDate);

                    // Resize Legend to fit all instruments on two columns
                    chPositionValuations.Legends[0].Position.Height = 3 + 3 * ((cblPositionInstruments.Items.Count - 1) / 2 + 1);
                    chPositionValuations.ChartAreas[0].Position.Height = 95 - chPositionValuations.Legends[0].Position.Height;
                    chPositionValuations.ChartAreas[0].Position.Y = 100 - chPositionValuations.ChartAreas[0].Position.Height;
                }
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }

        private int[] getSelectedInstrumentIds()
        {
            return cblPositionInstruments.Items.Cast<ListItem>()
                                               .Where(i => i.Selected)
                                               .Select(i => int.Parse(i.Value))
                                               .ToArray();
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
            seriesList = seriesList.Where(s => B4F.TotalGiro.Utils.Util.IsNotNullDate(s.StartDate));

            return seriesList.Count() > 0 ? seriesList.Select(s => s.StartDate).Min() : DateTime.Today;
        }

        private DateTime getMaxEndDate(IEnumerable<SeriesInfo> seriesList)
        {
            seriesList = seriesList.Where(s => B4F.TotalGiro.Utils.Util.IsNotNullDate(s.EndDate));

            return seriesList.Count() > 0 ? seriesList.Select(s => s.EndDate).Max() : DateTime.Today;
        }

        #endregion


        #region Allocation Chart

        protected void rblAllocationAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblPositionAccounts.SelectedIndex = rblAllocationAccounts.SelectedIndex;

            AccountId = int.Parse(rblAllocationAccounts.SelectedValue);
        }

        protected void odsAllocation_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (rblAllocationAccounts.Items.Count == 0)
                e.Cancel = true;
        }

        #endregion
    } 
}

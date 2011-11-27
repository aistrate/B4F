using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.Planning;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils.Tuple;
using B4F.Web.WebControls;
using Dundas.Charting.WebControl;

public partial class FinancialPlanner : System.Web.UI.Page
{
    #region Panel GivenFields

    #region Panel GivenFields - initialization

    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErrorFutureValue.Visible = false;
        elbGivenFields.Text = "";
        elbFutureValue.Text = "";

        if (ContactId != 0)
        {
            int contactId = ContactId;
            ContactId = 0;
            ContactId = contactId;
        }

        try
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Monitor Portefeuille";

                ctlPortfolioNavigationBar.Visible = Initialize && CurrentLoginType != LoginTypes.Customer;

                initializeGivenFields();
            }

            grayOutFirstLine(ddlMonth);
            grayOutFirstLine(ddlYear);
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            odsAccount.SelectParameters["hasEmptyFirstRow"].DefaultValue = AccountList_HasEmptyLine.ToString();
            odsModelPortfolio.SelectParameters["hasEmptyFirstRow"].DefaultValue = ModelList_HasEmptyLine.ToString();

            Utility.AddParameterContactId(odsAccount, CurrentLoginType == LoginTypes.Customer);
        }
    }

    private void initializeGivenFields()
    {
        ddlModelPortfolio.DataBind();

        if (Initialize || CurrentLoginType == LoginTypes.Customer)
        {
            pnlAccountList.Visible = true;
            ddlAccount.DataBind();
        }
        else
            displayModelDependants();

        ddlMonth.DataBind();
        ddlYear.DataBind();
        rfvYear.InitialValue = DateTime.Today.Year.ToString();

        FocusCandidates.FocusOnFirstVisible();
    }

    #endregion


    #region Panel GivenFields - behavior toggles

    protected bool AccountList_HasEmptyLine { get { return CurrentLoginType != LoginTypes.Customer; } }
    protected bool ModelList_HasEmptyLine { get { return CurrentLoginType != LoginTypes.Customer; } }

    protected bool AccountList_EmptyLineClearsAll { get { return false; } }
    protected bool ModelList_EmptyLineClearsAll { get { return false; } }

    protected bool AccountList_EmptyLineClearsModelList { get { return false; } }

    protected bool AllowMissingExpectedReturn { get { return CurrentLoginType != LoginTypes.Customer; } }

    #endregion


    #region Panel GivenFields - general properties

    protected bool Initialize
    {
        get { return Utility.GetQueryParameters().GetBoolValue("initialize", false); }
    }

    protected int ContactId
    {
        get
        {
            object i = Session["ContactId"];
            return i == null ? 0 : (int)i;
        }
        set { Session["ContactId"] = value; }
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

    protected LoginTypes CurrentLoginType
    {
        get
        {
            if (currentLoginType == null)
                currentLoginType = CommonAdapter.GetCurrentLoginType();
            return (LoginTypes)currentLoginType;
        }
    }
    private LoginTypes? currentLoginType = null;

    #endregion


    #region Panel GivenFields - event handlers

    protected void lnkClearAll_Command(object sender, CommandEventArgs e)
    {
        try
        {
            resetAccountDependants();
            resetPanelFutureValue(false);
            FocusCandidates.FocusOnFirstVisible();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void ddlAccount_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (AccountId != 0 && ddlAccount.Items.FindByValue(AccountId.ToString()) != null)
                ddlAccount.SelectedValue = AccountId.ToString();
            else
                selectFirstNonEmptyLine(ddlAccount, AccountList_HasEmptyLine);

            selectedAccountChanged();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            resetPanelFutureValue(false);

            selectedAccountChanged();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private void selectedAccountChanged()
    {
        try
        {
            if (!IsAccountKnown && AccountList_EmptyLineClearsAll)
                resetAccountDependants();
            else
            {
                if (AccountList_EmptyLineClearsModelList)
                    selectFirstLine(ddlModelPortfolio);

                displayAccountDependants();
            }

            FocusCandidates.FocusOnFirstVisible();
        }
        catch (ArgumentException ex)
        {
            pnlGivenFields.Visible = false;
            displayMessage("Bij deze portefeuille is de monitorfunctie nog niet mogelijk.");
            if (CurrentLoginType != LoginTypes.Customer)
                displayErrorMessage(ex);
        }
    }

    protected void ddlModelPortfolio_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            resetPanelFutureValue(false);

            if (ddlModelPortfolio.SelectedValue == int.MinValue.ToString() && ModelList_EmptyLineClearsAll)
                resetModelDependants();
            else
                displayModelDependants();
            
            FocusCandidates.FocusOnFirstVisible();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private void displayAccountDependants()
    {
        pnlGivenFields.Visible = true;

        if (IsAccountKnown)
            AccountId = SelectedAccountId;

        displayModel();
        displayModelDependants();

        displayPresentValue();

        pnlTargetUpdate.Visible = IsTargetUpdateAllowed;
        TargetUpdateMode = false;
        displayTargetDependants();
    }

    private void displayModelDependants()
    {
        displayExpectedReturn();
        displayStandardDeviation();
    }

    private void displayTargetDependants()
    {
        displayTargetValue();
        displayDepositPerYear();
        displayTargetEndDate();
    }

    private void resetAccountDependants()
    {
        if (pnlAccountList.Visible && AccountList_HasEmptyLine)
            ddlAccount.SelectedValue = int.MinValue.ToString();

        if (SelectedAccountId == 0 && (!pnlAccountList.Visible || AccountList_HasEmptyLine))
        {
            pnlGivenFields.Visible = true;

            mvwModelPortfolio.ActiveViewIndex = 0;
            mvwPresentValue.ActiveViewIndex = 0;
            mvwTargetValue.ActiveViewIndex = 0;
            mvwTargetEndDate.ActiveViewIndex = 0;
            mvwDepositPerYear.ActiveViewIndex = 0;

            pnlTargetUpdate.Visible = false;

            txtPresentValue.Text = "";
            txtTargetValue.Text = "";
            selectFirstLine(ddlMonth);
            selectFirstLine(ddlYear);
            txtDepositPerYear.Text = "";

            resetModelDependants();
        }
    }

    private void resetModelDependants()
    {
        selectFirstLine(ddlModelPortfolio);

        mvwExpectedReturn.ActiveViewIndex = 0;
        mvwStandardDeviation.ActiveViewIndex = 0;

        txtExpectedReturn.Text = "";
        txtStandardDeviation.Text = "";

        displayModelDependants();
    }

    private void selectFirstNonEmptyLine(DropDownList dropDownList, bool hasEmptyLine)
    {
        int emptyCount = hasEmptyLine ? 1 : 0;

        if (dropDownList.Items.Count > emptyCount)
            dropDownList.SelectedIndex = emptyCount;
    }

    private void selectFirstLine(DropDownList dropDownList)
    {
        if (dropDownList.Items.Count > 0)
            dropDownList.SelectedIndex = 0;
    }

    private void grayOutFirstLine(DropDownList dropDownList)
    {
        if (dropDownList.Items.Count > 0)
            dropDownList.Items[0].Attributes.CssStyle.Add(HtmlTextWriterStyle.Color, "DarkGray");
    }

    protected bool IsTargetUpdateAllowed
    {
        get { return IsAccountKnown && CurrentLoginType != LoginTypes.Customer; }
    }

    protected bool TargetUpdateMode
    {
        get { return mvwTargetUpdate.ActiveViewIndex == 1; }
        set
        {
            if (!IsTargetUpdateAllowed)
                pnlTargetUpdate.Visible = false;

            bool updateMode = IsTargetUpdateAllowed && value;

            mvwTargetUpdate.ActiveViewIndex = updateMode ? 1 : 0;

            resetPanelFutureValue(false);

            if (updateMode)
                txtTargetValue.Focus();
            else
                FocusCandidates.FocusOnFirstEnabled();
        }
    }

    protected void lnkUpdateTarget_Command(object sender, EventArgs e)
    {
        try
        {
            TargetUpdateMode = true;
            displayTargetDependants();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void lnkSaveTarget_Command(object sender, EventArgs e)
    {
        try
        {
            try
            {
                bool saved = FinancialPlannerAdapter.SaveAccountFinancialTarget(SelectedAccountId, DepositPerYear, TargetEndDate, TargetValue);

                displayMessage(saved ? "Nieuw doelvermogen opgeslagen." :
                                       "Doelvermogen niet aangepast.");
            }
            catch (Exception ex) { displayErrorMessage(ex); }

            TargetUpdateMode = false;
            displayTargetDependants();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void lnkCancelUpdateTarget_Command(object sender, EventArgs e)
    {
        try
        {
            TargetUpdateMode = false;
            displayTargetDependants();
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected Control[] FocusCandidates
    {
        get { return new Control[] { txtPresentValue, txtExpectedReturn, txtTargetValue }; }
    }

    #endregion


    #region Panel GivenFields - data properties

    protected int SelectedAccountId
    {
        get
        {
            return pnlAccountList.Visible && ddlAccount.SelectedValue != int.MinValue.ToString()
                      ? int.Parse(ddlAccount.SelectedValue)
                      : 0;
        }
    }

    protected int SelectedModelId
    {
        get
        {
            return ddlModelPortfolio.Visible && ddlModelPortfolio.SelectedValue != int.MinValue.ToString()
                      ? int.Parse(ddlModelPortfolio.SelectedValue)
                      : 0;
        }
    }

    protected FinancialDataView FinancialData
    {
        get
        {
            if (financialData == null)
            {
                if (SelectedAccountId != 0)
                    financialData = FinancialPlannerAdapter.GetFinancialDataFromAccount(SelectedAccountId, AllowMissingExpectedReturn);
                else if (SelectedModelId != 0)
                    financialData = FinancialPlannerAdapter.GetFinancialDataFromModel(SelectedModelId, AllowMissingExpectedReturn);
            }

            return financialData;
        }
    }
    private FinancialDataView financialData;

    protected bool IsAccountKnown { get { return SelectedAccountId != 0 && FinancialData != null; } }
    protected bool IsModelDataKnown { get { return FinancialData != null && FinancialData.ExpectedReturn != 0m; } }
    protected bool IsTargetKnown { get { return IsAccountKnown && FinancialData.TargetValue != 0m; } }

    private void displayModel()
    {
        if (IsAccountKnown)
        {
            if (ddlModelPortfolio.Items.FindByValue(FinancialData.ModelId.ToString()) != null)
                ddlModelPortfolio.SelectedValue = FinancialData.ModelId.ToString();
            else
                selectFirstLine(ddlModelPortfolio);

            lblModelName.Text = FinancialData.ModelName;
        }

        mvwModelPortfolio.ActiveViewIndex = IsAccountKnown ? 1 : 0;
    }

    protected decimal ExpectedReturn
    {
        get
        {
            return mvwExpectedReturn.ActiveViewIndex == 0 ? readDecimal(txtExpectedReturn, 3) / 100m :
                                                            FinancialData.ExpectedReturn;
        }
    }

    private void displayExpectedReturn()
    {
        if (IsModelDataKnown)
        {
            decimal expectedReturn = FinancialData.ExpectedReturn * 100m;

            txtExpectedReturn.Text = expectedReturn.ToString(textBoxDecimalFormat);
            lblExpectedReturn.Text = string.Format("{0:0.0##} %", expectedReturn);
        }
        else if (IsAccountKnown)
        {
            txtExpectedReturn.Text = "";
            lblExpectedReturn.Text = "";
        }

        mvwExpectedReturn.ActiveViewIndex = IsModelDataKnown ? 1 : 0;
    }

    protected decimal StandardDeviation
    {
        get
        {
            return mvwStandardDeviation.ActiveViewIndex == 0 ? readDecimal(txtStandardDeviation, 3) / 100m :
                                                               FinancialData.StandardDeviation;
        }
    }

    private void displayStandardDeviation()
    {
        if (IsModelDataKnown)
        {
            decimal standardDeviation = FinancialData.StandardDeviation * 100m;

            txtStandardDeviation.Text = standardDeviation.ToString(textBoxDecimalFormat);
            lblStandardDeviation.Text = string.Format("{0:0.0##} %", standardDeviation);
        }
        else if (IsAccountKnown)
        {
            txtStandardDeviation.Text = "";
            lblStandardDeviation.Text = "";
        }

        mvwStandardDeviation.ActiveViewIndex = IsModelDataKnown ? 1 : 0;
    }

    protected decimal PresentValue
    {
        get { return mvwPresentValue.ActiveViewIndex == 0 ? readDecimal(txtPresentValue) : FinancialData.PresentValue; }
    }

    private void displayPresentValue()
    {
        if (IsAccountKnown)
        {
            txtPresentValue.Text = FinancialData.PresentValue.ToString(textBoxDecimalFormat);
            lblPresentValue.Text = FinancialData.PresentValueDisplayString;
        }

        mvwPresentValue.ActiveViewIndex = IsAccountKnown ? 1 : 0;
    }

    protected decimal TargetValue
    {
        get { return mvwTargetValue.ActiveViewIndex == 0 ? readDecimal(txtTargetValue) : FinancialData.TargetValue; }
    }

    private void displayTargetValue()
    {
        if (IsTargetKnown)
        {
            txtTargetValue.Text = FinancialData.TargetValue.ToString(textBoxDecimalFormat);
            lblTargetValue.Text = FinancialData.TargetValueDisplayString;
        }
        else if (IsAccountKnown && !TargetUpdateMode)
        {
            txtTargetValue.Text = "";
            lblTargetValue.Text = "";
        }

        mvwTargetValue.ActiveViewIndex = IsTargetKnown && !TargetUpdateMode ? 1 : 0;
    }

    protected decimal DepositPerYear
    {
        get { return mvwDepositPerYear.ActiveViewIndex == 0 ? readDecimal(txtDepositPerYear) : FinancialData.DepositPerYear; }
    }

    private void displayDepositPerYear()
    {
        if (IsTargetKnown)
        {
            txtDepositPerYear.Text = FinancialData.DepositPerYear.ToString(textBoxDecimalFormat);
            lblDepositPerYear.Text = FinancialData.DepositPerYearDisplayString + " / jaar";
        }
        else if (IsAccountKnown && !TargetUpdateMode)
        {
            txtDepositPerYear.Text = "";
            lblDepositPerYear.Text = "";
        }

        mvwDepositPerYear.ActiveViewIndex = IsTargetKnown && !TargetUpdateMode ? 1 : 0;
    }

    protected DateTime TargetEndDate
    {
        get
        {
            return mvwTargetEndDate.ActiveViewIndex == 0 ? new DateTime(readInt(ddlYear), readInt(ddlMonth), 1) :
                                                           FinancialData.TargetEndDate;
        }
    }

    private void displayTargetEndDate()
    {
        if (IsTargetKnown)
        {
            ddlMonth.SelectedValue = FinancialData.TargetEndDate.Month.ToString();
            ddlYear.SelectedValue = FinancialData.TargetEndDate.Year.ToString();
            lblTargetEndDate.Text = FinancialData.TargetEndDate.ToString("MMMM yyyy");
        }
        else if (IsAccountKnown && !TargetUpdateMode)
        {
            selectFirstLine(ddlMonth);
            selectFirstLine(ddlYear);
            lblTargetEndDate.Text = "";
        }

        mvwTargetEndDate.ActiveViewIndex = IsTargetKnown && !TargetUpdateMode ? 1 : 0;
    }

    private decimal readDecimal(TextBox textBox)
    {
        return readDecimal(textBox, 2);
    }

    private decimal readDecimal(TextBox textBox, int decimals)
    {
        decimal d = 0m;
        decimal.TryParse(textBox.Text, out d);
        d = Math.Round(Math.Abs(d), decimals);

        textBox.Text = d.ToString(textBoxDecimalFormat);
        return d;
    }

    private int readInt(TextBox textBox)
    {
        return (int)readDecimal(textBox, 0);
    }

    private int readInt(DropDownList dropDownList)
    {
        return int.Parse(dropDownList.SelectedValue);
    }

    private const string textBoxDecimalFormat = "###0.###";
    private const string textBoxIntegerFormat = "###0";

    #endregion

    #endregion


    #region Panel FutureValue

    #region Panel FutureValue - event handlers

    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        try
        {
            Persisted = new FinancialDataView(PresentValue, ExpectedReturn, StandardDeviation,
                                              TargetValue, TargetEndDate, DepositPerYear);

            if (Persisted.YearsLeft <= 0)
            {
                resetPanelFutureValue(false);
                displayMessage("Calculation period is too short (minimum is 1 year).");
                return;
            }

            resetPanelFutureValue(true);

            displayFutureValueBeforeAdjust();
            displayChanceOfMeetingTarget();
            displayExtras();

            chPortfolioFutureValueDataBind(true);
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void txtExtra_TextChanged(object sender, EventArgs e)
    {
        try
        {
            displayChanceOfMeetingTarget();

            if (ExtrasSelectedIndex == 1)
                displayFutureValueAfterAdjust();

            chPortfolioFutureValueDataBind(false);
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    protected void lnkChoiceOfExtras_Command(object sender, CommandEventArgs e)
    {
        try
        {
            ExtrasSelectedIndex = 1 - ExtrasSelectedIndex;

            displayChanceOfMeetingTarget();
            displayExtras();

            chPortfolioFutureValueDataBind(false);
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private void resetPanelFutureValue(bool visible)
    {
        pnlFutureValue.Visible = visible;

        btnCalculate.Text = visible ? "Herbereken" : "Bereken";

        lblFutureValueBeforeAdjust.Text = "";
        lblEndOfPeriod.Text = "";

        lblChanceOfMeetingTarget.Text = "";
        pnlTrafficLight.Visible = false;
        TrafficLightValue = 0m;

        lblProposedPeriodical.Text = "";
        lblProposedInitial.Text = "";
        lblExtraPeriodicalMax.Text = "";
        lblExtraInitialMax.Text = "";
        sldExtraPeriodical.Maximum = 0;
        sldExtraInitial.Maximum = 0;
        IsSliderReset = true;

        ExtrasSelectedIndex = 0;
        ExtraPeriodical = 0m;
        ExtraInitial = 0m;

        lblFutureValueAfterAdjust.Text = "";
        lblTargetValueExtras.Text = "";

        chPortfolioFutureValue.Series.Clear();
    }

    private void displayFutureValueBeforeAdjust()
    {
        try
        {
            lblFutureValueBeforeAdjust.Text = formatCurrency(FinancialPlannerAdapter.GetFutureValue(
                                                                    Persisted.PresentValue, Persisted.DepositPerYear, Persisted.YearsLeft,
                                                                    Persisted.ExpectedReturn));
            lblEndOfPeriod.Text = DateTime.Today.AddYears(Persisted.YearsLeft).ToString("MMMM yyyy");
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private void displayChanceOfMeetingTarget()
    {
        try
        {
            decimal chanceOfMeetingTarget = Math.Round(FinancialPlannerAdapter.GetChanceOfMeetingTarget(
                                                            TotalPresentValue, TotalDepositPerYear, Persisted.YearsLeft,
                                                            Persisted.ExpectedReturn, Persisted.StandardDeviation, Persisted.TargetValue),
                                                       2);

            lblChanceOfMeetingTarget.Text = chanceOfMeetingTarget.ToString("p0");

            pnlTrafficLight.Visible = true;
            TrafficLightValue = chanceOfMeetingTarget;
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private void displayExtras()
    {
        try
        {
            switch (ExtrasSelectedIndex)
            {
                case 0:
                    lblProposedPeriodical.Text = formatCurrency(ProposedPeriodical);
                    lblProposedInitial.Text = formatCurrency(ProposedInitial);
                    break;

                case 1:
                    decimal proposedPeriodical = Math.Ceiling(Math.Round(ProposedPeriodical, 2));
                    decimal proposedInitial = Math.Ceiling(Math.Round(ProposedInitial, 2));

                    lblExtraPeriodicalMax.Text = proposedPeriodical.ToString("#,##0");
                    lblExtraInitialMax.Text = proposedInitial.ToString("#,##0");
                    sldExtraPeriodical.Maximum = (double)proposedPeriodical;
                    sldExtraInitial.Maximum = (double)proposedInitial;

                    if (IsSliderReset)
                    {
                        //ExtraPeriodical = proposedPeriodical;
                        IsSliderReset = false;
                    }
                    displayFutureValueAfterAdjust();
                    break;
            }
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private void displayFutureValueAfterAdjust()
    {
        try
        {
            lblFutureValueAfterAdjust.Text = formatCurrency(FinancialPlannerAdapter.GetFutureValue(
                                                                    Persisted.PresentValue + ExtraInitial,
                                                                    Persisted.DepositPerYear + ExtraPeriodical,
                                                                    Persisted.YearsLeft,
                                                                    Persisted.ExpectedReturn));
            lblTargetValueExtras.Text = formatCurrency(Persisted.TargetValue);
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private string formatCurrency(decimal quantity)
    {
        return FinancialPlannerAdapter.FormatCurrency(quantity, BaseCurrencyAltSymbol);
    }
    
    #endregion


    #region Panel FutureValue - chart

    private void chPortfolioFutureValueDataBind(bool recalculateAll)
    {
        try
        {
            var seriesSpecs = new[] {
                new { SeriesName = "Volatiliteit (95%)", CreateSeries = (Func<string, Series>)createSeries_Volatility_2x },
                new { SeriesName = "Volatiliteit (68%)", CreateSeries = (Func<string, Series>)createSeries_Volatility_1x },
                new { SeriesName = "Doelvermogen", CreateSeries = (Func<string, Series>)createSeries_TargetValue },
                new { SeriesName = "Zonder aanpassing", CreateSeries = (Func<string, Series>)createSeries_BeforeAdjust },
                new { SeriesName = "Met aanpassing", CreateSeries = (Func<string, Series>)createSeries_AfterAdjust },
                new { SeriesName = "Historie", CreateSeries = (Func<string, Series>)createSeries_History }
            };

            Func<string, Series>[] alwaysRecalculate = new Func<string, Series>[] {
                createSeries_Volatility_2x,
                createSeries_Volatility_1x,
                createSeries_AfterAdjust
            };

            Dictionary<string, Series> cachedSeriesDict = chPortfolioFutureValue.Series.Cast<Series>()
                                                                                       .ToDictionary(s => s.Name, s => s);

            Series[] newSeriesList = seriesSpecs.Select(s =>
                                                    recalculateAll || alwaysRecalculate.Contains(s.CreateSeries) ?
                                                        s.CreateSeries(s.SeriesName) :
                                                    cachedSeriesDict.ContainsKey(s.SeriesName) ?
                                                        cachedSeriesDict[s.SeriesName] :
                                                        null)
                                                .Where(series => series != null)
                                                .ToArray();

            chPortfolioFutureValue.Series.Clear();
            foreach (Series series in newSeriesList)
                chPortfolioFutureValue.Series.Add(series);
        }
        catch (Exception ex) { displayErrorMessage(ex); }
    }

    private Series createSeries(string seriesName, Color color, SeriesChartType chartType, MarkerStyle markerStyle,
                                IEnumerable<DateTime> dates, params IEnumerable<decimal>[] valueLists)
    {
        Series series = new Series(seriesName);

        series.LegendText = seriesName;
        series.XValueType = ChartValueTypes.DateTime;
        series.EmptyPointStyle.Color = Color.Transparent;
        series.BorderWidth = 2;
        series.Color = color;
        series.MarkerStyle = markerStyle;

        if (dates.Count() == 1 && chartType != SeriesChartType.SplineRange)
        {
            series.Type = SeriesChartType.Point;
            series.MarkerSize = 8;
        }
        else
            series.Type = chartType;

        series.YValuesPerPoint = valueLists.Length;
        series.Points.DataBindXY(dates, valueLists);

        return series;
    }

    private Series createSeries_Volatility_2x(string seriesName)
    {
        return createSeries_Volatility(seriesName, HouseStyleColor.VeryLightGray, 2);
    }

    private Series createSeries_Volatility_1x(string seriesName)
    {
        return createSeries_Volatility(seriesName, HouseStyleColor.LightGray, 1);
    }

    private Series createSeries_Volatility(string seriesName, Color color, decimal stdDevRangeMultiplier)
    {
        try
        {
            decimal[][] volatilityValues = FinancialPlannerAdapter.GetVolatilitySeries(
                                                TotalPresentValue, TotalDepositPerYear, Persisted.YearsLeft,
                                                Persisted.ExpectedReturn, Persisted.StandardDeviation, stdDevRangeMultiplier);

            return createSeries(seriesName, color, SeriesChartType.SplineRange, MarkerStyle.None,
                                FutureDates, volatilityValues);
        }
        catch (Exception ex) { displayErrorMessage(ex); }

        return null;
    }

    private Series createSeries_TargetValue(string seriesName)
    {
        try
        {
            // TODO: also check that if Persisted.PresentValue == 0, there is at least one yearly deposit
            // (i.e., Persisted.YearsLeft > 1, when there is no initial or final deposit).
            // Also, do this check when calculating ProposedPeriodical. Better yet, put the checks in FinancialPlannerAdapter.
            if (Persisted.TargetValue > 0m && (Persisted.PresentValue > 0m || Persisted.DepositPerYear > 0m))
            {
                decimal idealInterestRate = FinancialPlannerAdapter.GetIdealInterestRate(
                                                Persisted.PresentValue, Persisted.DepositPerYear, Persisted.YearsLeft,
                                                Persisted.ExpectedReturn, Persisted.TargetValue);

                var targetValues = FinancialPlannerAdapter.GetFutureValueSeries(
                                        Persisted.PresentValue, Persisted.DepositPerYear, Persisted.YearsLeft, idealInterestRate);

                return createSeries(seriesName, HouseStyleColor.Red, SeriesChartType.Spline, MarkerStyle.Circle,
                                    FutureDates, targetValues);
            }
        }
        catch (Exception ex) { displayErrorMessage(ex); }

        return null;
    }

    private Series createSeries_BeforeAdjust(string seriesName)
    {
        try
        {
            var futureValuesBeforeAdjust = FinancialPlannerAdapter.GetFutureValueSeries(
                                                Persisted.PresentValue, Persisted.DepositPerYear, Persisted.YearsLeft,
                                                Persisted.ExpectedReturn);

            return createSeries(seriesName, HouseStyleColor.LightBlue, SeriesChartType.Spline, MarkerStyle.Circle,
                                FutureDates, futureValuesBeforeAdjust);
        }
        catch (Exception ex) { displayErrorMessage(ex); }

        return null;
    }

    private Series createSeries_AfterAdjust(string seriesName)
    {
        try
        {
            if (ExtrasSelectedIndex == 1 && (ExtraPeriodical > 0m || ExtraInitial > 0m))
            {
                var futureValuesAfterAdjust = FinancialPlannerAdapter.GetFutureValueSeries(
                                                    Persisted.PresentValue + ExtraInitial, Persisted.DepositPerYear + ExtraPeriodical,
                                                    Persisted.YearsLeft, Persisted.ExpectedReturn);

                return createSeries(seriesName, HouseStyleColor.DarkBlue, SeriesChartType.Spline, MarkerStyle.Circle,
                                    FutureDates, futureValuesAfterAdjust);
            }
        }
        catch (Exception ex) { displayErrorMessage(ex); }

        return null;
    }

    private Series createSeries_History(string seriesName)
    {
        try
        {
            if (IsAccountKnown)
            {
                DateTime[] valuationDates = FinancialPlannerAdapter.GetValuationDates(SelectedAccountId, Persisted.YearsLeft);

                if (valuationDates.Length > 0)
                {
                    List<Tuple<DateTime, decimal>> valuations = FinancialPlannerAdapter.GetValuationsTotalPortfolio(SelectedAccountId,
                                                                                                                    valuationDates)
                                                                                       .Select(v => Tuple.Create(v.Item1, Math.Max(0, v.Item2)))
                                                                                       .ToList();
                    
                    valuations.Add(Tuple.Create(DateTime.Today, Persisted.PresentValue));

                    return createSeries(seriesName, HouseStyleColor.Yellow, SeriesChartType.Spline, MarkerStyle.None,
                                        valuations.Select(v => v.Item1).ToArray(),
                                        valuations.Select(v => v.Item2).ToArray());
                }
            }
        }
        catch (Exception ex) { displayErrorMessage(ex); }

        return null;
    }

    #endregion


    #region Panel FutureValue - properties

    protected bool IsSliderReset
    {
        get
        {
            object b = ViewState["IsSliderReset"];
            return b == null ? true : (bool)b;
        }
        set { ViewState["IsSliderReset"] = value; }
    }

    protected string BaseCurrencyAltSymbol
    {
        get
        {
            string s = ViewState["BaseCurrencyAltSymbol"] as string;
            if (string.IsNullOrEmpty(s))
            {
                s = FinancialPlannerAdapter.GetBaseCurrencyAltSymbol();
                ViewState["BaseCurrencyAltSymbol"] = s;
            }
            return s;
        }
    }

    protected decimal TrafficLightValue
    {
        set { imgTrafficLight.ImageUrl = string.Format("~/Images/TrafficLight/Set2/{0}-light.jpg",
                                                       CommonAdapter.GetTrafficLightColor(value).Name.ToLower()); }
    }

    protected int ExtrasSelectedIndex
    {
        get { return mvwExtras.ActiveViewIndex; }
        set
        {
            int newIndex = value,
                oldIndex = 1 - newIndex;

            string[] choiceLabels = new string[] { "Voorstel aanpassing",
                                                   "Aanpassingen combineren" };

            lblChoiceOfExtras.Text = choiceLabels[newIndex] + ":";
            lnkChoiceOfExtras.Text = choiceLabels[oldIndex];

            mvwExtras.ActiveViewIndex = newIndex;
        }
    }

    protected decimal ExtraPeriodical
    {
        get { return ExtrasSelectedIndex == 1 ? readInt(txtExtraPeriodical_BoundControl) : 0m; }
        set
        {
            string formattedValue = value.ToString(textBoxIntegerFormat);
            txtExtraPeriodical.Text = formattedValue;
            txtExtraPeriodical_BoundControl.Text = formattedValue;
        }
    }
    
    protected decimal ExtraInitial
    {
        get { return ExtrasSelectedIndex == 1 ? readInt(txtExtraInitial_BoundControl) : 0m; }
        set
        {
            string formattedValue = value.ToString(textBoxIntegerFormat);
            txtExtraInitial.Text = formattedValue;
            txtExtraInitial_BoundControl.Text = formattedValue;
        }
    }

    protected FinancialDataView Persisted
    {
        get
        {
            if (persisted == null)
                persisted = ViewState["PersistedFinancialDataView"] as FinancialDataView;

            return persisted;
        }
        set
        {
            ViewState["PersistedFinancialDataView"] = value;
            persisted = value;
        }
    }
    private FinancialDataView persisted;

    protected decimal TotalPresentValue
    {
        get { return ExtrasSelectedIndex == 0 ? Persisted.PresentValue : Persisted.PresentValue + ExtraInitial; }
    }

    protected decimal TotalDepositPerYear
    {
        get { return ExtrasSelectedIndex == 0 ? Persisted.DepositPerYear : Persisted.DepositPerYear + ExtraPeriodical; }
    }

    protected decimal ProposedPeriodical
    {
        get
        {
            try
            {
                decimal proposedPeriodical = FinancialPlannerAdapter.GetProposedPeriodical(
                                                Persisted.PresentValue, Persisted.DepositPerYear, Persisted.YearsLeft,
                                                Persisted.ExpectedReturn, Persisted.StandardDeviation, Persisted.TargetValue);

                return Math.Max(0, proposedPeriodical - Persisted.DepositPerYear);
            }
            catch (Exception ex)
            {
                displayErrorMessage("(Extra periodieke inleg) ", ex);
                return 0m;
            }
        }
    }

    protected decimal ProposedInitial
    {
        get
        {
            try
            {
                decimal proposedInitial = FinancialPlannerAdapter.GetProposedInitial(
                                                Persisted.PresentValue, Persisted.DepositPerYear, Persisted.YearsLeft,
                                                Persisted.ExpectedReturn, Persisted.StandardDeviation, Persisted.TargetValue);

                return Math.Max(0, proposedInitial - Persisted.PresentValue);
            }
            catch (Exception ex)
            {
                displayErrorMessage("(Extra eenmalige inleg) ", ex);
                return 0m;
            }
        }
    }

    protected DateTime[] FutureDates
    {
        get
        {
            if (futureDates == null)
                futureDates = FinancialPlannerAdapter.GetRangeOfDates(Persisted.YearsLeft);

            return futureDates;
        }
    }
    private DateTime[] futureDates;

    #endregion

    #endregion


    #region Error Handling

    private void displayErrorMessage(Exception ex)
    {
        displayErrorMessage("", ex);
    }

    private void displayErrorMessage(string message, Exception ex)
    {
        message += ex != null ? Utility.GetCompleteExceptionMessage(ex, " ", " ") : "";

        displayMessage("ERROR: " + message);
    }

    private void displayMessage(string message)
    {
        pnlErrorFutureValue.Visible = true;
        ActiveErrorLabel.Text += (ActiveErrorLabel.Text != "" ? "<br />" : "") +
                                  message;
    }

    protected ErrorLabel ActiveErrorLabel { get { return pnlFutureValue.Visible ? elbFutureValue : elbGivenFields; } }

    #endregion
}

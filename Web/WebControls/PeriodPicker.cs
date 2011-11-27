using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace B4F.Web.WebControls
{
    [DefaultProperty("DisplayPeriod")]
    [ValidationPropertyAttribute("DisplayPeriod")]
    [ToolboxData("<{0}:PeriodPicker runat=server></{0}:PeriodPicker>")]
    public class PeriodPicker : CompositeControl, INamingContainer
    {
        #region Props

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The currently selected period."), Category("Misc")]
        public int SelectedPeriod
        {
            get
            {
                int retval = 0;
                if (SelectedYear > 0 && SelectedQuarter > 0)
                    retval = SelectedYear * 100 + SelectedQuarter;
                return retval;
            }
            set
            {
                if (value > 0)
                {
                    int year = value / 100;
                    SelectedYear = year;
                    SelectedQuarter = value - (year * 100);
                }
                else
                {
                    SelectedYear = 0;
                    SelectedQuarter = 0;
                }
            }
        }


        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The currently selected year."), Category("Misc")]
        public int SelectedYear
        {
            get
            {
                object obj = ViewState["SelectedYear"];
                return (obj == null) ? 0 : Convert.ToInt32(obj);
            }
            set
            {
                if (isYear(value))
                {
                    ViewState["SelectedYear"] = value;
                    if (cboYear != null)
                    {
                        if (cboYear.Items.FindByValue(value.ToString()) != null)
                            cboYear.SelectedValue = value.ToString();
                        else 
                            cboYear.Text = value.ToString();
                    }
                }
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The currently selected quarter."), Category("Misc")]
        public int SelectedQuarter
        {
            get
            {
                object obj = ViewState["SelectedQuarter"];
                return (obj == null) ? 0 : Convert.ToInt32(obj);
            }
            set
            {
                if (value >= 0 && value < 5)
                {
                    ViewState["SelectedQuarter"] = value;
                    if (cboQuarter != null)
                        cboQuarter.SelectedValue = value.ToString();
                }
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        [Themeable(false)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DisplayPeriod
        {
            get
            {
                EnsureChildControls();
                if (cboYear.Text != "" && cboQuarter.Text != "" && cboYear.IsValid() && cboQuarter.IsValid())
                    return string.Format("{0}0{1}", cboYear.Text ,cboQuarter.Text);
                else
                    return "";
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(3)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Specifies how many years will be displayed in the DropDownList BEFORE the current year."), Category("Appearance")]
        public int ListYearsBeforeCurrent
        {
            get
            {
                object i = ViewState["ListYearsBeforeCurrent"];
                return ((i == null) ? 3 : (int)i);
            }
            set { ViewState["ListYearsBeforeCurrent"] = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(3)]
        [Description("Specifies how many years will be displayed in the DropDownList AFTER the current year."), Category("Appearance")]
        public int ListYearsAfterCurrent
        {
            get
            {
                object i = ViewState["ListYearsAfterCurrent"];
                return ((i == null) ? 3 : (int)i);
            }
            set { ViewState["ListYearsAfterCurrent"] = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(false)]
        [Description("Default to the current period."), Category("Behavior")]
        public bool DefaultToCurrentPeriod
        {
            get
            {
                object i = ViewState["DefaultToCurrentPeriod"];
                return ((i == null) ? false : (bool)i);
            }
            set { ViewState["DefaultToCurrentPeriod"] = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(true)]
        [Description("Specifies whether the Delete button (which empties the picker) is visible."), Category("Behavior")]
        public bool IsButtonDeleteVisible
        {
            get
            {
                object i = ViewState["IsButtonDeleteVisible"];
                return ((i == null) ? true : (bool)i);
            }
            set { ViewState["IsButtonDeleteVisible"] = value; }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            SelectedYear = 0;
            if (cboYear != null && !string.IsNullOrEmpty(cboYear.Text))
                cboYear.Text = string.Empty;
            SelectedQuarter = 0;
            if (cboQuarter != null && !string.IsNullOrEmpty(cboQuarter.Text))
                cboQuarter.Text = string.Empty;
        }

        #endregion

        #region Overrides

        protected override void CreateChildControls()
        {
            if (Controls.Count == 0)
            {
                Controls.Clear();

                cboYear = new ComboBox();
                cboYear.ID = CONST_YEAR;
                setComboProps(cboYear, 26, false);

                cboQuarter = new ComboBox();
                cboQuarter.ID = CONST_QUARTER;
                setComboProps(cboQuarter, 16, true);

                if (IsButtonDeleteVisible)
                {
                    imgDelete = new ImageButton();
                    imgDelete.ID = "DEL";
                    imgDelete.CausesValidation = false;
                    imgDelete.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ImageEx), "B4F.Web.WebControls.Images.delete.gif");
                    imgDelete.Visible = true;
                    imgDelete.Click += new ImageClickEventHandler(imgDelete_ClientClick);
                }

                int startYear = DateTime.Today.Year - ListYearsBeforeCurrent;
                int endYear = DateTime.Today.Year + ListYearsAfterCurrent;

                populateBox(cboYear, startYear, endYear, "0000");
                populateBox(cboQuarter, 1, 4, "00");

                if (SelectedYear > 0 || (!DefaultToCurrentPeriod && SelectedYear == 0))
                {
                    if (cboYear.Items.FindByValue(SelectedYear.ToString()) != null)
                        cboYear.SelectedValue = SelectedYear.ToString();
                    else
                        cboYear.Text = SelectedYear.ToString();
                }
                else
                {
                    SelectedYear = DateTime.Today.Year;
                    cboYear.SelectedValue = SelectedYear.ToString();
                }

                if (SelectedQuarter > 0 || (!DefaultToCurrentPeriod && SelectedQuarter == 0))
                    cboQuarter.SelectedValue = SelectedQuarter.ToString();
                else
                {
                    SelectedQuarter = getCurrentQuarter();
                    cboQuarter.SelectedValue = SelectedQuarter.ToString();
                }

                this.Controls.Add(cboYear);
                this.Controls.Add(cboQuarter);
                if (IsButtonDeleteVisible)
                    this.Controls.Add(imgDelete);
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.RenderBeginTag(HtmlTextWriterTag.Div);

            cboYear.RenderControl(output);
            cboQuarter.RenderControl(output);

            if (IsButtonDeleteVisible)
                imgDelete.RenderControl(output);

            output.RenderEndTag();
        }

        #endregion

        #region Helpers

        private void setComboProps(ComboBox cboBox, int pixels, bool limitToList)
        {
            cboBox.EditingStyle = EditingStyle.ComboBox;
            cboBox.LimitToList = limitToList;
            cboBox.SkinID = "custom-width";
            cboBox.Width = Unit.Pixel(pixels);
            cboBox.CssClass = "alignleft";
            cboBox.AutoPostBack = true;
            cboBox.SelectedIndexChanged += new EventHandler(ComboBox_Changed);
            cboBox.TextChanged += new EventHandler(ComboBox_Changed);
        }

        private void populateBox(ComboBox cboBox, int startValue, int endValue, string format)
        {
            int count = endValue - startValue + 1;
            int counter = 0;
            ListItem[] items = new ListItem[count];

            for (int i = startValue; i < (endValue + 1); i++)
            {
                items[counter] = new ListItem(i.ToString(format), i.ToString());
                counter++;
            }
            cboBox.Items.AddRange(items);
            cboBox.Items.Insert(0, new ListItem("", CONST_ZERO.ToString()));
        }

        private int getCurrentQuarter()
        {
            //Get the current month
            int i = DateTime.Today.Month;

            //Based on the current month return the quarter
            if (i <= 3)
            { return 1; }
            else if (i >= 4 && i <= 6)
            { return 2; }
            else if (i >= 7 && i <= 9)
            { return 3; }
            else if (i >= 10 && i <= 12)
            { return 4; }
            else
                //Something probably is wrong 
                return 0;
        }

        private bool isYear(int value)
        {
            bool retVal = false;
            if (value >= CONST_MIN_YEAR && value <= CONST_MAX_YEAR)
                retVal = true;
            return retVal;
        }

        #endregion

        #region Event Handling

        protected void ComboBox_Changed(object sender, EventArgs e)
        {
            ComboBox cboBox = (ComboBox)sender;
            int value = 0;
            bool hasChanged = false;

            if (cboBox.SelectedValue != "" && int.TryParse(cboBox.SelectedValue, out value))
            {
                if (cboBox.ID == CONST_YEAR && SelectedYear != value)
                {
                    SelectedYear = value;
                    hasChanged = true;
                }
                else if (SelectedQuarter != value)
                {
                    SelectedQuarter = value;
                    hasChanged = true;
                }
            }
            else if (cboBox.Text != "" && cboBox.ID == CONST_YEAR)
            {
                if (int.TryParse(cboBox.Text, out value) && isYear(value) && SelectedYear != value)
                {
                    SelectedYear = value;
                    hasChanged = true;
                }
            }
            if (hasChanged)
                OnPeriodChanged();
        }

        protected void imgDelete_ClientClick(object sender, ImageClickEventArgs e)
        {
            Clear();
            OnPeriodChanged();
        }

        protected void OnPeriodChanged()
        {
            if (PeriodChanged != null)
                PeriodChanged(this, EventArgs.Empty);
        }

        #endregion

        [Description("Event triggered when the period has changed."), Category("Behavior")]
        public EventHandler PeriodChanged;

        #region Private Members

        private ComboBox cboYear;
        private ComboBox cboQuarter;
        private ImageButton imgDelete;
        private const string CONST_YEAR = "YEAR";
        private const string CONST_QUARTER = "QUARTER";
        private const int CONST_ZERO = 0;
        private const int CONST_MIN_YEAR = 2000;
        private const int CONST_MAX_YEAR = 2050;

        #endregion

    }
}

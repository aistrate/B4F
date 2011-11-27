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
    [DefaultProperty("DisplayTime")]
    [ValidationPropertyAttribute("DisplayTime")]
    [ToolboxData("<{0}:TimePicker runat=server></{0}:TimePicker>")]
    public class TimePicker : CompositeControl, INamingContainer // IPostBackDataHandler  //, IScriptControl
    {
        #region Props

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The currently selected hour."), Category("Misc")]
        public int SelectedHour
        {
            get
            {
                object obj = ViewState["SelectedHour"];
                return (obj == null) ? 0 : Convert.ToInt32(obj);
            }
            set
            {
                if (value >= 0 && value < 24)
                {
                    ViewState["SelectedHour"] = value;
                    if (cboHour != null)
                        cboHour.SelectedValue = value.ToString();
                }
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The currently selected minute."), Category("Misc")]
        public int SelectedMinute
        {
            get
            {
                object obj = ViewState["SelectedMinute"];
                return (obj == null) ? 0 : Convert.ToInt32(obj);
            }
            set
            {
                if (value >= 0 && value < 60)
                {
                    ViewState["SelectedMinute"] = value;
                    if (cboMinute != null)
                        cboMinute.SelectedValue = value.ToString();
                }
            }
        }
        
        [Bindable(false)]
        [Browsable(false)]
        [Themeable(false)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DisplayTime
        {
            get
            {
                EnsureChildControls();
                if (cboHour.Text != "" && cboMinute.Text != "" && cboHour.IsValid() && cboMinute.IsValid())
                    return cboHour.Text + ":" + cboMinute.Text;
                else
                    return "";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The currently selected date.
        /// </summary>
        [Browsable(true)]
        public DateTime GetTime(DateTime date)
        {
            DateTime retval = date.Date;
            if (SelectedHour > 0)
                retval = retval.AddHours(SelectedHour);
            if (SelectedMinute > 0)
                retval = retval.AddMinutes(SelectedMinute);
            return retval;
        }

        #endregion

        protected override void CreateChildControls()
        {
            if (Controls.Count == 0)
            {
                Controls.Clear();

                cboHour = new ComboBox();
                cboHour.ID = CONST_HOUR;
                setComboProps(cboHour);

                cboMinute = new ComboBox();
                cboMinute.ID = CONST_MINUTE;
                setComboProps(cboMinute);

                populateBox(cboHour, 23);
                populateBox(cboMinute, 59);

                cboHour.SelectedValue = SelectedHour.ToString();
                cboMinute.SelectedValue = SelectedMinute.ToString();

                this.Controls.Add(cboHour);
                this.Controls.Add(cboMinute);
            }
        }

        private void setComboProps(ComboBox cboBox)
        {
            cboBox.EditingStyle = EditingStyle.ComboBox;
            cboBox.LimitToList = true;
            cboBox.SkinID = "custom-width";
            cboBox.Width = Unit.Pixel(20);
            cboBox.CssClass = "alignleft";
            cboBox.AutoPostBack = true;
            cboBox.SelectedIndexChanged += new EventHandler(ComboBox_Changed);
            cboBox.TextChanged += new EventHandler(ComboBox_Changed);
        }

        private void populateBox(ComboBox cboBox, int endValue)
        {
            ListItem[] items = new ListItem[endValue + 1];

            for (int i = 0; i < (endValue + 1); i++)
                items[i] = new ListItem(i.ToString("00"), i.ToString());
            cboBox.Items.AddRange(items);
        }

        //protected override void AddAttributesToRender(HtmlTextWriter writer)
        //{
        //    writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
        //    writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "nowrap");

        //    string accessKey = this.AccessKey;
        //    if (accessKey != null && accessKey.Length > 0)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
        //    }

        //    if (!this.Enabled)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
        //    }

        //    int tabIndex = this.TabIndex;
        //    if (tabIndex != 0)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
        //    }

        //    string toolTip = this.ToolTip;
        //    if (toolTip != null && toolTip.Length > 0)
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
        //    }

        //    if (this.ControlStyleCreated && !this.ControlStyle.IsEmpty)
        //    {
        //        this.ControlStyle.AddAttributesToRender(writer, this);
        //    }
        //    if (this.HasAttributes)
        //    {
        //        System.Web.UI.AttributeCollection attributes = this.Attributes;
        //        foreach (string str in attributes.Keys)
        //        {
        //            writer.AddAttribute(str, attributes[str]);
        //        }
        //    }

        //    string display = this.Attributes.CssStyle["display"];

        //    if (String.IsNullOrEmpty(display))
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
        //}


        //protected override void OnPreRender(EventArgs e)
        //{
        //    base.OnPreRender(e);

        //    if (Page != null)
        //    {
        //        Page.RegisterRequiresPostBack(this);
        //    }
        //}

        //protected override void Render(HtmlTextWriter writer)
        //{
        //    if (this.DesignMode)
        //    {
        //        EnsureChildControls();
        //    }

        //    //writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //    //cboHour.RenderControl(writer);
        //    //cboMinute.RenderControl(writer);

        //    //writer.RenderEndTag();
        //}

        protected override void RenderContents(HtmlTextWriter output)
        {
            //this.AddAttributesToRender(output);
            output.RenderBeginTag(HtmlTextWriterTag.Div);

            cboHour.RenderControl(output);
            //output.AddAttribute(HtmlTextWriterAttribute.Id, String.Format("{0}_{1}", this.ClientID, CONST_HOUR));
            cboMinute.RenderControl(output);
            //output.AddAttribute(HtmlTextWriterAttribute.Id, String.Format("{0}_{1}", this.ClientID, CONST_MINUTE));

            output.RenderEndTag();

        }

        //#region IPostBackDataHandler
        //bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        //{
        //    string hourKey = String.Format("{0}${1}", postDataKey, CONST_HOUR);
        //    string minuteKey = String.Format("{0}${1}", postDataKey, CONST_MINUTE);
        //    string[] hourArray = postCollection.GetValues(hourKey);
        //    string[] minuteArray = postCollection.GetValues(minuteKey);
        //    int hour = 0;
        //    int minute = 0;
        //    bool hasChanged = false;

        //    if (hourArray != null && hourArray.Length > 0 && int.TryParse(hourArray[0], out hour))
        //    {
        //        if (SelectedHour != hour)
        //        {
        //            SelectedHour = hour;
        //            hasChanged = true;
        //        }
        //    }

        //    if (minuteArray != null && minuteArray.Length > 0 && int.TryParse(minuteArray[0], out minute))
        //    {
        //        if (SelectedMinute != minute)
        //        {
        //            SelectedMinute = minute;
        //            hasChanged = true;
        //        }
        //    }

        //    if (hasChanged)
        //        OnSelectionChanged();

        //    return false;
        //}

        //void IPostBackDataHandler.RaisePostDataChangedEvent()
        //{
        //    // Part of the IPostBackDataHandler contract.  Invoked if we ever returned true from the
        //    // LoadPostData method (indicates that we want a change notification raised).  Since we
        //    // always return false, this method is just a no-op.
        //}
        //#endregion

        protected void ComboBox_Changed(object sender, EventArgs e)
        {
            ComboBox cboBox = (ComboBox)sender;
            int value = 0;
            if (cboBox.SelectedValue != "" && int.TryParse(cboBox.SelectedValue, out value))
            {
                if (cboBox.ID == CONST_HOUR)
                    SelectedHour = value;
                else
                    SelectedMinute = value;
            }
            OnSelectionChanged();
        }

        protected void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        [Description("Event triggered when the selected time is changed."), Category("Behavior")]
        public EventHandler SelectionChanged;

        #region Private Members

        private ComboBox cboHour;
        private ComboBox cboMinute;
        private const string CONST_HOUR = "HOUR";
        private const string CONST_MINUTE = "MINUTE";

        #endregion

        //#region IScriptControl Members

        //IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        //{
        //    ScriptControlDescriptor descriptor = new ScriptControlDescriptor("B4F.Web.WebControls.TimePicker", this.ClientID);
        //    descriptor.AddElementProperty("comboBox", String.Format("{0}_{1}", this.ClientID, CONST_HOUR));
        //    descriptor.AddElementProperty("comboBox", String.Format("{0}_{1}", this.ClientID, CONST_MINUTE));

        //    yield return descriptor;
        //}

        //IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        //{
        //    return null;
        //}

        //#endregion

    }
}

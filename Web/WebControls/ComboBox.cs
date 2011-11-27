using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace B4F.Web.WebControls
{
    public enum EditingStyle { DropDownList = 0, ComboBox = 1, DropDownCombo = 2 }

    [ToolboxData("<{0}:ComboBox runat=server></{0}:ComboBox>")]
    [ParseChildren(true, "Items")]
    [ControlValueProperty("SelectedValue")]
    [SupportsEventValidation()]
    [ValidationProperty("Text")]
    [Designer("System.Web.UI.Design.WebControls.ListControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public partial class ComboBox : ListControl, INamingContainer, IPostBackDataHandler, IScriptControl
    {
        #region Props

        [Browsable(true)]
        [Themeable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        public string HoverCssClass
        {
            get
            {
                EnsureChildControls();
                return _txtBox.HoverCssClass;
            }
            set
            {
                EnsureChildControls();
                _txtBox.HoverCssClass = value;
            }
        }
        [Browsable(true)]
        [Themeable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        public string FocusCssClass
        {
            get
            {
                EnsureChildControls();
                return _txtBox.FocusCssClass;
            }
            set
            {
                EnsureChildControls();
                _txtBox.FocusCssClass = value;
            }
        }
        [Browsable(true)]
        [Themeable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        public string ReadOnlyCssClass
        {
            get
            {
                EnsureChildControls();
                return _txtBox.ReadOnlyCssClass;
            }
            set
            {
                EnsureChildControls();
                _txtBox.ReadOnlyCssClass = value;
            }
        }
        [Browsable(true)]
        [Themeable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        public override string CssClass
        {
            get
            {
                EnsureChildControls();
                return _txtBox.CssClass;
            }
            set
            {
                EnsureChildControls();
                _txtBox.CssClass = value;
            }
        }

        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(EditingStyle.DropDownList)]
        [Category("Behavior")]
        public EditingStyle EditingStyle
        {
            get
            {
                object obj = ViewState["EditingStyle"];
                return (obj == null) ? EditingStyle.DropDownList : (EditingStyle)obj;
            }
            set
            {
                ViewState["EditingStyle"] = value;
            }
        }

        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool LimitToList
        {
            get
            {
                object obj = ViewState["LimitToList"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["LimitToList"] = value;
            }
        }

        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool MatchOnValue
        {
            get
            {
                object obj = ViewState["MatchOnValue"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["MatchOnValue"] = value;
            }
        }

        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public string OnClientChange
        {
            get
            {
                object obj = ViewState["OnClientChange"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["OnClientChange"] = value;
            }
        }

        #endregion

        #region Private Members

        private enum ComboBoxProps { Value = 0, Text = 1 }
        private TextBoxEx _txtBox;
        private Image _img;
        
        #endregion

        #region Overrides

        public override ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }

        #region Behavior Properties

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(false)]
        [DefaultValue("")]
        [Category("Behavior")]
        public override string ValidationGroup
        {
            get
            {
                EnsureChildControls();
                return _txtBox.ValidationGroup;
            }
            set
            {
                EnsureChildControls();

                _txtBox.ValidationGroup = value;
            }
        }

        [Bindable(true)]
        [Browsable(false)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public override string ToolTip
        {
            get
            {
                EnsureChildControls();
                return _txtBox.ToolTip;
            }
            set
            {
                EnsureChildControls();
                _txtBox.ToolTip = value;
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(false)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                EnsureChildControls();
                base.Enabled = value;
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(false)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                EnsureChildControls();
                return this._txtBox.ReadOnly;
            }
            set
            {
                EnsureChildControls();
                this._txtBox.ReadOnly = value;
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        [Themeable(false)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                EnsureChildControls();
                return _txtBox.Text;
            }
            set
            {
                EnsureChildControls();
                _txtBox.Text = value;
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        [Themeable(false)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string SelectedValue
        {
            get
            {
                EnsureChildControls();
                ListItem itm = getItem(_txtBox.Text, ComboBoxProps.Text);
                if (itm != null)
                    return itm.Value;
                else
                    return "";
            }
            set
            {
                EnsureChildControls();
                ListItem itm = getItem(value, ComboBoxProps.Value);
                if (itm != null)
                    _txtBox.Text = itm.Text;
            }
        }

        #endregion

        public bool IsValid()
        {
            return isValid(false);
        }

        protected bool isValid(bool matchOnValue)
        {
            bool retVal = true;
            if (Text.Length > 0 && LimitToList && Items.Count > 0)
            {
                retVal = false;
                foreach (ListItem itm in this.Items)
                {
                    if (compareValues(itm, Text, matchOnValue))
                        return true;
                }
            }
            return retVal;
        }

        private bool compareValues(ListItem itm, string text, bool matchOnValue)
        {
            if (text.Length > 0)
            {
                if (itm.Text.Equals(text))
                    return true;
                else if (matchOnValue && itm.Value.Equals(text))
                    return true;
            }
            return false;

        }
        #endregion

        #region Appearance & Style Properties
        public override Unit Width
        {
            get
            {
                EnsureChildControls();
                return _txtBox.Width;
            }
            set
            {
                EnsureChildControls();
                _txtBox.Width = value;
            }
        }

        public override Unit Height
        {
            get
            {
                EnsureChildControls();
                return _txtBox.Height;
            }
            set
            {
                EnsureChildControls();
                _txtBox.Height = value;
            }
        }

        public override FontInfo Font
        {
            get
            {
                return _txtBox.Font;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(5)]
        [Category("Appearance")]
        public int Rows
        {
            get
            {
                object obj = ViewState["Rows"];
                return (obj == null || (int)obj <= 0) ? 5 : (int)obj;
            }
            set
            {
                ViewState["Rows"] = value;
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [UrlProperty]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public string DropDownImageUrl
        {
            get
            {
                object obj = ViewState["ImageUrl"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["ImageUrl"] = value;
            }
        }

        
        #endregion       
                
        #region Protected Overridden Methods

        protected override void CreateChildControls()
        {
            Controls.Clear();

            _txtBox = new TextBoxEx();
            _txtBox.ID = "TB";
            _txtBox.TextChanged += new EventHandler(_txtBox_TextChanged);

            _img = new Image();
            _img.ID = "CDD";
            
            this.Controls.Add(_txtBox);
            this.Controls.Add(_img);
        }


        protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
        {
            base.PerformDataBinding(dataSource);
            if (this.SelectedIndex > -1)
            {
                this.Text = this.Items[this.SelectedIndex].Text;
            }
        }

        void _txtBox_TextChanged(object sender, EventArgs e)
        {
            if (isValid(false))
                this.OnTextChanged(e);
            else if (isValid(MatchOnValue))
                this.Text = this.Items[FindByValueInternal(Text, true)].Text;
            else if (this.SelectedIndex > -1)
                this.Text = this.Items[0].Text;
            else
                this.Text = "";
        }

        #endregion

        #region Render
        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager manager;
            manager = ScriptManager.GetCurrent(this.Page);

            if (manager == null)
                throw new InvalidOperationException("A ScriptManager is required on the page.");

            manager.RegisterScriptControl(this);

            if (String.IsNullOrEmpty(_img.ImageUrl))
                _img.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ComboBox), "B4F.Web.WebControls.Images.dropdown.gif");

            base.OnPreRender(e);
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "nowrap");

            string accessKey = this.AccessKey;
            if (accessKey != null && accessKey.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
            }
            
            if (!this.Enabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            
            int tabIndex = this.TabIndex;
            if (tabIndex != 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            
            string toolTip = this.ToolTip;
            if (toolTip != null && toolTip.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
            }

            if (this.ControlStyleCreated && !this.ControlStyle.IsEmpty)
            {
                this.ControlStyle.AddAttributesToRender(writer, this);
            }
            if (this.HasAttributes)
            {
                System.Web.UI.AttributeCollection attributes = this.Attributes;
                foreach (string str in attributes.Keys)
                {
                    writer.AddAttribute(str, attributes[str]);
                }
            }

            string display = this.Attributes.CssStyle["display"];
            
            if (String.IsNullOrEmpty(display))
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
            {
                EnsureChildControls();
            }
            else
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);

            this.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.AutoComplete, "off");
            _txtBox.RenderControl(writer);
            _img.RenderControl(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Id,String.Format("{0}_{1}",this.ClientID,"LBC"));
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Top, "-100px");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "-100px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "1px");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "Red");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "solid");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Size, this.Rows.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            writer.AddAttribute(HtmlTextWriterAttribute.Id, String.Format("{0}_{1}", this.ClientID, "LB"));
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            // Get the font stuff
            if (_txtBox != null)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, _txtBox.Font.Size.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, _txtBox.Font.Name);
            }
            writer.RenderBeginTag(this.TagName);
            base.RenderContents(writer);
            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.RenderEndTag();
            //Page.Response.Write( Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this._txtBox,"")));
        }
        #endregion

        #region IPostBackDataHandler
        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (!base.IsEnabled)
            {
                return false;
            }
            string textBoxKey = String.Format("{0}${1}", postDataKey, "TB");
            string listBoxKey = postDataKey;
            string[] textArray = postCollection.GetValues(textBoxKey);
            string[] valueArray = postCollection.GetValues(listBoxKey);
            
            this.EnsureDataBound();
            
            if (textArray != null && valueArray != null)
            {
                bool returnValue = false;
                this.ValidateEvent(listBoxKey, valueArray[0]);
                string text = textArray[0];
                int num1 = this.FindByValueInternal(valueArray[0], false);
                if (this.SelectedIndex != num1)
                {
                    base.SetPostDataSelection(num1);                                        
                    returnValue = true;
                }
                if (this.Text != text)
                {
                    this.Text = text;
                    returnValue = true;
                }
                if (!TextExists(text, false))
                {
                    base.SetPostDataSelection(-1);
                }

                return returnValue;
            }
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (this.AutoPostBack)
            {
                if (this.CausesValidation)
                {
                    this.Page.Validate(this.ValidationGroup);
                }
            }
            this.OnSelectedIndexChanged(EventArgs.Empty);
        }
        #endregion

        #region Helpers
        private void ValidateEvent(string uniqueID, string eventArgument)
        {
            if (this.Page != null && this.EnableViewState)
            {
                this.Page.ClientScript.ValidateEvent(uniqueID, eventArgument);
            }
        }

        private int FindByValueInternal(string value, bool includeDisabled)
        {
            int num1 = 0;
            foreach (ListItem itm in this.Items)
            {
                if (itm.Value.Equals(value) && (includeDisabled || itm.Enabled))
                {
                    return num1;
                }
                num1++;
            }
            return -1;
        }

        private bool TextExists(string text, bool includeDisabled)
        {
            foreach (ListItem itm in this.Items)
            {
                if (compareValues(itm, text, false) && (includeDisabled || itm.Enabled))
                {
                    return true;
                }
            }
            return false;
        }

        private ListItem getItem(string matchText, ComboBoxProps matchOn)
        {
            foreach (ListItem itm in this.Items)
            {
                if (matchOn == ComboBoxProps.Value && itm.Value.Equals(matchText))
                    return itm;
                else if (matchOn == ComboBoxProps.Text && itm.Text.Equals(matchText))
                    return itm;
            }
            return null;
        }
        
        #endregion
        
        #region IScriptControl Members

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("B4F.Web.WebControls.ComboBox", this.ClientID);
            descriptor.AddElementProperty("dropDownImage", String.Format("{0}_{1}",this.ClientID,"CDD"));
            descriptor.AddElementProperty("textBox", String.Format("{0}_{1}", this.ClientID, "TB"));
            descriptor.AddElementProperty("listBox", String.Format("{0}_{1}", this.ClientID, "LB"));
            descriptor.AddElementProperty("listBoxContainer", String.Format("{0}_{1}", this.ClientID, "LBC"));
            
            descriptor.AddProperty("autoPostBack", this.AutoPostBack);
            descriptor.AddProperty("editingStyle", this.EditingStyle);            
            descriptor.AddProperty("rtl", System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft);

            if (!String.IsNullOrEmpty(this.OnClientChange))
                descriptor.AddEvent("change", this.OnClientChange);

            yield return descriptor;
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            List<ScriptReference> references = new List<ScriptReference>(2);
            references.Add(new ScriptReference(Page.ClientScript.GetWebResourceUrl(typeof(ComboBox),
                "B4F.Web.WebControls.ClientScript.Common.js")));
            references.Add(new ScriptReference(Page.ClientScript.GetWebResourceUrl(typeof(ComboBox),
                "B4F.Web.WebControls.ClientScript.ComboBox.js")));
            
            return references;
        }

        #endregion
    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace B4F.Web.WebControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TruncatedLabel runat=server></{0}:TruncatedLabel>")]
    public class TruncatedLabel : Label
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public virtual string LongText
        {
            get
            {
                object t = ViewState["LongText"];
                return ((t == null) ? string.Empty : (string)t);
            }
            set
            {
                ViewState["LongText"] = value;

                string compactedText = Regex.Replace(value, @"\s+", " ");

                string toolTip = PermanentToolTip;
                if (MaxLength < 4 || compactedText.Length <= MaxLength)
                    Text = compactedText;
                else
                {
                    Text = compactedText.Substring(0, MaxLength - 3) + "...";
                    toolTip = value + (string.IsNullOrEmpty(toolTip) ? "" : " -> " + toolTip);
                }

                if (!CustomToolTip)
                    ToolTip = toolTip;
            }
        }

        /// <summary>
        /// Minimum length is 4;
        /// zero means unlimited length
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(0)]
        public virtual int MaxLength
        {
            get
            {
                object m = ViewState["MaxLength"];
                return ((m == null) ? 0 : (int)m);
            }
            set { ViewState["MaxLength"] = (value >= 4 ? value : 0); }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool CustomToolTip
        {
            get
            {
                object b = ViewState["CustomToolTip"];
                return ((b == null) ? false : (bool)b);
            }
            set { ViewState["CustomToolTip"] = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public virtual string PermanentToolTip
        {
            get
            {
                object t = ViewState["PermanentToolTip"];
                return ((t == null) ? string.Empty : (string)t);
            }
            set
            {
                ViewState["PermanentToolTip"] = value;
                string correctedText = Regex.Replace(LongText, @"\s+", " ");
                if (MaxLength < 4 || correctedText.Length <= MaxLength)
                    ToolTip = value;
                else
                    ToolTip = correctedText + (string.IsNullOrEmpty(PermanentToolTip) ? "" : " -> " + PermanentToolTip);
            }
        }
    }
}

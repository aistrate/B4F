using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Trunc
{
    public class TruncLabel2 : Label
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            string text = Regex.Replace(LongText, @"\s+", " ");
            string compactText = text;
            if (MaxLength < 4 || text.Length <= MaxLength)
                writer.AddAttribute("title", PermanentToolTip);
            else
            {
                compactText = text.Substring(0, MaxLength - 3) + "...";
                writer.AddAttribute("title", text + (string.IsNullOrEmpty(PermanentToolTip) ? "" : " -> " + PermanentToolTip));
            }
            writer.AddAttribute("class", this.CssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(compactText);
            writer.RenderEndTag();

        }

        
        [DefaultValue("")]
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
            }
        }

        /// <summary>
        /// Minimum length is 4;
        /// zero means unlimited length
        /// </summary>
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
            }
        }
    }
}

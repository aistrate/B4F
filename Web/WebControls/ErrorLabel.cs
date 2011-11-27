using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace B4F.Web.WebControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ErrorLabel runat=server></{0}:ErrorLabel>")]
    public class ErrorLabel : Label
    {
        public ErrorLabel()
            : base()
        {
            ForeColor = Color.Red;
            Width = Unit.Pixel(850);
        }

        public override string Text
        {
            get
            {
                return (Visible ? base.Text : string.Empty);
            }
            set
            {
                base.Text = value.Trim();
                Visible = (base.Text != string.Empty);
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.Write((PrecedingNewline ? "<br />\n" : "") +
                         "<span class='padding' style='display:block'>");
            base.RenderBeginTag(writer);
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            base.RenderEndTag(writer);
            writer.Write(@"</span>");
        }

        [DefaultValue(typeof(Color), "Red"), Category("Appearance")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        [DefaultValue(typeof(Unit), "850px"), Category("Appearance")]
        public override Unit Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        [DefaultValue(typeof(bool), "true"), Category("Appearance")]
        public bool PrecedingNewline
        {
            get
            {
                object b = ViewState["PrecedingNewline"];
                return (b == null ? true : (bool)b);
            }
            set { ViewState["PrecedingNewline"] = value; }
        }
    }
}

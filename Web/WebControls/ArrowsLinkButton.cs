using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace B4F.Web.WebControls
{
    [ParseChildren(false)]
    [SupportsEventValidation]
    [DefaultProperty("Text")]
    [DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ControlBuilder(typeof(LinkButtonControlBuilder))]
    [DefaultEvent("Click")]
    [ToolboxData(@"<{0}:ArrowsLinkButton runat=""server"">ArrowsLinkButton</{0}:ArrowsLinkButton>")]
    [Designer("System.Web.UI.Design.WebControls.LinkButtonDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class ArrowsLinkButton : LinkButton
    {
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("&nbsp;»");
            base.RenderEndTag(writer);
        }
    }
}

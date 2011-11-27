using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace B4F.TotalGiro.GUI
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TradeableInstrument runat=server></{0}:TradeableInstrument>")]
    public class ccTradeableInstrument : WebControl, INamingContainer
    {

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            this.Controls.Add(new LiteralControl("<tr><td class='alignlabel'>"));
            Label lblBoe = new Label();
            lblBoe.Text = "Value";
            this.Controls.Add(lblBoe);
            this.Controls.Add(new LiteralControl("</td><td>"));
            TextBox box = new TextBox();
            box.Text = "0";
            this.Controls.Add(box);
            this.Controls.Add(new LiteralControl("</td></tr>"));
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Table;
            }
        }
    }
}

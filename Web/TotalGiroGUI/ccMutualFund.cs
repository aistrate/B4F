using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace B4F.TotalGiro.GUI
{
    [ToolboxData("<{0}:ccMutualFund runat=server></{0}:ccMutualFund>")]
    public class ccMutualFund : ccTradeableInstrument, INamingContainer
    {

        protected override void CreateChildControls()
        {
 //           this.Controls.Add(new LiteralControl("<table>"));
            base.CreateChildControls();
            this.Controls.Add(new LiteralControl("<tr><td class='alignlabel'>"));
            Label lblBla = new Label();
            lblBla.Text = "Bla";
            this.Controls.Add(lblBla);
            this.Controls.Add(new LiteralControl("</td><td>"));
            TextBox box = new TextBox();
            box.Text = "1";
            this.Controls.Add(box);
            this.Controls.Add(new LiteralControl("</td></tr></table>"));
        }

    }
}

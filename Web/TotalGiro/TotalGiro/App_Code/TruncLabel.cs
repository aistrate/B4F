using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for TruncLabel
/// </summary>
namespace Trunc
{
	public class TruncLabel : Label
	{

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			string strText = this.Text;
			int intLenCol = Convert.ToInt32(this.Width.Value);
			int intLenText = this.Text.Length;
			if (intLenText > intLenCol)
			{
				strText = strText.Substring(0, intLenCol) + "...";
				writer.AddAttribute("title", this.Text);
			}
			writer.AddAttribute("class", this.CssClass);
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.Write(strText);
			writer.RenderEndTag();

		}

	}
}

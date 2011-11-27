 using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;

public partial class UC_FirstPreviousPageBackButton : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Make variable page dependant
        string sessNamePathToPrevPage = Request.Url.AbsolutePath;
        if (Session[sessNamePathToPrevPage] == null)
        {
            if (Request.UrlReferrer == null)
            {
                btnBack.Enabled = false;
            }
            else
            {
                string refererPagePath = Request.UrlReferrer.AbsolutePath;
                StringBuilder sb = new StringBuilder();
                sb.Append("");

                if (Request.QueryString.Count > 0)
                    sb.Append("?" + Request.QueryString.ToString());
                
                if (!sessNamePathToPrevPage.Equals(refererPagePath))
                {
                    if (!sessNamePathToPrevPage.Equals(refererPagePath) && refererPagePath.ToUpper().IndexOf("LOGIN.ASPX") < 1)
                    {

                        Session[sessNamePathToPrevPage] = refererPagePath + sb.ToString();
                    }
                    else
                    {
                        btnBack.Enabled = false;
                    }
                }
            }
        }
    }

    
    protected void btnBack_Click(object sender, EventArgs e)
    {
        string sessNamePathToPrevPage = Request.Url.AbsolutePath;

        if (Session[sessNamePathToPrevPage] != null)
        {
            string path = "";
            path = Session[sessNamePathToPrevPage].ToString();
            Session[sessNamePathToPrevPage] = null;
            Response.Redirect(path);
        }
    }

    public string Caption
    {
        get
        {
            HtmlInputControl control = this.FindControl("btnBack") as HtmlInputControl;
            if (control != null)
                return control.Value;
            else
                return string.Empty;
        }
        set
        {
            HtmlInputControl control = this.FindControl("btnBack") as HtmlInputControl;
            if (control != null)
                control.Value = value;
        }
    }
}

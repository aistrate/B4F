using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class GLDSTDOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErrorMess.Visible = false;
        lblMess.Text = "";

        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Exported GLDSTD Overview";

                calStart.SelectedDate = DateTime.Today.AddDays(-10).Date;
                calEnd.SelectedDate = DateTime.Today.Date;

                gvGLDSTDFileOverview.Sort("CreationDate", SortDirection.Descending);
                gvGLDSTDFileOverview.SelectedIndex = -1;
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    protected void calStart_SelectionChanged(object sender, EventArgs e)
    {
        if (calStart.SelectedDate.CompareTo(calEnd.SelectedDate.Date) > 0)
        {
            calEnd.SelectedDate = calStart.SelectedDate.Date;
        }
        txtReference.Text = "";
    }


    protected void calEnd_SelectionChanged(object sender, EventArgs e)
    {
        if (calEnd.SelectedDate.CompareTo(calStart.SelectedDate.Date) < 0)
        {
            calStart.SelectedDate = calEnd.SelectedDate.Date;
        }
        txtReference.Text = "";
    }
    protected void gvFileOverview_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.RecordView.Visible = true;
    }
    protected void gvGLDSTDFileOverview_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.gvFileOverview.Visible = true;
    }
}

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

public partial class ExternalCastodianPositionEntry : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "External Custodian Position Entry";
        }
    }
    protected void btnCalendar_Click(object sender, EventArgs e)
    {
        DateTime date = DateTime.Now;

        if (txtDate.Text != String.Empty)
            date = DateTime.Parse(txtDate.Text);

        cldDate.SelectedDate = date;
        cldDate.Visible = true;
        //gvInstrumentPrices.EditIndex = -1;
        //gvInstrumentPrices.Visible = false;
    }
    protected void cldrDate_SelectionChanged(object sender, EventArgs e)
    {
        txtDate.Text = cldDate.SelectedDate.ToShortDateString();
        cldDate.Visible = false;
        //gvInstrumentPrices.Visible = true;
        
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {

        grdPositions.Visible = true;
        grdPositions.DataBind();
    }

    protected void odsPositions_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        decimal size = 0m;
        string errorMessage = "";

        try
        {
            size = decimal.Parse((string)e.InputParameters["Size"]);
            //if (size <= 0m)
            //    errorMessage = "Size must be greater than zero.";
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }

        if (errorMessage != "")
        {
            lblErrorMessage.Text = errorMessage;
            e.Cancel = true;
        }
        else
            e.InputParameters["Size"] = size;
    }

}

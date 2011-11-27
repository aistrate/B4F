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

using System.Xml;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.Dal;

public partial class Test_TestByMlim : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((EG)this.Master).setHeaderText = "DataSets as XML";
        if (!IsPostBack){ }
    }

    protected void btnClientPortfolio_Click(object sender, EventArgs e)
    {
        int accountId = 681;
        txtDumpToFile.Text = @"C:\Temp\Schemas\ClientPortfolioDataSet.xml";

        try
        {
            AccountDetailsView accountDetailsView = ClientPortfolioAdapter.GetAccountDetails(accountId);
             dumpDataSetToXmlFile(ClientPortfolioAdapter.GetPositions(accountId));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "ERROR: " + ex.Message;
        }

    }

    private void dumpDataSetToXmlFile(DataSet ds)
    {
        XmlTextWriter xtw = new XmlTextWriter(txtDumpToFile.Text, null);
        ds.WriteXml(xtw, XmlWriteMode.WriteSchema);
        xtw.Close();

        lblErrorMessage.Text = "Done!";
    }
}


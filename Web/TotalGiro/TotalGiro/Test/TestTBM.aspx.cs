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
using B4F.TotalGiro.Communicator.TBM;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;


public partial class Orders_TBM_TBMData : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ddlInstruments.DataSource = TBMDataAdapter.GetMutualFunds();
        ddlInstruments.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            txtExchangeID.Text = "AEX";
            txtInstrumentID.Text = "NL0000286706";
        }
    }


    protected void btnGetQuoteCollection_Click(object sender, EventArgs e)
    {
        TBMRequest tbmrequest = new TBMRequest();
        tbmrequest.TickerType = "ISIN";
        txtResults.Text = tbmrequest.getQuoteCollection();
    }

    protected void btnGetQuoteHistory_Click(object sender, EventArgs e)
    {
        TBMRequest tbmrequest = new TBMRequest();
        tbmrequest.TickerType = "ISIN";
        tbmrequest.updateQuoteHistory(txtInstrumentID.Text);
    }

    protected void btnUpdatePrices_Click(object sender, EventArgs e)
    {
        TBMRequest tbmrequest = new TBMRequest();
        DataSet ds = TBMDataAdapter.GetMutualFunds();

        foreach ( DataRow dr in ds.Tables[0].Rows )
        {
            string isin = "";

            if (dr.ItemArray[1].GetType().ToString() != "System.DBNull")
            {
                isin = (string)dr.ItemArray[1];

                tbmrequest.updateQuoteHistory(isin);
            }
        }
    }
    protected void btnCheckMissingHistPrices_Click(object sender, EventArgs e)
    {
        TBMRequest tbmrequest = new TBMRequest();
        tbmrequest.TickerType = "ISIN";
        tbmrequest.CheckMissingHistoricalPrices();

    }
}

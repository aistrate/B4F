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
using B4F.TotalGiro.Dal;

public partial class Test_TestFillPrices : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        IDalSession session = NHSessionFactory.CreateSession();

        Hashtable parameters = new Hashtable();
        parameters.Add("InstrumentID", 1001);
        session.ExecuteStoredProcedure("EXEC dbo.TG_FillHistPricesWeekendsHolidays @p_intInstrumentID = :InstrumentID", parameters);

        session.Close();

    }
}

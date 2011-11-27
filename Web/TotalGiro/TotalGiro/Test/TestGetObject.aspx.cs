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
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;

public partial class Test_TestGetObject : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        IDalSession session;
        session = NHSessionFactory.CreateSession();

        IContact vc = ContactMapper.GetContact(session, 1);
        session.Close();

    }
}

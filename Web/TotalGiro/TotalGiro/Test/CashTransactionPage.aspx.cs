using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;



public partial class CashTransactionPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = Page.Title;
        }
    }





    
}

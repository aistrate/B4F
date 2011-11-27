using System;

public partial class Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Test";
        }
    }
} 

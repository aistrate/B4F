﻿using System;

public partial class FinancialReportsHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((Help)Master).HeaderText = "Handleiding voor de financiele rapportage";
        }
    }
}
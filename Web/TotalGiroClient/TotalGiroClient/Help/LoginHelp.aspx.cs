﻿using System;

namespace B4F.TotalGiro.Client.Web.Help
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((Help)Master).HeaderText = "Handleiding voor het loginscherm";
            }
        }
    } 
}

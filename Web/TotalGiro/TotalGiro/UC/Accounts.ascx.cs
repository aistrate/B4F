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
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Security;


public partial class UC_Accounts : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e) { }

    public string setGvWidth
    {
        set
        {
            string strDef;
            if (value.IndexOf("px") > 0)
            { 
                strDef = value.Substring(0, value.IndexOf("px"));
            }
            else
            {
                strDef = value;
            }
            Unit width = new Unit(Convert.ToDouble(strDef), UnitType.Pixel);
            gvAccounts.Width = width; 
        }
    }

    protected void gvAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
        {
            LinkButton lbtnDetach = (LinkButton)e.Row.FindControl("lbtnDetach");
            lbtnDetach.Enabled = true;
        }

        //DropDownList ddlAccountHolders = (DropDownList)e.Row.FindControl("ddlAccountHolders");
        //IDalSession session = NHSessionFactory.CreateSession();

        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{

        //    int contactid = -1;
        //    if (Session["contactid"] != null)
        //        contactid = (int)Session["contactid"];

        //    int accountID = (int)gvAccounts.DataKeys[e.Row.RowIndex].Values[0];

        //    ddlAccountHolders.DataTextField = "Contact_CurrentNAW_Name";
        //    ddlAccountHolders.DataValueField = "Contact_Key";

        //    DataSet ds = ucAccountsEditAdapter.GetAccountAccountHolders(accountID);


        //    string val = ds.Tables[0].Rows[0]["IsPrimaryAccountHolder"].ToString();
        //    string val2 = ds.Tables[0].Rows[0]["Contact_CurrentNAW_Name"].ToString();
        //    ddlAccountHolders.DataSource = ds;
        //    ddlAccountHolders.DataBind();
        //}
        //session.Close();
    }

    protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string commName = e.CommandName.ToUpper();
        if (commName.Equals("EDIT"))
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int giroaccountkey = (int)gvAccounts.SelectedDataKey.Values[0];
                    Session["accountnrid"] = giroaccountkey;
                    Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
                }
            }
        }
        if (commName.Equals("DETACH"))
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int giroaccountKey = (int)gvAccounts.SelectedDataKey.Values[0];
                    int contactKey = (int)gvAccounts.SelectedDataKey.Values[1];
                    ucAccountsEditAdapter.DetachAccountHolder(giroaccountKey, contactKey);
                    gvAccounts.DataBind();
                }
            }
        }
    }

}

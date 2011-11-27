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
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class UC_ContactDetails : System.Web.UI.UserControl
{


    public enum InternetEnabledEnum
    {
        Unknown = -1,
        No = 0,
        Yes = 1
    }
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        rbInternet.Items.Add(new ListItem(InternetEnabledEnum.Yes.ToString(), InternetEnabledEnum.Yes.ToString()));
        rbInternet.Items.Add(new ListItem(InternetEnabledEnum.No.ToString(), InternetEnabledEnum.No.ToString()));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public string Email
    {
        get { return tbEmail.Text; }
        set { tbEmail.Text = value; }
    }

    public string Telephone
    {
        get { return tbTelephone.Text; }
        set { tbTelephone.Text = value; }
    }
    public string TelephoneAH
    {
        get { return tbTelephoneAH.Text; }
        set { tbTelephoneAH.Text = value; }
    }
    public string Mobile
    {
        get { return tbMobile.Text; }
        set { tbMobile.Text = value; }
    }
    public string Fax
    {
        get { return tbFax.Text; }
        set { tbFax.Text = value; }
    }

    public bool SendNewsItem
    {
        get { return chkSendNewsItem.Checked; }
        set { chkSendNewsItem.Checked = value; }
    }

    //public bool ddlRemisierEmployeeVisible
    //{
    //    get { return ddlRemisierEmployee.Visible;}
    //    set { ddlRemisierEmployee.Visible = value;}
    //}

    //public bool ddlRemisierVisible
    //{
    //    get { return ddlRemisier.Visible; }
    //    set { ddlRemisier.Visible = value; }
    //}

    //public string ddlRemisierID
    //{
    //    get { return ddlRemisier.SelectedValue; }
    //    set { ddlRemisier.SelectedValue = value; }
    //}

    //public string ddlRemisierEmployeeID
    //{
    //    get { return ddlRemisierEmployee.SelectedValue; }
    //    set { ddlRemisierEmployee.SelectedValue = value; }
    //}
   
    public string InternetEnabled
    {
        get { return rbInternet.SelectedValue; }
        set { 
                rbInternet.SelectedValue = value; 
            }
    }
    //protected void ddlRemisierEmployee_DataBound(object sender, EventArgs e)
    //{
    //    if (ddlRemisierEmployeeID != null && ddlRemisierEmployeeID.Length > 0)
    //        ddlRemisierEmployee.SelectedValue = ddlRemisierEmployeeID;
    //}

    protected void chkTelephoneNumberCheck_CheckedChanged(object sender, EventArgs e)
    {
        bool enabled = chkTelephoneNumberCheck.Checked;
        regexTelephone.Enabled = enabled;
        regexTelephoneAH.Enabled = enabled;
        regexFax.Enabled = enabled;
        regexMobile.Enabled = enabled;
    }
}

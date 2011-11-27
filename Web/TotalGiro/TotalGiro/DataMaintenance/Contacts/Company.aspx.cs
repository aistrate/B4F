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
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Security;

public partial class DataMaintenance_Company : System.Web.UI.Page
{

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        //ucFoundation.Expanded += new EventHandler(ucFoundation_Expanded);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            elbErrorMessage.Text = "";
            int contactid;
            if (!IsPostBack)
            {
                Utility.DisablePageCaching();
                Utility.AlertSaveMessage();
                ((EG)this.Master).setHeaderText = "View Company";
                if (Request.UrlReferrer != null)
                {
                    string refererPagePath = Request.UrlReferrer.AbsolutePath;
                    if (refererPagePath.ToUpper().IndexOf("PERSON.ASPX") > 0 && Session["oldcompid"] != null)
                    {
                        Session["contactid"] = Session["oldcompid"];
                        Session["oldcompid"] = null;
                    }
                }

                //    Session["contactid"] = 19412;

                if (Session["contactid"] != null)
                {
                    contactid = int.Parse(Session["contactid"].ToString());
                }
                else
                {
                    contactid = 0;
                }
                if (contactid > 0)
                {
                    loadRecord(contactid);
                }
                else if (contactid == 0)
                {
                    cbActive.Checked = true;
                }

                if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                {
                    bntSave.Enabled = true;
                    btnSaveBypass.Enabled = true;

                    if (contactid != 0)
                    {
                        btnAddCompanyToAccount.Enabled = true;
                        btnAttachCounterAccountToPerson.Enabled = true; 
                        btnAttachPersonToCompany.Enabled = true;
                        ((EG)this.Master).setHeaderText = "Edit Company";
                    }
                    else
                        ((EG)this.Master).setHeaderText = "Create New Company";
                }
            }
            else
                mess.Text = String.Empty;
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    private void loadRecord(int id)
    {
        if (id != 0)
        {
            CompanyDetails compDetails = CompanyEditAdapter.GetCompany(id);
            if (compDetails != null)
            {
                cbActive.Checked = Convert.ToBoolean(compDetails.IsActive);
                tbCompanyName.Text = compDetails.CompanyName;
                //ucContactDetails.ddlRemisierID = compDetails.Introducer;
                //ucContactDetails.ddlRemisierEmployeeID = compDetails.IntroducerEmployee;
                if (compDetails.DateOfFoundation != null && Util.IsNotNullDate(Convert.ToDateTime(compDetails.DateOfFoundation)))
                    ucFoundation.SelectedDate = Convert.ToDateTime(compDetails.DateOfFoundation);
                ucContactDetails.InternetEnabled = compDetails.InternetEnabled;
                tbKvKNumber.Text = compDetails.KvKNumber;
                ucContactDetails.Mobile = compDetails.Mobile;
                ucContactDetails.Fax = compDetails.Fax;
                ucContactDetails.Telephone = compDetails.Telephone;
                ucContactDetails.TelephoneAH = compDetails.TelephoneAH;
                ucContactDetails.Email = compDetails.Email;

                ucAddress.Street = compDetails.Street;
                ucAddress.HouseNumber = compDetails.HouseNumber;
                ucAddress.HouseNumberSuffix = compDetails.HouseNumberSuffix;
                ucAddress.PostCode = compDetails.Postalcode;
                ucAddress.City = compDetails.City;
                ucAddress.Country = compDetails.Country;

                ucAddress.PAStreet = compDetails.PostalStreet;
                ucAddress.PAHouseNumber = compDetails.PostalHouseNumber;
                ucAddress.PAHouseNumberSuffix = compDetails.PostalHouseNumberSuffix;
                ucAddress.PAPostCode = compDetails.PostalPostalcode;
                ucAddress.PACity = compDetails.PostalCity;
                ucAddress.PACountry = compDetails.PostalCountry;



                //if (compDetails.PostalPostalcode != null)
                //{
                //    if ((!compDetails.Street.Equals(compDetails.PostalStreet)) ||
                //            (!compDetails.PostalHouseNumber.Equals(compDetails.HouseNumber)) ||
                //            (!compDetails.PostalPostalcode.Equals(compDetails.Postalcode)))
                //    {
                //    }
                //}
            }
        }
    }

    protected void btnSaveCompany_Click(object sender, EventArgs e)
    {
        try
        {
            int compID = 0;
            bool valid = false;
            bool blnSaveSuccess = false;
            string mess = "";

            Button but = (Button)sender;
            string butID = but.ID;


            if (butID.ToUpper().Equals("BTNSAVEBYPASS"))
            {
                valid = this.checkBypassRequiredfields();
            }
            else
            {
                Page.Validate();
                valid = Page.IsValid;
            }

            if (Session["contactid"] != null)
                compID = (int)Session["contactid"];

            if (valid)
            {
                CompanyDetails compDetails = new CompanyDetails();
                compDetails.IsActive = Convert.ToString(cbActive.Checked);
                //compDetails.Introducer = ucContactDetails.ddlRemisierID;
                //compDetails.IntroducerEmployee = ucContactDetails.ddlRemisierEmployeeID;

                compDetails.CompanyName = tbCompanyName.Text;

                if (Util.IsNotNullDate(ucFoundation.SelectedDate) ||
                            ucFoundation.SelectedDate.Year > 1800)
                    compDetails.DateOfFoundation = Convert.ToString(ucFoundation.SelectedDate);
                else
                    compDetails.DateOfFoundation = string.Empty;

                compDetails.KvKNumber = tbKvKNumber.Text;

                compDetails.Email = ucContactDetails.Email;
                compDetails.Mobile = ucContactDetails.Mobile;
                compDetails.Fax = ucContactDetails.Fax;
                compDetails.Telephone = ucContactDetails.Telephone;
                compDetails.TelephoneAH = ucContactDetails.TelephoneAH;

                if (ucContactDetails.InternetEnabled.Length > 0)
                    compDetails.InternetEnabled = ucContactDetails.InternetEnabled;
                else
                    compDetails.InternetEnabled = InternetEnabled.Unknown.ToString();

                compDetails.Street = ucAddress.Street;
                compDetails.HouseNumber = ucAddress.HouseNumber;
                compDetails.HouseNumberSuffix = ucAddress.HouseNumberSuffix;
                compDetails.Postalcode = ucAddress.PostCode;
                compDetails.City = ucAddress.City;
                compDetails.Country = ucAddress.Country;

                if ((!ucAddress.Street.Equals(ucAddress.PAStreet)) ||
                    (!ucAddress.HouseNumber.Equals(ucAddress.PAHouseNumber)) ||
                    (!ucAddress.PostCode.Equals(ucAddress.PAPostCode)))
                {
                    compDetails.PostalStreet = ucAddress.PAStreet;
                    compDetails.PostalHouseNumber = ucAddress.PAHouseNumber;
                    compDetails.PostalHouseNumberSuffix = ucAddress.PAHouseNumberSuffix;
                    compDetails.PostalPostalcode = ucAddress.PAPostCode;
                    compDetails.PostalCity = ucAddress.PACity;
                    compDetails.PostalCountry = ucAddress.PACountry;
                }
                else
                {
                    compDetails.PostalStreet = ucAddress.Street;
                    compDetails.PostalHouseNumber = ucAddress.HouseNumber;
                    compDetails.PostalHouseNumberSuffix = ucAddress.HouseNumberSuffix;
                    compDetails.PostalPostalcode = ucAddress.PostCode;
                    compDetails.PostalCity = ucAddress.City;
                    compDetails.PostalCountry = ucAddress.Country;
                }

                CompanyEditAdapter.SaveCompany(ref compID, ref blnSaveSuccess, compDetails);
                Session["contactid"] = compID;
                Session["blnSaveSuccess"] = blnSaveSuccess.ToString();

                Response.Redirect("Company.aspx");
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void btnNewcompany_Click(object sender, EventArgs e)
    {
        Session["contactid"] = null;
        Session["accountnrid"] = null;
        Session["addcontactid"] = null;
        Response.Redirect("Company.aspx");
    }

    protected void btnAddNewAccount_Click(object sender, EventArgs e)
    {
        Session["accountnrid"] = null;
        Session["addcontactid"] = null;
        Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
    }

    protected void gvContactPerson_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            string commName = e.CommandName.ToUpper();
            if (commName.Equals("EDITCONTACTPERSON"))
            {
                if (e.CommandSource.GetType() == typeof(LinkButton))
                {
                    TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                    if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                    {
                        gvContactPerson.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                        int contactPersonKey = (int)gvContactPerson.SelectedDataKey.Value;

                        Session["oldcompid"] = Session["contactid"];
                        Session["contactid"] = Convert.ToString(contactPersonKey);
                        Response.Redirect("Person.aspx");
                    }
                }
            }
            else if (commName.Equals("DETACHCONTACTPERSON"))
            {
                if (e.CommandSource.GetType() == typeof(LinkButton))
                {
                    TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                    if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                    {
                        gvContactPerson.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                        int contactPersonKey = (int)gvContactPerson.SelectedDataKey.Value;
                        int companyID = (int)Session["contactid"];
                        CompanyEditAdapter.DetachContactperson(companyID, contactPersonKey);
                        gvContactPerson.DataBind();
                        
                    }
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    //protected void ucFoundation_Expanded(object sender, EventArgs e)
    //{
    //    //if (ucFoundation.IsExpanded)
    //    //{
    //    //    ucContactDetails.ddlRemisierVisible = false;
    //    //}
    //    //else
    //    //{
    //    //    ucContactDetails.ddlRemisierVisible = true;
    //    //}
    //}

    private bool checkBypassRequiredfields()
    {
        ValidatorCollection validators = null;
        bool isValid = true;
        validators = Page.Validators;
        foreach (IValidator validator in validators)
        {
            if (!(validator is RequiredFieldValidator) && !(validator is RangeValidator))
            {
                validator.Validate();
                if (!validator.IsValid)
                    isValid = false;
            }
        }
        return isValid;
    }

    protected void btnNewCompany_Click(object sender, EventArgs e)
    {
        Session["contactid"] = null;
        Response.Redirect("Company.aspx");
    }

    protected void gvContactPerson_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow && SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                LinkButton lbtnDetachContactperson = (LinkButton)e.Row.FindControl("lbtnDetachContactperson");
                lbtnDetachContactperson.Enabled = true;

            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void CompanyNameNotEmpty(object source, ServerValidateEventArgs value)
    {
        if (tbCompanyName.Text.Length == 0)
            value.IsValid = false;
    }

    protected void gvCounterAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            string commName = e.CommandName.ToUpper();
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvCounterAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int accID = (int)gvCounterAccounts.SelectedDataKey.Value;

                    if (commName.Equals("DETACHCONTACTPERSON"))
                    {
                        int companyID = (int)Session["contactid"];
                        CompanyEditAdapter.DetachCounterAccount(companyID, accID);
                        gvCounterAccounts.DataBind();
                    }
                    else if (commName.Equals("EDITCOUNTERACCOUNT"))
                    {
                        Session["counteraccountid"] = accID;
                        Response.Redirect("CounterAccount.aspx");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void gvCounterAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow && SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                LinkButton lbtnDetach = (LinkButton)e.Row.FindControl("lbtnDetachContactperson");
                lbtnDetach.Enabled = true;

                LinkButton lbtnEdit = (LinkButton)e.Row.FindControl("lbtnEditCounterAccount");
                if (lbtnEdit != null)
                    lbtnEdit.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }
}

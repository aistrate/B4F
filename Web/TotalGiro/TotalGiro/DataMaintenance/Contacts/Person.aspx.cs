using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Security;
using System.Text;

public partial class DataMaintenance_Person : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ucBirthDate.Expanded += new EventHandler(ucBirthDate_Expanded);
        ucBirthDate.SelectedDate = DateTime.Today.AddYears(-45);
        ucIdExpirationDate.Expanded += new EventHandler(ucIdExpirationDate_Expanded);
    }

    protected override void OnPreRender(EventArgs e)
    {
        try
        {
            bool contactPersChecked = cbContactPersonOnly.Checked;

            base.OnPreRender(e);

            if (contactPersChecked)
            {
                cpeAddress.Collapsed = true;
                cpeCounterAccount.Collapsed = true;
                cpeAccount.Collapsed = true;
            }
            cpeAddress.Enabled = !contactPersChecked;
            cpeCounterAccount.Enabled = !contactPersChecked;
            cpeAccount.Enabled = !contactPersChecked;
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            int contactid;
            elbErrorMessage.Text = "";
            if (!IsPostBack)
            {
                Utility.DisablePageCaching();
                Utility.AlertSaveMessage();
                ((EG)this.Master).setHeaderText = "View Person";

                bool? isCompanyContact = QueryStringModule.GetBValueFromQueryString(Request.RawUrl, "IsCompanyContact");
                IsCompanyContact = isCompanyContact.HasValue ? isCompanyContact.Value : false;
                CompanyID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "CompanyID");

                rbGender.Items.Add(new ListItem(Gender.Male.ToString(), Convert.ToString((int)Gender.Male)));
                rbGender.Items.Add(new ListItem(Gender.Female.ToString(), Convert.ToString((int)Gender.Female)));

                rbResidentStatus.Items.Add(new ListItem(ResidentStatus.Resident.ToString(), Convert.ToString((int)ResidentStatus.Resident)));
                rbResidentStatus.Items.Add(new ListItem(ResidentStatus.NonResident.ToString(), Convert.ToString((int)ResidentStatus.NonResident)));

                DataSet dsNat = PersonEditAdapter.GetNationalities();
                ddNationality.DataSource = dsNat;
                ddNationality.DataTextField = "Description";
                ddNationality.DataValueField = "Key";
                ddNationality.DataBind();

                DataSet dsIDType = PersonEditAdapter.GetIdentificationType();
                ddIDType.DataSource = dsIDType;
                ddIDType.DataTextField = "IdType";
                ddIDType.DataValueField = "Key";
                ddIDType.DataBind();

                cbActive.Checked = true;

                if (Request.UrlReferrer != null)
                {
                    string refererPagePath = Request.UrlReferrer.AbsolutePath;
                    if (refererPagePath.ToUpper().IndexOf("COMPANY.ASPX") > 0 && Session["oldpersid"] != null)
                    {
                        Session["contactid"] = Session["oldpersid"];
                        Session["oldpersid"] = null;
                    }
                }

                if (IsCompanyContact)
                {
                    cbContactPersonOnly.Checked = true;
                    cbContactPersonOnly_CheckedChanged(cbContactPersonOnly, null);
                }

                if (Session["contactid"] != null)
                {
                    contactid = (int)Session["contactid"];
                }
                else
                {
                    contactid = 0;
                }
                if (contactid > 0)
                {
                    loadRecord(contactid);
                }

                if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                {
                    bntSave.Enabled = true;
                    btnSaveBypass.Enabled = true;

                    if (contactid != 0)
                    {
                        btnAttachCounterAccountToPerson.Enabled = true;
                        btnAttachPersonToAccount.Enabled = true;
                        btnAttachCompanyToPerson.Enabled = true;
                        ((EG)this.Master).setHeaderText = "Edit Person";
                    }
                    else
                        ((EG)this.Master).setHeaderText = "Create New Person";
                }

            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    public int CompanyID
    {
        get
        {
            object i = ViewState["CompanyID"];
            return ((i == null) ? int.MinValue : (int)i);
        }
        set { ViewState["CompanyID"] = value; }
    }

    private bool IsCompanyContact
    {
        get
        {
            object i = ViewState["IsCompanyContact"];
            return ((i == null) ? false : (bool)i);
        }
        set { ViewState["IsCompanyContact"] = value; }
    }

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

    protected void bntSave_Click(object sender, EventArgs e)
    {
        try
        {
            int persID = 0;
            bool valid = false;
            bool blnSaveSuccess = false;
            String js = String.Empty;

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
                persID = (int)Session["contactid"];

            if (valid)
            {
                PersonDetails persDetails = new PersonDetails();
                persDetails.IsActive = Convert.ToString(cbActive.Checked);
                persDetails.HasMinimumData = cbContactPersonOnly.Checked;

                //persDetails.Introducer = ucContactDetails.ddlRemisierID;
                //persDetails.IntroducerEmployee = ucContactDetails.ddlRemisierEmployeeID;

                persDetails.LastName = tbLastName.Text;
                persDetails.Initials = tbInitials.Text;
                persDetails.MiddleName = tbMiddleName.Text;
                if (ucBirthDate.SelectedDate != DateTime.MinValue ||
                        ucBirthDate.SelectedDate.Year > 1800)
                    persDetails.BirthDate = Convert.ToString(ucBirthDate.SelectedDate);
                else
                    persDetails.BirthDate = string.Empty;

                persDetails.Gender = rbGender.SelectedValue;
                persDetails.ResidentialState = rbResidentStatus.SelectedValue;

                if (ucContactDetails.InternetEnabled.Length > 0)
                    persDetails.InternetEnabled = ucContactDetails.InternetEnabled;
                else
                    persDetails.InternetEnabled = InternetEnabled.Unknown.ToString();

                persDetails.Nationality = ddNationality.SelectedValue;
                persDetails.Title = tbTitle.Text;
                persDetails.IDType = ddIDType.SelectedValue;
                persDetails.IDNumber = tbIdentificationID.Text;
                if (ucIdExpirationDate.SelectedDate != DateTime.MinValue ||
                                    ucIdExpirationDate.SelectedDate.Year > 1800)
                    persDetails.IDExpirationDate = Convert.ToString(ucIdExpirationDate.SelectedDate);
                else
                    persDetails.IDExpirationDate = string.Empty;

                persDetails.BurgerServiceNummer = tbBurgerServiceNummer.Text;

                persDetails.Email = ucContactDetails.Email;
                persDetails.SendNewsItem = ucContactDetails.SendNewsItem;
                persDetails.Mobile = ucContactDetails.Mobile;
                persDetails.Fax = ucContactDetails.Fax;
                persDetails.Telephone = ucContactDetails.Telephone;
                persDetails.TelephoneAH = ucContactDetails.TelephoneAH;

                persDetails.Street = ucAddress.Street;
                persDetails.HouseNumber = ucAddress.HouseNumber;
                persDetails.HouseNumberSuffix = ucAddress.HouseNumberSuffix;
                persDetails.Postalcode = ucAddress.PostCode;
                persDetails.City = ucAddress.City;
                persDetails.Country = ucAddress.Country;

                if ((!ucAddress.Street.Equals(ucAddress.PAStreet)) ||
                    (!ucAddress.HouseNumber.Equals(ucAddress.PAHouseNumber)) ||
                    (!ucAddress.PostCode.Equals(ucAddress.PAPostCode)))
                {
                    persDetails.PostalStreet = ucAddress.PAStreet;
                    persDetails.PostalHouseNumber = ucAddress.PAHouseNumber;
                    persDetails.PostalHouseNumberSuffix = ucAddress.PAHouseNumberSuffix;
                    persDetails.PostalPostalcode = ucAddress.PAPostCode;
                    persDetails.PostalCity = ucAddress.PACity;
                    persDetails.PostalCountry = ucAddress.PACountry;
                }
                else
                {
                    persDetails.PostalStreet = ucAddress.Street;
                    persDetails.PostalHouseNumber = ucAddress.HouseNumber;
                    persDetails.PostalHouseNumberSuffix = ucAddress.HouseNumberSuffix;
                    persDetails.PostalPostalcode = ucAddress.PostCode;
                    persDetails.PostalCity = ucAddress.City;
                    persDetails.PostalCountry = ucAddress.Country;
                }

                PersonEditAdapter.SavePerson(ref persID, ref blnSaveSuccess, persDetails);
                Session["contactid"] = persID;
                Session["blnSaveSuccess"] = blnSaveSuccess.ToString();

                if (CompanyID > 0)
                    CompanyContactPersonEditAdapter.AddContactPerson(CompanyID, persID);

                Response.Redirect("Person.aspx");
            }
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
            PersonDetails persDetails = PersonEditAdapter.GetPerson(id);

            if (persDetails != null)
            {
                cbActive.Checked = Convert.ToBoolean(persDetails.IsActive);
                cbContactPersonOnly.Checked = persDetails.HasMinimumData;
                cbContactPersonOnly_CheckedChanged(cbContactPersonOnly, null);
                //ucContactDetails.ddlRemisierID = persDetails.Introducer;
                //ucContactDetails.ddlRemisierEmployeeID = persDetails.IntroducerEmployee;

                ucContactDetails.InternetEnabled = persDetails.InternetEnabled;

                tbLastName.Text = persDetails.LastName;
                tbInitials.Text = persDetails.Initials;
                tbMiddleName.Text = persDetails.MiddleName;
                if (persDetails.BirthDate != null && Util.IsNotNullDate(Convert.ToDateTime(persDetails.BirthDate)))
                {
                    ucBirthDate.SelectedDate = Convert.ToDateTime(persDetails.BirthDate);
                }
                rbGender.SelectedValue = persDetails.Gender;
                rbResidentStatus.SelectedValue = persDetails.ResidentialState;
                ddNationality.SelectedValue = persDetails.Nationality;
                tbTitle.Text = persDetails.Title;
                ddIDType.SelectedValue = persDetails.IDType;
                tbIdentificationID.Text = persDetails.IDNumber;
                if (persDetails.IDExpirationDate != null && Util.IsNotNullDate(Convert.ToDateTime(persDetails.IDExpirationDate)))
                {
                    ucIdExpirationDate.SelectedDate = Convert.ToDateTime(persDetails.IDExpirationDate);
                }

                tbBurgerServiceNummer.Text = persDetails.BurgerServiceNummer;
                ucContactDetails.Mobile = persDetails.Mobile;
                ucContactDetails.Fax = persDetails.Fax;
                ucContactDetails.Telephone = persDetails.Telephone;
                ucContactDetails.TelephoneAH = persDetails.TelephoneAH;
                ucContactDetails.Email = persDetails.Email;
                ucContactDetails.SendNewsItem = persDetails.SendNewsItem;

                ucAddress.Street = persDetails.Street;
                ucAddress.HouseNumber = persDetails.HouseNumber;
                ucAddress.HouseNumberSuffix = persDetails.HouseNumberSuffix;
                ucAddress.PostCode = persDetails.Postalcode;
                ucAddress.City = persDetails.City;
                ucAddress.Country = persDetails.Country;

                ucAddress.PAStreet = persDetails.PostalStreet;
                ucAddress.PAHouseNumber = persDetails.PostalHouseNumber;
                ucAddress.PAHouseNumberSuffix = persDetails.PostalHouseNumberSuffix;
                ucAddress.PAPostCode = persDetails.PostalPostalcode;
                ucAddress.PACity = persDetails.PostalCity;
                ucAddress.PACountry = persDetails.PostalCountry;



            }
        }

        
          //tbLastName.Text = "Veltheer";
          //tbInitials.Text = "D.J.";
          //ucBirthDate.SelectedDate = "13-9-1960";
          //rbGender.SelectedValue = Convert.ToString((int)Gender.Male);
          //ddNationality.SelectedValue = "1";
          //ddIDType.SelectedValue = "2";
          //tbIdentificationID.Text = "1";
          //tbSofiNummer.Text = "179218773";
        //ucContactDetails.Email = "dv@bits4finance.com";
          //ucContactDetails.Telephone = "020-6821778";
          //ucAddress.Street = "Buyskade";
          //ucAddress.HouseNumber = "78";
          //ucAddress.PostCode = "1018JC";
          //ucAddress.City = "Amsterdam";
          //ucAddress.Country = "21";
          //ucIdExpirationDate.Value = "20-9-2006";
          
    }

    protected void customValBurgerServiceNummer_ServerValidate(object source, ServerValidateEventArgs value)
    {
        value.IsValid = Util.PerformElfProefCheck(ElfProefCheckType.BSN, tbBurgerServiceNummer.Text);
    }

    protected void cvResidentStatus_ServerValidate(object source, ServerValidateEventArgs value)
    {
        bool isValid = true;
        if (rbResidentStatus.SelectedIndex > -1)
        {
            ResidentStatus status = (ResidentStatus)Convert.ToInt32(rbResidentStatus.SelectedValue);
            if (status == ResidentStatus.Resident && string.IsNullOrEmpty(tbBurgerServiceNummer.Text))
                isValid = false;
        }
        value.IsValid = isValid;
    }

    protected void btnAddNewAccount_Click(object sender, EventArgs e)
    {
        Session["accountnrid"] = null;
        Session["addcontactid"] = null;
        Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
    }

    protected void cbContactPersonOnly_CheckedChanged(object sender, EventArgs e)
    {
        bool isCPOnly = cbContactPersonOnly.Checked;
        pnlPersonAsAccountHolderDetails.Visible = !isCPOnly;
        pnlAddressHeader.Visible = !isCPOnly;
        pnlAddress.Visible = !isCPOnly;
        pnlAccountHeader.Visible = !isCPOnly;
        pnlAccount.Visible = !isCPOnly;
        pnlCounterAccount.Visible = !isCPOnly;
        pnlCounterAccountHeader.Visible = !isCPOnly;
    }

    protected void LastNameNotEmpty(object source, ServerValidateEventArgs value)
    {
        if (tbLastName.Text.Length == 0)
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
                        int contactPersonKey = (int)Session["contactid"];
                        ContactPersonEditAdapter.DetachCounterAccount(contactPersonKey, accID);
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

    protected void gvContactPersonCompanies_RowDataBound(object sender, GridViewRowEventArgs e)
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

    protected void gvContactPersonCompanies_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            string commName = e.CommandName.ToUpper();
            if (commName.Equals("EDITCOMPANY"))
            {
                if (e.CommandSource.GetType() == typeof(LinkButton))
                {
                    TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                    if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                    {
                        gvContactPersonCompanies.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                        // Change contactid
                        int companyID = (int)gvContactPersonCompanies.SelectedDataKey.Value;
                        Session["oldpersid"] = Session["contactid"];
                        Session["contactid"] = Convert.ToString(companyID);
                        Response.Redirect("Company.aspx");
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
                        gvContactPersonCompanies.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                        int companyID = (int)gvContactPersonCompanies.SelectedDataKey.Value;
                        int contactPersonKey = (int)Session["contactid"];
                        CompanyEditAdapter.DetachContactperson(companyID, contactPersonKey);
                        gvContactPersonCompanies.DataBind();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void ucBirthDate_Expanded(object sender, EventArgs e)
    {
        if (ucBirthDate.IsExpanded)
        {
            ddNationality.Visible = false;
            //ucContactDetails.ddlRemisierEmployeeVisible = false;
        }
        else
        {
            ddNationality.Visible = true;
            //ucContactDetails.ddlRemisierEmployeeVisible = true;
        }
    }

    protected void ucIdExpirationDate_Expanded(object sender, EventArgs e)
    {
        if (ucIdExpirationDate.IsExpanded)
        {
            //ucContactDetails.ddlRemisierVisible = false;
        }
        else
        {
            //ucContactDetails.ddlRemisierVisible = true;
        }
    }
}
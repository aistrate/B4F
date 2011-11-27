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
using B4F.TotalGiro.ApplicationLayer;
using B4F.TotalGiro.ApplicationLayer.Remisers;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.StaticData;

public partial class DataMaintenance_Remisier_Remisier : System.Web.UI.Page
{
    public int RemisierID
    {
        get
        {
            object b = ViewState["RemisierID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["RemisierID"] = value;
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Remisier Maintenance";
            rbCPGender.Items.Add(new ListItem(Gender.Male.ToString(), Convert.ToString((int)Gender.Male)));
            rbCPGender.Items.Add(new ListItem(Gender.Female.ToString(), Convert.ToString((int)Gender.Female)));

            try
            {
                if (Session["RemisierID"] != null)
                {
                    RemisierID = (int)Session["RemisierID"];
                    Session["RemisierID"] = null;
                }

                DataBind();
                showRemiserDetails();
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }
        else
            elbErrorMessage.Text = "";
    }

    public bool IsLoggedInAsAssetManager
    {
        get
        {
            object b = ViewState["IsLoggedInAsAssetManager"];
            return ((b == null) ? RemisierAdapter.IsLoggedInAsAssetManager() : (bool)b);
        }
    }

    public override void DataBind()
    {
        int managementCompanyID = 0;
        string companyName = "";

        if (!IsLoggedInAsAssetManager && RemisierID == 0)
        {
            mvwAssetManager.ActiveViewIndex = 1;
            ddlAssetManager.DataBind();
            ddlAssetManager.SelectedIndex = -1;
        }
        else
        {
            mvwAssetManager.ActiveViewIndex = 0;
            if (RemisierID == 0)
            {
                RemisierAdapter.GetCurrentManagmentCompany(ref companyName, ref managementCompanyID);
                hfAssetMAnagerID.Value = Convert.ToString(managementCompanyID);
                lblAssetManager.Text = companyName;
            }
        }
        base.DataBind();
    }

    protected void ddlParentRemisier_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bool enabled = (ddlParentRemisier.SelectedValue != int.MinValue.ToString());
            dbParentRemisierKickBackPercentage.Enabled = enabled;
            if (!enabled)
                dbParentRemisierKickBackPercentage.Clear();
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void rblBankChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlBank.SelectedValue = int.MinValue.ToString();
        this.mvwBank.ActiveViewIndex = this.rblBankChoice.SelectedIndex;
    }

    private void clearRemiserDetails()
    {
        lblRemisierName.Text = "Name";
        tbRemisierName.Text = string.Empty;
        tbInternalRef.Text = string.Empty;
        ddlBank.SelectedValue = int.MinValue.ToString();
        tbBankName.Text = string.Empty;
        tbTegenrekNR.Text = string.Empty;
        tbTegenrekTNV.Text = string.Empty;
        tbBankCity.Text = string.Empty;
        tbTelephone.Text = string.Empty;
        tbFax.Text = string.Empty;
        tbEmail.Text = string.Empty;

        ddlParentRemisier.SelectedValue = int.MinValue.ToString();
        dbParentRemisierKickBackPercentage.Clear();
        dbParentRemisierKickBackPercentage.Enabled = false;

        ucAddress.Reset();

        tbCPLastName.Text = string.Empty;
        tbCPInitials.Text = string.Empty;
        tbCPMiddleName.Text = string.Empty;
        tbCPTelephone.Text = string.Empty;
        tbCPEmail.Text = string.Empty;
        rbCPGender.ClearSelection();

        rblBankChoice.SelectedIndex = 0;
        mvwBank.ActiveViewIndex = 0;

        tbDatumOvereenkomst.Text = string.Empty;
        tbNummerOvereenkomst.Text = string.Empty;
        tbNummerAFM.Text = string.Empty;
        tbNummerKasbank.Text = string.Empty;
    }

    private void showRemiserDetails()
    {
        try
        {
            if (RemisierID != 0)
            {
                RemisierDetailsView remisierDetailsView = RemisierAdapter.GetRemisierDetails(RemisierID);

                hfAssetMAnagerID.Value = remisierDetailsView.CompanyID.ToString();
                lblAssetManager.Text = remisierDetailsView.CompanyName;
                ddlRemisierType.SelectedValue = ((int)remisierDetailsView.RemisierType).ToString();
                ddlRemisierType.Enabled = false;
                //lblRemisierName.Text = remisierDetailsView.RemisierType.ToString() + " Name";
                tbRemisierName.Text = remisierDetailsView.Name;
                tbInternalRef.Text = remisierDetailsView.InternalRef;

                if (remisierDetailsView.BankID != int.MinValue)
                    ddlBank.SelectedValue = remisierDetailsView.BankID.ToString();
                else if (!string.IsNullOrEmpty(remisierDetailsView.BankName))
                {
                    tbBankName.Text = remisierDetailsView.BankName;
                    rblBankChoice.SelectedIndex = 1;
                    mvwBank.ActiveViewIndex = 1;
                }
                tbTegenrekNR.Text = remisierDetailsView.BankAccountNumber;
                tbTegenrekTNV.Text = remisierDetailsView.BankAccountName;
                tbBankCity.Text = remisierDetailsView.BankCity;
                tbTelephone.Text = remisierDetailsView.Telephone;
                tbFax.Text = remisierDetailsView.Fax;
                tbEmail.Text = remisierDetailsView.Email;

                if (remisierDetailsView.ParentRemisierKey.HasValue)
                {
                    ddlParentRemisier.SelectedValue = remisierDetailsView.ParentRemisierKey.Value.ToString();
                    dbParentRemisierKickBackPercentage.Enabled = true;
                }
                if (remisierDetailsView.ParentRemisierKickBackPercentage != 0)
                    dbParentRemisierKickBackPercentage.Value = remisierDetailsView.ParentRemisierKickBackPercentage;

                ucAddress.MainAddress = remisierDetailsView.OfficeAddress;
                ucAddress.PostAddress = remisierDetailsView.PostalAddress;

                if (remisierDetailsView.ContactPerson != null)
                {
                    tbCPLastName.Text = remisierDetailsView.ContactPerson.LastName;
                    tbCPInitials.Text = remisierDetailsView.ContactPerson.Initials;
                    tbCPMiddleName.Text = remisierDetailsView.ContactPerson.MiddleName;
                    if (remisierDetailsView.ContactPerson.Telephone != null)
                        tbCPTelephone.Text = remisierDetailsView.ContactPerson.Telephone.Number;
                    tbCPEmail.Text = remisierDetailsView.ContactPerson.Email;
                    if (remisierDetailsView.ContactPerson.Gender != 0)
                        rbCPGender.SelectedValue = ((int)remisierDetailsView.ContactPerson.Gender).ToString();
                }

                tbDatumOvereenkomst.Text = remisierDetailsView.DatumOvereenkomst;
                tbNummerOvereenkomst.Text = remisierDetailsView.NummerOvereenkomst;
                tbNummerAFM.Text = remisierDetailsView.NummerAFM;
                tbNummerKasbank.Text = remisierDetailsView.NummerKasbank;

                if (!remisierDetailsView.IsActive)
                {
                    chkIsActive.Visible = true;
                    chkIsActive.Checked = false;
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSaveRemisier_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                RemisierDetailsView details = new RemisierDetailsView();

                if (RemisierID != 0)
                    details.RemisierKey = RemisierID;
                if (mvwAssetManager.ActiveViewIndex == 1)
                    details.CompanyID = Utility.GetKeyFromDropDownList(ddlAssetManager);
                details.RemisierType = (RemisierTypes)Utility.GetKeyFromDropDownList(ddlRemisierType);
                details.Name = tbRemisierName.Text;
                details.InternalRef = tbInternalRef.Text;

                if (mvwBank.ActiveViewIndex == 0)
                    details.BankID = Utility.GetKeyFromDropDownList(ddlBank);
                else
                    details.BankName = tbBankName.Text;

                details.BankAccountNumber = tbTegenrekNR.Text;
                details.BankAccountName  = tbTegenrekTNV.Text;
                details.BankCity = tbBankCity.Text;
                details.Telephone = tbTelephone.Text;
                details.Fax = tbFax.Text;
                details.Email = tbEmail.Text;

                if (ddlParentRemisier.SelectedValue != int.MinValue.ToString())
                    details.ParentRemisierKey = Utility.GetKeyFromDropDownList(ddlParentRemisier);
                if (!dbParentRemisierKickBackPercentage.IsEmpty)
                    details.ParentRemisierKickBackPercentage = dbParentRemisierKickBackPercentage.Value;


                details.OfficeAddress = ucAddress.MainAddress;
                details.PostalAddress = ucAddress.PostAddress;

                details.DatumOvereenkomst = tbDatumOvereenkomst.Text;
                details.NummerOvereenkomst = tbNummerOvereenkomst.Text;
                details.NummerAFM = tbNummerAFM.Text;
                details.NummerKasbank = tbNummerKasbank.Text;

                if (!string.IsNullOrEmpty(tbCPLastName.Text) || !string.IsNullOrEmpty(tbCPInitials.Text) ||
                    !string.IsNullOrEmpty(tbCPMiddleName.Text) || !string.IsNullOrEmpty(tbCPTelephone.Text) ||
                    !string.IsNullOrEmpty(tbCPEmail.Text))
                {
                    Person p = new Person();
                    p.LastName = tbCPLastName.Text;
                    p.Initials = tbCPInitials.Text;
                    p.MiddleName = tbCPMiddleName.Text;
                    p.Telephone = new TelephoneNumber(tbCPTelephone.Text);
                    p.Email = tbCPEmail.Text;
                    if (rbCPGender.SelectedIndex > -1)
                        p.Gender = (Gender)Convert.ToInt32(rbCPGender.SelectedValue);
                    details.ContactPerson = p;
                }

                details.IsActive = (chkIsActive.Visible ? chkIsActive.Checked : true);

                int key;
                if (RemisierAdapter.SaveRemisier(details, out key))
                {
                    RemisierID = key;
                    showRemiserDetails();
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

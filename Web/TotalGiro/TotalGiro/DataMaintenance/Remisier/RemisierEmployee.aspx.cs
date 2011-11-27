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
using System.Collections.Specialized;

public partial class DataMaintenance_Remisier_RemisierEmployee : System.Web.UI.Page
{
    public int RemisierEmployeeID
    {
        get
        {
            object b = ViewState["RemisierEmployeeID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["RemisierEmployeeID"] = value;
        }
    }

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
            ((EG)this.Master).setHeaderText = "Remisier Employee Maintenance";
            rbGender.Items.Add(new ListItem(Gender.Male.ToString(), Convert.ToString((int)Gender.Male)));
            rbGender.Items.Add(new ListItem(Gender.Female.ToString(), Convert.ToString((int)Gender.Female)));

            try
            {
                RemisierEmployeeID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "RemisierEmployeeID");
                if (RemisierEmployeeID != 0)
                    showRemisierEmployeeDetails();

                RemisierID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "RemisierID");
                if (RemisierID != 0)
                    lblRemisierName.Text = "Remisier: " + RemisierAdapter.GetRemisierName(RemisierID);
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }
        else
            elbErrorMessage.Text = "";
    }


    private void showRemisierEmployeeDetails()
    {
        try
        {
            if (RemisierEmployeeID != 0)
            {
                RemisierEmployeeDetailsView details = RemisierAdapter.GetRemisierEmployeeDetails(RemisierEmployeeID);

                if (details != null)
                {
                    tbTitle.Text = details.Title;
                    tbLastName.Text = details.LastName;
                    tbInitials.Text = details.Initials;
                    tbMiddleName.Text = details.MiddleName;
                    tbTelephone.Text = details.Telephone;
                    tbTelephoneAH.Text = details.TelephoneAH;
                    tbMobile.Text = details.Telephone;
                    tbEmail.Text = details.Email;
                    if (details.Gender != Gender.Unknown)
                        rbGender.SelectedValue = ((int)details.Gender).ToString();
                    ddlRole.SelectedValue = ((int)details.Role).ToString();
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                RemisierEmployeeDetailsView details = new RemisierEmployeeDetailsView();

                if (RemisierEmployeeID != 0)
                    details.EmployeeID = RemisierEmployeeID;

                if (RemisierID != 0)
                    details.RemisierID = RemisierID;
                else
                    throw new ApplicationException("Unknown Remisier");

                details.Title = tbTitle.Text;
                details.LastName = tbLastName.Text;
                details.Initials = tbInitials.Text;
                details.MiddleName = tbMiddleName.Text;
                details.Telephone = tbTelephone.Text;
                details.TelephoneAH = tbTelephoneAH.Text;
                details.Mobile = tbMobile.Text;
                details.Email = tbEmail.Text;
                details.Role = (EmployeeRoles)Utility.GetKeyFromDropDownList(ddlRole);
                if (rbGender.SelectedIndex > -1)
                    details.Gender = (Gender)Convert.ToInt32(rbGender.SelectedValue);

                int key;
                if (RemisierAdapter.SaveRemisierEmployee(details, out key))
                {
                    RemisierEmployeeID = key;
                    showRemisierEmployeeDetails();
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

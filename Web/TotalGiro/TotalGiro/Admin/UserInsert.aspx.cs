using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Admin;
using B4F.TotalGiro.Stichting.Login;

public partial class UserInsert : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Create New User";
            wizCreateUser.Focus();
        }

        elbErrorMessage.Text = "";
    }

    protected void wizCreateUser_ActiveStepChanged(object sender, EventArgs e)
    {
        try
        {
            if (wizCreateUser.ActiveStepIndex == 1)
            {
                Password = wizCreateUser.Password;

                if (UserInsertAdapter.LoginExists(wizCreateUser.UserName))
                {
                    elbErrorMessage.Text = string.Format(
                        "Login already exists for User Name '{0}'.<br />Step 2: 'Set Login Type' has been skipped.",
                        wizCreateUser.UserName);

                    wizCreateUser.ActiveStepIndex = 2;
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    // triggered by button 'Activate' on the last step of the wizard
    protected void wizCreateUser_ContinueButtonClick(object sender, EventArgs e)
    {
        try
        {
            UserInsertAdapter.ActivateUser(wizCreateUser.UserName);

            if (Password != "")
                UserOverviewAdapter.SendPasswordEmail(wizCreateUser.UserName,
                                                      Password,
                                                      PasswordEmailType.FirstTime);
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected string Password
    {
        get
        {
            object s = Session["Password"];
            return s == null ? "" : (string)s;
        }
        set { Session["Password"] = value; }
    }

    protected void btnSetLoginType_Click(object sender, EventArgs e)
    {
        try
        {
            if (!UserInsertAdapter.LoginExists(wizCreateUser.UserName))
            {
                RadioButtonList radioButtonList = (RadioButtonList)findControl("rblLoginType");

                LoginTypes loginType =
                    radioButtonList.SelectedValue == "Asset Manager" ? LoginTypes.AssetManagerEmployee :
                    radioButtonList.SelectedValue == "Stichting"     ? LoginTypes.StichtingEmployee    :
                    radioButtonList.SelectedValue == "Compliance"    ? LoginTypes.ComplianceEmployee   :
                                                                       0;

                int managementCompanyId =
                    loginType == LoginTypes.AssetManagerEmployee ?
                        int.Parse(((DropDownList)findControl("ddlAssetManager")).SelectedValue) :
                        UserInsertAdapter.GetEffectenGiroId();

                UserInsertAdapter.SetLoginType(wizCreateUser.UserName, loginType, managementCompanyId);
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void rblLoginType_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool assetManagerEnabled = ((RadioButtonList)sender).SelectedValue == "Asset Manager";

        findControl("lblAssetManager").Visible = assetManagerEnabled;
        findControl("ddlAssetManager").Visible = assetManagerEnabled;
        findControl("valAssetManagerRequired").Visible = assetManagerEnabled;
    }

    private Control findControl(string id)
    {
        if (wizardStepTwo == null)
            wizardStepTwo = (TemplatedWizardStep)wizCreateUser.WizardSteps[1];

        return wizardStepTwo.Controls[0].FindControl(id);
    }

    private TemplatedWizardStep wizardStepTwo = null;
}

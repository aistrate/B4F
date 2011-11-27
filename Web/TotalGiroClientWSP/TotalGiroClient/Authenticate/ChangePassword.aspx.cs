using System;
using B4F.TotalGiro.ClientApplicationLayer.Authenticate;

public partial class ChangePassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ChangePasswordAdapter.AssertIsLoggedIn();

        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Wachtwoord wijzigen";
            ((TotalGiroClient)Master).HelpUrl = "~/Help/ChangePasswordHelp.aspx";

            ctlChangePassword.Focus();
        }

        elbErrorMessage.Text = "";
    }

    protected void ctlChangePassword_ChangedPassword(object sender, EventArgs e)
    {
        try
        {
            bool emailSent = ChangePasswordAdapter.SendPasswordEmail(ctlChangePassword.NewPassword);

            ctlChangePassword.SuccessText = (emailSent ?
                "U hebt uw wachtwoord gewijzigd. Deze is naar uw <span>e-mailadres</span> gestuurd." :
                "Uw wachtwoord is gewijzigd.");
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = "There was an error while sending the confirmation e-mail: <br />" +
                                   Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

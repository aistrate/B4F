using System;
using B4F.TotalGiro.ApplicationLayer.Admin;

public partial class ChangePassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Change Password";
            ctlChangePassword.Focus();
        }

        elbErrorMessage.Text = "";
    }

    protected void ctlChangePassword_ChangedPassword(object sender, EventArgs e)
    {
        try
        {
            UserOverviewAdapter.SendPasswordEmail(ctlChangePassword.UserName,
                                                  ctlChangePassword.NewPassword,
                                                  PasswordEmailType.ChangedByUser);

            elbErrorMessage.Text = "An e-mail notification was sent, containing your new password.";
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = string.Format("Error sending e-mail notification: {0}",
                                                 Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void ctlChangePassword_CancelButtonClick(object sender, EventArgs e)
    {
        Response.Redirect("~/Default.aspx");
    }
}

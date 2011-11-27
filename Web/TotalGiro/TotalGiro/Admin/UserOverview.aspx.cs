using System;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Admin;

public partial class UserOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Users Overview";
        }

        elbErrorMessage.Text = "";
    }

    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvUsers.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                string userName = (string)gvUsers.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "EDITUSER":
                        Session["UserName"] = userName;
                        gvUsers.SelectedIndex = -1;
                        Response.Redirect("UserEdit.aspx");
                        break;

                    case "DELETEUSER":
                        Membership.DeleteUser(userName);
                        gvUsers.DataBind();
                        break;

                    case "RESETPASSWORD":
                        resetPassword(userName);
                        break;
                }
            }
        }

        gvUsers.SelectedIndex = -1;
    }

    private void resetPassword(string userName)
    {
        string successMessage = string.Format("Password was reset for user '{0}'.", userName);

        try
        {
            if (userName != "")
            {
                UserOverviewAdapter.ResetPassword(userName);

                elbErrorMessage.Text = string.Format(
                    "{0}<br/><br/>" +
                    "An e-mail notification was sent, informing the user of their new password.",
                    successMessage);
            }
        }
        catch (SmtpException ex)
        {
            elbErrorMessage.Text = string.Format(
                "{0}<br/><br/>" +
                "Error sending e-mail notification: {1}",
                successMessage,
                Utility.GetCompleteExceptionMessage(ex));
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = string.Format(
                "Error resetting password for user '{0}': {1}",
                userName,
                Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("UserInsert.aspx");
    }
}

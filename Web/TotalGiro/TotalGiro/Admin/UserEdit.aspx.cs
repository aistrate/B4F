using System;
using System.Linq;
using System.Net.Mail;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Admin;

public partial class UserEdit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Edit User";
            loadUser();
            txtEmail.Focus();
        }

        elbErrorMessage.Text = "";

        loadRoles(IsPostBack);
    }

    private void loadUser()
    {
        string userName = (string)Session["UserName"];
        string email, comment;
        bool isApproved;

        UserEditAdapter.GetUserData(userName, out email, out comment, out isApproved);

        lblUserName.Text = userName;
        txtEmail.Text = email;
        txtDescription.Text = comment;
        cbActive.Checked = isApproved;
    }

    private void loadRoles(bool isPostBack)
    {
        string userName = (string)Session["UserName"];
        string[] allRoles;
        bool[] activeRoles;

        UserEditAdapter.GetUserRoles(userName, out allRoles, out activeRoles);
        
        for (int i = 0; i < allRoles.Length; i++)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Text = allRoles[i];
            pnlRoles.Controls.Add(checkBox);

            if (!isPostBack && activeRoles[i])
                checkBox.Checked = true;

            Label label = new Label();
            label.Text = "<br/>";
            pnlRoles.Controls.Add(label);
        }
    }
    
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string userName = (string)Session["UserName"];

            UserEditAdapter.SaveUser(userName, txtEmail.Text.Trim(), txtDescription.Text, cbActive.Checked);

            saveRoles();

            Response.Redirect("UserOverview.aspx");
        }
        catch (SmtpException ex)
        {
            elbErrorMessage.Text = "[User updated successfully.]<br />" + Utility.GetCompleteExceptionMessage(ex);
            btnSave.Focus();
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            btnSave.Focus();
        }
    }

    private void saveRoles()
    {
        string userName = (string)Session["UserName"];
        string[] selectedRoles = pnlRoles.Controls.OfType<CheckBox>().Where(c => c.Checked)
                                                                     .Select(c => c.Text).ToArray();

        UserEditAdapter.SaveUserRoles(userName, selectedRoles);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Session["UserName"] = null;
        Response.Redirect("UserOverview.aspx");
    }

    protected void btnResetPassword_Click(object sender, EventArgs e)
    {

    }
}

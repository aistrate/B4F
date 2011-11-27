using System;
using System.Data;
using System.Net.Mail;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Admin;

public partial class RoleOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Roles Overview";
            mvwEditRole.SetActiveView(vwNoEdit);
        }

        elbErrorMessage.Text = "";
    }

    protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvRoles.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                string roleName = (string)gvRoles.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "EDITROLE":
                        gvRoles.Enabled = false;
                        mvwEditRole.SetActiveView(vwModify);
                        gvUsersInRole.DataBind();
                        gvUsersInRole.Caption = string.Format("Users In Role: '{0}'", roleName);
                        if (gvUsersInRole.PageCount > 0)
                            gvUsersInRole.PageIndex = 0;
                        btnCancelModify.Focus();
                        break;

                    case "DELETEROLE":
                        try
                        {
                            RoleOverviewAdapter.DeleteRole(roleName);
                            gvRoles.DataBind();
                        }
                        catch (Exception ex)
                        {
                            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
                        }
                        gvRoles.SelectedIndex = -1;
                        break;

                    default:
                        setModeToNoEdit();
                        gvRoles.SelectedIndex = -1;
                        break;
                }
            }
        }
    }

    protected void gvUsersInRole_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        btnCancelModify.Focus();
    }
    
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        gvRoles.Enabled = false;
        mvwEditRole.SetActiveView(vwAdd);
        txtRoleName.Focus();
    }

    protected void btnSaveAdd_Click(object sender, EventArgs e)
    {
        try
        {
            RoleOverviewAdapter.CreateRole(txtRoleName.Text);
            gvRoles.DataBind();
            setModeToNoEdit();
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSaveModify_Click(object sender, EventArgs e)
    {
        try
        {
            saveCheckBoxesToDataSet();

            string roleName = (string)gvRoles.SelectedValue;

            // selected users
            DataSet ds = (DataSet)Session["dsUsersInRole"];
            DataRow[] selectedRows = ds.Tables[0].Select("IsInRole = true");

            int i = 0;
            string[] selectedUsers = new string[selectedRows.Length];
            foreach (DataRow dr in selectedRows)
                selectedUsers[i++] = dr.ItemArray.GetValue(0).ToString();

            try
            {
                RoleOverviewAdapter.SaveRole(roleName, selectedUsers);
            }
            catch (SmtpException ex)
            {
                elbErrorMessage.Text = "[Users in role updated successfully.]<br />" + Utility.GetCompleteExceptionMessage(ex);
            }
            
            setModeToNoEdit();
            gvRoles.DataBind();
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            btnSaveModify.Focus();
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        setModeToNoEdit();
    }

    private void setModeToNoEdit()
    {
        gvRoles.Enabled = true;
        txtRoleName.Text = "";
        Session["dsUsersInRole"] = null;
        mvwEditRole.SetActiveView(vwNoEdit);
    }

    private void saveCheckBoxesToDataSet()
    {
        DataSet ds = (DataSet)Session["dsUsersInRole"];
        
        if (ds != null)
            foreach (GridViewRow row in gvUsersInRole.Rows)
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox cbIsInRole = (CheckBox)row.FindControl("cbIsInRole");
                    string userName = (string)gvUsersInRole.DataKeys[row.RowIndex].Value;

                    DataRow[] selectedRows = ds.Tables[0].Select(string.Format("UserName = '{0}'", userName));
                    if (selectedRows.Length == 1)
                        selectedRows[0]["IsInRole"] = cbIsInRole.Checked;
                }
    }

    protected void gvUsersInRole_Sorting(object sender, GridViewSortEventArgs e)
    {
        saveCheckBoxesToDataSet();
    }

    protected void gvUsersInRole_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        saveCheckBoxesToDataSet();
    }
}

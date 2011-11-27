using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using System.Data;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class LifecycleMaintenance : System.Web.UI.Page
{
    private bool isLoggedInAsStichting = AccountFinderAdapter.IsLoggedInAsStichting();
    
    public int LifecycleID
    {
        get
        {
            object b = ViewState["LifecycleID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["LifecycleID"] = value;
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Lifecycle Maintenance";
                if (IsLifecycleInsert)
                    gvLifecycles.Sort("Name", SortDirection.Ascending);
                else
                    gvLifecycles.Sort("Key", SortDirection.Descending);

                btnCreateNewLifecycle.Enabled = !isLoggedInAsStichting;
                btnCreateNewLifecycleLine.Enabled = !isLoggedInAsStichting;
            }
            Utility.EnableScrollToBottom(this, hdnScrollToBottom);
            lblErrorMessage.Text = "";
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected bool IsLifecycleInsert
    {
        get
        {
            bool b;
            return (hdnIsLifecycleInsert.Value != string.Empty && bool.TryParse(hdnIsLifecycleInsert.Value, out b) ? bool.Parse(hdnIsLifecycleInsert.Value) : false);
        }
        set
        {
            hdnIsLifecycleInsert.Value = value.ToString();
            btnCreateNewLifecycle.Enabled = !value;
            gvLifecycleLines.Enabled = !value;
            btnCreateNewLifecycleLine.Enabled = !value;
        }
    }

    protected bool IsLifecycleLineInsert
    {
        get
        {
            bool b;
            return (hdnIsLifecycleLineInsert.Value != string.Empty && bool.TryParse(hdnIsLifecycleLineInsert.Value, out b) ? bool.Parse(hdnIsLifecycleLineInsert.Value) : false);
        }
        set
        {
            hdnIsLifecycleLineInsert.Value = value.ToString();
            btnCreateNewLifecycleLine.Enabled = !value;
            gvLifecycles.Enabled = !value;
            btnCreateNewLifecycle.Enabled = !value;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvLifecycles.DataBind();
    }

    #region Lifecycles

    protected void gvLifecycles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dataRowView = (DataRowView)e.Row.DataItem;

                if (gvLifecycles.EditIndex >= 0 && e.Row.RowIndex == gvLifecycles.EditIndex)
                {
                    editingRow = e.Row;

                    lbtViewLines.Visible = false;
                    lbtDeleteCycle.Visible = false;
                    lbtEditCycle.Visible = false;
                    lbtDeActivateCycle.Visible = false;
                    lbtUpdateCycle.Visible = true;
                    lbtCancelCycle.Visible = true;
                    lbtDeActivateCycle.Visible = false;
                    chkIsLifecycleActive.Enabled = !chkIsLifecycleActive.Checked;
                    btnCreateNewLifecycle.Enabled = false;
                    gvLifecycleLines.Enabled = false;
                    btnCreateNewLifecycleLine.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvLifecycles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName.ToUpper())
            {
                case "":
                    gvLifecycles.EditIndex = -1;
                    TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                    gvLifecycles.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    break;
                case "CANCEL":
                    IsLifecycleInsert = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvLifecycles_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            e.NewValues["isActive"] =  chkIsLifecycleActive.Enabled ? chkIsLifecycleActive.Checked : true;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvLifecycles_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
        else
        {
            IsLifecycleInsert = false;
        }
    }

    protected void lbtViewLines_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int lifecycleId = int.Parse((string)e.CommandArgument);
            gvLifecycles.SelectedIndex = findRowIndex(gvLifecycles, lifecycleId);
            IsLifecycleInsert = false;
            pnlLifecycleLines.Visible = true;
            Utility.ScrollToBottom(hdnScrollToBottom);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtEditCycle_Command(object sender, CommandEventArgs e)
    {
        try
        {
            IsLifecycleInsert = false;

            int lifecycleId = int.Parse((string)e.CommandArgument);
            gvLifecycles.EditIndex = findRowIndex(gvLifecycles, lifecycleId);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDeleteCycle_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int lifecycleId = int.Parse((string)e.CommandArgument);
            LifecycleMaintenanceAdapter.DeleteLifecycle(lifecycleId);

            IsLifecycleInsert = false;
            gvLifecycles.EditIndex = -1;
            gvLifecycles.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDeActivateCycle_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int lifecycleId = int.Parse((string)e.CommandArgument);
            if (LifecycleMaintenanceAdapter.DeActivateLifecycle(lifecycleId))
                gvLifecycles.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void odsLifecycles_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        try
        {
            if (e.Exception != null)
            {
                string errMessage = "";
                if (e.Exception.InnerException != null)
                    errMessage = Utility.GetCompleteExceptionMessage(e.Exception.InnerException);
                else
                    errMessage = Utility.GetCompleteExceptionMessage(e.Exception);

                if (errMessage != "")
                {
                    lblErrorMessage.Text = errMessage;
                }
                e.ExceptionHandled = true;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateNewLifecycle_Click(object sender, EventArgs e)
    {
        try
        {
            IsLifecycleInsert = true;
            gvLifecycles.DataBind();

            if (gvLifecycles.PageIndex != gvLifecycles.PageCount - 1 && (gvLifecycles.PageCount - 1) >= 0)
            {
                gvLifecycles.PageIndex = gvLifecycles.PageCount - 1;
                gvLifecycles.DataBind();
            }
            gvLifecycles.EditIndex = findRowIndex(gvLifecycles, 0); // gvLifecycles.Rows.Count - 1;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnUpdateLifecycleModelToAge_Click(object sender, EventArgs e)
    {
        try
        {
            BatchExecutionResults results = new BatchExecutionResults();
            LifecycleMaintenanceAdapter.UpdateLifecycleModelToAge(results);
            lblErrorMessage.Text = LifecycleMaintenanceAdapter.FormatErrorsForUpdateLifecycleModelToAge(results);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    #endregion

    #region LifecycleLines

    protected void gvLifecycleLines_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dataRowView = (DataRowView)e.Row.DataItem;

                if (gvLifecycleLines.EditIndex >= 0 && e.Row.RowIndex == gvLifecycleLines.EditIndex)
                {
                    editingRow = e.Row;

                    lbtDeleteLine.Visible = false;
                    lbtEditLine.Visible = false;
                    lbtUpdateLine.Visible = true;
                    lbtCancelLine.Visible = true;
                    btnCreateNewLifecycleLine.Enabled = false;

                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvLifecycleLines_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName.ToUpper())
            {
                case "":
                    gvLifecycleLines.EditIndex = -1;
                    TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                    gvLifecycleLines.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    break;
                case "CANCEL":
                    IsLifecycleLineInsert = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvLifecycleLines_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            DropDownList ddlModel = (DropDownList)gvLifecycleLines.Rows[e.RowIndex].FindControl("ddlModel");
            e.NewValues["modelId"] = Utility.GetKeyFromDropDownList(ddlModel);

            DecimalBox dbAgeFrom = (DecimalBox)gvLifecycleLines.Rows[e.RowIndex].FindControl("dbAgeFrom");
            e.NewValues["ageFrom"] = (int)dbAgeFrom.Value;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvLifecycleLines_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
        else
        {
            IsLifecycleLineInsert = false;
        }
    }

    protected void lbtEditLine_Command(object sender, CommandEventArgs e)
    {
        try
        {
            IsLifecycleLineInsert = false;

            int lineId = int.Parse((string)e.CommandArgument);
            gvLifecycleLines.EditIndex = findRowIndex(gvLifecycleLines, lineId);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDeleteLine_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int lineId = int.Parse((string)e.CommandArgument);
            int lifecycleId = Utility.GetCurrentRowKey(gvLifecycles);
            LifecycleMaintenanceAdapter.DeleteLifecycleLine(lifecycleId, lineId);

            IsLifecycleLineInsert = false;
            gvLifecycleLines.EditIndex = -1;
            gvLifecycleLines.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateNewLifecycleLine_Click(object sender, EventArgs e)
    {
        try
        {
            IsLifecycleLineInsert = true;
            gvLifecycleLines.DataBind();

            if (gvLifecycleLines.PageIndex != gvLifecycleLines.PageCount - 1 && (gvLifecycleLines.PageCount - 1) >= 0)
            {
                gvLifecycleLines.PageIndex = gvLifecycleLines.PageCount - 1;
                gvLifecycleLines.DataBind();
            }
            gvLifecycleLines.EditIndex = findRowIndex(gvLifecycleLines, 0); // gvLifecycles.Rows.Count - 1;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    #endregion

    #region Helpers

    private int findRowIndex(GridView grid, int key)
    {
        int rowIndex = -1;
        for (int i = 0; i < grid.DataKeys.Count; i++)
            if ((int)grid.DataKeys[i].Value == key)
                rowIndex = i;

        return rowIndex;
    }

    protected GridViewRow EditingRow
    {
        get
        {
            if (editingRow == null)
                editingRow = (gvLifecycles.EditIndex >= 0 ? gvLifecycles.Rows[gvLifecycles.EditIndex] : null);

            return editingRow;
        }
    }

    protected LinkButton lbtDeleteCycle { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtDeleteCycle"); } }
    protected LinkButton lbtEditCycle { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtEditCycle"); } }
    protected LinkButton lbtUpdateCycle { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtUpdateCycle"); } }
    protected LinkButton lbtCancelCycle { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtCancelCycle"); } }
    protected LinkButton lbtDeActivateCycle { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtDeActivateCycle"); } }
    protected LinkButton lbtViewLines { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtViewLines"); } }
    protected CheckBox chkIsLifecycleActive { get { return (CheckBox)Utility.FindControl(EditingRow, "chkIsActive"); } }

    protected LinkButton lbtDeleteLine { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtDeleteLine"); } }
    protected LinkButton lbtEditLine { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtEditLine"); } }
    protected LinkButton lbtUpdateLine { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtUpdateLine"); } }
    protected LinkButton lbtCancelLine { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtCancelLine"); } }

    private GridViewRow editingRow;

    #endregion

}

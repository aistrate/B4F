using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class NotificationMaintenance : System.Web.UI.Page
{
    public int NotificationID
    {
        get
        {
            object b = ViewState["NotificationID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["NotificationID"] = value;
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Notification Maintenance";
        }
        lblErrorMessage.Text = string.Empty;
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ctlAccountFinder.DoSearch();
        gvNotifications.Visible = true;
        btnCreateNewNotification.Visible = true;
        gvNotifications.DataBind();
    }

    protected void btnCreateNewNotification_Click(object sender, EventArgs e)
    {
        clear(); ;
        EditMode = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (saveData())
        {
            gvNotifications.DataBind();
            btnCancel_Click(sender, e);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        NotificationID = 0;
        EditMode = false;
    }

    protected void lbtEdit_Command(object sender, CommandEventArgs e)
    {
        try
        {
            EditMode = true;

            int notificationId = int.Parse((string)e.CommandArgument);
            loadData(notificationId);
            gvNotifications.SelectedIndex = findRowIndex(notificationId);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDeActivate_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int notificationId = int.Parse((string)e.CommandArgument);
            if (NotificationMaintenanceAdapter.DeActivateNotification(notificationId))
                gvNotifications.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private int findRowIndex(int key)
    {
        int rowIndex = -1;
        for (int i = 0; i < gvNotifications.DataKeys.Count; i++)
            if ((int)gvNotifications.DataKeys[i].Value == key)
                rowIndex = i;

        return rowIndex;
    }

    private void clear()
    {
        NotificationID = 0; ;
        ddlNotificationType_Edit.SelectedValue = int.MinValue.ToString();
        cldStartDate.Clear();
        cldDueDate.Clear();
        txtMessage.Text = "";
        ctlAccContSelector.Selection = null;
        ctlAccContSelector.Clear();
    }

    private void loadData(int notificationId)
    {
        clear();
        NotificationDetails details = NotificationMaintenanceAdapter.GetNotificationData(notificationId);
        if (details != null)
        {
            NotificationID = details.NotificationKey;
            ddlNotificationType_Edit.SelectedValue = details.NotificationTypeId.ToString();
            cldStartDate.SelectedDate = details.StartDate;
            cldDueDate.SelectedDate = details.DueDate;
            txtMessage.Text = details.Message;

            ctlAccContSelector.Selection = details.Selection;
        }
    }

    private bool saveData()
    {
        bool success = false;
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                NotificationDetails details = new NotificationDetails();
                details.NotificationKey = NotificationID;
                details.NotificationTypeId = Utility.GetKeyFromDropDownList(ddlNotificationType_Edit);
                details.Message = txtMessage.Text;
                details.StartDate = cldStartDate.SelectedDate;
                details.DueDate = cldDueDate.SelectedDate;
                details.Selection = ctlAccContSelector.Selection;
                success = NotificationMaintenanceAdapter.SaveNotificationData(details);
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
        return success;
    }

    private bool EditMode
    {
        get { return !pnlDetails.Visible; }
        set
        {
            pnlDetails.Visible = value;
            gvNotifications.Enabled = !value;
            btnCreateNewNotification.Enabled = !value;
            btnSave.Visible = value;
            btnCancel.Visible = value;

            if (value)
                Utility.ScrollToBottom(hdnScrollToBottom);
        }
    }

}

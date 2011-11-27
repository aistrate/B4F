using System;
using B4F.TotalGiro.ApplicationLayer.Reports;
using B4F.TotalGiro.Utils;


public partial class Reports_PrintNotas : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            ((EG)this.Master).setHeaderText = "Nota's";

        lblErrorMessage.Text = "";
    }


    protected void btnCreateNotas_Click(object sender, EventArgs e)
    {
        try
        {
            int currentManagmentCompanyId;
            string currentManagmentCompanyName;
            PrintNotasAdapter.GetCurrentManagmentCompany(out currentManagmentCompanyId, out currentManagmentCompanyName);

            BatchExecutionResults results = new BatchExecutionResults();
            PrintNotasAdapter.CreateNotas(results, currentManagmentCompanyId);
            lblErrorMessage.Text = PrintNotasAdapter.FormatErrorsForCreateNotas(results, currentManagmentCompanyName);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "<br/>ERROR: " + Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnPrintNotas_Click(object sender, EventArgs e)
    {
        try
        {
            int currentManagmentCompanyId;
            string currentManagmentCompanyName;
            PrintNotasAdapter.GetCurrentManagmentCompany(out currentManagmentCompanyId, out currentManagmentCompanyName);

            BatchExecutionResults results = new BatchExecutionResults();
            PrintNotasAdapter.PrintNotas(results, currentManagmentCompanyId);
            lblErrorMessage.Text = PrintNotasAdapter.FormatErrorsForPrintNotas(results, currentManagmentCompanyName);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "<br/>ERROR: " + Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSendEmailNotifications_Click(object sender, EventArgs e)
    {
        try
        {
            int currentManagmentCompanyId;
            string currentManagmentCompanyName;
            PrintNotasAdapter.GetCurrentManagmentCompany(out currentManagmentCompanyId, out currentManagmentCompanyName);

            BatchExecutionResults results = new BatchExecutionResults();
            PrintNotasAdapter.SendEmailNotifications(results, currentManagmentCompanyId);
            lblErrorMessage.Text = PrintNotasAdapter.FormatErrorsForSendEmails(results, currentManagmentCompanyName);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "<br/>ERROR: " + Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

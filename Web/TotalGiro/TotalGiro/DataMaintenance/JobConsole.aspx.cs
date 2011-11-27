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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Security;
using CustomControls;

public partial class DataMaintenance_JobConsole : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Job Management Console";
            showJobData();
        }
        lblError.Text = "";
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            showJobData();
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnStartJob_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            JobConsoleAdapter.StartJob(getSelectedJob());
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnStopJob_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            JobConsoleAdapter.StopJob(getSelectedJob());
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    protected void gvJobs_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            showJobDetailsData(null);
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    #region Functions

    protected void showJobData()
    {
        DataSet dsJobData = JobConsoleAdapter.GetJobReport();

        if (dsJobData != null)
        {
            // set caption gridview
            bool status = Convert.ToBoolean(dsJobData.Tables[0].Rows[0][0]);
            DateTime heartBeat = Convert.ToDateTime(dsJobData.Tables[0].Rows[0][1]);
            lblStatus.Text = string.Format("Status:\t\t\t{0}", (status ? "Running" : "Stopped"));
            lblStatus.Visible = true;
            lblHeartBeat.Text = string.Format("HeartBeat:\t\t\t{0}", heartBeat.ToLongTimeString());
            lblHeartBeat.Visible = true;

            createColumns(dsJobData, gvJobs, 1);
            gvJobs.DataSource = dsJobData.Tables[1].DefaultView;
            gvJobs.Visible = true;
            gvJobs.DataBind();

            showJobDetailsData(dsJobData);
        }
    }

    protected void showJobDetailsData(DataSet dsJobData)
    {
        if (gvJobs.SelectedRow != null)
        {
            if (dsJobData == null)
                dsJobData = JobConsoleAdapter.GetJobReport();
            if (dsJobData != null)
            {
                createColumns(dsJobData, gvJobComponents, 2);
                int id = getSelectedJob();
                if (id != 0)
                {
                    dsJobData.Tables[2].DefaultView.RowFilter = "JobID =" + id.ToString();
                    gvJobComponents.DataSource = dsJobData.Tables[2].DefaultView;
                    gvJobComponents.Visible = true;
                    gvJobComponents.DataBind();

                    if (SecurityManager.IsCurrentUserInRole("Data Mtce: Job Console Management"))
                    {
                        btnStartJob.Visible = true;
                        btnStopJob.Visible = true;
                    }
                }
            }
        }
    }

    private void createColumns(DataSet dsJobData, GridView grid, int tableIndex)
    {
        if (dsJobData != null)
        {
            grid.Columns.Clear();
            foreach (DataColumn column in dsJobData.Tables[tableIndex].Columns)
            {
                if (column.ColumnName == "LastResult")
                {
                    TemplateField tf = new TemplateField();
                    tf.HeaderText = column.ColumnName;
                    CustomGridViewTemplate t = new CustomGridViewTemplate(CustomGridViewTemplateTypes.LiteralText, column.ColumnName);
                    tf.ItemTemplate = t;
                    grid.Columns.Add(tf);
                }
                else
                {
                    BoundField boundField = new BoundField();
                    boundField.DataField = column.ColumnName;
                    boundField.HeaderText = column.ColumnName;
                    grid.Columns.Add(boundField);
                }
            }
        }
    }

    private int getSelectedJob()
    {
        int id;
        int.TryParse(gvJobs.SelectedRow.Cells[1].Text, out id);
        return id;
    }

    #endregion
}

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Text;
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;

public partial class FSFileOverview : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        if (!IsPostBack)
        {
            calStart.SelectedDate = DateTime.Today.AddDays(-10).Date;
            calEnd.SelectedDate = DateTime.Today.Date;

            ((EG)this.Master).setHeaderText = "Fund Settle Export Files";

            gvFSFileOverview.Sort("CreationDate", SortDirection.Descending);
            gvFSFileOverview.SelectedIndex = -1;

            gvOrderOverview.Sort("TradedInstrument_DisplayName", SortDirection.Ascending);
            gvOrderOverview.Visible = false;

            gvCurrencyOverview.Sort("CurrencyName", SortDirection.Ascending);
            gvCurrencyOverview.Visible = false;
        }
	}

	protected void gvFSFileOverview_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			object fsnumber = ((DataRowView)e.Row.DataItem)["FSNumber"];
			if (fsnumber.GetType().ToString() == "System.DBNull" || ((string)fsnumber).Length == 0)
			{
				e.Row.FindControl("lbtDeleteFile").Visible = true;
			}

            //Disable AutoComplete
            if (e.Row.RowState == DataControlRowState.Edit)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.Controls.Count > 0 && cell.Controls[0] is TextBox)
                    {
                        TextBox txtBox = (TextBox)cell.Controls[0];
                        txtBox.AutoCompleteType = AutoCompleteType.Disabled;
                    }
                }
            }
        }
	}

	protected void gvFSFileOverview_RowCommand(Object sender, GridViewCommandEventArgs e)
	{
		try
		{
            lblErrorMessage.Text = "";

            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    // Select row
                    gvFSFileOverview.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int fileId = (int)gvFSFileOverview.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "DELETEFILE":
                            FSFileOverviewAdapter.DeleteFSExportFile(fileId);
                            gvFSFileOverview.DataBind();
                            break;
                        case "VIEWORDERS":
                            gvFSFileOverview.EditIndex = -1;
                            gvOrderOverview.Visible = true;
                            gvCurrencyOverview.Visible = false;
                            gvOrderOverview.DataBind();
                            return;
                        case "TOTAL":
                            gvFSFileOverview.EditIndex = -1;
                            gvOrderOverview.Visible = false;
                            gvCurrencyOverview.Visible = true;
                            gvCurrencyOverview.DataBind();
                            return;
                    }
                }
            }
			
            gvFSFileOverview.SelectedIndex = -1;
            gvOrderOverview.Visible = false;
            gvCurrencyOverview.Visible = false;
		}
		catch (Exception ex)
		{
			lblErrorMessage.Text = ex.Message;
		}
	}

	public void gvFSFileOverview_RowEditing(object sender, GridViewEditEventArgs e)
	{
        gvFSFileOverview.EditIndex = e.NewEditIndex;
        gvFSFileOverview.DataBind();
	}

	public void gvFSFileOverview_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
	{
		gvFSFileOverview.EditIndex = -1;
		gvFSFileOverview.DataBind();
	}

	public void gvFSFileOverview_RowUpdated(object sender,  GridViewUpdatedEventArgs e)
	{
        if (e.Exception != null)
        {
            lblErrorMessage.Text = "Error updating fund settle number, check for duplicate numbers";
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
        int fileId = Convert.ToInt32(e.Keys[0].ToString());
        
        FSFileOverviewAdapter.PlaceOrdersForFile(fileId);
	}


	protected void btnToManualDesk_Click(object sender, EventArgs e)
	{
        Response.Redirect("FSDesk.aspx");
	}

    protected void calStart_SelectionChanged(object sender, EventArgs e)
    {
        if (calStart.SelectedDate.CompareTo(calEnd.SelectedDate.Date) > 0)
        {
            calEnd.SelectedDate = calStart.SelectedDate.Date;
        }
        txtFsNumber.Text = "";
        txtFile.Text = "";
    }
    protected void calEnd_SelectionChanged(object sender, EventArgs e)
    {
        if (calEnd.SelectedDate.CompareTo(calStart.SelectedDate.Date) < 0)
        {
            calStart.SelectedDate = calEnd.SelectedDate.Date;
        }
        txtFsNumber.Text = "";
        txtFile.Text = "";
    }
    protected void txtFile_TextChanged(object sender, EventArgs e)
    {
        int l_intRes=0;
        if (int.TryParse(txtFile.Text, out l_intRes)) txtFsNumber.Text = "";
    }
    protected void txtFsNumber_TextChanged(object sender, EventArgs e)
    {
        txtFile.Text = "";
    }
}

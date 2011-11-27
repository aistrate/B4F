using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using System.Data;
using B4F.TotalGiro.Dal.Utils;

public partial class ClientCashBalance : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Breakdown of TotalClient Liability for Date";
            this.dpTransactionDateFrom.SelectedDate = DateTime.Today;
            this.gvFullBalance.Sort("LineNumber", SortDirection.Ascending);
            //this.xml1.ObjectDataSourcetoBeExported = this.odsFullBalance.
        }
    }

    protected void gvFullBalance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["Account_Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void btnDownloadasExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime liabilityDate = this.dpTransactionDateFrom.SelectedDate;
            DataSet ds = ClientCashPositionFromGLLedgerAdapter.GetClientCashPositionFromGLLedgerDownload(liabilityDate);
            string xml = ExportToExcel.CreateWorkbook(ds);
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.Write(xml);
            Response.Flush();
            Response.End();
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
        }

    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        return;
    }
}

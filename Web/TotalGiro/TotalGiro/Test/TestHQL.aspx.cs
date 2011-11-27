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
using B4F.TotalGiro.ApplicationLayer.Test;

public partial class Test_TestHQL : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            ((EG)this.Master).setHeaderText = "Test HQL";
        
        lblError.Text = "";
        lblObjectCount.Text = "";
    }
    protected void btnGo_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            int objectCount;
            DateTime start = DateTime.Now;
            DataSet ds = TestHQLAdapter.GetData(txtHQL.Text, txtFields.Text, rblSource.Items[0].Selected, out objectCount);
            TimeSpan span = DateTime.Now - start;
            gvResult.DataSource = ds;
            createColumns(ds);
            lblObjectCount.Text = string.Format("Object Count: {0}    Duration: {1}:{2}:{3}", 
                objectCount, 
                span.Minutes.ToString("00"), 
                span.Seconds.ToString("00"), 
                span.Milliseconds.ToString("000"));
            gvResult.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void createColumns(DataSet ds)
    {
        gvResult.Columns.Clear();
        foreach (DataColumn column in ds.Tables[0].Columns)
        {
            BoundField boundField = new BoundField();
            boundField.DataField = column.ColumnName;
            boundField.HeaderText = column.ColumnName;
            gvResult.Columns.Add(boundField);
        }
    }
}

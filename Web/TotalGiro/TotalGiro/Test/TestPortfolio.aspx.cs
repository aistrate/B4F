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
using System.Text;

public partial class TestPortfolio : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Test Portfolio";
            //MultipleSelectionGridView1.MultipleSelection = false;

            MultipleSelectionGridView1.Sort("InstrumentName", SortDirection.Ascending);
            GridView1.Sort("InstrumentName", SortDirection.Ascending);
        }
    }

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        MultipleSelectionGridView1.ClearSelection();

        MultipleSelectionGridView1.DataBind();
        GridView1.DataBind();
    }
    
    protected void btnCheckSelectAll_Click(object sender, EventArgs e)
    {
        MultipleSelectionGridView1.SelectAllChecked = true;
    }
    
    protected void btnUncheckSelectAll_Click(object sender, EventArgs e)
    {
        MultipleSelectionGridView1.SelectAllChecked = false;
    }

    protected void btnShowSelectAll_Click(object sender, EventArgs e)
    {
        lblShowSelectAll.Text = MultipleSelectionGridView1.SelectAllChecked.ToString();
    }

    protected void btnClearSelection_Click(object sender, EventArgs e)
    {
        MultipleSelectionGridView1.ClearSelection();
    }

    protected void btnSelectAllRecords_Click(object sender, EventArgs e)
    {
        MultipleSelectionGridView1.SelectAllChecked = true;
        MultipleSelectionGridView1.SelectAllIds(true);
    }

    protected void btnShowSelectedIds_Click(object sender, EventArgs e)
    {
        int[] ids = MultipleSelectionGridView1.GetSelectedIds();
        
        StringBuilder str = new StringBuilder();
        foreach (int id in ids)
            str.Append(id.ToString() + ", ");
        if (str.Length > 0)
            str.Remove(str.Length - 2, 2);

        lblSelectedIds.Text = str.ToString();
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int rowIndex = Int32.Parse((String)e.CommandArgument);
        int positionId = (Int32)GridView1.DataKeys[rowIndex].Value;
    }

    protected void chkMultipleSelection_CheckedChanged(object sender, EventArgs e)
    {
        MultipleSelectionGridView1.MultipleSelection = chkMultipleSelection.Checked;
        MultipleSelectionGridView1.DataBind();
    }
}

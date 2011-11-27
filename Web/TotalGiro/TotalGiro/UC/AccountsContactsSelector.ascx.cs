using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.Instructions;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class AccountsContactsSelector : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.SetDefaultButton(this.Page, txtFilter, btnFilter);
        if (!IsPostBack)
        {
            gvSelection.DataSource = Selection;
            gvSelection.DataBind();
        }
    }

    public void Clear()
    {
        txtFilter.Text = "";
        rblSelectorType.SelectedIndex = 0;
        mvwSelection.ActiveViewIndex = 0;
        DataBind();
        Selection.Clear();
        gvSelection.DataSource = Selection;
        gvSelection.DataBind();
        gvContacts.DataBind();
        gvAccounts.DataBind();
    }

    protected void rblSelectorType_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtFilter.Text = "";
        mvwSelection.ActiveViewIndex = rblSelectorType.SelectedIndex;
        switch (mvwSelection.ActiveViewIndex)
        {
            case 0:
                gvContacts.DataBind();
                break;
            case 1:
                gvAccounts.DataBind();
                break;
        }
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (mvwSelection.ActiveViewIndex == 0)
            gvContacts.DataBind();
        else
            gvAccounts.DataBind();
    }

    protected void odsContacts_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        int[] excludedIds = (from a in Selection
                             where a.SelectedType == AccountContactSelectedTypes.Contact
                             select a.EntityKey).ToArray();

        e.InputParameters.Add("excludedKeys", excludedIds);
    }

    protected void odsAccounts_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        int[] excludedIds = (from a in Selection
                             where a.SelectedType == AccountContactSelectedTypes.Account
                             select a.EntityKey).ToArray();

        e.InputParameters.Add("excludedKeys", excludedIds);
    }

    protected void gvExclusion_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        try
        {
            GridView gvSender = (GridView)sender;

            int key = (int)gvSender.DataKeys[e.NewSelectedIndex].Value;
            string name = "";
            AccountContactSelectedTypes selectedType;

            switch (gvSender.ID)
            {
                case "gvContacts":
                    selectedType = AccountContactSelectedTypes.Contact;
                    name = gvSender.Rows[e.NewSelectedIndex].Cells[0].Text + " (" + gvSender.Rows[e.NewSelectedIndex].Cells[1].Text + ")";
                    break;
                default:
                    selectedType = AccountContactSelectedTypes.Account;
                    name = gvSender.Rows[e.NewSelectedIndex].Cells[1].Text + " (" + gvSender.Rows[e.NewSelectedIndex].Cells[0].Text + ")";
                    break;
            }


            Selection.Add(new AccountContactSelectedDetails(key, selectedType, name));
            gvSelection.DataSource = Selection;
            gvSelection.DataBind();
            gvSender.DataBind();
        }
        catch (ApplicationException ex)
        {
            lblErrorMessage.Text = ex.Message;
        }
    }

    protected void gvSelection_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Add here your method for DataBinding
        gvSelection.DataSource = Selection;
        gvSelection.PageIndex = e.NewPageIndex;
        gvSelection.DataBind();
    }

    protected void gvSelection_Sorting(object sender, GridViewSortEventArgs e)
    {
        sortDirection = (SortDirection)Math.Abs((int)sortDirection - 1);

        switch (e.SortExpression)
        {
            case "SelectedType":
                if (sortDirection == SortDirection.Ascending)
                    gvSelection.DataSource = Selection.OrderBy(u => u.SelectedType).ToList();
                else
                    gvSelection.DataSource = Selection.OrderByDescending(u => u.SelectedType).ToList();
                break;
            default:
                if (sortDirection == SortDirection.Ascending)
                    gvSelection.DataSource = Selection.OrderBy(u => u.SelectedType).ToList();
                else
                    gvSelection.DataSource = Selection.OrderByDescending(u => u.SelectedType).ToList();
                break;
        }
        gvSelection.DataBind();
    }

    protected void gvSelection_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

        if (e.RowIndex > -1 && Selection.Count > 0)
        {
            string key = gvSelection.DataKeys[e.RowIndex].Value.ToString();
            if (Selection.Exists(u => u.Key == key))
                Selection.Remove(Selection.First(u => u.Key == key));
            gvSelection.DataSource = Selection;
            gvSelection.DataBind();
            gvContacts.DataBind();
            gvAccounts.DataBind();
        }
    }

    public List<AccountContactSelectedDetails> Selection
    {
        get
        {
            object col = ViewState["Selection"];
            if (col == null)
            {
                col = new List<AccountContactSelectedDetails>();
                ViewState["Selection"] = col;
            }
            return (List<AccountContactSelectedDetails>)col;
        }
        set 
        {
            ViewState["Selection"] = value;
            gvSelection.DataSource = value;
            gvSelection.DataBind();
        }
    }

    private SortDirection sortDirection
    {
        get
        {
            object e = ViewState["SortDirection"];
            return ((e == null) ? SortDirection.Ascending : (SortDirection)e);
        }
        set { ViewState["SortDirection"] = value; }
    }

}

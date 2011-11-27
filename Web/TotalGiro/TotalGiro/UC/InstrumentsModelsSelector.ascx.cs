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

public partial class InstrumentsModelsSelector : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.SetDefaultButton(this.Page, txtFilter, btnFilter);
        if (!IsPostBack)
        {
            gvExclusions.DataSource = Exclusions;
            gvExclusions.DataBind();
        }
    }

    public void Clear()
    {
        txtFilter.Text = "";
        rblExclusionType.SelectedIndex = 0;
        mvwExclusions.ActiveViewIndex = 0;
        DataBind();
        Exclusions.Clear();
        gvExclusions.DataSource = Exclusions;
        gvExclusions.DataBind();
        gvSelectInstrumentsToExclude.DataBind();
        gvSelectModelsToExclude.DataBind();
    }

    protected void rblExclusionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtFilter.Text = "";
        mvwExclusions.ActiveViewIndex = rblExclusionType.SelectedIndex;
        switch (mvwExclusions.ActiveViewIndex)
        {
            case 0:
                gvSelectInstrumentsToExclude.DataBind();
                break;
            case 1:
                gvSelectModelsToExclude.DataBind();
                break;
        }
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (mvwExclusions.ActiveViewIndex == 0)
            gvSelectInstrumentsToExclude.DataBind();
        else
            gvSelectModelsToExclude.DataBind();
    }

    protected void odsSelectInstrumentsToExclude_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        int[] excludedIds = (from a in Exclusions
                             where a.ComponentType == ModelComponentType.Instrument
                             select a.ComponentKey).ToArray();

        e.InputParameters.Add("excludedKeys", excludedIds);
    }

    protected void odsSelectModelsToExclude_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        int[] excludedIds = (from a in Exclusions
                             where a.ComponentType == ModelComponentType.Model
                             select a.ComponentKey).ToArray();

        e.InputParameters.Add("excludedKeys", excludedIds);
    }

    protected void gvExclusion_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        try
        {
            GridView gvSender = (GridView)sender;

            int key = (int)gvSender.DataKeys[e.NewSelectedIndex].Value;
            string name = "";
            ModelComponentType compType;

            switch (gvSender.ID)
            {
                case "gvSelectInstrumentsToExclude":
                    compType = ModelComponentType.Instrument;
                    name = gvSender.Rows[e.NewSelectedIndex].Cells[1].Text + " (" + gvSender.Rows[e.NewSelectedIndex].Cells[0].Text + ")";
                    break;
                default:
                    compType = ModelComponentType.Model;
                    name = gvSender.Rows[e.NewSelectedIndex].Cells[0].Text;
                    break;
            }


            Exclusions.Add(new RebalanceExclusionDetails(key, name, compType));
            gvExclusions.DataSource = Exclusions;
            gvExclusions.DataBind();
            gvSender.DataBind();
        }
        catch (ApplicationException ex)
        {
            lblErrorMessage.Text = ex.Message;
        }
    }

    protected void gvExclusions_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Add here your method for DataBinding
        gvExclusions.DataSource = Exclusions;
        gvExclusions.PageIndex = e.NewPageIndex;
        gvExclusions.DataBind();
    }

    protected void gvExclusions_Sorting(object sender, GridViewSortEventArgs e)
    {
        sortDirection = (SortDirection)Math.Abs((int)sortDirection - 1);

        switch (e.SortExpression)
        {
            case "ComponentType":
                if (sortDirection == SortDirection.Ascending)
                    gvExclusions.DataSource = Exclusions.OrderBy(u => u.ComponentType).ToList();
                else
                    gvExclusions.DataSource = Exclusions.OrderByDescending(u => u.ComponentType).ToList();
                break;
            default:
                if (sortDirection == SortDirection.Ascending)
                    gvExclusions.DataSource = Exclusions.OrderBy(u => u.ComponentName).ToList();
                else
                    gvExclusions.DataSource = Exclusions.OrderByDescending(u => u.ComponentName).ToList();
                break;
        }
        gvExclusions.DataBind();
    }

    protected void gvExclusions_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

        if (e.RowIndex > -1 && Exclusions.Count > 0)
        {
            string key = gvExclusions.DataKeys[e.RowIndex].Value.ToString();
            if (Exclusions.Exists(u => u.Key == key))
                Exclusions.Remove(Exclusions.First(u => u.Key == key));
            gvExclusions.DataSource = Exclusions;
            gvExclusions.DataBind();
            gvSelectInstrumentsToExclude.DataBind();
            gvSelectModelsToExclude.DataBind();
        }
    }

    public List<RebalanceExclusionDetails> Exclusions
    {
        get
        {
            object col = ViewState["Exclusions"];
            if (col == null)
            {
                col = new List<RebalanceExclusionDetails>();
                ViewState["Exclusions"] = col;
            }
            return (List<RebalanceExclusionDetails>)col;
        }
        set 
        { 
            ViewState["Exclusions"] = value;
            gvExclusions.DataSource = value;
            gvExclusions.DataBind();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;

public partial class TrialBalance : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "View Trial Balance";
            this.hdnBalanceType.Value = TrialBalanceType.Nett.ToString();
            this.dpTransactionDateFrom.SelectedDate = DateTime.Today;
            this.gvFullBalance.Sort("LineNumber", SortDirection.Ascending);

        }
    }

    protected TrialBalanceType BalanceType
    {
        get
        {
            object i = ViewState["BalanceType"];
            return ((i == null) ? TrialBalanceType.Nett : (TrialBalanceType)i);
        }
        set { ViewState["BalanceType"] = value; }
    }

    protected void btnNett_Click(object sender, EventArgs e)
    {
        this.hdnBalanceType.Value = TrialBalanceType.Nett.ToString();
    }

    protected void btnFull_Click(object sender, EventArgs e)
    {
        this.hdnBalanceType.Value = TrialBalanceType.Full.ToString();
    }

    protected void btnGroup_Click(object sender, EventArgs e)
    {
        this.hdnBalanceType.Value = TrialBalanceType.Exact.ToString();
    }
}

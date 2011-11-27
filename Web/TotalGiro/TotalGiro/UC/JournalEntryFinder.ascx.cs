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
using B4F.TotalGiro.GeneralLedger.Journal;
using System.ComponentModel;
using B4F.TotalGiro.GeneralLedger.Static;

public partial class JournalEntryFinder : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            JournalType = JournalType;      // this sets the default value
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    public JournalTypes JournalType
    {
        get
        {
            return ((hdnJournalType.Value == string.Empty) ? 
                            JournalTypes.BankStatement : 
                            (JournalTypes)Enum.Parse(typeof(JournalTypes), hdnJournalType.Value));
        }
        set { hdnJournalType.Value = ((int)value).ToString(); }
    }

    public int JournalId
    {
        get
        {
            object i = ViewState["JournalId"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            ViewState["JournalId"] = value;
            ddlJournal.SelectedValue = (value != 0 ? value : int.MinValue).ToString();
        }
    }

    public DateTime TransactionDateFrom
    {
        get
        {
            object d = ViewState["TransactionDateFrom"];
            return ((d == null) ? DateTime.MinValue : (DateTime)d);
        }
        set
        {
            ViewState["TransactionDateFrom"] = value;
            dpTransactionDateFrom.SelectedDate = value;
        }
    }

    public DateTime TransactionDateTo
    {
        get
        {
            object d = ViewState["TransactionDateTo"];
            return ((d == null) ? DateTime.MinValue : (DateTime)d);
        }
        set
        {
            ViewState["TransactionDateTo"] = value;
            dpTransactionDateTo.SelectedDate = value;
        }
    }

    public string JournalEntryNumber
    {
        get
        {
            object s = ViewState["JournalEntryNumber"];
            return ((s == null) ? "" : (string)s);
        }
        set
        {
            ViewState["JournalEntryNumber"] = value;
            txtJournalEntryNumber.Text = value;
        }
    }

    public JournalEntryStati Statuses
    {
        get
        {
            object e = ViewState["Statuses"];
            return ((e == null) ? JournalEntryStati.None : (JournalEntryStati)e);
        }
        set
        {
            ViewState["Statuses"] = value;
            for (int i = 0; i < cblStatus.Items.Count; i++)
            {
                JournalEntryStati nominalStatus = getCheckBoxNominalStatus(i);
                cblStatus.Items[i].Selected = (nominalStatus & value) == nominalStatus;
            }
        }
    }

    private JournalEntryStati getSelectedStatuses()
    {
        JournalEntryStati statuses = JournalEntryStati.None;
        for (int i = 0; i < cblStatus.Items.Count; i++)
            statuses |= (cblStatus.Items[i].Selected ? getCheckBoxNominalStatus(i) : JournalEntryStati.None);
        return statuses;
    }

    private JournalEntryStati getCheckBoxNominalStatus(int index)
    {
        return (JournalEntryStati)Enum.Parse(typeof(JournalEntryStati), cblStatus.Items[index].Value);
    }

    public void DoSearch()
    {
        dpTransactionDateFrom.IsExpanded = false;
        dpTransactionDateTo.IsExpanded = false;

        JournalId = (ddlJournal.SelectedValue != int.MinValue.ToString() ? int.Parse(ddlJournal.SelectedValue) : 0);
        TransactionDateFrom = dpTransactionDateFrom.SelectedDate;
        TransactionDateTo = dpTransactionDateTo.SelectedDate;
        JournalEntryNumber = txtJournalEntryNumber.Text;
        Statuses = getSelectedStatuses();

        OnSearch();
    }

    protected void OnSearch()
    {
        if (Search != null)
            Search(this, EventArgs.Empty);
    }

    [Description("Event triggered when button Search is clicked."), Category("Behavior")]
    public EventHandler Search;
}

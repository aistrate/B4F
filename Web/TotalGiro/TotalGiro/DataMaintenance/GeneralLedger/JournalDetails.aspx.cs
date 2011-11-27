using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.GeneralLedger;
using B4F.TotalGiro.GeneralLedger.Static;

public partial class JournalDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";

        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Create/Edit GL Journal Details";
            GLJournalID = (Session["glJournalID"] != null ? (int)Session["glJournalID"] : 0);
            loadDetails();
            DataBind();

        }
    }


    public int GLJournalID
    {
        get
        {
            object i = ViewState["GLJournalID"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["GLJournalID"] = value; }
    }

    public string JournalNumber
    {
        get
        {
            return this.txtJournalNumber.Text;
        }
        set { this.txtJournalNumber.Text = value; }
    }

    public string Description
    {
        get
        {
            return this.txtDescription.Text;
        }
        set { this.txtDescription.Text = value; }
    }

    public string FixedAccount
    {
        get
        {
            return this.txtFixedAccount.Text;
        }
        set { this.txtFixedAccount.Text = value; }
    }

    public string BankAccountNumber
    {
        get
        {
            return this.txtBankAccountNumber.Text;
        }
        set { this.txtBankAccountNumber.Text = value; }
    }

    public JournalTypes JournalType
    {
        get
        {
            return (JournalTypes) int.Parse( this.ddlJournalType.SelectedValue);
        }
        set { this.ddlJournalType.SelectedValue = ((int)value).ToString(); }
    }

    private void loadDetails()
    {
        if (GLJournalID != 0)
        {
            JournalsAdapter.GLJournalDetails details = JournalsAdapter.GetGLJournalDetails(this.GLJournalID);
            this.JournalNumber = details.JournalNumber;
            this.Description = details.Description;
            this.JournalType = details.JournalType;
            this.FixedAccount = details.FixedAccount;
            this.BankAccountNumber = details.BankAccountNumber;
        }
    }

    private void SaveDetails()
    {
        JournalsAdapter.GLJournalDetails details = new JournalsAdapter.GLJournalDetails();
        details.Key = this.GLJournalID;
        details.JournalNumber = this.JournalNumber;
        details.Description = this.Description;
        if (this.JournalType == JournalTypes.BankStatement) details.BankAccountNumber = this.BankAccountNumber;
        this.GLJournalID =  JournalsAdapter.SaveGLJournalDetails(details);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            SaveDetails();
            loadDetails();

        }
        catch (Exception ex)
        {
            lblMessage.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }


}

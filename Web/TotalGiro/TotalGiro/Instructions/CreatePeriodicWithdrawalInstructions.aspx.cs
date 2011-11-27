using System;
using System.Collections;
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
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.Instructions;
using System.Drawing;

public partial class Instructions_CreatePeriodicWithdrawalInstructions : System.Web.UI.Page
{

    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += (s, ev) =>
        {
            gvWithDrawals.Visible = true;
            InstructionVisible = false;
        };
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Create Periodic Withdrawal Instructions";
        }
    }

    protected void rblCreatePeriodicWithdrawalsChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblMessage2.Text = "";
        this.mlvCreatePeriodicWithdrawals.ActiveViewIndex = this.rblCreatePeriodicWithdrawalsChoice.SelectedIndex;
    }

    protected void gvWithDrawals_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        gvInstructions.Visible = false;
        lblMessage2.Text = "";

        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvWithDrawals.EditIndex = -1;

                    // Select row
                    gvWithDrawals.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWINSTRUCTIONS":
                            InstructionVisible = true;
                            break;
                        case "CREATEWITHDRAWALS":
                            setEditMode(true);

                            int key = 0;
                            int.TryParse(gvWithDrawals.SelectedValue.ToString(), out key);
                            DateTime endDate = CreatePeriodicWithdrawalInstructionsAdapter.GetMaximumWithdrawalCreationDate(key);
                            dtpEndDate.SelectedDate = endDate;
                            rvEndDate.MinimumValue = DateTime.Today.ToShortDateString();

                            dtpEndDate.SelectionChanged += (s, ev) =>
                            {
                                lblMessage2.Text = "";
                            };
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblMessage2.Text = Utility.GetCompleteExceptionMessage(ex);
            gvWithDrawals.SelectedIndex = -1;
        }
    }

    protected void gvWithDrawals_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["AccountId"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void btnCreatePeriodicWithdrawals_Click(object sender, EventArgs e)
    {
        try
        {
            lblMessage2.Text = "";
            BatchExecutionResults results = new BatchExecutionResults();
            CreatePeriodicWithdrawalInstructionsAdapter.CreatePeriodicWithdrawals(results);
            lblMessage2.Text = CreatePeriodicWithdrawalInstructionsAdapter.FormatErrorsForCreatePeriodicWithdrawals(results);
        }
        catch (Exception ex)
        {
            lblMessage2.ForeColor = Color.Red;
            lblMessage2.Text = "<br/>ERROR: " + Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreatePeriodicWithdrawals2_Click(object sender, EventArgs e)
    {
        try
        {
            lblMessage2.Text = "";
            int key = 0;

            if (int.TryParse(gvWithDrawals.SelectedValue.ToString(), out key))
            {
                BatchExecutionResults results = new BatchExecutionResults();
                CreatePeriodicWithdrawalInstructionsAdapter.CreatePeriodicWithdrawals(results, key, dtpEndDate.SelectedDate);
                lblMessage2.Text = CreatePeriodicWithdrawalInstructionsAdapter.FormatErrorsForCreatePeriodicWithdrawals(results);
                setEditMode(false);
            }
            else
                throw new ApplicationException("Please select a withdrawal rule first.");
        }
        catch (Exception ex)
        {
            lblMessage2.ForeColor = Color.Red;
            lblMessage2.Text = "<br/>ERROR: " + Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        setEditMode(false);
    }

    public bool InstructionVisible 
    {
        get
        {
            return gvInstructions.Visible;
        }
        set
        {
            gvInstructions.Visible = value;
            btnHideInstructions.Visible = value;
        }
    }

    protected void btnHideInstructions_Click(object sender, EventArgs e)
    {
        InstructionVisible = false;
    }

    private void setEditMode(bool editMode)
    {
        pnlCreateWithdrawals.Visible = editMode;
        gvWithDrawals.Enabled = !editMode;
        InstructionVisible = false;
    }

}

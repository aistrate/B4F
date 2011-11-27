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
using B4F.TotalGiro.ApplicationLayer.Compliance;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class Compliance_ApproveStornos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Approve Storno Transactions";

            if (!isLoggedInAsCompliance)
            {
                gvStornos.MultipleSelection = false;
                btnApprove.Visible = false;
                btnDisapprove.Visible = false;
            }

            gvStornos.Sort("Key", SortDirection.Descending);
        }

        lblErrorMessage.Text = "";
    }

    private bool isLoggedInAsCompliance = AccountFinderAdapter.IsLoggedInAsStichting() || AccountFinderAdapter.IsLoggedInAsCompliance();

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        try
        {
            int[] selectedStornoTxIds = gvStornos.GetSelectedIds();

            if (selectedStornoTxIds.Length > 0)
            {
                Exception[] exceptions = ApproveStornosAdapter.ApproveStornoTransactions(selectedStornoTxIds);

                if (selectedStornoTxIds.Length > exceptions.Length)
                    lblErrorMessage.Text = string.Format("<br/>{0} transactions have been successfully approved.<br/>",
                        selectedStornoTxIds.Length - exceptions.Length);

                if (exceptions.Length > 0)
                {
                    lblErrorMessage.Text += string.Format("<br/>{0} transactions could not be approved:<br/>", exceptions.Length);

                    foreach (Exception ex in exceptions)
                        lblErrorMessage.Text += string.Format("<br/>{0}", Utility.GetCompleteExceptionMessage(ex));
                }
                gvStornos.DataBind();
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnDisapprove_Click(object sender, EventArgs e)
    {
        try
        {
            int[] selectedStornoTxIds = gvStornos.GetSelectedIds();

            if (selectedStornoTxIds.Length > 0)
            {
                ApproveStornosAdapter.DisapproveStornoTransactions(selectedStornoTxIds);

                lblErrorMessage.Text = string.Format("<br/>{0} transactions have been disapproved.<br/>", selectedStornoTxIds.Length);

                gvStornos.DataBind();
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }
}

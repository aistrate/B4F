using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Interfaces.Util;
using B4F.Web.WebControls;

/// <summary>
/// Helper for class CommonLogins.
/// </summary>
public class PageContext
{
    public PageContext(MultipleSelectionGridView gridView, ErrorLabel errorLabel, HiddenField scrollToBottomHiddenField)
    {
        GridView = gridView;
        ErrorLabel = errorLabel;
        ScrollToBottomHiddenField = scrollToBottomHiddenField;
    }

    public MultipleSelectionGridView GridView { get; private set; }
    public ErrorLabel ErrorLabel { get; private set; }
    public HiddenField ScrollToBottomHiddenField { get; private set; }
}

/// <summary>
/// Common methods for ClientLogins.aspx.cs and RemisierLogins.aspx.cs.
/// </summary>
public static class CommonLogins
{
    public static void OnCheckedChanged(PageContext pageContext, CheckBox checkBox,
                                        Action<int, bool> saveValue, Func<bool, string> getConfirmMessage)
    {
        try
        {
            int personKey = (int)pageContext.GridView.DataKeys[((GridViewRow)checkBox.NamingContainer).RowIndex].Value;
            saveValue(personKey, checkBox.Checked);
            pageContext.GridView.DataBind();
            pageContext.ErrorLabel.Text = getConfirmMessage(checkBox.Checked);
        }
        catch (Exception ex)
        {
            pageContext.GridView.DataBind();
            pageContext.ErrorLabel.Text = Utility.GetCompleteExceptionMessage(ex);
        }

        Utility.ScrollToBottom(pageContext.ScrollToBottomHiddenField);
    }

    public static void OnLinkButtonClick(PageContext pageContext, LinkButton linkButton,
                                         Action<int> doAction, bool dataBindOnSuccess, bool dataBindOnError, string afterMessage)
    {
        try
        {
            int personKey = int.Parse(linkButton.CommandArgument);
            doAction(personKey);
            if (dataBindOnSuccess)
                pageContext.GridView.DataBind();
            pageContext.ErrorLabel.Text = afterMessage;
        }
        catch (Exception ex)
        {
            if (dataBindOnError)
                pageContext.GridView.DataBind();
            pageContext.ErrorLabel.Text = Utility.GetCompleteExceptionMessage(ex);
        }

        Utility.ScrollToBottom(pageContext.ScrollToBottomHiddenField);
    }

    public static void OnGenerateSend(PageContext pageContext,
                                      Func<int[], BatchExecutionResults2> doGenerateSend, string entityName)
    {
        try
        {
            BatchExecutionResults2 batchExecutionResults = doGenerateSend(pageContext.GridView.GetSelectedIds());

            if (batchExecutionResults.SuccessCount > 0)
                pageContext.GridView.DataBind();
            pageContext.GridView.ClearSelection();
            pageContext.ErrorLabel.Text = Utility.FormatBatchExecutionResults(
                                                        batchExecutionResults,
                                                        string.Format("No {0}s were sent.", entityName),
                                                        string.Format("{{0}} {0} was successfully sent.", entityName),
                                                        string.Format("{{0}} {0}s were successfully sent.", entityName),
                                                        string.Format("{{0}} error occured while sending {0}s:", entityName),
                                                        string.Format("{{0}} errors occured while sending {0}s:", entityName));
        }
        catch (Exception ex)
        {
            pageContext.ErrorLabel.Text = Utility.GetCompleteExceptionMessage(ex);
        }

        Utility.ScrollToBottom(pageContext.ScrollToBottomHiddenField);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Interfaces.Util;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Linq;
using B4F.TotalGiro.Utils.Tuple;
using System.Drawing;

namespace B4F.TotalGiro.Client.Web.Util
{
    public static class Utility
    {
        public static string GetCompleteExceptionMessage(Exception ex)
        {
            return GetCompleteExceptionMessage(ex, "<br/>", "<br/>");
        }

        public static string GetCompleteExceptionMessage(Exception ex, string separator, string afterLast)
        {
            ex = GetRealException(ex);

            if (ex.InnerException != null)
                return ex.Message + separator + GetCompleteExceptionMessage(ex.InnerException, separator, afterLast);
            else
                return ex.Message + afterLast;
        }

        public static Exception GetRealException(Exception ex)
        {
            if (ex is System.Reflection.TargetInvocationException && ex.InnerException != null)
                return ex.InnerException;
            else
                return ex;
        }

        public static string FormatAccountNumbers<T>(IEnumerable<T> accountNumbers, Func<T, string> formatAccountNumber)
        {
            return accountNumbers.Split(3)
                                 .Select(g => g.Select(n => formatAccountNumber(n))
                                               .JoinStrings(", "))
                                 .JoinStrings(",<br />");
        }

        public static string FormatAccountNumbers(IEnumerable<string> accountNumbers)
        {
            return FormatAccountNumbers<string>(accountNumbers, n => n);
        }

        public static string FormatAccountNumbersByActive(IEnumerable<Tuple<string, bool>> accountNumbers)
        {
            return FormatAccountNumbers<Tuple<string, bool>>(accountNumbers,
                                                             t => formatAccountNumberByActive(t.Item1, t.Item2));
        }

        private static string formatAccountNumberByActive(string accountNumber, bool isActive)
        {
            return isActive ?
                accountNumber :
                string.Format("<span style=\"color: DarkGray; font-style: italic\">{0}</span>", accountNumber);
        }

        public static string GetTrafficLightColorDescription(Color color)
        {
            return trafficLightColorDescriptions.ContainsKey(color) ? trafficLightColorDescriptions[color] : "";
        }

        // TODO: translate
        private static Dictionary<Color, string> trafficLightColorDescriptions = new Dictionary<Color, string>()
    {
        { Color.Red, "Thunderstorm (under 50%)" },
        { Color.Yellow, "Cloudy with a bit of sun (50% to 79%)" },
        { Color.Green, "Sunny (80% or over)" }
    };

        public static string FormatBatchExecutionResults(BatchExecutionResults2 batchExecutionResults, string nothingProcessedMessage,
                                                         string successMessage, string errorsMessage)
        {
            return FormatBatchExecutionResults(batchExecutionResults, nothingProcessedMessage,
                                               successMessage, successMessage, errorsMessage, errorsMessage);
        }

        public static string FormatBatchExecutionResults(BatchExecutionResults2 batchExecutionResults, string nothingProcessedMessage,
                                                         string successMessageSingular, string successMessagePlural,
                                                         string errorsMessageSingular, string errorsMessagePlural)
        {
            StringBuilder message = new StringBuilder();

            if (batchExecutionResults.SuccessCount == 0 && batchExecutionResults.ErrorCount == 0)
                message.Append(nothingProcessedMessage);
            else
            {
                if (batchExecutionResults.SuccessCount > 0)
                    message.Append(string.Format(batchExecutionResults.SuccessCount == 1 ? successMessageSingular : successMessagePlural,
                                                 batchExecutionResults.SuccessCount) + "<br/>");

                if (batchExecutionResults.SuccessCount > 0 && batchExecutionResults.ErrorCount > 0)
                    message.Append("<br/>");

                if (batchExecutionResults.ErrorCount > 0)
                {
                    message.Append(string.Format(batchExecutionResults.ErrorCount == 1 ? errorsMessageSingular : errorsMessagePlural,
                                                 batchExecutionResults.ErrorCount) + "<br/>");

                    foreach (Exception ex in batchExecutionResults.Errors)
                        message.Append("<br/>" + GetCompleteExceptionMessage(ex));
                }
            }

            return message.ToString();
        }

        public static void SetDefaultButton(Page page, WebControl control, Button defaultButton)
        {
            // Sets default buttons.
            // Originally created by Janus Kamp Hansen - http://www.kamp-hansen.dk
            // Extended by Darrell Norton - http://dotnetjunkies.com/weblog/darrell.norton/ 
            //   -- added Mozilla support, fixed a few issues, improved performance

            string theScript = @"
function fnTrapKD(btn, event){
    if (document.all){
        if (event.keyCode == 13){
            event.returnValue=false;
            event.cancel = true;
            btn.click();
            }
        }
        else if (document.getElementById){
            if (event.which == 13){
            event.returnValue=false;
            event.cancel = true;
            btn.click();
            }
        }
        else if(document.layers){
            if(event.which == 13){
            event.returnValue=false;
            event.cancel = true;
            btn.click();
        }
    }
}";

            if (!page.ClientScript.IsClientScriptBlockRegistered(page.GetType(), "ForceDefaultToScript"))
            {
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "ForceDefaultToScript", theScript, true);
            }
            control.Attributes.Add("onkeydown", "fnTrapKD(" + defaultButton.ClientID + ",event)");
        }

        /// <summary>
        /// Sets up JavaScript code for method ScrollToBottom(). Must be called on Page_Load.
        /// The page must contain a ScriptManager control.
        /// </summary>
        /// <param name="page">The containing page.</param>
        /// <param name="hiddenField">A hidden field inside the page. 
        /// If using partial postbacks, the hidden field must be inside the same UpdatePanel as 
        /// the web control (e.g. Button) that calls method ScrollToBottom().</param>
        public static void EnableScrollToBottom(Page page, HiddenField hiddenField)
        {
            string script = string.Format(@"
            doScrollToBottom = function() {{
                var h = document.getElementById('{0}');
                var s = (h == null) ? false : (h.value == 'true');
                if (s) {{
                    scrollTo(0, 9999999);
                    h.value = 'false';
                }};
            }};",
                hiddenField.ClientID);
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "doScrollToBottom", script, true);

            script = @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(doScrollToBottom);";
            page.ClientScript.RegisterStartupScript(page.GetType(), "addDoScrollToBottom", script, true);
        }

        /// <summary>
        /// Causes the page to scroll all the way down, on the next page-load (with both full and partial postbacks).
        /// Call EnableScrollToBottom() on Page_Load before calling this method.
        /// The page must contain a ScriptManager control.
        /// </summary>
        /// <param name="hiddenField">A hidden field inside the page. 
        /// If using partial postbacks, the hidden field must be inside the same UpdatePanel as 
        /// the web control (e.g. Button) that calls this method.</param>
        public static void ScrollToBottom(HiddenField hiddenField)
        {
            hiddenField.Value = "true";
        }

        public static QueryParameterCollection GetQueryParameters()
        {
            return new QueryParameterCollection(HttpContext.Current);
        }

        public static GridViewNavigationSettings GetNavigationSettings(this GridView gridView)
        {
            GridViewNavigationSettings settings = new GridViewNavigationSettings();

            settings.PageIndex = gridView.PageIndex;
            settings.SortExpression = gridView.SortExpression;
            settings.SortDirection = gridView.SortDirection;

            return settings;
        }

        public static void SetNavigationSettings(this GridView gridView, GridViewNavigationSettings settings)
        {
            gridView.Sort(settings.SortExpression, settings.SortDirection);
            gridView.PageIndex = settings.PageIndex;
        }

        public static void SetSelectedKey(this GridView gridView, int key)
        {
            gridView.SelectedIndex = gridView.DataKeys.Cast<DataKey>().ToList()
                                                      .FindIndex(dk => (int)dk.Value == key);
        }

        public static void FocusOnFirst(this IEnumerable<Control> controls, Func<Control, bool> cond)
        {
            Control first = controls.FirstOrDefault(c => cond(c));
            if (first != null)
                SetFocus(first);
        }

        public static void FocusOnFirstVisible(this IEnumerable<Control> controls)
        {
            controls.FocusOnFirst(c => c.Visible);
        }

        public static void FocusOnFirstEnabled(this IEnumerable<Control> controls)
        {
            controls.FocusOnFirst(c => c.Visible && c is WebControl && ((WebControl)c).Enabled);
        }

        public static void SetFocus(Control control)
        {
            control.Focus();
        }

        /// <summary>
        /// Also returns true for Google Chrome, which uses a related rendering engine.
        /// </summary>
        public static bool BrowserIsSafari
        {
            get { return HttpContext.Current.Request.Browser.Browser.ToLower().IndexOf("safari") >= 0; }
        }

        /// <summary>
        /// For customer logins, this will make the page load faster immediately after Session state has expired.
        /// </summary>
        public static void AddParameterContactId(ObjectDataSource objectDataSource, bool isCustomerLogin)
        {
            Parameter param = isCustomerLogin ? new Parameter("contactId", TypeCode.Int32, "0")
                                              : new SessionParameter("contactId", TypeCode.Int32, "ContactId");

            objectDataSource.SelectParameters.Add(param);
        }
    } 
}

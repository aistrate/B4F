using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using System.Collections;
using System.Reflection;
using System.Globalization;
using B4F.Web.WebControls;
using B4F.TotalGiro.Utils;

public static class Utility
{
    private const string NAMESESSION = "NameSession";

    private static Hashtable lastErrorByUser = new Hashtable();

    public static UnhandledError LastUnhandledError
    {
        get { return (UnhandledError)lastErrorByUser[currentUser]; }

        set { lastErrorByUser[currentUser] = value; }
    }

    private static string currentUser
    {
        get { return HttpContext.Current.User.Identity.Name; }
    }

    public static int GetCurrentRowKey(LinkButton button)
    {
        return GetCurrentRowKey(button, true);
    }

    public static int GetCurrentRowKey(LinkButton button, bool unSelectRow)
    {
        int key = int.MinValue;
        GridViewRow gvRow = button.Parent.Parent as GridViewRow;
        if (gvRow != null && gvRow.RowType == DataControlRowType.DataRow)
        {
            GridView gridView = (GridView)gvRow.Parent.Parent;
            gridView.SelectedIndex = gvRow.RowIndex;
            key = (int)gridView.SelectedDataKey.Value;
            if (unSelectRow)
                gridView.SelectedIndex = -1;
            else
                gridView.SelectedIndex = gvRow.RowIndex;
        }        
        return key;
    }

    public static int GetCurrentRowKey(GridView gvView)
    {
        int key = int.MinValue;
        if (gvView.SelectedIndex > -1 && Util.IsNumeric(gvView.SelectedDataKey.Value))
            key = Convert.ToInt32(gvView.SelectedDataKey.Value);
        return key;
    }

    public static int FindGridRowIndex(GridView gridView, int key)
    {
        int rowIndex = -1;
        for (int i = 0; i < gridView.DataKeys.Count; i++)
            if ((int)gridView.DataKeys[i].Value == key)
                rowIndex = i;
        return rowIndex;
    }

    public static int GetKeyFromDropDownList(DropDownList ctlDDL)
    {
        int key = int.MinValue;
        if (ctlDDL != null && ctlDDL.SelectedIndex > -1)
        {
            if (int.TryParse(ctlDDL.SelectedValue, out key))
                return key;
        }
        return key;
    }

    // the return value indicates whether the row will remain selected
    public delegate bool RowCommandHandler(string commandName, int key);

    public static void ProcessRowCommand(object commandSource, string commandName, RowCommandHandler rowCommandHandler)
    {
        if (commandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)commandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                GridViewRow gridViewRow = (GridViewRow)tableRow;
                GridView gridView = (GridView)gridViewRow.Parent.Parent;
                
                // Select row
                gridView.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int key = (int)gridView.SelectedDataKey.Value;

                bool selected = rowCommandHandler(commandName, key);

                if (!selected)
                    gridView.SelectedIndex = -1;
            }
        }
    }

    public static string GetCompleteExceptionMessage(Exception ex)
    {
        if (ex is System.Reflection.TargetInvocationException && ex.InnerException != null)
            return GetCompleteExceptionMessage(ex.InnerException);
        else
            return ex.Message + "<br/>" + (ex.InnerException != null ? GetCompleteExceptionMessage(ex.InnerException) : "");
    }

    public static void EnableGridView(GridView gridView, bool enabled, params int[] affectedColumns)
    {
        gridView.HeaderRow.Enabled = enabled;
        gridView.BottomPagerRow.Enabled = enabled;

        bool multipleSelection = (gridView is MultipleSelectionGridView && ((MultipleSelectionGridView)gridView).MultipleSelection);
        
        foreach (GridViewRow row in gridView.Rows)
            if (row.RowType == DataControlRowType.DataRow)
            {
                if (multipleSelection)
                    row.Cells[0].Enabled = enabled;
                foreach (int columnIndex in affectedColumns)
                    row.Cells[columnIndex].Enabled = enabled;
            }
    }

    public static Control FindControl(Control parent, string controlId)
    {
        Control control = null;

        if (parent != null)
            control = parent.FindControl(controlId);

        if (control != null)
            return control;
        else
            throw new ArgumentException(string.Format("Could not find control with id '{0}'.", controlId));
    }

    public static void CreatePrevPageSessionWithPageName(string previousPage)
    {
        string sessPathToPrevPage = HttpContext.Current.Request.Url.AbsolutePath;
        string sessNamePathToPrevPage = Utility.NAMESESSION + sessPathToPrevPage;
        if (HttpContext.Current.Session[sessNamePathToPrevPage] == null)
        {
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                string refererPagePath = HttpContext.Current.Request.UrlReferrer.AbsolutePath;
                if (previousPage.Equals(string.Empty))
                {
                    if (!sessPathToPrevPage.Equals(refererPagePath) && refererPagePath.ToUpper().IndexOf("LOGIN.ASPX") < 1)
                        HttpContext.Current.Session[sessNamePathToPrevPage] = refererPagePath;
                }
                else 
                {
                    if (!sessNamePathToPrevPage.Equals(refererPagePath) && refererPagePath.ToUpper().IndexOf(previousPage.ToUpper()) > 0)
                        HttpContext.Current.Session[sessNamePathToPrevPage] = refererPagePath;
                }
            }
        }
    }

    public static void CreatePrevPageSession()
    {
        Utility.CreatePrevPageSessionWithPageName(string.Empty);
    }

    public static void NavigateToPrevPageSessionIfAnyWithQueryString(string queryString)
    {
        string sessNamePathToPrevPage = Utility.NAMESESSION + HttpContext.Current.Request.Url.AbsolutePath;
        if (HttpContext.Current.Session[sessNamePathToPrevPage] != null)
        {
            string path = HttpContext.Current.Session[sessNamePathToPrevPage].ToString() + queryString;
            HttpContext.Current.Session[sessNamePathToPrevPage] = null;
            HttpContext.Current.Response.Redirect(path);
        }
    }

    public static void NavigateToPrevPageSessionIfAny()
    {
        Utility.NavigateToPrevPageSessionIfAnyWithQueryString(string.Empty);
    }

    public static void DisablePageCaching()
    {
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        HttpContext.Current.Response.Cache.SetNoStore();
        HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");
    }

    public static void AlertSaveMessage()
    {
        if (HttpContext.Current.Session["blnSaveSuccess"] != null)
        {
            bool blnSaveSuccess = Convert.ToBoolean(HttpContext.Current.Session["blnSaveSuccess"]);
            string alertMess = "The data is successfully saved.";

            if (blnSaveSuccess)
                alertMess = "The data is successfully saved.";
            else
                alertMess = "The data is NOT saved successfully.";

            Page executingPage = HttpContext.Current.Handler as Page;
            executingPage.RegisterHiddenField("hfMessSaveSucceed", alertMess);

            HttpContext.Current.Session["blnSaveSuccess"] = null;

        }
    }

    public static string DecimalSeparator()
    {
        return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
    }

    public static string NumberGroupSeparator()
    {
        return CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
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

    public static void SetDefaultButton(Page page, UserControl control, Button defaultButton)
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

    public static void ExportToExcel(HttpResponse response, DataSet ds)
    {
        NumberFormatInfo numInfo = Util.GetYankeeNumberFormat();
        DataTable dt = ds.Tables[0];

        response.ClearContent();
        response.ContentEncoding = System.Text.Encoding.UTF8;
        response.ContentType = "application/vnd.ms-excel";
        string tab = "";
        foreach (DataColumn dc in dt.Columns)
        {
            response.Write(tab + dc.ColumnName);
            tab = "\t";
        }
        response.Write("\n");
        int i;
        foreach (DataRow dr in dt.Rows)
        {
            tab = "";
            for (i = 0; i < dt.Columns.Count; i++)
            {
                switch (Util.GetMainTypeCode(dr[i].GetType()))
                {
                    case MainTypeCodes.Number:
                        response.Write(tab + ((decimal)dr[i]).ToString("0.00#####", numInfo));
                        break;
                    case MainTypeCodes.DateTime:
                        response.Write(tab + ((DateTime)dr[i]).ToString("yyyy-MM-dd"));
                        break;
                    default:
                        response.Write(tab + dr[i].ToString());
                        break;
                }
                tab = "\t";
            }
            response.Write("\n");
        }
        response.End();
    }
}

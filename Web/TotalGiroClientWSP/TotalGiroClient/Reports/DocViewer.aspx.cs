using System;
using System.Collections.Specialized;
using System.Web;
using B4F.TotalGiro.ClientApplicationLayer.Reports;

public partial class DocViewer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int documentId;
        bool isDownload;
        readQueryParams(out documentId, out isDownload);
        
        string fileName;
        byte[] documentContents = DocViewerAdapter.GetDocumentContent(documentId, out fileName);
        
        Response.ContentType = "application/pdf";
        if (isDownload)
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(documentContents);
    }

    private void readQueryParams(out int documentId, out bool isDownload)
    {
        documentId = 0;
        isDownload = false;

        string rawUrl = HttpContext.Current.Request.RawUrl;
        int indexQuery = rawUrl.IndexOf('?');
        if (indexQuery >= 0 && indexQuery < rawUrl.Length - 1)
        {
            NameValueCollection pageParams = HttpUtility.ParseQueryString(rawUrl.Substring(indexQuery + 1));
            foreach (string paramKey in pageParams.AllKeys)
                if (paramKey.ToLower() == "id")
                    int.TryParse(pageParams[paramKey], out documentId);
                else if (paramKey.ToLower() == "download")
                    bool.TryParse(pageParams[paramKey], out isDownload);
        }
    }
}

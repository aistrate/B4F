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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class ImportFiles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Import third party files";
        }

   }

    protected void btnImportExchangeRates_Click(object sender, EventArgs e)
    {
        ImportFilesAdapter.ImportExchangeRates(DateTime.MinValue);
        
        Server.Transfer("ImportFiles.aspx");
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ExactJournalDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";

        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Create/Edit Exact Journal Details";
            //GLJournalID = (Session["glJournalID"] != null ? (int)Session["glJournalID"] : 0);
            //loadDetails();
            DataBind();

        }

    }
}

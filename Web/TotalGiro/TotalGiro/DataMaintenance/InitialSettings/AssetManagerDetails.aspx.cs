using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.InitialSettings;

public partial class AssetManagerDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Create/Edit Asset Manager Details";
                AssetManagerID = (Session["assetManagerID"] != null ? (int)Session["assetManagerID"] : 0);
                loadDetails();
                DataBind();

            }
        }
        catch (Exception ex)
        {
            //pnlErrorMess.Visible = true;
            lblMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    public int AssetManagerID
    {
        get
        {
            object i = ViewState["AssetManagerID"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["AssetManagerID"] = value; }
    }

    public string Name
    {
        get { return this.txtName.Text; }
        set { this.txtName.Text = value; }
    }

    public string TradingAccountName
    {
        get { return this.txtTradingAccount.Text; }
        set { this.txtTradingAccount.Text = value; }
    }

    public string NostroAccountName
    {
        get { return this.txtNostroAccountName.Text; }
        set { this.txtNostroAccountName.Text = value; }
    }

    public string Initials
    {
        get { return this.txtInitials.Text; }
        set { this.txtInitials.Text = value; }
    }

    public bool IsActive
    {
        get { return this.chkIsActive.Checked; }
        set { this.chkIsActive.Checked = value; }
    }

    public bool SupportLifecycles
    {
        get { return this.chkSupportLifecycles.Checked; }
        set { this.chkSupportLifecycles.Checked = value; }
    }

    private void loadDetails()
    {
        if (AssetManagerID != 0)
        {
            AssetManagerAdapter.AssetManagerRecordDetails details = AssetManagerAdapter.GetAssetManagerDetails(this.AssetManagerID);
            this.AssetManagerID = details.Key;
            this.Name = details.Name;
            this.TradingAccountName = details.TradingAccount;
            this.NostroAccountName = details.NostroAccount;
            this.Initials = details.Initials;
            this.IsActive = details.IsActive;
            this.SupportLifecycles = details.SupportLifecycles;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            AssetManagerAdapter.AssetManagerRecordDetails details = new AssetManagerAdapter.AssetManagerRecordDetails(this.AssetManagerID);
            details.Key = this.AssetManagerID;
            details.Name = this.Name ;
            details.Initials = this.Initials;
            details.IsActive = this.IsActive;
            details.SupportLifecycles = this.SupportLifecycles;
            
            this.AssetManagerID =  AssetManagerAdapter.SaveManagerDetails(details);

            loadDetails();

        }
        catch (Exception ex)
        {
            lblMessage.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }
}

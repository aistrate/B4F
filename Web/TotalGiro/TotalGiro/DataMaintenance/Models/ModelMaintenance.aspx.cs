using System;
using System.Collections;
using System.Collections.Generic;
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
using B4F.TotalGiro.Stichting;
using System.Globalization;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Security;
using B4F.Web.WebControls;
using B4F.TotalGiro.Utils;

public partial class ModelMaintenance : System.Web.UI.Page
{
    protected enum EDITMODE
    {
        None = 0,
        Edit,
        Insert,
        Both
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        ctlModelFinder.Search += new EventHandler(ctlModelFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Model Maintenance";
                gvModels.Sort("ModelName", SortDirection.Ascending);
                gvModelVersions.Sort("VersionNumber", SortDirection.Descending);
                gvModelComponents.Sort("ComponentName", SortDirection.Ascending);
                gvModelInstruments.Sort("Component_Name", SortDirection.Ascending);
                setupModelComponentList();
                if (B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.IsLoggedInAsAssetManager())
                {
                    IAssetManager am = B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.getAssetManager();
                    if (am != null)
                    {
                        this.AssetManagerId = am.Key;
                        this.AssetManagerName = am.CompanyName;
                        this.gvModels.Caption = "Current Models available to " + am.CompanyName;
                    }

                    ILogin loginDetails = B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.GetLoginDetails();
                    if (loginDetails != null)
                    {
                        this.LoginId = loginDetails.Key;
                        this.LoginName = loginDetails.UserName;
                    }
                    btnCreateCR.Enabled = SecurityManager.IsCurrentUserInRole("Calculation Rules Mtce");
                }
                ModelID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "ModelID");
                if (ModelID != 0)
                {
                    string name = ModelMaintenanceAdapter.GetModel(ModelID).Get(x => x.ModelName);
                    if (!string.IsNullOrEmpty(name))
                    {
                        ctlModelFinder.ModelName = name;
                        ctlModelFinder.ActiveStatus = (int)B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.All;
                        ctlModelFinder_Search(sender, e);
                        if (gvModels.Rows.Count > 0)
                        {
                            gvModels.SelectedIndex = 0;
                            setupEditMode(false, EDITMODE.Edit);
                        }
                    }
                }
                ViewState["InsertFeeRuleMode"] = false;
                ViewState["InsertModelBenchMarkPerformanceMode"] = false;
            }
            lblErrorMessage.Text = "";
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvModels_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((string)((DataRowView)e.Row.DataItem)["AssetManager_CompanyName"]) == AssetManagerName)
            {
                e.Row.FindControl("lbtEditModel").Visible = true;
            }
            //if (Request.Params["ModelID"] != null)
            //{
            //    if (((DataRowView)(e.Row.DataItem)).Row.ItemArray[0].ToString().Equals(Request.Params["ModelID"]))
            //        gvModels.SelectedIndex = e.Row.RowIndex;
            //}
        }
    }

    private void initialSetup()
    {
        InsertFeeRuleMode = false;
        InsertModelBenchMarkPerformanceMode = false;
        this.mlvVersion.ActiveViewIndex = 0;
        this.gvModels.DataBind();
        this.gvModels.SelectedIndex = -1;
        this.gvModelVersions.Visible = false;
        this.gvModelComponents.Visible = false;
        this.gvModelInstruments.Visible = false;
        this.CurrentEditMode = EDITMODE.None;

    }

    protected void gvModels_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.mlvVersion.ActiveViewIndex = 0;
        this.CurrentModelName = ((GridView)sender).SelectedRow.Cells[0].Text.ToString();
        this.gvModelVersions.Caption = "List of Versions for " + CurrentModelName;

        this.gvModelVersions.Visible = true;
        this.gvModelComponents.Visible = false;
        this.gvModelInstruments.Visible = false;
    }

    protected void gvModelVersions_SelectedIndexChanged(object sender, EventArgs e)
    {
        string versionNumber = ((GridView)sender).SelectedRow.Cells[0].Text.ToString();

        this.gvModelComponents.Visible = true;
        this.gvModelComponents.Caption = "List of Component Allocations for " + CurrentModelName + " Version : " + versionNumber;

        this.gvModelInstruments.Visible = true;
        this.gvModelInstruments.Caption = "List of Instrument Allocations for " + CurrentModelName + " Version : " + versionNumber;

    }

    protected void gvModels_PageIndexChanged(object sender, EventArgs e)
    {
        this.gvModelVersions.Visible = false;
        this.gvModelComponents.Visible = false;
        this.gvModelInstruments.Visible = false;
    }

    protected void gvModelVersions_PageIndexChanged(object sender, EventArgs e)
    {
        this.gvModelComponents.Visible = false;
        this.gvModelInstruments.Visible = false;
    }

    protected void gvModels_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvModels.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                if (this.CurrentEditMode == EDITMODE.None)
                {
                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITMODEL":
                            setupEditMode(false, EDITMODE.Edit);
                            return;
                    }
                }
            }
        }
        gvModels.SelectedIndex = -1;
    }

    protected void gvModelEditComponents_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            DecimalBox dbAllocation = (DecimalBox)(gvModelEditComponents.Rows[e.RowIndex].FindControl("dbAllocation"));
            decimal newTotal = getSumAllocations((int)e.Keys[0], dbAllocation.Value);
            //if (newTotal > 1M)
            //    e.Cancel = true;
            //else
            //{
                e.NewValues["allocation"] = dbAllocation.Value;
            //}
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvModelEditComponents_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        try
        {
            sumAllocations();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCommissionRules_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    // Select row
                    gvCommissionRules.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int ruleId = (int)gvCommissionRules.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITRULE":
                            gvCommissionRules.SelectedIndex = -1;
                            string qStr = QueryStringModule.Encrypt(string.Format("id={0}&ModelID={1}", ruleId, ModelID));
                            Response.Redirect(string.Format("~/DataMaintenance/Commission/RuleEdit.aspx{0}", qStr));
                            break;
                    }
                }
            }
            gvCommissionRules.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCommissionRules_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnEditCommissionRule = (LinkButton)e.Row.FindControl("lbtnEditCommissionRule");
            if (!SecurityManager.IsCurrentUserInRole("Calculation Rules Mtce"))
                lbtnEditCommissionRule.Enabled = false;
        }
    }

    protected void gvFeeRules_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            YearMonthPicker ppEndPeriod = (YearMonthPicker)(gvFeeRules.Rows[e.RowIndex].FindControl("ppEndPeriod"));
            e.NewValues["endPeriod"] = ppEndPeriod.SelectedPeriod;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateCR_Click(object sender, EventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("ModelID={0}", ModelID));
            Response.Redirect(string.Format("~/DataMaintenance/Commission/RuleEdit.aspx{0}", qStr));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtnDelete_Command(object sender, CommandEventArgs e)
    {
        try
        {
            deleteComponentLine(getRowIndex(gvModelEditComponents, (string)e.CommandArgument));
            gvModelEditComponents.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    //protected void lbtnEditFeeRule_Command(object sender, CommandEventArgs e)
    //{
    //    try
    //    {
    //        gvFeeRules.EditIndex = getRowIndex(gvFeeRules, (string)e.CommandArgument);
    //    }
    //    catch (Exception ex)
    //    {
    //        lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
    //    }
    //}

    protected void cvEndPeriod_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = !(ppEndPeriod.SelectedPeriod > 0 && (ppStartPeriod.SelectedPeriod == 0 || ppStartPeriod.SelectedPeriod >= ppEndPeriod.SelectedPeriod));
    }

    protected int getRowIndex(GridView gv, string key)
    {
        int rowIndex = -1;
        for (int i = 0; i < gv.DataKeys.Count; i++)
            if (gv.DataKeys[i].Value.ToString() == key)
                rowIndex = i;
        return rowIndex;
    }

    private void deleteComponentLine(int index)
    {
        try
        {
            string oldCashFundAlternativeKey = this.ddlCashFundAlternative.SelectedValue;
            ModelComponentHelper component = this.NewModelVersion[index];
            this.NewModelVersion.RemoveAt(index);
            //this.gvModelEditComponents.DataSource = this.NewModelVersion;
            this.gvModelEditComponents.DataBind();
            fillCashFundAlternativeList();

            if (oldCashFundAlternativeKey.Equals(component.ComponentID.ToString()))
                this.ddlCashFundAlternative.SelectedValue = int.MinValue.ToString();
            else
                this.ddlCashFundAlternative.SelectedValue = oldCashFundAlternativeKey;

            this.gvModelEditComponents.Visible = true;
            sumAllocations();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private decimal getSumAllocations()
    {
        return getSumAllocations(int.MinValue, 0M);
    }

    private decimal getSumAllocations(int key, decimal newValue)
    {
        decimal sum = 0M;
        if (this.NewModelVersion != null)
        {
            foreach (ModelComponentHelper dr in this.NewModelVersion)
            {
                if (key > int.MinValue && dr.Key == key)
                    sum += newValue;
                else
                    sum += dr.Allocation;
            }
        }
        return sum;
    }

    private void sumAllocations()
    {
        this.TotalAllocations = getSumAllocations();
        this.tbAllocations.Text = this.TotalAllocations.ToString("P5", CultureInfo.CreateSpecificCulture("nl-NL"));
    }

    protected void ctlModelFinder_Search(object sender, EventArgs e)
    {
        gvModels.DataBind();
    }

    protected void btnAddNewComponentLine_Click(object sender, EventArgs e)
    {
        try
        {
            this.pnlEditComponent.Visible = true;
            this.btnAddNewComponentLine.Enabled = false;
            this.btnUpdateModel.Enabled = false;
            this.btnCancelAddNewComponentLine.Visible = true;
            this.gvModelEditComponents.Enabled = false;
            this.ddlComponentType.SelectedValue = ((int)ModelComponentType.Instrument).ToString();
            setupComponentChoice(((int)ModelComponentType.Instrument).ToString());
            if (this.TotalAllocations < 1m)
                this.tbAddAllocation.Text = (Decimal.Subtract(1m, TotalAllocations)).ToString("G5", CultureInfo.CreateSpecificCulture("nl-NL"));
            else
                this.tbAddAllocation.Text = "0,00000";

            int cashManagementFundKey = InstrumentEditAdapter.GetCashManagementFundKey();
            if (cashManagementFundKey != 0)
                this.ddlComponent.SelectedValue = cashManagementFundKey.ToString();
            else
            {
                if (this.ddlComponent.Items != null && this.ddlComponent.Items.Count > 0)
                    this.ddlComponent.SelectedValue = this.ddlComponent.Items[0].Value;
            }
            CurrentEditMode = EDITMODE.Both;
            string oldCashFundAlternativeKey = this.ddlCashFundAlternative.SelectedValue;
            fillCashFundAlternativeList();
            this.ddlCashFundAlternative.SelectedValue = oldCashFundAlternativeKey;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancelAddNewComponentLine_Click(object sender, EventArgs e)
    {
        setupEditMode(false, this.CurrentEditMode);
    }

    protected void btnCreateNewModel_Click(object sender, EventArgs e)
    {
        setupEditMode(true, EDITMODE.Insert);
    }

    protected void btnCancelChanges_Click(object sender, EventArgs e)
    {
        setupEditMode(false, EDITMODE.None);
        initialSetup();
    }

    protected void btnUpdateModel_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (Page.IsValid)
            {
                ModelVersionHelper mvh = new ModelVersionHelper();
                mvh.AssetManagerId = AssetManagerId;
                mvh.ModelID = ModelID;
                mvh.ModelName = this.tbModelName.Text;
                mvh.IsSubModel = this.chkIsSubModel.Checked;
                mvh.ModelShortName = this.tbModelShortName.Text;
                mvh.Description = this.tbDescription.Text;
                mvh.Notes = this.tbNotes.Text;
                mvh.IsPublic = this.chkIsPublic.Checked;
                mvh.IsActive = this.chkIsActive.Checked;
                mvh.ExpectedReturn = this.dbExpectedReturn.Value / 100m;
                mvh.CashFundAlternativeID = Convert.ToInt32(ddlCashFundAlternative.SelectedValue);
                mvh.ModelDetailID = Convert.ToInt32(ddlModelDetail.SelectedValue);
                mvh.Components = this.NewModelVersion;
                mvh.ExecutionOptions = this.ExecutionOption;

                ModelMaintenanceAdapter.UpdateVersion(mvh);
                initialSetup();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnUpdateNewComponentLine_Click(object sender, EventArgs e)
    {
        try
        {
            //if (getSumAllocations() + this.tbAddAllocation.Value > 1M)
            //{
            //    lblErrorMessage.Text = "The model can not be more than 100% Allocated.";
            //    return;
            //}

            if (this.NewModelVersion == null)
                this.NewModelVersion = new List<ModelComponentHelper>();
                ModelComponentHelper dr = ModelMaintenanceAdapter.GetModelComponentHelper(ModelID,
                (ModelComponentType)(int.Parse(this.ddlComponentType.SelectedValue)),
                int.Parse(ddlComponent.SelectedValue), 
                this.NewModelVersion.Count, 
                this.tbAddAllocation.Value);

            //Check if already Present
            int check = checkIndexOfEntry(dr.ModelComponentType, dr.ComponentID);
            if (check == -1)
            {
                this.NewModelVersion.Add(dr);
            }
            else
            {
                (this.NewModelVersion[check]).Allocation += dr.Allocation;
            }
            string oldCashFundAlternativeKey = this.ddlCashFundAlternative.SelectedValue;
            fillCashFundAlternativeList();
            this.ddlCashFundAlternative.SelectedValue = oldCashFundAlternativeKey;
            setupEditMode(false, CurrentEditMode);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateFeeRule_Click(object sender, EventArgs e)
    {
        try
        {
            if (!InsertFeeRuleMode)
                InsertFeeRuleMode = true;
            else
            {
                rfvFeeCalculation.Validate();
                cvEndPeriod.Validate();
                if (rfvFeeCalculation.IsValid && cvEndPeriod.IsValid)
                {
                    if (ModelMaintenanceAdapter.CreateModelFeeRule(
                        ModelID,
                        Convert.ToInt32(ddlFeeCalculation.SelectedValue),
                        chkExecutionOnly.Checked,
                        chkSendByPost.Checked,
                        ppStartPeriod.SelectedPeriod,
                        ppEndPeriod.SelectedPeriod))
                    {
                        gvFeeRules.DataBind();
                        InsertFeeRuleMode = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancelCreateFeeRule_Click(object sender, EventArgs e)
    {
        InsertFeeRuleMode = false;
    }

    private int checkIndexOfEntry(ModelComponentType modelComponentType, int componentID)
    {
        int returnValue = -1;
        for (int i = 0; i < this.NewModelVersion.Count; i++)
        {
            ModelComponentHelper dr = this.NewModelVersion[i];
            if ((dr.ModelComponentType == modelComponentType) && (dr.ComponentID == componentID))
            {
                returnValue = i;
            }
        }
        return returnValue;
    }

    protected void ddlModelDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlModelDetail.SelectedIndex > -1 && ddlModelDetail.SelectedValue != int.MinValue.ToString())
            {
                bool inclCashFund = ModelMaintenanceAdapter.IsModelDetailInclCashManagementFund(Convert.ToInt32(ddlModelDetail.SelectedValue));
                if (inclCashFund)
                    ddlCashFundAlternative.SelectedIndex = -1;
                ddlCashFundAlternative.Enabled = !inclCashFund;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void ddlComponentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        setupComponentChoice(((DropDownList)sender).SelectedValue);
    }

    private void setupComponentChoice(string selectedValue)
    {
        switch (selectedValue)
        {

            case "0":
                this.ddlComponent.DataSourceID = "odsAddModel";
                this.ddlComponent.DataTextField = "ModelName";
                this.ddlComponent.DataValueField = "Key";
                break;
            case "1":
                this.ddlComponent.DataSourceID = "odsAddInstrument";
                this.ddlComponent.DataTextField = "Name";
                this.ddlComponent.DataValueField = "Key";
                break;
            default:
                break;

        }
        this.ddlComponent.DataBind();
    }

    private void setupEditMode(bool newModel, EDITMODE currentEditMode)
    {
        if (CurrentEditMode == EDITMODE.None)
        {
            this.CurrentModelName = "";
            this.ModelID = 0;
            this.tbVersion.Text = "1";
            this.ExecutionOption = ExecutionOnlyOptions.Allowed;
            this.tbModelShortName.Text = "";
            this.tbDescription.Text = "";
            this.chkIsActive.Checked = true;
            this.chkIsActive.Enabled = false;
            this.dbExpectedReturn.Text = "";
            this.ddlModelDetail.SelectedValue = int.MinValue.ToString();
            this.tbNotes.Text = "";
            if (this.gvModels.SelectedValue != null)
            {
                this.ModelID = (int)this.gvModels.SelectedValue;
                this.NewModelVersion = B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.GetModelInstrumentsforLatestModel(this.ModelID);
                fillCashFundAlternativeList();
            }

            if (!newModel)
            {
                this.ModelID = (int)this.gvModels.SelectedValue;
                IModelBase mb = B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.GetModel(this.ModelID);
                this.CurrentModelName = mb.ModelName;
                this.IsPublic = mb.IsPublic;
                this.tbVersion.Text = (mb.LatestVersion.VersionNumber + 1).ToString();
                if (mb.ModelType == ModelType.PortfiolioModel)
                {
                    this.ExecutionOption = ((IPortfolioModel)mb).ExecutionOptions;
                    if (((IPortfolioModel)mb).Details != null)
                    {
                        this.ddlModelDetail.SelectedValue = ((IPortfolioModel)mb).Details.Key.ToString();
                        ddlModelDetail_SelectedIndexChanged(null, null);
                    }
                    if (((IPortfolioModel)mb).CashFundAlternative != null)
                    {
                        string key = ((IPortfolioModel)mb).CashFundAlternative.Key.ToString();
                        if (this.ddlCashFundAlternative.Items == null || this.ddlCashFundAlternative.Items.Count == 0)
                            this.ddlCashFundAlternative.DataBind();
                        if (this.ddlCashFundAlternative.Items.FindByValue(key) != null)
                            this.ddlCashFundAlternative.SelectedValue = key;
                    }
                }
                this.tbModelShortName.Text = mb.ShortName;
                this.chkIsSubModel.Checked = mb.IsSubModel;
                this.tbDescription.Text = mb.Description;
                this.tbNotes.Text = mb.ModelNotes;
                this.chkIsActive.Checked = mb.IsActive;
                this.chkIsActive.Enabled = true;
                if (mb.ExpectedReturn != 0m)
                    this.dbExpectedReturn.Value = mb.ExpectedReturn * 100m;
                else
                    this.dbExpectedReturn.Clear();
                this.btnCreateCR.Enabled = true;
                this.btnCreateFeeRule.Enabled = true;
                this.btnAddNewModelPerformanceLine.Enabled = true;
            }
            else
            {
                this.btnCreateCR.Enabled = false;
                this.btnCreateFeeRule.Enabled = false;
                this.btnAddNewModelPerformanceLine.Enabled = false;
            }

            this.tbModelName.Text = this.CurrentModelName;
            //this.tbVersion.Text = B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter.GetNextVersionforModel(this.ModelID).ToString();
            CurrentEditMode = currentEditMode;
            this.ddlExecutionOption.SelectedValue = ((int)this.ExecutionOption).ToString();
        }

        CurrentEditMode = currentEditMode;
        switch (CurrentEditMode)
        {
            case EDITMODE.Edit:
                btnCreateNewModel.Enabled = false;
                ctlModelFinder.Enabled = false;
                break;
            case EDITMODE.Insert:
                btnCreateNewModel.Enabled = false;
                ctlModelFinder.Enabled = false;
                this.NewModelVersion = null;
                fillCashFundAlternativeList();
                break;
            case EDITMODE.Both:
                break;
            default:
                btnCreateNewModel.Enabled = true;
                ctlModelFinder.Enabled = true;
                break;
        }

        mlvVersion.ActiveViewIndex = 1;

        this.gvModelEditPerformances.Enabled = true;
        this.gvModelEditPerformances.DataBind();

        this.gvModelEditComponents.Enabled = true;
        //this.gvModelEditComponents.DataSource = this.NewModelVersion;
        this.gvModelEditComponents.DataBind();

        this.btnUpdateModel.Enabled = true;
        this.btnAddNewComponentLine.Enabled = true;
        this.pnlEditComponent.Visible = false;
        sumAllocations();
    }

    private void fillCashFundAlternativeList()
    {
        ddlCashFundAlternative.Items.Clear();
        if (this.NewModelVersion != null && this.NewModelVersion.Count > 1)
        {
            if (this.NewModelVersion.Where(u => u.IsCashManagementFund).Count() == 0)
            {
                var ds = this.NewModelVersion
                    .SelectMany(x => x.Instruments)
                    .GroupBy(x => x.Key)
                    .OrderBy(x => x.First().Value)
                    .Select(c => new
                    {
                        Key = c.Key,
                        Name = c.First().Value
                    })
                    .ToList();
                this.ddlCashFundAlternative.DataSource = ds;
                this.ddlCashFundAlternative.DataBind();
            }
            else
                ddlModelDetail.SelectedValue = ((int)CashManagementFundOptions.Included).ToString();
        }
        this.ddlCashFundAlternative.Items.Insert(0, new ListItem("", int.MinValue.ToString()));
    }

    private void setupModelComponentList()
    {
        this.ddlComponentType.Items.Add(new ListItem("Instrument", ((int)ModelComponentType.Instrument).ToString()));
        this.ddlComponentType.Items.Add(new ListItem("Model", ((int)ModelComponentType.Model).ToString()));
        setupComponentChoice(((int)ModelComponentType.Instrument).ToString());
    }

    protected int AssetManagerId
    {
        get
        {
            object i = ViewState["AssetManagerId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["AssetManagerId"] = value; }
    }

    protected int LoginId
    {
        get
        {
            object i = ViewState["LoginId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["LoginId"] = value; }
    }

    protected Decimal TotalAllocations
    {
        get
        {
            object i = ViewState["TotalAllocations"];
            return ((i == null) ? 0m : (decimal)i);
        }
        set { ViewState["TotalAllocations"] = value; }
    }

    protected string AssetManagerName
    {
        get
        {
            object i = ViewState["AssetManagerName"];
            return ((i == null) ? "" : (string)i);
        }
        set { ViewState["AssetManagerName"] = value; }
    }

    protected string LoginName
    {
        get
        {
            object i = ViewState["LoginName"];
            return ((i == null) ? "" : (string)i);
        }
        set { ViewState["LoginName"] = value; }
    }

    protected string CurrentModelName
    {
        get
        {
            object i = ViewState["CurrentModelName"];
            return ((i == null) ? "" : (string)i);
        }
        set
        {
            ViewState["CurrentModelName"] = value;
        }
    }

    protected List<ModelComponentHelper> NewModelVersion
    {
        get
        {
            object i = Session["NewModelVersion"];
            return ((i == null) ? null : (List<ModelComponentHelper>)i);
        }
        set 
        { 
            Session["NewModelVersion"] = value;
            ModelComponentHelper.modelComponents = value;
        }
    }

    protected Int32 ModelID
    {
        get
        {
            object i = ViewState["ModelID"];
            return ((i == null) ? 0 : (Int32)i);
        }
        set { ViewState["ModelID"] = value; }
    }

    protected bool IsPublic
    {
        get
        {
            object i = ViewState["IsPublic"];
            return ((i == null) ? false : (bool)i);
        }
        set { ViewState["IsPublic"] = value; }
    }

    protected EDITMODE CurrentEditMode
    {
        get
        {
            object i = ViewState["CurrentEditMode"];
            return ((i == null) ? EDITMODE.None : (EDITMODE)i);
        }
        set { ViewState["CurrentEditMode"] = value;
        gvModels.Enabled = (value == EDITMODE.None);

        }
    }

    private bool InsertFeeRuleMode
    {
        get
        {
            object t = ViewState["InsertFeeRuleMode"];
            return ((t == null) ? false : (bool)t);
        }
        set
        {
            pnlFeeRuleEntry.Visible = value;
            btnCancelCreateFeeRule.Visible = value;
            
            btnUpdateModel.Enabled = !value;
            gvFeeRules.Enabled = !value;
            btnCreateCR.Enabled = !value;
            gvCommissionRules.Enabled = !value;

            this.btnAddNewComponentLine.Enabled = !value;
            this.btnUpdateModel.Enabled = !value;

            this.gvModelEditComponents.Enabled = !value;
            this.btnCreateFeeRule.Focus();
            ViewState["InsertFeeRuleMode"] = value;
        }
    }

    protected ExecutionOnlyOptions ExecutionOption
    {
        get
        {
            object i = ViewState["ExecutionOption"];
            return ((i == null) ? ExecutionOnlyOptions.NotAllowed : (ExecutionOnlyOptions)i);
        }
        set { ViewState["ExecutionOption"] = value; }
    }

    protected void customTotalAllocations_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = this.TotalAllocations.Equals(1m);
    }

    // Model BenchMark Performances
    protected void gvModelEditPerformances_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            DecimalBox dbBenchMarkValue = (DecimalBox)(gvModelEditPerformances.Rows[e.RowIndex].FindControl("dbBenchMarkValue"));
            e.NewValues["benchmarkvalue"] = dbBenchMarkValue.Value;
        }

        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnAddNewModelPerformanceLine_Click(object sender, EventArgs e)
    {
        try
        {
            if (!InsertModelBenchMarkPerformanceMode)
            {
                InsertModelBenchMarkPerformanceMode = true;
            }
            else
                InsertModelBenchMarkPerformanceMode = false;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }

    }
    
    protected bool ValidateModelBenchMarkPerformance()
    {
        this.dbCategoryWeighting.Value = this.dbBoxxTarget.Value + this.dbMSCIWorldTarget.Value + this.dbCompositeTarget.Value;

        if (this.dbCategoryWeighting.Value != 100)
        {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = "The composition of these distribution should be 100%.";
            return true;
        }

        if ((ModelMaintenanceAdapter.GetModelPerformances(this.ModelID
                                                    , this.YearOfBenchmarkPerformance.SelectedMonth
                                                    , this.YearOfBenchmarkPerformance.SelectedYear)).Tables[0].Rows.Count != 0)
        {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = "The benchmark already exists. Please select another period.";
            return true;
        }

        this.lblMessage.Visible = false;
        return false;
    }

    protected void btnSaveNewModelPerformance_Click(object sender, EventArgs e)
    {
        try
        {
            if (!InsertModelBenchMarkPerformanceMode)
                InsertModelBenchMarkPerformanceMode = true;        // b4= true
            else
            {
              if (!ValidateModelBenchMarkPerformance())
              {
                  if (ModelMaintenanceAdapter.CreateModelBenchMarkPerformance(
                        this.ModelID
                      , this.dbBoxxTarget.Value
                      , this.dbMSCIWorldTarget.Value
                      , this.dbCompositeTarget.Value
                      , this.dbBenchmarkPerformance.Value
                      , this.YearOfBenchmarkPerformance.SelectedMonth
                      , this.YearOfBenchmarkPerformance.SelectedYear))
                  {
                      this.gvModelEditPerformances.DataBind();
                      InsertModelBenchMarkPerformanceMode = false;
                  }
              }
            }

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancleNewModelPerformance_Click(object sender, EventArgs e)
    {
        InsertModelBenchMarkPerformanceMode = false;
    }

    protected void lbtnEditModelBenchMarkPerformance_Command(object sender, CommandEventArgs e)
    {
        try
        {
            gvModelEditPerformances.EditIndex = getRowIndex(gvModelEditPerformances, (string)e.CommandArgument);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    protected void lbtnDeleteModelBenchMarkPerformanceLine_Command(object sender, CommandEventArgs e)
    {
        try
        {
            bool success = ModelMaintenanceAdapter.DeleteModelBenchMarkPerformance((string)e.CommandArgument);
            if (success)
            {
                this.gvModelEditPerformances.DataBind();
                this.gvModelEditPerformances.Visible = true;
                gvModelEditPerformances.SelectedIndex = -1;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private bool InsertModelBenchMarkPerformanceMode
    {
        get
        {
            object t = ViewState["InsertModelBenchMarkPerformanceMode"];
            return ((t == null) ? false : (bool)t);
        }
        set
        {
            this.dbBoxxTarget.Value = 0;
            this.dbMSCIWorldTarget.Value = 0;
            this.dbCompositeTarget.Value = 0;
            this.dbCategoryWeighting.Value = 0;
            this.dbBenchmarkPerformance.Value = 0;

            this.btnSaveNewModelPerformance.Visible = value;
            this.btnCancleNewModelPerformance.Visible = value;
            this.pnlEditModelBenchmarkPerformance.Visible = value;
            ViewState["InsertModelBenchMarkPerformanceMode"] = value;
        }
    }
}
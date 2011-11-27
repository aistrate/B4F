using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.Settings;
using B4F.TotalGiro.CRM;

public partial class Settings : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        changeableOptions = new Dictionary<CheckBox, OptionView>()
            {
                { cbNotasAndQuarterlyReportsByPost,  new OptionView { Category = SendableDocumentCategories.NotasAndQuarterlyReports,
                                                                      Option   = SendingOptions.ByPost } },

                { cbNotasAndQuarterlyReportsByEmail, new OptionView { Category = SendableDocumentCategories.NotasAndQuarterlyReports, 
                                                                      Option   = SendingOptions.ByEmail } }
            };

        nonChangeableOptions = new Dictionary<CheckBox, OptionView>()
            {
                { cbYearlyReportsByPost,             new OptionView { Category = SendableDocumentCategories.YearlyReports,
                                                                      Option   = SendingOptions.ByPost } },

                { cbYearlyReportsByEmail,            new OptionView { Category = SendableDocumentCategories.YearlyReports,
                                                                      Option   = SendingOptions.ByEmail } }
            };

        allOptions = changeableOptions.Union(nonChangeableOptions).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private Dictionary<CheckBox, OptionView> changeableOptions;
    private Dictionary<CheckBox, OptionView> nonChangeableOptions;
    private Dictionary<CheckBox, OptionView> allOptions;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Instellingen";
            ((TotalGiroClient)Master).HelpUrl = "~/Help/SettingsHelp.aspx";

            loadSettings();
        }

        elbErrorMessage.Text = "";
    }

    protected int ContactId
    {
        get
        {
            object i = Session["ContactId"];
            return (i == null ? 0 : (int)i);
        }
    }

    private void loadSettings()
    {
        bool changingAllowed = CommonAdapter.IsCurrentUserInRole("Client: Basic");
        btnSave.Visible = changingAllowed;

        List<OptionView> optionsToLoad = SettingsAdapter.GetContactSendingOptions(ContactId, allOptions.Values);

        foreach (CheckBox checkBox in allOptions.Keys)
        {
            OptionView displayOption = allOptions[checkBox];

            checkBox.Checked = optionsToLoad.Single(o => o.Category == displayOption.Category && o.Option == displayOption.Option).Value;
            
            if (!changingAllowed)
                checkBox.Enabled = false;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        //SettingsAdapter.Test_GetContacts();
        //return;

        if (CommonAdapter.IsCurrentUserInRole("Client: Basic"))
        {
            List<OptionView> optionsToSave = new List<OptionView>();

            foreach (CheckBox checkBox in changeableOptions.Keys)
            {
                OptionView displayOption = changeableOptions[checkBox];

                optionsToSave.Add(new OptionView { Category = displayOption.Category, Option = displayOption.Option, Value = checkBox.Checked });
            }

            SettingsAdapter.SaveContactSendingOptions(ContactId, optionsToSave);
        }
    }
}

using System;
using System.Collections;
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
//using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using System.Text.RegularExpressions;
using B4F.TotalGiro.ApplicationLayer.BackOffice;
using B4F.TotalGiro.BackOffice.Orders;
using System.Collections.Generic;

public partial class PredefinedBeneficiaries : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErrorMess.Visible = false;
        lblMess.Text = "";

        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Create/Edit Predefined Beneficiaries";

            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvPredefinedBeneficiary_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ShowEditForm(true);
            int predefinedKey = (int)(((GridView)sender).SelectedValue);
            FillEditDetails(predefinedKey);
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void ShowEditForm(bool Show)
    {
        if (Show)
        {
            AsyncPostBackTrigger trig = new AsyncPostBackTrigger();
            trig.ControlID = "gvPredefinedBeneficiary";
            trig.EventName = "SelectedIndexChanged";
            this.pnlEditDetails.Triggers.Add(trig);
        }
        else
        {
            this.pnlEditDetails.Triggers.Clear();
            this.gvPredefinedBeneficiary.DataBind();
        }
        this.pnlEditDetails.Visible = Show;
        this.btnSave.Visible = Show;
        this.btnCancel.Visible = Show;
    }

    private void FillEditDetails(int benefKey)
    {
        PredefinedBeneficiariesDetails pbd = PredefinedBeneficiariesAdapter.GetPredefinedBeneficiary(benefKey);
        this.PredefinedBeneficiaryKey = pbd.Key;
        this.tbSwiftAddress.Text = pbd.SwiftAddress;
        this.tbBenefBankAcctNr.Text = pbd.BankAcctNr;
        this.tbNarBenef1.Text = pbd.NarBenef1;
        this.tbNarBenef2.Text = pbd.NarBenef2;
        this.tbNarBenef3.Text = pbd.NarBenef3;
        this.tbNarBenef4.Text = pbd.NarBenef4;
        this.tbDescription1.Text = pbd.Description1;
        this.tbDescription2.Text = pbd.Description2;
        this.tbDescription3.Text = pbd.Description3;
        this.tbDescription4.Text = pbd.Description4;
        this.ddlCostIndication.SelectedValue = pbd.CostIndicationKey.ToString();
    }

    private void ResetEditData()
    {
        this.tbBenefBankAcctNr.Text = "";
        this.tbSwiftAddress.Text = "";
        this.tbNarBenef1.Text = "";
        this.tbNarBenef2.Text = "";
        this.tbNarBenef3.Text = "";
        this.tbNarBenef4.Text = "";
        this.tbDescription1.Text = "";
        this.tbDescription2.Text = "";
        this.tbDescription3.Text = "";
        this.tbDescription4.Text = "";
        this.ddlCostIndication.SelectedValue = ((int)IndicationOfCosts.Ours).ToString();
        this.hfPredefinedBeneficiaryKey.Value = "0";
    }

    private bool saveRecord()
    {
        bool blnSaveSuccess = false;
        Page.Validate();
        if (Page.IsValid)
        {
            PredefinedBeneficiariesDetails pbd = new PredefinedBeneficiariesDetails();
            pbd.Key = PredefinedBeneficiaryKey;
            pbd.BankAcctNr = tbBenefBankAcctNr.Text.Trim();
            pbd.SwiftAddress = tbSwiftAddress.Text.Trim();
            pbd.NarBenef1 = tbNarBenef1.Text;
            pbd.NarBenef2 = tbNarBenef2.Text;
            pbd.NarBenef3 = tbNarBenef3.Text;
            pbd.NarBenef4 = tbNarBenef4.Text;
            pbd.Description1 = tbDescription1.Text;
            pbd.Description2 = tbDescription2.Text;
            pbd.Description3 = tbDescription3.Text;
            pbd.Description4 = tbDescription4.Text;
            pbd.CostIndicationKey = int.Parse(this.ddlCostIndication.SelectedValue);

            if (string.IsNullOrEmpty(pbd.BankAcctNr) && string.IsNullOrEmpty(pbd.SwiftAddress))
                throw new ApplicationException("Either Swift address or Bank Acct Nr are mandatory.");

            PredefinedBeneficiariesAdapter.SavePredefinedBeneficiary(ref blnSaveSuccess, ref pbd);
        }

        return blnSaveSuccess;

    }

    public int PredefinedBeneficiaryKey
    {
        get
        {
            return ((hfPredefinedBeneficiaryKey.Value.Length == 0) ? 0 : int.Parse(hfPredefinedBeneficiaryKey.Value));
        }
        set { hfPredefinedBeneficiaryKey.Value = value.ToString(); }
    }

    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        try
        {
            ResetEditData();
            ShowEditForm(true);
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (saveRecord())
                ShowEditForm(false);
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            ResetEditData();
            ShowEditForm(false);
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void customValIbanNummer_ServerValidate(object source, ServerValidateEventArgs value)
    {
        if (!(this.tbSwiftAddress.Text.Length == 0))
        {
            string input = Regex.Replace(this.tbBenefBankAcctNr.Text.ToUpper(), @"\s", "");
            //if ((input.Length == 20)  && char.IsLetter(input[0]) && char.IsLetter(input[1]))
            if(((input.Length == 20) && (input.Substring(0,2).ToUpper() == "LU"))
                || ((input.Length == 22) && (input.Substring(0,2).ToUpper() == "DE")))
            {
                input = input.Substring(4, (input.Length - 4)) + (Convert.ToInt16(input[0]) - 55).ToString() + (Convert.ToInt16(input[1]) - 55).ToString() + input.Substring(2, 2);

                int len = Convert.ToInt32((Math.Ceiling(input.Length / 6m)));
                input = input.PadRight((6 * len), ' ');
                string[] sRebuild = new string[len];
                string runner = "00";

                for (int i = 0; i < (6 * len); i += 6)
                {
                    runner = (int.Parse((runner + input.Substring(i, 6))) % 97).ToString();
                }

                value.IsValid = (int.Parse(runner) == 1);
            }
            else
                value.IsValid = false;

        }
        else
            value.IsValid = true;
    }
    protected void tbBenefBankAcctNr_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string input = Regex.Replace(this.tbBenefBankAcctNr.Text.ToUpper(), @"\s", "");
            tbBenefBankAcctNr.Text = input.ToUpper();
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

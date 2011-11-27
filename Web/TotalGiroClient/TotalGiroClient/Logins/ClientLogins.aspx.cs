using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.Logins;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Client.Web.Util;

namespace B4F.TotalGiro.Client.Web.Logins
{
    public partial class ClientLogins : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CommonAdapter.GetCurrentLoginType() == LoginTypes.RemisierEmployee)
                throw new SecurityLayerException();

            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Client Logins";

                gvContacts.Sort("ShortName", SortDirection.Ascending);

                ctlAccountFinder.Focus();
            }

            gvContacts.Columns[0].ItemStyle.BackColor = gvContacts.Columns[1].ItemStyle.BackColor;
            elbErrorMessage.Text = "";
            Utility.EnableScrollToBottom(this, hdnScrollToBottom);
            PageContext = new PageContext(gvContacts, elbErrorMessage, hdnScrollToBottom);
        }

        protected PageContext PageContext { get; private set; }

        protected void ctlAccountFinder_Search(object sender, EventArgs e)
        {
            pnlContacts.Visible = true;
            gvContacts.DataBind();
        }

        protected void gvContacts_DataBound(object sender, EventArgs e)
        {
            gvContacts.Caption = string.Format("Clients ({0})", gvContacts.DataRowCount);
        }

        protected void cboSendByPost_CheckedChanged(object sender, EventArgs e)
        {
            CommonLogins.OnCheckedChanged(PageContext, (CheckBox)sender,
                                          ClientLoginsAdapter.SetSendByPost,
                                          isChecked => string.Format("Sending by Post was turned {0}.", isChecked ? "ON" : "OFF"));
        }

        protected void cboSendByEmail_CheckedChanged(object sender, EventArgs e)
        {
            CommonLogins.OnCheckedChanged(PageContext, (CheckBox)sender,
                                          ClientLoginsAdapter.SetSendByEmail,
                                          isChecked => string.Format("Sending by Email was turned {0}.", isChecked ? "ON" : "OFF"));
        }

        protected void cboLoginActive_CheckedChanged(object sender, EventArgs e)
        {
            CommonLogins.OnCheckedChanged(PageContext, (CheckBox)sender,
                                          ClientLoginsAdapter.GetInstance().SetActive,
                                          isChecked => string.Format("Login was {0}activated.", isChecked ? "" : "in"));
        }

        protected void lbtUnlockLogin_Click(object sender, EventArgs e)
        {
            CommonLogins.OnLinkButtonClick(PageContext, (LinkButton)sender,
                                           ClientLoginsAdapter.GetInstance().UnlockLogin, true, false, "Login was unlocked.");
        }

        protected void lbtResetPassword_Click(object sender, EventArgs e)
        {
            CommonLogins.OnLinkButtonClick(PageContext, (LinkButton)sender,
                                           ClientLoginsAdapter.GetInstance().ResetPassword, false, true, "Password was reset and sent.");
        }

        protected void lbtDeleteLogin_Click(object sender, EventArgs e)
        {
            CommonLogins.OnLinkButtonClick(PageContext, (LinkButton)sender,
                                           ClientLoginsAdapter.GetInstance().DeleteLogin, true, false, "Login was deleted.");
        }

        protected void btnSendLoginName_Click(object sender, EventArgs e)
        {
            CommonLogins.OnGenerateSend(PageContext,
                                        ClientLoginsAdapter.GetInstance().GenerateSendLogins, "login name");
        }

        protected void btnSendPassword_Click(object sender, EventArgs e)
        {
            CommonLogins.OnGenerateSend(PageContext,
                                        ClientLoginsAdapter.GetInstance().GenerateSendPasswords, "password");
        }
    }
}
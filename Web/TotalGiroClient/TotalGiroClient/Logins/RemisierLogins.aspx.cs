using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.Logins;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Client.Web.Util;

namespace B4F.TotalGiro.Client.Web.Logins
{
    public partial class RemisierLogins : System.Web.UI.Page
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
                ((TotalGiroClient)Master).HeaderText = "Remisier Logins";

                gvRemisierEmployees.Sort("ShortName", SortDirection.Ascending);

                ctlAccountFinder.Focus();
            }

            gvRemisierEmployees.Columns[0].ItemStyle.BackColor = gvRemisierEmployees.Columns[1].ItemStyle.BackColor;
            elbErrorMessage.Text = "";
            Utility.EnableScrollToBottom(this, hdnScrollToBottom);
            PageContext = new PageContext(gvRemisierEmployees, elbErrorMessage, hdnScrollToBottom);
        }

        protected PageContext PageContext { get; private set; }

        protected void ctlAccountFinder_Search(object sender, EventArgs e)
        {
            pnlRemisierEmployees.Visible = true;
            gvRemisierEmployees.DataBind();
        }

        protected void gvRemisierEmployees_DataBound(object sender, EventArgs e)
        {
            gvRemisierEmployees.Caption = string.Format("Remisier Employees ({0})", gvRemisierEmployees.DataRowCount);
        }

        //protected void cboIsLocalAdmin_CheckedChanged(object sender, EventArgs e)
        //{
        //    CommonLogins.OnCheckedChanged(PageContext, (CheckBox)sender,
        //                                  RemisierLoginsAdapter.SetLocalAdministrator,
        //                                  isChecked => string.Format("Sending by Post was turned {0}.", isChecked ? "ON" : "OFF"));
        //}

        protected void cboLoginActive_CheckedChanged(object sender, EventArgs e)
        {
            CommonLogins.OnCheckedChanged(PageContext, (CheckBox)sender,
                                          RemisierLoginsAdapter.GetInstance().SetActive,
                                          isChecked => string.Format("Login was {0}activated.", isChecked ? "" : "in"));
        }

        protected void lbtUnlockLogin_Click(object sender, EventArgs e)
        {
            CommonLogins.OnLinkButtonClick(PageContext, (LinkButton)sender,
                                           RemisierLoginsAdapter.GetInstance().UnlockLogin, true, false, "Login was unlocked.");
        }

        protected void lbtResetPassword_Click(object sender, EventArgs e)
        {
            CommonLogins.OnLinkButtonClick(PageContext, (LinkButton)sender,
                                           RemisierLoginsAdapter.GetInstance().ResetPassword, false, true, "Password was reset and sent.");
        }

        protected void lbtDeleteLogin_Click(object sender, EventArgs e)
        {
            CommonLogins.OnLinkButtonClick(PageContext, (LinkButton)sender,
                                           RemisierLoginsAdapter.GetInstance().DeleteLogin, true, false, "Login was deleted.");
        }

        protected void btnSendLoginName_Click(object sender, EventArgs e)
        {
            CommonLogins.OnGenerateSend(PageContext,
                                        RemisierLoginsAdapter.GetInstance().GenerateSendLogins, "login name");
        }

        protected void btnSendPassword_Click(object sender, EventArgs e)
        {
            CommonLogins.OnGenerateSend(PageContext,
                                        RemisierLoginsAdapter.GetInstance().GenerateSendPasswords, "password");
        }
    } 
}

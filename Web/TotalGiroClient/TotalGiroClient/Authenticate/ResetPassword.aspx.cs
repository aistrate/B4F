using System;
using System.Collections.Generic;
using System.Web.UI;
using B4F.TotalGiro.ClientApplicationLayer.Authenticate;
using B4F.TotalGiro.Client.Web.Util;

namespace B4F.TotalGiro.Client.Web.Authenticate
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Aanvragen nieuw wachtwoord";

                txtSofiNumber.Focus();

                addOnClickScript();
            }

            elbErrorMessage.Text = "";
        }

        private void addOnClickScript()
        {
            string jsScript = string.Format(
                @"  var errMsg = document.getElementById('{0}');
                if (errMsg != null)
                    errMsg.innerHTML = '';
                if (Page_ClientValidate('{1}') == false)
                    return true;
                this.disabled = true;
                {2};
                return true;",
                elbErrorMessage.ClientID,
                btnSubmit.ValidationGroup,
                Page.ClientScript.GetPostBackEventReference(btnSubmit, null));

            btnSubmit.Attributes.Add("onclick", jsScript);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitedMoreThan(1000))
                {
                    ResetPasswordResult result = ResetPasswordAdapter.ResetPassword(txtSofiNumber.Text.Trim(), txtAccountNumber.Text.Trim());
                    elbErrorMessage.Text = ErrorMessages[result];

                    if (result == ResetPasswordResult.Success)
                    {
                        pnlSubmitButton.Visible = false;
                        lnkLogin.Visible = true;
                    }
                }
                else
                    elbErrorMessage.Text = "Locked out. Please try again.";
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = "De rekeninghouder is bekend maar er is een fout opgetreden tijdens het resetten van het wachtwoord: <br />" +
                                       Utility.GetCompleteExceptionMessage(ex);
            }
        }

        protected static Dictionary<ResetPasswordResult, string> ErrorMessages = new Dictionary<ResetPasswordResult, string>()
    {
        { ResetPasswordResult.Success,    
            "Uw nieuwe wachtwoord is succesvol verstuurd naar uw <nobr>e-mailadres</nobr>." },
        
        { ResetPasswordResult.ElfproefFailed,
            "Dit burgerservicenummer is ongeldig. Vul a.u.b. opnieuw in." },

        { ResetPasswordResult.PersonNotFound,
            @"De combinatie van het burgerservicenummer met de rekeningnummer komt niet voor in onze administratie.
              <br /><br />
              <span style='color: #313457'>
                  Controleer of u het correct heeft ingevuld en probeer het opnieuw.
                  Mocht het niet lukken, stuur dan a.u.b. een <nobr>e-mail</nobr> naar
                  <a href='mailto:servicedesk@paerel.nl'>servicedesk@paerel.nl</a>
              </span>" },

        { ResetPasswordResult.NoEmail,
            "De (mede)rekeninghouder is bekend maar het <nobr>e-mailadres</nobr> bestaat niet." },

        { ResetPasswordResult.NoLogin,
            "De rekeninghouder is bekend maar heeft geen inlogcode." }
    };

        protected bool WaitedMoreThan(int milliseconds)
        {
            bool waitedEnough = ElapsedAfterLastReset >= milliseconds;

            if (waitedEnough)
                LastPasswordReset = DateTime.Now;

            return waitedEnough;
        }

        protected int ElapsedAfterLastReset
        {
            get { return (int)(DateTime.Now - LastPasswordReset).TotalMilliseconds; }
        }

        protected DateTime LastPasswordReset
        {
            get
            {
                object dt = Application["LastPasswordReset"];
                return (dt == null ? DateTime.Now.AddDays(-1) : (DateTime)dt);
            }
            set { Application["LastPasswordReset"] = value; }
        }
    } 
}

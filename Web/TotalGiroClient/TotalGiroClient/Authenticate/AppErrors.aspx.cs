using System;
using System.Drawing;
using System.Web;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Client.Web.Util;

namespace B4F.TotalGiro.Client.Web.Authenticate
{
    public partial class AppErrors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)this.Master).HeaderText = "Application Error";
                showErrorDetails();
            }
        }

        private void showErrorDetails()
        {
            UnhandledError error = (UnhandledError)SafeSession.Current.GetValueAndRemove("LastUnhandledError");

            if (error != null)
            {
                if (error.Exception is ContactNotFoundException && ((ContactNotFoundException)error.Exception).ContactId == 0)
                    Response.Redirect("~/Default.aspx");

                pnlErrorInfo.Visible = true;
                lblExecutionPath.Text = error.ExecutionPath;
                lblTimestamp.Text = error.Timestamp.ToString(@"dd-MM-yyyy \/ HH:mm");
                lblUserName.Text = error.UserName;

                if (error.Exception != null)
                {
                    lblErrorDetails.ForeColor = Color.Firebrick;

                    lblErrorDetails.Text = error.Exception is HttpCompileException ? "Compiling error in .aspx page." :
                                           error.Exception is HttpParseException ? "Parsing error in .aspx page." :
                                                                                     Utility.GetCompleteExceptionMessage(error.Exception);
                }
                else
                    lblErrorDetails.Text = "";
            }
            else
            {
                pnlErrorInfo.Visible = false;
                lblErrorDetails.ForeColor = Color.Black;
                lblErrorDetails.Text = "No error information was found.";
            }
        }
    } 
}

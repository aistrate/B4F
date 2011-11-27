using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace B4F.Web.WebControls
{
    [ParseChildren(false)]
    [SupportsEventValidation]
    [DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ControlValueProperty("Checked")]
    [DefaultProperty("Text")]
    [DefaultEvent("CheckedChanged")]
    [ToolboxData(@"<{0}:QuestionCheckBox runat=""server""></{0}:QuestionCheckBox>")]
    [Designer("System.Web.UI.Design.WebControls.CheckBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class QuestionCheckBox : CheckBox
    {
        /// <summary>
        /// Question displayed in a message box when the user clicks on the check box.
        /// </summary>
        [Description("Question displayed in a message box when the user clicks on the check box."), DefaultValue(""), Category("Appearance")]
        public string QuestionText
        {
            get
            {
                object s = ViewState["QuestionText"];
                return ((s == null) ? string.Empty : (string)s);
            }
            set { ViewState["QuestionText"] = value; }
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);

            if (Visible && Enabled)
            {
                string script = string.Format(@"if (confirm('{0}')) \
                                                    {{ {1}; }} \
                                                else \
                                                    {{ var cb = document.getElementById('{2}'); cb.checked = !cb.checked; }}",
                                               QuestionText.Replace("'", @"\\'"),
                                               Page.ClientScript.GetPostBackEventReference(this, ""),
                                               ClientID);

                InputAttributes.Add("onclick",
                                    string.Format("javascript:setTimeout('{0}', 0)", script.Replace("'", @"\'")));
            }

        }
    }
}

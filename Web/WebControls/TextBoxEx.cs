using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace B4F.Web.WebControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TextBoxEx runat=server></{0}:TextBoxEx>")]
    public partial class TextBoxEx : TextBox, System.Web.UI.IScriptControl
    {
        public string HoverCssClass
        {
            get 
            {
                object obj = ViewState["HoverCssClass"];
                return (obj != null) ? (string)obj : String.Empty;
            }
            set { ViewState["HoverCssClass"] = value; }
        }
        public string FocusCssClass
        {
            get
            {
                object obj = ViewState["FocusCssClass"];
                return (obj != null) ? (string)obj : String.Empty;
            }
            set { ViewState["FocusCssClass"] = value; }
        }
        public string ReadOnlyCssClass
        {
            get
            {
                object obj = ViewState["ReadOnlyCssClass"];
                return (obj != null) ? (string)obj : String.Empty;
            }
            set { ViewState["ReadOnlyCssClass"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
                ScriptManager manager;
                manager = ScriptManager.GetCurrent(this.Page);

                if (manager == null)
                    throw new InvalidOperationException("A ScriptManager is required on the page.");

                manager.RegisterScriptControl(this);
            
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            
            if (!this.DesignMode)
            {
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
            }
        }
        
        #region IScriptControl Members

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("B4F.Web.WebControls.TextBoxEx", this.ClientID);
            descriptor.AddProperty("cssClass", CssClass);
            descriptor.AddProperty("hoverCssClass", HoverCssClass);
            descriptor.AddProperty("focusCssClass", FocusCssClass);
            descriptor.AddProperty("readOnlyCssClass", ReadOnlyCssClass);
            yield return descriptor;
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            yield return new ScriptReference(Page.ClientScript.GetWebResourceUrl(typeof(TextBoxEx),
                "B4F.Web.WebControls.ClientScript.TextBoxEx.js"));
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace B4F.Web.WebControls
{
    [ToolboxData("<{0}:ImageEx runat=server></{0}:ImageEx>")]
    public partial class ImageEx : Image, System.Web.UI.IScriptControl
    {
        
        [System.ComponentModel.Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [System.ComponentModel.Bindable(true)]
        [System.ComponentModel.Category("Appearance")]
        [System.ComponentModel.DefaultValue("")]
        [System.Web.UI.UrlProperty]
        [System.Web.UI.Themeable(true)]
        public string ImageHoverUrl
        {
            get 
            {
                object obj = ViewState["HoverImageUrl"];
                return (obj != null) ? (string)obj : String.Empty;
            }
            set { ViewState["HoverImageUrl"] = value; }
        }
        
        [System.ComponentModel.Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [System.ComponentModel.Bindable(true)]
        [System.ComponentModel.Category("Appearance")]
        [System.ComponentModel.DefaultValue("")]
        [System.Web.UI.UrlProperty]
        [System.Web.UI.Themeable(true)]
        public string ImageActiveUrl
        {
            get
            {
                object obj = ViewState["ActiveImageUrl"];
                return (obj != null) ? (string)obj : String.Empty;
            }
            set { ViewState["ActiveImageUrl"] = value; }
        }

        [System.ComponentModel.Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [System.ComponentModel.Bindable(true)]
        [System.ComponentModel.Category("Appearance")]
        [System.ComponentModel.DefaultValue("")]
        [System.Web.UI.UrlProperty]
        [System.Web.UI.Themeable(true)]
        public string ImageDisableUrl
        {
            get
            {
                object obj = ViewState["DisableImageUrl"];
                return (obj != null) ? (string)obj : String.Empty;
            }
            set { ViewState["DisableImageUrl"] = value; }
        }


        public string OnClientClick
        {
            get
            {
                object obj = ViewState["OnClientClick"];
                return (obj != null) ? (string)obj : null;
            }
            set { ViewState["OnClientClick"] = value; }
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
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("B4F.Web.WebControls.ImageEx", this.ClientID);
            descriptor.AddProperty("normalImageUrl", this.ResolveClientUrl(ImageUrl));
            descriptor.AddProperty("hoverImageUrl", this.ResolveClientUrl(ImageHoverUrl));
            descriptor.AddProperty("activeImageUrl", this.ResolveClientUrl(ImageActiveUrl));
            descriptor.AddProperty("disableImageUrl", this.ResolveClientUrl(ImageDisableUrl));
            descriptor.AddEvent("click", OnClientClick);
            yield return descriptor;
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            yield return new ScriptReference(Page.ClientScript.GetWebResourceUrl(typeof(ImageEx),
                "B4F.Web.WebControls.ClientScript.ImageEx.js"));
        }

        #endregion
    }
}

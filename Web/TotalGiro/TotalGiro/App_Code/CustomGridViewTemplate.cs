using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CustomControls
{
    /// <summary>
    /// The different types available for CustomGridViewTemplate
    /// </summary>
    public enum CustomGridViewTemplateTypes
	{
        LiteralText
	}

    /// <summary>
    /// CustomGridViewTemplate is used for dynamic creation of gridviews
    /// </summary>
    public class CustomGridViewTemplate : ITemplate
    {
        public CustomGridViewTemplate(CustomGridViewTemplateTypes templateType, string columnName)
        {
            this.templateType = templateType;
            this.columnName = columnName;
        }

        public void InstantiateIn(Control container) 
        { 
            LiteralControl l = new LiteralControl(); 
            l.DataBinding += new EventHandler(this.OnDataBinding); 
            container.Controls.Add(l); 
        }

        public void OnDataBinding(object sender, EventArgs e) 
        { 
            LiteralControl l = (LiteralControl)sender; 
            GridViewRow container = (GridViewRow)l.NamingContainer;
            l.Text = ((DataRowView)container.DataItem)[columnName].ToString(); 
        }

        #region Privates

        CustomGridViewTemplateTypes templateType;
        string columnName;

        #endregion
    }
}

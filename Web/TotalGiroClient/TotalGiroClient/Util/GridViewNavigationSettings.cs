using System.Web.UI.WebControls;

namespace B4F.TotalGiro.Client.Web.Util
{
    public class GridViewNavigationSettings
    {
        public int PageIndex { get; set; }
        public string SortExpression { get; set; }
        public SortDirection SortDirection { get; set; }
    } 
}

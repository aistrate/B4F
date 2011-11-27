using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.Communicator.PearelLeven;
using B4F.TotalGiro.Utils;

public partial class Export_paerelleven : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            calShowDate.SelectedDate = DateTime.Today;
            this.ExternalInterfaceKey = 3;
        }
        for (int i = -1; i > -4; i--)
        {
            DateTime pricedate = calShowDate.SelectedDate.AddDays(i);
            if (!(pricedate.DayOfWeek == DayOfWeek.Saturday || pricedate.DayOfWeek == DayOfWeek.Sunday))
            {
                calPricedate.SelectedDate = pricedate;
                break;
            }
        }

        
    }
    
    protected void btnExportPrices_Click(object sender, EventArgs e)
    {
        Page.Validate();
        if (Page.IsValid)
        {
            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Disposition", "attachment;filename=pl_" + calShowDate.SelectedDate.ToString("yyyyMMdd"));
            Response.Write(PearelLevenMapper.Export(calPricedate.SelectedDate, calShowDate.SelectedDate, ExternalInterfaceKey));
            Response.End();
        }
    }

    protected void cvDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Calendar ctlCal = null;
        Control ctl = (Control)source;
        if (ctl.ID == "cvShowDate")
            ctlCal = calShowDate;
        else
            ctlCal = calPricedate;

        if (ctlCal != null)
        {
            if (Util.IsNullDate(ctlCal.SelectedDate))
                args.IsValid = false;
            else if (ctlCal.SelectedDate.DayOfWeek == DayOfWeek.Saturday || calShowDate.SelectedDate.DayOfWeek == DayOfWeek.Sunday)
                args.IsValid = false;
        }
    }
    public int ExternalInterfaceKey
    {
        get
        {
            return int.Parse(this.ddlExternalInterfaces.SelectedValue);
        }
        set { this.ddlExternalInterfaces.SelectedValue = ((int)value).ToString(); }
    }

}

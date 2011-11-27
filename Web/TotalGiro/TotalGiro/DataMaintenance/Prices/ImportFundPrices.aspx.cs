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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class DataMaintenance_ImportFundPrices : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Import Fund Prices";
        }
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    public void grdList_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        int lastColumn = grdList.Columns.Count;
        foreach (TableCell cell in e.Item.Cells)
        {
            ///e.Item.DataSetIndex = row 
            ///e.Item.Cells.GetCellIndex(cell) = column 
            if (e.Item.Cells.GetCellIndex(cell) == (lastColumn -1) && e.Item.DataSetIndex > 0)
            {
                if (cell.Controls.Count >= 2 && cell.Controls[1].Visible)
                {
                    e.Item.Cells[4].BackColor = System.Drawing.Color.Tomato;
                }
            }
        }
    }

    protected void cmdCancel_Click(object sender, EventArgs e)
    {
        divStart.Visible = true;
        divRes.Visible = false;
        cmdCancel.Visible = false;
        cmdImport.Visible = false;
        cmdAbort.Visible = false;
        lblDat.Visible = false;
        lblErrorFundPrices.Visible = false;
        cmdImport.Text = "Import";
    }

    protected void cmdNext_Click(object sender, EventArgs e)
    {
        if (ctrUpload.HasFile == true)
        {
            ShowList();
        }
    }

    protected void cmdImport_Click(object sender, EventArgs e)
    {
        if (cmdImport.Text == "Done")
        {
            cmdCancel_Click(null, null);
        }
        else
        {
            cmdCancel.Enabled = false;
            foreach (DataGridItem dgRow in grdList.Items)
            {
                if (!dgRow.Cells[0].Text.ToLower().Contains("checked"))
                {
                    try
                    {
                        ImportFundPricesAdapter.UpdateHistoricalPrice(calPrice.SelectedDate.Date, decimal.Parse(dgRow.Cells[3].Text), dgRow.Cells[2].Text);
                        dgRow.BackColor = System.Drawing.Color.LightGreen;
                    }
                    catch
                    {
                        dgRow.BackColor = System.Drawing.Color.Red;
                    }

                }
                else
                {
                    dgRow.BackColor = System.Drawing.Color.Gray;
                }
            }
            cmdImport.Text = "Done";
            cmdAbort.Visible = false;
        }
    }

    // Show and set the current date on the datetimepicker.
    protected void calPrice_Init(object sender, EventArgs e)
    {
        calPrice.SelectedDate = calPrice.TodaysDate;
    }

    private void ShowList()
    {
        //hide first div, show other relevant controls now
        divStart.Visible = false;
        divRes.Visible = true;
        cmdImport.Visible = true;
        cmdCancel.Visible = true;
        cmdAbort.Visible = true;
        lblDat.Visible = true;


        //get the rows into a datasource
        grdList.DataSource = ImportFundPricesAdapter.GetPriceList(calPrice.SelectedDate, ctrUpload.FileContent);
        grdList.DataBind();


        // **********************************************************************************

        //show the price date
        lblDat.Text = "The date of the selected Price is : " + calPrice.SelectedDate.ToShortDateString();

        string strYYYY, strMM, strDD;
        DateTime selectedDate;
        DateTime importDate;
        string impYYYY, impMM, impDD;
        strYYYY = calPrice.SelectedDate.Year.ToString();    // Year of the selected date.
        strMM = calPrice.SelectedDate.Month.ToString();     // Month of the selected date.
        strDD = calPrice.SelectedDate.Day.ToString();       // Day of the selected date.
        selectedDate = DateTime.Parse(strDD.ToString() + '-' + strMM.ToString() + '-' + strYYYY.ToString());

        foreach (DataGridItem objRow in grdList.Items)
        {
            if (objRow.Cells[0].Text.ToLower().Contains("checked"))
            {
                {
                    impYYYY = objRow.Cells[1].Text.Substring(7, 8).Substring(0, 4);    // Year of the imported price date.
                    impMM = objRow.Cells[1].Text.Substring(7, 8).Substring(4, 2);      // Month of the imported price date.
                    impDD = objRow.Cells[1].Text.Substring(7, 8).Substring(6, 2);      // Day of the imported price date.

                    importDate = DateTime.Parse(impDD.ToString() + '-' + impMM.ToString() + '-' + impYYYY.ToString());
                    if ((selectedDate) != (importDate))
                    {
                        lblErrorFundPrices.Visible = true;
                        this.lblErrorFundPrices.Text = "Attention : The selected date is not the same as the date of the imported prices.";
                    }
                    break;
                }
            }
        }
        // **********************************************************************************
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Instruments;
using System.Globalization;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using System.ComponentModel;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.Data;
using B4F.TotalGiro.Orders.Transfers;

public partial class TransferPositionDetailsEditor : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private Money TotalUnitPrice;
    protected Money GetValue(Money value)
    {
        TotalUnitPrice += value;
        return value;
    }
    protected Money GetTotal()
    {
        return TotalUnitPrice;
    }

    public override void DataBind()
    {
        gvNTMTransfer.DataBind();
    }

    public int TransferID
    {
        get { return getIntegerValue(hdnTransferID.Value); }
        set { hdnTransferID.Value = value.ToString(); }
    }

    public bool IsManual
    {
        get { return getBooleanValue(hdnIsManual.Value); }
        set { hdnIsManual.Value = value.ToString(); }
    }

    public bool TransferPriceEditable
    {
        get
        {
            object b = ViewState["TransferPriceEditable"];
            return ((b == null) ? false : (bool)b);
        }
        set { ViewState["TransferPriceEditable"] = value; }
    }

    public decimal MaxTransferSize
    {
        get
        {
            object b = ViewState["TransferPriceEditable"];
            return ((b == null) ? 0m : (decimal)b);
        }
        set { ViewState["TransferPriceEditable"] = value; }
    }


    private int getIntegerValue(string value)
    {
        NumberStyles numberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                                    NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        int result = 0;
        if (value != string.Empty)
        {
            if (!int.TryParse(value, numberStyles, null, out result))
                throw new ArgumentException(string.Format("Invalid numeric format: '{0}'.", value));
        }

        return result;
    }

    private bool getBooleanValue(string value)
    {
        bool b;
        return (value != string.Empty && bool.TryParse(value, out b) ? bool.Parse(value) : false);
    }

    public void InsertLine()
    {
        IsLineInsert = true;
        //AllowGiroAccountsDataBind = false;

        // TODO: restore sorting criteria after Insert
        //gvLines.Sort("LineNumber", SortDirection.Ascending);

        gvNTMTransfer.DataBind();
        if (gvNTMTransfer.PageIndex != gvNTMTransfer.PageCount - 1)
        {
            gvNTMTransfer.PageIndex = gvNTMTransfer.PageCount - 1;
            gvNTMTransfer.DataBind();
        }
        gvNTMTransfer.EditIndex = gvNTMTransfer.Rows.Count - 1;

        OnEditing();
    }

    protected bool IsLineInsert
    {
        get { return getBooleanValue(hdnIsLineInsert.Value); }
        set
        {
            hdnIsLineInsert.Value = value.ToString();
        }
    }

    protected void lbtEdit_Command(object sender, CommandEventArgs e)
    {
        try
        {
            IsLineInsert = false;
            //AllowGiroAccountsDataBind = false;

            int Positionid = int.Parse((string)e.CommandArgument);
            gvNTMTransfer.EditIndex = findRowIndex(gvNTMTransfer, Positionid);

            OnEditing();
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }
    protected void lbtDelete_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int transferDetailID = int.Parse((string)e.CommandArgument);
            TransferPositionDetailsAdapter.DeletePositionTransferDetail(this.TransferID, transferDetailID);

            IsLineInsert = false;
            gvNTMTransfer.EditIndex = -1;
            gvNTMTransfer.DataBind();
            OnUpdated();
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }
 

    protected void OnEditing()
    {
        if (Editing != null)
            Editing(this, EventArgs.Empty);
    }

    private int findRowIndex(GridView gridView, int key)
    {
        int rowIndex = -1;
        for (int i = 0; i < gridView.DataKeys.Count; i++)
            if ((int)gridView.DataKeys[i].Value == key)
                rowIndex = i;

        return rowIndex;
    }

    protected void OnUpdated()
    {
        if (Updated != null)
            Updated(this, EventArgs.Empty);
    }



    protected void gvNTMTransfer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dataRowView = (DataRowView)e.Row.DataItem;

                decimal sign = 1M;
                if ((TransferDirection)dataRowView["TxDirection"] == TransferDirection.FromBtoA)
                {
                    e.Row.BackColor = System.Drawing.Color.LightSalmon;
                    sign = -1M;
                }

                if ((decimal)dataRowView["ValueinEuroQty"] != 0m)
                    totalTransferAmount += ((decimal)dataRowView["ValueinEuroQty"] * sign);

                if (gvNTMTransfer.EditIndex >= 0 && e.Row.RowIndex == gvNTMTransfer.EditIndex)
                {
                    if ((int)dataRowView["Key"] == 0 || (bool)dataRowView["IsEditable"])
                    {
                        editingRow = e.Row;

                        lbtDelete.Visible = false;
                        lbtEdit.Visible = false;
                        lbtUpdate.Visible = true;
                        //lbtCancel.Visible = true;

                        if ((decimal)dataRowView["PriceQuantity"] != 0m)
                            PriceQuantity = (decimal)dataRowView["PriceQuantity"];

                        if ((int)dataRowView["TxDirection"] != 0)
                            TxDirectionId = (int)dataRowView["TxDirection"];


                        if ((decimal)dataRowView["Size"] != 0m)
                            Size = (decimal)dataRowView["Size"];

                        if ((int)dataRowView["InstrumentID"] != 0)
                            InstrumentID = (int)dataRowView["InstrumentID"];

                        this.ddlInstrumentOfPosition.DataBind();

                    }
                }
                else
                {

                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[6].Text = totalTransferAmount.ToString("0.00");
            }
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void ddlInstrumentOfPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.MaxTransferSize = TransferPositionDetailsAdapter.GetMaximumSize((TransferDirection)this.TxDirectionId, int.Parse(ddlInstrumentOfPosition.SelectedValue), this.TransferID);
        this.dbSize.Value = this.MaxTransferSize;
        //this.dbSize.MaximumValue = this.MaxTransferSize;

    }

    protected void ddlTxDirection_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.dbSize.Value = 0m;

    }
    

    protected void gvNTMTransfer_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            e.NewValues["ParentTransfer"] = this.TransferID;
            e.NewValues["PositionSize"] = this.Size;
            e.NewValues["TxDirection"] = (TransferDirection)this.TxDirectionId;
            e.NewValues["Instrumentid"] = this.InstrumentID;
            IsLineInsert = false;

            /****
             * 
                public int Key { get; set; }
                public int ParentTransfer { get; set; }
                public decimal PositionSize { get; set; }
                public TransferDirection TxDirection { get; set; }
                public int Instrumentid { get; set; }
                public decimal ActualPriceQuantity { get; set; }
                public decimal TransferPriceQuantity { get; set; }
             * 
             * ****/
        }
        catch (Exception ex)
        {
            e.Cancel = true;
            OnError(ex);
        }
    }

    protected int TxDirectionId
    {
        get { return int.Parse(ddlTxDirection.SelectedValue); }
        set { ddlTxDirection.SelectedValue = value.ToString(); }
    }

    protected decimal PriceQuantity
    {
        get { return dbPriceQuantity.Value; }
        set { dbPriceQuantity.Value = value; }
    }

    protected decimal Size
    {
        get { return dbSize.Value; }
        set { dbSize.Value = value; }
    }

    protected int InstrumentID
    {
        get { return int.Parse( ddlInstrumentOfPosition.SelectedValue); }
        set { ddlInstrumentOfPosition.SelectedValue = value.ToString(); }
    }

    protected void OnError(Exception exception)
    {
        if (Error != null)
            Error(this, new ErrorEventArgs(exception));
    }

    [Description("Event triggered when an exception was generated by the control."), Category("Action")]
    public ErrorEventHandler Error;
    [Description("Event triggered when entering Insert or Update mode."), Category("Action")]
    public EventHandler Editing;
    [Description("Event triggered after a line was inserted, updated, deleted, or stornoed."), Category("Action")]
    public EventHandler Updated;



    protected DropDownList ddlInstrumentOfPosition { get { return (DropDownList)Utility.FindControl(EditingRow, "ddlInstrumentOfPosition"); } }
    protected DropDownList ddlTxDirection { get { return (DropDownList)Utility.FindControl(EditingRow, "ddlTxDirection"); } }
    protected LinkButton lbtDelete { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtDelete"); } }
    protected LinkButton lbtEdit { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtEdit"); } }
    protected LinkButton lbtUpdate { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtUpdate"); } }
    protected LinkButton lbtCancel { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtCancel"); } }
    protected DecimalBox dbPriceQuantity { get { return (DecimalBox)Utility.FindControl(EditingRow, "dbPriceQuantity"); } }
    protected DecimalBox dbSize { get { return (DecimalBox)Utility.FindControl(EditingRow, "dbSize"); } }
    protected GridViewRow EditingRow
    {
        get
        {
            if (editingRow == null)
                editingRow = (gvNTMTransfer.EditIndex >= 0 ? gvNTMTransfer.Rows[gvNTMTransfer.EditIndex] : null);

            return editingRow;
        }
    }

    private GridViewRow editingRow;
    private decimal totalTransferAmount = 0;
}

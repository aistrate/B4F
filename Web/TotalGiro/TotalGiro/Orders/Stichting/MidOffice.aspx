<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="MidOffice.aspx.cs" Inherits="MidOffice" Theme="Neutral" %>

<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
        <cc1:MultipleSelectionGridView 
            ID="gvUnapprovedTrades" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataSourceID="odsUnapprovedTrades"
            cssclass="padding"
            AllowPaging="True" 
            PageSize="20" 
            Caption="Unapproved Trades" 
            CaptionAlign="Left"
            AllowSorting="True" 
            DataKeyNames="Key"
            SelectionBoxEnabledBy="IsApproveable"
            OnRowCommand="gvUnapprovedTrades_RowCommand">
            <Columns>
                <asp:BoundField DataField="Key" HeaderText="TradeID" SortExpression="Key" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>                
                <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrumentDisplayName">
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <trunc:TruncLabel
                            runat="server" ID="Instrument"
                            Width="30"
                            Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrumentDisplayName") %>' 
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Side" SortExpression="TxSide">
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <%# (Side)DataBinder.Eval(Container.DataItem, "TxSide") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ValueSizeDisplayString" HeaderText="Value" SortExpression="ValueSize" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Price" SortExpression="Price" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="CounterValueSizeDisplayString" HeaderText="CounterValue" SortExpression="CounterValueSize" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="AccountBShortName" HeaderText="CounterParty" SortExpression="AccountBShortName">
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ExchangeRate" HeaderText="ExRate" SortExpression="ExchangeRate" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="TransactionDate" HeaderText="TxDate" SortExpression="TransactionDate" DataFormatString="{0:d}" HtmlEncode="False" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ContractualSettlementDate" HeaderText="Settlement" SortExpression="ContractualSettlementDate" DataFormatString="{0:d}" HtmlEncode="False" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ServiceCharge" HeaderText="ServiceCh." SortExpression="ServiceCharge" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="AccruedInterest" HeaderText="Accr.Int." SortExpression="AccruedInterest" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:CheckBoxField DataField="Approved" HeaderText="Approved" SortExpression="Approved" />
                <asp:BoundField DataField="ChildOrderCount_Count" HeaderText="child#" SortExpression="ChildOrderCount_Count" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Allocations_Count" HeaderText="alloc#" SortExpression="Allocations_Count" >
                    <ItemStyle wrap="False" />
                </asp:BoundField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <asp:LinkButton 
                            runat="server" 
                            ID="lbtCancelTrade" 
                            Text="Cancel"
                            CommandName="CancelTrade"
                            OnClientClick="return confirm('Cancel trade?')"/>
                        <asp:LinkButton 
                            runat="server" 
                            ID="lbtViewOrder" 
                            Text="View Order"
                            CommandName="ViewOrder"/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <SelectedRowStyle BackColor="Gainsboro" />
        </cc1:MultipleSelectionGridView>
        <asp:ObjectDataSource ID="odsUnapprovedTrades" runat="server" SelectMethod="GetUnApprovedTrades"
            TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.MidOfficeAdapter"></asp:ObjectDataSource>
    <br />
    <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" />
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>&nbsp;<br />
    <br />
    
    <asp:GridView ID="gvApprovedOrders" runat="server" AutoGenerateColumns="False" Caption="Orders"
        CaptionAlign="Left" DataSourceID="odsApprovedOrders" Visible="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
                <ItemStyle wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Value_DisplayString" HeaderText="Value" SortExpression="Value" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="OpenValue_DisplayString" HeaderText="OpenValue" SortExpression="OpenValue" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsApprovedOrders" runat="server" SelectMethod="GetOrderFromTransaction"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.MidOfficeAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvUnapprovedTrades" Name="tradeId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
</asp:Content>

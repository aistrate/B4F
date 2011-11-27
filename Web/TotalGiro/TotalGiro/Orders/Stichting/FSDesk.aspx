<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="FSDesk.aspx.cs" Inherits="FSDesk" Theme="Neutral" Title="FundSettle Desk"%>

<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/OrderFill.ascx" TagName="OrderFill" TagPrefix="uc2" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="false" />
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvRoutedOrders" 
        runat="server" 
        AutoGenerateColumns="False" 
        DataSourceID="odsRoutedOrders"
        CssClass="padding"
        AllowPaging="True" 
        Caption="Routed Orders" 
        CaptionAlign="Left"
        AllowSorting="True" 
        DataKeyNames="OrderID"
        OnRowDataBound="gvRoutedOrders_RowDataBound"
        OnRowCommand="gvRoutedOrders_RowCommand" 
        SelectionBoxEnabledBy="IsFsSendable"
        style="position: relative; top: -10px;" >
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" SortExpression="Key" >
                <ItemStyle wrap="False" />
            </asp:BoundField>                
            <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrument_DisplayName">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel
                        runat="server" ID="Instrument"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PlacedValue_DisplayString" HeaderText="Value" SortExpression="PlacedValue" >
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
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtFillOrder" 
                        Text="Fill" 
                        CommandName="FillOrder"
                        Visible="false"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtEditOrder" 
                        Text="Edit" 
                        CommandName="EditOrder"
                        Visible="false"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtCancelOrder" 
                        Text="Cancel"
                        CommandName="CancelOrder"
                        Visible="false"
                        OnClientClick="return confirm('Cancel order?')"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewTransaction" 
                        Text="View Transaction"
                        CommandName="ViewTransaction"
                        Visible="false"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtReset" 
                        Text="Reset"
                        CommandName="ResetOrder"
                        OnClientClick="return confirm('Reset order?')"
                        Visible="false"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsRoutedOrders" runat="server" SelectMethod="GetRoutedOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.FSDeskAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                Type="Object" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Button ID="btnCreateFundSettleFile" runat="server" Text="Create FundSettle File" OnClick="btnCreateFundSettleFile_Click" ToolTip="Creates a file containing selected orders so it can be uploaded to FundSettle" />
    <asp:Button ID="btnFSExports" runat="server" Text="View FundSettle Files" OnClick="btnFSExports_Click" ToolTip="Go to the Fundsettle Export file overview" />
    <asp:Button ID="btnUnapprove" runat="server" OnClick="btnUnapprove_Click" Text="Unapprove" OnClientClick="return confirm('Unapprove orders?')" />
    <br /><br />
    <uc2:OrderFill ID="ctlOrderFill" runat="server" DataSourceID="odsDetailsOrderFill" Visible="false" />
    
    <asp:ObjectDataSource 
        ID="odsDetailsOrderFill" 
        runat="server" 
        SelectMethod="GetOrderFills"
        UpdateMethod="UpdateOrderFill"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.FSDeskAdapter"
        DataObjectTypeName="B4F.TotalGiro.ApplicationLayer.UC.OrderFillView" 
        OldValuesParameterFormatString="original_{0}" >
        <SelectParameters>
            <asp:ControlParameter ControlID="gvRoutedOrders" Name="orderId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:DetailsView 
        ID="dvOrderEdit" 
        runat="server" 
        DataSourceID="odsDetailsOrderEdit" 
        AutoGenerateRows="False"
        DataKeyNames="OrderID"
        Caption="Edit Order" 
        CaptionAlign="Left"
        Width="270px"
        OnItemCommand="dvOrderEdit_ItemCommand" Visible="False"> 
        <Fields>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblOrderID" runat="server">OrderID</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="OrderID" runat="server" 
                        Text='<%# DataBinder.Eval(Container.DataItem, "OrderID") %>'>
                    </asp:Label>
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblRoute" runat="server">Route</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList DataSource='<%# B4F.TotalGiro.ApplicationLayer.Orders.Stichting.ManualDeskAdapter.GetRoutes() %>' DataTextField="Name" DataValueField="Key" ID="ddlRoutes" runat="server" SelectedValue='<%# DataBinder.Eval(Container.DataItem, "RouteId") %>'>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator
                         ID="reqValSize"
                         ControlToValidate="ddlRoutes"
                         runat="server"
                         Text="*"
                         ErrorMessage="Route is an obligatory field" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDvSaveOrder" 
                        Text="OK" 
                        CommandName="dvSaveOrder"
                        Visible="true"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDvCancelEdit"
                        CausesValidation="false" 
                        Text="Cancel" 
                        CommandName="dvCancelEdit"
                        Visible="true"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:ObjectDataSource 
        ID="odsDetailsOrderEdit" 
        runat="server" 
        SelectMethod="GetOrderEditData"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.FSDeskAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvRoutedOrders" Name="orderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False" Caption="Transactions"
        CaptionAlign="Left" DataSourceID="odsTransactions" Visible="False">
        <Columns>
            <asp:BoundField DataField="Order_OrderID" HeaderText="OrderID" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ValueSize_DisplayString" HeaderText="Value Size" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CounterValueSize_DisplayString" HeaderText="Countervalue Size" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Price_DisplayString" HeaderText="Price" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ExchangeRate" HeaderText="Exchange Rate" DataFormatString="{0:F4}" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TransactionDate" HeaderText="Transaction Date" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Key" HeaderText="TradeID" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsTransactions" runat="server" SelectMethod="GetOrderTransactions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.ManualDeskAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvRoutedOrders" Name="orderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
</asp:Content>

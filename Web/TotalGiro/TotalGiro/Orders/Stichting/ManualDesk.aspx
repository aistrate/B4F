<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="ManualDesk.aspx.cs" Inherits="ManualDesk" Theme="Neutral" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/OrderFill.ascx" TagName="OrderFill" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="false" />
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvRoutedOrders" 
        runat="server" 
        CellPadding="0"
        AutoGenerateColumns="False" 
        DataSourceID="odsRoutedOrders"
        cssclass="padding"
        AllowPaging="True" 
        PageSize="15" 
        Caption="Routed Orders" 
        CaptionAlign="Left"
        AllowSorting="True" 
        DataKeyNames="OrderID"
        OnRowDataBound="gvRoutedOrders_RowDataBound"
        OnRowCommand="gvRoutedOrders_RowCommand" 
        style="position: relative; top: -10px;" SelectionBoxEnabledBy="IsUnApproveable" >
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" SortExpression="Key" ReadOnly="True" >
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
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" ReadOnly="True" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PlacedValue_DisplayString" HeaderText="Value" SortExpression="PlacedValue" ReadOnly="True" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="OpenValue_DisplayString" HeaderText="OpenValue" SortExpression="OpenValue" ReadOnly="True" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" ReadOnly="True" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="PlacedValue_NumberOfDecimals" HeaderText="Decimals" SortExpression="PlacedValue_NumberOfDecimals" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status" ReadOnly="True" >
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
                        ID="lbtSendOrder" 
                        Text="Send" 
                        CommandName="SendOrder"
                        Visible="false"
                        ToolTip="Set order status to Routed"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtEditOrder" 
                        Text="Edit" 
                        CommandName="EditOrder"
                        Visible="false"
                        ToolTip="Edit order route"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtPlaceOrder" 
                        Text="Place" 
                        CommandName="PlaceOrder"
                        Visible="false"
                        ToolTip="Set order status to Placed"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtFillOrder" 
                        Text="Fill" 
                        CommandName="FillOrder"
                        Visible="false"
                        ToolTip="Fill the transaction with price and size"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtCancelOrder" 
                        Text="Cancel"
                        CommandName="CancelOrder"
                        Visible="false"
                        ToolTip="Cancel this order"
                        OnClientClick="return confirm('Cancel order?')"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewTransaction" 
                        Text="View Transaction"
                        CommandName="ViewTransaction"
                        ToolTip="View transaction details of this order"
                        Visible="false"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsRoutedOrders" runat="server" SelectMethod="GetRoutedOrders" UpdateMethod="EditDecimalPlaces"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.ManualDeskAdapter">
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
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label><br />
    <asp:Button ID="btnUnapprove" runat="server" OnClick="btnUnapprove_Click" Text="Unapprove" 
        OnClientClick="return confirm('Unapprove orders?')" /><br />
    <uc2:OrderFill ID="ctlOrderFill" runat="server" DataSourceID="odsDetailsOrderFill" Visible="false" />
    
    <asp:ObjectDataSource 
        ID="odsDetailsOrderFill" 
        runat="server" 
        SelectMethod="GetOrderFills"
        UpdateMethod="UpdateOrderFill"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.ManualDeskAdapter" 
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
                    <asp:Label ID="lblNumberOfDecimals" runat="server">Number of decimals</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <db:DecimalBox ID="dbNumberOfDecimals" runat="server" 
                        Value='<%# (int)DataBinder.Eval(Container.DataItem, "NumberOfDecimals") %>'
                        Enabled='<%# (bool)DataBinder.Eval(Container.DataItem, "IsSizeBased") %>' 
                        DecimalPlaces="0" AllowNegativeSign="false" />
                    <asp:RequiredFieldValidator ID="reqNDec"
                         ControlToValidate="dbNumberOfDecimals:tbDecimal"
                         runat="server"
                         Text="*"
                         ErrorMessage="Number of decimals is a mandatory field" />
                    <asp:RegularExpressionValidator
                        runat="server"
                        id="regvNDec"
                        controlToValidate="dbNumberOfDecimals:tbDecimal"
                        Text="*"
                        errorMessage="Enter a number (max 2 digits)"
                        validationExpression="[0-9][0-9]?"
                        display="Static"/>
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
                         ErrorMessage="Route is a mandatory field" />
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
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.ManualDeskAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvRoutedOrders" Name="orderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <br />
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
    <asp:Button ID="btnHideTransactions" runat="server" Text="Hide" OnClick="btnHideTransactions_Click" Visible="false" />&nbsp
    <br />
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>

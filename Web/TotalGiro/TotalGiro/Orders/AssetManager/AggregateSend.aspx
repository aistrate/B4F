<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="AggregateSend.aspx.cs" Inherits="AggregateSend" %>

<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="false" />
    <cc1:MultipleSelectionGridView 
        ID="gvApprovedOrders" 
        runat="server" 
        AutoGenerateColumns="False"
        DataSourceID="odsApprovedOrders"
        DataKeyNames="Key"
        cssclass="padding"
        CellPadding="0"
        AllowPaging="True"
        Caption="Approved/New Orders" 
        CaptionAlign="Left" 
        PageSize="10" 
        AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" SortExpression="Key" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Account_Number" HeaderText="Account No." SortExpression="Account_Number" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account Name" SortExpression="Account_ShortName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrument_DisplayName">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value" SortExpression="Value">
                <ItemStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                <HeaderStyle cssclass="alignright" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Value_DisplayString") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Commission_DisplayString" HeaderText="Commission" SortExpression="Commission">
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:Label ID="lblErrorAggregate" runat="server" ForeColor="Red"></asp:Label>
    <br />&nbsp;
    <asp:Button ID="btnAggregate" runat="server" OnClick="btnAggregate_Click" Text="Aggregate" />&nbsp;
    <asp:Button ID="btnAggregateSpecial" runat="server" OnClick="btnAggregateSpecial_Click" Text="Aggregate Special" />&nbsp;
    <asp:Button ID="btnUnApprove" runat="server" OnClick="btnUnApprove_Click" Text="Unapprove" 
        OnClientClick="return confirm('Unapprove orders?')" />
    <asp:ObjectDataSource ID="odsApprovedOrders" runat="server" SelectMethod="GetUnAggregatedChildOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.AggregateSendAdapter">
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
    <br />
    <br />
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvAggregatedOrders"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="10" 
        AutoGenerateColumns="False"
        DataKeyNames="OrderID"
        OnRowCommand="gvAggregatedOrders_OnRowCommand"
        Caption="Aggregated Orders"
        CaptionAlign="Left"
        DataSourceID="odsAggregatedOrders" 
        AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" SortExpression="Key" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ChildOrders_Count" HeaderText="Child Orders" SortExpression="ChildOrders_Count">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Right" wrap="False" />
                <HeaderStyle cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrument_DisplayName">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel2" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value" SortExpression="Value">
                <ItemStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                <HeaderStyle cssclass="alignright" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel3" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Value_DisplayString") %>' 
                        />
                    </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:CommandField SelectText="View" ShowSelectButton="True" >
                <ItemStyle wrap="False" />
            </asp:CommandField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:Label ID="lblErrorAggregatedOrders" runat="server" ForeColor="Red"></asp:Label><br />&nbsp;
    <asp:Button ID="btnSend" runat="server" Text="Send to Stichting" OnClick="btnSend_Click" />&nbsp;
    <asp:Button ID="btnDeAggregate" runat="server" Text="De-aggregate" OnClick="btnDeAggregate_Click" 
        OnClientClick="return confirm('De-aggregate orders?')" />
    <asp:ObjectDataSource ID="odsAggregatedOrders" runat="server" SelectMethod="GetAggregatedOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.AggregateSendAdapter">
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
    &nbsp;<br />
    &nbsp;&nbsp;<br />
    <br />
    <br />
</asp:Content>


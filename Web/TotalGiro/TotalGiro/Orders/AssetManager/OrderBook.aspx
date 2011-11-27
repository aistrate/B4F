<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="OrderBook.aspx.cs" Inherits="Orders_AssetManager_OrderBook" Title="Order Book"  Theme="Neutral"%>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="ucCalendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm"  runat="server" />
    <table cellpadding="0" cellspacing="5" border="0" style="width: 419px">
        <tr>
            <td style="vertical-align: middle; text-align: right; width: 145px; height: 24px;">
                <asp:Label ID="lblOrderID" runat="server" Text="OrderID">
                </asp:Label>
            </td>
            <td style="width: 230px; height: 24px;">
                <asp:TextBox ID="txtOrderID" runat="server" />
            </td>
            <td/>
            <td style="vertical-align: middle; text-align: right; width: 150px;">
                <asp:Label ID="lblIsin" runat="server" Text="ISIN">
                </asp:Label>
            </td>
            <td style="width: 230px">
                <asp:TextBox ID="txtIsin" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="3" />
            <td style="vertical-align: middle; text-align: right; width: 150px;">
                <asp:Label ID="lblInstrumentName" runat="server" Text="Instrument Name" Width="136px"></asp:Label>
            </td>
            <td style="width: 230px">
                <asp:TextBox ID="txtInstrumentName" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align: middle; text-align: right; width: 145px;">
                <asp:Label ID="lblOrderType" runat="server" Text="Order Type">
                </asp:Label>
            </td>
            <td style="width: 230px">
                <asp:DropDownList ID="ddlOrderType" runat="server" DataSourceID="odsOrderType"
                    DataTextField="Description" DataValueField="Key" Width="500px">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsOrderType" runat="server" SelectMethod="GetOrderTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.OrderBookAdapter">
                </asp:ObjectDataSource>
            </td>
            <td />
            <td style="vertical-align: middle; text-align: right; width: 150px;">
                <asp:Label ID="lblSecurityCategory" runat="server" Text="Security Category" Width="123px"></asp:Label>
            </td>
            <td style="width: 230px">
                <asp:DropDownList ID="ddlSecCategory" runat="server" Width="165px" DataSourceID="odsSecCategory" 
                DataTextField="Description" DataValueField="Key">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="odsSecCategory" runat="server" SelectMethod="GetSecCategories"
                TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
            </asp:ObjectDataSource>
            </td>
        </tr>
        
        <tr>
            <td style="height: 26px; vertical-align: middle; text-align: right; width: 145px;">
                <asp:Label ID="lblAccountNr" runat="server" Text="AccountNr">
                </asp:Label>
            </td>
            <td style="height: 26px; width: 230px;">
                <asp:TextBox ID="txtAccountNr" runat="server" />
            </td>
            <td colspan="3" />
        </tr>
        
        <tr>
            <td style="vertical-align: middle; text-align: right; width: 145px;">
                <asp:Label ID="lblAccountName" runat="server" Text="AccountName">
                </asp:Label>
            </td>
            <td style="width: 230px">
                <asp:TextBox ID="txtAccountName" runat="server" />
            </td>
            <td />
            <td style="vertical-align: middle; text-align: right; height: 22px; width: 145px;">
                <asp:Label ID="lblDateFrom" runat="server" Text="Date from" />
            </td>
            <td width="30%" style="vertical-align: middle; text-align: left; height: 25px;">
                <ucCalendar:Calendar ID="cldDateFrom" runat="server" Format="dd-MM-yyyy" />
            </td>
        </tr>
        
        <tr>
            <td style="vertical-align: middle; text-align: right; width: 145px;">
                <asp:Label ID="lblActiveFlag" runat="server" Text="Active Flag">
                </asp:Label>
            </td>
            <td style="width: 230px">
                <asp:DropDownList ID="ddlActiveFlag" runat="server" Width="500px" DataSourceID="odsActiveFlag" DataTextField="Description" DataValueField="Key" />
                <asp:ObjectDataSource ID="odsActiveFlag" runat="server" SelectMethod="GetActiveStati" TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.OrderBookAdapter" />
            </td>
            <td>
            </td>
            <td style="vertical-align: middle; text-align: right; height: 22px; width: 145px;">
                <asp:Label ID="lblDateTo" runat="server" Text="Date to">
                </asp:Label>
            </td>
            <td width="30%" style="vertical-align: middle; text-align: left; height: 25px;">
                <ucCalendar:Calendar ID="cldDateTo" runat="server" Format="dd-MM-yyyy" />
            </td>
        </tr>

        <tr>
            <td />
            <td style="height: 37px; width: 230px;" colspan="2" >
                <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Text="Refresh" />
                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" OnClick="btnResetFilter_Click" />
            </td>
            <td style="vertical-align: middle; text-align: right; height: 22px; width: 145px;">
                <asp:Label ID="lblLastRefreshedLabel" runat="server" Width="99px" Text="Last Refreshed" Visible="false" />
            </td>
            <td style="height: 22px; width: 230px;">
                <asp:Label ID="lblLastRefreshed" runat="server" />
            </td>
        </tr>
    </table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="590px"></asp:Label><br />
    <table width="770" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:GridView 
                    ID="gvOrders" 
                    runat="server" 
                    AllowSorting="True" 
                    DataSourceID="odsOrders"
                    AutoGenerateColumns="False" 
                    PageSize="15"
                    AllowPaging="True"
                    Visible="False" 
                    OnDataBound="gvOrders_DataBound" 
                    OnRowCommand="gvOrders_RowCommand"
                    DataKeyNames="OrderID" OnRowDataBound="gvOrders_RowDataBound" >
                    <Columns>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:ImageButton ID="imbDetails" CommandName="Details" runat="server" ImageUrl="~/layout/images/audit.gif" 
                                    Height="16px" Width="16px" ToolTip="View Audit Trail" /></ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="OrderID" HeaderText="OrderID" SortExpression="OrderID" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Account_Number" HeaderText="Accountnr" SortExpression="Account_Number" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                         <asp:TemplateField 
                                HeaderText="AccountName"
                                SortExpression="Account_ShortName">
                                <ItemTemplate>
                                    <asp:Label 
                                        runat="server"
                                        Text='<%# Abbreviation((DataBinder.Eval(Container.DataItem, "Account_ShortName")).ToString(), 20) %>' 
                                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' />
                                </ItemTemplate>
                             <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                         <asp:TemplateField 
                                HeaderText="Instrument"
                                SortExpression="RequestedInstrument_DisplayName">
                                <ItemTemplate>
                                    <asp:Label 
                                        runat="server"
                                        Text='<%# Abbreviation((DataBinder.Eval(Container.DataItem, "RequestedInstrument_DisplayName")).ToString(), 20) %>' 
                                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "RequestedInstrument_DisplayName") %>' />
                                </ItemTemplate>
                             <ItemStyle Wrap="False" />
                        </asp:TemplateField>            
                        <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Side" SortExpression="Side">
                            <ItemStyle wrap="False" />
                            <ItemTemplate>
                                <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Value_DisplayString" HeaderText="Value" SortExpression="Value_DisplayString" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                         <asp:TemplateField 
                                HeaderText="Commission"
                                SortExpression="CommissionInfo">
                                <ItemTemplate>
                                    <asp:Label 
                                        runat="server"
                                        Text='<%# Abbreviation((DataBinder.Eval(Container.DataItem, "CommissionInfo")).ToString(), 15) %>' 
                                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "CommissionInfo") %>' />
                                </ItemTemplate>
                             <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                            <ItemStyle wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:CommandField InsertVisible="False" SelectText="Transactions" ShowCancelButton="False"
                            ShowSelectButton="True" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
    <asp:ObjectDataSource ID="odsOrders" runat="server" SelectMethod="GetOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.OrderBookAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlOrderType" Name="orderType" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtAccountNr" Name="accountNumber" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtAccountName" Name="accountName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ddlActiveFlag" Name="activeFlag" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtIsin" Name="isin" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtInstrumentName" Name="instrumentName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ddlSecCategory" Name="SecCategoryID" PropertyName="SelectedValue" Type="int32" />
            <asp:ControlParameter ControlID="txtOrderID" Name="orderID" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="cldDateFrom" Name="dateFrom" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="cldDateTo" Name="dateTo" PropertyName="SelectedDate" Type="DateTime" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />

    <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False" Caption="Transactions"
        CaptionAlign="Left" DataSourceID="odsTransactions" Visible="false">
        <Columns>
            <asp:BoundField DataField="Order_OrderID" HeaderText="OrderID" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TransactionDate" HeaderText="Transaction Date" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
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
            <asp:BoundField DataField="Commission_DisplayString" HeaderText="Commission" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ServiceCharge_DisplayString" HeaderText="Service Charge" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="AccruedInterest_DisplayString" HeaderText="Accr.Int." >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ExchangeRate" HeaderText="Exchange Rate" DataFormatString="{0:F4}" >
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsTransactions" runat="server" SelectMethod="GetOrderTransactions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.OrderBookAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvOrders" Name="OrderID" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


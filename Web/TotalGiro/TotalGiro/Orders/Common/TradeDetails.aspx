<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TradeDetails.aspx.cs" Inherits="Orders_Common_TradeDetails" Title="Trade Details" Theme="Neutral" %>

<%@ Register Src="~/UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Import Namespace="B4F.TotalGiro.Utils" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <uc1:BackButton ID="BackButton1" runat="server" />
    <asp:Button ID="Button1" runat="server" OnClick="btnAuditLogDetails_Click"
        Text="Audit Log Details..." />
    <br /><br />
    <asp:DetailsView ID="dvTrade" runat="server" AutoGenerateRows="False" Caption="Transaction"
        CaptionAlign="Left" DataSourceID="odsTrade" Width="125px" DataKeyNames="ClassName,AuditLogKey"
        OnItemCommand="dvTrade_ItemCommand" ondatabound="dvTrade_DataBound">
        <Fields>
            <asp:BoundField DataField="ClassName" HeaderText="Type">
                <ItemStyle Wrap="False" BackColor="Gainsboro" />
                <HeaderStyle BackColor="LightSteelBlue" />
            </asp:BoundField>
            <asp:BoundField DataField="Key" HeaderText="TradeID">
                <ItemStyle Wrap="False" BackColor="Gainsboro" />
                <HeaderStyle BackColor="LightSteelBlue" />
            </asp:BoundField>
            <asp:BoundField DataField="AccountAName" HeaderText="Account A">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="AccountBName" HeaderText="Account B">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TxSide" HeaderText="Side">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Transaction Date">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "TransactionDate"))%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TradedInstrument" HeaderText="Traded Instrument">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ValueSize" HeaderText="Value">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CounterValueSize" HeaderText="Counter Value">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Price" HeaderText="Price">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ExchangeRate" HeaderText="Exchange Rate">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <%--<asp:BoundField DataField="Commission_DisplayString" HeaderText="Commission">
                <ItemStyle Wrap="False" />
            </asp:BoundField>--%>
            <asp:BoundField DataField="ServiceCharge" HeaderText="Service Charge">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="AccruedInterest" HeaderText="Accrued Interest" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <%--<asp:BoundField DataField="Tax" HeaderText="Tax">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Tax Percentage">
                <ItemTemplate>
                    <%# ((decimal)DataBinder.Eval(Container.DataItem, "TaxPercentage") == 0 ? "" : DataBinder.Eval(Container.DataItem, "TaxPercentage"))%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TransferFee_DisplayString" HeaderText="Transfer Fee">
                <ItemStyle Wrap="False" />
            </asp:BoundField>--%>
            <asp:TemplateField HeaderText="Contractual SettlementDate">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "ContractualSettlementDate"))%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Creation Date">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")) %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TransactionType" HeaderText="Transaction Type">
                <ItemStyle Wrap="False"  />
            </asp:BoundField>
            <asp:BoundField DataField="IsStorno" HeaderText="Is Storno">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Approved" HeaderText="Approved">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ExchangeName" HeaderText="Exchange">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <%--<asp:BoundField DataField="Description" HeaderText="Description">
                <ItemStyle Wrap="False" />
            </asp:BoundField>--%>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:LinkButton 
                        ID="lbtnOrder" runat="server" 
                        Text="Show Order" 
                        Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "Order_Key") == 0 ? false : true) %>'
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Order_Key") %>'
                        />
                </ItemTemplate>                   
            </asp:TemplateField>
        </Fields>
        <FieldHeaderStyle Wrap="False" Height="14px" />
    </asp:DetailsView>
    <asp:ObjectDataSource ID="odsTrade" runat="server" SelectMethod="GetTradeDetails"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Common.OrderDetailsAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="tradeId" SessionField="TradeId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


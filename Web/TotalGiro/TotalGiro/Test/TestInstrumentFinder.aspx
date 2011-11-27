<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestInstrumentFinder.aspx.cs" Inherits="TestInstrumentFinder" Theme="Neutral" Title="Untitled Page" %>

<%@ Register Src="../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" />
    <br />
    <br />
    <asp:GridView ID="gvInstruments" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsInstruments" PageSize="20">
        <Columns>
            <asp:BoundField DataField="Isin" HeaderText="Isin" SortExpression="Isin">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="Instrument" SortExpression="Name">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="SecCategory_Description" HeaderText="Sec Category" SortExpression="SecCategory_Description">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DefaultExchangeName" HeaderText="Exchange" SortExpression="DefaultExchangeName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CurrencyNominal_Symbol" HeaderText="Symbol" SortExpression="CurrencyNominal_Symbol">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CurrencyNominal_DisplayName" HeaderText="Currency Nominal" SortExpression="CurrencyNominal_DisplayName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsInstruments" runat="server" SelectMethod="GetTradeableInstruments"
        TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                Type="Object" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="exchangeId" PropertyName="ExchangeId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                Type="Int32" />
            <asp:Parameter DefaultValue="Key, Isin, Name, SecCategory.Description, DefaultExchangeName, CurrencyNominal.DisplayName, CurrencyNominal.Symbol" Name="propertyList" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


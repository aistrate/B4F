<%@ Control Language="C#" CodeFile="InstrumentFinder.ascx.cs" Inherits="InstrumentFinder" %>

<table width="440px">
<tr>
<td>
<table>
    <tr>
        <td style="width: 130px; height: 24px">
            <asp:Label ID="lblIsin" runat="server" Text="ISIN:"></asp:Label></td>
        <td style="width: 190px">
            <asp:TextBox ID="txtIsin" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td style="height: 24px">
            <asp:Label ID="lblInstrumentName" runat="server" Text="Instrument Name:"></asp:Label></td>
        <td>
            <asp:TextBox ID="txtInstrumentName" runat="server"></asp:TextBox></td>
    </tr>
    <asp:Panel ID="pnlSecCategory" runat="server" Visible="false">
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label3" runat="server" Text="Security Category:"></asp:Label></td>
            <td>
                <asp:DropDownList ID="ddlSecCategory" runat="server" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlExchange" runat="server" Visible="false">
        <tr>
            <td style="height: 0px">
                <asp:Label ID="lblExchange" runat="server" Text="Exchange:" ></asp:Label></td>
            <td>
                <asp:DropDownList ID="ddlExchange" runat="server" DataSourceID="odsExchange" 
                    DataTextField="ExchangeName" DataValueField="Key" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter"></asp:ObjectDataSource>
            </td>
        </tr>
    </asp:Panel>
    <tr>
        <td style="height: 24px">
            <asp:Label ID="lblCurrencyNominal" runat="server" Text="Currency Nominal:"></asp:Label></td>
        <td>
            <asp:DropDownList ID="ddlCurrencyNominal" runat="server" DataSourceID="odsCurrencyNominal" 
                DataTextField="DisplayName" DataValueField="Key">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="odsCurrencyNominal" runat="server" SelectMethod="GetCurrencies"
                TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter"></asp:ObjectDataSource>
        </td>
    </tr>
    <asp:Panel ID="pnlActivityFilter" runat="server" Visible="false">
        <tr>
            <td style="height: 0px">
                <asp:Label ID="lblActivityFilter" runat="server" Text="Activity Filter:" ></asp:Label></td>
            <td>
                <asp:DropDownList ID="ddlActivityFilter" runat="server" DataSourceID="odsActivityFilter" 
                    DataTextField="Description" DataValueField="Key" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsActivityFilter" runat="server" SelectMethod="GetActivityFilterOptions"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter"></asp:ObjectDataSource>
            </td>
        </tr>
    </asp:Panel>
</table>
</td>
<td style="width: 100px; vertical-align: bottom">
    <div style="position: relative">
        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                    CommandName="SearchAccounts" CausesValidation="False" Width="90px" />
    </div>
</td>
</tr>
</table>
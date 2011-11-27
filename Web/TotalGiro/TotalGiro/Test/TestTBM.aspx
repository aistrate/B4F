<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestTBM.aspx.cs" Inherits="Orders_TBM_TBMData" Title="Tijd Beursmedia" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
<table width="100%">
    <tr>
        <td>
            <asp:DropDownList ID="ddlInstruments" runat="server" DataValueField="Isin" DataTextField="Isin"></asp:DropDownList>
            <asp:TextBox ID="txtInstrumentID" runat="server"></asp:TextBox></td>
        <td>
            <asp:Button ID="btnUpdatePrices" runat="server" Text="Update historical prices" OnClick="btnUpdatePrices_Click" />
            <asp:Button ID="btnGetQuoteCollection" runat="server" Text="Get all quotes according to contract" OnClick="btnGetQuoteCollection_Click" />
            <asp:Button ID="btnGetQuoteHistory" runat="server" Text="Get quote history for one symbol" OnClick="btnGetQuoteHistory_Click" />
            <asp:Button ID="btnCheckMissingHistPrices" runat="server" Text="Check for missing historical prices" OnClick="btnCheckMissingHistPrices_Click" /></td>
    </tr>
    <tr>
        <td><asp:TextBox ID="txtExchangeID" runat="server"></asp:TextBox></td>
    </tr>
    <tr><td colspan="2"><asp:TextBox ID="txtResults" AutoPostBack="false" runat="server" Height="500px" TextMode="MultiLine" Width="100%"></asp:TextBox></td></tr>
</table>
</asp:Content>



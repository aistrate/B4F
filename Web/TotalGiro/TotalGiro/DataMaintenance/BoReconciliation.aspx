<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="BoReconciliation.aspx.cs" Inherits="ImportFiles" Title="Importeer files van derden naar TG" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    &nbsp;<asp:Label ID="lblHistRatesUpdated" runat="server"></asp:Label>
    <br />
    <br />
    <asp:Button ID="btnImportExchangeRates" runat="server" OnClick="btnImportExchangeRates_Click"
        Text="Import Historical Exchange Rates" ToolTip="Exchanges Rates from ECB are refreshed round noon" /><br />
    <br />
    <br />
</asp:Content>


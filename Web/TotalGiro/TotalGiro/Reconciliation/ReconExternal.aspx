<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ReconExternal.aspx.cs" Inherits="Reconciliation_ReconHistorical" Title="Historical Reconciliation" Theme="Neutral" %>

<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="bodyContentPlaceHolder">
    Dit Rapport is een overzicht van alle externe reconciliaties op de ingevoerde data.<br />
    <br />
    <uc1:DatePicker ID="DatePicker1" runat="server" />
    <br />
    <br />
    <asp:Button ID="Button1" runat="server" Text="Generate Report" OnClick="Button1_Click" /></asp:Content>


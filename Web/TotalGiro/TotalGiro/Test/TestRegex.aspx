<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestRegex.aspx.cs" 
Inherits="TestRegex" Title="Untitled Page" Theme="Neutral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:Label ID="lblCounter" runat="server"></asp:Label><br />
    <br />
    <asp:GridView ID="gvMovements" runat="server" DataSourceID="ObjectDataSource1">
    </asp:GridView>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetMovements"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.BankStatementLinesAdapter"></asp:ObjectDataSource>
</asp:Content>


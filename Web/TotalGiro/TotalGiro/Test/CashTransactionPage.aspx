<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="CashTransactionPage.aspx.cs"
    Inherits="CashTransactionPage" Title="Create cash transaction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:DropDownList ID="ddlAccountA" SkinID="custom-width" runat="server" Width="200px"
        DataSourceID="odsSelectedAccountA" DataTextField="DisplayNumberWithName" DataValueField="Key"
        AutoPostBack="True" EnableViewState="true" TabIndex="1">
        <asp:ListItem></asp:ListItem>
    </asp:DropDownList>
    &nbsp;
    <asp:ObjectDataSource ID="odsSelectedAccountA" runat="server" SelectMethod="GetCustomerAccounts"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.SingleOrderAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountAFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountAFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountAFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountAFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

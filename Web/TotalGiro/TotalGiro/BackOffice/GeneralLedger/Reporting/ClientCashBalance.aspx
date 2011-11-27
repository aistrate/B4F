<%@ Page Title="Client Cash Balance" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" EnableEventValidation ="false" 
    CodeFile="ClientCashBalance.aspx.cs" Inherits="ClientCashBalance" %>

<%@ Register Src="~/UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="~/UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <table style="width: 770px">
        <tr>
            <td style="width: 160px; height: 24px">
                <asp:Label ID="Label2" runat="server" Text="Trial Balance Date:"></asp:Label>
            </td>
            <td style="width: 210px">
                <uc1:DatePicker ID="dpTransactionDateFrom" runat="server" />
            </td>
            <td style="width: 50px">                
            </td>
            <td style="width: 210px">
            </td>
            <td>
            </td>
        </tr>
    </table>
    <asp:GridView ID="gvFullBalance" runat="server" AutoGenerateColumns="False" PageSize="25"
        AllowPaging="True" Caption="Lines" CaptionAlign="Left" AllowSorting="True" DataSourceID="odsFullBalance"
        SkinID="spreadsheet" DataKeyNames="Key" Visible="True"
        OnRowDataBound="gvFullBalance_RowDataBound" >
        <Columns>
            <asp:TemplateField HeaderText="#" SortExpression="LineNumber">
                <ItemTemplate>
                    <asp:Label ID="lblLineNumber" runat="server" Width="15px">
                    <%# DataBinder.Eval(Container.DataItem, "LineNumber")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account Number" SortExpression="Account_AccountNumber">
                <ItemTemplate>
                    <uc2:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="false" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountDisplayOption="DisplayNumber"
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account Short Name" SortExpression="Account_FullDescription">
                <ItemTemplate>
                    <asp:Label ID="lblAccountShortName" runat="server" Width="200px" CssClass="padding">
                                <%# DataBinder.Eval(Container.DataItem, "Account_FullDescription")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Debit" SortExpression="Debit">
                <ItemTemplate>
                    <asp:Label ID="lblDebit" runat="server" Width="150px">
                    <%# DataBinder.Eval(Container.DataItem, "DebitDisplayString") %></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Credit" SortExpression="Credit">
                <ItemTemplate>
                    <asp:Label ID="lblCredit" runat="server" Width="150px">
                    <%# DataBinder.Eval(Container.DataItem, "CreditDisplayString")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFullBalance" runat="server" SelectMethod="GetClientCashPositionFromGLLedger"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.ClientCashPositionFromGLLedgerAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="dpTransactionDateFrom" Name="transactionDate" PropertyName="SelectedDate"
                Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
        <asp:Button ID="btnDownloadasExcel" runat="server" Text="Download As Excel" CausesValidation="False"
                        Enabled="True" OnClick="btnDownloadasExcel_Click" />&nbsp;
        

</asp:Content>

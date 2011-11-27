<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/EG.master" CodeFile="AccountValues.aspx.cs" Inherits="AccountValues" Theme="Neutral" Title="Accounts Values" %>

<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    &nbsp;<uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowRemisier="true" ShowRemisierEmployee="true" ShowModelPortfolio="true" 
        ShowContactActiveCbl="true" ShowAccountTradeabilityDdl="true" />
    <br />
    <asp:Panel ID="pnlAccounts" runat="server" Width="125px" Visible="False">
        <asp:GridView ID="gvAccounts" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" 
            DataSourceID="odsAccounts" DataKeyNames="Key" 
            Caption="Accounts" CaptionAlign="Top" PageSize="15"
            OnRowDataBound="gvAccounts_RowDataBound" >
            <Columns>
                <asp:TemplateField HeaderText="Account#" SortExpression="Number">
                    <HeaderStyle Wrap="False" />
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <ItemTemplate>
                        <uc2:AccountLabel ID="ctlAccountLabel" 
                            runat="server" 
                            RetrieveData="false" 
                            Width="120px" 
                            NavigationOption="PortfolioView"
                            AccountDisplayOption="DisplayNumber"
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Account Name" SortExpression="ShortName">
                    <ItemTemplate>
                        <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                            MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' />
                    </ItemTemplate>
                    <HeaderStyle wrap="False" />
                    <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="AccountOwner_CompanyName" HeaderText="Asset Manager" SortExpression="AccountOwner_CompanyName">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="ModelPortfolioName" HeaderText="Model" SortExpression="ModelPortfolioName">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="TotalAll" HeaderText="Value" SortExpression="TotalAll">
                    <ItemStyle wrap="False" horizontalalign="Right" />
                    <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        &nbsp;<br />
        <table>
            <tr>
                <td style="width: 55px; height: 21px;">
                    <asp:Label ID="Label1" runat="server" Text="Count:"></asp:Label></td>
                <td style="width: 55px; height: 21px;">
                    <asp:Label ID="lblCount" runat="server" Font-Bold="True" Width="140px"></asp:Label></td>
                <td style="width: 55px; height: 21px;">
                    <asp:Label ID="Label2" runat="server" Text="Total:"></asp:Label></td>
                <td style="width: 55px; height: 21px;">
                    <asp:Label ID="lblTotal" runat="server" Font-Bold="True" Width="300px"></asp:Label></td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <br />
    <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCustomerAccounts"
        TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.AccountValuesAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierId" PropertyName="RemisierId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierEmployeeId" PropertyName="RemisierEmployeeId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="lifecycleId" PropertyName="LifecycleId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showTradeable" PropertyName="AccountTradeable"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showNonTradeable" PropertyName="AccountNonTradeable"
                Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

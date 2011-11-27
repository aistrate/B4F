<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/EG.master" CodeFile="AccountOverview.aspx.cs"
    Inherits="AccountOverview" Theme="Neutral" Title="Accounts Overview" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowRemisier="true" ShowRemisierEmployee="true" ShowModelPortfolio="true" 
        ShowContactActiveCbl="true" ShowAccountTradeabilityDdl="true" ShowBsN_KvK="true" />
    <br />
    <asp:Button ID="btnAddAccountsFromEffectenGiro" ToolTip="Add Accounts from EffectenGiro"
        runat="server" Text="Add Accts from EffectenGiro" OnClick="btnAddAccountsFromEffectenGiro_Click"
        Width="190px" />
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
    <br />
    <asp:MultiView ID="mvwAccounts" runat="server" ActiveViewIndex="0" EnableTheming="True">
        <asp:View ID="vwSearchForAccounts" runat="server">
            <asp:Panel ID="pnlAccounts" runat="server" Width="125px" Visible="False">
                <asp:GridView ID="gvAccounts" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsAccounts" DataKeyNames="Key" Caption="Accounts"
                    CaptionAlign="Left" PageSize="20" OnRowDataBound="gvAccounts_RowDataBound" OnRowCommand="gvAccounts_RowCommand"
                    OnDataBinding="gvAccounts_DataBinding">
                    <Columns>
                        <asp:TemplateField HeaderText="Account#" SortExpression="Number" >
                            <ItemTemplate>
                                <uc3:AccountLabel ID="ctlAccountLabel" 
                                    runat="server" 
                                    RetrieveData="false" 
                                    Width="120px" 
                                    NavigationOption="PortfolioView"
                                    />
                            </ItemTemplate>
                            <HeaderStyle wrap="False" />
                            <ItemStyle wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Account Name" SortExpression="ShortName">
                            <ItemStyle wrap="False" />
                            <HeaderStyle wrap="False" />
                            <ItemTemplate>
                                <trunc:TruncLabel ID="trlAccountName" 
                                    runat="server"
                                    cssclass="alignright"
                                    Width="20"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' 
                                    />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remisier" SortExpression="RemisierEmployee_Remisier_Name">
                            <ItemStyle wrap="False" />
                            <HeaderStyle wrap="False" />
                            <ItemTemplate>
                                <trunc:TruncLabel ID="trlRemisier"
                                    runat="server"
                                    cssclass="alignright"
                                    Width="20"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "RemisierEmployee_Remisier_Name") %>' 
                                    />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Employee" SortExpression="RemisierEmployee_Employee_FullName">
                            <ItemStyle wrap="False" />
                            <HeaderStyle wrap="False" />
                            <ItemTemplate>
                                <trunc:TruncLabel ID="trlEmployee"
                                    runat="server"
                                    cssclass="alignright"
                                    Width="20"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "RemisierEmployee_Employee_FullName") %>' 
                                    />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Model" SortExpression="ModelPortfolioName">
                            <ItemStyle wrap="False" />
                            <HeaderStyle wrap="False" />
                            <ItemTemplate>
                                <trunc:TruncLabel ID="trlModel" 
                                    runat="server"
                                    cssclass="alignright"
                                    Width="23"
                                    Text='<%# DataBinder.Eval(Container.DataItem, "ModelPortfolioName") %>' 
                                    />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat='server' Text='<%# (AccountStati)DataBinder.Eval(Container.DataItem, "Status") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEditAccount" runat="server" CommandName="EditAccount" Text='<%# (((bool)DataBinder.Eval(Container.DataItem, "IsOpen") && UserHasEditRights) ? "Edit" : "View") %>'
                                    Visible='<%# ((AccountTypes)DataBinder.Eval(Container.DataItem, "AccountType") == AccountTypes.Customer ? true : false) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCustomerAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountOverviewAdapter">
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
            </asp:Panel>
        </asp:View>
        <asp:View ID="vwImportedAccounts" runat="server">
            <asp:Panel ID="pnlImportedAccounts" runat="server" Width="125px" Visible="False">
                <asp:GridView ID="gvImportedAccounts" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsImportedAccounts" DataKeyNames="Key"
                    Caption="Accounts created in the last 7 days" CaptionAlign="Left" PageSize="25"
                    OnRowDataBound="gvImportedAccounts_RowDataBound" OnRowCommand="gvImportedAccounts_RowCommand"
                    OnDataBinding="gvImportedAccounts_DataBinding">
                    <Columns>
                        <asp:BoundField DataField="Number" HeaderText="Account Number" SortExpression="Number">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ShortName" HeaderText="Account Name" SortExpression="ShortName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreationDate" HeaderText="Created On" DataFormatString="{0:d MMMM yyyy}"
                            SortExpression="CreationDate">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ModelPortfolioName" HeaderText="Model" SortExpression="ModelPortfolioName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat='server' Text='<%# (AccountStati)DataBinder.Eval(Container.DataItem, "Status") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEditAccount" runat="server" CommandName="EditAccount" Text='<%# (((bool)DataBinder.Eval(Container.DataItem, "IsOpen") && UserHasEditRights) ? "Edit" : "View") %>'
                                    Visible='<%# ((AccountTypes)DataBinder.Eval(Container.DataItem, "AccountType") == AccountTypes.Customer ? true : false) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsImportedAccounts" runat="server" SelectMethod="GetCustomerAccountsToday"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountOverviewAdapter">
                </asp:ObjectDataSource>
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
    <br />
</asp:Content>

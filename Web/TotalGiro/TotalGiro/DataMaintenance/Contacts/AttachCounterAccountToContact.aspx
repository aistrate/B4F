<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AttachCounterAccountToContact.aspx.cs" Inherits="DataMaintenance_AttachCounterAccountToContact" Title="AttachCounterAccountToContact" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton"
    TagPrefix="uc2" %>
<%@ Register Src="../../UC/CounterAccountFinder.ascx" TagName="CounterAccountFinder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td style="text-align:left; font-weight: bold;"><div>Contact to attach an counter account to: 
                <asp:Label ID="lblContact" runat="server" /></div>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="600px" style="border: solid 1px black;">
                    <tr>
                        <td style=" background-color: #AAB9C2; height: 20px; border-bottom: solid 1px black;">
                            <asp:Label ID="lblAccountFinder" Font-Bold="True" runat="server" 
                                Text="Find Counter Account" />
                        </td>
                    </tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr>
                        <td>
                            <uc1:CounterAccountFinder Visible="true" 
                                        ShowCounterAccountNumber="true"
                                        ShowCounterAccountName="true" 
                                        ShowContactName="true"
                                        ShowContactActiveCbl="true"
                                        ShowIsPublic="true"
                                        ID="ctlAccountFinder" 
                                        runat="server" />
                        </td>
                    </tr>                
                </table>
            </td>
        </tr>
        <asp:Panel ID="pnlAccounts" runat="server" Visible="false">
            <tr><td>&nbsp;</td></tr>
            <tr>
                <td>
                    <asp:GridView 
                          ID="gvAccounts" 
                          runat="server" 
                          AllowPaging="True" 
                          AllowSorting="True"
                          SkinID="custom-width"
                          Width="600px" 
                          AutoGenerateColumns="False"
                          DataSourceID="odsAccounts"
                          DataKeyNames="Key" 
                          Caption="Accounts" CaptionAlign="Left" PageSize="10" 
                          OnRowCommand="gvAccounts_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BankName" HeaderText="Bank" SortExpression="BankName" ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AccountName" HeaderText="Account Name" SortExpression="AccountName" ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="IsPublic" HeaderText="General Account" SortExpression="IsPublic" ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton
                                         ID="lbtnAdd"
                                         runat="server"
                                         CommandName="AddAccount"
                                         Text="Attach" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCounterAccounts"
                        TypeName="B4F.TotalGiro.ApplicationLayer.UC.CounterAccountFinderAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                Type="Int32" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="counterAccountNumber" PropertyName="CounterAccountNumber"
                                Type="String" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="counterAccountName" PropertyName="CounterAccountName"
                                Type="String" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactName" PropertyName="ContactName"
                                Type="String" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                Type="String" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                                Type="Boolean" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                                Type="Boolean" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="isPublic" PropertyName="IsPublic"
                                Type="Boolean" />
                            <asp:Parameter Name="propertyList" DefaultValue="Key, BankName, Number, AccountName, IsPublic"
                                Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
       </asp:Panel>
            <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" style="width: 100%">
                    <tr>
                        <td style="width: 7%;">
                            <uc2:FirstPreviousPageBackButton ID="btnBack" runat="server" />
                        
                        </td>
                        <td style="width: 93%;">
                            <asp:Button ID="btnAddNewAccount" runat="server" Text="Create  Counter Account" 
                                OnClick="btnAddNewAccount_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <asp:Label ID="lblError" runat="server" ForeColor="Red" />
        </tr>
    </table>
</asp:Content>


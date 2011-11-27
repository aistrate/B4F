<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="AttachAccountToContact.aspx.cs" Inherits="DataMaintenance_AttachAccountToContact"
    Title="Attach Account To Contact" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton"
    TagPrefix="uc2" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton2" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="text-align: left; font-weight: bold;">
                <div>
                    Contact to attach an account to:
                    <asp:Label ID="lblContact" runat="server" /></div>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="600px" style="border: solid 1px black;">
                    <tr>
                        <td style="background-color: #AAB9C2; height: 20px; border-bottom: solid 1px black;">
                            <asp:Label ID="lblAccountFinder" Font-Bold="true" runat="server" Text="Find Account" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:AccountFinder Visible="true" ShowTegenrekening="false" ShowAccountName="false"
                                ID="ctlAccountFinder" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <asp:Panel ID="pnlAccounts" runat="server" Visible="false">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvAccounts" runat="server" AllowPaging="True" AllowSorting="True"
                        SkinID="custom-width" Width="600px" AutoGenerateColumns="False" DataSourceID="odsAccounts"
                        DataKeyNames="Key" Caption="Accounts" CaptionAlign="Left" PageSize="10" OnRowCommand="gvAccounts_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Number" HeaderText="Account Number" SortExpression="Number"
                                ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ShortName" HeaderText="Account Name" SortExpression="ShortName"
                                ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AccountOwner_CompanyName" HeaderText="Asset Manager" SortExpression="AccountOwner_CompanyName"
                                ReadOnly="True">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnAdd" runat="server" CommandName="AddAccount" Text="Attach" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCustomerAccounts"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AttachAccountToContactEditAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                Type="Int32" />
                            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                Type="String" />
                            <asp:Parameter Name="propertyList" DefaultValue="Key, ShortName, Number, AccountOwner.CompanyName, ModelPortfolioName"
                                Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" style="width: 100%">
                    <tr>
                        <td style="width: 7%;">
                            <uc2:FirstPreviousPageBackButton ID="btnBack" runat="server" />
                        </td>
                        <td style="width: 93%;">
                            <asp:Button ID="btnAddNewAccount" runat="server" Text="Create and attach Account" OnClick="btnAddNewAccount_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    
    <asp:Panel ID="pnlCreateNewAccounts" runat="server" Visible="false">
        <table>
            <tr>
                <td colspan="4" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                    border-bottom: solid 1px black;">
                    <asp:Label ID="lblCreateNewAccount" Font-Bold="true" runat="server" Text="Complete minimal requirements for Account" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblAcctPrefix" Font-Bold="False" runat="server" Text="Choose Account Prefix" />
                </td>
                <td>
                </td>
                <td>
                    <asp:DropDownList ID="ddlAccountFamily" runat="server" Width="165px" DataSourceID="odsAccountFamily"
                        DataTextField="AccountPrefix" DataValueField="Key" AutoPostBack="False">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator 
                        ID="rfvAccountFamily" 
                        runat="server"
                        ControlToValidate="ddlAccountFamily"
                        SetFocusOnError="True"
                        InitialValue="-2147483648" 
                        ErrorMessage="Account Family is mandatory">*
                    </asp:RequiredFieldValidator>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblAccountName" Font-Bold="False" runat="server" Text="AccountName" />
                </td>
                <td>
                </td>
                <td style="height: 25px; width: 317px;">
                    <asp:TextBox ID="tbShortName" MaxLength="50" runat="server" Width="256px"></asp:TextBox>
                    <asp:RequiredFieldValidator 
                        ID="rfvAccountName" 
                        runat="server" 
                        ErrorMessage="AccountName is mandatory"
                        ControlToValidate="tbShortName">*
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSave" ToolTip="Save and Create Account" runat="server" Text="Save and Create Account"
                        OnClick="btnSave_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE ACCOUNT WAS NOT CREATED. Correct the following fields:"
                        ID="valSum" runat="server" />
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="odsAccountFamily" runat="server" SelectMethod="GetAccountFamilies"
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountOverviewAdapter">
            <SelectParameters>
                <asp:Parameter Name="accountID" Type="Int32" DefaultValue="0" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>

</asp:Content>

<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AttachCompanyToContactperson.aspx.cs" Inherits="DataMaintenance_AttachCompanyToContactperson" Title="Attach Company To Contactperson" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton"
    TagPrefix="uc2" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton2" runat="server" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="text-align:left; font-weight: bold;"><div>Person to attach a company to: 
                <asp:Label ID="lblPerson" runat="server" /></div>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="600px" style="border: solid 1px black;">
                    <tr>
                        <td style=" background-color: #AAB9C2; height: 20px; border-bottom: solid 1px black;">
                            <asp:Label ID="lblCompanyFinder" Font-Bold="true" runat="server" Text="Find Company" />
                        </td>
                    </tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr>
                        <td>
                            <uc1:AccountFinder 
                                Visible="true" 
                                ShowTegenrekening="false"
                                ShowAccountNumber="false" 
                                ID="ctlContactFinder"
                                AccountNameLabel="Company Name:" 
                                runat="server" />
                        </td>
                    </tr>                
                </table>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <asp:Panel runat="server"
                    ID="pnlContactOverview"
                    Visible="false">
            <tr>
                <td>
                    <asp:GridView ID="gvContactOverview"
                        AutoGenerateColumns="false"
                        PageSize="10"
                        SkinID="custom-width"
                        DataSourceID="odsContactOverview"
                        Width="600px"
                        Caption="Company"
                        OnRowCommand="gvContactOverview_RowCommand"
                        AllowPaging="True" 
                        AllowSorting="True" 
                        DataKeyNames="Key"
                        runat="server">                       
                        <Columns>
                           <asp:BoundField DataField="CurrentNAW_Name" HeaderText="Company Name" SortExpression="CurrentNAW_Name">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="GetBSN"
                                            SortExpression="GetBSN"
                                            HeaderText="KVK">
                                <ItemStyle Wrap="false" />
                                <HeaderStyle Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CurrentNAW_ResidentialAddress_DisplayAddress"
                                            SortExpression="CurrentNAW_ResidentialAddress_DisplayAddress"
                                            HeaderText="Address">
                                <ItemStyle Wrap="false" />
                                <HeaderStyle Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ContactDetails_Telephone_Number"
                                            SortExpression="ContactDetails_Telephone_Number"
                                            HeaderText="Telephone">
                                <ItemStyle Wrap="false" />
                                <HeaderStyle Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ContactDetails_Mobile_Number"
                                            SortExpression="ContactDetails_Mobile_Number"
                                            HeaderText="Mobile">
                                <ItemStyle Wrap="false" />
                                <HeaderStyle Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ContactDetails_Email"
                                            SortExpression="ContactDetails_Email"
                                            HeaderText="Email">
                                <ItemStyle Wrap="false" />
                                <HeaderStyle Wrap="false" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton
                                         ID="lbtnSelect"
                                         runat="server"
                                         CommandName="AddContact"
                                         Text="Attach" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsContactOverview" runat="server" SelectMethod="GetCompanies"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ContactPersonEditAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ctlContactFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                Type="Int32" />
                            <asp:ControlParameter ControlID="ctlContactFinder" Name="accountNumber" PropertyName="AccountNumber"
                                Type="String" />
                            <asp:ControlParameter ControlID="ctlContactFinder" Name="contactName" PropertyName="AccountName"
                                Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
            <tr><td>&nbsp;</td></tr>
        </asp:Panel>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" style="width: 100%">
                    <tr>
                        <td style="width: 7%;">
                            <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton1" runat="server" />
                        
                        </td>
                        <td style="width: 93%;">
                            <asp:Button ID="btnAddNewCompany" runat="server" Text="Create Company" Enabled="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    

</asp:Content>


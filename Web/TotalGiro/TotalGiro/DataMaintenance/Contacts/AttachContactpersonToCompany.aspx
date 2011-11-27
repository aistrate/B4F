<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AttachContactpersonToCompany.aspx.cs" Inherits="DataMaintenance_AttachContactpersonToCompany" Title="Attach Contactperson To Company" %>

<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td colspan="4">
                <uc3:BackButton ID="BackButton" runat="server" />
                <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton1" runat="server" />
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td style="text-align:left; font-weight: bold;"><div>Company to attach a contactperson to: 
                <asp:Label ID="lblCompany" runat="server" /></div>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="600px" style="border: solid 1px black;">
                    <tr>
                        <td style=" background-color: #AAB9C2; height: 20px; border-bottom: solid 1px black;">
                            <asp:Label ID="lblCompanyFinder" Font-Bold="true" runat="server" Text="Find Person" />
                        </td>
                    </tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr>
                        <td>
                            <uc1:AccountFinder 
                                Visible="true" 
                                ShowTegenrekening="false"
                                ShowAccountNumber="false"
                                AccountNameLabel="FamiliyName:"  
                                ID="ctlContactFinder" 
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
                    Width="600px"
                    Caption="Persons"
                    DataSourceID="odsContactOverview"
                    OnRowCommand="gvContactOverview_RowCommand"
                    AllowPaging="True" 
                    AllowSorting="True" 
                    DataKeyNames="Key"
                    runat="server">
                    <Columns>
                        <asp:BoundField DataField="FullName" 
                                    HeaderText="FullName" 
                                    SortExpression="FullName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="GetBSN"
                                        SortExpression="GetBSN"
                                        HeaderText="BSN">
                            <ItemStyle Wrap="false" />
                            <HeaderStyle Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="GetBirthFounding"
                                        SortExpression="GetBirthFounding"
                                        HeaderText="Birth Date" DataFormatString="{0:d MMM yyyy}" HtmlEncode="False"   >
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
                <asp:ObjectDataSource ID="odsContactOverview" runat="server" SelectMethod="GetPersons"
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
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="width: 93%;">
                            <asp:Button ID="btnAddNewPerson" runat="server" Text="Create Person" 
                                onclick="btnAddNewPerson_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    

</asp:Content>


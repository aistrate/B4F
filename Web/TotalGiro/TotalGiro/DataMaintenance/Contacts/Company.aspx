<%@ Page Language="C#" StylesheetTheme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="Company.aspx.cs" Inherits="DataMaintenance_Company" Title="Company" %>

<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc1" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc3" %>
<%@ Register Src="../../UC/Accounts.ascx" TagName="Accounts" TagPrefix="ucAccounts" %>
<%@ Register Src="../../UC/ContactDetails.ascx" TagName="ContactDetails" TagPrefix="ucContact" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="dp" %>
<%@ Register Src="../../UC/Address.ascx" TagName="Address" TagPrefix="ucAddress" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <asp:Label runat="server" ID="mess" Text="" />
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="4">
                    <uc3:BackButton ID="BackButton" runat="server" />
                    <uc1:FirstPreviousPageBackButton ID="btnBackAbove" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px" Font-Bold="true"></b4f:ErrorLabel>
                    <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:" 
                                           ID="valSum" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top;">
                
                    <asp:Panel ID="pnlCompanyDetailsHeader" runat="server" Width="800px" CssClass="PanelCollapsible"> 
                            <div class="divPanelCollapsible">Company Details
                                <asp:LinkButton ID="lbtnCompanyDetails" runat="server">
                                    <asp:Label ID="lblCompanyDetails" SkinID="Header" runat="server"></asp:Label>
                                </asp:LinkButton>
                             </div>            
                    </asp:Panel>      
                    <asp:Panel ID="pnlCompanyDetails" runat="server">
                        <asp:UpdatePanel ID="upDetails" runat="server">
                            <ContentTemplate>
                                 <table style="width: 801px" cellspacing="0" cellpadding="0" class="TabArea" border="1">
                                    <tr>
                                        <td>
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td style="vertical-align: middle; height: 19px; text-align: right; width: 23.1%;">
                                                        <asp:Label ID="lblCompanyName" runat="server" Text="Company Name"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: left; height: 19px; width: 29.9%;">
                                                        <asp:TextBox ID="tbCompanyName" MaxLength="100" runat="server" SkinID="broad"></asp:TextBox>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: left; padding-left: 3px; width: 2%;">
                                                        <asp:CustomValidator ID="cvCompanyName" 
                                                            ValidateEmptyText="true" runat="server" 
                                                            OnServerValidate="CompanyNameNotEmpty" ControlToValidate="tbCompanyName" 
                                                            ErrorMessage="Company Name">*</asp:CustomValidator>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: right; width: 12%;">
                                                        <asp:Label ID="lblKvKNumber" runat="server" Text="KvK Number"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: left; height: 19px; width: 26%">
                                                        <asp:TextBox ID="tbKvKNumber" MaxLength="10" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: left; width: 7%;">
                                                        <asp:RequiredFieldValidator ID="rfKvKNumber" runat="server" ControlToValidate="tbKvKNumber" ErrorMessage="KvK Number">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                               
                                                <tr>
                                                    <td style="text-align: right; vertical-align: middle; height: 28px;">
                                                        <asp:Label ID="lblDateOfFoundation" runat="server" Text="Foundation date"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: left; height: 28px;">
                                                        <dp:DatePicker ID="ucFoundation" ListYearsBeforeCurrent="70" runat="server" />            
                                                    </td>
                                                    <td style="height: 20px;">
                                                        <asp:RequiredFieldValidator 
                                                            ID="reqDateOfFoundation" 
                                                            ErrorMessage="Date of Foundation" 
                                                            ControlToValidate="ucFoundation:txtDate" 
                                                            runat="server">*</asp:RequiredFieldValidator>
                                                    </td>
                                                    <td style="text-align: right;">
                                                        <asp:Label ID="lblActive" runat="server" Text="Active"></asp:Label>
                                                    </td>
                                                    <td colspan="2" style="text-align: left;">
                                                      <asp:CheckBox ID="cbActive" runat="server" />  
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" style="padding-left: 28px;">
                                                        <ucContact:ContactDetails ID="ucContactDetails" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </td>
            </tr>
            
            <tr>
                <td>
                    <asp:Panel CssClass="PanelCollapsible" ID="pnlCompanyAddressesHeader" runat="server" Width="800px" Height="30px"> 
                            <div class="divPanelCollapsible">Addresses
                                <asp:LinkButton ID="lbtnCompanyAddresses" runat="server">
                                    <asp:Label ID="lblCompanyAddresses" SkinID="Header" runat="server"></asp:Label>
                                </asp:LinkButton></div> 
                    </asp:Panel> 
                    <asp:Panel ID="pnlCompanyAddresses" runat="server">
                        <asp:UpdatePanel ID="updPnlUcAddress" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table width="801px" cellpadding="0" cellspacing="0" class="TabArea">
                                    <tr>
                                        <td>
                                            <ucAddress:Address ID="ucAddress" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Panel CssClass="PanelCollapsibleBottom" ID="pnlCounterAccountHeader" runat="server" Width="800px" Height="30px"> 
                            <div class="divPanelCollapsible" style="padding:5px">Counter Accounts
                               <asp:LinkButton ID="lbtnCounterAccount" runat="server">
                                    <asp:Label ID="lblCounterAccount" SkinID="Header" runat="server"></asp:Label>
                                </asp:LinkButton>
                             </div>            
                    </asp:Panel>   
                    <asp:Panel ID="pnlCounterAccount" runat="server">
                                <table style="width: 801px; border-bottom: solid 1px black; border-left: solid 1px black; border-right: solid 1px black;" cellpadding="0" cellspacing="0">
                                    <tr style="font-size: 8px; height: 8px;"><td>&nbsp;</td></tr>
                                    <tr>
                                        <td style="width: 81px;">&nbsp;</td>
                                        <td style=" width: 705px; vertical-align: top;">
                                            <asp:GridView
                                                runat="server"
                                                PageSize="5"
                                                Width="600px"
                                                DataKeyNames="Key"
                                                SkinID="custom-width"
                                                AllowSorting="True"
                                                DataSourceID="odsCounterAccounts"
                                                AutoGenerateColumns="false"
                                                ID="gvCounterAccounts" 
                                                OnRowDataBound="gvCounterAccounts_RowDataBound" OnRowCommand="gvCounterAccounts_RowCommand">
                                                <Columns>
                                                    <asp:BoundField HeaderText="Bank" DataField="BankName" SortExpression="BankName" />
                                                    <asp:BoundField HeaderText="Number" DataField="Number" SortExpression="Number" />
                                                    <asp:BoundField HeaderText="Account Name" DataField="AccountName" SortExpression="AccountName" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton
                                                                 ID="lbtnEditCounterAccount"
                                                                 runat="server"
                                                                 CausesValidation="false"
                                                                 Enabled="false"
                                                                 ToolTip="Edit this counter account."
                                                                 CommandName="EditCounterAccount"
                                                                 Text="Edit Counter Account" />
                                                            <asp:LinkButton
                                                                 ID="lbtnDetachContactperson"
                                                                 runat="server"
                                                                 CausesValidation="false"
                                                                 Enabled="false"
                                                                 ToolTip="Detaches this counter account from the contactperson."
                                                                 CommandName="DetachContactperson"
                                                                 Text="Detach" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:ObjectDataSource ID="odsCounterAccounts" runat="server" SelectMethod="GetCounterAccounts"
                                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.CompanyEditAdapter">
                                                <SelectParameters>
                                                    <asp:SessionParameter Name="companyID" SessionField="contactid" Type="Int32" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr style="font-size: 5px; height: 5px;"><td>&nbsp;</td></tr>
                                     <tr>
                                        <td>&nbsp;</td>
                                        <td style="width: 705px">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td style=" width: 201px;">
                                                      <asp:Button 
                                                                ID="btnAttachCounterAccountToPerson" 
                                                                runat="server"
                                                                Enabled="false"
                                                                CausesValidation="false"
                                                                PostBackUrl="~/DataMaintenance/Contacts/AttachCounterAccountToContact.aspx" 
                                                                Text="Attach Counter Account" />
                                                    </td>
                                                    <td  style=" width: 600px;">
                                                        &nbsp;</td>
                                                </tr>
                                            </table>

                                        </td>
                                    </tr>
                                    <tr style="font-size: 8px; height: 8px;"><td>&nbsp;</td></tr>
                                </table>
                        </asp:Panel>  
                    </td>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    <asp:Panel CssClass="PanelCollapsibleBottom" ID="pnlCompanyAccountsHeader" runat="server" Width="800px" Height="30px"> 
                        <div style="padding:5px">Accounts
                           <asp:LinkButton ID="lbtnCompanyAccounts" runat="server">
                                <asp:Label ID="lblCompanyAccounts" SkinID="Header" runat="server"></asp:Label>
                            </asp:LinkButton>
                         </div>            
                    </asp:Panel>   
                    <asp:Panel ID="pnlCompanyAccounts" runat="server">
                        <table style="width: 801px; border-left: solid 1px black; border-right: solid 1px black;" cellpadding="0" cellspacing="0">
                            <tr style="font-size: 8px; height: 8px;"><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td style="width: 81px;">&nbsp;</td>
                                <td style=" width: 720px; vertical-align: top;">
                                    <ucAccounts:Accounts setGvWidth="600px" ID="ucAccounts" runat="server" />
                                </td>
                            </tr>
                            <tr style="font-size: 5px; height: 5px;"><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td style=" width: 201px;">
                                              <asp:Button 
                                                    ID="btnAddCompanyToAccount" 
                                                    runat="server"
                                                    Enabled="false"
                                                    CausesValidation="false"
                                                    PostBackUrl="~/DataMaintenance/Contacts/AttachAccountToContact.aspx" 
                                                    Text="Attach Account" />
                                            </td>
                                            <td style=" width: 600px;">
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr style="font-size: 8px; height: 8px;"><td>&nbsp;</td></tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                     <asp:Panel CssClass="PanelCollapsibleBottom" ID="pnlCompanyContactsHeader" runat="server" Width="800px" Height="30px"> 
                            <div style="padding:5px">Contactpersons
                               <asp:LinkButton ID="lbtnCompanyContacts" runat="server">
                                    <asp:Label ID="lblCompanyContacts" SkinID="Header" runat="server"></asp:Label>
                                </asp:LinkButton>
                             </div>            
                    </asp:Panel>   
                     <asp:Panel ID="pnlCompanyContacts" runat="server">
                        <table style="width: 801px; border-bottom: solid 1px black; border-left: solid 1px black; border-right: solid 1px black;" cellpadding="0" cellspacing="0">
                            <tr style="font-size: 8px; height: 8px;"><td>&nbsp;</td></tr>
                            <tr>
                                    <td style="width: 81px;">&nbsp;</td>
                                    <td style=" width: 720px; vertical-align: top;">
                                            <asp:GridView
                                                 runat="server"
                                                 AutoGenerateColumns="false"
                                                 Width="600px"
                                                 SkinID="custom-width"
                                                 AllowSorting="True"
                                                 DataSourceID="odsContactPerson"
                                                 DataKeyNames="ContactPerson_Key"
                                                 ID="gvContactPerson" 
                                                 OnRowCommand="gvContactPerson_RowCommand" OnRowDataBound="gvContactPerson_RowDataBound">
                                                 <Columns>
                                                    <asp:BoundField
                                                         DataField="ContactPerson_FullName"
                                                         HeaderText="Name"
                                                         SortExpression="ContactPerson_CurrentNAW_Name" >
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ContactPerson_GetBSN"
                                                                    SortExpression="ContactPerson_GetBSN"
                                                                    HeaderText="BSN">
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ContactPerson_GetBirthFounding"
                                                                    SortExpression="ContactPerson_GetBirthFounding"
                                                                    HeaderText="Birth Date" DataFormatString="{0:d MMM yyyy}" HtmlEncode="False"   >
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ContactPerson_CurrentNAW_ResidentialAddress_DisplayAddress"
                                                                    SortExpression="ContactPerson_CurrentNAW_ResidentialAddress_DisplayAddress"
                                                                    HeaderText="Address">
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField>
                                                    <%-- <asp:BoundField DataField="ContactPerson_ContactDetails_Telephone_Number"
                                                                    SortExpression="ContactDetails_Telephone_Number"
                                                                    HeaderText="Telephone">
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ContactPerson_ContactDetails_Mobile_Number"
                                                                    SortExpression="ContactDetails_Mobile_Number"
                                                                    HeaderText="Mobile">
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ContactPerson_ContactDetails_Email"
                                                                    SortExpression="ContactDetails_Email"
                                                                    HeaderText="Email">
                                                        <ItemStyle Wrap="false" />
                                                        <HeaderStyle Wrap="false" />
                                                    </asp:BoundField> --%>
                                                    <asp:TemplateField HeaderText="AuthorizedSignature">
                                                        <ItemTemplate>
                                                            <asp:Label 
                                                                    runat="server" ID="lblAuthorizedSignature"
                                                                    Text='<%# Convert.ToBoolean(Eval("AuthorizedSignature")) == true ? "Yes" : "No" %>'
                                                            />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                   <%-- <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton
                                                                 ID="lbtnEditContactperson"
                                                                 runat="server"
                                                                 CausesValidation="false"
                                                                 CommandName="EditContactPerson"
                                                                 Text="Edit Person" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton
                                                                 ID="lbtnDetachContactperson"
                                                                 runat="server"
                                                                 Enabled="false"
                                                                 CausesValidation="false"
                                                                 ToolTip="Detaches this contact as a contactperson for the company."
                                                                 CommandName="DetachContactperson"
                                                                 Text="Detach" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                 </Columns>
                                            </asp:GridView>
                                            <asp:ObjectDataSource 
                                                ID="odsContactPerson" 
                                                runat="server" 
                                                SelectMethod="GetCompanyContactPersons" 
                                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.CompanyEditAdapter">
                                                <SelectParameters>
                                                    <asp:SessionParameter DefaultValue="0" Name="companyKey" SessionField="contactid" Type="Int32" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr style="font-size: 5px; height: 5px;"><td>&nbsp;</td></tr>
                                     <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td style=" width: 201px;">
                                                      <asp:Button 
                                                                ID="btnAttachPersonToCompany" 
                                                                runat="server"
                                                                Enabled="false"
                                                                CausesValidation="false"
                                                                PostBackUrl="~/DataMaintenance/Contacts/AttachContactpersonToCompany.aspx" 
                                                                Text="Attach Person" />
                                                    </td>
                                                    <td  style=" width: 600px;">
                                                        &nbsp;</td>
                                                </tr>
                                            </table>

                                        </td>
                                    </tr>
                                    <tr style="font-size: 8px; height: 8px;"><td>&nbsp;</td></tr>
                                </table>
                     </asp:Panel>
                </td>
            </tr>
            <tr><td>&nbsp;</td></tr>
            <tr>
                <td>
                    <asp:Button ID="btnSaveBypass" 
                        runat="server"
                        CausesValidation="false" 
                        Enabled="false"
                        OnClick="btnSaveCompany_Click" 
                        ToolTip="Bypasses required field validation" Text="Save incomplete NAR" />&nbsp;
                    <asp:Button ID="bntSave" Enabled="false" runat="server" OnClick="btnSaveCompany_Click" ToolTip="Save Person" Text="Save" />
                </td>
              </tr>
        </table>
        <script type="text/javascript">
            if (document.getElementById('hfMessSaveSucceed')) {
                alert(document.getElementById('hfMessSaveSucceed').value);
            }
        </script>
      
    <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpe" runat="server"
        TargetControlID="pnlCompanyDetails" 
        ExpandControlID="lbtnCompanyDetails" 
        CollapseControlID="lbtnCompanyDetails"  
        Collapsed="false" 
        TextLabelID="lblcompanyDetails" 
        CollapsedText="show" 
        ExpandedText="hide" 
        SuppressPostBack="true"/>
     <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpeCompanyAddresses"
        runat="server"
        TargetControlID="pnlCompanyAddresses" 
        ExpandControlID="lbtnCompanyAddresses" 
        CollapseControlID="lbtnCompanyAddresses"  
        Collapsed="false" 
        TextLabelID="lblCompanyAddresses" 
        CollapsedText="show" 
        ExpandedText="hide" 
        SuppressPostBack="true"/>
    <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpeCounterAccount"
        runat="server"
        TargetControlID="pnlCounterAccount"
        ExpandControlID="lbtnCounterAccount" 
        CollapseControlID="lbtnCounterAccount"  
        Collapsed="false" 
        TextLabelID="lblCounterAccount" 
        CollapsedText="show" 
        ExpandedText="hide" 
        SuppressPostBack="true"/>
    <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpeCompanyAccounts"
        runat="server"
        TargetControlID="pnlCompanyAccounts"
        ExpandControlID="lbtnCompanyAccounts" 
        CollapseControlID="lbtnCompanyAccounts"  
        Collapsed="false" 
        TextLabelID="lblCompanyAccounts" 
        CollapsedText="show" 
        ExpandedText="hide" 
        SuppressPostBack="true"/>
    <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpeCompanyContacts"
        runat="server"
        TargetControlID="pnlCompanyContacts"
        ExpandControlID="lbtnCompanyContacts" 
        CollapseControlID="lbtnCompanyContacts"  
        Collapsed="false" 
        TextLabelID="lblCompanyContacts" 
        CollapsedText="show" 
        ExpandedText="hide" 
        SuppressPostBack="true"/>
</asp:Content>
                
   
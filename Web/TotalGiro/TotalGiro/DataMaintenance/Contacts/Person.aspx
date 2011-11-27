<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="Person.aspx.cs" Inherits="DataMaintenance_Person" Title="Person" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc1" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="dp" %>
<%@ Register Src="../../UC/Accounts.ascx" TagName="Accounts" TagPrefix="ucAccounts" %>
<%@ Register Src="../../UC/ContactDetails.ascx" TagName="ContactDetails" TagPrefix="ucContactDetails" %>
<%@ Register Src="../../UC/Address.ascx" TagName="Address" TagPrefix="ucAddress" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="sm"
        runat="Server" />
    <input id="hfViewState" type="hidden" name="hfViewState" />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td colspan="4">
                <uc2:BackButton ID="BackButton" runat="server" />
                <uc1:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton2" runat="server" />
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
            <td style="width: 2%; height: 24px;">
                <asp:CheckBox ID="cbContactPersonOnly" AutoPostBack="true" runat="server" OnCheckedChanged="cbContactPersonOnly_CheckedChanged" />
            </td>
            <td style="width: 18%; height: 24px;">
                <asp:Label ID="lblCbContactPerson" runat="server" Text="Only contactperson"></asp:Label>
            </td>
            <td style="width: 2%; height: 24px;">
                <asp:CheckBox ID="cbActive" runat="server" />
            </td>
            <td style="width: 78%; height: 24px;">
                <asp:Label ID="lblActive" runat="server" Text="Active" />
            </td>
        </tr>
        <tr style="font-size: 3px; height: 3px;">
            <td colspan="4" style="height: 2px">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlDetailsHeader" runat="server" Width="800px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Personal Details
                        <asp:LinkButton ID="lbtnDetails" runat="server">
                            <asp:Label ID="lblDetails" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlDetails" runat="server">
                    <asp:UpdatePanel ID="upDetails" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 780px;">
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblLastName" runat="server" Text="Last Name"></asp:Label>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:TextBox ID="tbLastName" runat="server" MaxLength="30" SkinID="broad" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:CustomValidator ID="customValLastName" ValidateEmptyText="true" runat="server"
                                                        OnServerValidate="LastNameNotEmpty" ControlToValidate="tbLastName" ErrorMessage="LastName">*</asp:CustomValidator>
                                                </td>
                                                <td colspan="2" style="text-align: left;">
                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="vertical-align: middle; height: 25px; text-align: right; width: 34%;">
                                                                <asp:Label ID="lblInitials" runat="server">Initials</asp:Label>
                                                            </td>
                                                            <td style="vertical-align: middle; height: 25px; text-align: left; width: 20%;">
                                                                <asp:TextBox ID="tbInitials" MaxLength="20" Width="50px" SkinID="small" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ErrorMessage="Initials" ID="reqInitials" runat="server"
                                                                    ControlToValidate="tbInitials">*</asp:RequiredFieldValidator>
                                                            </td>
                                                            <td style="vertical-align: middle; height: 25px; text-align: left; width: 10%;">
                                                                <asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label>
                                                            </td>
                                                            <td style="vertical-align: middle; height: 21px; text-align: left; width: 36%;">
                                                                <asp:TextBox ID="tbTitle" SkinID="small" Width="50px" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name"></asp:Label>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <asp:TextBox ID="tbMiddleName" MaxLength="10" runat="server" SkinID="small"></asp:TextBox>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                </td>
                                                <td style="text-align: right; vertical-align: middle; height: 25px; width: 374px;">
                                                    <asp:Label ID="lblBirthDate" runat="server" Text="Date of Birth"></asp:Label>
                                                </td>
                                                <td>
                                                    <dp:DatePicker ID="ucBirthDate" ListYearsBeforeCurrent="100" ListYearsAfterCurrent="100"
                                                        runat="server" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="reqDateOfFoundation" ErrorMessage="Date of Birth"
                                                        ControlToValidate="ucBirthDate:txtDate" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <asp:Panel ID="pnlPersonAsAccountHolderDetails" runat="server">
                                                <tr>
                                                    <td style="vertical-align: middle; height: 19px; text-align: right; width: 490px;">
                                                        <asp:Label ID="lblGender" runat="server" Text="Gender"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: left; width: 201px;">
                                                        <asp:RadioButtonList ID="rbGender" RepeatDirection="Horizontal" runat="server">
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: left; width: 2px;">
                                                        <asp:RequiredFieldValidator ID="reqGender" runat="server" ErrorMessage="Gender" ControlToValidate="rbGender">*</asp:RequiredFieldValidator>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                        <asp:Label ID="lblNationality" runat="server" Text="Nationality"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                        <asp:DropDownList ID="ddNationality" runat="server" />
                                                    </td>
                                                    <td style="height: 19px; width: 95px;">
                                                        <asp:RangeValidator ID="rangeNationality" ErrorMessage="Nationality" runat="server"
                                                            ControlToValidate="ddNationality" Type="Integer" MinimumValue="0" MaximumValue="1000000">*</asp:RangeValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="vertical-align: middle; height: 21px; text-align: right; width: 490px;">
                                                        <asp:Label ID="lblIDType" runat="server">ID Type</asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 21px; text-align: left; width: 201px;">
                                                        <asp:DropDownList ID="ddIDType" runat="server" />
                                                    </td>
                                                    <td style="width: 2px">
                                                        <asp:RangeValidator runat="server" ID="rangeIDType" ControlToValidate="ddIDType"
                                                            Type="Integer" MinimumValue="0" ErrorMessage="ID Type" MaximumValue="1000000">*</asp:RangeValidator>
                                                    </td>
                                                    <td style="height: 16px; text-align: right; vertical-align: middle; height: 25px;
                                                        width: 374px;">
                                                        <asp:Label ID="lblIdentificationID" runat="server" Text="ID Number"></asp:Label>
                                                    </td>
                                                    <td style="height: 16px; vertical-align: middle; text-align: left; width: 535px;">
                                                        <asp:TextBox ID="tbIdentificationID" runat="server"></asp:TextBox><br />
                                                    </td>
                                                    <td style="vertical-align: middle; height: 16px; text-align: left; width: 95px;">
                                                        <asp:RequiredFieldValidator ID="reqIDNumber" runat="server" ErrorMessage="ID Number"
                                                            ControlToValidate="tbIdentificationID">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                        <asp:Label ID="lblIdExpirationDate" runat="server" Text="ID Expiration Date"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <dp:DatePicker ID="ucIdExpirationDate" ListYearsBeforeCurrent="2" ListYearsAfterCurrent="15" CalenderHeight="100px"
                                                            runat="server" />
                                                    </td>
                                                    <td>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ErrorMessage="ID Expiration Date"
                                                            ControlToValidate="ucIdExpirationDate:txtDate" runat="server">*</asp:RequiredFieldValidator>
                                                    </td>
                                                    <td style="width: 374px; text-align: right; height: 24px; vertical-align: middle">
                                                        <asp:Label ID="lblBurgerServiceNummer" runat="server">BurgerServiceNummer</asp:Label>
                                                    </td>
                                                    <td style="width: 535px; vertical-align: middle; text-align: left; height: 24px;">
                                                        <asp:TextBox ID="tbBurgerServiceNummer" MaxLength="9" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="vertical-align: middle; width: 95px; height: 24px; text-align: left">
                                                        <asp:RequiredFieldValidator ID="reqBurgerServiceNummer" runat="server" ErrorMessage="BurgerServiceNummer"
                                                            ControlToValidate="tbBurgerServiceNummer">*</asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="customValBurgerServiceNummer" ErrorMessage="The BurgerServiceNummer is not correct."
                                                            ControlToValidate="tbBurgerServiceNummer" runat="server" OnServerValidate="customValBurgerServiceNummer_ServerValidate">*</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                    </td>
                                                    <td style="width: 374px; text-align: right; height: 24px; vertical-align: middle">
                                                        <asp:Label ID="lblResidentStatus" runat="server">Resident Status</asp:Label>
                                                    </td>
                                                    <td style="vertical-align: middle; height: 19px; text-align: left; width: 201px;">
                                                        <asp:RadioButtonList ID="rbResidentStatus" RepeatDirection="Horizontal" runat="server">
                                                        </asp:RadioButtonList>
                                                        <asp:CustomValidator ID="cvResidentStatus" runat="server" ErrorMessage="The BurgerServiceNummer is mandatory for residents."
                                                            ControlToValidate="rbResidentStatus" runat="server" OnServerValidate="cvResidentStatus_ServerValidate">*</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <ucContactDetails:ContactDetails ID="ucContactDetails" runat="server" />
                                                    </td>
                                                </tr>
                                            </asp:Panel>
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
            <td colspan="4">
                <asp:Panel CssClass="PanelCollapsibleNoTopborder" ID="pnlAddressHeader" runat="server"
                    Width="800px" Height="30px">
                    <div class="divPanelCollapsible">
                        Addresses
                        <asp:LinkButton ID="lbtnAddress" runat="server">
                            <asp:Label ID="lblAddress" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton></div>
                </asp:Panel>
                <asp:Panel ID="pnlAddress" runat="server">
                    <asp:UpdatePanel ID="updPnlUcAddress" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table style="width: 801px;" cellpadding="0" cellspacing="0" class="TabArea">
                                <tr>
                                    <td>
                                        <ucAddress:Address ID="ucAddress" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <asp:Panel CssClass="PanelCollapsibleBottom" ID="pnlCounterAccountHeader" runat="server"
                    Width="800px" Height="30px">
                    <div class="divPanelCollapsible" style="padding: 5px">
                        Counter Accounts
                        <asp:LinkButton ID="lbtnCounterAccount" runat="server">
                            <asp:Label ID="lblCounterAccount" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlCounterAccount" runat="server">
                    <table style="width: 801px; border-bottom: solid 1px black; border-left: solid 1px black;
                        border-right: solid 1px black;" cellpadding="0" cellspacing="0">
                        <tr style="font-size: 8px; height: 8px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 81px;">
                                &nbsp;
                            </td>
                            <td style="width: 705px; vertical-align: top;">
                                <asp:GridView runat="server" PageSize="5" Width="600px" DataKeyNames="Key" SkinID="custom-width"
                                    AllowSorting="True" DataSourceID="odsCounterAccounts" AutoGenerateColumns="false"
                                    ID="gvCounterAccounts" OnRowDataBound="gvContactPersonCompanies_RowDataBound"
                                    OnRowCommand="gvCounterAccounts_RowCommand">
                                    <Columns>
                                        <asp:BoundField HeaderText="Bank" DataField="BankName" SortExpression="BankName" />
                                        <asp:BoundField HeaderText="Number" DataField="Number" SortExpression="Number" />
                                        <asp:BoundField HeaderText="Account Name" DataField="AccountName" SortExpression="AccountName" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lbtnEditCounterAccount" runat="server" CausesValidation="false"
                                                    Enabled="false" ToolTip="Edit this counter account." CommandName="EditCounterAccount"
                                                    Text="Edit Counter Account" />
                                                <asp:LinkButton ID="lbtnDetachContactperson" runat="server" CausesValidation="false"
                                                    Enabled="false" ToolTip="Detaches this counter account from the contactperson."
                                                    CommandName="DetachContactperson" Text="Detach" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:ObjectDataSource ID="odsCounterAccounts" runat="server" SelectMethod="GetCounterAccounts"
                                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ContactPersonEditAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="personKey" SessionField="contactid" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                        <tr style="font-size: 5px; height: 5px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td style="width: 705px">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td style="width: 201px;">
                                            <asp:Button ID="btnAttachCounterAccountToPerson" runat="server" Enabled="false" CausesValidation="false"
                                                        PostBackUrl="~/DataMaintenance/Contacts/AttachCounterAccountToContact.aspx" Text="Attach CounterAccount"
                                                        Width="190px" />
                                        </td>
                                        <td style="width: 600px;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="font-size: 8px; height: 8px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel CssClass="PanelCollapsible" ID="pnlAccountHeader" runat="server" Width="800px"
                    Height="30px">
                    <div class="divPanelCollapsible" style="padding: 5px">
                        Accounts
                        <asp:LinkButton ID="lbtnAccount" runat="server">
                            <asp:Label ID="lblAccount" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlAccount" runat="server">
                    <table style="width: 801px; border-top: solid 1px black; border-left: solid 1px black;
                        border-right: solid 1px black;" cellpadding="0" cellspacing="0">
                        <tr style="font-size: 8px; height: 8px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 81px;">
                                &nbsp;
                            </td>
                            <td style="width: 735px; vertical-align: top;">
                                <ucAccounts:Accounts setGvWidth="600px" ID="ucAccounts" runat="server" />
                            </td>
                        </tr>
                        <tr style="font-size: 5px; height: 5px;">
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td style="width: 735px">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td style="width: 201px; height: 24px;">
                                            <asp:Button ID="btnAttachPersonToAccount" runat="server" Enabled="false" PostBackUrl="~/DataMaintenance/Contacts/AttachAccountToContact.aspx"
                                                CausesValidation="false" Text="Attach Account" />
                                        </td>
                                        <td style="width: 600px; height: 24px;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="font-size: 8px; height: 8px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel CssClass="PanelCollapsibleBottom" ID="pnlContactPersonForCompanyHeader"
                    runat="server" Width="800px" Height="30px">
                    <div class="divPanelCollapsible" style="padding: 5px">
                        Contactperson for Companies
                        <asp:LinkButton ID="lbtnContactPersonForCompany" runat="server">
                            <asp:Label ID="lblContactPersonForCompany" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlContactPersonForCompany" runat="server">
                    <table style="width: 801px; border-bottom: solid 1px black; border-left: solid 1px black;
                        border-right: solid 1px black;" cellpadding="0" cellspacing="0">
                        <tr style="font-size: 8px; height: 8px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 81px;">
                                &nbsp;
                            </td>
                            <td style="width: 705px; vertical-align: top;">
                                <asp:GridView runat="server" PageSize="5" Width="600px" DataKeyNames="Company_Key"
                                    SkinID="custom-width" AllowSorting="True" DataSourceID="odsCompanies" AutoGenerateColumns="false"
                                    ID="gvContactPersonCompanies" OnRowDataBound="gvContactPersonCompanies_RowDataBound"
                                    OnRowCommand="gvContactPersonCompanies_RowCommand">
                                    <Columns>
                                        <asp:BoundField HeaderText="Company Name" DataField="Company_CurrentNAW_Name" SortExpression="Company_CurrentNAW_Name" />
                                        <asp:BoundField HeaderText="KvKNumber" DataField="Company_KvKNumber" SortExpression="Company_KvKNumber" />
                                        <asp:BoundField HeaderText="Address" DataField="Company_CurrentNAW_ResidentialAddress_DisplayAddress" SortExpression="Company_CurrentNAW_ResidentialAddress_DisplayAddress" />
                                        <asp:BoundField HeaderText="Email" DataField="Company_ContactDetails_Email" SortExpression="Company_ContactDetails_Email" />
                                        <%--<asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton
                                                                 ID="lbtnEditContactperson"
                                                                 runat="server"
                                                                 CausesValidation="false"
                                                                 CommandName="EditCompany"
                                                                 ToolTip="View Company data"
                                                                 Text="Edit Company" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lbtnDetachContactperson" runat="server" CausesValidation="false"
                                                    Enabled="false" ToolTip="Detaches this contact as a contactperson for the company."
                                                    CommandName="DetachContactperson" Text="Detach" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:ObjectDataSource ID="odsCompanies" runat="server" SelectMethod="GetCompanyContactPersons"
                                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ContactPersonEditAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="personKey" SessionField="contactid" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                        <tr style="font-size: 5px; height: 5px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td style="width: 705px">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td style="width: 201px;">
                                            <asp:Button ID="btnAttachCompanyToPerson" runat="server" Enabled="false" CausesValidation="false"
                                                PostBackUrl="~/DataMaintenance/Contacts/AttachCompanyToContactperson.aspx" Text="Attach Company" />
                                        </td>
                                        <td style="width: 600px;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="font-size: 8px; height: 8px;">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr style="font-size: 3px; height: 3px;">
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button ID="btnSaveBypass" runat="server" CausesValidation="false" Enabled="false"
                    OnClick="bntSave_Click" ToolTip="Bypasses required field validation" Text="Save incomplete NAR" />&nbsp;
                <asp:Button ID="bntSave" runat="server" OnClick="bntSave_Click" Enabled="false" Text="Save" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        if (document.getElementById('hfMessSaveSucceed')) {
            alert(document.getElementById('hfMessSaveSucceed').value);
        }
    </script>

    <ajaxToolkit:CollapsiblePanelExtender ID="cpeDetails" TargetControlID="pnlDetails" ExpandControlID="lbtnDetails"
        CollapseControlID="lbtnDetails" Collapsed="false" TextLabelID="lblDetails" CollapsedText="show"
        ExpandedText="hide" SuppressPostBack="true" runat="server" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeAddress" TargetControlID="pnlAddress" ExpandControlID="lbtnAddress"
        CollapseControlID="lbtnAddress" Collapsed="false" TextLabelID="lblAddress" CollapsedText="show"
        ExpandedText="hide" SuppressPostBack="true" runat="server" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeCounterAccount" TargetControlID="pnlCounterAccount" ExpandControlID="lbtnCounterAccount"
        CollapseControlID="lbtnCounterAccount" Collapsed="false" TextLabelID="lblCounterAccount" CollapsedText="show"
        ExpandedText="hide" SuppressPostBack="true" runat="server" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeAccount" TargetControlID="pnlAccount" ExpandControlID="lbtnAccount"
        CollapseControlID="lbtnAccount" Collapsed="false" TextLabelID="lblAccount" CollapsedText="show"
        ExpandedText="hide" SuppressPostBack="true" runat="server" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeContactPersonForCompany" TargetControlID="pnlContactPersonForCompany" ExpandControlID="lbtnContactPersonForCompany"
        CollapseControlID="lbtnContactPersonForCompany" Collapsed="true" TextLabelID="lblContactPersonForCompany"
        CollapsedText="show" ExpandedText="hide" SuppressPostBack="true" runat="server" />
</asp:Content>

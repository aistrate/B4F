<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="Remisier.aspx.cs"
    Inherits="DataMaintenance_Remisier_Remisier" Title="Remisier Maintenance" Theme="Neutral" %>

<%@ Register Src="../../UC/Address.ascx" TagName="Address" TagPrefix="ucAddress" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc3" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="Server" />
    <asp:HiddenField ID="hfAssetMAnagerID" runat="server" />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td colspan="4">
                <uc3:BackButton ID="BackButton" runat="server" />
                <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton2" runat="server" />
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
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlDetailsHeader" runat="server" Width="800px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Remiser Details
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
                                                <td style="width: 112px; text-align: left;">
                                                    <asp:Label ID="lblAssetManagerLabel" runat="server" Text="Asset Manager"></asp:Label>
                                                </td>
                                                <td style="text-align: left;" >
                                                    <asp:MultiView ID="mvwAssetManager" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                                        <asp:View ID="vwAssetManager" runat="server">
                                                            <asp:Label ID="lblAssetManager" runat="server" Font-Bold="True"></asp:Label>
                                                        </asp:View>
                                                        <asp:View ID="vwStichting" runat="server">
                                                            <asp:DropDownList ID="ddlAssetManager" runat="server" SkinID="custom-width" Width="206px" DataSourceID="odsAssetManager"
                                                                DataTextField="CompanyName" DataValueField="Key" AutoPostBack="True">
                                                            </asp:DropDownList>
                                                            <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                                                                TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter"></asp:ObjectDataSource>
                                                            <asp:RequiredFieldValidator ID="rfvAssetManager" runat="server" ControlToValidate="ddlAssetManager" SetFocusOnError="true"
                                                                ErrorMessage="Asset Manager" InitialValue="-2147483648" >*</asp:RequiredFieldValidator>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                </td>
                                                <td style="width: 125px; text-align: left;">
                                                    <asp:Label ID="lblRemisierType" runat="server" Text="Type" />
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:DropDownList ID="ddlRemisierType" runat="server" DataSourceID="odsRemisierTypes"
                                                        DataTextField="Description" DataValueField="Key" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                    <asp:ObjectDataSource ID="odsRemisierTypes" runat="server" SelectMethod="GetRemisierTypes"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter"></asp:ObjectDataSource>
                                                    <asp:RequiredFieldValidator ID="rfvRemisierType" runat="server" ControlToValidate="ddlRemisierType" SetFocusOnError="true"
                                                        ErrorMessage="Remisier Type" InitialValue="-2147483648" >*</asp:RequiredFieldValidator>
                                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Active" Visible="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 112px; text-align: left;" >
                                                    <asp:Label ID="lblRemisierName" runat="server" Text="Name" Width="112px" />
                                                </td>
                                                <td style="width: 280px; text-align: left;">
                                                    <asp:TextBox ID="tbRemisierName" runat="server" MaxLength="30" SkinID="broad" />
                                                    <asp:RequiredFieldValidator ID="rfvRemisierName" runat="server" ControlToValidate="tbRemisierName" SetFocusOnError="true"
                                                        ErrorMessage="Remisier Name" >*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="width: 125px; text-align: left;">
                                                    <asp:Label ID="lblInternalRef" runat="server" Text="Internal Reference" />
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:TextBox ID="tbInternalRef" runat="server" Width="250px" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td></td>
                                                <td colspan="3">
                                                    <asp:RadioButtonList ID="rblBankChoice" runat="server" RepeatDirection="Horizontal" Width="225px" 
                                                        AutoPostBack="True" OnSelectedIndexChanged="rblBankChoice_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" >Known bank</asp:ListItem>
                                                        <asp:ListItem>Unknown bank</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTegenrekeningBankLabel" runat="server" Text="Bank"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:MultiView ID="mvwBank" runat="server" ActiveViewIndex="0" >
                                                        <asp:View ID="vwKnownBank" runat="server" >
                                                            <asp:DropDownList 
                                                                ID="ddlBank" DataSourceID="odsBank" DataTextField="Name" DataValueField="Key" 
                                                                runat="server" SkinID="custom-width" Width="206px" /> 
                                                            <asp:ObjectDataSource ID="odsBank" runat="server" SelectMethod="GetBanks"
                                                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.CounterAccountEditAdapter">
                                                            </asp:ObjectDataSource>
                                                        </asp:View>
                                                        <asp:View ID="vwUnknownBank" runat="server">
                                                            <asp:TextBox ID="tbBankName" MaxLength="100" runat="server" SkinID="broad" ></asp:TextBox>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblTegenrekeningNumberLabel" runat="server" Text="Account Number"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbTegenrekNR" MaxLength="15" runat="server" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTegenrekeningTnvLabel" runat="server" Text="Account Name" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbTegenrekTNV" MaxLength="100" runat="server" SkinID="broad" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblBankCity" runat="server" Text="Bank City"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbBankCity" MaxLength="15" runat="server" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTelephone" runat="server" Text="Telephone Number" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbTelephone" MaxLength="100" runat="server" SkinID="broad" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblFax" runat="server" Text="Fax Number" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbFax" MaxLength="15" runat="server" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="width: 112px; text-align: left;" >
                                                    <asp:Label ID="lblEmail" runat="server" Text="Email" Width="112px" />
                                                </td>
                                                <td colspan="3" style="width: 280px; text-align: left;">
                                                    <asp:TextBox ID="tbEmail" runat="server" MaxLength="50" SkinID="broad" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblParentRemisier" runat="server" Text="Parent Remisier" />
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlParentRemisier" runat="server" Width="206px" SkinID="custom-width" DataSourceID="odsParentRemisier"
                                                        OnSelectedIndexChanged="ddlParentRemisier_SelectedIndexChanged" AutoPostBack="True"
                                                        DataTextField="DisplayNameAndRefNumber" DataValueField="Key" >
                                                    </asp:DropDownList>
                                                    <asp:ObjectDataSource ID="odsParentRemisier" runat="server" SelectMethod="GetAssetManagerRemisiers"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter" >
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                                                PropertyName="SelectedValue" Type="Int32" />
                                                            <asp:Parameter Name="remisierFilterType" DefaultValue="1" Type="Int32" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblParentRemisierKickBackPercentage" runat="server" Text="Parent KickBack %" />
                                                </td>
                                                <td>
                                                    <db:DecimalBox ID="dbParentRemisierKickBackPercentage" AllowNegativeSign="false" DecimalPlaces="4" runat="server" Enabled="false" 
                                                        ToolTip="Enter a number between 0 and 100" />
                                                    <asp:RangeValidator ID="rvMaxKickBack" runat="server"
                                                        Type="Double"
                                                        ControlToValidate="dbParentRemisierKickBackPercentage:tbDecimal"
                                                        Text="*"
                                                        MinimumValue="0"
                                                        MaximumValue="100"
                                                        ErrorMessage="Enter a number less than 100 %">
                                                    </asp:RangeValidator>
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
            <td colspan="4">
                <asp:Panel CssClass="PanelCollapsibleNoTopborder" ID="pnlAddressHeader" runat="server"
                    Width="800px" Height="30px">
                    <div class="divPanelCollapsible">
                        Addresses
                        <asp:LinkButton ID="lbtnAddress" runat="server" Text="">
                            <asp:Label ID="lblAddress" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlAddress" runat="server">
                    <asp:UpdatePanel ID="updPnlUcAddress" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table style="width: 801px;" cellpadding="0" cellspacing="0" class="TabArea">
                                <tr>
                                    <td >
                                        <ucAddress:Address ID="ucAddress" runat="server" CaptionResidentialAddress="Office Address" DataCheckingEnabled="false" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>

        <tr>
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlContactPersonHeader" runat="server" Width="800px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Contact Person Details
                        <asp:LinkButton ID="lbtnContactPerson" runat="server">
                            <asp:Label ID="lblContactPerson" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlContactPerson" runat="server">
                    <asp:UpdatePanel ID="updPnlContactPerson" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 780px;">
                                            <tr>
                                                <td style="width: 112px; text-align: left;" >
                                                    <asp:Label ID="lblCPLastName" runat="server" Text="Last Name" Width="112px" />
                                                </td>
                                                <td style="width: 280px; text-align: left;">
                                                    <asp:TextBox ID="tbCPLastName" runat="server" MaxLength="30" SkinID="broad" />
                                                </td>
                                                <td style="width: 125px; text-align: left;">
                                                    <asp:Label ID="lblCPInitials" runat="server" Text="Initials" />
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:TextBox ID="tbCPInitials" runat="server" MaxLength="20" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblCPMiddleName" runat="server" Text="Middle Name"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbCPMiddleName" MaxLength="10" runat="server" ></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblCPGender" runat="server" Text="Gender"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="rbCPGender" RepeatDirection="Horizontal" runat="server">
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblCPTelephone" runat="server" Text="Telephone Number" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbCPTelephone" MaxLength="100" runat="server" SkinID="broad" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblCPEmail" runat="server" Text="E-mail" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbCPEmail" MaxLength="100" runat="server" SkinID="broad" />
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
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlAdditionalDetailsHeader" runat="server" Width="800px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Additional Details
                        <asp:LinkButton ID="lbtnAdditionalDetails" runat="server">
                            <asp:Label ID="lblAdditionalDetails" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlAdditionalDetails" runat="server">
                    <asp:UpdatePanel ID="updPnlAdditionalDetails" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 780px;">
                                            <tr>
                                                <td style="width: 112px; text-align: left;" >
                                                    <asp:Label ID="lblDatumOvereenkomst" runat="server" Text="Datum Overeenkomst" Width="112px" />
                                                </td>
                                                <td style="width: 280px; text-align: left;">
                                                    <asp:TextBox ID="tbDatumOvereenkomst" runat="server" MaxLength="10" SkinID="broad" />
                                                </td>
                                                <td style="width: 125px; text-align: left;">
                                                    <asp:Label ID="lblNummerOvereenkomst" runat="server" Text="Nummer Overeenkomst" />
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:TextBox ID="tbNummerOvereenkomst" runat="server" MaxLength="12" SkinID="broad" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblNummerAFM" runat="server" Text="AFM Nummer" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbNummerAFM" MaxLength="12" runat="server" SkinID="broad" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblNummerKasbank" runat="server" Text="Kasbank Nummer" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbNummerKasbank" MaxLength="12" runat="server" SkinID="broad" />
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
    </table>
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" TargetControlID="pnlDetails"
        Collapsed="false" ExpandControlID="lbtnDetails" CollapseControlID="lbtnDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblDetails"
        CollapsedText="Show Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />

    <ajaxToolkit:CollapsiblePanelExtender ID="cpeAddress" runat="Server" TargetControlID="pnlAddress"
        Collapsed="false" ExpandControlID="lbtnAddress" CollapseControlID="lbtnAddress"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblAddress"
        CollapsedText="Show Address..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />

    <ajaxToolkit:CollapsiblePanelExtender ID="cpeContactPerson" runat="Server" TargetControlID="pnlContactPerson"
        Collapsed="false" ExpandControlID="lbtnContactPerson" CollapseControlID="lbtnContactPerson"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblContactPerson"
        CollapsedText="Show Contact Person..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />

    <ajaxToolkit:CollapsiblePanelExtender ID="cpeAdditionalDetails" runat="Server" TargetControlID="pnlAdditionalDetails"
        Collapsed="false" ExpandControlID="lbtnAdditionalDetails" CollapseControlID="lbtnAdditionalDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblAdditionalDetails"
        CollapsedText="Show Additional Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
    <br />
    <asp:Button ID="btnSave" runat="server" OnClick="btnSaveRemisier_Click" Text="Save Changes" />
</asp:Content>

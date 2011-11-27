<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="Turbo.aspx.cs" Inherits="DataMaintenance_Turbo" Title="Turbo" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc1" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
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
        <tr style="font-size: 3px; height: 3px;">
            <td colspan="4" style="height: 2px">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlDetailsHeader" runat="server" Width="900px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Turbo Details
                        <asp:LinkButton ID="lbtnDetails" runat="server">
                            <asp:Label ID="lblDetails" SkinID="Header" runat="server" />
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlDetails" runat="server">
                    <asp:UpdatePanel ID="upDetails" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 880px;">
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblDerivativeName" runat="server" Text="Derivative Name:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:TextBox ID="tbDerivativeName" runat="server" MaxLength="100" SkinID="broad" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvDerivativeName" runat="server" ErrorMessage="Derivative Name is mandatory"
                                                        ControlToValidate="tbDerivativeName">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblISIN" runat="server" Text="ISIN:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <asp:TextBox ID="tbISIN" runat="server" MaxLength="30" SkinID="broad" />
                                                </td>
                                                <td style="height: 19px; width: 95px;">
                                                    <asp:RequiredFieldValidator ID="rfvISIN" runat="server" ErrorMessage="ISIN is mandatory"
                                                        ControlToValidate="tbISIN">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblSign" runat="server" Text="Sign:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 201px;">
                                                    <asp:RadioButtonList ID="rblSign" RepeatDirection="Horizontal" runat="server" DataSourceID="odsSignOptions" 
                                                        DataTextField="Description" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsSignOptions" runat="server" SelectMethod="GetSignOptions" 
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvSign" runat="server" ErrorMessage="Sign is mandatory" 
                                                        ControlToValidate="rblSign" >*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblStopLoss" runat="server" Text="Stop Loss:" />
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <db:DecimalBox ID="dbStopLoss" runat="server" Width="40px" DecimalPlaces="6" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvStopLoss" ErrorMessage="Stop Loss is mandatory"
                                                        ControlToValidate="dbStopLoss:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblLeverage" runat="server" Text="Leverage:" />
                                                </td>
                                                <td>
                                                    <db:DecimalBox ID="dbLeverage" runat="server" Width="40px" DecimalPlaces="4" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvLeverage" ErrorMessage="Leverage is mandatory"
                                                        ControlToValidate="dbLeverage:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblFinanceLevel" runat="server" Text="Finance Level:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbFinanceLevel" runat="server" Width="40px" DecimalPlaces="4" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblRatio" runat="server" Text="Ratio (1:n):" />
                                                </td>
                                                <td colspan="5" style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <db:DecimalBox ID="dbRatio" runat="server" Width="40px" DecimalPlaces="0" />
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
                <asp:Button ID="bntSave" runat="server" OnClick="bntSave_Click" Enabled="false" Text="Save" Width="80px" />
            </td>
        </tr>
    </table>

    <ajaxToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" TargetControlID="pnlDetails"
        Collapsed="false" ExpandControlID="lbtnDetails" CollapseControlID="lbtnDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblDetails"
        CollapsedText="Show Instrument Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
</asp:Content>

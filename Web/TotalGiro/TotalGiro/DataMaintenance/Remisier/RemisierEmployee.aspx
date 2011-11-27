<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="RemisierEmployee.aspx.cs"
    Inherits="DataMaintenance_Remisier_RemisierEmployee" Title="RemisierEmployee Maintenance" Theme="Neutral" %>

<%@ Register Src="../../UC/Address.ascx" TagName="Address" TagPrefix="ucAddress" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc3" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="Server" />
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
            <td>
                <asp:Label ID="lblRemisierName" runat="server" Font-Bold="true" Text="" />
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlEmployeeHeader" runat="server" Width="800px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Contact Person Details
                        <asp:LinkButton ID="lbtnEmployee" runat="server">
                            <asp:Label ID="lblEmployee" SkinID="Header" runat="server"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlEmployee" runat="server">
                    <asp:UpdatePanel ID="updPnlEmployee" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 780px;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbTitle" MaxLength="10" runat="server" ></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblGender" runat="server" Text="Gender"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="rbGender" RepeatDirection="Horizontal" runat="server">
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="width: 112px; text-align: left;" >
                                                    <asp:Label ID="lblLastName" runat="server" Text="Last Name" Width="112px" />
                                                </td>
                                                <td style="width: 280px; text-align: left;">
                                                    <asp:TextBox ID="tbLastName" runat="server" MaxLength="30" SkinID="broad" />
                                                </td>
                                                <td style="width: 125px; text-align: left;">
                                                    <asp:Label ID="lblInitials" runat="server" Text="Initials" />
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:TextBox ID="tbInitials" runat="server" MaxLength="20" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbMiddleName" MaxLength="10" runat="server" ></asp:TextBox>
                                                </td>
                                                <td colspan="2" />
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTelephone" runat="server" Text="Telephone Number" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbTelephone" MaxLength="15" runat="server"  />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblTelephoneAH" runat="server" Text="Home Telephone Number" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbTelephoneAH" MaxLength="15" runat="server"  />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMobile" runat="server" Text="Mobile Number" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbMobile" MaxLength="15" runat="server" />
                                                </td>
                                                <td colspan="2" />
                                            </tr>
                                            
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblEmail" runat="server" Text="E-mail" />
                                                </td>
                                                <td colspan="3" >
                                                    <asp:TextBox ID="tbEmail" MaxLength="300" runat="server" SkinID="Ultra-broad" />
                                                </td>
                                            </tr>
                                            
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblRole" runat="server" Text="Role" />
                                                </td>
                                                <td colspan="3" >
                                                        <asp:DropDownList 
                                                            ID="ddlRole" DataSourceID="odsRoles" DataTextField="Description" DataValueField="Key" 
                                                            runat="server" /> 
                                                        <asp:ObjectDataSource ID="odsRoles" runat="server" SelectMethod="GetEmployeeRoles"
                                                            TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter">
                                                        </asp:ObjectDataSource>
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
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeEmployee" runat="Server" TargetControlID="pnlEmployee"
        Collapsed="false" ExpandControlID="lbtnEmployee" CollapseControlID="lbtnEmployee"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblEmployee"
        CollapsedText="Show Contact Person..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
    <br />
    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save Changes" />
</asp:Content>

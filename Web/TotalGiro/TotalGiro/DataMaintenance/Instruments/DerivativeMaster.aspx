<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="DerivativeMaster.aspx.cs" Inherits="DataMaintenance_DerivativeMaster" Title="Derivative Master" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc1" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="dp" %>
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
                        Derivative Master Details
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
                                                    <asp:Label ID="lblDerivativeMasterName" runat="server" Text="Derivative Master Name:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:TextBox ID="tbDerivativeMasterName" runat="server" MaxLength="100" SkinID="broad" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvInstrumentName" runat="server" ErrorMessage="Instrument Name is mandatory"
                                                        ControlToValidate="tbDerivativeMasterName">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="height: 16px; text-align: right; vertical-align: middle; height: 25px;
                                                    width: 374px;">
                                                    <asp:Label ID="lblExchange" runat="server" Text="Exchange:" />
                                                </td>
                                                <td style="height: 16px; vertical-align: middle; text-align: left; width: 535px;">
                                                    <asp:DropDownList ID="ddlExchange" runat="server" DataSourceID="odsExchange" 
                                                        DataTextField="ExchangeName" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td >
                                                    <asp:RequiredFieldValidator ID="rfvExchange" runat="server" ErrorMessage="Exchange is mandatory"
                                                        ControlToValidate="ddlExchange" InitialValue="-2147483648" >*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 21px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblUnderlyingSecCategory" runat="server" Text="Underlying Category:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 21px; text-align: left; width: 201px;">
                                                    <asp:DropDownList ID="ddlUnderlyingSecCategory" runat="server" DataSourceID="odsSecCategories" 
                                                        AutoPostBack="true" OnSelectedIndexChanged="ddlUnderlyingSecCategory_SelectedIndexChanged"
                                                        DataTextField="Description" DataValueField="Key">
                                                    </asp:DropDownList>
                                                    <asp:ObjectDataSource ID="odsSecCategories" runat="server" SelectMethod="GetSecCategories"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
                                                        <SelectParameters>
                                                            <asp:Parameter Name="secCategoryFilter" Type="Int32" DefaultValue="13" />
                                                            <asp:Parameter Name="includeNotSupported" Type="Boolean" DefaultValue="true" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="width: 2px">
                                                </td>
                                                <td style="height: 16px; text-align: right; vertical-align: middle; height: 25px;
                                                    width: 374px;">
                                                    <asp:Label ID="lblUnderlyingInstrument" runat="server" Text="Underlying Instrument:" />
                                                </td>
                                                <td style="height: 16px; vertical-align: middle; text-align: left; width: 535px;">
                                                    <asp:DropDownList ID="ddlUnderlyingInstrument" runat="server" SkinID="broad" 
                                                        DataSourceID="odsSecurities" DataTextField="DisplayName" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsSecurities" runat="server" SelectMethod="GetSecurities"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                        <SelectParameters>
                                                            <asp:ControlParameter Name="secCategoryId" Type="Int32" ControlID="ddlUnderlyingSecCategory" PropertyName="SelectedValue" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td >
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblDecimalPlaces" runat="server" Text="Decimal Places:" />
                                                </td>
                                                <td>
                                                    <db:DecimalBox ID="dbDecimalPlaces" runat="server" MaximumValue="6" Width="30px" DecimalPlaces="0" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvDecimalPlaces" ErrorMessage="Decimal Places is mandatory"
                                                        ControlToValidate="dbDecimalPlaces:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblSymbol" runat="server" Text="Symbol:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <asp:TextBox ID="txtSymbol" runat="server" MaxLength="100" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblContractSize" runat="server" Text="Contract Size:" />
                                                </td>
                                                <td>
                                                    <db:DecimalBox ID="dbContractSize" runat="server" MaximumValue="6" Width="30px" DecimalPlaces="0" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvContractSize" ErrorMessage="Contract Size is mandatory"
                                                        ControlToValidate="dbContractSize:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="Label1" runat="server" Text="Nominal Currency:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <asp:DropDownList ID="ddCurrencyNominal" runat="server" SkinID="custom-width" Width="50px" 
                                                        DataSourceID="odsCurrencies" DataTextField="Symbol" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsCurrencies" runat="server" SelectMethod="GetCurrencies" 
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.MoneyTransferOrderAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td>
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

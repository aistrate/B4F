<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="Security.aspx.cs" Inherits="DataMaintenance_Security" Title="Security" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc1" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="dp" %>
<%@ Register Src="../../UC/Calendar.ascx" TagName="Calendar" TagPrefix="cl" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <input id="hfViewState" type="hidden" name="hfViewState" />
    <asp:HiddenField ID="hdnIsCouponRateLineInsert" runat="server" Value="false" />
    <asp:HiddenField ID="hdnInstrumentId" runat="server" Value="0" />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <uc2:BackButton ID="BackButton" runat="server" />
                <uc1:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton2" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px" Font-Bold="true"></b4f:ErrorLabel>
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </td>
        </tr>
        <tr style="font-size: 3px; height: 3px;">
            <td style="height: 2px">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top;">
                <asp:Panel ID="pnlDetailsHeader" runat="server" Width="900px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Instrument Details
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
                                                    <asp:Label ID="lblInstrumentName" runat="server" Text="Instrument Name:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:TextBox ID="tbInstrumentName" runat="server" MaxLength="100" SkinID="broad" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvInstrumentName" runat="server" ErrorMessage="Instrument Name is mandatory"
                                                        ControlToValidate="tbInstrumentName">*</asp:RequiredFieldValidator>
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
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblCompanyName" runat="server" Text="Company:" />
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <asp:TextBox ID="tbCompanyName" runat="server" SkinID="broad"></asp:TextBox>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                </td>
                                                <td style="text-align: right; vertical-align: middle; height: 25px; width: 374px;">
                                                    <asp:Label ID="lblIssueDate" runat="server" Text="Issue Date:" />
                                                </td>
                                                <td>
                                                    <dp:DatePicker ID="ucIssueDate" ListYearsBeforeCurrent="50" ListYearsAfterCurrent="0"
                                                        runat="server" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 21px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblHomeExchange" runat="server" Text="Home Exchange:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 21px; text-align: left; width: 201px;">
                                                    <asp:DropDownList ID="ddlHomeExchange" runat="server" DataSourceID="odsExchange" 
                                                        DataTextField="ExchangeName" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="width: 2px">
                                                </td>
                                                <td style="height: 16px; text-align: right; vertical-align: middle; height: 25px;
                                                    width: 374px;">
                                                    <asp:Label ID="lblDefaultExchange" runat="server" Text="Default Exchange:" />
                                                </td>
                                                <td style="height: 16px; vertical-align: middle; text-align: left; width: 535px;">
                                                    <asp:DropDownList ID="ddlDefaultExchange" runat="server" DataSourceID="odsExchange" 
                                                        DataTextField="ExchangeName" DataValueField="Key" />
                                                </td>
                                                <td >
                                                    <asp:RequiredFieldValidator ID="rfvDefaultExchange" runat="server" ErrorMessage="Default Exchange is mandatory"
                                                        ControlToValidate="ddlDefaultExchange" InitialValue="-2147483648" >*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 21px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblCountry" runat="server" Text="Country:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 21px; text-align: left; width: 201px;">
                                                    <asp:DropDownList ID="ddlCountry" runat="server" DataSourceID="odsCountry" 
                                                        DataTextField="CountryName" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsCountry" runat="server" SelectMethod="GetCountries"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.PersonEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="width: 2px">
                                                </td>
                                                <td style="height: 16px; text-align: right; vertical-align: middle; height: 25px;
                                                    width: 374px;">
                                                    <asp:Label ID="lblDefaultRoute" runat="server" Text="Default Route:" />
                                                </td>
                                                <td style="height: 16px; vertical-align: middle; text-align: left; width: 535px;">
                                                    <asp:DropDownList ID="ddlDefaultRoute" runat="server" SkinID="custom-width" Width="50px"
                                                        DataSourceID="odsRoutes" DataTextField="Name" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsRoutes" runat="server" SelectMethod="GetRoutes"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
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
                                                    <asp:Label ID="lblCurrencyNominal" runat="server" Text="Nominal Currency:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <asp:DropDownList ID="ddCurrencyNominal" runat="server" SkinID="custom-width" Width="50px" 
                                                        DataSourceID="odsCurrencies" DataTextField="Symbol" DataValueField="Key" />
                                                    <asp:CheckBox ID="chkActiveCurrenciesOnly" runat="server" Checked="true" Text="Active Only" 
                                                        OnCheckedChanged="chkActiveCurrenciesOnly_CheckedChanged" AutoPostBack="true" />
                                                    <asp:ObjectDataSource ID="odsCurrencies" runat="server" SelectMethod="GetCurrencies" 
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="chkActiveCurrenciesOnly" Name="onlyActive" PropertyName="Checked" Type="Boolean" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblPricingType" runat="server" Text="PricingType:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 201px;">
                                                    <asp:RadioButtonList ID="rbPricingType" RepeatDirection="Horizontal" runat="server" DataSourceID="odsPricingTypes" 
                                                        DataTextField="Description" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsPricingTypes" runat="server" SelectMethod="GetPricingTypes" 
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="reqPricingType" runat="server" ErrorMessage="PricingType is mandatory" ControlToValidate="rbPricingType">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td>
                                                </td>
                                                <td style="width: 535px; vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkAllowNetting" runat="server" Text="Allow Netting" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkGreenFund" runat="server" Text="Green Fund" />
                                                </td>
                                                <td colspan="2">
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkCultureFund" runat="server" Text="Culture Fund" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>

                                            <asp:Panel ID="pnlActivity" runat="server" Visible="false">
                                            <tr>
                                                <td>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Active" AutoPostBack="true" 
                                                        OnCheckedChanged="chkIsActive_CheckedChanged" />
                                                </td>
                                                <td> 
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblInActiveDateLabel" runat="server" Text="InActive Date:" />
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <cl:Calendar ID="cldInActiveDate" runat="server" Format="dd-MM-yyyy" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlParentInstrument" runat="server" Visible="false">
                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblParentInstrumentLabel" runat="server" Text="Parent:" />
                                                </td>
                                                <td colspan="5" style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:Label ID="lblParentInstrument" runat="server" />
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

        <asp:Panel ID="pnlVirtualFund" runat="server" Visible="false">
        <tr>
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlVirtualFundDetailsHeader" runat="server" Width="900px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Virtual Fund Details
                        <asp:LinkButton ID="lbtnVirtualFundDetails" runat="server">
                            <asp:Label ID="lblVirtualFundDetails" SkinID="Header" runat="server" />
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlVirtualFundDetails" runat="server">
                    <asp:UpdatePanel ID="upVirtualFundDetails" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 880px;">
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblTradingAccountNumber" runat="server" Text="Trading Account#:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:TextBox ID="txtTradingAccountNumber" runat="server" SkinID="broad" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblHoldingAccountNumber" runat="server" Text="Holding Account#:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <asp:TextBox ID="txtHoldingAccountNumber" runat="server" SkinID="broad" />
                                                </td>
                                                <td style="height: 19px; width: 95px;">
                                                </td>
                                            </tr>
                                            
                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblInitailNAVperUnit" runat="server" Text="Initail NAV per unit:" />
                                                </td>
                                                <td>
                                                    <db:DecimalBox ID="dbInitailNAVperUnit" runat="server" Width="60px" DecimalPlaces="2" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvInitailNAVperUnit" ErrorMessage="Initail NAV per Unit is mandatory"
                                                        ControlToValidate="dbInitailNAVperUnit:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblExactJournalNumber" runat="server" Text="Exact Journal#:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbExactJournalNumber" runat="server" width="50px" DecimalPlaces="0" />
                                                </td>
                                                <td style="height: 19px; width: 95px;">
                                                    <asp:RequiredFieldValidator ID="rfvExactJournalNumber" ErrorMessage="Exact Journal Number is mandatory"
                                                        ControlToValidate="dbExactJournalNumber:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: right; vertical-align: middle; height: 28px; width: 374px;">
                                                    <asp:Label ID="lblJournalNumber" runat="server" Text="Journal Number:" />
                                                </td>
                                                <td>
                                                    <db:DecimalBox ID="dbJournalNumber" runat="server" Width="60px" DecimalPlaces="0" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvJournalNumber" ErrorMessage="Journal Number is mandatory"
                                                        ControlToValidate="dbJournalNumber:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="revJournalNumber" 
                                                        runat="server"
                                                        ControlToValidate="dbJournalNumber:tbDecimal" 
                                                        ValidationExpression="^([\S\s]{0,3})$" 
                                                        ErrorMessage="Journal Number can exist of max 3 chars">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblJournalDescription" runat="server" Text="Journal Description:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <asp:TextBox ID="txtJournalDescription" runat="server" SkinID="broad"></asp:TextBox>
                                                </td>
                                                <td style="height: 19px; width: 95px;">
                                                    <asp:RequiredFieldValidator ID="rfvJournalDescription" ErrorMessage="Journal Description is mandatory"
                                                        ControlToValidate="txtJournalDescription" runat="server">*</asp:RequiredFieldValidator>
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
        </asp:Panel>

        <asp:Panel ID="pnlBond" runat="server" Visible="false" >
        <tr>
            <td colspan="4" style="vertical-align: top;">
                <asp:Panel ID="pnlBondDetailsHeader" runat="server" Width="900px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Bond Details
                        <asp:LinkButton ID="lbtnBondDetails" runat="server">
                            <asp:Label ID="lblBondDetails" SkinID="Header" runat="server" />
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlBondDetails" runat="server">
                    <asp:UpdatePanel ID="upBondDetails" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 880px;">
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblAccruedInterestCalcType" runat="server" Text="Calc Type:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:DropDownList ID="ddlAccruedInterestCalcType" runat="server" 
                                                        DataSourceID="odsAccruedInterestCalcType" DataTextField="Description" DataValueField="Key" 
                                                        OnSelectedIndexChanged="ddlAccruedInterestCalcType_SelectedIndexChanged" AutoPostBack="true" />
                                                    <asp:ObjectDataSource ID="odsAccruedInterestCalcType" runat="server" SelectMethod="GetAccruedInterestCalcTypes"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvAccruedInterestCalcType" runat="server" ErrorMessage="Calc type is mandatory"
                                                        ControlToValidate="ddlAccruedInterestCalcType" InitialValue="-2147483648" >*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblNominalValue" runat="server" Text="Nominal Value:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbNominalValue" runat="server" Width="50px" DecimalPlaces="2" />
                                                </td>
                                                <td style="height: 19px; width: 95px;">
                                                    <asp:RequiredFieldValidator ID="rfvNominalValue" ErrorMessage="Nominal Value is mandatory"
                                                        ControlToValidate="dbNominalValue:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblCouponFreq" runat="server" Text="Coupon Frequency:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <asp:DropDownList ID="ddlCouponFreq" runat="server" DataSourceID="odsRegularities" 
                                                        DataTextField="Description" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsRegularities" runat="server" SelectMethod="GetBondCouponFrequencies"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvCouponFreq" runat="server" ErrorMessage="Coupon freq is mandatory"
                                                        ControlToValidate="ddlCouponFreq" InitialValue="-2147483648" >*</asp:RequiredFieldValidator>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblCouponRate" runat="server" Text="Coupon Rate:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbCouponRate" runat="server" Width="50px" DecimalPlaces="4" />
                                                    <asp:CheckBox ID="chkFixedCouponRate" runat="server" Text="Fixed Rate" Checked="true"
                                                      OnCheckedChanged="chkFixedCouponRate_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td style="height: 19px; width: 95px;">
                                                    <asp:RequiredFieldValidator ID="rfvCouponRate" ErrorMessage="Coupon Rate is mandatory"
                                                        ControlToValidate="dbCouponRate:tbDecimal" runat="server">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblRedemptionAmount" runat="server" Text="Redemption Amount:" />
                                                </td>
                                                <td colspan="3" style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <db:DecimalBox ID="dbRedemptionAmount" runat="server" Width="50px" DecimalPlaces="2" />
                                                    <asp:Label ID="lblRedemptionAmountLabel" runat="server" Text="(when different from nominal value)" />
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="chkIsPerpetual" runat="server" Text="Is Perpetual" 
                                                     OnCheckedChanged="chkIsPerpetual_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblFirstCouponPaymntDate" runat="server" Text="First Coupon Paymnt Date:" />
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <dp:DatePicker ID="dpFirstCouponPaymntDate" ListYearsBeforeCurrent="25" ListYearsAfterCurrent="2" IsButtonDeleteVisible="false"
                                                        runat="server" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                    <asp:RequiredFieldValidator ID="rfvFirstCouponPaymntDate"
                                                        ControlToValidate="dpFirstCouponPaymntDate:txtDate"
                                                        runat="server"
                                                        Text="*"
                                                        ErrorMessage="First Coupon Paymnt Date is mandatory" />
                                                </td>
                                                <td style="text-align: right; vertical-align: middle; height: 25px; width: 374px;">
                                                    <asp:Label ID="lblMaturityDate" runat="server" Text="Maturity Date:" />
                                                </td>
                                                <td>
                                                    <dp:DatePicker ID="dpMaturityDate" ListYearsBeforeCurrent="2" ListYearsAfterCurrent="20"
                                                        runat="server" />
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvMaturityDate"
                                                        ControlToValidate="dpMaturityDate:txtDate"
                                                        runat="server"
                                                        Text="*"
                                                        ErrorMessage="Maturity Date is mandatory" />
                                                </td>
                                            </tr>
                                            
                                            <tr>
                                                <td>                                                    
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <asp:CheckBox ID="chkUltimoDating" runat="server" Text="Ultimo Dating" />
                                                </td>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            
                                        </table>
                                    </td>
                                </tr>
                                <asp:Panel ID="pnlCouponRates" runat="server" Visible="false" >
                                <tr>
                                    <td >
                                        <asp:GridView ID="gvCouponRates" Caption="Coupon Rates" SkinID="custom-width"
                                            DataKeyNames="Key" Width="80%" AutoGenerateColumns="False" runat="server" DataSourceID="odsCouponRates"
                                            PageSize="25" AllowPaging="True" AllowSorting="True"
                                            OnRowCommand="gvCouponRates_RowCommand"
                                            OnRowDataBound="gvCouponRates_RowDataBound"
                                            OnRowUpdating="gvCouponRates_RowUpdating"
                                            OnRowUpdated="gvCouponRates_RowUpdated" >
                                            <Columns>
                                                <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("StartDate", "{0:dd MMM yyyy}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate >
                                                        <cl:Calendar ID="cldStartDate" runat="server" SelectedDate='<%# Bind("StartDate") %>' CssClass="gridview-calendar" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                                                    <ItemTemplate>
                                                        <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) ? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToString("dd MMM yyyy"))%>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <cl:Calendar ID="cldEndDate" runat="server" SelectedDate='<%# Bind("EndDate") %>' CssClass="gridview-calendar" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Coupon Rate" SortExpression="CouponRate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCouponRate" runat="server" Text='<%# Bind("CouponRate", "{0:##0.00###}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <db:DecimalBox ID="dbCouponRate" runat="server" Text='<%# Bind("CouponRate") %>' DecimalPlaces="4" />
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMM yyyy}" HtmlEncode="False" ReadOnly="true" >
                                                    <ItemStyle wrap="False" />
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="CreatedBy" DataField="CreatedBy" SortExpression="CreatedBy"
                                                    ReadOnly="True" />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton 
                                                            ID="lbtEdit" 
                                                            runat="server" 
                                                            CausesValidation="False" 
                                                            Text="Edit"
                                                            CommandName="EditLine"
                                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                                            OnCommand="lbtEdit_Command" />
                                                        <asp:LinkButton 
                                                            ID="lbtDelete" 
                                                            runat="server" 
                                                            CausesValidation="False" 
                                                            Text="Delete"
                                                            CommandName="DeleteLine"
                                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                                            OnCommand="lbtDelete_Command"
                                                            OnClientClick="return confirm('Delete line?')" />
                                                        <asp:LinkButton 
                                                            ID="lbtUpdate" 
                                                            runat="server" 
                                                            CausesValidation="True" 
                                                            Text='<%# ((int)DataBinder.Eval(Container.DataItem, "Key") != 0 ? "Update" : "Insert") %>'
                                                            CommandName="Update"
                                                            Visible="False" />
                                                        <asp:LinkButton 
                                                            ID="lbtCancel" 
                                                            runat="server" 
                                                            CausesValidation="False" 
                                                            Text="Cancel"
                                                            CommandName="Cancel"
                                                            Visible="False" />
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="False" HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:ObjectDataSource ID="odsCouponRates" runat="server" SelectMethod="GetBondCouponRateLines" 
                                            UpdateMethod="UpdateBondCouponRateLine" OldValuesParameterFormatString="original_{0}"
                                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter" 
                                            onupdated="odsCouponRates_Updated">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdnInstrumentId" Name="InstrumentId" PropertyName="Value" Type="Int32" />
                                                <asp:ControlParameter ControlID="hdnIsCouponRateLineInsert" DefaultValue="False" Name="isInsert"
                                                    PropertyName="Value" Type="Boolean" />
                                            </SelectParameters>
                                            <UpdateParameters>
                                                <asp:ControlParameter ControlID="hdnInstrumentId" Name="instrumentId" PropertyName="Value" Type="Int32" DefaultValue="0" />
                                            </UpdateParameters>
                                        </asp:ObjectDataSource>
                                        <asp:Button ID="btnAddCouponRateHistory" runat="server" Text="Add Coupon Rate" OnClick="btnAddCouponRateHistory_Click" Enabled="false" />
                                    </td>
                                </tr>
                                </asp:Panel>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>
        </asp:Panel>


        <tr>
            <td style="vertical-align: top;">
                <asp:Panel ID="pnlExchangeDetailsHeader" runat="server" Width="900px" CssClass="PanelCollapsibleBottom">
                    <div class="divPanelCollapsible">
                        Exchange Details
                        <asp:LinkButton ID="lbtnExchangeDetails" runat="server">
                            <asp:Label ID="lblExchangeDetails" SkinID="Header" runat="server" />
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlExchangeDetails" runat="server">
                    <asp:UpdatePanel ID="upExchangeDetails" runat="server">
                        <ContentTemplate>
                            <table cellspacing="0" cellpadding="0" class="TabAreaTop" border="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" style="width: 880px;">
                                            <tr>
                                                <td style="vertical-align: middle; height: 21px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblDefaultCounterparty" runat="server" Text="Default Counterparty:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 21px; text-align: left; width: 201px;">
                                                    <asp:DropDownList ID="ddlDefaultCounterparty" runat="server" DataSourceID="odsCounterparties" 
                                                        SkinID="custom-width" Width="200px"
                                                        DataTextField="DisplayNumberWithName" DataValueField="Key" />
                                                    <asp:ObjectDataSource ID="odsCounterparties" runat="server" SelectMethod="GetCounterParties"
                                                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                                                    </asp:ObjectDataSource>
                                                </td>
                                                <td>
                                                </td>
                                                <td style="height: 16px; text-align: right; vertical-align: middle; height: 25px;
                                                    width: 374px;">
                                                    <asp:Label ID="lblDefaultSettlementPeriod" runat="server" Text="Settlement Period:" />
                                                </td>
                                                <td style="height: 16px; vertical-align: middle; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbDefaultSettlementPeriod" runat="server" Width="40px" DecimalPlaces="0" />
                                                </td>
                                                <td >
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblNumberOfDecimals" runat="server" Text="Number Decimals:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <db:DecimalBox ID="dbNumberOfDecimals" runat="server" MaximumValue="6" Width="30px" DecimalPlaces="0" />
                                                </td>
                                                <td>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblTickSize" runat="server" Text="Tick Size:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbTickSize" runat="server" Width="40px" DecimalPlaces="4" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkDoesSupportAmountBasedBuy" runat="server" Text="Supports Amount Based Buy" />
                                                </td>
                                                <td colspan="2">
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkDoesSupportAmountBasedSell" runat="server" Text="Supports Amount Based Sell" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 24px;">
                                                    <asp:CheckBox ID="chkDoesSupportServiceCharge" runat="server" Text="Supports Service Charge" />
                                                </td>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblServiceChargePercentageBuy" runat="server" Text="Service Charge Buy%:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 201px;">
                                                    <db:DecimalBox ID="dbServiceChargePercentageBuy" runat="server" Width="40px" DecimalPlaces="5" />
                                                </td>
                                                <td>
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: right; width: 374px;">
                                                    <asp:Label ID="lblServiceChargePercentageSell" runat="server" Text="Service Charge Sell%:" />
                                                </td>
                                                <td style="vertical-align: middle; height: 19px; text-align: left; width: 535px;">
                                                    <db:DecimalBox ID="dbServiceChargePercentageSell" runat="server" Width="40px" DecimalPlaces="5" />
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblRegisteredInNameof" runat="server" Text="Registered in name of:" />
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <asp:TextBox ID="tbRegisteredInNameof" runat="server" SkinID="broad"></asp:TextBox>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                </td>
                                                <td style="text-align: right; vertical-align: middle; height: 25px; width: 374px;">
                                                    <asp:Label ID="lblDividendPolicy" runat="server" Text="Dividend Policy:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="tbDividendPolicy" runat="server" SkinID="broad"></asp:TextBox>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: middle; height: 25px; text-align: right; width: 490px;">
                                                    <asp:Label ID="lblCommissionRecipientName" runat="server" Text="Commission Recipient Name:" />
                                                </td>
                                                <td style="vertical-align: middle; text-align: left; height: 25px; width: 201px;">
                                                    <asp:TextBox ID="tbCommissionRecipientName" runat="server" SkinID="broad"></asp:TextBox>
                                                </td>
                                                <td style="vertical-align: middle; height: 25px; text-align: left; width: 2px;">
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="chkCertificationRequired" runat="server" Text="Certification Required:" />
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
            <td>
                <asp:Button ID="bntSave" runat="server" OnClick="bntSave_Click" Enabled="false" Text="Save" Width="80px" />
                <asp:Button ID="btnViewPrices" runat="server" OnClick="btnViewPrices_Click" Visible="false" Text="View Prices" Width="110px" OnClientClick="aspnetForm.target='_blank';"/>
            </td>
        </tr>
    </table>

    <ajaxToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" TargetControlID="pnlDetails"
        Collapsed="false" ExpandControlID="lbtnDetails" CollapseControlID="lbtnDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblDetails"
        CollapsedText="Show Instrument Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeVirtualFundDetails" runat="Server" TargetControlID="pnlVirtualFundDetails"
        Collapsed="false" ExpandControlID="lbtnVirtualFundDetails" CollapseControlID="lbtnVirtualFundDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblVirtualFundDetails"
        CollapsedText="Show Virtual Fund Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeBondDetails" runat="Server" TargetControlID="pnlBondDetails"
        Collapsed="false" ExpandControlID="lbtnBondDetails" CollapseControlID="lbtnBondDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblBondDetails"
        CollapsedText="Show Bond Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeExchangeDetails" runat="Server" TargetControlID="pnlExchangeDetails"
        Collapsed="false" ExpandControlID="lbtnExchangeDetails" CollapseControlID="lbtnExchangeDetails"
        AutoCollapse="False" AutoExpand="False" ScrollContents="False" TextLabelID="lblExchangeDetails"
        CollapsedText="Show Exchange Details..." ExpandedText="Hide" SuppressPostBack="true" ExpandDirection="Vertical" />
</asp:Content>

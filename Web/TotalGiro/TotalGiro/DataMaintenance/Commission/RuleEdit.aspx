<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="RuleEdit.aspx.cs" Inherits="RuleEdit" Title="Edit Rule" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc2" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hdnIdValue" runat="server" EnableViewState="False" />
    <table cellpadding="0" cellspacing="0" border="0" style="border-color: black" width="780">
        <tr>
            <td class="tblHeader">
                Commission Rule
            </td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width: 100%">
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label1" runat="server" Text="Commission Rule Name"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:TextBox ID="txtCommissionRuleName" runat="server" SkinID="custom-width" Width="300px"></asp:TextBox>&nbsp;
                            <asp:RequiredFieldValidator ID="reqCommissionRuleName" runat="server" ControlToValidate="txtCommissionRuleName"
                                Text="*" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label2" runat="server" Text="Commission Rule Type"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlCommissionRuleType" runat="server" Width="200px" />
                            <asp:RequiredFieldValidator ID="reqCommissionRuleType" runat="server" InitialValue="-2147483648"
                                ControlToValidate="ddlCommissionRuleType" Text="*" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label6" runat="server" Text="Start Date"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <uc1:DatePicker ID="DatePickerStartDate" runat="server" ListYearsAfterCurrent="10"
                                ListYearsBeforeCurrent="10" />
                            <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="DatePickerStartDate:txtDate"
                                Text="*" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label7" runat="server" Text="End Date"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <uc1:DatePicker ID="DatePickerEndDate" runat="server" ListYearsAfterCurrent="10"
                                ListYearsBeforeCurrent="10" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label11" runat="server" Text="Model"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlModelPortfolio" runat="server" Width="200px" DataSourceID="odsModelPortfolio"
                                DataTextField="ModelName" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label10" runat="server" Text="Account"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="310px"
                                DataSourceID="odsAccount" DataTextField="DisplayNumberWithName" DataValueField="Key"
                                OnDataBound="ddlAccount_DataBound">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button ID="btnFilterAccount" runat="server" CausesValidation="false" Text="Filter  >>"
                                Height="22px" OnClick="btnFilterAccount_Click" />
                            <asp:ObjectDataSource ID="odsAccount" runat="server" SelectMethod="GetCustomerAccounts"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                                        Type="String" />
                                    <asp:Parameter Name="propertyList" DefaultValue="Key, DisplayNumberWithName" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 0px">
                        </td>
                        <td>
                            <asp:Panel ID="pnlAccountFinder" runat="server" BorderColor="Silver" BorderStyle="Solid"
                                BorderWidth="1px" Visible="False">
                                <uc2:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label9" runat="server" Text="Exchange"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlExchange" runat="server" Width="200px" DataSourceID="odsExchange"
                                DataTextField="ExchangeName" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label8" runat="server" Text="Instrument"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlInstrument" SkinID="custom-width" runat="server" Width="310px"
                                DataSourceID="odsInstrument" DataTextField="DisplayIsinWithName" DataValueField="Key">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button ID="btnFilterInstrument" runat="server" CausesValidation="false" Text="Filter  >>"
                                Height="22px" OnClick="btnFilterInstrument_Click" />
                            <asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetTradeableInstruments"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                                        Type="Object" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="exchangeId" PropertyName="ExchangeId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 0px">
                        </td>
                        <td>
                            <asp:Panel ID="pnlInstrumentFinder" runat="server" BorderColor="Silver" BorderStyle="Solid"
                                BorderWidth="1px" Visible="False">
                                <uc3:InstrumentFinder ID="ctlInstrumentFinder" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label4" runat="server" Text="Buy/Sell"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlBuySell" runat="server" Width="80px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="lblOpenClose" runat="server" Text="Open/Close"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlOpenClose" runat="server" Width="80px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label14" runat="server" Text="Original OrderType"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlOriginalOrderType" runat="server" Width="80px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label5" runat="server" Text="Security Category"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlSecCategory" runat="server" Width="370px" DataSourceID="odsSecCategories"
                                DataTextField="Description" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label3" runat="server" Text="Commission Calculation"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlCommissionCalculation" runat="server" Width="200px" DataSourceID="odsCommissionCalculation"
                                DataTextField="Name" DataValueField="Key">
                            </asp:DropDownList>
                            &nbsp;<asp:RequiredFieldValidator ID="reqCommissionCalculation" InitialValue="-2147483648"
                                runat="server" ControlToValidate="ddlCommissionCalculation">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label13" runat="server" Text="Additional Commission Calculation"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlAdditionalCalculation" runat="server" Width="200px" DataSourceID="odsCommissionCalculation"
                                DataTextField="Name" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label15" runat="server" Text="Order Action Type"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:ListBox ID="lbOrderActionType" runat="server" SelectionMode="Multiple" Width="150px"
                                Height="112px"></asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label12" runat="server" Text="Apply to all accounts"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:CheckBox ID="chkApplyToAllAccounts" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="lblHasEmployerRelation" runat="server" Text="Has Employer Relation"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:CheckBox ID="chkHasEmployerRelation" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:ObjectDataSource ID="odsCommissionCalculation" runat="server" SelectMethod="GetCommissionCalculations"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsSecCategories" runat="server" SelectMethod="GetSecCategories"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsAccountType" runat="server" SelectMethod="GetAccountTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    <br />
    <asp:ValidationSummary ID="valSummary" runat="server" DisplayMode="SingleParagraph"
        HeaderText="* Fill in all required fields." CssClass="padding" />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>&nbsp;<br />
    <uc1:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton1" runat="server" />
    &nbsp;
    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />&nbsp;
    <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
        CausesValidation="False" />
</asp:Content>

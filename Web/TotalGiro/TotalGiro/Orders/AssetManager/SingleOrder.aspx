<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="SingleOrder.aspx.cs"
    Inherits="Orders_AssetManager_SingleOrder" AutoEventWireup="true" Title="Manual Order"
    Theme="Neutral" EnableEventValidation="true" %>

<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <br />
    <table>
        <tr>
            <td style="width: 120px; height: 24px">
                <asp:Label ID="lblAccountLabel" runat="server" Text="Account:"></asp:Label>
            </td>
            <td style="width: 30px">
                <asp:UpdatePanel ID="updRfvAccount" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:RequiredFieldValidator ID="rfvAccount" runat="server" ControlToValidate="ddlAccount"
                            ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" Width="0px"
                            ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 230px; white-space: nowrap">
                <asp:UpdatePanel ID="updAccountsList" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="200px"
                            DataSourceID="odsSelectedAccount" DataTextField="DisplayNumberWithName" DataValueField="Key"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged"
                            OnDataBound="ddlAccount_DataBound" EnableViewState="true" TabIndex="1">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;
                        <asp:ObjectDataSource ID="odsSelectedAccount" runat="server" SelectMethod="GetCustomerAccounts"
                            TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.SingleOrderAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                    Type="String" />
                                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                                    Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 100px">
                <asp:UpdatePanel ID="updFilterAccountButton" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Button ID="btnFilterAccount" runat="server" CausesValidation="false" Text="Filter  >>"
                            OnClick="btnFilterAccount_Click" TabIndex="2" Width="90px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 96px">
            </td>
            <td style="width: 30px">
            </td>
            <td rowspan="12" style="width: 100px; text-align: right; vertical-align: top">
                <asp:UpdateProgress ID="upSpinner" runat="server" DisplayAfter="500">
                    <ProgressTemplate>
                        <asp:Image ID="imgWheel" runat="server" ImageUrl="~/layout/images/wheel.gif" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td colspan="4">
                <asp:UpdatePanel ID="updAccountFinder" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnFilterAccount" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Panel ID="pnlAccountFinder" runat="server" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" Visible="False" Width="440px">
                            <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true"
                                TabIndex="3" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblInstrumentLabel" runat="server" Text="Instrument:"></asp:Label>
            </td>
            <td>
                <asp:UpdatePanel ID="updRfvInstrument" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:RequiredFieldValidator ID="rfvInstrument" runat="server" ControlToValidate="ddlInstrument"
                            ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" Width="0px"
                            ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="height: 24px" colspan="2">
                <asp:UpdatePanel ID="updInstrumentsList" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctlInstrumentFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInstrument" SkinID="custom-width" runat="server" Width="300px"
                            DataSourceID="odsSelectedInstrument" DataTextField="DisplayIsinWithName" DataValueField="Key"
                            AutoPostBack="True" OnDataBound="ddlInstrument_DataBound" EnableViewState="true"
                            TabIndex="11" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsSelectedInstrument" runat="server" SelectMethod="GetTradeableInstrumentsDDL"
                            TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
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
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:UpdatePanel ID="updFilterInstrumentButton" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Button ID="btnFilterInstrument" runat="server" CausesValidation="false" Text="Filter  >>"
                            OnClick="btnFilterInstrument_Click" TabIndex="12" Width="90px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td colspan="4">
                <asp:UpdatePanel ID="updInstrumentFinder" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnFilterInstrument" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Panel ID="pnlInstrumentFinder" runat="server" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" Visible="False" Width="440px">
                            <uc2:InstrumentFinder ID="ctlInstrumentFinder" runat="server" TabIndex="13" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
                <asp:Label ID="lblCashPositionLabel" runat="server" Text="Cash Position:"></asp:Label>
            </td>
            <td>
            </td>
            <td colspan="4">
                <asp:UpdatePanel ID="updCashPosition" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="lblCashPosition" runat="server" Font-Bold="True"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
                <asp:Label ID="lblMoneyFundLabel" runat="server" Text="Money Fund Position:"></asp:Label>
            </td>
            <td>
            </td>
            <td colspan="4">
                <asp:UpdatePanel ID="updMoneyFund" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="lblMoneyFund" runat="server" Font-Bold="True"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
                <asp:Label ID="lblOpenOrdersLabel" runat="server" Text="Open Orders:"></asp:Label>
            </td>
            <td>
            </td>
            <td colspan="4">
                <asp:UpdatePanel ID="updOpenOrders" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="lblOpenOrders" runat="server" Font-Bold="True"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
                <asp:Label ID="lblSideLabel" runat="server" Text="Side:"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:UpdatePanel ID="updRfvSide" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:RequiredFieldValidator ID="rfvSide" runat="server" ControlToValidate="rblSide"
                            ErrorMessage="" SetFocusOnError="True" ValidationGroup="PlaceOrder" Width="0px">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="4">
                <asp:RadioButtonList ID="rblSide" runat="server" Width="180px" RepeatDirection="Horizontal"
                    TabIndex="19">
                    <asp:ListItem>Buy</asp:ListItem>
                    <asp:ListItem>Sell</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
                <asp:Label ID="lblTypeSizeLabel" runat="server" Text="Type:"></asp:Label>
            </td>
            <td>
            </td>
            <td colspan="4">
                <div style="position: relative; width: 200px">
                    <div style="position: absolute; left: 3px; top: -10px">
                        <asp:RadioButton ID="rbTypeSize" runat="server" GroupName="rbType" Text="Size" OnCheckedChanged="rbType_CheckedChanged"
                            OnPreRender="rbType_PreRender" TabIndex="20" />
                    </div>
                    <div style="position: absolute; left: 93px; top: -10px">
                        <asp:RadioButton ID="rbTypeAmount" runat="server" GroupName="rbType" Text="Amount"
                            OnCheckedChanged="rbType_CheckedChanged" OnPreRender="rbType_PreRender" TabIndex="21" />
                    </div>
                </div>
            </td>
        </tr>
        <tr style="height: 0px">
            <td style="white-space: nowrap">
                <asp:UpdatePanel ID="updLabels" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="rbTypeSize" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="rbTypeAmount" EventName="CheckedChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:MultiView ID="mvwLabels" runat="server">
                            <asp:View ID="vwSizeLb" runat="server">
                                <table cellpadding="0px" cellspacing="0px">
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap">
                                            <asp:Label ID="lblSizeLabel" runat="server" Text="Size:"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap;">
                                            <asp:Label ID="lblSizeCommissionLabel" runat="server" Text="Commission:"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="vwAmountLb" runat="server">
                                <table cellpadding="0px" cellspacing="0px">
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap">
                                            <asp:Label ID="lblAmountLabel" runat="server" Text="Amount:"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap">
                                            <asp:Label ID="lblAmountCommissionLabel" runat="server" Text="Commission:"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="white-space: nowrap; height: 0px">
                <asp:UpdatePanel ID="updValid" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlInstrument" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountFinder" />
                        <asp:AsyncPostBackTrigger ControlID="ctlInstrumentFinder" />
                        <asp:AsyncPostBackTrigger ControlID="rbTypeSize" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="rbTypeAmount" EventName="CheckedChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:MultiView ID="mvwValid" runat="server" Visible="False">
                            <asp:View ID="vwSizeVal" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap">
                                            <asp:RequiredFieldValidator ID="rfvSize" runat="server" ControlToValidate="dbSize:tbDecimal"
                                                ErrorMessage="" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="vwAmountVal" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap">
                                            <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="dbAmount:tbDecimal"
                                                ErrorMessage="" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap">
                                            <asp:RequiredFieldValidator ID="rfvCommission" runat="server" ControlToValidate="rblCommission"
                                                ErrorMessage="" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="4">
                <asp:UpdatePanel ID="updSizeAmount" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlAccount" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlInstrument" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountFinder" />
                        <asp:AsyncPostBackTrigger ControlID="ctlInstrumentFinder" />
                        <asp:AsyncPostBackTrigger ControlID="rbTypeSize" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="rbTypeAmount" EventName="CheckedChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:MultiView ID="mvwSizeAmount" runat="server" Visible="False">
                            <asp:View ID="vwSize" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap; width: 270px;">
                                            <db:DecimalBox ID="dbSize" runat="server" DecimalPlaces="6" TabIndex="22" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 270px; white-space: nowrap">
                                            <asp:CheckBox ID="chkNoCommission" runat="server" Text="No commission" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="vwAmount" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="height: 30px; white-space: nowrap; width: 270px;">
                                            <db:DecimalBox ID="dbAmount" runat="server" DecimalPlaces="2" TabIndex="23" />
                                            <asp:DropDownList ID="ddlCurrency" SkinID="custom-width" runat="server" Width="60px"
                                                DataSourceID="odsCurrency" DataTextField="Symbol" DataValueField="Key" OnDataBound="ddlCurrency_DataBound"
                                                TabIndex="24">
                                            </asp:DropDownList>
                                            &nbsp;
                                            <asp:RequiredFieldValidator ID="rfvCurrency" runat="server" ControlToValidate="ddlCurrency"
                                                ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" Width="0px"
                                                ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                            <asp:ObjectDataSource ID="odsCurrency" runat="server" SelectMethod="GetCurrencies"
                                                TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.SingleOrderAdapter"
                                                OldValuesParameterFormatString="original_{0}">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlAccount" Name="accountId" PropertyName="SelectedValue"
                                                        Type="Int32" />
                                                    <asp:ControlParameter ControlID="ddlInstrument" Name="instrumentId" PropertyName="SelectedValue"
                                                        Type="Int32" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 350px; white-space: nowrap">
                                            <asp:RadioButtonList ID="rblCommission" runat="server" Width="300px" RepeatDirection="Horizontal"
                                                TabIndex="25">
                                                <asp:ListItem>Inclusive</asp:ListItem>
                                                <asp:ListItem>Exclusive</asp:ListItem>
                                                <asp:ListItem>No commission</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: normal" colspan="6">
                <asp:UpdatePanel ID="updErrorMessages" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px"></b4f:ErrorLabel>
                        <br />
                        <span id="waitMessage" class='padding' style='display: block; color: Red'></span>
                        <span class='padding' style='display: block'>
                            <asp:ValidationSummary ID="vsValSummary" runat="server" Width="570px" HeaderText="Please fill in all required fields."
                                ValidationGroup="PlaceOrder" Height="0px" />
                            <asp:RadioButtonList ID="rblIgnoreWarning" runat="server" RepeatDirection="Horizontal"
                                Visible="False" Width="158px" TabIndex="26">
                                <asp:ListItem>Yes</asp:ListItem>
                                <asp:ListItem Selected="True">No</asp:ListItem>
                            </asp:RadioButtonList>
                        </span>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
                <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click"
                    ValidationGroup="PlaceOrder" CausesValidation="true" TabIndex="27" />
            </td>
            <td colspan="3">
                <asp:CheckBox ID="cbClearScreen" runat="server" Checked="True" Text="Clear screen after placing order"
                    TabIndex="28" />
            </td>
            <td colspan="2">
                <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" Text="Clear Screen"
                    UseSubmitBehavior="false" TabIndex="30" />
            </td>
        </tr>
        <tr>
            <td style="height: 26px; white-space: nowrap">
            </td>
            <td colspan="5">
                <asp:UpdatePanel ID="updBypassValidation" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:CheckBox ID="cbBypassValidation" runat="server" Checked="False" Text="Bypass validation"
                            Visible="False" TabIndex="29" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

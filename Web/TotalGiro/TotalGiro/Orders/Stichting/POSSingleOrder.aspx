<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="POSSingleOrder.aspx.cs" 
    Inherits="POSSingleOrder" Title="Single Order for Stichting POS ONLY!" Theme="Neutral" %>

<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <br />
    <table style="width: 580px">
        <tr>
            <td style="width: 120px; height: 24px">
                <asp:Label ID="Label1" runat="server" Text="Account:"></asp:Label>
            </td>
            <td style="width: 30px; height: 24px">
                <asp:RequiredFieldValidator ID="rfvAccount" runat="server" ControlToValidate="ddlAccount"
                    ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator></td>
            <td style="width: 200px; height: 24px;white-space:nowrap;">
                <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="200px" DataSourceID="odsSelectedAccount"
                    DataTextField="DisplayNumberWithName" DataValueField="Key" AutoPostBack="True" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" OnDataBound="ddlAccountInstrument_DataBound">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:ObjectDataSource ID="odsSelectedAccount" runat="server" SelectMethod="GetPosAccount"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.SinglePOSOrderAdapter">
                </asp:ObjectDataSource>
            </td>
            <td style="width: 100px; height: 24px">
                </td>
            <td style="width: 30px; height: 24px">
            </td>
        </tr>
        <tr>
            <td></td>
            <td style="width: 20px">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Instrument:"></asp:Label>
            </td>
            <td style="width: 20px">
                <asp:RequiredFieldValidator ID="rfvInstrument" runat="server" ControlToValidate="ddlInstrument"
                    ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator></td>
            <td style="width: 300px; height: 24px" colspan="2">
                <asp:DropDownList ID="ddlInstrument" SkinID="custom-width" runat="server" Width="300px" DataSourceID="odsSelectedInstrument"
                    DataTextField="DisplayIsinWithName" DataValueField="Key" AutoPostBack="True" OnDataBound="ddlAccountInstrument_DataBound">
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
            </td>
            <td>
                <asp:Button ID="btnFilterInstrument" runat="server" CausesValidation="false" Text="Filter  >>" OnClick="btnFilterInstrument_Click" />
            </td>
            <td>
                </td>
        </tr>
        <tr>
            <td></td>
            <td style="width: 20px">
            </td>
            <td colspan="4">
                <asp:Panel ID="pnlInstrumentFinder" runat="server" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Visible="False">
                    <uc2:InstrumentFinder ID="ctlInstrumentFinder" runat="server" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;">
                </td>
            <td style="white-space: nowrap; height: 26px; width: 20px;">
            </td>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;">
                </td>
            <td style="white-space: nowrap; height: 26px; width: 20px;">
            </td>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;">
                <asp:Label ID="Label6" runat="server" Text="Side:"></asp:Label></td>
            <td style="white-space: nowrap; height: 26px; width: 20px;">
                <asp:RequiredFieldValidator ID="rfvSide" runat="server" ControlToValidate="rblSide"
                    ErrorMessage="" SetFocusOnError="True" ValidationGroup="PlaceOrder"
                    Width="0px">*</asp:RequiredFieldValidator></td>
            <td colspan="4">
                <asp:RadioButtonList ID="rblSide" runat="server" Width="180px" CausesValidation="True" RepeatDirection="Horizontal" AutoPostBack="True" >
                    <asp:ListItem>Buy</asp:ListItem>
                    <asp:ListItem>Sell</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;">
                <asp:Label ID="Label5" runat="server" Text="Type:"></asp:Label></td>
            <td style="white-space: nowrap; height: 26px; width: 20px;">
                <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="rblType"
                    ErrorMessage="" SetFocusOnError="True" ValidationGroup="PlaceOrder" Width="0px">*</asp:RequiredFieldValidator></td>
            <td colspan="4">
                <asp:RadioButtonList ID="rblType" runat="server" Width="220px" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblType_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem>Size</asp:ListItem>
                    <asp:ListItem>Amount</asp:ListItem>
                </asp:RadioButtonList></td>
        </tr>
        <tr>
            <td style="height: 0px; white-space: nowrap;" >
                <asp:MultiView ID="mvwLabels" runat="server">
                    <asp:View ID="vwSizeLb" runat="server">
                        <table cellpadding="0px" cellspacing="0px">
                            <tr>
                                <td style="height: 30px;white-space:nowrap;">
                                    <asp:Label ID="Label7" runat="server" Text="Size:"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="vwAmountLb" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 30px;white-space:nowrap;">
                                    <asp:Label ID="Label8" runat="server" Text="Amount:"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 30px;white-space:nowrap;">
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView></td>
            <td style="white-space: nowrap; height: 0px; width: 20px;">
                <asp:MultiView ID="mvwValid" runat="server" Visible="False">
                    <asp:View ID="vwSizeVal" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 30px;white-space:nowrap">
                                    <asp:RequiredFieldValidator ID="rfvSize" runat="server" ControlToValidate="dbSize:tbDecimal"
                                        ErrorMessage="" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="vwAmountVal" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 30px;white-space:nowrap">
                                    <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="dbAmount:tbDecimal"
                                        ErrorMessage="" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </td>
            <td colspan="4">
                <asp:MultiView ID="mvwSizeAmount" runat="server" Visible="False">
                    <asp:View ID="vwSize" runat="server">
                        <db:DecimalBox ID="dbSize" runat="server" DecimalPlaces="6" />
                    </asp:View>
                    <asp:View ID="vwAmount" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 30px;white-space:nowrap; width: 270px;">
                                    <db:DecimalBox ID="dbAmount" runat="server" DecimalPlaces="2" />
                                    <asp:DropDownList ID="ddlCurrency" SkinID="custom-width" runat="server" Width="60px" DataSourceID="odsCurrency" DataTextField="Symbol" DataValueField="Key" OnDataBound="ddlCurrency_DataBound"></asp:DropDownList>
                                    &nbsp;
                                    <asp:RequiredFieldValidator ID="rfvCurrency" runat="server" ControlToValidate="ddlCurrency"
                                        ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" Width="0px" ValidationGroup="PlaceOrder">*</asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsCurrency" runat="server" SelectMethod="GetCurrencies"
                                        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.SingleOrderAdapter" OldValuesParameterFormatString="original_{0}">
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
                                <td style="width: 270px">
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;width:580px" colspan="6">
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label><br />
                <asp:ValidationSummary ID="vsValSummary" runat="server" Width="570px" HeaderText="Please fill in all required fields." 
                    ValidationGroup="PlaceOrder" Height="0px" />
                <asp:RadioButtonList ID="rblIgnoreWarning" runat="server" RepeatDirection="Horizontal" Visible="False" Width="158px">
                    <asp:ListItem>Yes</asp:ListItem>
                    <asp:ListItem Selected="True">No</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;">
                <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click" ValidationGroup="PlaceOrder" 
                    OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').style.visibility = 'hidden'" />
            </td>
            <td colspan="3">
                <asp:CheckBox ID="chkClearScreen" runat="server" Checked="True" Text="Clear screen after placing order" /></td>
            <td colspan="2">
                <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" Text="Clear Screen" UseSubmitBehavior="False" />
            </td>
        </tr>
        <tr>
            <td style="height: 26px;white-space:nowrap;"></td>
            <td colspan="5">
                <asp:CheckBox ID="chkBypassValidation" runat="server" Checked="False" Text="Bypass validation" Visible="False" />
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">
        //var ob = document.getElementById('btnSearch').title;
    </script>
</asp:Content>


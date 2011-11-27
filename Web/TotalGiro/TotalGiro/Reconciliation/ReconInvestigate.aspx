<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ReconInvestigate.aspx.cs" Inherits="Reconciliation_ReconInvestigate" Title="Investigate Reconciliation" Theme="Neutral" %>

<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc4" %>

<%@ Register Src="../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc3" %>

<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="bodyContentPlaceHolder">
    <br />
    Met dit report kan een overzicht gegenereerd worden over de reconciliatie van een
    instrument of klant binnen de bepaalde tijd.<br />
    <table style="width: 580px">
        <tr>
            <td style="width: 120px; height: 24px">
    Begindatum:</td>
            <td style="width: 30px; height: 24px">
            </td>
            <td style="width: 200px; white-space: nowrap; height: 24px">
    <uc1:DatePicker ID="DatePickerBegin" runat="server" SelectedDate='12/31/2004' />
            </td>
            <td style="width: 100px; height: 24px">
            </td>
            <td style="width: 100px; height: 24px">
            </td>
            <td style="width: 30px; height: 24px">
            </td>
        </tr>
        <tr>
            <td style="width: 120px; height: 24px">
    Einddatum:&nbsp;</td>
            <td style="width: 30px; height: 24px">
            </td>
            <td style="width: 200px; white-space: nowrap; height: 24px">
    <uc1:DatePicker ID="DatePickerEnd" runat="server" />
            </td>
            <td style="width: 100px; height: 24px">
            </td>
            <td style="width: 100px; height: 24px">
            </td>
            <td style="width: 30px; height: 24px">
            </td>
        </tr>
        <tr>
            <td style="width: 120px; height: 24px">
                <asp:Label ID="Label1" runat="server" Text="Account:"></asp:Label>
            </td>
            <td style="width: 30px; height: 24px">
                <asp:RequiredFieldValidator ID="rfvAccount" runat="server" ControlToValidate="ddlAccount"
                    ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" ValidationGroup="PlaceOrder"
                    Width="0px">*</asp:RequiredFieldValidator></td>
            <td style="width: 200px; white-space: nowrap; height: 24px">
                <asp:DropDownList ID="ddlAccount" runat="server" AutoPostBack="True" DataSourceID="odsSelectedAccount"
                    DataTextField="DisplayNumberWithName" DataValueField="Number"  SkinID="custom-width"
                    Width="200px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>&nbsp;
                <asp:ObjectDataSource ID="odsSelectedAccount" runat="server" SelectMethod="GetCustomerAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter" >
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
            </td>
            <td style="width: 100px; height: 24px">
                <asp:Button ID="btnFilterAccount" runat="server" CausesValidation="false" OnClick="btnFilterAccount_Click"
                    Text="Filter  >>" /></td>
            <td style="width: 100px; height: 24px">
            </td>
            <td style="width: 30px; height: 24px">
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="width: 20px">
            </td>
            <td colspan="3">
                <asp:Panel ID="pnlAccountFinder" runat="server" BorderColor="Silver" BorderStyle="Solid"
                    BorderWidth="1px" Visible="False">
                    <uc4:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true">
                    </uc4:AccountFinder>
                </asp:Panel>
                &nbsp;
            </td>
            <td colspan="1">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Instrument:"></asp:Label>
            </td>
            <td style="width: 20px">
                <asp:RequiredFieldValidator ID="rfvInstrument" runat="server" ControlToValidate="ddlInstrument"
                    ErrorMessage="" InitialValue="-2147483648" SetFocusOnError="True" ValidationGroup="PlaceOrder"
                    Width="0px">*</asp:RequiredFieldValidator></td>
            <td colspan="2" style="width: 300px; height: 24px">
                <asp:DropDownList ID="ddlInstrument" runat="server" AutoPostBack="True" DataSourceID="odsSelectedInstrument"
                    DataTextField="DisplayIsinWithName" DataValueField="Isin" 
                    SkinID="custom-width" Width="300px">
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
                <asp:Button ID="btnFilterInstrument" runat="server" CausesValidation="false" OnClick="btnFilterInstrument_Click"
                    Text="Filter  >>" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="width: 20px">
            </td>
            <td colspan="4">
                <asp:Panel ID="pnlInstrumentFinder" runat="server" BorderColor="Silver" BorderStyle="Solid"
                    BorderWidth="1px" Visible="False">
                    <uc3:InstrumentFinder ID="ctlInstrumentFinder" runat="server">
                    </uc3:InstrumentFinder>
                </asp:Panel>
                &nbsp;
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="Button1" runat="server" Text="Generate Report" OnClick="Button1_Click" /><br />
</asp:Content>



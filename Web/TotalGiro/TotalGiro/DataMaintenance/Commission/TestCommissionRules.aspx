<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestCommissionRules.aspx.cs" Inherits="Commission_TestCommissionRules" Title="Test Commission Rules" %>

<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc2" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="scriptman" runat="server" />
    <asp:HiddenField ID="hdnIdValue" runat="server" EnableViewState="False" />
    
    <table cellpadding="0" cellspacing="0" border="0" style="border-color:black" width="780">
        <tr>
            <td class="tblHeader">Test Commission Rule</td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width:100%">
                    <tr>
                        <td style="width: 200px; height: 35px;"><asp:Label ID="lblAccount" runat="server" Text="Account"></asp:Label></td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="205px" DataSourceID="odsAccount"
                                DataTextField="DisplayNumberWithName" DataValueField="Key" OnDataBound="ddlAccount_DataBound">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button ID="btnFilterAccount" runat="server" CausesValidation="false" Text="Filter  >>" Height="22px"
                                OnClick="btnFilterAccount_Click"/>
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
                                    <asp:Parameter  Name="propertyList" DefaultValue="Key, DisplayNumberWithName" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:RequiredFieldValidator 
                                ID="reqAccountVal" 
                                runat="server"
                                ControlToValidate="ddlAccount"
                                InitialValue="-2147483648" 
                                Text="*" 
                                ErrorMessage="Accounts"/>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 0px"></td>
                        <td>
                            <asp:Panel ID="pnlAccountFinder" runat="server" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Visible="False">
                                <uc2:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;"><asp:Label ID="Label8" runat="server" Text="Instrument"></asp:Label></td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlInstrument" SkinID="custom-width" runat="server" Width="205px" 
                                DataSourceID="odsInstrument" DataTextField="DisplayIsinWithName" DataValueField="Key">
                            </asp:DropDownList>
                            <asp:Button ID="btnFilterInstrument" runat="server" CausesValidation="false" Text="Filter  >>" Height="22px"
                                OnClick="btnFilterInstrument_Click" />
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
                            <asp:RequiredFieldValidator 
                                ID="reqInstrumentVal" 
                                runat="server"
                                ControlToValidate="ddlInstrument"
                                InitialValue="-2147483648" 
                                Text="*" 
                                ErrorMessage="Instrument"/>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 0px"></td>
                        <td>
                            <asp:Panel ID="pnlInstrumentFinder" runat="server" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Visible="False" >
                                <uc3:InstrumentFinder ID="ctlInstrumentFinder" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="lblOrderActionType" runat="server" Text="Order Action Type"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlOrderActionType" runat="server" Width="200px"
                                DataSourceID="odsOrderActionType" DataTextField="Description" DataValueField="Key" />
                            <asp:ObjectDataSource ID="odsOrderActionType" runat="server" SelectMethod="GetOrderActionTypeOptions"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.TestCommissionRuleAdapter">
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label4" runat="server" Text="Buy/Sell"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlBuySell" runat="server" Width="200px"
                                DataSourceID="odsBuySell" DataTextField="Description" DataValueField="Key" />
                            <asp:ObjectDataSource ID="odsBuySell" runat="server" SelectMethod="GetBuySellOptions"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.TestCommissionRuleAdapter">
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label14" runat="server" Text="Original OrderType"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlOriginalOrderType" runat="server" Width="80px"
                                DataSourceID="odsOriginalOrderType" DataTextField="Description" DataValueField="Key"
                                OnSelectedIndexChanged="ddlOriginalOrderType_SelectedIndexChanged"
                                AutoPostBack="true" />
                            <asp:ObjectDataSource ID="odsOriginalOrderType" runat="server" SelectMethod="GetBaseOrderTypes"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.TestCommissionRuleAdapter">
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="lblDate" runat="server" Text="Date" />
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <uc1:DatePicker ID="dtpDate" runat="server" ListYearsAfterCurrent="1" ListYearsBeforeCurrent="10"  />
                        </td>
                    </tr>
                    
                    <tr>
                        <td><asp:Label ID="lblAmount" runat="server">Amount</asp:Label></td>
                        <td><db:DecimalBox 
                                ID="dbAmount" 
                                runat="server" 
                                DecimalPlaces="4"
                                Enabled="true"
                                />
                            <asp:Label ID="lblCurrency" runat="server" />
                            <asp:RequiredFieldValidator 
                                ID="reqAmount" 
                                runat="server"
                                ControlToValidate="dbAmount:tbDecimal"
                                Text="*" 
                                ErrorMessage="Amount"/>
                       </td>
                   </tr>                    
                    <tr>
                        <td><asp:Label ID="lblOrderValue" runat="server">Size</asp:Label></td>
                        <td><db:DecimalBox 
                                ID="dbSize" 
                                DecimalPlaces="6"
                                runat="server"
                                Enabled="false" />
                            <asp:RequiredFieldValidator 
                                ID="reqSize" 
                                runat="server"
                                ControlToValidate="dbSize:tbDecimal"
                                Text="*" 
                                ErrorMessage="Size"
                                Enabled="false" />
                            
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblPrice" runat="server">Price</asp:Label></td>
                        <td><db:DecimalBox 
                                ID="dbPrice" 
                                runat="server" 
                                DecimalPlaces="4"
                                Enabled="true"
                                />
                       </td>
                   </tr>                    
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label3" runat="server" Text="Is Value Including Commission"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:CheckBox ID="cbIsValueIncludingCommission" runat="server"/>
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblCalcFoundLabel" runat="server">Calculation found</asp:Label></td>
                        <td><asp:Label ID="lblCalcFound" runat="server"></asp:Label></td>
                   </tr>                    
                </table>
            </td>
        </tr>
    </table>
    <br />
    <asp:ValidationSummary ID="valSummary" runat="server" DisplayMode="SingleParagraph" HeaderText="* Fill in all required fields:" CssClass="padding" />
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
    &nbsp;
    <asp:Button ID="btnTest" runat="server" OnClick="btnTest_Click" Text="Test" />&nbsp;
 </asp:Content>

<%@ Page Title="Test Bonds" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="BondTest.aspx.cs" Inherits="DataMaintenance_Instruments_BondTest" %>

<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="scriptman" runat="server" />

    <table cellpadding="0" cellspacing="0" border="0" style="border-color:black" width="780">
        <tr>
            <td class="tblHeader">Test Bonds</td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width:100%">
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
                                <uc3:InstrumentFinder ID="ctlInstrumentFinder" runat="server" SecCategoryId="2" />
                            </asp:Panel>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="lblDate" runat="server" Text="Settlement Date" />
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <uc1:DatePicker ID="dtpSettlementDate" runat="server" ListYearsAfterCurrent="10" ListYearsBeforeCurrent="10"  />
                            <asp:RequiredFieldValidator 
                                ID="reqSettlementDate" 
                                runat="server"
                                ControlToValidate="dtpSettlementDate:txtDate"
                                Text="*" 
                                ErrorMessage="Settlement Date"/>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 200px; height: 35px;">
                            <asp:Label ID="Label14" runat="server" Text="Original OrderType"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;">
                            <asp:DropDownList ID="ddlOrderType" runat="server" Width="80px"
                                DataSourceID="odsOrderType" DataTextField="Description" DataValueField="Key"
                                OnSelectedIndexChanged="ddlOrderType_SelectedIndexChanged"
                                AutoPostBack="true" />
                            <asp:ObjectDataSource ID="odsOrderType" runat="server" SelectMethod="GetBaseOrderTypes"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.TestCommissionRuleAdapter">
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    
                    <tr>
                        <td><asp:Label ID="lblSize" runat="server">Size</asp:Label></td>
                        <td><db:DecimalBox 
                                ID="dbSize" 
                                DecimalPlaces="6"
                                runat="server"
                                Enabled="true" />
                            <asp:RequiredFieldValidator 
                                ID="reqSize" 
                                runat="server"
                                ControlToValidate="dbSize:tbDecimal"
                                Text="*" 
                                ErrorMessage="Size"
                                Enabled="true" />
                            
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblAmount" runat="server">Amount</asp:Label></td>
                        <td><db:DecimalBox 
                                ID="dbAmount" 
                                runat="server" 
                                DecimalPlaces="4"
                                Enabled="false"
                                />
                            <asp:Label ID="lblCurrency" runat="server" />
                            <asp:RequiredFieldValidator 
                                ID="reqAmount" 
                                runat="server"
                                ControlToValidate="dbAmount:tbDecimal"
                                Text="*" 
                                ErrorMessage="Amount"
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
                        <td>
                            <asp:Label ID="lblExchange" runat="server">Exchange</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlExchange" runat="server" Width="165px" DataSourceID="odsExchange" 
                                DataTextField="ExchangeName" DataValueField="Key" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter"></asp:ObjectDataSource>
                       </td>
                   </tr>
                    <tr>
                   </tr>
                    <tr>
                        <td><asp:Label ID="lblResultLabel" runat="server">Calculated acc. interest</asp:Label></td>
                        <td><asp:Label ID="lblResult" runat="server"></asp:Label></td>
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


<%@ Page Language="C#" EnableEventValidation="false" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="EditMutualFund.aspx.cs" Inherits="DataMaintenance_MutualFund" Title="Maintenance Mutual Fund" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table border="1" style="border-bottom: 1px Black; border-left: 1px Black; border-right: 1px Black;" cellpadding="5" cellspacing="0" width="770">
        <caption>Instrument Definition</caption>
        <tr>
            <td style="width: 770px; vertical-align: middle; text-align: center;">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="width: 113px; text-align: right; vertical-align: middle; height: 25px;">
                            <asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>
                        </td>
                        <td style="width: 170px; vertical-align: middle; text-align: left; height: 25px;">
                            <asp:TextBox ID="tbName" SkinID="broad" runat="server"></asp:TextBox>
                        </td>
                        <td style="width: 114px; text-align: right; vertical-align: middle; height: 25px;">
                            <asp:Label ID="lblCompanyName" runat="server">CompanyName</asp:Label>
                        </td>
                        <td style="width: 183px; vertical-align: middle; text-align: left; height: 25px;">
                            <asp:TextBox ID="tbCompanyName" SkinID="broad" runat="server"></asp:TextBox>
                        </td>
                        <td >&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 113px; height: 21px; text-align: right; vertical-align: middle; height: 25px;">
                            <asp:Label ID="lblCurrencyNominal" runat="server">Curr. Nominal</asp:Label></td>
                        <td style="width: 170px; height: 21px;  vertical-align: middle; text-align: left;">
                            <asp:DropDownList ID="ddCurrencyNominal" runat="server">
                            </asp:DropDownList></td>
                        <td style="width: 183px; height: 21px; vertical-align: middle; text-align: left;">
                            <asp:CheckBox ID="cbDayTradeable" runat="server" /></td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 113px; text-align: right; vertical-align: middle; height: 25px;">
                            <asp:Label ID="lblIsCash" runat="server">Cash</asp:Label></td>
                        <td style="width: 170px; vertical-align: middle; text-align: left; height: 25px;">
                            <asp:CheckBox ID="cbIsCash" runat="server" /></td>
                        <td style="width: 114px; text-align: right; vertical-align: middle; height: 25px;">
                            <asp:Label ID="lblIsin" runat="server">ISIN</asp:Label></td>
                        <td style="width: 183px;text-align: left; vertical-align: middle; height: 25px;">
                            <asp:TextBox ID="tbIsin" runat="server"></asp:TextBox>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 113px; text-align: right; height: 23px; vertical-align: middle;">
                            <asp:Label ID="lblIsTradeable" runat="server">Tradeable</asp:Label></td>
                        <td style="width: 170px; vertical-align: middle; text-align: left; height: 23px;">
                            <asp:CheckBox ID="cbIsTradeable" runat="server" /></td>
                        <td style="width: 114px; text-align: right; vertical-align: middle; height: 23px;">
                            <asp:Label ID="lblDefaultExchange" runat="server" Text="Default Exchange"></asp:Label>&nbsp;</td>
                        <td style="width: 183px; vertical-align: middle; text-align: left; height: 23px;">
                            &nbsp;<asp:DropDownList ID="ddDefaultExchange" runat="server">
                            </asp:DropDownList></td>
                        <td style="height: 23px">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <table border="0" cellpadding="0" cellspacing="0" width="770px">
        <tr>
            <td style="vertical-align: top; text-align: left; height: 231px;">
                <table border="1" style="border-bottom: solid 1px Black; border-left: solid 1px Black; border-right: solid 1px Black;" cellpadding="0" cellspacing="0" width="100%">
                    <caption>Exchanges</caption>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlExchanges" runat="server">
                                <asp:GridView
                                    ID="gvExchanges"
                                    AllowSorting="true"
                                    AllowPaging="true"
                                    OnSelectedIndexChanging="gvExchanges_SelectedIndexChanging"
                                    PageSize="5"
                                    SkinID="custom-width" Width="380px"
                                    AutoGenerateColumns="False"
                                    runat="server"
                                    DataKeyNames="Key" 
                                    DataSourceID="odsExchanges" 
                                    OnRowDataBound="gvExchanges_RowDataBound">
                                    <Columns>
                                        <asp:ButtonField ButtonType="Link" Visible="false" CommandName="Select" />
                                        <asp:BoundField DataField="ExchangeName" HeaderText="ExchangeName" SortExpression="ExchangeName" />
                                        <asp:BoundField DataField="DefaultCountry" HeaderText="DefaultCountry" SortExpression="DefaultCountry" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 10px; height: 231px;">&nbsp;</td>
            <td style="width: 380px; vertical-align: top; text-align: left; height: 231px;">
                <table border="1" style="border-bottom: 1px Black; border-left: 1px Black; border-right: 1px Black;" cellpadding="5" cellspacing="0" width="100%">
                    <caption>Exchange Details</caption>
                    <tr>
                        <td style="width: 380px; vertical-align: middle; text-align: left;">
                            <asp:Panel 
                                ID="pnlExchangeDetails" 
                                runat="server">
                                <table width="370px" border="0" cellpadding="0" cellspacing="0" style="height: 105px;">
                                    <tr>
                                        <td style="vertical-align: middle; height: 25px; text-align: right">
                                            <asp:Label ID="lblExchangeName" runat="server">Name Exchange</asp:Label></td>
                                        <td style="height: 25px">
                                            <asp:TextBox ID="tbExchangeName" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align: middle; text-align: right; height: 25px;">
                                            <asp:Label ID="lblExchangeCurrency" runat="server" Text="Default Currency"></asp:Label></td>
                                        <td style="vertical-align: middle; text-align: left; height: 25px;">
                                            <asp:DropDownList ID="ddDefaultExchangeCurrency" runat="server">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align: middle; text-align: right; height: 25px;">
                                            <asp:Label ID="Label1" runat="server" Text="Default Country"></asp:Label></td>
                                        <td style="vertical-align: middle; text-align: left; height: 25px;">
                                            <asp:DropDownList
                                                ID="ddDefaultExchangeCountry"
                                                runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align: middle; text-align: right; height: 25px;">
                                            <asp:Label ID="lblNumberOfDecimals" runat="server">Number of Decimals</asp:Label></td>
                                        <td style="vertical-align: middle; text-align: left; height: 25px;">
                                            <asp:TextBox ID="tbNumberOfDecimals" runat="server"></asp:TextBox></td>
                                    </tr>
                                </table>
                            </asp:Panel>                        
                        </td>
                     </tr>
                </table>
             </td>
        </tr>
    </table>
    <asp:ObjectDataSource 
        ID="odsExchanges"
        runat="server" 
        SelectMethod="GetExchanges" 
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.EditMutualFund">
        <SelectParameters>
            <asp:ControlParameter ControlID="hfInstrumentID" Name="instrumentID" PropertyName="Value"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:HiddenField ID="hfInstrumentID" Value="-1" runat="server" />
</asp:Content>


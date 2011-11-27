<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="InstrumentCurrentPrices.aspx.cs" Inherits="InstrumentCurrentPrices" Title="Instrument Current Prices" Theme="Neutral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:RadioButtonList ID="rblDataSourceChoice" runat="server" DataSourceID="odsDataSourceChoices" DataTextField="InstrumentType" DataValueField="ID" OnSelectedIndexChanged="rblDataSourceChoice_SelectedIndexChanged" AutoPostBack="true">
    </asp:RadioButtonList><br />
    <asp:ObjectDataSource ID="odsDataSourceChoices" runat="server" SelectMethod="GetDataSourceChoices" 
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter">
    </asp:ObjectDataSource>
    <asp:MultiView ID="mlvPricesView" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweInstrumentCurrentPrices" runat="server">
            <asp:GridView ID="gvInstrumentPrices" runat="server" AutoGenerateColumns="false" Caption="Current Prices" CaptionAlign="Top" PageSize="20" DataSourceID="odsInstrumentPrices" AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:BoundField HeaderText="ISIN" DataField="Isin" SortExpression="Isin">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Instrument" DataField="DisplayName" SortExpression="DisplayName">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Current Price" DataField="DisplayCurrentPrice" SortExpression="DisplayCurrentPrice">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Last Updated" SortExpression="DisplayCurrentPriceDate">
                        <ItemTemplate>
                            <%# ((DateTime)DataBinder.Eval(Container.DataItem, "DisplayCurrentPriceDate") != DateTime.MinValue ? 
                                ((DateTime)DataBinder.Eval(Container.DataItem, "DisplayCurrentPriceDate")).ToString("d MMMM yyyy") : "") %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        <asp:ObjectDataSource ID="odsInstrumentPrices" runat="server" SelectMethod="GetInstrumentCurrentPrices" 
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter"></asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="VweCurrencyRates" runat="server">
            <asp:GridView ID="gdCurrencyRates" runat="server" AutoGenerateColumns="false" Caption="Current Exchange Rates" CaptionAlign="Top" PageSize="21" DataSourceID="odsCurrencyRates" AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:BoundField HeaderText="Symbol" DataField="Symbol" SortExpression="Symbol">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Country" DataField="CountryOfOrigin_CountryName" SortExpression="CountryOfOrigin_CountryName">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="AltSymbol" DataField="AltSymbol" SortExpression="AltSymbol">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Rate" DataField="ExchangeRate_Rate" SortExpression="ExchangeRate_Rate">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Rate Date" SortExpression="ExchangeRate_RateDate">
                        <ItemTemplate>
                            <%# ((DateTime)DataBinder.Eval(Container.DataItem, "ExchangeRate_RateDate") != DateTime.MinValue ?
                                                                ((DateTime)DataBinder.Eval(Container.DataItem, "ExchangeRate_RateDate")).ToString("d MMMM yyyy") : "")%>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsCurrencyRates" runat="server" SelectMethod="GetCurrencyCurrentRates" 
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter"></asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="VweBenchMarkPrices" runat="server">
            <asp:GridView ID="gdBenchMarkPrices" runat="server" AutoGenerateColumns="false" Caption="Current BenchMarks" CaptionAlign="Top" PageSize="20" DataSourceID="odsBenchMarkPrices" AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:BoundField HeaderText="ISIN" DataField="Isin" SortExpression="Isin">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Instrument" DataField="DisplayName" SortExpression="DisplayName">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Current Price" DataField="DisplayCurrentPrice" SortExpression="DisplayCurrentPrice">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Last Updated" SortExpression="DisplayCurrentPriceDate">
                        <ItemTemplate>
                            <%# ((DateTime)DataBinder.Eval(Container.DataItem, "DisplayCurrentPriceDate") != DateTime.MinValue ? 
                                ((DateTime)DataBinder.Eval(Container.DataItem, "DisplayCurrentPriceDate")).ToString("d MMMM yyyy") : "") %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        <asp:ObjectDataSource ID="odsBenchMarkPrices" runat="server" SelectMethod="GetBenchMarkPrices" 
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter"></asp:ObjectDataSource>
        </asp:View>
    </asp:MultiView><br />

</asp:Content>


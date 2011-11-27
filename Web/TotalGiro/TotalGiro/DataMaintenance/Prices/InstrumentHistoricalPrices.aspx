<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="InstrumentHistoricalPrices.aspx.cs" 
    Inherits="InstrumentHistoricalPrices" Title="Instrument Historical Prices" Theme="Neutral" %>

<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/CalendarPlusNavigation.ascx" TagName="CalendarPlusNav" TagPrefix="cldp" %>
<%@ Register Src="~/UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="scriptMan" runat="server" />
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblDate" runat="server" Text="Date:" />
            </td>
            <td style="width: 190px">
                <cldp:CalendarPlusNav ID="cldpDate" runat="server" />
            </td>
        </tr>
    </table>
    <asp:RadioButtonList ID="rblDataSourceChoice" runat="server" DataSourceID="odsDataSourceChoices" Width="320px" 
        DataTextField="InstrumentType" DataValueField="ID" OnSelectedIndexChanged="rblDataSourceChoice_SelectedIndexChanged" 
        AutoPostBack="true" Visible="false" RepeatDirection="Horizontal" >
    </asp:RadioButtonList>
    <asp:ObjectDataSource ID="odsDataSourceChoices" runat="server" SelectMethod="GetDataSourceChoices" 
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentHistoricalPricesAdapter">
    </asp:ObjectDataSource>
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" SecCategoryFilter="Securities" ShowExchange="false" 
        ShowSecCategory="false" ShowActivityFilter="true" ActivityFilter="Active" Visible="false" />
    <br />
    <br />
    <asp:MultiView ID="mlvHistoricalPrices" runat="server" ActiveViewIndex="-1" >
        <asp:View ID="vweInstrumentPrices" runat="server">
            <asp:GridView ID="gvInstrumentPrices" runat="server" AutoGenerateColumns="False" Caption="Prices" CaptionAlign="Left" PageSize="20" 
                DataSourceID="odsInstrumentPrices" AllowPaging="True" AllowSorting="True" Visible="true" DataKeyNames="InstrumentId" 
                OnRowCommand="gvInstrumentPrices_RowCommand"
                OnRowUpdating="gvSender_RowUpdating" 
                OnRowUpdated="gvSender_RowUpdated" >
            <Columns>
                <asp:BoundField HeaderText="ISIN" DataField="Isin" SortExpression="Isin" ReadOnly="True">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Instrument" DataField="InstrumentName" SortExpression="InstrumentName" 
                        ReadOnly="True">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Currency" DataField="Currency" SortExpression="Currency" ReadOnly="True">
                    <ItemStyle wrap="False" horizontalalign="Right" />
                    <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Price" SortExpression="PriceQuantity">
                    <ItemTemplate>
                        <asp:Label ID="lblPriceQuantity" runat="server"
                            Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "PriceQuantity") != 0 ? DataBinder.Eval(Container.DataItem, "PriceQuantity", "{0:##0.#####}") : "") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <db:DecimalBox ID="dbNewQuantity" runat="server" 
                            DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "DecimalPlaces") %>'
                            Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "PriceQuantity") != 0 ? DataBinder.Eval(Container.DataItem, "PriceQuantity", "{0:##0.#####}") : "") %>'
                             />
                    </EditItemTemplate>
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True">
                    <ItemStyle Wrap="False" Width="100px"/>
                </asp:CommandField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <asp:LinkButton 
                            runat="server" 
                            ID="lbtMissingPrices" 
                            Text="Missing Prices"
                            CommandName="MissingPrices"/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsInstrumentPrices" runat="server" SelectMethod="GetHistoricalPrices" UpdateMethod="UpdateHistoricalPrice"
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentHistoricalPricesAdapter" OldValuesParameterFormatString="original_{0}" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="cldpDate" Name="date" PropertyName="SelectedDate" 
                        Type="DateTime" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="activityFilter" PropertyName="ActivityFilter"
                        Type="Int32" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="cldpDate" Name="Date" PropertyName="SelectedDate" Type="DateTime" />
                </UpdateParameters>
            </asp:ObjectDataSource>   
            <br /> 
            <asp:GridView ID="gvMissingPrices" runat="server" AutoGenerateColumns="False" Caption="Missing Prices" CaptionAlign="Left" PageSize="20" 
                DataSourceID="odsMissingPrices" AllowPaging="True" AllowSorting="True" Visible="False" DataKeyNames="Date" 
                OnRowUpdating="gvSender_RowUpdating" 
                OnRowUpdated="gvSender_RowUpdated" >
            <Columns>
                <asp:BoundField HeaderText="Date" DataField="Date" SortExpression="Date" ReadOnly="True" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField HeaderText="IsWorkingDay" DataField="IsBizzDay" SortExpression="IsBizzDay" 
                        ReadOnly="True">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblRateQuantity" runat="server"
                            Text='<%# (DataBinder.Eval(Container.DataItem, "Price") != DBNull.Value && (Decimal)DataBinder.Eval(Container.DataItem, "Price") != 0 ? DataBinder.Eval(Container.DataItem, "Price", "{0:##0.####}") : "") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <db:DecimalBox ID="dbNewQuantity" runat="server" 
                            DecimalPlaces="6"
                            Text='<%# (DataBinder.Eval(Container.DataItem, "Price") != DBNull.Value && (Decimal)DataBinder.Eval(Container.DataItem, "Price") != 0 ? DataBinder.Eval(Container.DataItem, "Price", "{0:##0.####}")  : "") %>'
                             />
                    </EditItemTemplate>
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True">
                    <ItemStyle Wrap="False" Width="100px"/>
                </asp:CommandField>
            </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsMissingPrices" runat="server" SelectMethod="GetHistoricalMissingPriceDates" UpdateMethod="UpdateHistoricalPrice"  
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentHistoricalPricesAdapter" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvInstrumentPrices" Name="instrumentId" PropertyName="SelectedValue" Type="Int32" />
                    <asp:ControlParameter ControlID="cldpDate" Name="date" PropertyName="SelectedDate" Type="DateTime" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="gvInstrumentPrices" Name="original_InstrumentId" PropertyName="SelectedValue" Type="Int32" />
                </UpdateParameters>
            </asp:ObjectDataSource>    
        </asp:View>
        
        <asp:View ID="vweCurrencyRates" runat="server">
            <asp:GridView ID="gvCurrencyRates" runat="server" AutoGenerateColumns="False" Caption="Currency Rates" CaptionAlign="Left" PageSize="20" 
                DataSourceID="odsCurrencyRates" AllowPaging="True" AllowSorting="True" Visible="true" DataKeyNames="InstrumentId" 
                OnRowCommand="gvCurrencyRates_RowCommand"
                OnRowUpdating="gvSender_RowUpdating"  
                OnRowUpdated="gvSender_RowUpdated" >                
            <Columns>
                <asp:BoundField HeaderText="Symbol" DataField="Symbol" SortExpression="Symbol" ReadOnly="True">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Country" DataField="CountryName" SortExpression="CountryName" 
                        ReadOnly="True">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField HeaderText="AltSymbol" DataField="AltSymbol" SortExpression="AltSymbol" ReadOnly="True">
                    <ItemStyle wrap="False" horizontalalign="Right" />
                    <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblRateQuantity" runat="server"
                            Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "Rate") != 0 ? DataBinder.Eval(Container.DataItem, "Rate", "{0:##0.####}") : "") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <db:DecimalBox ID="dbNewQuantity" runat="server" 
                            DecimalPlaces="6"
                            Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "Rate") != 0 ? DataBinder.Eval(Container.DataItem, "Rate", "{0:##0.####}")  : "") %>'
                             />
                    </EditItemTemplate>
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True">
                    <ItemStyle Wrap="False" Width="100px"/>
                </asp:CommandField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <asp:LinkButton 
                            runat="server" 
                            ID="lbtMissingRates" 
                            Text="Missing Rates"
                            CommandName="MissingRates"/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsCurrencyRates" runat="server" SelectMethod="GetHistoricalCurrencyRates" UpdateMethod="UpdateHistoricalExRate"
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.CurrencyHistoricalRatesAdapter" OldValuesParameterFormatString="original_{0}" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="cldpDate" Name="date" PropertyName="SelectedDate" 
                        Type="DateTime" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyName" PropertyName="Isin"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="activityFilter" PropertyName="ActivityFilter"
                        Type="Int32" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="cldpDate" Name="Date" PropertyName="SelectedDate" Type="DateTime" />
                </UpdateParameters>
            </asp:ObjectDataSource>        
            <br /> 
            <asp:GridView ID="gvMissingExRates" runat="server" AutoGenerateColumns="False" Caption="Missing Prices" CaptionAlign="Left" PageSize="20" 
                DataSourceID="odsMissingExRates" AllowPaging="True" AllowSorting="True" Visible="False" DataKeyNames="Date" 
                OnRowUpdating="gvSender_RowUpdating" 
                OnRowUpdated="gvSender_RowUpdated" >
            <Columns>
                <asp:BoundField HeaderText="Date" DataField="Date" SortExpression="Date" ReadOnly="True" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField HeaderText="IsWorkingDay" DataField="IsBizzDay" SortExpression="IsBizzDay" 
                        ReadOnly="True">
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblRateQuantity" runat="server"
                            Text='<%# (DataBinder.Eval(Container.DataItem, "Rate") != DBNull.Value && (Decimal)DataBinder.Eval(Container.DataItem, "Rate") != 0 ? DataBinder.Eval(Container.DataItem, "Rate", "{0:##0.####}") : "") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <db:DecimalBox ID="dbNewQuantity" runat="server" 
                            DecimalPlaces="6"
                            Text='<%# (DataBinder.Eval(Container.DataItem, "Rate") != DBNull.Value && (Decimal)DataBinder.Eval(Container.DataItem, "Rate") != 0 ? DataBinder.Eval(Container.DataItem, "Rate", "{0:##0.####}")  : "") %>'
                             />
                    </EditItemTemplate>
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True">
                    <ItemStyle Wrap="False" Width="100px"/>
                </asp:CommandField>
            </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsMissingExRates" runat="server" SelectMethod="GetHistoricalMissingExRates" UpdateMethod="UpdateHistoricalExRate"  
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.CurrencyHistoricalRatesAdapter" 
                >
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvCurrencyRates" Name="currencyId" PropertyName="SelectedValue" Type="Int32" />
                    <asp:ControlParameter ControlID="cldpDate" Name="date" PropertyName="SelectedDate" Type="DateTime" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="gvCurrencyRates" Name="original_InstrumentId" PropertyName="SelectedValue" Type="Int32" />
                </UpdateParameters>
            </asp:ObjectDataSource>    
        </asp:View>

        <asp:View ID="vweBenchMarks" runat="server">
            <asp:GridView ID="gvBenchMarks" runat="server" AutoGenerateColumns="False" Caption="Prices" CaptionAlign="Left" PageSize="20" 
                DataSourceID="odsBenchMarks" AllowPaging="True" AllowSorting="True" Visible="true" DataKeyNames="InstrumentId" 
                OnRowUpdating="gvSender_RowUpdating" 
                OnRowUpdated="gvSender_RowUpdated" >               
                <Columns>
                    <asp:BoundField HeaderText="ISIN" DataField="Isin" SortExpression="Isin" ReadOnly="True">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Instrument" DataField="InstrumentName" SortExpression="InstrumentName" 
                            ReadOnly="True">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Currency" DataField="Currency" SortExpression="Currency" ReadOnly="True">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Price" SortExpression="PriceQuantity">
                        <ItemTemplate>
                            <asp:Label ID="lblPriceQuantity" runat="server"
                                Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "PriceQuantity") != 0 ? DataBinder.Eval(Container.DataItem, "PriceQuantity", "{0:##0.####}") : "") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <db:DecimalBox ID="dbNewQuantity" runat="server"
                                DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "DecimalPlaces") %>'
                                Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "PriceQuantity") != 0 ? DataBinder.Eval(Container.DataItem, "PriceQuantity", "{0:##0.####}")  : "") %>'
                                 />
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="True">
                        <ItemStyle Wrap="False" Width="100px"/>
                    </asp:CommandField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsBenchMarks" runat="server" SelectMethod="GetHistoricalBenchMarks" UpdateMethod="UpdateHistoricalPrice"
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentHistoricalPricesAdapter" OldValuesParameterFormatString="original_{0}" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="cldpDate" Name="date" PropertyName="SelectedDate"
                        Type="DateTime" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="activityFilter" PropertyName="ActivityFilter"
                        Type="Int32" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="cldpDate" Name="date" PropertyName="SelectedDate" Type="DateTime" />
                </UpdateParameters>
            </asp:ObjectDataSource>        
        </asp:View>
    </asp:MultiView><br />
    
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:Panel ID="pnlDialog" runat="server" Visible="false" >
        <asp:RadioButtonList ID="rblDialog" runat="server" RepeatDirection="Horizontal" Width="120px" 
            AutoPostBack="true" OnSelectedIndexChanged="rblDialog_SelectedIndexChanged" >
            <asp:ListItem Text="Yes" Value="1" />
            <asp:ListItem Text="No" Value="0" />
        </asp:RadioButtonList>
    </asp:Panel>

    <br />
    <asp:Button ID="btnHideMissingPrices" runat="server" Text="Hide" OnClick="btnHideMissingPrices_Click" Visible="false" />
</asp:Content>


<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/EG.master" CodeFile="DerivativeOverview.aspx.cs"
    Inherits="DerivativeOverview" Theme="Neutral" Title="Derivatives Overview" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="true"
        SecCategoryFilter="Derivatives" />
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red" />
    <br />
    <asp:GridView ID="gvInstruments" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsInstruments" DataKeyNames="Key"
        Caption="Instruments" CaptionAlign="Left" PageSize="20" Visible="false" OnRowCommand="gvInstruments_RowCommand"
        OnDataBinding="gvInstruments_DataBinding">
        <Columns>
            <asp:TemplateField HeaderText="Instrument" SortExpression="Name">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel runat="server" CssClass="alignright" Width="30" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Category" SortExpression="SecCategory">
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <%# (SecCategories)DataBinder.Eval(Container.DataItem, "SecCategory")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ExchangeName" HeaderText="Exchange" SortExpression="ExchangeName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NominalCurrency" HeaderText="Currency" SortExpression="NominalCurrency">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Underlying" SortExpression="UnderlyingName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlUnderlying" runat="server" CssClass="alignright" Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "UnderlyingName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UnderlyingIsin" HeaderText="Underlying ISIN" SortExpression="UnderlyingIsin">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Symbol" HeaderText="Symbol" SortExpression="Symbol">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnSeries" runat="server" CommandName="ViewSeries" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SecCategory") %>'
                        Text="Series" />
                    <asp:LinkButton ID="lbtnEditInstrument" runat="server" CommandName="EditInstrument"
                        Text='<%# (UserHasEditRights ? "Edit" : "View") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsInstruments" runat="server" SelectMethod="GetDerivativeMasters"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
        <SelectParameters>
            <asp:Parameter Name="secCategoryFilter" Type="Int32" DefaultValue="2" />
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
    <br />
    <asp:Panel ID="pnlCreation" runat="server" Visible="false">
        <asp:Button ID="btnCreateDerivativeMaster" runat="server" Text="Create ..." OnClick="btnCreateDerivativeMaster_Click" />
        <asp:DropDownList ID="ddlSecCategory" runat="server" DataSourceID="odsSecCategory"
            DataTextField="Description" DataValueField="Key">
        </asp:DropDownList>
        <asp:ObjectDataSource ID="odsSecCategory" runat="server" SelectMethod="GetSecCategories"
            TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
            <SelectParameters>
                <asp:Parameter Name="secCategoryFilter" Type="Int32" DefaultValue="2" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <br />
    <asp:MultiView ID="mlvSeries" runat="server" ActiveViewIndex="0" Visible="false">
        <asp:View ID="vweOptions" runat="server">
            <asp:GridView ID="gvOptionSeries" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="odsOptionSeries" DataKeyNames="Key"
                Caption="Option Series" CaptionAlign="Left" PageSize="20" OnRowCommand="gvInstruments_RowCommand"
                OnDataBinding="gvInstruments_DataBinding">
                <Columns>
                    <asp:TemplateField HeaderText="Instrument" SortExpression="SortOrder">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                        <ItemTemplate>
                            <trunc:TruncLabel ID="trlOptionSerieName" runat="server" CssClass="alignright" Width="30"
                                Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type" SortExpression="OptionType">
                        <ItemStyle Wrap="False" />
                        <ItemTemplate>
                            <%# (OptionTypes)DataBinder.Eval(Container.DataItem, "OptionType")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ExpiryDate" HeaderText="Expiry" SortExpression="ExpiryDate"
                        DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                        <HeaderStyle Wrap="False" />
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="StrikePrice_ShortDisplayString" HeaderText="Strike" SortExpression="StrikePrice_Quantity">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnEditOptionSerie" runat="server" CommandName="EditOption"
                                Text='<%# (UserHasEditRights ? "Edit" : "View") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsOptionSeries" runat="server" SelectMethod="GetDerivativeSeries"
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvInstruments" Name="derivativeMasterId" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:Parameter DefaultValue="Key, Name, OptionType, StrikePrice.Quantity, StrikePrice.ShortDisplayString, ExpiryDate, SortOrder"
                        Name="propertyList" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
            <asp:Button ID="btnCreateOption" runat="server" Text="Create Option" OnClick="btnCreateOption_Click" />
        </asp:View>
        <asp:View ID="vweTurbos" runat="server">
            <asp:GridView ID="gvTurboSeries" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="odsTurboSeries" DataKeyNames="Key"
                Caption="Turbo Series" CaptionAlign="Left" PageSize="20" OnRowCommand="gvInstruments_RowCommand"
                OnDataBinding="gvInstruments_DataBinding">
                <Columns>
                    <asp:TemplateField HeaderText="Instrument" SortExpression="Name">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                        <ItemTemplate>
                            <trunc:TruncLabel ID="trlTurboSerieName" runat="server" CssClass="alignright" Width="30"
                                Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sign" SortExpression="Sign">
                        <ItemStyle Wrap="False" />
                        <ItemTemplate>
                            <%# (B4F.TotalGiro.Accounts.Portfolios.IsLong)DataBinder.Eval(Container.DataItem, "Sign")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StopLoss_ShortDisplayString" HeaderText="StopLoss" SortExpression="StopLoss_Quantity">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Leverage" HeaderText="Leverage" SortExpression="Leverage">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="FinanceLevel" HeaderText="Finance Level" SortExpression="FinanceLevel">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayRatio" HeaderText="Ratio" SortExpression="DisplayRatio">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnEditTurboSerie" runat="server" CommandName="EditTurbo" Text='<%# (UserHasEditRights ? "Edit" : "View") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <SelectedRowStyle BackColor="Gainsboro" />
            </asp:GridView>
            <asp:ObjectDataSource ID="odsTurboSeries" runat="server" SelectMethod="GetDerivativeSeries"
                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvInstruments" Name="derivativeMasterId" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:Parameter DefaultValue="Key, Name, Sign, StopLoss.Quantity, StopLoss.ShortDisplayString, DisplayRatio, Leverage, FinanceLevel"
                        Name="propertyList" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
            <asp:Button ID="btnCreateTurbo" runat="server" Text="Create Turbo" OnClick="btnCreateTurbo_Click" />
        </asp:View>
    </asp:MultiView>
</asp:Content>

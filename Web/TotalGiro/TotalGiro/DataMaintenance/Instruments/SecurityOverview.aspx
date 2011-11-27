<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/EG.master" CodeFile="SecurityOverview.aspx.cs"
    Inherits="SecurityOverview" Theme="Neutral" Title="Securities Overview" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="true" ShowActivityFilter="true" SecCategoryFilter="Securities" />
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red" />
    <br />
    <br />
    <asp:GridView ID="gvInstruments" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsInstruments" 
        DataKeyNames="Key" Caption="Instruments"
        CaptionAlign="Left" PageSize="20" 
        Visible="false"
        OnRowCommand="gvInstruments_RowCommand"
        OnDataBinding="gvInstruments_DataBinding">
        <Columns>
            <asp:TemplateField HeaderText="Instrument" SortExpression="DisplayName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlDisplayName" 
                        runat="server"
                        cssclass="alignright"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "DisplayName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayIsin" HeaderText="ISIN" SortExpression="DisplayIsin" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="SecCategory_Name" HeaderText="Category" SortExpression="SecCategory_Name" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CurrencyNominal_DisplayName" HeaderText="Currency" SortExpression="CurrencyNominal_DisplayName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DefaultExchangeName" HeaderText="Default Exchange" SortExpression="DefaultExchangeName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Date InActive" SortExpression="InActiveDate">
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "InActiveDate") != DateTime.MinValue ?
                                             ((DateTime)DataBinder.Eval(Container.DataItem, "InActiveDate")).ToString("d MMMM yyyy") : "")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnEditInstrument" runat="server" 
                        CommandName="EditInstrument" 
                        Text='<%# (UserHasEditRights ? "Edit" : "View") %>' />
                    <asp:LinkButton ID="lbtViewPrices" runat="server" 
                        CommandName="ViewPrices" 
                        Text="View Prices" />
                    <asp:LinkButton ID="lbtnShowCorporateAction" runat="server" 
                        CommandName="ShowStockDiv"
                        Text="Stock Div"
                        Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "CorporateActionInstruments_Count") > 0) %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsInstruments" runat="server" SelectMethod="GetTradeableInstruments"
        TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
        <SelectParameters>
            <asp:Parameter Name="secCategoryFilter" Type="Int32" DefaultValue="1" />
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
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="activityFilter" PropertyName="ActivityFilter"
                Type="Int32" />
            <asp:Parameter DefaultValue="Key, DisplayIsin, DisplayName, SecCategory.Name, DefaultExchangeName, CurrencyNominal.DisplayName, InActiveDate, IsActive, IsSecurity, CorporateActionInstruments.Count" Name="propertyList" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Panel ID="pnlCreation" runat="server" Visible="false" >
        <asp:Button ID="btnCreate" runat="server" Text="Create ..." OnClick="btnCreate_Click" />
        <asp:DropDownList ID="ddlSecCategory" runat="server" DataSourceID="odsSecCategory" 
            DataTextField="Description" DataValueField="Key">
        </asp:DropDownList>
        <asp:ObjectDataSource ID="odsSecCategory" runat="server" SelectMethod="GetSecCategories"
            TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter">
            <SelectParameters>
                <asp:Parameter Name="secCategoryFilter" Type="Int32" DefaultValue="1" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    
    <br/>
    <br/>
    <asp:GridView ID="gvStockDividendInstruments" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsStockDividendInstruments" 
        DataKeyNames="Key" Caption="Instruments"
        CaptionAlign="Left" PageSize="20" 
        OnRowUpdated="gvStockDividendInstruments_RowUpdated"
        OnDataBinding="gvInstruments_DataBinding"
        Visible="false" >
        <Columns>
            <asp:BoundField DataField="DisplayName" HeaderText="Name" SortExpression="DisplayName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsin" HeaderText="ISIN" SortExpression="DisplayIsin" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="SecCategory_Name" HeaderText="Category" SortExpression="SecCategory_Name" ReadOnly="true" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Created" SortExpression="CreationDate">
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("d MMMM yyyy") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="true" />
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsStockDividendInstruments" runat="server" SelectMethod="GetStockDividend" UpdateMethod="UpdateStockDividend"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentEditAdapter" OldValuesParameterFormatString="original_{0}" >
        <SelectParameters>
            <asp:ControlParameter ControlID="gvInstruments" Name="instrumentId" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:Parameter DefaultValue="Key, DisplayIsin, DisplayName, SecCategory.Name, CreationDate" Name="propertyList" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
</asp:Content>

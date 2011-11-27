<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="AssetManagerInstrumentSelection.aspx.cs" Inherits="AssetManagerInstrumentSelection" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="smScriptManager" runat="server" />
    <asp:Panel ID="pnlAssetManager" runat="server" Visible="false" >
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblAssetManagerLabel" runat="server" Text="Asset Manager:"></asp:Label>
            </td>
            <td style="width: 190px">
                <asp:MultiView ID="mvwAssetManager" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwAssetManager" runat="server">
                        <asp:Label ID="lblAssetManager" runat="server" Font-Bold="True"></asp:Label></asp:View>
                    <asp:View ID="vwStichting" runat="server">
                        <asp:DropDownList ID="ddlAssetManager" runat="server" DataSourceID="odsAssetManager"
                            DataTextField="CompanyName" DataValueField="Key" AutoPostBack="False">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                            TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </table>
    </asp:Panel>
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="false" />
    <cc1:MultipleSelectionGridView 
        ID="gvUnMappedInstruments" 
        runat="server" 
        AutoGenerateColumns="False"
        DataSourceID="odsUnMappedInstruments"
        DataKeyNames="Key"
        cssclass="padding"
        CellPadding="0"
        AllowPaging="True"
        Caption="Instruments not mapped" 
        CaptionAlign="Left" 
        PageSize="10" 
        AllowSorting="True">
        <Columns>
            <asp:TemplateField HeaderText="Instrument" SortExpression="DisplayName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
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
            <asp:BoundField DataField="CurrencyNominal_Name" HeaderText="Currency" SortExpression="CurrencyNominal_Name" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="HomeExchange_ExchangeName" HeaderText="HomeExchange" SortExpression="HomeExchange_ExchangeName" >
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
       </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsUnMappedInstruments" runat="server" SelectMethod="GetUnMappedInstruments"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments.AssetManagerInstrumentSelectionAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                Type="Object" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:Button ID="btnMapInstruments" runat="server" OnClick="btnMapInstruments_Click" Text="Map" Width="90px" />
    <br />
    <br />
    <asp:RadioButtonList ID="rblActivityFilter" runat="server" RepeatDirection="Horizontal" Height="22px" Width="140px"
        OnSelectedIndexChanged="rblActivityFilter_SelectedIndexChanged" AutoPostBack="true" >
        <asp:ListItem Value="0">All</asp:ListItem>
        <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
        <asp:ListItem Value="2">Inactive</asp:ListItem>
    </asp:RadioButtonList>
    <asp:GridView 
        ID="gvMappedInstruments"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        Caption="Mapped Instruments"
        CaptionAlign="Left"
        DataSourceID="odsMappedInstruments" 
        AllowSorting="True"
        OnRowUpdating="gvMappedInstruments_RowUpdating">
        <Columns>
            <asp:TemplateField HeaderText="Instrument" SortExpression="Instrument_DisplayName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlMappedInstrumentName" 
                        runat="server"
                        cssclass="alignright"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Instrument_DisplayName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Instrument_DisplayIsin" HeaderText="ISIN" SortExpression="Instrument_DisplayIsin" ReadOnly="true" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Asset" SortExpression="AssetClass_AssetName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlAssetClass" 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "AssetClass_AssetName") %>' 
                        />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlAssetClass" runat="server" SkinID="custom-width" Width="80px"
                        DataSourceID="odsAssetClass" DataTextField="AssetName" DataValueField="Key"
                        SelectedValue='<%# (DataBinder.Eval(Container.DataItem, "AssetClass_Key") == System.DBNull.Value ? int.MinValue : DataBinder.Eval(Container.DataItem, "AssetClass_Key")) %>' >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsAssetClass" runat="server" SelectMethod="GetAssetClasses"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments.AssetManagerInstrumentSelectionAdapter">
                    </asp:ObjectDataSource>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Region" SortExpression="RegionClass_RegionName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlRegionClass" 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "RegionClass_RegionName") %>' 
                        />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlRegionClass" runat="server" SkinID="custom-width" Width="100px"
                        DataSourceID="odsRegionClass" DataTextField="RegionName" DataValueField="Key"
                        SelectedValue='<%# (DataBinder.Eval(Container.DataItem, "RegionClass_Key") == System.DBNull.Value ? int.MinValue : DataBinder.Eval(Container.DataItem, "RegionClass_Key")) %>' >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsRegionClass" runat="server" SelectMethod="GetRegionClasses"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments.AssetManagerInstrumentSelectionAdapter">
                    </asp:ObjectDataSource>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Category" SortExpression="InstrumentsCategories_InstrumentsCategoryName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlInstrumentsCategories" 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "InstrumentsCategories_InstrumentsCategoryName") %>' 
                        />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlInstrumentsCategories" runat="server" SkinID="custom-width" Width="160px"
                        DataSourceID="odsInstrumentsCategories" DataTextField="InstrumentsCategoryName" DataValueField="Key"
                        SelectedValue='<%# (DataBinder.Eval(Container.DataItem, "InstrumentsCategories_Key") == System.DBNull.Value ? int.MinValue : DataBinder.Eval(Container.DataItem, "InstrumentsCategories_Key")) %>' >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsInstrumentsCategories" runat="server" SelectMethod="GetInstrumentsCategories"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments.AssetManagerInstrumentSelectionAdapter">
                    </asp:ObjectDataSource>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Sector" SortExpression="SectorClass_SectorName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlSector" 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "SectorClass_SectorName") %>' 
                        />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlSectorClass" runat="server" SkinID="custom-width" Width="130px"
                        DataSourceID="odsSectorClass" DataTextField="SectorName" DataValueField="Key" 
                        SelectedValue='<%# (DataBinder.Eval(Container.DataItem, "SectorClass_Key") == System.DBNull.Value ? int.MinValue : DataBinder.Eval(Container.DataItem, "SectorClass_Key"))  %>' >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsSectorClass" runat="server" SelectMethod="GetSectorClasses"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments.AssetManagerInstrumentSelectionAdapter">
                    </asp:ObjectDataSource>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="MaxWithdrawal%" SortExpression="MaxWithdrawalAmountPercentage" >
                <HeaderStyle cssclass="alignright" wrap="False" />
                <ItemStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlMaxWithdrawalAmountPercentage" 
                        runat="server"
                        cssclass="alignright"
                        Width="5"
                        Text='<%# DataBinder.Eval(Container.DataItem, "MaxWithdrawalAmountPercentage") %>' 
                        />
                </ItemTemplate>
                <EditItemTemplate>
                    <db:DecimalBox ID="dbMaxWithdrawalAmountPercentage" runat="server" Width="50px"
                        Value='<%# (decimal)(short)DataBinder.Eval(Container.DataItem, "MaxWithdrawalAmountPercentage") %>' 
                     />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField HeaderText="Active" SortExpression="IsActive" DataField="IsActive" >
                <HeaderStyle wrap="False" />
            </asp:CheckBoxField>
            <asp:CommandField SelectText="Edit" ShowEditButton="true" >
                <ItemStyle wrap="False" />
            </asp:CommandField>
            <asp:TemplateField>
	            <ItemTemplate>
		            <asp:LinkButton ID="lbtnDelete" runat="server" CommandName="Delete" Text="Delete"
			            OnClientClick="return confirm('Are you sure you want to delete this instrument?');" />
	            </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsMappedInstruments" runat="server" SelectMethod="GetMappedInstruments"
        DeleteMethod="DeleteInstrumentMapping" UpdateMethod="EditInstrumentMapping"
        OldValuesParameterFormatString="original_{0}"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments.AssetManagerInstrumentSelectionAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="rblActivityFilter" DefaultValue="0" Name="activityFilter"
                PropertyName="SelectedIndex" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


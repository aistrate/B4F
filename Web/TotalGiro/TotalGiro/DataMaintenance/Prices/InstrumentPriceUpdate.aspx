<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="InstrumentPriceUpdate.aspx.cs" Inherits="InstrumentPriceUpdate" Title="Instrument Price Update" Theme="Neutral" %>
<%@ Register Src="~/UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="ucCalendar" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <style type="text/css">
        .ajax__tab_xp .ajax__tab_tab { width: 180px; }
    </style>

    <asp:RadioButtonList ID="rblDataSourceChoice" runat="server" DataSourceID="odsDataSourceChoices" DataTextField="InstrumentType" DataValueField="ID" Width="320px"
        OnSelectedIndexChanged="rblDataSourceChoice_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" />
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" SecCategoryFilter="Securities" ShowExchange="false" ShowSecCategory="false" ShowActivityFilter="true" />
    <br />
    <asp:ObjectDataSource ID="odsDataSourceChoices" runat="server" SelectMethod="GetDataSourceChoices" 
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter">
    </asp:ObjectDataSource>
    <asp:Panel ID="pnlSelectedInstrument" runat="server" Visible="False" Width="700px">
        <hr style="position: relative; top: -20px; width: 778px;" />
        <table style="width: 480px; position: relative; top: -10px">
            <tr>
                <td style="width: 120px; height: 24px">
                    <asp:Label ID="Label1" runat="server" Text="Sel. Instrument:"></asp:Label></td>
                <td style="width: 315px; height: 24px">
                    <asp:DropDownList ID="ddlSelectedInstrument" SkinID="custom-width" runat="server" Width="300px" DataSourceID="odsSelectedInstrument" DataTextField="Description" 
                        DataValueField="Key" OnSelectedIndexChanged="ddlSelectedInstrument_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <table style="width: 555px; position: relative; top: -10px">
            <tr>
                <td style="width: 120px; height: 24px">
                    <asp:Label ID="lblDateFrom" runat="server" Text="Date from" />
                </td>
                <td style="width: 200px;">
                    <ucCalendar:Calendar ID="cldDateFrom" runat="server" Format="dd-MM-yyyy" AutoPostBack="true" />
                </td>
                <td style="width: 60px;">
                    <asp:Label ID="lblDateTo" runat="server" Text="Date to" />
                </td>
                <td style="width: 100px;">
                    <ucCalendar:Calendar ID="cldDateTo" runat="server" Format="dd-MM-yyyy" AutoPostBack="true" />
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="odsSelectedInstrument" runat="server" SelectMethod="GetFilteredInstruments"
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentPriceUpdateAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="rblDataSourceChoice" Name="secCatChoice" PropertyName="SelectedIndex"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="activityFilter" PropertyName="ActivityFilter"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <br />
    <ajaxToolkit:TabContainer runat="server" ID="tbPriceHistory" AutoPostBack="true" ActiveTabIndex="0" 
        Width="860px" CssClass="ajax__tab_xp" Visible="false"
        OnActiveTabChanged="tbPriceHistory_ActiveTabChanged"  >
    
        <ajaxToolkit:TabPanel ID="pnlPriceHistory" runat="server" HeaderText="Price Update Tool"  >
            <ContentTemplate>
                <b4f:MultipleSelectionGridView ID="gvPriceHistory" runat="server" AutoGenerateColumns="False" 
                    Caption="History" CaptionAlign="Left" PageSize="15"
                    DataSourceID="odsPriceHistory" AllowPaging="True" AllowSorting="True" Visible="true" 
                    DataKeyNames="DateKey,Key" 
                    SkinID="custom-width" Width="650px"
                    OnRowEditing="gvPriceHistory_RowEditing"
                    OnRowUpdating="gvPriceHistory_RowUpdating" 
                    OnRowCancelingEdit="gvPriceHistory_RowCancelingEdit"
                    OnRowUpdated="gvPriceHistory_RowUpdated"
                    OnRowDataBound="gvPriceHistory_RowDataBound"
                    OnDataBound="gvPriceHistory_DataBound" >
                    <Columns>
                        <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" ReadOnly="true" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" >
                            <ItemStyle wrap="False" />
                            <HeaderStyle wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" ReadOnly="true" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" >
                            <ItemStyle wrap="False" />
                            <HeaderStyle wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Currency" DataField="Currency" SortExpression="Currency" ReadOnly="True">
                            <ItemStyle wrap="False" horizontalalign="Right" />
                            <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Price" SortExpression="PriceQuantity">
                            <ItemTemplate>
                                <asp:Label ID="lblPriceQuantity" runat="server"
                                    Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "PriceQuantity") != 0 ? DataBinder.Eval(Container.DataItem, "PriceQuantity", "{0:##0.0000##}") : "") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <db:DecimalBox ID="dbNewQuantity" runat="server" 
                                    DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "DecimalPlaces") %>'
                                    Text='<%# ((Decimal)DataBinder.Eval(Container.DataItem, "PriceQuantity") != 0 ? DataBinder.Eval(Container.DataItem, "PriceQuantity", "{0:##0.0000##}") : "") %>'
                                     />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" CausesValidation="false">
                            <ItemStyle Wrap="False" Width="100px"/>
                        </asp:CommandField>
                    </Columns>
                </b4f:MultipleSelectionGridView>
                <br />
                <asp:Button ID="btnUpdatePriceRange" runat="server" Text="Update Price Range" OnClick="btnUpdatePriceRange_Click" />
                <asp:Label ID="lblNewPrice" runat="server" Text="New Price" />
                <db:DecimalBox ID="dbNewPrice" runat="server" DecimalPlaces="5" />
                <asp:Label ID="lblNewPriceCurrencyLabel" runat="server" Text="€" />
                <asp:RequiredFieldValidator ID="rvCheckUpdatePriceRange" runat="server" ControlToValidate="dbNewPrice:tbDecimal"
                    SetFocusOnError="true" ErrorMessage="There is no Price entered" >*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvCheckUpdatePriceRange" runat="server" OnServerValidate="cvCheckUpdatePriceRange_ServerValidate"
                    SetFocusOnError="true" ErrorMessage="There are no dates selected" >*</asp:CustomValidator>
                <asp:ObjectDataSource ID="odsPriceHistory" runat="server" SelectMethod="GetInstrumentPriceHistory" UpdateMethod="UpdateInstrumentPriceHistory"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentPriceUpdateAdapter" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlSelectedInstrument" Name="instrumentId" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cldDateFrom" Name="startDate" PropertyName="SelectedDate" Type="DateTime" />
                        <asp:ControlParameter ControlID="cldDateTo" Name="endDate" PropertyName="SelectedDate" Type="DateTime" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:ControlParameter ControlID="gvPriceHistory" DefaultValue="0" Name="DateKey"
                            PropertyName="SelectedDataKey.Values[0]" Type="Int32" />
                        <asp:ControlParameter ControlID="gvPriceHistory" DefaultValue="0" Name="Key"
                            PropertyName="SelectedDataKey.Values[1]" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlSelectedInstrument" Name="instrumentId" PropertyName="SelectedValue" Type="Int32" />
                    </UpdateParameters>
                </asp:ObjectDataSource>   
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="pnlGraph" runat="server" HeaderText="Graph" >
            <ContentTemplate>
                <dundas:Chart ID="chHistoryGraph" runat="server" BorderLineColor="26, 59, 105"
                    Palette="DundasDark" Width="850px" Height="433px" AntiAliasing="Graphics" 
                    BorderLineWidth="0" EnableViewState="true">
                    <BorderSkin FrameBackColor="CornflowerBlue" FrameBackGradientEndColor="CornflowerBlue"
                        FrameBorderColor="100, 0, 0, 0" FrameBorderWidth="2" PageColor="Window" />
                    <ChartAreas>
                        <dundas:ChartArea BorderColor="26, 59, 105" BackColor="White" BorderStyle="Solid" ShadowOffset="2"
                            Name="Default" >
                            <AxisX Margin="False" LabelsAutoFitMaxFontSize="9" LabelsAutoFitMinFontSize="7">
                                <MinorGrid LineColor="Silver" />
                                <LabelStyle Format="MMM yy" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                            Font="Trebuchet MS, 12px" FontColor="#313457" IntervalType="Auto" />
                                <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                    LineColor="Silver" />
                                <MinorTickMark Size="2" />
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisX>
                            <Area3DStyle Light="Realistic" RightAngleAxes="False" WallWidth="10" />
                            <AxisY LabelsAutoFitMinFontSize="7" LabelsAutoFitMaxFontSize="9">
                                <MinorGrid LineColor="Silver" />
                                <LabelStyle Format="C" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                            Font="Trebuchet MS, 12px" FontColor="#313457" IntervalType="Auto" />
                                <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                    LineColor="Silver" />
                                <MinorTickMark Size="2" />
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisY>
                            <AxisX2>
                                <MinorGrid LineColor="Silver" />
                                <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                    LineColor="Silver" />
                                <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisX2>
                            <AxisY2>
                                <MinorGrid LineColor="Silver" />
                                <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                    LineColor="Silver" />
                                <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisY2>
                            <Position Height="85" Width="94" X="3" Y="10" />
                        </dundas:ChartArea>
                    </ChartAreas>
                </dundas:Chart>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxtoolkit:TabContainer>
    <br />
    <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
        ID="valSum" runat="server" />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <asp:RadioButtonList ID="rblIgnoreWarning" runat="server" RepeatDirection="Horizontal"
        Visible="False" Width="158px" 
        AutoPostBack="true" OnSelectedIndexChanged="rblIgnoreWarning_SelectedIndexChanged" >
            <asp:ListItem Text="Yes" Value="1" />
            <asp:ListItem Text="No" Value="0" />
    </asp:RadioButtonList>
    
</asp:Content>


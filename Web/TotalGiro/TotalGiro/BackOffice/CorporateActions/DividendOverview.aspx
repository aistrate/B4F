<%@ Page Title="Dividend Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="DividendOverview.aspx.cs" Inherits="DividendOverview" %>

<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments.CorporateAction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" SecCategoryFilter="Securities" ShowExchange="false" ShowSecCategory="true" ShowActivityFilter="true" />
    <br/>
    <asp:Panel ID="pnlSelectedInstrument" runat="server" Visible="False" Width="700px">
        <hr style="position: relative; top: -20px; width: 778px;" />
        <table style="width: 480px; position: relative; top: -10px">
            <tr>
                <td style="width: 120px; height: 24px">
                    <asp:Label ID="Label3" runat="server" Text="Sel. Instrument:"></asp:Label></td>
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
                    <uc1:Calendar ID="cldDateFrom" runat="server" Format="dd-MM-yyyy" AutoPostBack="true" />
                </td>
                <td style="width: 60px;">
                    <asp:Label ID="lblDateTo" runat="server" Text="Date to" />
                </td>
                <td style="width: 100px;">
                    <uc1:Calendar ID="cldDateTo" runat="server" Format="dd-MM-yyyy" AutoPostBack="true" />
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="odsSelectedInstrument" runat="server" SelectMethod="GetFilteredInstruments"
            TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.DividendAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="activityFilter" PropertyName="ActivityFilter"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>



    <br />
    <br />
    <asp:GridView ID="gvDividends" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsDividends" AutoGenerateColumns="False" Caption="Dividends" CaptionAlign="Left"
        DataKeyNames="Key" PageSize="20" Visible="false" >
        <Columns>
            <asp:BoundField DataField="InstrumentName" HeaderText="Instrument" SortExpression="InstrumentName">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ex-Dividend Date" SortExpression="ExDividendDate">
                <ItemTemplate>
                    <asp:Label ID="lblTransferDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "ExDividendDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "ExDividendDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Settlement Date" SortExpression="SettlementDate">
                <ItemTemplate>
                    <asp:Label ID="lblSettlementDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "SettlementDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "SettlementDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Type" SortExpression="DividendType">
                <ItemTemplate>
                    <%# (DividendTypes)DataBinder.Eval(Container.DataItem, "DividendType")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="UnitPriceDisplay" HeaderText="Unit Price" SortExpression="UnitPriceDisplay">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="DisplayStatus">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDetails" runat="server" CausesValidation="False" Text="Details"
                        CommandName="ViewDetails" ToolTip="View Details" OnCommand="lbtDetails_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsDividends" runat="server" SelectMethod="GetDividendHistories"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.DividendAdapter"
        OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlSelectedInstrument" Name="instrumentKey"
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cldDateFrom" Name="startDate" PropertyName="SelectedDate"
                Type="DateTime" />
            <asp:ControlParameter ControlID="cldDateTo" Name="endDate" PropertyName="SelectedDate"
                Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
<%--    <asp:Table ID="Table2" runat="server" Width="1000px" CellSpacing="0" BorderStyle="Solid" BorderWidth="1px" BorderColor="Black"
        Caption="New Dividend">
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell3" runat="server">
                <asp:Label ID="lblChooseInstrument" runat="server" Text="Choose Instrument:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server">
                <asp:DropDownList ID="ddlInstrumentOfPosition" runat="server" AutoPostBack="true"
                    DataSourceID="odsInstrumentOfPosition" SkinID="custom-width" Width="275" DataTextField="Description"
                    DataValueField="Key" OnSelectedIndexChanged="ddlInstrumentOfPosition_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrumentOfPosition" runat="server" SelectMethod="GetTradeableInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.DividendAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvInstrumentOfPosition" runat="server" 
                    ControlToValidate="ddlInstrumentOfPosition" SetFocusOnError="True" InitialValue="-2147483648"
                    ErrorMessage="Instrument is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblChooseExDate" runat="server" Text="Choose Ex-Dividend Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <uc1:Calendar ID="dpExDividendDate" runat="server" IsButtonDeleteVisible="false" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvExDividendDate"
                    controlToValidate="dpExDividendDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="ExDividend Date is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableCell ID="TableCell5" runat="server">
                <asp:Label ID="Label1" runat="server" Text="Choose Payment Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server">
                <uc1:Calendar ID="dpPaymentDate" runat="server" IsButtonDeleteVisible="false" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server">
                <asp:RequiredFieldValidator ID="rfvPaymentDate"
                    controlToValidate="dpPaymentDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Payment Date is Mandatory" />
                <asp:CompareValidator 
                    ID="cvPaymentDate" 
                    runat="server"
                    ControlToValidate="dpPaymentDate:txtCalendar"
                    ControlToCompare="dpExDividendDate:txtCalendar"
                    Operator="GreaterThanEqual"
                    Type="Date"
                    Text="*" 
                    ErrorMessage="The payment date can not be before the ex-dividend date"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableCell ID="TableCell7" runat="server">
                <asp:Label ID="Label2" runat="server" Text="Price:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server">
                <db:DecimalBox ID="dbPriceQuantity" DecimalPlaces="4" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" />
                <asp:Label ID="lblPriceCurrency" runat="server" Text="EUR"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server">
                <asp:RequiredFieldValidator ID="rfvPriceQuantity"
                    controlToValidate="dbPriceQuantity:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Price is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>
                <asp:TableRow  runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="lblExternalDescription" runat="server" Text="External Description:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:TextBox ID="txtExternalDescription" runat="server" SkinID="custom-width" MaxLength="50" Text=""
                Width="200" />
            </asp:TableCell>
            <asp:TableCell  runat="server">
                
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
--%>    
        <asp:Table ID="Table1" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell1" runat="server">
                <asp:Button ID="btnNewDividend" runat="server" Text="Create New Dividend" 
                    OnClick="btnNewDividend_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell runat="server">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

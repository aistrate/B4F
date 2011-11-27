<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="BondCouponAccrual.aspx.cs" Inherits="BondCouponAccrual" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.GeneralLedger.Journal.Bookings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="smScriptManager" runat="server" />
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" ShowExchange="false" ShowSecCategory="false" SecCategoryFilter="Securities" />
    <table>
        <tr>
            <td width="0.15px"> </td>
            <td width="128px">
                <asp:Label ID="lblDateFrom" runat="server" Text="From" />
            </td>
            <td width="195px">
                <uc2:DatePicker ID="dtpDateFrom" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblDateTo" runat="server" Text="To" />
            </td>
            <td width="195px">
                <uc2:DatePicker ID="dtpDateTo" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btnCalculate" runat="server" Text="Calculate Coupons" Style="float: left; position: relative; top: -2px" Width="150px" 
        OnClick="btnCalculate_Click" />
    <br />
    <br />
    <asp:GridView 
        ID="gvCoupons" 
        runat="server" 
        AutoGenerateColumns="False"
        DataSourceID="odsCoupons"
        DataKeyNames="Key"
        cssclass="padding"
        CellPadding="0"
        AllowPaging="True"
        Caption="Coupons" 
        CaptionAlign="Left" 
        PageSize="10" 
        AllowSorting="True"
        Visible="false"
        onselectedindexchanged="gvCoupons_SelectedIndexChanged">
        <Columns>
            <asp:TemplateField HeaderText="Instrument" SortExpression="InstrumentName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        runat="server"
                        cssclass="alignright"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "InstrumentName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ISIN" HeaderText="ISIN" SortExpression="ISIN" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:CommandField ShowSelectButton="True" SelectText="Details" >
                <ItemStyle Wrap="False" Width="82px" />
            </asp:CommandField>
       </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCoupons" runat="server" SelectMethod="GetCoupons"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.BondCouponAccrualAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                Type="Int32" />
            <asp:ControlParameter ControlID="dtpdateFrom" Name="dateFrom" PropertyName="SelectedDate" 
                Type="DateTime" />
            <asp:ControlParameter ControlID="dtpdateTo" Name="dateTo" PropertyName="SelectedDate" 
                Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:GridView 
        ID="gvCouponPaymentDetails"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="10" 
        AutoGenerateColumns="False"
        DataKeyNames="Key,PositionID"
        Caption="Coupon Details"
        CaptionAlign="Left"
        DataSourceID="odsCouponPaymentDetails" 
        AllowSorting="True"
        Visible="false"
        onselectedindexchanged="gvCouponPaymentDetails_SelectedIndexChanged" >
        <Columns>
            <asp:BoundField DataField="AccountNumber" HeaderText="AccountNumber" SortExpression="AccountNumber" ReadOnly="true" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="AccountName" SortExpression="AccountName" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlAccountName" 
                        runat="server"
                        cssclass="alignright"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "AccountName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PositionSize" HeaderText="Position Size" SortExpression="PositionSize" ReadOnly="true" >
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="AccruedAmount" HeaderText="Accrued Amount" SortExpression="AccruedAmount" ReadOnly="true" >
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Status" SortExpression="Status" >
                <ItemTemplate>
                    <%# (BondCouponPaymentStati)DataBinder.Eval(Container.DataItem, "Status") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Calculations_Count" HeaderText="#Calc" SortExpression="Calculations_Count" ReadOnly="true" >
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="LastCalcDate" HeaderText="Last CalcDate" SortExpression="LastCalcDate" ReadOnly="true" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:CommandField ShowSelectButton="True" SelectText="Details" >
                <ItemStyle Wrap="False" Width="82px" />
            </asp:CommandField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCouponPaymentDetails" runat="server" SelectMethod="GetCouponPaymentDetails"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.BondCouponAccrualAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvCoupons" DefaultValue="0" Name="couponId"
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <br />
    <asp:GridView 
        ID="gvCalculations"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        Caption="Coupon Calculations"
        CaptionAlign="Left"
        DataSourceID="odsCalculations" 
        AllowSorting="True"
        Visible="false" >
        <Columns>
            <asp:BoundField DataField="CalcDate" HeaderText="Calc Date" SortExpression="CalcDate" ReadOnly="true" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="SettlementDate" HeaderText="Settlement Date" SortExpression="SettlementDate" ReadOnly="true" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="PositionSize" HeaderText="Position Size" SortExpression="PositionSize" ReadOnly="true" >
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="CalculatedAccruedInterestUpToDate" HeaderText="Interest UpToDate" SortExpression="CalculatedAccruedInterestUpToDate" ReadOnly="true" >
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="DailyInterest" HeaderText="Daily Interest" SortExpression="DailyInterest" ReadOnly="true" >
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCalculations" runat="server" SelectMethod="GetCouponPaymentCalculations"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.BondCouponAccrualAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvCoupons" DefaultValue="0" Name="couponId"
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="gvCouponPaymentDetails" DefaultValue="0" Name="positionId"
                PropertyName="SelectedDataKey.Values[1]" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


<%@ Page Title="Manual Settlement Matching" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="ManualSettlementMatching.aspx.cs" Inherits="ManualSettlementMatching" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.GeneralLedger.Journal" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">

    <script type="text/javascript">
        function ConfirmSettlement() {
            settlementDiff = document.getElementById('ctl00_bodyContentPlaceHolder_hdfSettlementDifference').value
            if (settlementDiff != 0)
                return confirm('There is a settlement difference. Are you sure?');
        }
    </script>

    <table border="0">
        <tr>
            <td align="left">
                Show data from this date:<br />
                <uc1:DatePicker ID="dpStartDate" runat="server" OnSelectionChanged="dtpStart_SelectionChanged" />
            </td>
            <td align="left">
                Until(incl.) this date:<br />
                <uc1:DatePicker ID="dpEndDate" runat="server" OnSelectionChanged="dtpEnd_SelectionChanged" />
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfSettlementDifference" runat="server" />
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <asp:Button ID="btnPlace" runat="server" OnClick="btnPlace_OnClick" Text="Place on Selection Grid" />
    <br />
    <asp:Panel runat="server" ID="pnlMainSource">
        <asp:Table Width="1000px" runat="server">
            <asp:TableRow>
                <asp:TableCell>
                    <cc1:MultipleSelectionGridView ID="gvNewBankSettlements" runat="server" Caption="Bank Settlements"
                        CaptionAlign="Left" PageSize="10" AllowPaging="True" AllowSorting="True" DataSourceID="odsNewBankSettlements"
                        AutoGenerateColumns="False" DataKeyNames="Key" Width="1000px" SkinID="custom-width">
                        <Columns>
                            <asp:BoundField DataField="Parent_TransactionDate" HeaderText="Booking Date" DataFormatString="{0:d MMMM yyyy}"
                                SortExpression="Parent_TransactionDate">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Parent_Journal_BankAccountDescription" HeaderText="Journal"
                                SortExpression="Parent_Journal_BankAccountDescription">
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DebitDisplayString" HeaderText="Debit" SortExpression="DebitDisplayString">
                                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CreditDisplayString" HeaderText="Credit" SortExpression="CreditDisplayString">
                                <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Description" SortExpression="Description">
                                <ItemTemplate>
                                    <trunc:TruncLabel2 ID="lblDescription" runat="server" Width="75px" CssClass="padding"
                                        MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "Description") %>' />
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Original Description" SortExpression="OriginalDescription">
                                <ItemTemplate>
                                    <trunc:TruncLabel2 ID="lblOriginalDescription" runat="server" Width="250px" CssClass="padding"
                                        MaxLength="40" LongText='<%# DataBinder.Eval(Container.DataItem, "OriginalDescription") %>' />
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:TemplateField>
                        </Columns>
                    </cc1:MultipleSelectionGridView>
                    <asp:ObjectDataSource ID="odsNewBankSettlements" runat="server" SelectMethod="GetUnmatchedBankSettlements"
                        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.ManualSettlementMatchingAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="dpStartDate" Name="startDate" PropertyName="SelectedDate"
                                Type="DateTime" />
                            <asp:ControlParameter ControlID="dpEndDate" Name="endDate" PropertyName="SelectedDate"
                                Type="DateTime" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Height="10">
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <cc1:MultipleSelectionGridView ID="gvUnsettledTrades" runat="server" Caption="Unsettled Trades"
                        CaptionAlign="Left" PageSize="10" AllowPaging="True" AllowSorting="True" DataSourceID="odsUnsettledTrades"
                        AutoGenerateColumns="False" DataKeyNames="Key" Width="1000px" SkinID="custom-width">
                        <Columns>
                            <asp:BoundField DataField="TransactionDate" HeaderText="Transaction Date" DataFormatString="{0:d MMMM yyyy}"
                                SortExpression="TransactionDate">
                                <ItemStyle Wrap="False" Width="150px" />
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                                <ItemTemplate>
                                    <%# (Side)DataBinder.Eval(Container.DataItem, "TxSide") %>
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="CounterValueDisplayString" HeaderText="Value" SortExpression="CounterValueQuantity">
                                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Description" SortExpression="Description">
                                <ItemTemplate>
                                    <trunc:TruncLabel2 ID="lblOriginalDescription" runat="server" CssClass="padding"
                                        MaxLength="36" LongText='<%# DataBinder.Eval(Container.DataItem, "Description") %>' />
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Instrument" SortExpression="TotalGiroInstrumentName">
                                <ItemTemplate>
                                    <trunc:TruncLabel2 ID="lblTotalGiroInstrumentName" runat="server" CssClass="padding"
                                        MaxLength="25" LongText='<%# DataBinder.Eval(Container.DataItem, "TotalGiroInstrumentName") %>' />
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                                <HeaderStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="TotalGiroTradeSizeQuantity" HeaderText="Size" SortExpression="TotalGiroTradeSizeQuantity">
                                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TotalGiroOrderID" HeaderText="Order ID" SortExpression="TotalGiroOrderID">
                                <ItemStyle Wrap="False" HorizontalAlign="Center" />
                                <HeaderStyle Wrap="False" HorizontalAlign="Center" CssClass="aligncenter" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CounterParty" HeaderText="CounterParty" SortExpression="CounterParty">
                                <ItemStyle Wrap="False" HorizontalAlign="Center" />
                                <HeaderStyle Wrap="False" HorizontalAlign="Center" CssClass="aligncenter" />
                            </asp:BoundField>
                        </Columns>
                    </cc1:MultipleSelectionGridView>
                    <asp:ObjectDataSource ID="odsUnsettledTrades" runat="server" SelectMethod="GetUnsettledExternalTrades"
                        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.ManualSettlementMatchingAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="dpStartDate" Name="startDate" PropertyName="SelectedDate"
                                Type="DateTime" />
                            <asp:ControlParameter ControlID="dpEndDate" Name="endDate" PropertyName="SelectedDate"
                                Type="DateTime" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <br />
    <asp:Panel runat="server" ID="pnSelections">
        <asp:UpdatePanel ID="upSelections" runat="server">
            <ContentTemplate>
                <asp:Table Width="1000px" runat="server" GridLines="Both">
                    <asp:TableRow runat="server" Height="0">
                        <asp:TableCell ID="TableCell5" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell6" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell7" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell8" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell9" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell10" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell11" ColumnSpan="1" runat="server"></asp:TableCell>
                        <asp:TableCell ID="TableCell12" ColumnSpan="1" runat="server"></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server" ColumnSpan="8">
                            <asp:GridView ID="gvBankStatementBookingSelections" runat="server" Caption="Bank Selections"
                                CaptionAlign="Left" DataKeyNames="Key" AllowSorting="True" AutoGenerateColumns="false"
                                SkinID="custom-width" Width="1000px">
                                <Columns>
                                    <asp:BoundField DataField="Parent_TransactionDate" HeaderText="Booking Date" DataFormatString="{0:d MMMM yyyy}"
                                        SortExpression="Parent_TransactionDate">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Parent_Journal_BankAccountDescription" HeaderText="Journal"
                                        SortExpression="Parent_Journal_BankAccountDescription">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DebitDisplayString" HeaderText="Debit" SortExpression="DebitDisplayString">
                                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CreditDisplayString" HeaderText="Credit" SortExpression="CreditDisplayString">
                                        <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Original Description" SortExpression="OriginalDescription">
                                        <ItemTemplate>
                                            <trunc:TruncLabel2 ID="lblOriginalDescription" runat="server" Width="450px" CssClass="padding"
                                                MaxLength="60" LongText='<%# DataBinder.Eval(Container.DataItem, "OriginalDescription") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server" Height="10">
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server" ColumnSpan="8">
                            <asp:GridView ID="gvTradeBookingSelections" runat="server" Caption="Trade Selections"
                                CaptionAlign="Left" AllowSorting="True" AutoGenerateColumns="False" SkinID="custom-width"
                                DataKeyNames="Key" Width="1000px">
                                <Columns>
                                    <asp:BoundField DataField="TransactionDate" HeaderText="Transaction Date" DataFormatString="{0:d MMMM yyyy}"
                                        SortExpression="TransactionDate">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Side" SortExpression="Side">
                                        <ItemTemplate>
                                            <%# (Side)DataBinder.Eval(Container.DataItem, "TxSide") %>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CounterValueDisplayString" HeaderText="Value" SortExpression="CounterValueQuantity">
                                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Description" SortExpression="Description">
                                        <ItemTemplate>
                                            <trunc:TruncLabel2 ID="lblOriginalDescription" runat="server" CssClass="padding"
                                                MaxLength="40" LongText='<%# DataBinder.Eval(Container.DataItem, "Description") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Instrument" SortExpression="TotalGiroInstrumentName">
                                        <ItemTemplate>
                                            <trunc:TruncLabel2 ID="lblTotalGiroInstrumentName" runat="server" CssClass="padding"
                                                MaxLength="25" LongText='<%# DataBinder.Eval(Container.DataItem, "TotalGiroInstrumentName") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TotalGiroTradeSizeQuantity" HeaderText="Size" SortExpression="TotalGiroTradeSizeQuantity">
                                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TotalGiroOrderID" HeaderText="Order ID" SortExpression="TotalGiroOrderID">
                                        <ItemStyle Wrap="False" HorizontalAlign="Center" />
                                        <HeaderStyle Wrap="False" HorizontalAlign="Center" CssClass="aligncenter" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server" Height="10">
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell ID="TableCell1" ColumnSpan="3" runat="server">
                            <asp:GridView ID="gvBankSummary" runat="server" Caption="Bank Statements Summary"
                                CaptionAlign="Left" AutoGenerateColumns="False" SkinID="custom-width" Width="375px">
                                <Columns>
                                    <asp:BoundField DataField="Debit" HeaderText="Debit" SortExpression="Debit">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Credit" HeaderText="Credit" SortExpression="Credit">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell2" ColumnSpan="5" runat="server">
                            <asp:GridView ID="gvTradeSummary" runat="server" Caption="Trade Selection Summary"
                                CaptionAlign="Left" AutoGenerateColumns="False" SkinID="custom-width" Width="625px">
                                <Columns>
                                    <asp:TemplateField HeaderText="Instrument" SortExpression="InstrumentName">
                                        <ItemTemplate>
                                            <b4f:TruncatedLabel ID="lblInstrumentName" runat="server" MaxLength="30" Font-Bold='<%# (bool)(DataBinder.Eval(Container.DataItem, "InstrumentName") == "Total") %>'>
                                                <%# DataBinder.Eval(Container.DataItem, "InstrumentName")%></b4f:TruncatedLabel>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Size" SortExpression="Size">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSize" runat="server">
                                                <%#((Decimal)DataBinder.Eval(Container.DataItem, "Size") != 0M ? DataBinder.Eval(Container.DataItem, "Size") : "")%></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Debit" HeaderText="Debit" SortExpression="Debit">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Credit" HeaderText="Credit" SortExpression="Credit">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Button ID="btnCreateEntries" runat="server" Visible="false" Text="Create Ledger Entries"
            OnClientClick="return ConfirmSettlement()" OnClick="btnCreateEntries_OnClick" />
    </asp:Panel>
    <br />
    <br />
    <asp:Label ID="lblErrMessage" ForeColor="Red" runat="server" />
</asp:Content>

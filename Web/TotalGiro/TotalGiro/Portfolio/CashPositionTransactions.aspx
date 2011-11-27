<%@ Page Title="CashPosition Transactions" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="CashPositionTransactions.aspx.cs" Inherits="CashPositionTransactions" %>

<%@ Register Src="../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<%@ Register Src="../UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <asp:Table ID="Table1" runat="server" Width="760px">
        <asp:TableRow ID="TableRow1" runat="server" Height="24px">
            <asp:TableCell ID="TableCell1" runat="server" Width="110px">Account:</asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" Width="650px" Font-Bold="True">
                <asp:Label ID="lblAccount" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server" Height="24px">
            <asp:TableCell ID="TableCell3" runat="server">Instrument:</asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" Font-Bold="True">
                <asp:Label ID="lblInstrument" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" Height="24px">
            <asp:TableCell ID="TableCell5" runat="server">Position Value:</asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server" Font-Bold="True">
                <asp:Label ID="lblValue" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table ID="Table2" runat="server" Width="760px">
        <asp:TableRow ID="TableRow4" runat="server" Height="24px">
            <asp:TableCell ID="TableCell7" runat="server" Width="110px">Date From:</asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server" Width="200px">
                <uc2:Calendar ID="ctlDateFrom" runat="server" Format="dd-MM-yyyy" AutoPostBack="true" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server" Width="80px">Date To:</asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server" >
                <uc2:Calendar ID="ctlDateTo" runat="server" Format="dd-MM-yyyy" AutoPostBack="true" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:MultiView ID="mvwGridViews" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwPositionTxsCashBaseCurrency" runat="server">
            <b4f:MultipleSelectionGridView ID="gvPositionTxsCashBaseCurrency" runat="server"
                SkinID="custom-width" Width="1050px" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                DataSourceID="odsPositionTxsCashBaseCurrency" PageSize="20" Caption="Position Transactions"
                CaptionAlign="Left" DataKeyNames="Key" OnRowCommand="gvPositionTx_RowCommand"
                OnSorting="gvPositionTx_Sorting" >
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbDetails" CommandName="Details" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SearchKey") %>'
                                runat="server" ImageUrl="~/layout/images/audit.gif" Height="16px" Width="16px"
                                ToolTip="View Audit Trail" />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
 <%--                   <asp:BoundField DataField="Key" HeaderText="Key" SortExpression="Key">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>--%>
                    <asp:BoundField DataField="SearchKey" HeaderText="Reference" SortExpression="SearchKey">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Tx Type" SortExpression="TransactionType">
                        <ItemTemplate>
                            <%# ((string)Eval("TransactionType")).Replace("\n", "<br />")%>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="FullDescription">
                        <ItemTemplate>
                            <%# ((string)Eval("FullDescription")).Replace("\n", "<br />") %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" Width="550px" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="DisplayAmount" HeaderText="Amount" SortExpression="AmountQuantity">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Saldo" HeaderText="Saldo" SortExpression="SaldoQuantity">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Tx Date" DataFormatString="{0:dd-MM-yyyy}"
                        HtmlEncode="False" SortExpression="TransactionDate">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreationDate" HeaderText="Creation Date"
                        DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" SortExpression="CreationDate">
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:BoundField>
                </Columns>
            </b4f:MultipleSelectionGridView>
            <asp:ObjectDataSource ID="odsPositionTxsCashBaseCurrency" runat="server" 
            SelectMethod="GetCashPositionTransactions" SortParameterName="sortColumn"
                TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.CashPositionTransactionsAdapter">
                <SelectParameters>
                    <asp:SessionParameter Name="positionId" SessionField="SelectedSubPositionId" Type="Int32"
                        DefaultValue="0" />
                    <asp:ControlParameter ControlID="ctlDateFrom" Name="beginDate" PropertyName="SelectedDate"
                        Type="DateTime" />
                    <asp:ControlParameter ControlID="ctlDateTo" Name="endDate" PropertyName="SelectedDate"
                        Type="DateTime" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
    </asp:MultiView>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label><br />
    <uc1:BackButton ID="ctlBackButton" runat="server" />
    <asp:Button ID="btnExport" runat="server" Text="Export" Width="80px" OnClick="btnExport_Click" />
</asp:Content>

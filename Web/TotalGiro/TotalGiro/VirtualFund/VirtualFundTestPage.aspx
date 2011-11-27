<%@ Page Title="" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="VirtualFundTestPage.aspx.cs" Inherits="VirtualFundTestPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:Table runat="server" Width="100%">
        <asp:TableRow runat="server">
            <asp:TableCell ID="TableCell1" runat="server">
                <asp:Button ID="btnTest" runat="server" Text="Get Cash Details" OnClick="btnTest_Click" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Label ID="lblAccountID" runat="server" Text="AccountID" />
            </asp:TableCell><asp:TableCell ID="TableCell2" runat="server">
                <asp:TextBox ID="txtAccountID" Text="951" runat="server" />
            </asp:TableCell></asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell3" runat="server">                
            </asp:TableCell><asp:TableCell ID="TableCell4" runat="server">
                <asp:Label ID="lblSettledCashPosition" runat="server" Text="SettledCashPosition" />
            </asp:TableCell><asp:TableCell ID="TableCell5" runat="server">
                <asp:TextBox ID="txtSettledCashPosition" runat="server" Text="NothingYet" />
            </asp:TableCell></asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell6" runat="server">                
            </asp:TableCell><asp:TableCell ID="TableCell7" runat="server">
                <asp:Label ID="lblUnSettledCashPosition" runat="server" Text="UnSettledCashPosition" />
            </asp:TableCell><asp:TableCell ID="TableCell8" runat="server">
                <asp:TextBox ID="txtUnSettledCashPosition" runat="server" Text="NothingYet" />
            </asp:TableCell></asp:TableRow>
    </asp:Table>
    <asp:Button ID="btnTestClientLines" runat="server" Text="Test ClientLines" OnClick="btnTestClientLines_Click" />
    <asp:Button ID="btnTestPositions" runat="server" Text="Test Positions" OnClick="btnTestPositions_Click" />
    <asp:TextBox ID="txtJournalEntryLineID" runat="server" Text="4763" />
    <asp:Button ID="btnMigrate" runat="server" Text="Test Migration" OnClick="btnMigrate_Click" />
    <asp:TextBox ID="txtMessage" runat="server" Text="NothingYet" />
    <asp:Button ID="btnApprove" runat="server" Text="Approve Execution" OnClick="btnApprove_Click" />
    <asp:Button ID="btnClientSettle" runat="server" Text="Settle Execution" OnClick="btnClientSettle_Click" />
    <asp:TextBox ID="txtApproveID" runat="server" Text="8" />
    <asp:Table ID="Table1" runat="server" Width="100%">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtExecutionID" runat="server" Text="4763" />
                <asp:Button ID="btnDisplayAllocations" runat="server" Text="Display Allocations"
                    OnClick="btnDisplayAllocations_Click" />
                <asp:TextBox ID="txtExecutionValue" runat="server" Text="NothingYet" />
                <asp:TextBox ID="txtAllocationValue" runat="server" Text="NothingYet" />
            </asp:TableCell></asp:TableRow>
    </asp:Table>
    <asp:Button ID="btnShowGridView" runat="server" Text="Show GridView" OnClick="btnShowGridView_Click" />
    <asp:GridView ID="gvCashLines" runat="server" AllowPaging="True" AllowSorting="true"
        SkinID="custom-width" DataSourceID="odsCashLines" AutoGenerateColumns="False"
        Caption="Cash Overview" Visible="false" CaptionAlign="Left" DataKeyNames="Key"
        PageSize="20" Width="100%">
        <Columns>
            <asp:BoundField DataField="TradeID" HeaderText="TradeID" SortExpression="GroupKey">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Side" HeaderText="Side" SortExpression="Side">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Size">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                <ItemTemplate>
                    <%# ((decimal)DataBinder.Eval(Container.DataItem, "Size") != 0m ? Eval("Size") : "")%></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Fund" HeaderText="Fund" SortExpression="Fund">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
<%--            <asp:BoundField DataField="PriceDisplay" HeaderText="PriceDisplay" SortExpression="PriceDisplay">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>--%>
            <asp:TemplateField HeaderText="Transactiondate">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Transactiondate") != DateTime.MinValue ? Eval("Transactiondate", "{0:d MMMM yyyy}") : "")%></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CreationDate">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate") != DateTime.MinValue ? Eval("CreationDate", "{0:d MMMM yyyy}") : "")%></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CashComponent" HeaderText="CashComponent" SortExpression="CashComponent">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="LedgerCode" HeaderText="LedgerCode" SortExpression="LedgerCode">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="AmountDisplay" HeaderText="AmountDisplay" SortExpression="AmountDisplay">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCashLines" runat="server" SelectMethod="GetCashDetailLines"
        TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.CashDetailLineMapper"></asp:ObjectDataSource>
</asp:Content>

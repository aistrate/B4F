﻿<%@ Page Title="Trading Bookings" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" 
CodeFile="TradingBookings.aspx.cs" Inherits="TradingBookings" %>


<%@ Register Src="~/UC/JournalEntryFinder.ascx" TagName="JournalEntryFinder" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <uc1:JournalEntryFinder ID="ctlJournalEntryFinder" runat="server" />
    <br /><br />
    <asp:GridView ID="gvBookings" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsBookings" AutoGenerateColumns="False" Caption="Bookings" CaptionAlign="Left" DataKeyNames="Key" 
        PageSize="20">
        <Columns>
            <asp:BoundField DataField="JournalEntryNumber" HeaderText="Journal Entry#" SortExpression="JournalEntryNumber" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Journal_JournalNumber" HeaderText="Journal#" SortExpression="Journal_JournalNumber">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Transaction Date" SortExpression="TransactionDate">
                <ItemTemplate>
                    <asp:Label ID="lblTransactionDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "TransactionDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "TransactionDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False"/>
                <HeaderStyle Wrap="False"/>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="False"/>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtLines" runat="server" CausesValidation="False" Text="Lines" CommandName="ViewLines" 
                                    ToolTip="View lines of this booking" OnCommand="lbtLines_Command"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsBookings" runat="server" SelectMethod="GetTradingBookings"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.TradingBookingsAdapter" OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlJournalEntryFinder" Name="journalId" PropertyName="JournalId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlJournalEntryFinder" Name="transactionDateFrom" PropertyName="TransactionDateFrom"
                Type="DateTime" />
            <asp:ControlParameter ControlID="ctlJournalEntryFinder" Name="transactionDateTo" PropertyName="TransactionDateTo"
                Type="DateTime" />
            <asp:ControlParameter ControlID="ctlJournalEntryFinder" Name="journalEntryNumber" PropertyName="JournalEntryNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlJournalEntryFinder" Name="statuses" PropertyName="Statuses"
                Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <table width="770">
        <tr>
            <td style="width:150px">
                <asp:Button ID="btnJournals" runat="server" Text="Journals" CausesValidation="False" 
                            PostBackUrl="~/BackOffice/GeneralLedger/TradingJournals.aspx" />
            </td>
            <td align="right">
                
            </td>
        </tr>
    </table>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>


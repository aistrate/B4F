<%@ Control Language="C#" AutoEventWireup="true" CodeFile="JournalEntryFinder.ascx.cs" Inherits="JournalEntryFinder" %>

<%@ Register Src="~/UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>

<table style="width: 770px">
    <tr>
        <td style="width: 160px; height: 24px">
            <asp:Label ID="Label1" runat="server" Text="Journal:"></asp:Label></td>
        <td colspan="4">
            <asp:DropDownList ID="ddlJournal" runat="server" SkinID="custom-width" Width="250px" DataSourceID="odsJournals" 
                              DataTextField="FullDescription" DataValueField="Key">
            </asp:DropDownList><asp:ObjectDataSource ID="odsJournals" runat="server" SelectMethod="GetJournals"
                TypeName="B4F.TotalGiro.ApplicationLayer.UC.JournalEntryFinderAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdnJournalType" Name="journalType" PropertyName="Value"
                        Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:HiddenField ID="hdnJournalType" runat="server" />
        </td>
    </tr>
    <tr>
        <td style="height: 24px">
            <asp:Label ID="Label2" runat="server" Text="Transaction Date From:"></asp:Label></td>
        <td style="width: 210px">
            <uc1:DatePicker ID="dpTransactionDateFrom" runat="server"/></td>
        <td style="width: 50px">
            <asp:Label ID="Label3" runat="server" Text="To:"></asp:Label></td>
        <td style="width: 210px">
            <uc1:DatePicker ID="dpTransactionDateTo" runat="server"/></td>
        <td></td>
    </tr>
    <tr>
        <td style="height: 24px">
            <asp:Label ID="Label4" runat="server" Text="Journal Entry Number:"></asp:Label></td>
        <td colspan="4">
            <asp:TextBox ID="txtJournalEntryNumber" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td style="height: 24px">
            <asp:Label ID="Label5" runat="server" Text="Status:"></asp:Label></td>
        <td colspan="3">
            <asp:CheckBoxList ID="cblStatus" runat="server" RepeatDirection="Horizontal" Width="263px">
                <asp:ListItem Selected="False" Text="New" Value="New"></asp:ListItem>
                <asp:ListItem Selected="False" Text="Booked" Value="Booked"></asp:ListItem>
                <asp:ListItem Selected="False" Text="Open" Value="Open"></asp:ListItem>
            </asp:CheckBoxList></td>
        <td align="right">
            <asp:Button ID="btnSearch" runat="server" Text="Search" CausesValidation="False" OnClick="btnSearch_Click" /></td>
    </tr>
</table>

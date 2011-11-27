<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TradingJournals.aspx.cs"
    Inherits="TradingJournals" Title="Trading Journals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <asp:GridView ID="gvJournals" runat="server" Caption="Journals" CaptionAlign="Left"
        PageSize="25" AllowPaging="True" AllowSorting="True" DataSourceID="odsJournals"
        AutoGenerateColumns="False" DataKeyNames="Key">
        <Columns>
            <asp:BoundField DataField="JournalNumber" HeaderText="Journal#" SortExpression="JournalNumber">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="BankAccountDescription" HeaderText="Description" SortExpression="BankAccountDescription">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ManagementCompany_CompanyName" HeaderText="Company" SortExpression="ManagementCompany_CompanyName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Currency_Symbol" HeaderText="Curr" SortExpression="Currency_Symbol">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtBookings" runat="server" CausesValidation="False" Text="Bookings"
                        CommandName="ViewBookings" ToolTip="View bookings of this journal" OnCommand="lbtBookings_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsJournals" runat="server" SelectMethod="GetTradingJournals"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.TradingJournalsAdapter">
    </asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

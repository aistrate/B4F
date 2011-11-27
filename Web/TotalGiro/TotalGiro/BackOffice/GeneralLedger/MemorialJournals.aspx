<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="MemorialJournals.aspx.cs" Inherits="MemorialJournals" 
         Title="Memorial Journals" Theme="Neutral" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
<script type="text/javascript">
    function ConfirmNewBooking(openBookingsCount)
    {
        if (openBookingsCount > 0)
            return confirm(openBookingsCount + ' open bookings already exist. Create new?');
        else
            return true;
    }
</script>

    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <br />
    <asp:GridView ID="gvJournals" runat="server" Caption="Journals" CaptionAlign="Left" PageSize="25" AllowPaging="True" 
                  AllowSorting="True" DataSourceID="odsJournals" AutoGenerateColumns="False" DataKeyNames="Key" >
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
                <ItemStyle Wrap="False"/>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtNew" runat="server" CausesValidation="False" Text="New" CommandName="NewBooking"
                                    ToolTip="Add a new memorial booking to this journal" OnCommand="lbtNew_Command" 
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                    OnClientClick='<%# string.Format("return ConfirmNewBooking({0});", DataBinder.Eval(Container.DataItem, "OpenEntries")) %>' ></asp:LinkButton>
                    <asp:LinkButton ID="lbtBookings" runat="server" CausesValidation="False" Text="Bookings" CommandName="ViewBookings"
                                ToolTip="View memorial bookings of this journal" OnCommand="lbtBookings_Command"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsJournals" runat="server" SelectMethod="GetMemorialJournals"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.MemorialJournalsAdapter"></asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>


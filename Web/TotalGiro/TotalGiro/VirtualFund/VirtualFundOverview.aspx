<%@ Page Title="Virtual Fund Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="VirtualFundOverview.aspx.cs" Inherits="VirtualFundOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:GridView ID="gvFundOverview" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsFundOverview" AutoGenerateColumns="False" Caption="Fund Overview"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:BoundField DataField="InstrumentName" HeaderText="InstrumentName" SortExpression="InstrumentName">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Creation Date" SortExpression="CreationDate">
                <ItemTemplate>
                    <asp:Label ID="lblChangeDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "CreationDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <%--            <asp:BoundField DataField="UnitsAllocated" HeaderText="Units Allocated" SortExpression="UnitsAllocated">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>--%>
            <%--            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDetails" runat="server" CausesValidation="False" Text="Details"
                        CommandName="ViewDetails" ToolTip="View Details" OnCommand="lbtDetails_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>--%>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFundOverview" runat="server" SelectMethod="GetVirtualFunds"
        TypeName="B4F.TotalGiro.ApplicationLayer.VirtualFunds.VirtualFundOverviewAdapter"
        OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
    <br />

    <br />
    <asp:UpdatePanel ID="upNewFunds" runat="server">
        <ContentTemplate>
            <asp:Table runat="server" Width="1000px" CellSpacing="0" BorderStyle="Solid" Caption="New Virtual Fund">
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="lblFundName" runat="server" Text="Fund Name:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txtFundName" runat="server" SkinID="custom-width" MaxLength="100"
                            Text="" Width="200" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow  runat="server">
                    <asp:TableCell  runat="server">
                        <asp:Label ID="lblSuggestedHoldingName" runat="server" Text="Holding Account Name:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell  runat="server">
                        <asp:TextBox ID="txt" runat="server" SkinID="custom-width" MaxLength="100" Text=""
                            Width="200" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </ContentTemplate>
    </asp:UpdatePanel>
        <br />
    <asp:Button ID="btnCreateNewFund" runat="server" Text="Create New Fund" />
</asp:Content>

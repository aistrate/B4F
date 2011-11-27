<%@ Page Title="Virtual Fund Nav Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="VirtualFundNavOverview.aspx.cs" Inherits="VirtualFundNavOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <br />
    <asp:GridView ID="gvInstruments" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsInstruments" AutoGenerateColumns="False" Caption="VirtualFunds"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Isin" HeaderText="Isin" SortExpression="Isin">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Last Nav Date" SortExpression="LastNavDate">
                <ItemTemplate>
                    <asp:Label ID="LastNavDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "LastNavDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "LastNavDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtNew" runat="server" CausesValidation="False" Text="New" CommandName="NewCalculation"
                        ToolTip="Add a new Calculation to this fund" OnCommand="lbtNew_Command" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                    <asp:LinkButton ID="lbtalculations" runat="server" CausesValidation="False" Text="Calculations"
                        CommandName="ViewCalculations" ToolTip="View calculations of this journal" OnCommand="lbtCalculations_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsInstruments" runat="server" SelectMethod="GetVirtualFunds"
        TypeName="B4F.TotalGiro.ApplicationLayer.VirtualFunds.VirtualFundNavOverviewAdapter">
    </asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

<%@ Page Title="Exact Journal Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" 
CodeFile="ExactJournalOverview.aspx.cs" Inherits="ExactJournalOverview" %>


<%@ Import Namespace="B4F.TotalGiro.Communicator.Exact" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:GridView ID="gvExactJournals" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsExactJournals" AutoGenerateColumns="False" Caption="Asset Managers"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:BoundField DataField="JournalNumber" HeaderText="Journal Number" SortExpression="JournalNumber">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Exact Journal Type" SortExpression="TypeOfLedger">
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <%# (LedgerTypes)DataBinder.Eval(Container.DataItem, "TypeOfLedger")%>
                </ItemTemplate>
            </asp:TemplateField>            
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDetails" runat="server" CausesValidation="False" Text="Details"
                        CommandName="ViewDetails" ToolTip="View Details" OnCommand="lbtDetails_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsExactJournals" runat="server" SelectMethod="GetJournals"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.GeneralLedger.ExactJournalsAdapter"
        OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
    <br />
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell1" runat="server" ColumnSpan="9"></asp:TableCell>
            <%--           <asp:TableCell ID="TableCell2" runat="server">
                <asp:Button ID="btnNewAssetManger" runat="server" Text="New AssetManger" CausesValidation="False"
                    OnClick="btnNewAssetManger_Click" />
            </asp:TableCell>--%>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>


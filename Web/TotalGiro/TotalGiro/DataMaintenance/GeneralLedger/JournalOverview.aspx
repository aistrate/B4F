<%@ Page Title="Journal Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="JournalOverview.aspx.cs" Inherits="JournalOverview" %>

<%@ Import Namespace="B4F.TotalGiro.GeneralLedger.Static" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:GridView ID="gvGLJournals" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsGLJournals" AutoGenerateColumns="False" Caption="Asset Managers"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:BoundField DataField="JournalNumber" HeaderText="Journal Number" SortExpression="JournalNumber">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Journal Type" SortExpression="JournalType">
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <%# (JournalTypes)DataBinder.Eval(Container.DataItem, "JournalType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="BankAccountDescription" HeaderText="Description" SortExpression="BankAccountDescription">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="BankAccountNumber" HeaderText="Bank Account Number" SortExpression="BankAccountNumber">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="FixedGLAccount" HeaderText="Fixed GLAccount" SortExpression="FixedGLAccount">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
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
    <asp:ObjectDataSource ID="odsGLJournals" runat="server" SelectMethod="GetGLJournals"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.GeneralLedger.JournalsAdapter"
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

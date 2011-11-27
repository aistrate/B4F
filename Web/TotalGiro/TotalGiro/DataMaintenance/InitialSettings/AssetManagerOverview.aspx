<%@ Page Title="AssetManager Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="AssetManagerOverview.aspx.cs" Inherits="AssetManagerOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:GridView ID="gvAssetManagers" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsAssetManagers" AutoGenerateColumns="False" Caption="Asset Managers"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:BoundField DataField="CompanyName" HeaderText="CompanyName" SortExpression="CompanyName">
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
    <asp:ObjectDataSource ID="odsAssetManagers" runat="server" SelectMethod="GetAssetManagers"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.InitialSettings.AssetManagerAdapter"
        OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
    <br />
    <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="9"></asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Button ID="btnNewAssetManger" runat="server" Text="New AssetManger" CausesValidation="False"
                    OnClick="btnNewAssetManger_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

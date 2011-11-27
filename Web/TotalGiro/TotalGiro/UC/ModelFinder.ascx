<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ModelFinder.ascx.cs" Inherits="ModelFinder" %>
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="Label1" runat="server" Text="Model Name:"></asp:Label>
            </td>
            <td style="width: 190px">
                <asp:TextBox ID="txtModelName" runat="server"></asp:TextBox>
            </td>
            <td></td>
        </tr>
        <asp:Panel ID="pnlActivityFilter" runat="server">
        <tr>
            <td style="height: 24px">
                <asp:Label ID="lblModelActive" runat="server" Text="Status:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlModelActive" runat="server" DataSourceID="odsModelActive"
                    DataTextField="Status" DataValueField="ID" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsModelActive" runat="server" SelectMethod="GetAccountStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                   CausesValidation="False" Width="90px" />
            </td>
        </tr>
        </asp:Panel>
    </table>

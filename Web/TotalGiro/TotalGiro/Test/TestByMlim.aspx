<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestByMlim.aspx.cs" Inherits="Test_TestByMlim" Title="Untitled Page"  Theme="Neutral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy id="sm" runat="Server" />
    <table cellpadding="1" cellspacing="0" border="1" style="border-color:red" width="100%">
        <tr>
            <td>
                <asp:Button ID="btnClientPortfolio" runat="server" OnClick="btnClientPortfolio_Click" Text="ClientPortfolio" Width="150px" />
            </td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtDumpToFile" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label></td>
        </tr>
    </table>
</asp:Content>


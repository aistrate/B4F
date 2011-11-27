<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AppErrors.aspx.cs" Inherits="AppErrors" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    The application generated an error. &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
    <asp:Button ID="btnDetails" runat="server" OnClick="btnDetails_Click" Text="Show Details >>" />
    &nbsp;<br />
    <asp:HiddenField ID="hidDetailsVisible" runat="server" Value="false" />
    <br />
    <br />
    <asp:Panel ID="pnlErrorInfo" runat="server" Visible="False">
        <table>
            <tr>
                <td style="height: 21px; width: 150px" nowrap="noWrap">
                    Execution Path:</td>
                <td style="width: 300px">
                    <asp:Label ID="lblExecutionPath" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 21px" nowrap="noWrap">
                    Timestamp:</td>
                <td>
                    <asp:Label ID="lblTimestamp" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 21px" nowrap="noWrap">
                    User:</td>
                <td>
                    <asp:Label ID="lblUserName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 21px" nowrap="noWrap">
                    User Host Name:</td>
                <td>
                    <asp:Label ID="lblUserHostName" runat="server"></asp:Label></td>
            </tr>
        </table>
        </asp:Panel>
    <asp:Label ID="lblErrorDetails" runat="server"></asp:Label>
    <br /><br /><br />
    
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Authenticate.AppErrors" Codebehind="AppErrors.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:HiddenField ID="hidDetailsVisible" runat="server" Value="false" />
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="2" style="height: 28px" nowrap="noWrap">
                &nbsp;<input id="btnBack" type="button" value="Back" onclick="history.go(-1)" style="width: 98px"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height: 20px"></td>
        </tr>
        
        <asp:Panel ID="pnlErrorInfo" runat="server" Visible="True">
            <tr>
                <td style="height: 28px; width: 150px" nowrap="noWrap">
                    <asp:Label ID="Label2" runat="server" Text="Execution Path:"></asp:Label></td>
                <td style="width: 600px" nowrap="noWrap">
                    <asp:Label ID="lblExecutionPath" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 28px" nowrap="noWrap">
                    <asp:Label ID="Label3" runat="server" Text="Date / Time:"></asp:Label></td>
                <td nowrap="noWrap">
                    <asp:Label ID="lblTimestamp" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 28px" nowrap="noWrap">
                    <asp:Label ID="Label4" runat="server" Text="User:"></asp:Label></td>
                <td nowrap="noWrap">
                    <asp:Label ID="lblUserName" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
        </asp:Panel>
        
        <tr>
            <td colspan="2" style="height: 10px"></td>
        </tr>
        <tr>
            <td colspan="2" style="height: 28px">
                <asp:Label ID="lblErrorDetails" runat="server" Font-Bold="True" Width="720px" Font-Size="1.15em"></asp:Label>
            </td>
        </tr>
    </table>
    
</asp:Content>
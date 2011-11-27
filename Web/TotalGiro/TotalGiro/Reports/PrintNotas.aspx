<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="PrintNotas.aspx.cs" Inherits="Reports_PrintNotas" 
    Title="Nota's" Theme="Neutral" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td style="width: 300px; text-align: left">
                <asp:Button ID="btnCreateNotas" runat="server" Text="Create Nota's" OnClick="btnCreateNotas_Click" Width="120px" 
                    OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerHTML = '<br />Creating nota\'s...';" />&nbsp;
                <asp:Button ID="btnPrintNotas" runat="server" Text="Print Nota's" OnClick="btnPrintNotas_Click" Width="120px" 
                    OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerHTML = '<br />Printing nota\'s...';" />
            </td>
            <td style="width: 435px; text-align: right">
                <asp:Button ID="btnSendEmailNotifications" runat="server" Text="Send Email Notifications" 
                    OnClick="btnSendEmailNotifications_Click" Width="190px" 
                    OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerHTML = '<br />Sending email notifications for nota\'s and financial reports...';" />
            </td>
        </tr>
    </table>
    
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    
</asp:Content>


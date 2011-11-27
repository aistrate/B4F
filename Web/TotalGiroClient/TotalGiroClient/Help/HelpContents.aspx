<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Help.HelpContents" Codebehind="HelpContents.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td style="height: 5px">
            </td>
        </tr>
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkPortfolioHelp" runat="server"
                                      PostBackUrl="~/Help/PortfolioHelp.aspx">Portefeuille</b4f:ArrowsLinkButton>
            </td>
        </tr>
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkChartsHelp" runat="server"
                                      PostBackUrl="~/Help/ChartsHelp.aspx">Grafieken</b4f:ArrowsLinkButton>
            </td>
        </tr>        
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkNotasHelp" runat="server"
                                      PostBackUrl="~/Help/NotasHelp.aspx">Afschriften</b4f:ArrowsLinkButton>
            </td>
        </tr>
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkFinancialReportsHelp" runat="server"
                                      PostBackUrl="~/Help/FinancialReportsHelp.aspx">Rapportages</b4f:ArrowsLinkButton>
            </td>
        </tr>
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkSettingsHelp" runat="server"
                                      PostBackUrl="~/Help/SettingsHelp.aspx">Instellingen</b4f:ArrowsLinkButton>
            </td>
        </tr>
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkChangePasswordHelp" runat="server"
                                      PostBackUrl="~/Help/ChangePasswordHelp.aspx">Wachtwoord wijzigen</b4f:ArrowsLinkButton>
            </td>
        </tr>
        <tr>
            <td style="height: 30px">
                <b4f:ArrowsLinkButton ID="lnkLoginHelp" runat="server"
                                      PostBackUrl="~/Help/LoginHelp.aspx">Inloggen</b4f:ArrowsLinkButton>
            </td>
        </tr>
    </table>
    
</asp:Content>

<%@ Control Language="C#" 
            Inherits="B4F.TotalGiro.Client.Web.UC.PortfolioNavigationBar" Codebehind="PortfolioNavigationBar.ascx.cs" %>

<table cellpadding="0" cellspacing="0">
    <tr class="screen-only">
        <td colspan="2" style="height: 17px">
            <b4f:ArrowsLinkButton ID="lnkClientPortfolios" runat="server" SkinID="padding" TabIndex="1"
                                  PostBackUrl="~/Clients/ClientPortfolios.aspx?loadPersisted=true">Back to Clients</b4f:ArrowsLinkButton>
            
            <asp:PlaceHolder ID="phdPortfolio" runat="server">
                &nbsp;&nbsp;&nbsp;
                <b4f:ArrowsLinkButton ID="lnkPortfolio" runat="server" SkinID="padding" TabIndex="2"
                                      PostBackUrl="~/Portfolio/PortfolioPositions.aspx">Portefeuille</b4f:ArrowsLinkButton>
            </asp:PlaceHolder>
            
            <asp:PlaceHolder ID="phdCharts" runat="server">
                &nbsp;&nbsp;&nbsp;
                <b4f:ArrowsLinkButton ID="lnkCharts" runat="server" SkinID="padding" TabIndex="3"
                                      PostBackUrl="~/Charts/Charts.aspx">Grafieken</b4f:ArrowsLinkButton>
            </asp:PlaceHolder>
            
            <asp:PlaceHolder ID="phdPlanner" runat="server">
                &nbsp;&nbsp;&nbsp;
                <b4f:ArrowsLinkButton ID="lnkPlanner" runat="server" SkinID="padding" TabIndex="4"
                                      PostBackUrl="~/Planning/FinancialPlanner.aspx?initialize=true">Monitor Portefeuille</b4f:ArrowsLinkButton>
            </asp:PlaceHolder>
        </td>
    </tr>
    <tr>
        <td style="width: 155px; height: 12px"></td>
        <td style="width: 360px"></td>
    </tr>
    <tr>
        <td style="height: 25px; white-space: nowrap">
            <asp:Label ID="lblContactLabel" runat="server" Text="Contact:"></asp:Label>
        </td>
        <td style="white-space: nowrap">
            <asp:Label ID="lblContactFullName" runat="server" Font-Bold="True"></asp:Label>
        </td>
    </tr>
</table>

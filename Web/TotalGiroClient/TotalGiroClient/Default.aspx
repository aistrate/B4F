<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Default" Codebehind="Default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:MultiView ID="mvwWelcome" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwUserLoggedOut" runat="server">
            <asp:Panel ID="pnlWelcomeLoggedOut" runat="server" Font-Size="1.15em">
                Welkom op uw persoonlijke 
                <asp:Literal ID="litWebsiteName" runat="server"/> 
                website. U kunt 
                <asp:LinkButton ID="lnkLogIn" runat="server" PostBackUrl="~/Authenticate/Login.aspx">hier</asp:LinkButton>
                inloggen.
            </asp:Panel>
            
            <asp:Panel ID="pnlAnnouncement" runat="server" Width="650px" Font-Size="1.15em" Visible="false" >
                <br /><br />
                <p>
                    Wij danken u voor uw bezoek aan onze nieuwe website. Uw gebruikersnaam 
                    is donderdag 25 september, per brief, naar u verzonden. Het wachtwoord 
                    volgt, maandag 29 september, per email.
                </p>
                <p>
                    Wij wensen u veel plezier toe op de nieuwe website.
                </p>
                <p>
                    Met vriendelijk groet, <br />
                    De medewerkers van de servicedesk van Paerel Vermogensbeheer.
                </p>
            </asp:Panel>
            
            <br /><br />
            <p class="info" style="width: 650px; margin-left: 0px" >
                De getoonde waarde van uw portefeuille wordt berekend op basis van de laatste gepubliceerde koersen van de 
                verschillende fondsen. Deze koersen zijn altijd enkele dagen vertraagd. In beweeglijke markten kan de huidige 
                waarde sterk afwijken van de getoonde waarde.
            </p>
        </asp:View>
        <asp:View ID="vwUserLoggedIn" runat="server">
            <asp:Panel ID="pnlWelcomeLoggedIn" runat="server" Font-Size="1.15em">
                Welkom,&nbsp;<asp:Literal ID="litContactName" runat="server"></asp:Literal>.</asp:Panel>
            <asp:Panel ID="pnlLastLoginDate" runat="server" Visible="False">
                <br />
                Uw laatste bezoek was op 
                <asp:Literal ID="litLastLoginDate" runat="server"></asp:Literal> om
                <asp:Literal ID="litLastLoginTime" runat="server"></asp:Literal> uur.</asp:Panel>
            <br /><br /><br /><br />
            <b4f:ArrowsLinkButton ID="lnkPortfolio" runat="server" PostBackUrl="~/Portfolio/PortfolioPositions.aspx">Portefeuille</b4f:ArrowsLinkButton>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <b4f:ArrowsLinkButton ID="lnkNotas" runat="server" PostBackUrl="~/Reports/Notas.aspx">Afschriften</b4f:ArrowsLinkButton>
        </asp:View>
    </asp:MultiView>
</asp:Content>

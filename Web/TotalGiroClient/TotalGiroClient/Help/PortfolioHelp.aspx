<%@ Page Language="C#" MasterPageFile="~/Help/Help.master" 
         Inherits="B4F.TotalGiro.Client.Web.Help.PortfolioHelp" Codebehind="PortfolioHelp.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="helpBodyContentPlaceHolder" Runat="Server">

    <img src="../Help/Images/PortfolioPositions.png" alt="Uw portefeuille overzicht." />
    <h5>
        1. Rekening(en)</h5>
    <p>
        Selecteer een rekening om uw portefeuille te bekijken.</p>
    <h5>
        2. Snel koppeling <q>Toon gesloten posities</q></h5>
    <p>
        U kunt de gesloten portefeuille posities raadplegen door op deze koppeling te
        klikken. Zie voor verder informaties in de punten 2A en 2B.</p>
    <h5>
        3. Uw model</h5>
    <p>
        Uw model met de bijbehorende fondsen, aantal participatie, gewichten verdeling in
        percentage, de laatst bekende prjs en de bijhorende de waarde van het fonds. U kunt
        op fonds niveau gedetailleerd positie transactie bekijken door op een fonds te klikken.
        Zie voor verder informaties in de punten 2A-1 en 2B-1.
    </p>
    <h5>
        4. Snel koppeling <q>Verwerkte geldboekingen</q></h5>
    <p>
        U kunt de verwerkte geldboekingen raadplegen door op deze koppeling te klikken.
        Zie voor verder informaties in de punten 4A en 4B.</p>
    <hr />
    <img src="../Help/Images/ClosedPositions.png" alt="Uw verwerkte posities overzicht." />
    <h5>
        2A. Snel koppeling <q>Toon huidige posities</q></h5>
    <p>
        Klik op deze koppeling om terug te gaan naar uw huidige portefeuille overzicht.</p>
    <h5>
        2B. Gesloten fondsen</h5>
    <p>
        De gesloten fondsen met de bijbehorende uitoefenprijs en de wisselkoers wordt
        hier weergegeven.</p>
    <p>
        U kunt op een fonds klikken om de positie transactie(s) te bekijken. Zie voor verder
        informaties in de punten 2B-1 en 2B-2.
    </p>
    <a id="positionTxs" name="positionTxs"></a>
    <hr />
    <img src="../Help/Images/PositionTransactions.png" alt="Uw positie transacties overzicht." />
    <h5>
        2B-1. Fonds(en)</h5>
    <p>
        Selecteer een fonds om de positie transacties(s) te bekijken.</p>
    <h5>
        2B-2. Overzicht positie transacties</h5>
    <p>
        U vindt hier de verwerkte positie transacties van de selecteerde fonds.
    </p>
    <a id="cashMutations" name="cashMutations"></a>
    <hr />
    <img src="../Help/Images/CashMutations.png" alt="Het overzicht van uw geldboekingen." />
    <h5>
        4A. Snel koppeling <q>Terug naar Portefeuille</q></h5>
    <p>
        Klik op deze koppeling om terug te gaan naar uw portefeuille overzicht.</p>
    <h5>
        4B. Overzicht verwerkte geldboeking</h5>
    <p>
        Het overzicht van uw verwerkte geldboekingen.</p>
        
</asp:Content>

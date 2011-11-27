<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="DumpDataSets.aspx.cs" Inherits="Test_DumpDataSets" 
    Title="Dump DataSets" Theme="Neutral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <asp:Label ID="Label1" runat="server" Text="Dump to File:"></asp:Label>
    &nbsp;
    <asp:TextBox ID="txtDumpToFile" runat="server" SkinID="custom-width" Width="617px"></asp:TextBox>
    <br /><br /><br />
    <asp:Button ID="btnAggStichtingOrders" runat="server" Text="Agg. Stichting Orders" OnClick="btnAggStichtingOrders_Click" Width="150px" />
    <br /><br />
    <asp:Button ID="btnNota" runat="server" OnClick="btnNota_Click" Text="Nota" Width="150px"
        OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerText = 'Dumping DataSet...'" />
    <br /><br />
    <asp:Button ID="btnLetter" runat="server" Text="Letter" Width="150px"
        OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerText = 'Dumping DataSet...'" 
        onclick="btnLetter_Click" />
    <br /><br />
    <asp:Button ID="btnFinancialReport" runat="server" OnClick="btnFinancialReport_Click" Text="Financial Report" Width="150px"
          OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerText = 'Dumping DataSet...'" />
    <br /><br />
    <asp:Button ID="btnTestByMJN" runat="server" OnClick="btnTestByMJN_Click" Text="Test Reports" Width="150px" />
    <br /><br />
    <asp:Button ID="btnPrintBulkQ3Report" runat="server" OnClick="btnPrintBulkQ3Report_Click" Text="PrintBulk Q3 Report" Width="150px" />
    <br /><br />
    <asp:Button ID="btnFiscaalJaarOpgaaf" runat="server" OnClick="btnFiscaalJaarOpgaaf_Click" Text="Fiscaal Jaar Opgaaf" Width="150px" />
    <br /><br />
    <asp:Button ID="btnTestByMLim" runat="server" OnClick="btnTestByMLim_Click" Text="Printed Status" Width="150px" />
    &nbsp;
    <asp:TextBox ID="txtAccountId" runat="server"></asp:TextBox>
    <br />
        <asp:Button ID="btnTestPositionTransferReport" runat="server" OnClick="btnTestPositionTransferReport_Click" Text="PositionTransfer" Width="150px" />

    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
</asp:Content>


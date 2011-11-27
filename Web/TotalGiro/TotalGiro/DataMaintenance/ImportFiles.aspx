<%@ Page Language="C#" StylesheetTheme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ImportFiles.aspx.cs" Inherits="ImportFiles" Title="Importeer files van derden naar TG" %>

<%@ Register Src="../UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="scm" runat="server" /> <%--EnableScriptGlobalization="true"--%>
    <br />
    <br />
    <table style="width: 459px;">
        <tr>
            <td style="width: 459px">
                <asp:Label ID="lblHistRatesUpdated" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 459px; height: 45px;">
            <asp:Button ID="btnImportExchangeRates" runat="server" OnClick="btnImportExchangeRates_Click"
                    Text="Import Historical Exchange Rates" ToolTip="Exchanges Rates from ECB are refreshed round noon" Width="292px" />
            <uc2:DatePicker ID="dtpImportExchangeRates" runat="server" IsButtonDeleteVisible="false" />
            <br />
                <asp:Label ID="lblErrorImportExchangeRates" runat="server" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 459px; height: 45px;">
                <asp:Button ID="btnErrorImportPrices" runat="server" OnClick="btnImportPrices_Click"
                    Text="Import Historical Prices" ToolTip="Imports prices from Tijdbeurs Media" Width="292px" />
                <br />
                <asp:Label ID="lblErrorImportPrices" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 459px; height: 45px;">
                <asp:Button ID="btnImportFundPrices" runat="server" OnClick="btnImportFundPrices_Click"
                    Text="Import Fund Prices" ToolTip="Imports fund prices from file" Width="292px" Visible="False" />
                <br />
                <asp:Label ID="lblErrorImportFundPrices" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
   </table>
    
<br />
    &nbsp;<br />
    <br />
    <br />
    &nbsp;<br />

    <br />
    <br />
    <br />
    
    <br />
    <br />
    

    <br />
    <asp:Calendar ID="cldDate" runat="server" 
        SelectionMode="Day" TodayDayStyle-ForeColor="blue" Visible="false" Format="dd-MM-yyyy" >
        <TodayDayStyle ForeColor="Blue" />
    </asp:Calendar>
    <br />
    <asp:Button ID="btnImportTransactionReceipts" runat="server" OnClick="btnImportTransactionReceipts_Click"
        Text="Import PDF receipts from BO" ToolTip="Import PDF receipts from Back Office" Width="292px" /><br />
    <asp:Label ID="btnErrorImportTransactionReceipts" runat="server" Text="" ForeColor="Red"></asp:Label>   
    <br />
    
    
    <script type="text/javascript">
        function setCalendarVisible(item)
        {
            var divCalendar = document.getElementById(item);
            
            if(divCalendar.style.visibility == 'hidden')
                divCalendar.style.visibility = 'visible';
            else
                divCalendar.style.visibility = 'hidden';
        }
    </script>
    
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="XEConverter.aspx.cs" 
    Inherits="DataMaintenance_Prices_XEConverter" Title="XE Converter" %>

<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/CalendarPlusNavigation.ascx" TagName="Calendar" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <br />
    <table width="600px">
        <tr>
            <td>
                <asp:Label ID="lblAmount" runat="server" Text="Amount:" />
            </td>
            <td>
                <db:DecimalBox ID="dbAmount" runat="server" DecimalPlaces="6" Width="145px" />
            </td>
            <td>
                <asp:Label ID="lblFrom" runat="server" Text="From:" />
                <asp:DropDownList ID="ddlCurrencyFrom" SkinID="custom-width" runat="server" Width="60px"
                    DataSourceID="odsCurrencies" DataTextField="Symbol" DataValueField="Key"
                    OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged" AutoPostBack="true"  >
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnSwitch" runat="server" Text="<->" OnClick="btnSwitch_Click" />
            </td>
            <td>
                <asp:Label ID="lblTo" runat="server" Text="To:" />
                <asp:DropDownList ID="ddlCurrencyTo" SkinID="custom-width" runat="server" Width="60px"
                    DataSourceID="odsCurrencies" DataTextField="Symbol" DataValueField="Key"
                    OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged" AutoPostBack="true" >
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnConvert" runat="server" Text="Convert" OnClick="btnConvert_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDate" runat="server" Text="Date:" />
            </td>
            <td >
                <uc1:Calendar ID="dpDate" runat="server" IsButtonDeleteVisible="true" />
            </td>
            <td colspan="2">
                <asp:Label ID="lblExRateDateFrom" runat="server" />
            </td>
            <td colspan="2" >
                <asp:Label ID="lblExRateDateTo" runat="server" />
            </td>
        </tr>
        <tr></tr>
        <tr>
            <td colspan="6" >
                <asp:Label ID="lblConvertedAmount" runat="server" Font-Bold="true" />
            </td>
        </tr>
    </table>

    <br />
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
    <br />

    <asp:Button ID="btnCurrentBaseAmount" runat="server" Text="CurrentBaseAmount" OnClick="btnCurrentBaseAmount_Click" />
    <asp:Button ID="btnBaseAmount" runat="server" Text="BaseAmount" OnClick="btnBaseAmount_Click" />



    <asp:ObjectDataSource ID="odsCurrencies" runat="server" SelectMethod="GetCurrencies"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Prices.XEConverterAdapter" >
    </asp:ObjectDataSource>


</asp:Content>


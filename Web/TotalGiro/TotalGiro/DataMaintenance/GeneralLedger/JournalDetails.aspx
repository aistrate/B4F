<%@ Page Title="Journal Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="JournalDetails.aspx.cs" Inherits="JournalDetails" %>

<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell ColumnSpan="10" runat="server">
                <asp:Label ID="lblMessage" Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table ID="Table2" runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblJournalNumber" Text="GL Journal Number:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtJournalNumber" Font-Bold="true" runat="server" SkinID="custom-width"
                    Width="250px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblJournalType" Text="Journal Type:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:DropDownList ID="ddlJournalType" runat="server" DataTextField="Description"
                    DataValueField="Key" DataSourceID="odsJournalType" SkinID="custom-width" Width="250px">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsJournalType" runat="server" SelectMethod="GetJournalTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.GeneralLedger.JournalsAdapter">
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblDescription" Text="Description:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtDescription" Font-Bold="true" runat="server" SkinID="custom-width"
                    Width="250px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblBankAccountNumber" Text="Bank Account Number:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtBankAccountNumber" Font-Bold="true" runat="server" SkinID="custom-width"
                    Width="250px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblFixedAccount" Text="Fixed Account:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtFixedAccount" Font-Bold="true" runat="server" SkinID="custom-width"
                    Width="250px" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <br />
    <asp:TableRow runat="server">
        <asp:TableCell  runat="server">
                
        </asp:TableCell>
        <asp:TableCell runat="server">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save Data"
                ToolTip="Saves / Inserts " Width="250px" />
        </asp:TableCell>
    </asp:TableRow>
    <uc1:BackButton ID="ctlBackButton" runat="server" />
</asp:Content>

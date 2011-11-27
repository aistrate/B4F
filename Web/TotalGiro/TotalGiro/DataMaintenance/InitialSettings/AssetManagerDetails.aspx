<%@ Page Title="AssetManager Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="AssetManagerDetails.aspx.cs" Inherits="AssetManagerDetails" %>

<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hfAssetManagerID" runat="server" />
    <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell ColumnSpan="10" runat="server">
                <asp:Label ID="lblMessage" Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblName" Text="Asset Manager:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtName" Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
                <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblInitials" Text="Initials:" runat="server" />
            </asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:TextBox ID="txtInitials" Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblTradingAccount" Text="Trading Account:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtTradingAccount" Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblNostroAccountName" Text="Nostro Account:" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtNostroAccountName" Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="lblIsActive" Text="Active?" runat="server" />
            </asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:CheckBox ID="chkIsActive"  Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblSupportLifecycles" Text="Support Lifecycles?" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:CheckBox ID="chkSupportLifecycles"  Font-Bold="true" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click"
                    Text="Save Data" ToolTip="Saves / Inserts "
                    Width="292px" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <uc1:BackButton ID="ctlBackButton" runat="server" />
</asp:Content>

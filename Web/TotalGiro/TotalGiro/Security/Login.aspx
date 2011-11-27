<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:Login ID="ctlLogin" runat="server" BackColor="#F7F6F3" BorderColor="#E6E2D8" BorderPadding="4"
        BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="11px" ForeColor="#333333" 
        DisplayRememberMe="False" DestinationPageUrl="~/Default.aspx" >
        <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="1.15em" ForeColor="White" Height="17px" 
                        BorderWidth="1px" BorderColor="#F7F6F3" />
        <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
        <LabelStyle Width="80px" Height="23px" />
        <TextBoxStyle Width="145px" Height="14px" />
        <LoginButtonStyle Width="75px" />
    </asp:Login>
</asp:Content>
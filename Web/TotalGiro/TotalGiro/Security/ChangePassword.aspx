<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ChangePassword.aspx.cs"
         Inherits="ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <asp:ChangePassword ID="ctlChangePassword" runat="server"
        BackColor="#F7F6F3" 
        Font-Names="Verdana" Font-Size="11px" ForeColor="#333333"
        BorderStyle="Solid" BorderPadding="4" BorderWidth="1px" BorderColor="#E6E2D8"
        PasswordLabelText="Old Password:"
        OnChangedPassword="ctlChangePassword_ChangedPassword"
        OnCancelButtonClick="ctlChangePassword_CancelButtonClick"
        ContinueDestinationPageUrl="~/Default.aspx" >
        
        <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="1.15em" ForeColor="White" Height="17px" 
                        BorderWidth="1px" BorderColor="#F7F6F3" Width="320px" HorizontalAlign="Center" />
        <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
        <LabelStyle Width="150px" Height="23px" />
        <TextBoxStyle Width="145px" Height="14px" />
        <CancelButtonStyle Width="75px" />
        <ContinueButtonStyle Width="75px" />
        <SuccessTextStyle Height="30px" />
        
    </asp:ChangePassword>
    
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="650px"></b4f:ErrorLabel>
    
</asp:Content>
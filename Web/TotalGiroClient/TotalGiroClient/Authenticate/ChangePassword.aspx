<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Authenticate.ChangePassword" ViewStateEncryptionMode="Always" Codebehind="ChangePassword.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    
    <asp:ChangePassword ID="ctlChangePassword" runat="server" Width="332px" 
            BorderColor="#AAB9C2" BorderPadding="8" BorderStyle="Solid" BorderWidth="1px" 
            ForeColor="#313457" BackColor="#F3F4F6"
            
            ChangePasswordTitleText="" 
            SuccessTitleText=""
            
            PasswordLabelText="Huidig wachtwoord:" 
            NewPasswordLabelText="Nieuw wachtwoord:" 
            ConfirmNewPasswordLabelText="Bevestig nieuw wachtwoord:"
            
            InstructionText="Het nieuwe wachtwoord heeft een minimale lengte van 8 tekens en het is hoofdletters gevoelig."
            ConfirmPasswordCompareErrorMessage="Het nieuwe wachtwoord en de bevestiging van het nieuwe wachtwoord moeten overeenkomen.<br /><br />"
            ChangePasswordFailureText="Het huidige wachtwoord of het nieuwe wachtwoord is onjuist. Het nieuwe wachtwoord moet minimaal 8 tekens bevatten.<br /><br />"
            SuccessText="Uw wachtwoord is gewijzigd."
            
            ChangePasswordButtonText="Verander wachtwoord" 
            CancelButtonText="Annuleren"
            ContinueButtonText="Doorgaan" 
            
            CancelDestinationPageUrl="~/Portfolio/PortfolioPositions.aspx"
            ContinueDestinationPageUrl="~/Portfolio/PortfolioPositions.aspx" 
            
            OnChangedPassword="ctlChangePassword_ChangedPassword" >
        
        <LabelStyle Width="168px" Height="26px" ForeColor="#313457" />
        <TextBoxStyle Width="145px" Height="16px" Font-Size="1em" />
        
        <InstructionTextStyle Width="320px" Height="25px" ForeColor="#313457" HorizontalAlign="Left" CssClass="padding" />
        <FailureTextStyle Width="320px" CssClass="padding" />
        <SuccessTextStyle Width="320px" CssClass="padding" />
        
        <ChangePasswordButtonStyle Width="166px" CssClass="push-button" />
        <CancelButtonStyle Width="98px" CssClass="push-button" />
        <ContinueButtonStyle Width="98px" CssClass="push-button" />
        
    </asp:ChangePassword>
    
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="650px"></b4f:ErrorLabel>

</asp:Content>
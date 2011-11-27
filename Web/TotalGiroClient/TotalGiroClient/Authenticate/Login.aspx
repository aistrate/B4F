<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" Inherits="B4F.TotalGiro.Client.Web.Authenticate.Login"
         ViewStateEncryptionMode="Always" Codebehind="Login.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    
    <asp:Login ID="ctlLogin" runat="server"
            BorderColor="#AAB9C2" BorderPadding="8" BorderStyle="Solid" BorderWidth="1px"
            ForeColor="#313457" BackColor="#F3F4F6"
            
            OnLoggingIn="ctlLogin_LoggingIn"
            OnLoginError="ctlLogin_LoginError"
            DisplayRememberMe="False"
            
            TitleText=""
            
            UserNameLabelText="Gebruikersnaam:"
            PasswordLabelText="Wachtwoord:"
            
            FailureText="Uw poging tot inloggen is niet geslaagd. Probeert u het alstublieft opnieuw."
            
            LoginButtonText="Inloggen" >
        
        <LabelStyle Width="110px" Height="26px" ForeColor="#313457" />
        <TextBoxStyle Width="145px" Height="16px" Font-Size="1em" />
        
        <FailureTextStyle Width="260px" CssClass="padding" HorizontalAlign="Left" />
        
        <LoginButtonStyle Width="98px" CssClass="push-button" />
        
    </asp:Login>

    <asp:Panel ID="pnlAnnouncement" runat="server" Width="650px" Font-Size="1.15em" Visible="false" ForeColor="Firebrick" >
        <br />
        <p>
            Als u gebruikt maakt van <u>www.mijnpaerel.nl</u> ontvangt u vanaf 1 april 2009 geen fysieke post 
            meer. Uw overzichten en transactienota’s staan op uw persoonlijke pagina op 
            <u>www.mijnpaerel.nl</u>. U ontvangt een e-mail wanneer er nieuwe documenten gereed staan.
        </p>
    </asp:Panel>    
    
    <br /><br />
    <p class="info" style="width: 650px; margin-left: 0px" >
        Indien u uw wachtwoord bent vergeten, klik dan op 
        <asp:LinkButton id="lbtResetPassword" runat="server" Text="aanvragen nieuw wachtwoord" 
                        PostBackUrl="~/Authenticate/ResetPassword.aspx" Visible="true" >
        </asp:LinkButton>.
    </p>
    
</asp:Content>
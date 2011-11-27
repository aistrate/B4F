<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Authenticate.ResetPassword" ViewStateEncryptionMode="Always" Codebehind="ResetPassword.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <asp:Panel ID="pnlHelpText" runat="server" Visible="true">
        <p class="info" style="width: 650px">
            Wij sturen uw nieuwe wachtwoord naar het bij ons bekende e-mailadres.
            <br />
            Uit veiligheidsoverwegingen verzoeken wij u vriendelijk onderstaande gegevens in te vullen.
        </p>
    </asp:Panel>
    
    <table cellpadding="0" cellspacing="0">
        <tr style="height: 10px" >
            <td style="width: 250px"></td>
            <td style="width: 300px"></td>
        </tr>
        <tr>
            <td style="height: 28px; white-space: nowrap">
                <asp:Label ID="Label1" runat="server" Text=" Uw burgerservicenummer (sofinummer):"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:TextBox ID="txtSofiNumber" runat="server" AutoComplete="Off"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSofiNumber" runat="server" ControlToValidate="txtSofiNumber"
                    ErrorMessage="" InitialValue="" SetFocusOnError="True" Width="0px"
                    ValidationGroup="ResetPassword" >*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="height: 28px; white-space: nowrap">
                <asp:Label ID="Label2" runat="server" Text="Uw Paerel rekeningnummer:"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:TextBox ID="txtAccountNumber" runat="server" AutoComplete="Off"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber"
                    ErrorMessage="" InitialValue="" SetFocusOnError="True" Width="0px"
                    ValidationGroup="ResetPassword" >*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <asp:Panel ID="pnlSubmitButton" runat="server" Visible="true">
            <tr style="height:20px" >
                <td colspan="2"></td>
            </tr>
            <tr>
                <td colspan="2" style="height: 28px; padding-left: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="Verzend" Width="98px" OnClick="btnSubmit_Click"
                                ValidationGroup="ResetPassword" CausesValidation="true" />
                </td>
            </tr>
        </asp:Panel>
    </table>
    
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="650px"></b4f:ErrorLabel>
    <br />
    <span class="padding" style="display: block; color: WindowText">
        <asp:LinkButton ID="lnkLogin" runat="server" Text="Inloggen" Visible="false"
                        PostBackUrl="~/Authenticate/Login.aspx"></asp:LinkButton>
        <asp:ValidationSummary ID="vsValSummary" runat="server" Width="650px" Height="0px" ValidationGroup="ResetPassword"
                               HeaderText="Zowel burgerservicenummer als rekeningnummer moeten worden ingevuld." />
    </span>
    
</asp:Content>


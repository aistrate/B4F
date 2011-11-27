<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
         CodeFile="UserInsert.aspx.cs" Inherits="UserInsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table cellpadding="0" cellspacing="0" style="border: Solid 1px Black">
        <tr>
            <td class="tblHeader">New User</td>
        </tr>
        <tr>
            <td>
            
                <asp:CreateUserWizard ID="wizCreateUser" runat="server"
                    ActiveStepIndex="0"
                    ContinueDestinationPageUrl="~/Admin/UserOverview.aspx" 
                    DisableCreatedUser="True" ContinueButtonText="Activate"
                    CompleteSuccessText="User account has been successfully created.<br/>Should it be activated?<br/><br/>(Upon activation, an e-mail notification will be sent to user,<br/>containing their username and password.)<br/><br/><br/>"
                    UnknownErrorMessage="User account could not be created."
                    OnActiveStepChanged="wizCreateUser_ActiveStepChanged"
                    OnContinueButtonClick="wizCreateUser_ContinueButtonClick" >
                    
                    <WizardSteps>
                    
                        <asp:CreateUserWizardStep runat="server">
                            <ContentTemplate>
                                <table border="0">
                                    <tr>
                                        <td align="center" colspan="2" style="height: 12px"></td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 130px; height: 20px">
                                            <label for="UserName">User Name:</label>
                                        </td>
                                        <td style="width: 200px">
                                            <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="valUserNameRequired" runat="server"
                                                ControlToValidate="UserName"
                                                ErrorMessage="User Name is required." ToolTip="User Name is required."
                                                ValidationGroup="wizCreateUser">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="height: 20px">
                                            <label for="Password">Password:</label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="valPasswordRequired" runat="server"
                                                ControlToValidate="Password"
                                                ErrorMessage="Password is required." ToolTip="Password is required."
                                                ValidationGroup="wizCreateUser">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="height: 20px">
                                            <label for="ConfirmPassword">Confirm Password:</label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="valConfirmPasswordRequired" runat="server"
                                                ControlToValidate="ConfirmPassword"
                                                ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                                                ValidationGroup="wizCreateUser">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="height: 20px">
                                            <label for="Email">E-mail:</label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="valEmailRequired" runat="server"
                                                ControlToValidate="Email"
                                                ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                                                ValidationGroup="wizCreateUser">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="2" style="height: 20px">
                                            <asp:CompareValidator ID="valPasswordCompare" runat="server" ControlToCompare="Password"
                                                ControlToValidate="ConfirmPassword" Display="Dynamic"
                                                ErrorMessage="The Password and Confirmation Password must match."
                                                ValidationGroup="wizCreateUser"></asp:CompareValidator>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <CustomNavigationTemplate>
                                <table border="0" cellspacing="5" style="width: 100%; height: 100%;">
                                    <tr align="right">
                                        <td align="right" colspan="0">
                                            <asp:Button ID="btnCreateUser" runat="server" CommandName="MoveNext" Text="Create User"
                                                        ValidationGroup="wizCreateUser" />
                                        </td>
                                    </tr>
                                </table>
                            </CustomNavigationTemplate>
                        </asp:CreateUserWizardStep>
                        
                        <asp:TemplatedWizardStep runat="server" Title="Set Login Type">
                            <ContentTemplate>
                                <table border="0">
                                    <tr>
                                        <td align="center" colspan="3" style="height: 12px"></td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 130px; height: 20px; vertical-align: middle">
                                            <asp:Label ID="Label1" runat="server" Text="Login Type:"></asp:Label>
                                        </td>
                                        <td style="width: 330px; vertical-align: top">
                                            <asp:RadioButtonList ID="rblLoginType" runat="server" RepeatDirection="Horizontal" Width="320px" 
                                                AutoPostBack="True" OnSelectedIndexChanged="rblLoginType_SelectedIndexChanged">
                                                <asp:ListItem>Asset Manager</asp:ListItem>
                                                <asp:ListItem>Stichting</asp:ListItem>
                                                <asp:ListItem>Compliance</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td style="width: 20px">
                                            <asp:RequiredFieldValidator ID="valLoginTypeRequired" runat="server"
                                                ControlToValidate="rblLoginType"
                                                ErrorMessage="Login Type is required." ToolTip="Login Type is required." 
                                                ValidationGroup="wizCreateUser">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr style="height: 0px">
                                        <td align="right">
                                            <asp:Label ID="lblAssetManager" runat="server" Text="Asset Manager:" Visible="False"></asp:Label>
                                        <td>
                                            <asp:DropDownList ID="ddlAssetManager" runat="server" DataSourceID="odsAssetManager"
                                                DataTextField="CompanyName" DataValueField="Key" Visible="False">
                                            </asp:DropDownList>
                                            <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                                                TypeName="B4F.TotalGiro.ApplicationLayer.Admin.UserInsertAdapter"></asp:ObjectDataSource>
                                        </td>
                                        <td>
                                            <asp:RequiredFieldValidator ID="valAssetManagerRequired" runat="server"
                                                ControlToValidate="ddlAssetManager"
                                                ErrorMessage="Asset Manager is required." ToolTip="Asset Manager is required." 
                                                ValidationGroup="wizCreateUser" Visible="false"
                                                InitialValue="-2147483648">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <CustomNavigationTemplate>
                                <table border="0" cellspacing="5" style="width: 100%; height: 100%;">
                                    <tr align="right">
                                        <td align="right" colspan="0">
                                            <asp:Button ID="btnSetLoginType" runat="server" CommandName="MoveNext" Text="Set Login Type"
                                                        ValidationGroup="wizCreateUser" OnClick="btnSetLoginType_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </CustomNavigationTemplate>
                        </asp:TemplatedWizardStep>
                        
                        <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                        </asp:CompleteWizardStep>
                        
                    </WizardSteps>
                    
                </asp:CreateUserWizard>
                
            </td>
        </tr>
    </table>
    
    <br />
    &nbsp;<asp:Button ID="btnBack" runat="server" Text="Users Overview" PostBackUrl="~/Admin/UserOverview.aspx"
                      UseSubmitBehavior="False" />
    
    <br />
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px" Font-Bold="false"></b4f:ErrorLabel>
    
</asp:Content>
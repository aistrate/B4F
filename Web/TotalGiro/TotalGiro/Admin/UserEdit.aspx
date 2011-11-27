<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="UserEdit.aspx.cs" Inherits="UserEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table style="width: 600px" cellpadding="0" cellspacing="0" border="1" bordercolor="black">
        <tr>
            <td class="tblHeader" colspan="2">User</td>
        </tr>
        <tr style="height: 24px">
            <td style="width: 130px">
                <asp:Label ID="Label1" runat="server" Text="User Name:"></asp:Label>
            </td>
            <td style="width: 470px">
                <asp:Label ID="lblUserName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr style="height: 24px">
            <td>
                <asp:Label ID="Label2" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtEmail" runat="server" Width="350px"></asp:TextBox>&nbsp;
                <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail"
                    SetFocusOnError="true">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="rexEmail" runat="server" ControlToValidate="txtEmail" SetFocusOnError="true" 
                    ValidationExpression="\s*\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\s*">invalid e-mail format</asp:RegularExpressionValidator>    
            </td>
        </tr>
        <tr style="height: 24px">
            <td>
                <asp:Label ID="Label3" runat="server" Text="Description:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtDescription" runat="server" Width="350px"></asp:TextBox>
            </td>
        </tr>
        <tr style="height: 24px">
            <td/>
            <td>
                <asp:CheckBox ID="cbActive" runat="server" Text="Active" />
            </td>
        </tr>
        <tr style="height: 5px">
            <td/>
            <td/>
        </tr>
        <tr>
            <td style="vertical-align: top" bgcolor="gainsboro">
                <asp:Label ID="Label4" runat="server" Text="Roles:" Font-Bold="True"></asp:Label>
            </td>
            <td style="white-space: nowrap" bgcolor="gainsboro">
                <asp:Panel ID="pnlRoles" runat="server" Width="350px">
                </asp:Panel>
            </td>
        </tr>
    </table>
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px" Font-Bold="true"></b4f:ErrorLabel>
    
    <table style="width: 600px" cellpadding="0" cellspacing="0">
        <tr style="height: 9px">
            <td colspan="2"></td>
        </tr>
        <tr>
            <td style="width: 350px">
                <asp:Button ID="btnSave" runat="server" Text="Save" Width="85px" OnClick="btnSave_Click" />&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="85px" OnClick="btnCancel_Click" CausesValidation="False" />
            </td>
            <td style="width: 150px; text-align: right">
                <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" Width="140px" Visible="false"
                    OnClick="btnResetPassword_Click" />
            </td>
        </tr>
    </table>
    
</asp:Content>

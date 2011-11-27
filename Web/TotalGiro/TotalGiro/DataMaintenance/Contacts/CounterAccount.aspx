<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="CounterAccount.aspx.cs" Inherits="DataMaintenance_CounterAccount" Title="CounterAccount" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton"
    TagPrefix="uc2" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/Address.ascx" TagName="Address" TagPrefix="ucAddress" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="cmg" runat="server" />
    <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:" ID="valSum" runat="server" />
    <asp:HiddenField ID="hfContactID" runat="server" />
    <asp:HiddenField ID="hfCounterAccountID" runat="server" />
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
    <asp:Panel ID="pnlErrorMess" runat="server" Visible="false">
        <asp:Label ID="lblMess" Font-Bold="true" runat="server" />
    </asp:Panel>
    <table style="border: solid 1px black;"  cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="6" style="height: 30px; padding-left: 5px; background-color: #AAB9C2; border-bottom: solid 1px black;">
                <asp:Label ID="lblAccount" Font-Bold="true" runat="server" Text="Counter Account Details" />
            </td>
        </tr>
        <tr><td colspan="6"></td></tr>
        <tr>
            <td style="text-align: right">
                <asp:Label ID="lblTegenrekeningBankLabel" runat="server" Text="Bank:"></asp:Label>
            </td>
            <td></td>
            <td>
                <asp:MultiView ID="mvwBank" runat="server" ActiveViewIndex="0" >
                    <asp:View ID="vwKnownBank" runat="server" >
                        <asp:DropDownList 
                            ID="ddlBank" DataSourceID="odsBank" DataTextField="Name" DataValueField="Key" 
                            runat="server" onselectedindexchanged="ddlBank_SelectedIndexChanged" AutoPostBack="true" /> 
                        <asp:ObjectDataSource ID="odsBank" runat="server" SelectMethod="GetBanks"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.CounterAccountEditAdapter">
                        </asp:ObjectDataSource>
                    </asp:View>
                    <asp:View ID="vwUnknownBank" runat="server">
                        <asp:TextBox ID="tbTegenrekNameBank" MaxLength="100" runat="server"></asp:TextBox>
                    </asp:View>
                </asp:MultiView>
            </td>
            <td>
                <asp:MultiView ID="mvwBankValidator" runat="server" ActiveViewIndex="0" >
                    <asp:View ID="vwKnownBankValidator" runat="server" >
                        <asp:RequiredFieldValidator 
                            ID="rfvBank" 
                            runat="server" 
                            ControlToValidate="ddlBank"
                            SetFocusOnError="True"
                            InitialValue="-2147483648" 
                            ErrorMessage="Bank is mandatory">*</asp:RequiredFieldValidator>
                    </asp:View>
                    <asp:View ID="vwUnknownBankValidator" runat="server" >
                        <asp:RequiredFieldValidator 
                            ID="rfTegenrekNameBank" 
                            ControlToValidate=tbTegenrekNameBank
                            runat="server"
                            Enabled="false" 
                            ErrorMessage="Bank is mandatory">*</asp:RequiredFieldValidator>
                    </asp:View>
                </asp:MultiView>
            </td>
            <td style="width: 40px">
                <asp:RadioButtonList ID="rblBankChoice" runat="server" RepeatDirection="Horizontal" Width="225px" 
                    AutoPostBack="True" OnSelectedIndexChanged="rblBankChoice_SelectedIndexChanged">
                    <asp:ListItem Selected="True" >Known bank</asp:ListItem>
                    <asp:ListItem>Unknown bank</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td style="width: 575px">
            </td>
        </tr>
        <tr>
            <td style="height: 26px; text-align: right">
                <asp:Label ID="lblTegenrekeningNumberLabel" runat="server" Text="Number:"></asp:Label>
            </td>
            <td style="width: 25px"></td>
            <td width="30px">
                <asp:TextBox ID="tbTegenrekNR" MaxLength="15" runat="server" />
            </td>
            <td style="width: 10px">
                <asp:RequiredFieldValidator 
                    ID="rfTegenrekNR" 
                    ControlToValidate=tbTegenrekNR
                    runat="server" 
                    ErrorMessage="Account Number is mandatory">*</asp:RequiredFieldValidator>
                <asp:CustomValidator 
                    ID="cvTegenrekNr" 
                    runat="server"
                    ControlToValidate=tbTegenrekNR
                    OnServerValidate="cvTegenrekNr_ServerValidate"
                    ValidateEmptyText="false"
                    ErrorMessage="Account Number is not valid">*</asp:CustomValidator>
            </td>
            <td>
                <asp:CheckBox ID="chkUseElfProef" runat="server" Text="Use Elf proef?" Checked="true" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td style="height: 25px; text-align: right">
                <asp:Label ID="lblTegenrekeningTnvLabel" runat="server" Text="Name:"></asp:Label>
            </td>
            <td></td>
            <td>
                <asp:TextBox ID="tbTegenrekTNV" MaxLength="100" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator 
                    ID="rfTegenrekTNV" 
                    ControlToValidate=tbTegenrekTNV
                    runat="server" 
                    ErrorMessage="Account Name is mandatory">*</asp:RequiredFieldValidator>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
            <td colspan="4">
                <ucAddress:Address ID="ucBankAddress" runat="server" ShowPostalAddress="false" CaptionResidentialAddress="Bank Address" DataCheckingEnabled="false" />
            </td>
        </tr>
        <tr><td colspan="6"></td></tr>
        <tr>
            <td colspan="2"></td>
            <td>
                <asp:CheckBox ID="chkIsPublic" runat="server" Text="Is public" 
                    AutoPostBack="True" oncheckedchanged="chkIsPublic_CheckedChanged" />
            </td>
            <td colspan="3"></td>
        </tr>
        <asp:Panel ID="pnlBeneficiaryAddress" runat="server" Visible="false" >
            <tr>
                <td colspan="2">
                </td>
                <td colspan="4">
                    <ucAddress:Address ID="ucBeneficiaryAddress" runat="server" ShowPostalAddress="false" CaptionResidentialAddress="Beneficiary Address" DataCheckingEnabled="false" />
                </td>
            </tr>
        </asp:Panel>
        <tr><td colspan="6"></td></tr>
    </table>
    <br />
    <asp:Button ID="btnSaveAccount" runat="server" OnClick="btnSaveAccount_Click" Text="Save Changes" />&nbsp;&nbsp;
    <br />
    <br />
    <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton1" runat="server" />
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="PredefinedBeneficiaries.aspx.cs"
    Inherits="PredefinedBeneficiaries" Title="Predefined Beneficiaries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hfPredefinedBeneficiaryKey" runat="server" />
    <table border="0" cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td width="100%">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnlErrorMess" runat="server" Visible="false">
            <tr>
                <td width="100%" style="color: Red; height: 30px;">
                    <asp:Label ID="lblMess" Font-Bold="true" runat="server" />
                </td>
            </tr>
        </asp:Panel>
    </table>
    <br />
    <asp:GridView ID="gvPredefinedBeneficiary" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" Caption="Predefined Beneficiaries" CaptionAlign="Left"
        DataKeyNames="Key" DataSourceID="odsPredefinedBeneficiary" OnSelectedIndexChanged="gvPredefinedBeneficiary_SelectedIndexChanged"
        PageSize="20" SkinID="custom-width" Width="1000px">
        <SelectedRowStyle BackColor="Beige" />
        <Columns>
            <asp:BoundField DataField="BenefBankAcctNr" HeaderText="Bank Acct Nr" SortExpression="BenefBankAcctNr">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NarBenef1" HeaderText="NarBenef1" SortExpression="NarBenef1">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NarBenef2" HeaderText="NarBenef2" SortExpression="NarBenef2">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Description1" HeaderText="Description1" SortExpression="Description1">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Description2" HeaderText="Description2" SortExpression="Description2">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
            </asp:BoundField>
            <asp:CommandField SelectText="Choose" ShowSelectButton="True" />
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsPredefinedBeneficiary" runat="server" SelectMethod="GetPredefinedBeneficiaries"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.PredefinedBeneficiariesAdapter">
    </asp:ObjectDataSource>
    <br />
    <asp:UpdatePanel ID="pnlEditDetails" runat="server" UpdateMode="Conditional" Visible="false">
        <%--        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gvPredefinedBeneficiary" EventName="SelectedIndexChanged" />
        </Triggers>--%>
        <ContentTemplate>
            <table border="1" cellpadding="0" cellspacing="0" width="1000px">
                <tr>
                    <td colspan="20" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                        border-bottom: solid 1px black;">
                        <asp:Label ID="lblTransferDetails" Font-Bold="true" runat="server" Text="Edit Details" />
                    </td>
                </tr>
                <tr>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                    <td width="5%">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: right;">
                        <asp:Label ID="lblSwiftAddress" runat="server" Text="Swift Address"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbSwiftAddress" MaxLength="11" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="0"></asp:TextBox>
                    </td>
                    <td colspan="1">
                    </td>
                    <td colspan="3" style="text-align: right;">
                        <asp:Label ID="lblCostIndication" runat="server" Text="Cost Indication"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:DropDownList ID="ddlCostIndication" runat="server" DataTextField="Description" DataValueField="Key"
                            DataSourceID="odsCostIndication" SkinID="custom-width" Width="250px">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsCostIndication" runat="server" SelectMethod="GetCostIndications"
                            TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.PredefinedBeneficiariesAdapter">
                        </asp:ObjectDataSource>                        
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: right;">
                        <asp:Label ID="lblBenefBankAcctNr" runat="server" Text="Bank Acct Nr"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbBenefBankAcctNr" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="0" AutoPostBack="true" OnTextChanged="tbBenefBankAcctNr_TextChanged"></asp:TextBox>
                    </td>
                    <td colspan="1">
                        <asp:RequiredFieldValidator ID="rfvBenefBankAcctNr" runat="server" ControlToValidate="tbBenefBankAcctNr"
                            SetFocusOnError="True" ErrorMessage="Bank Acct Nr is mandatory">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="customValIbanNummer" ErrorMessage="If Swift Address is provided , then a Valid Iban Number is Compulsory."
                            ControlToValidate="tbBenefBankAcctNr" runat="server" OnServerValidate="customValIbanNummer_ServerValidate">*</asp:CustomValidator>
                    </td>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: right;">
                        <asp:Label ID="lblNarBenef1" runat="server" Text="Beneficiary NAR"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbNarBenef1" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="1"></asp:TextBox>
                    </td>
                    <td colspan="1">
                        <asp:RequiredFieldValidator ID="rfvNarBenef1" runat="server" ControlToValidate="tbNarBenef1"
                            SetFocusOnError="True" ErrorMessage="NarBenef1 is mandatory">*</asp:RequiredFieldValidator>
                    </td>
                    <td colspan="3" style="text-align: right;">
                        <asp:Label ID="lblDescription1" runat="server" Text="Description"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbDescription1" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="5"></asp:TextBox>
                    </td>
                    <td colspan="1">
                        <asp:RequiredFieldValidator ID="rfvDescription1" runat="server" ControlToValidate="tbDescription1"
                            SetFocusOnError="True" ErrorMessage="Description1 is mandatory">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbNarBenef2" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="2"></asp:TextBox>
                    </td>
                    <td colspan="1">
                        <asp:RequiredFieldValidator ID="rfvNarBenef2" runat="server" ControlToValidate="tbNarBenef2"
                            SetFocusOnError="True" ErrorMessage="NarBenef2 is mandatory">*</asp:RequiredFieldValidator>
                    </td>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbDescription2" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="6"></asp:TextBox>
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbNarBenef3" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="3"></asp:TextBox>
                    </td>
                    <td colspan="1">
                    </td>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbDescription3" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="7"></asp:TextBox>
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbNarBenef4" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="4"></asp:TextBox>
                    </td>
                    <td colspan="1">
                    </td>
                    <td colspan="3" style="text-align: right;">
                    </td>
                    <td>
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="tbDescription4" MaxLength="35" runat="server" SkinID="custom-width"
                            Width="250px" TabIndex="8"></asp:TextBox>
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <asp:Button ID="btnAddNew" runat="server" Text="Add Record" CausesValidation="False"
        OnClick="btnAddNew_Click" />
    <asp:Button ID="btnSave" runat="server" Text="Save Record" CausesValidation="False"
        Visible="false" OnClick="btnSave_Click" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel Edit" CausesValidation="False"
        Visible="false" OnClick="btnCancel_Click" />
</asp:Content>

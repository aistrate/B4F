<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="MoneyTransferOrder.aspx.cs"
    Inherits="MoneyTransferOrder" Title="External Money Transfer Order"
    Theme="Neutral" %>

<%@ Register Src="../../UC/CounterAccountFinder.ascx" TagName="CounterAccountFinder"
    TagPrefix="uc1" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="dp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hfMoneyTransferOrderID" runat="server" />
    <asp:HiddenField ID="hfAccountid" runat="server" />
    <asp:HiddenField ID="hfCounterAccountID" runat="server" />
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
    <table border="0" cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td>
                <table border="1" cellpadding="0" cellspacing="0" width="1000px">
                    <tr>
                        <td colspan="7" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                            border-bottom: solid 1px black;">
                            <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Stichting Debet Account" />
                        </td>
                    </tr>
                    <tr>
                        <td width="5%">
                        </td>
                        <td width="30%" style="text-align: center;">
                        </td>
                        <td width="10%">
                        </td>
                        <td width="20%" style="text-align: center;">
                            <asp:Label ID="lblStichtingDebetAccount" runat="server" Text="Account NAR Details"></asp:Label>
                        </td>
                        <td width="2%">
                        </td>
                        <td width="31%" style="text-align: justify;">
                            <asp:TextBox MaxLength="35" ID="tbNarDebet1" runat="server" SkinID="broad"></asp:TextBox>
                        </td>
                        <td width="2%">
                        </td>
                    </tr>
                    <tr>
                        <td width="5%">
                            <asp:CustomValidator ID="customValAccountNumbers" ErrorMessage="The Beneficiary number may not be the same as the Debet number"
                                ControlToValidate="ddlWithdrawJournal" runat="server" OnServerValidate="customValAccountNumbers_ServerValidate">*</asp:CustomValidator>
                        </td>
                        <td width="30%" style="text-align: center;">
                            <asp:DropDownList ID="ddlWithdrawJournal" runat="server" Width="200px" AutoPostBack="true"
                                DataTextField="BankAccountNumber" DataValueField="Key" DataSourceID="odsWithdrawJournal"
                                OnSelectedIndexChanged="ddlWithdrawJournal_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsWithdrawJournal" runat="server" SelectMethod="GetWithdrawalJournals"
                                TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.MoneyTransferOrderAdapter">
                            </asp:ObjectDataSource>
                        </td>
                        <td width="10%">
                        </td>
                        <td width="20%" style="text-align: center;">
                        </td>
                        <td width="2%">
                        </td>
                        <td width="31%" style="text-align: justify;">
                            <asp:TextBox MaxLength="35" ID="tbNarDebet2" runat="server" SkinID="broad"></asp:TextBox>
                        </td>
                        <td width="2%">
                        </td>
                    </tr>
                    <tr>
                        <td width="5%">
                        </td>
                        <td width="30%" style="text-align: center;">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlWithdrawJournal" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:Label ID="lblDebetAcctBank" runat="server" SkinID="broad"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td width="10%">
                        </td>
                        <td width="20%" style="text-align: center;">
                        </td>
                        <td width="2%">
                        </td>
                        <td width="31%" style="text-align: justify;">
                            <asp:TextBox MaxLength="35" ID="tbNarDebet3" runat="server" SkinID="broad"></asp:TextBox>
                        </td>
                        <td width="2%">
                        </td>
                    </tr>
                    <tr>
                        <td width="5%">
                        </td>
                        <td width="30%" style="text-align: center;">
                            <asp:Label ID="lblDebetAcctDetails" runat="server" SkinID="broad"></asp:Label>
                        </td>
                        <td width="10%">
                        </td>
                        <td width="20%" style="text-align: center;">
                        </td>
                        <td width="2%">
                        </td>
                        <td width="31%" style="text-align: justify;">
                            <asp:TextBox MaxLength="35" ID="tbNarDebet4" runat="server" SkinID="broad"></asp:TextBox>
                        </td>
                        <td width="2%">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="20">
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="updPnlUcChooseBeneficiary" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table border="1" cellpadding="0" cellspacing="0" width="1000px">
                            <tr>
                                <td colspan="20" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                                    border-bottom: solid 1px black;">
                                    <asp:Label ID="Label2" Font-Bold="true" runat="server" Text="Choose Beneficiary" />
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
                                <td colspan="2">
                                </td>
                                <td colspan="6" style="text-align: center;">
                                    <asp:Button ID="btnPredefinedBeneficiary" runat="server" Enabled="true" CausesValidation="false"
                                        Text="Choose Predefined Beneficiary" OnClick="btnPredefinedBeneficiary_Click" />
                                </td>
                                <td colspan="2">
                                </td>
                                <td colspan="2">
                                </td>
                                <td colspan="6" style="text-align: center;">
                                    <asp:Button ID="btnAccountCounterAccount" runat="server" Enabled="true" CausesValidation="false"
                                        Text="Choose Account CounterAccount" Visible="false" OnClick="btnAccountCounterAccount_Click" />
                                </td>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="20">
                                    <asp:MultiView ID="mvwBeneficiarySelect" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                        <asp:View ID="vwePredefinedBeneficiary" runat="server">
                                            <asp:GridView ID="gvPredefinedBeneficiary" runat="server" AllowPaging="True" AllowSorting="True"
                                                AutoGenerateColumns="False" Caption="Predefined Beneficiaries" CaptionAlign="Left"
                                                DataKeyNames="Key" DataSourceID="odsPredefinedBeneficiary" OnSelectedIndexChanged="gvPredefinedBeneficiary_SelectedIndexChanged"
                                                PageSize="25" SkinID="custom-width" Width="100%">
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
                                                    <asp:BoundField DataField="NarBenef3" HeaderText="NarBenef3" SortExpression="NarBenef3">
                                                        <ItemStyle Wrap="False" />
                                                        <HeaderStyle HorizontalAlign="Center" Wrap="False" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Description1" HeaderText="Description1" SortExpression="Description1">
                                                        <ItemStyle Wrap="False" />
                                                        <HeaderStyle HorizontalAlign="Center" Wrap="False" />
                                                    </asp:BoundField>
                                                    <asp:CommandField SelectText="Choose" ShowSelectButton="True" />
                                                </Columns>
                                            </asp:GridView>
                                            <asp:ObjectDataSource ID="odsPredefinedBeneficiary" runat="server" SelectMethod="GetPredefinedBeneficiaries"
                                                TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.MoneyTransferOrderAdapter">
                                            </asp:ObjectDataSource>
                                        </asp:View>
                                        <asp:View ID="vweAccountCounterAccount" runat="server">
                                            <table cellpadding="0" cellspacing="0" width="1000px">
                                                <tr>
                                                    <td colspan="6" rowspan="2">
                                                        <uc1:CounterAccountFinder ID="ctlAccountFinder" runat="server" ShowContactActiveCbl="true"
                                                            ShowContactName="true" Visible="true" />
                                                        <br />
                                                    </td>
                                                    <td colspan="14" rowspan="1">
                                                        <asp:Panel ID="pnlAccounts" runat="server" Visible="false">
                                                            <asp:GridView ID="gvAccounts" runat="server" AllowPaging="True" AllowSorting="True"
                                                                AutoGenerateColumns="False" Caption="Accounts" CaptionAlign="Left" DataKeyNames="Key"
                                                                DataSourceID="odsAccounts" OnRowCommand="gvAccounts_RowCommand" SkinID="custom-width"
                                                                Width="500px">
                                                                <Columns>
                                                                    <asp:BoundField DataField="BankName" HeaderText="Bank" ReadOnly="True" SortExpression="BankName">
                                                                        <ItemStyle Wrap="False" />
                                                                        <HeaderStyle Wrap="False" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Number" HeaderText="Number" ReadOnly="True" SortExpression="Number">
                                                                        <ItemStyle Wrap="False" />
                                                                        <HeaderStyle Wrap="False" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="AccountName" HeaderText="Account Name" ReadOnly="True"
                                                                        SortExpression="AccountName">
                                                                        <ItemStyle Wrap="False" />
                                                                        <HeaderStyle Wrap="False" />
                                                                    </asp:BoundField>
                                                                    <asp:CommandField ShowSelectButton="True" Visible="true" />
                                                                </Columns>
                                                            </asp:GridView>
                                                            <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCounterAccounts"
                                                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.CounterAccountFinderAdapter">
                                                                <SelectParameters>
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                                                        Type="Int32" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="counterAccountNumber" PropertyName="CounterAccountNumber"
                                                                        Type="String" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="counterAccountName" PropertyName="CounterAccountName"
                                                                        Type="String" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactName" PropertyName="ContactName"
                                                                        Type="String" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                                                        Type="String" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                                                                        Type="Boolean" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                                                                        Type="Boolean" />
                                                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="isPublic" PropertyName="IsPublic"
                                                                        Type="Boolean" />
                                                                    <asp:Parameter DefaultValue="Key, BankName, Number, AccountName, IsPublic" Name="propertyList"
                                                                        Type="String" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:View>
                                    </asp:MultiView>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="20">
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvPredefinedBeneficiary" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <table border="1" cellpadding="0" cellspacing="0" width="1000px">
                            <tr>
                                <td colspan="20" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                                    border-bottom: solid 1px black;">
                                    <asp:Label ID="lblTransferDetails" Font-Bold="true" runat="server" Text="Transfer Details" />
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
                                    <asp:DropDownList ID="ddlCostIndication" runat="server" DataTextField="Description"
                                        DataValueField="Key" DataSourceID="odsCostIndication" SkinID="custom-width" Width="250px">
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
                                    <asp:CustomValidator ID="customValBankAccountNumbers" ErrorMessage="The Beneficiary account number does not appear to be a Valid Dutch Bank account Number"
                                        ControlToValidate="tbBenefBankAcctNr" runat="server" OnServerValidate="customValBankAccountNumbers_ServerValidate">*</asp:CustomValidator>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="tbBenefBankAcctNr" MaxLength="35" runat="server" SkinID="custom-width"
                                        Width="250px"></asp:TextBox>
                                </td>
                                <td colspan="1">
                                </td>
                                <td colspan="3" style="text-align: right;">
                                    <asp:Label ID="lblReference" runat="server" Text="Reference Nr."></asp:Label>
                                </td>
                                <td>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="tbReference" Font-Bold="true" ReadOnly="true" MaxLength="16" runat="server"
                                        SkinID="custom-width" Width="250px"></asp:TextBox>
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
                                        Width="250px"></asp:TextBox>
                                </td>
                                <td colspan="1">
                                </td>
                                <td colspan="3" style="text-align: right;">
                                    <asp:Label ID="lblDescription1" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="tbDescription1" MaxLength="35" runat="server" SkinID="custom-width"
                                        Width="250px"></asp:TextBox>
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
                                    <asp:TextBox ID="tbNarBenef2" MaxLength="35" runat="server" SkinID="custom-width"
                                        Width="250px"></asp:TextBox>
                                </td>
                                <td colspan="1">
                                </td>
                                <td colspan="3" style="text-align: right;">
                                </td>
                                <td>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="tbDescription2" MaxLength="35" runat="server" SkinID="custom-width"
                                        Width="250px"></asp:TextBox>
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
                                        Width="250px"></asp:TextBox>
                                </td>
                                <td colspan="1">
                                </td>
                                <td colspan="3" style="text-align: right;">
                                </td>
                                <td>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="tbDescription3" MaxLength="35" runat="server" SkinID="custom-width"
                                        Width="250px"></asp:TextBox>
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
                                        Width="250px"></asp:TextBox>
                                </td>
                                <td colspan="1">
                                </td>
                                <td colspan="3" style="text-align: right;">
                                </td>
                                <td>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="tbDescription4" MaxLength="35" runat="server" SkinID="custom-width"
                                        Width="250px"></asp:TextBox>
                                </td>
                                <td colspan="1">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align: right;">
                                    <asp:Label ID="lblAmount" runat="server" Text="Amount"></asp:Label>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="rfvAmount" ControlToValidate="tbAmount" runat="server"
                                        Text="*" ErrorMessage="Amount is mandatory" />
                                    <asp:RegularExpressionValidator ID="revAmount" runat="server" ControlToValidate="tbAmount"
                                        Text="*" ErrorMessage="Formaat: ####,##" ValidationExpression="^(-)?\d+(\,\d{1,2})?$"></asp:RegularExpressionValidator>
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="tbAmount" runat="server" SkinID="custom-width" Width="150px"></asp:TextBox>
                                </td>
                                <td colspan="2" style="text-align: right;">
                                    <asp:DropDownList ID="ddlCurrency" runat="server" SkinID="custom-width" Width="75px"
                                        AutoPostBack="true" DataTextField="Symbol" DataValueField="Key" DataSourceID="odsCurrencies">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsCurrencies" runat="server" SelectMethod="GetCurrencies"
                                        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.MoneyTransferOrderAdapter">
                                    </asp:ObjectDataSource>
                                </td>
                                <td>
                                </td>
                                <td colspan="3" style="text-align: right;">
                                    <asp:Label ID="lblProcessDate" runat="server" Text="Process Date"></asp:Label>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="reqProcessDate" ErrorMessage="Process Date" ControlToValidate="ucProcessDate:txtDate"
                                        runat="server">*</asp:RequiredFieldValidator>
                                </td>
                                <td colspan="5">
                                    <dp:DatePicker ID="ucProcessDate" ListYearsBeforeCurrent="11" ListYearsAfterCurrent="1"
                                        runat="server" />
                                </td>
                                <td colspan="1">
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btnSave" runat="server" Enabled="true" Visible="false" CausesValidation="true" Text="Save Order"
        OnClick="btnSave_Click" />
    <uc2:BackButton ID="ctlBackButton" runat="server" />
    <br />
    <asp:Panel ID="pnlDialog" runat="server" Visible="false" >
        <asp:Label ID="lblMess2" Font-Bold="true" runat="server" ForeColor="Red" />
        <asp:RadioButtonList ID="rblDialog" runat="server" RepeatDirection="Horizontal" Width="120px" 
            AutoPostBack="true" OnSelectedIndexChanged="rblDialog_SelectedIndexChanged" >
            <asp:ListItem Text="Yes" Value="1" />
            <asp:ListItem Text="No" Value="0" />
        </asp:RadioButtonList>
    </asp:Panel>
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>

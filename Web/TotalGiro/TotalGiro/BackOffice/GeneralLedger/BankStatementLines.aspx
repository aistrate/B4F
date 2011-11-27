<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="BankStatementLines.aspx.cs" Inherits="BankStatementLines" Title="Bank Statement Lines" 
    Theme="Neutral" %>

<%@ Register Src="~/UC/JournalEntryLines.ascx" TagName="JournalEntryLines" TagPrefix="uc1" %>
<%@ Register Src="~/UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table style="width: 910px">
        <tr>
            <td style="width: 145px; height: 24px">
                <asp:Label ID="Label1" runat="server" Text="Journal Entry Number:"></asp:Label></td>
            <td style="width: 370px">
                <asp:Label ID="lblJournalEntryNumber" runat="server" Font-Bold="True"></asp:Label>
                <asp:HiddenField ID="hdnJournalEntryId" runat="server" />
            </td>
            <td style="width: 150px">
                <asp:Label ID="Label2" runat="server" Text="Starting Balance Date:"></asp:Label></td>
            <td style="width: 245px">
                <asp:Label ID="lblStartingBalanceDate" runat="server" Font-Bold="True"></asp:Label></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label3" runat="server" Text="Status:"></asp:Label></td>
            <td>
                <asp:Label ID="lblStatus" runat="server" Font-Bold="True"></asp:Label></td>
            <td>
                <asp:Label ID="Label4" runat="server" Text="Starting Balance:"></asp:Label></td>
            <td>
                <asp:Label ID="lblStartingBalance" runat="server" Font-Bold="True"></asp:Label></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label5" runat="server" Text="Journal:"></asp:Label></td>
            <td>
                <asp:Label ID="lblJournal" runat="server" Font-Bold="True"></asp:Label></td>
            <td>
                <asp:Label ID="Label6" runat="server" Text="Transaction Date:"></asp:Label></td>
            <td>
                <asp:Label ID="lblTransactionDate" runat="server" Font-Bold="True"></asp:Label></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label7" runat="server" Text="Open Amount:"></asp:Label></td>
            <td>
                <asp:Label ID="lblOpenAmount" runat="server" Font-Bold="True"></asp:Label></td>
            <td>
                <asp:Label ID="Label8" runat="server" Text="Closing Balance:"></asp:Label></td>
            <td>
                <asp:Label ID="lblClosingBalance" runat="server" Font-Bold="True"></asp:Label></td>
        </tr>
    </table>
    <br />
    <uc1:JournalEntryLines ID="ctlJournalEntryLines" runat="server" />
    <br />
    <table width="1061px">
        <tr>
            <td style="width:300px">
                <asp:Button ID="btnJournals" runat="server" Text="Journals" CausesValidation="False" 
                            PostBackUrl="~/BackOffice/GeneralLedger/BankStatementJournals.aspx" />&nbsp;
                <asp:Button ID="btnStatements" runat="server" Text="Statements" CausesValidation="False" OnClick="btnStatements_Click" />
            </td>
            <td align="right">
                <asp:Panel ID="pnlActionButtons" runat="server">
                    <asp:Button ID="btnImport" runat="server" Text="Import" CausesValidation="False" Enabled="False" 
                                OnClick="btnImport_Click" />&nbsp;
                    <asp:Button ID="btnEditStatement" runat="server" Text="Edit Statement" CausesValidation="False" Enabled="False" 
                                OnClick="btnEditStatement_Click" />&nbsp;
                    <asp:Button ID="btnNewLine" runat="server" Text="New Line" CausesValidation="False" OnClick="btnNewLine_Click" />&nbsp;
                    <asp:Button ID="btnBook" runat="server" Text="Book" CausesValidation="False" Enabled="False" OnClick="btnBook_Click"
                     OnClientClick="return confirm('Are you sure you want to book this Bank Statement?')" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height:40px">
                <asp:Label ID="lblErrorMessageMain" runat="server" ForeColor="Red" Height="0px"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <asp:MultiView ID="mvwBankStatement" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwMain" runat="server">
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <br />
            <asp:DetailsView ID="dvBankStatement" runat="server" AutoGenerateRows="False" Caption="Statement"
                CaptionAlign="Left" Height="50px" Width="395px" DefaultMode="Edit" DataSourceID="odsStatementDetails" 
                OnItemUpdated="dvBankStatement_ItemUpdated" OnItemUpdating="dvBankStatement_ItemUpdating" DataKeyNames="JournalEntryId" 
                OnItemCommand="dvBankStatement_ItemCommand" OnDataBound="dvBankStatement_DataBound">
                <Fields>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblTransactionDate" runat="server">Transaction Date</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table style="margin: 0px 0px 0px -3px">
                                <tr>
                                    <td>
                                        <uc2:DatePicker ID="dpDVTransactionDate" runat="server" IsButtonDeleteVisible="false"
                                                        SelectedDate='<%# (DateTime)DataBinder.Eval(Container.DataItem, "TransactionDate") %>' />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="rfvTransactionDate"
                                            ControlToValidate="dpDVTransactionDate:txtDate"
                                            runat="server"
                                            Text="*"
                                            ErrorMessage="Transaction Date is mandatory" />
                                        <asp:RangeValidator
                                            runat="server"
                                            id="rvTransactionDate"
                                            controlToValidate="dpDVTransactionDate:txtDate"
                                            Text="*"
                                            errorMessage="Transaction Date should be no later than today" 
                                            Type="Date"
                                            MinimumValue='<%# DateTime.MinValue.ToShortDateString() %>'
                                            MaximumValue='<%# DateTime.Today.ToShortDateString() %>' />
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblClosingBalance" runat="server">Closing Balance</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:TextBox 
                                ID="txtDVClosingBalanceQuantity" 
                                runat="server" 
                                autocomplete="off" 
                                Text='<%# DataBinder.Eval(Container.DataItem, "ClosingBalanceQuantity", "{0:###0.00}")%>'>
                            </asp:TextBox>&nbsp;
                            <asp:Label
                                ID="lblClosingBalanceCurrency" 
                                runat="server"
                                Text='<%# DataBinder.Eval(Container.DataItem, "ClosingBalanceCurrency")%>'>
                            </asp:Label>
                            <asp:RequiredFieldValidator
                                ID="rfvClosingBalanceQuantity"
                                ControlToValidate="txtDVClosingBalanceQuantity"
                                runat="server"
                                Text="*"
                                ErrorMessage="Closing Balance is mandatory" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                id="revClosingBalanceQuantity"
                                controlToValidate="txtDVClosingBalanceQuantity"
                                Text="*"
                                errorMessage="Closing Balance has an invalid format (should be a number with maximum 2 decimals)"
                                validationExpression="^(-)?(\d\.*)+(\,\d{1,2}0*)?$"
                                display="static" />
                            <asp:HiddenField ID="hdnDVCanChangeTransactionDate" runat="server" 
                                             Value='<%# DataBinder.Eval(Container.DataItem, "CanChangeTransactionDate") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField>
                        <EditItemTemplate>
                            <asp:LinkButton ID="lbtDVOk" runat="server" CausesValidation="True" CommandName="Update"
                                Text="OK"></asp:LinkButton>
                            <asp:LinkButton ID="lbtDVCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel"></asp:LinkButton>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsStatementDetails" runat="server" SelectMethod="GetBankStatementEditView" UpdateMethod="UpdateBankStatement"
                TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.BankStatementLinesAdapter" OldValuesParameterFormatString="original_{0}"
                DataObjectTypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.BankStatementEditView" >
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdnJournalEntryId" Name="journalEntryId" PropertyName="Value" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
            <asp:Label ID="lblErrorMessageDV" runat="server" ForeColor="Red" Height="0px"></asp:Label>
            &nbsp;&nbsp;<br />
            <span class="padding" style="display:block">
                <asp:ValidationSummary ID="vsBankStatement" runat="server" ForeColor="Red" Height="0px" Width="700px"/>
            </span>
        </asp:View>
        <asp:View ID="vwQuestion" runat="server">
            <br />
            <span class="padding" style="display:block">
                <asp:Label ID="lblWarningMessages" runat="server" ForeColor="Black"
                           Width="700px">Closing Balance is the same as Starting Balance. Are you sure you want to book this Bank Statement?</asp:Label>
                <br /><br />&nbsp;
                <asp:Button ID="btnYes" runat="server" Width="75px" Text="Yes" OnClick="btnYes_Click" />&nbsp;
                <asp:Button ID="btnNo" runat="server" Width="75px" Text="No" OnClick="btnNo_Click" />
            </span>
        </asp:View>
        
        <asp:View ID="vweAddTransferFee" runat="server" >
            <br />
            <asp:DetailsView ID="dvAddTransferFee" runat="server" AutoGenerateRows="False" Caption="Add TransferFee"
                CaptionAlign="Left" Height="50px" Width="355px" DefaultMode="Insert" 
                onmodechanging="dvAddTransferFee_ModeChanging" 
                oniteminserting="dvAddTransferFee_ItemInserting" >
                <Fields>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblLineIDLabel" runat="server">Key</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblLineID" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblDescription" runat="server">Description</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="txtDescription" runat="server" />
                            <asp:RequiredFieldValidator
                                ID="rfvDescription"
                                ControlToValidate="txtDescription"
                                runat="server"
                                Text="*"
                                ErrorMessage="Description is mandatory" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblTransferFee" runat="server">Transfer Fee</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <uc4:decimalbox 
                                ID="dbTransferFee" 
                                runat="server" 
                                autocomplete="off" >
                            </uc4:decimalbox>
                            <asp:RequiredFieldValidator
                                ID="rfvTransferFee" 
                                ControlToValidate="dbTransferFee:tbDecimal" 
                                runat="server"
                                Text="*"
                                ErrorMessage="Transfer Fee is mandatory" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <EditItemTemplate>
                            <asp:LinkButton ID="lbtDVOk" runat="server" CausesValidation="True" CommandName="Insert"
                                Text="OK"></asp:LinkButton>
                            <asp:LinkButton ID="lbtDVCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel"></asp:LinkButton>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <br />
            <asp:Label ID="lblErrorMessageATF" runat="server" ForeColor="Red" Height="0px"></asp:Label>
            &nbsp;&nbsp;<br />
            <span class="padding" style="display:block">
                <asp:ValidationSummary ID="vsAddTransferFee" runat="server" ForeColor="Red" Height="0px" Width="700px"/>
            </span>
        </asp:View>
        
    </asp:MultiView>
</asp:Content>


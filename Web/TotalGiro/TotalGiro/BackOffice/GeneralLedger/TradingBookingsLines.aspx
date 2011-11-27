<%@ Page Title="Trading Bookings Lines" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="TradingBookingsLines.aspx.cs" Inherits="TradingBookingsLines" %>

<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Import Namespace="B4F.TotalGiro.GeneralLedger.Journal" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="HiddenField1" runat="server" Value="0" />
    <table style="width: 1000px">
        <tr>
            <td style="width: 145px; height: 24px">
                <asp:Label ID="Label1" runat="server" Text="Journal Entry Number:"></asp:Label>
            </td>
            <td style="width: 370px">
                <asp:Label ID="lblJournalEntryNumber" runat="server" Font-Bold="True"></asp:Label>
                <asp:HiddenField ID="hdnJournalEntryId" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label2" runat="server" Text="Status:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblStatus" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label3" runat="server" Text="Journal:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblJournal" runat="server" Font-Bold="True"></asp:Label>
            </td>
            <td style="height: 24px">
                <asp:Label ID="Label8" runat="server" Text="CounterParty:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblCounterParty" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label4" runat="server" Text="Transaction Date:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblTransactionDate" runat="server" Font-Bold="True"></asp:Label>
            </td>
            <td style="height: 24px">
                <asp:Label ID="Label5" runat="server" Text="Description:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblDescription" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label6" runat="server" Text="Volume:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblVolume" runat="server" Font-Bold="True"></asp:Label>
            </td>
            <td style="height: 24px">
                <asp:Label ID="Label7" runat="server" Text="Price:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblPrice" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlExchangeRate" runat="server">
        <table style="width: 515px">
            <tr>
                <td style="width: 145px; height: 24px">
                    <asp:Label ID="Label9" runat="server" Text="ExchangeRate:"></asp:Label></td>
                <td style="width: 370px">
                    <asp:Label ID="lblExchangeRate" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
        </table>
    </asp:Panel>    
    <br />
    <asp:MultiView ID="mveLines" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweLines" runat="server">
            <asp:GridView ID="gvLines" runat="server" AutoGenerateColumns="False" PageSize="25"
                AllowPaging="True" Caption="Lines" CaptionAlign="Left" AllowSorting="True" 
                DataSourceID="odsJournalEntryLines"
                SkinID="spreadsheet" DataKeyNames="Key" Visible="True"
                OnRowDataBound="gvLines_RowDataBound" >
                <Columns>
                    <asp:TemplateField HeaderText="#" SortExpression="LineNumber">
                        <ItemTemplate>
                            <asp:Label ID="lblLineNumber" runat="server" Width="15px">
                    <%# DataBinder.Eval(Container.DataItem, "LineNumber")%></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status" SortExpression="Status">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Width="43px">
                    <%# (JournalEntryLineStati)DataBinder.Eval(Container.DataItem, "Status")%></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="GL Account" SortExpression="GLAccount_FullDescription">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblGLAccount" runat="server" Width="200px" CssClass="padding"
                                MaxLength="50" LongText='<%# DataBinder.Eval(Container.DataItem, "GLAccount_FullDescription") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlGLAccount" runat="server" SkinID="custom-width" Width="202px"
                                DataSourceID="odsGLAccounts" DataTextField="FullDescription" DataValueField="Key">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsGLAccounts" runat="server" SelectMethod="GetGLAccounts"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.JournalEntryLinesAdapter"></asp:ObjectDataSource>
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="ExRate" SortExpression="ExchangeRate">
                        <ItemTemplate>
                            <asp:Label ID="lblExchangeRate" runat="server" Width="50px">
                                <%# DataBinder.Eval(Container.DataItem, "ExchangeRate", "{0:###0.00000}")%></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <db:DecimalBox ID="dbExRate" runat="server" SkinID="custom-width" DecimalPlaces="5"
                                Width="75px" />
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Debit" SortExpression="Debit">
                        <ItemTemplate>
                            <asp:Label ID="lblDebit" runat="server" Width="71px">
                    <%# DataBinder.Eval(Container.DataItem, "DebitDisplayString") %></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <db:DecimalBox ID="dbDebitQuantity" runat="server" SkinID="custom-width"
                                Width="75px" />
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Credit" SortExpression="Credit">
                        <ItemTemplate>
                            <asp:Label ID="lblCredit" runat="server" Width="71px">
                    <%# DataBinder.Eval(Container.DataItem, "CreditDisplayString")%></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <db:DecimalBox ID="dbCreditQuantity" runat="server" SkinID="custom-width"
                                Width="75px" />
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Giro Account" SortExpression="GiroAccount_Number">
                        <ItemTemplate>
                            <uc1:AccountLabel ID="ctlAccountLabel" 
                                runat="server" 
                                RetrieveData="false" 
                                Width="120px" 
                                NavigationOption="PortfolioView"
                                AccountDisplayOption="DisplayNumber"
                                ToolTip='<%# DataBinder.Eval(Container.DataItem, "GiroAccount_ShortName") %>'
                                />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Trade Number" SortExpression="TxComponent_Key">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblDescription" runat="server" Width="99px" CssClass="padding"
                                MaxLength="22" LongText='<%# DataBinder.Eval(Container.DataItem, "TxComponent_Key") %>' />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order Number" SortExpression="TotalGiroOrderID">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblTotalGiroOrderID" runat="server" Width="99px" CssClass="padding"
                                MaxLength="22" LongText='<%# DataBinder.Eval(Container.DataItem, "TotalGiroOrderID") %>' />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Transaction Type" SortExpression="TotalGiroTXType">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblTXType" runat="server" Width="99px" CssClass="padding"
                                MaxLength="22" LongText='<%# DataBinder.Eval(Container.DataItem, "TotalGiroTXType") %>' />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <table cellpadding="0" cellspacing="0" border="0" width="1048px">
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsJournalEntryLines" runat="server" SelectMethod="GetJournalEntryLines"
                UpdateMethod="UpdateJournalEntryLine" TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.TradingBookingsLinesAdapter"
                OldValuesParameterFormatString="original_{0}">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdnJournalEntryId" Name="journalEntryId" PropertyName="Value"
                        Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="vweSummary" runat="server">
            <asp:GridView ID="gvSummary" runat="server" AutoGenerateColumns="False" PageSize="25"
                AllowPaging="True" Caption="Summary" CaptionAlign="Left" AllowSorting="True" DataSourceID="odsSummary"
                SkinID="spreadsheet"  Visible="True">
                <Columns>
                    <asp:TemplateField HeaderText="GL Account" SortExpression="account_FullDescription">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblGLAccount" runat="server" Width="200px" CssClass="padding"
                                MaxLength="50" LongText='<%# DataBinder.Eval(Container.DataItem, "account_FullDescription") %>' />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Giro Account" SortExpression="giroacct_Number">
                        <ItemTemplate>
                            <asp:Label ID="lblGiroAccount" runat="server" Width="99px" ToolTip='<%# DataBinder.Eval(Container.DataItem, "giroacct_Number") %>'>
                    <%# DataBinder.Eval(Container.DataItem, "giroacct_Number")%></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Account Name" SortExpression="giroacct_ShortName">
                        <ItemTemplate>
                            <asp:Label ID="lblGiroAccount" runat="server" Width="99px" ToolTip='<%# DataBinder.Eval(Container.DataItem, "giroacct_ShortName") %>'>
                    <%# DataBinder.Eval(Container.DataItem, "giroacct_ShortName")%></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Debit" SortExpression="Debit">
                        <ItemTemplate>
                            <asp:Label ID="lblDebit" runat="server" Width="71px">
                    <%# DataBinder.Eval(Container.DataItem, "Debit_DisplayString") %></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <db:DecimalBox ID="dbDebitQuantity" runat="server" SkinID="custom-width"
                                Width="75px" />
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Credit" SortExpression="Credit">
                        <ItemTemplate>
                            <asp:Label ID="lblCredit" runat="server" Width="71px">
                    <%# DataBinder.Eval(Container.DataItem, "Credit_DisplayString")%></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <db:DecimalBox ID="dbCreditQuantity" runat="server" SkinID="custom-width"
                                Width="75px" />
                        </EditItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsSummary" runat="server" SelectMethod="GetLineSummary"
                UpdateMethod="UpdateJournalEntryLine" TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.TradingBookingsLinesAdapter"
                OldValuesParameterFormatString="original_{0}">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hdnJournalEntryId" Name="journalEntryId" PropertyName="Value"
                        Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
    </asp:MultiView>
    <br />
    <table width="1061px">
        <tr>
            <td style="width: 300px">
                <asp:Button ID="btnJournals" runat="server" Text="Journals" CausesValidation="False"
                    PostBackUrl="~/BackOffice/GeneralLedger/TradingJournals.aspx" />&nbsp;
            </td>
            <td align="right">
                <asp:Button ID="btnToggleLines" runat="server" Text="View Summary" CausesValidation="False"
                    OnClick="btnToggleLines_Click" />&nbsp;
            </td>
        </tr>
    </table>
    <br />
    <asp:MultiView ID="mvwTradingBookings" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwMain" runat="server">
            <asp:Label ID="lblErrorMessageMain" runat="server" ForeColor="Red" Height="0px"></asp:Label>
        </asp:View>
    </asp:MultiView>
</asp:Content>

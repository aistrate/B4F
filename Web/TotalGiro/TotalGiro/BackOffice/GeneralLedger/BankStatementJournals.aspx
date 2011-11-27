<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="BankStatementJournals.aspx.cs" Inherits="BankStatementJournals" 
         Title="Bank Statement Journals" Theme="Neutral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <asp:GridView ID="gvJournals" runat="server" Caption="Journals" CaptionAlign="Left" PageSize="25" AllowPaging="True" 
                  AllowSorting="True" DataSourceID="odsJournals" AutoGenerateColumns="False" DataKeyNames="Key" >
        <Columns>
            <asp:BoundField DataField="JournalNumber" HeaderText="Journal#" SortExpression="JournalNumber">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="BankAccountNumber" HeaderText="Bank Account" SortExpression="BankAccountNumber">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="BankAccountDescription" HeaderText="Description" SortExpression="BankAccountDescription">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="FixedGLAccount_GLAccountNumber" HeaderText="Fixed GL Acct" SortExpression="FixedGLAccount_GLAccountNumber">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ManagementCompany_CompanyName" HeaderText="Company" SortExpression="ManagementCompany_CompanyName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Currency_Symbol" HeaderText="Curr" SortExpression="Currency_Symbol">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Balance" SortExpression="Balance">
                <ItemTemplate>
                    <asp:Label ID="lblBalance" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "DateLastBooked") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "Balance", "{0:#,##0.00}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Last Booked" SortExpression="DateLastBooked">
                <ItemTemplate>
                    <asp:Label ID="lblDateLastBooked" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "DateLastBooked") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "DateLastBooked", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="False"/>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtNew" runat="server" CausesValidation="False" Text="New" CommandName="NewStatement"
                                    ToolTip="Add a new statement to this journal" OnCommand="lbtNew_Command" 
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                    <asp:LinkButton ID="lbtStatements" runat="server" CausesValidation="False" Text="Statements" CommandName="ViewStatements"
                                    ToolTip="View statements of this journal" OnCommand="lbtStatements_Command"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsJournals" runat="server" SelectMethod="GetBankStatementJournals"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.BankStatementJournalsAdapter"></asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>


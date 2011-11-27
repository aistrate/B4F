<%@ Page Title="Trial Balance" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="TrialBalance.aspx.cs" Inherits="TrialBalance" %>

<%@ Register Src="~/UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <table style="width: 770px">
        <tr>
            <td style="width: 160px; height: 24px">
                <asp:Label ID="Label2" runat="server" Text="Trial Balance Date:"></asp:Label>
            </td>
            <td style="width: 210px">
                <uc1:DatePicker ID="dpTransactionDateFrom" runat="server" />
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 210px">
            </td>
            <td>
            </td>
        </tr>
    </table>
    <table style="width: 770px">
        <tr>
            <td style="height: 24px">
                <asp:Label ID="Label5" runat="server" Text="Type Of Balance:"></asp:Label>
                <asp:HiddenField ID="hdnBalanceType" runat="server" />
            </td>
            <td colspan="1">
                <asp:Button ID="btnNett" runat="server" Text="Nett" CausesValidation="False" OnClick="btnNett_Click" />
            </td>
            <td colspan="1">
                <asp:Button ID="btnFull" runat="server" Text="Full" CausesValidation="False" OnClick="btnFull_Click" />
            </td>
            <td colspan="1">
                <asp:Button ID="btnGrouped" runat="server" Text="Group" CausesValidation="False" OnClick="btnGroup_Click"/>
            </td>
            <td align="right">
            </td>
        </tr>
    </table>
    <asp:MultiView ID="mveTrialBalance" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweFullBalance" runat="server">
            <asp:GridView ID="gvFullBalance" runat="server" AutoGenerateColumns="False" PageSize="40"
                AllowPaging="True" Caption="Lines" CaptionAlign="Left" AllowSorting="True" DataSourceID="odsFullBalance"
                SkinID="spreadsheet" DataKeyNames="Key" Visible="True">
                <Columns>
                    <asp:TemplateField HeaderText="#" SortExpression="LineNumber">
                        <ItemTemplate>
                            <asp:Label ID="lblLineNumber" runat="server" Width="15px">
                    <%# DataBinder.Eval(Container.DataItem, "LineNumber")%></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="GL Account" SortExpression="GLAccount_FullDescription">
                        <ItemTemplate>
                            <asp:Label ID="lblGLAccount" runat="server" Width="200px" CssClass="padding">
                                <%# DataBinder.Eval(Container.DataItem, "Account_FullDescription") %></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Debit" SortExpression="Debit">
                        <ItemTemplate>
                            <asp:Label ID="lblDebit" runat="server" Width="100px">
                    <%# DataBinder.Eval(Container.DataItem, "DebitDisplayString") %></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Credit" SortExpression="Credit">
                        <ItemTemplate>
                            <asp:Label ID="lblCredit" runat="server" Width="100px">
                    <%# DataBinder.Eval(Container.DataItem, "CreditDisplayString")%></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsFullBalance" runat="server" SelectMethod="GetTrialBalance"
                TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.TrialBalanceAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="dpTransactionDateFrom" Name="transactionDate" PropertyName="SelectedDate"
                        Type="DateTime" />
                    <asp:ControlParameter ControlID="hdnBalanceType" Name="BalanceType" PropertyName="Value"
                        Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
    </asp:MultiView>
</asp:Content>

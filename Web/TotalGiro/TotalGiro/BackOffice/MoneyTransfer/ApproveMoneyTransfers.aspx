<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ApproveMoneyTransfers.aspx.cs"
    Inherits="ApproveMoneyTransfers" Title="Approve Money Transfer Orders" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <table cellpadding="0" cellspacing="0" border="0"  style="border-color:black;" width="100%">
        <tr>
            <td class="tblHeader">Money Transfer Orders Filter</td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width:100%">
                    <tr style="height :25px;">
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="lblDateFrom" runat="server" Text="Date From" />
                        </td>
                        <td style="width: 420px;">
                            <uc1:DatePicker ID="dtpDateFrom" runat="server" />
                        </td>
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="lblDateTo" runat="server" Text="Date To" />
                        </td>
                        <td style="width: 600px;">
                            <uc1:DatePicker ID="dtpDateTo" runat="server" />
                        </td>
                    </tr>
                    
                    <tr style="height :25px;">
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="lblMinimumAmount" runat="server" Text="Minimum Amount" />
                        </td>
                        <td style="width: 420px;">
                            <db:DecimalBox ID="dbMinimumAmount" runat="server" DecimalPlaces="2" Width="145px" />
                        </td>
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="lblMaximumAmount" runat="server" Text="Maximum Amount" />
                        </td>
                        <td style="width: 600px;">
                            <db:DecimalBox ID="dbMaximumAmount" runat="server" DecimalPlaces="2" Width="145px" />
                        </td>
                    </tr>
                    
                    <tr style="height :25px;">
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number" />
                        </td>
                        <td style="width: 420px;">
                            <asp:TextBox ID="txtAccountNumber" runat="server" Width="194px" />
                        </td>
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="lblBeneficiary" runat="server" Text="Beneficiary" />
                        </td>
                        <td style="width: 600px;">
                            <asp:TextBox ID="txtBeneficiary" runat="server" Width="194px" />
                        </td>
                    </tr>

                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblDescription" runat="server" Text="Description" />
                        </td>
                        <td style="width: 420px;">
                            <asp:TextBox ID="txtDescription" runat="server" Width="194px" />
                        </td>
                        <td colspan="2" />
                    </tr>
                    <tr style="height :25px;">
                        <td style="height: 37px; width: 230px;" colspan="4">
                            <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Text="Refresh" />
                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" OnClick="btnResetFilter_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
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
    <cc1:MultipleSelectionGridView ID="gvMoneyOrders" runat="server" CellPadding="0"
        AutoGenerateColumns="false" DataSourceID="odsMoneyOrders" AllowPaging="True"
        PageSize="15" Caption="Money Transfer Orders" CaptionAlign="Left" AllowSorting="True"
        DataKeyNames="Key" SkinID="custom-width" Width="100%" SelectionBoxEnabledBy="IsApproveable"
        OnRowCommand="gvMoneyOrders_RowCommand" OnRowDataBound="gvMoneyOrders_RowDataBound" >
        <Columns>
            <asp:BoundField DataField="Reference" HeaderText="Reference" SortExpression="Reference"
                ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="Amount_DisplayString" HeaderText="Amount" SortExpression="Amount"
                ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="TotalGiro(optional)" SortExpression="TransfereeAccount_Number">
                <ItemTemplate>
                    <uc2:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="false" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountDisplayOption="DisplayNumber"
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="BenefBankAcctNr" HeaderText="Beneficiary" SortExpression="BenefBankAcctNr"
                ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Beneficiary NAR" SortExpression="NarBenef1">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlNarBenef" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="25"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "NarBenef1") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description" SortExpression="DisplayDescription">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlDisplayDescription" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="25"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "DisplayDescription") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ProcessDate" HeaderText="To Be Processed" SortExpression="ProcessDate"
                DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status"
                ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Edit" Text='<%# (Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsEditable")) == true ? "Edit" : "View") %>' />
                    <asp:LinkButton runat="server" ID="lbtCancel" Text="Cancel" CommandName="Cancel"
                        ToolTip="Cancel Money Order" 
                        OnClientClick="return confirm('Cancel order?')" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsMoneyOrders" runat="server" SelectMethod="GetMoneyTransferOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.MoneyTransferOrderOverviewAdapter">
        <SelectParameters>
            <asp:Parameter Name="approved" DefaultValue="false" Type="Boolean" />
            <asp:Parameter Name="status" DefaultValue="1" Type="Int32" />
            <asp:ControlParameter ControlID="dtpDateFrom" Name="fromDate"  PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="dtpDateTo" Name="toDate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="dbMinimumAmount" Name="minAmountQty" PropertyName="Value" Type="Decimal" />
            <asp:ControlParameter ControlID="dbMaximumAmount" Name="maxAmountQty"  PropertyName="Value" Type="Decimal" />
            <asp:ControlParameter ControlID="txtAccountNumber" Name="accountNumber" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBeneficiary" Name="beneficiary" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtDescription" Name="description" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Button ID="btnApprove" runat="server" Text="Approve" ToolTip="Approves the selected money transfer orders"
        OnClick="btnApprove_Click" />
</asp:Content>

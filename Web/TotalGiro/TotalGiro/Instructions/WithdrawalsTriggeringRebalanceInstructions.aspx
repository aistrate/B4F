<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="WithdrawalsTriggeringRebalanceInstructions.aspx.cs" Inherits="WithdrawalsTriggeringRebalanceInstructions" Title="Withdrawals Triggering Rebalance Instructions" %>

<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true" />
    <table style="width: 395px">
        <tr>
            <td style="width: 0.15px; height: 0px"/>
            <td style="width: 120px; height: 0px">
                <asp:Label ID="lblFilter" runat="server" Text="Filter:"></asp:Label></td>
            <td style="width: 175px; height: 0px">
                <asp:DropDownList ID="ddlFilter" runat="server" Width="266px" SkinID="custom-width" >
                    <asp:ListItem Value="ALL">All accounts</asp:ListItem>
                    <asp:ListItem Value="NEED" Selected="true">Accounts who need to be rebalanced</asp:ListItem>
                    <asp:ListItem Value="NONEED">Accounts who do not need to be rebalanced</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvAccounts"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        OnRowCommand="gvAccounts_OnRowCommand"
        Caption="Accounts with Cash Withdrawal Instructions"
        CaptionAlign="Left"
        DataSourceID="odsAccounts" 
        AllowSorting="True"
        Visible="false" 
        style="position: relative; top: -10px;">
        <Columns>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:Image ID="imgWarning" runat="server" 
                    ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                    Visible='<%# DataBinder.Eval(Container.DataItem, "NeedsRebalance") %>'
                    ToolTip="This account needs to be rebalanced since there is not enough cash available for the upcoming withdrawal"
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Number" HeaderText="Account No." SortExpression="Number">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ShortName" HeaderText="Account Name" SortExpression="ShortName">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ActiveWithdrawalInstructions_FirstWithdrawalDate" HeaderText="First WithdrawalDate" SortExpression="ActiveWithdrawalInstructions_FirstWithdrawalDate" DataFormatString="{0:dd MMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ActiveWithdrawalInstructions_Count" HeaderText="#" SortExpression="ActiveWithdrawalInstructions_Count" >
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ActiveWithdrawalInstructions_TotalAmount" HeaderText="WithdrawalAmount" SortExpression="ActiveWithdrawalInstructions_TotalAmount" DataFormatString="{0:###,##0.00}" HtmlEncode="False" >
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TotalBothCash" HeaderText="Total Amount" SortExpression="TotalBothCash" DataFormatString="{0:###,##0.00}" HtmlEncode="False" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="TotalCash" HeaderText="Cash Amount" SortExpression="TotalCash" DataFormatString="{0:###,##0.00}" HtmlEncode="False" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="TotalCashFund" HeaderText="CashFund Amount" SortExpression="TotalCashFund" DataFormatString="{0:###,##0.00}" HtmlEncode="False" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                         ID="lbtnViewInstructions"
                         runat="server"
                         CommandName="ViewInstructions"
                         Text="View" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </cc1:MultipleSelectionGridView>
    <asp:Panel ID="pnlRebalanceInstructionProps" runat="server" Visible="False">
        <table cellpadding="1" cellspacing="1" border="0" >
          <tr>
            <td>
                <asp:Label ID="lblExecDate" runat="server" Text="Execution Date" />
            </td>
            <td>
                <uc2:DatePicker ID="dtpExecDate" runat="server" ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" IsButtonDeleteVisible="false" />
            </td>
            <td>
                <asp:RangeValidator
                    runat="server"
                    id="rvExecDate"
                    controlToValidate="dtpExecDate:txtDate"
                    Text="*"
                    errorMessage="Date should be later or equal as today" 
                    Type="Date"
                    MinimumValue='2008-03-16'
                    MaximumValue='2035-03-25'/>&nbsp
                <asp:RequiredFieldValidator ID="rfvExecDate"
                    ControlToValidate="dtpExecDate:txtDate"
                    runat="server"
                    Text="*"
                    ErrorMessage="Date is mandatory" />
            </td>
          </tr>

          <tr>
            <td>&nbsp</td>
            <td>
                <asp:CheckBox ID="chkNoCharges" runat="server" Text="No Commission" Checked="true" />
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>
        </table>
        <br />
    </asp:Panel>
    <span class="padding" style="display:block">
        <asp:ValidationSummary ID="vsInstructionProps" runat="server" ForeColor="Red" Height="0px" Width="450px"/>
    </span>
    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False" Font-Bold="True" />
    <asp:Label ID="lblResult" runat="server" Font-Bold="True" ForeColor="Black" Visible="False" />
    <asp:Button ID="btnCreateRebalanceInstructions" runat="server" Text="Create Rebalance Instruction" OnClick="btnCreateRebalanceInstructions_Click" Visible="false" />&nbsp
    <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" Visible="False" CausesValidation="False" />
    <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetAccountsWithCashWithdrawals"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalsTriggeringRebalanceInstructionsAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="ddlFilter" Name="filterOption" PropertyName="SelectedValue"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <br />
    <br />
    <br />
    <asp:GridView 
        ID="gvInstructions"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        Caption="Withdrawal Instructions"
        CaptionAlign="Left"
        DataSourceID="odsInstructions" 
        AllowSorting="True"
        Visible="false" 
        style="position: relative; top: -10px;">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="ID" SortExpression="Key" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayRegularity" HeaderText="Regularity" SortExpression="DisplayRegularity">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Amount_DisplayString" HeaderText="Amount" SortExpression="Amount_DisplayString" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="DisplayStatus" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Message" SortExpression="Message">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="lblMessage"
                        runat="server"
                        Width="35"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Message") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreationDate" HeaderText="Creation Date" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="WithdrawalDate" HeaderText="Withdrawal Date" SortExpression="WithdrawalDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:Button ID="btnHideInstructions" runat="server" Text="Hide" OnClick="btnHideInstructions_Click" Visible="false" />
    <asp:ObjectDataSource ID="odsInstructions" runat="server" SelectMethod="GetInstructions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalsTriggeringRebalanceInstructionsAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvAccounts" Name="accountId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
</asp:Content>


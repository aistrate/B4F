<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="WithdrawalInstructionManagement.aspx.cs" Inherits="WithdrawalInstructionManagement" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register Src="../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts.Instructions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true" ShowSearchButton="false" />
    <table>
        <tr>
            <td width="0.15px"> </td>
            <td width="128px">
                <asp:Label ID="lblDateFrom" runat="server" Text="From" />
            </td>
            <td width="195px">
                <uc2:DatePicker ID="dtpDateFrom" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblDateTo" runat="server" Text="To" />
            </td>
            <td width="195px">
                <uc2:DatePicker ID="dtpDateTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:Label ID="lblStatus" runat="server" Text="Status:"></asp:Label>
            </td>
            <td colspan="2" >
                <asp:DropDownList ID="ddlStatus" runat="server" DataSourceID="odsStatus"
                    DataTextField="Status" DataValueField="ID" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsStatus" runat="server" SelectMethod="GetAccountStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:Label ID="lblWithdrawalType" runat="server" Text="Type"  />
            </td>
            <td colspan="2" >
                <asp:RadioButtonList ID="rblWithdrawalType" runat="server" RepeatDirection="Horizontal" >
                    <asp:ListItem Text="Periodic" Value="1"  />
                    <asp:ListItem Text="Ad-Hoc" Value="0" />
                    <asp:ListItem Text="All" Value="-1" Selected="True" />
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="Search" Style="float: right; position: relative; top: -2px" Width="90px" 
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvInstructionOverview"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        OnRowCommand="gvInstructionOverview_OnRowCommand"
        Caption="Withdrawal Instructions"
        CaptionAlign="Left"
        DataSourceID="odsInstructions" 
        AllowSorting="True"
        Visible="false"
        style="position: relative; top: -10px; z-index:2;"
        SelectionBoxEnabledBy="IsActive">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="ID" SortExpression="Key" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account#" SortExpression="Account_Number">
                <HeaderStyle Wrap="False" />
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <ItemTemplate>
                    <uc2:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="true" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountDisplayOption="DisplayNumber"
                        AccountID='<%#  DataBinder.Eval(Container.DataItem, "Account_Key") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account Name" SortExpression="Account_ShortName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="50px" CssClass="padding"
                        MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayRegularity" HeaderText="Regularity" SortExpression="Regularity">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Amount_DisplayString" HeaderText="Amount" SortExpression="Amount_Quantity" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:CheckBoxField DataField="DoNotChargeCommission" HeaderText="Ex.Com." SortExpression="DoNotChargeCommission">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:CheckBoxField>
            <asp:TemplateField HeaderText="Status" SortExpression="DisplayStatus">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblStatus" runat="server" Width="50px" CssClass="padding"
                        MaxLength="11" LongText='<%# DataBinder.Eval(Container.DataItem, "DisplayStatus") %>' />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Message" SortExpression="Message">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="lblMessage"
                        runat="server"
                        Width="25"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Message") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:Image ID="imgWarning" runat="server" 
                    ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                    Visible='<%# DataBinder.Eval(Container.DataItem, "Warning") %>'
                    ToolTip="This instruction needs attention"
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:dd MMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="WithdrawalDate" HeaderText="Withdrawal" SortExpression="WithdrawalDate" DataFormatString="{0:dd MMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="FreeUp Cash" SortExpression="LatestPossibleFreeUpCashDate">
                <ItemTemplate>
                    <asp:Label ID="lblMaxDate" runat="server" Width="55px">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "LatestPossibleFreeUpCashDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "LatestPossibleFreeUpCashDate", "{0:dd MMM yyyy}") : "")%>
                    </asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                         ID="lbtnEditInstruction"
                         runat="server"
                         CommandName="EditInstruction"
                         Text="Edit"
                         Visible='<%# DataBinder.Eval(Container.DataItem, "IsEditable") %>' />
                    <asp:LinkButton
                         ID="lbtnCancelInstruction"
                         runat="server"
                         CommandName="CancelInstruction"
                         Text="Cancel"
                         Visible='<%# DataBinder.Eval(Container.DataItem, "IsCancellable") %>'
                         OnClientClick="return confirm('Are you sure you want to cancel this withdrawal instruction?')" />
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewOrders" 
                        Text="Orders" 
                        CommandName="ViewOrders"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "HasOrders") %>' />
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewMoneyOrder" 
                        Text="MoneyOrder" 
                        CommandName="ViewMoneyOrder"
                        Visible='<%# (DataBinder.Eval(Container.DataItem, "MoneyTransferOrder_Key") != System.DBNull.Value && (int)DataBinder.Eval(Container.DataItem, "MoneyTransferOrder_Key") != 0 ? true : false) %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </cc1:MultipleSelectionGridView>
    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False" Font-Bold="True"></asp:Label><asp:Label
        ID="lblResult" runat="server" Font-Bold="True" ForeColor="Black" Visible="False"></asp:Label><br />
    <asp:Button ID="btnProcessInstructions" runat="server" Text="Process" OnClick="btnProcessInstructions_Click" Visible="false" />
    <br />
    <br />
    <asp:ObjectDataSource ID="odsInstructions" runat="server" SelectMethod="GetInstructions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalInstructionManagementAdapter" SortParameterName="sortColumn">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="dtpdateFrom" Name="dateFrom" PropertyName="SelectedDate" 
                Type="DateTime" />
            <asp:ControlParameter ControlID="dtpdateTo" Name="dateTo" PropertyName="SelectedDate" 
                Type="DateTime" />
            <asp:ControlParameter ControlID="ddlStatus" Name="activeStatus" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter ControlID="rblWithdrawalType" Name="returnType" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="maximumRows" PropertyName="PageSize" 
                Type="Int32" />
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="pageIndex" PropertyName="PageIndex" 
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />

    <asp:DetailsView 
        ID="dvInstructionEdit" 
        runat="server" 
        DataSourceID="odsInstructionEdit" 
        AutoGenerateRows="False"
        DataKeyNames="InstructionID"
        Caption="Edit Instruction" 
        CaptionAlign="Left"
        OnItemCommand="dvInstructionEdit_ItemCommand" 
        Width=480px
        Visible="False" ondatabound="dvInstructionEdit_DataBound"> 
        <Fields>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblInstructionID" runat="server">InstructionID</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="InstructionID" runat="server" 
                        Text='<%# DataBinder.Eval(Container.DataItem, "InstructionID") %>'>
                    </asp:Label>
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblWithdrawalDate" runat="server">Withdrawal Date</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <uc2:DatePicker ID="dtpWithdrawalDate" runat="server" 
                        ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" 
                        IsButtonDeleteVisible="false" 
                        SelectedDate ='<%# DataBinder.Eval(Container.DataItem, "WithdrawalDate") %>' />&nbsp
                    <asp:RangeValidator
                        runat="server"
                        id="rvWithdrawalDate"
                        controlToValidate="dtpWithdrawalDate:txtDate"
                        Text="*"
                        errorMessage="Date should be later or equal as today" 
                        Type="Date"
                        MinimumValue='2008-03-16'
                        MaximumValue='2035-03-25'/>&nbsp
                    <asp:RequiredFieldValidator ID="rfvWithdrawalDate"
                        ControlToValidate="dtpWithdrawalDate:txtDate"
                        runat="server"
                        Text="*"
                        ErrorMessage="Date is mandatory" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblExecDate" runat="server">Relevance Date</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <uc2:DatePicker ID="dtpExecDate" runat="server" 
                        ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" 
                        IsButtonDeleteVisible="false" 
                        SelectedDate ='<%# DataBinder.Eval(Container.DataItem, "ExecutionDate") %>' />&nbsp
                    <asp:CompareValidator 
                        ID="cvExecDate" 
                        runat="server"
                        ControlToValidate="dtpExecDate:txtDate"
                        ControlToCompare="dtpWithdrawalDate:txtDate"
                        Operator="LessThanEqual"
                        Type="Date"
                        Text="*" 
                        ErrorMessage="The relevance date can not be after the withdrawal date"/>&nbsp
                    <asp:RequiredFieldValidator ID="rfvExecDate"
                        ControlToValidate="dtpExecDate:txtDate"
                        runat="server"
                        Text="*"
                        ErrorMessage="Date is mandatory" />
                        
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblWithdrawalAmount" runat="server" Text="Withdrawal Amount" />
                </HeaderTemplate>
                <ItemTemplate>
                    <db:DecimalBox ID="dbWithdrawalAmount" runat="server" 
                        Value ='<%# DataBinder.Eval(Container.DataItem, "WithdrawalAmount") %>'
                        DecimalPlaces="2" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblCounterAccount" runat="server" Text="Counter Account" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList 
                        ID="ddlCounterAccount" DataSourceID="odsCounterAccount" DataTextField="DisplayName" DataValueField="Key" 
                        SkinID="broad" runat="server" /> 
                    <asp:ObjectDataSource ID="odsCounterAccount" runat="server" 
                        SelectMethod="GetCounterAccounts"
                        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalInstructionManagementAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvInstructionOverview" Name="instructionId" PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblTransferDescription" runat="server">Transfer Description</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:TextBox ID="txtTransferDescription" SkinID="custom-width" Width="245px" 
                        Text ='<%# DataBinder.Eval(Container.DataItem, "TransferDescription") %>'
                        runat="server" /> 
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    &nbsp
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkNoChargesWithdrawal" runat="server" Text="No Commission" Checked='<%# (bool)DataBinder.Eval(Container.DataItem, "DoNotChargeCommission") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDvSaveInstruction" 
                        Text="OK" 
                        CommandName="dvSaveInstruction"
                        Visible="true"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDvCancelEdit"
                        CausesValidation="false" 
                        Text="Cancel" 
                        CommandName="dvCancelEdit"
                        Visible="true"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <span class="padding" style="display:block">
        <asp:ValidationSummary ID="vsInstructionProps" runat="server" ForeColor="Red" Height="0px" Width="450px" Visible="false"/>
    </span>
    
    <asp:ObjectDataSource 
        ID="odsInstructionEdit" 
        runat="server" 
        SelectMethod="GetInstructionEditData"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalInstructionManagementAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="instructionId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:GridView 
        ID="gvOrders"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="OrderID"
        DataSourceID="odsOrders"
        AutoGenerateColumns="False"
        Visible="false">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="Account_Number" HeaderText="Account Number">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account Name">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel1" 
                        runat="server"
                        CssClass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Instrument">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel2" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "RequestedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Side">
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel3" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Value_DisplayString") %>' 
                        />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:TemplateField>
            <asp:BoundField DataField="Commission_DisplayString" HeaderText="Commission">
                <HeaderStyle CssClass="alignright" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TopParentDisplayStatus" HeaderText="ParentStatus">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:Button ID="btnHideOrders" runat="server" Text="Hide" OnClick="btnHideOrders_Click" Visible="false" />&nbsp
    <asp:ObjectDataSource ID="odsOrders" runat="server" 
        SelectMethod="GetOrdersByInstruction"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalInstructionManagementAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="instructionId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:GridView ID="gvMoneyOrder" runat="server" CellPadding="0"
        AutoGenerateColumns="false" DataSourceID="odsMoneyOrder" 
        Caption="Money Transfer Order" CaptionAlign="Left"
        DataKeyNames="Key" SkinID="custom-width" Width="100%"
        Visible="false" >
        <Columns>
            <asp:BoundField DataField="Reference" HeaderText="Reference" ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Amount_DisplayString" HeaderText="Amount" ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TransfereeAccount_Number" HeaderText="TotalGiro (optional)" ReadOnly="True">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="BenefBankAcctNr" HeaderText="Beneficiary" ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Center" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NarBenef1" HeaderText="Beneficiary NAR" ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TransferDescription1" HeaderText="Description" ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ProcessDate" HeaderText="To Be Processed" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" ReadOnly="True">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsMoneyOrder" runat="server" SelectMethod="GetMoneyTransferOrder"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.WithdrawalInstructionManagementAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="instructionId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Button ID="btnHideMoneyOrders" runat="server" Text="Hide" OnClick="btnHideMoneyOrders_Click" Visible="false" />&nbsp
</asp:Content>


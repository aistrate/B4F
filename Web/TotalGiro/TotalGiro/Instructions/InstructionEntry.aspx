<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="InstructionEntry.aspx.cs" Inherits="InstructionEntry" Theme="Neutral" %>

<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc2" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register Src="../UC/Address.ascx" TagName="Address" TagPrefix="ucAddress" %>
<%@ Register Src="../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../UC/InstrumentsModelsSelector.ascx" TagName="InstrumentsModelsSelector" TagPrefix="uc3" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.Accounts.Instructions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true"/>&nbsp
    <asp:HiddenField ID="hdfInstructionType" runat="server" />
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvAccounts"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="15" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        OnRowCommand="gvAccounts_OnRowCommand"
        Caption="Accounts"
        CaptionAlign="Left"
        DataSourceID="odsAccounts" 
        AllowSorting="True" 
        Visible="False"
        SkinID="custom-width"
        Width="900px">
        <Columns>
            <asp:TemplateField HeaderText="Account#" SortExpression="Number">
                <HeaderStyle Wrap="False" />
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <ItemTemplate>
                    <uc2:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="true" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountDisplayOption="DisplayNumber"
                        AccountID='<%#  DataBinder.Eval(Container.DataItem, "Key") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account Name" SortExpression="ShortName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                        MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Model" SortExpression="ModelPortfolio_ModelName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblModel" runat="server" Width="50px" CssClass="padding"
                        MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ModelPortfolio_ModelName") %>' />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Last Rebalance" SortExpression="LastRebalanceDate">
                <HeaderStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="lblLastRebalanceDate"
                        runat="server"
                        Width="35"
                        Text='<%# (DataBinder.Eval(Container.DataItem, "LastRebalanceDate") == DBNull.Value || (DateTime)DataBinder.Eval(Container.DataItem, "LastRebalanceDate") == DateTime.MinValue ? "" : Eval("LastRebalanceDate", "{0:dd-MM-yyyy}")) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Current Rebalance" SortExpression="CurrentRebalanceDate">
                <HeaderStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                <itemtemplate>
                    <trunc:TruncLabel ID="lblCurrentRebalanceDate"
                        runat="server"
                        Width="35"
                        Text='<%# (DataBinder.Eval(Container.DataItem, "CurrentRebalanceDate") == DBNull.Value || (DateTime)DataBinder.Eval(Container.DataItem, "CurrentRebalanceDate") == DateTime.MinValue ? "" : Eval("CurrentRebalanceDate", "{0:dd-MM-yyyy}")) %>' 
                        />
                </itemtemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Active Instructions" SortExpression="ActiveAccountInstructions_Count">
                <HeaderStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "ActiveAccountInstructions_Count") == DBNull.Value || (Int32)DataBinder.Eval(Container.DataItem, "ActiveAccountInstructions_Count") == 0 ? "" : DataBinder.Eval(Container.DataItem, "ActiveAccountInstructions_Count")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewTransactions" 
                        Text="View"
                        Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "ActiveAccountInstructions_Count") > 0 ? true : false) %>' 
                        CommandName="SELECT"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtSetCounterAccount" 
                        Text="Set CounterAccount"
                        Visible='<%# (DataBinder.Eval(Container.DataItem, "CounterAccount_Key") == System.DBNull.Value ? true : false) %>' 
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        CommandName="SetCounterAccount"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCustomerAccounts"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter" SortParameterName="sortColumn"
        OldValuesParameterFormatString="original_{0}" >
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="gvAccounts" Name="maximumRows" PropertyName="PageSize" 
                Type="Int32" />
            <asp:ControlParameter ControlID="gvAccounts" Name="pageIndex" PropertyName="PageIndex" 
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Panel ID="pnlRebalanceInstructionProps" runat="server" Visible="False">
        <table cellpadding="1" cellspacing="1" border="0" >
          <tr>
            <td style="width: 120px"><asp:Label ID="lblOrderActionTypes" runat="server" Text="Action Type"></asp:Label></td>
            <td style="width: 150px">
                <asp:DropDownList ID="ddlOrderActionTypes" runat="server" DataSourceID="odsOrderActionTypes" 
                    DataTextField="Description" DataValueField="Key" SkinID="custom-width" Width="85px">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsOrderActionTypes" runat="server" SelectMethod="GetOrderActionTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter">
                </asp:ObjectDataSource>            
            </td>
            <td style="width: 150px" align="left">&nbsp</td>
          </tr>

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

          <tr>
            <td>&nbsp</td>
            <td>
                <asp:CheckBox ID="chkExclude" runat="server" Text="Exclude instruments/submodels" AutoPostBack="true" OnCheckedChanged="chkExclude_CheckedChanged" />
            </td>
            <td style="width: 150px">&nbsp</td>
          </tr>
        </table>
        <asp:Panel ID="pnlSelector" runat="server" Visible="false" >
            <uc3:InstrumentsModelsSelector ID="ucInstrumentsModelsSelector" runat="server" />
        </asp:Panel>

        <br />
    </asp:Panel>
    <asp:Panel ID="pnlWithdrawalInstruction" runat="server" Visible="False">
        <table cellpadding="1" cellspacing="1" border="0" >
          <tr>
            <td>
                <asp:Label ID="lblWithdrawalDate" runat="server" Text="Withdrawal Date" />
            </td>
            <td>
                <uc2:DatePicker ID="dtpWithdrawalDate" runat="server" ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" IsButtonDeleteVisible="false" />
            </td>
            <td>
                <asp:RangeValidator
                    runat="server"
                    id="rvWithDrawalDate"
                    controlToValidate="dtpWithdrawalDate:txtDate"
                    Text="*"
                    errorMessage="Withdrawal date should be later or equal as today" 
                    Type="Date"
                    MinimumValue='2008-03-16'
                    MaximumValue='2035-03-25'/>&nbsp
                <asp:RequiredFieldValidator ID="rvfWithDrawalDate"
                    ControlToValidate="dtpWithdrawalDate:txtDate"
                    runat="server"
                    Text="*"
                    ErrorMessage="Withdrawal date is mandatory" />
            </td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblRelevanceDate" runat="server" Text="Relevance Date" />
            </td>
            <td>
                <uc2:DatePicker ID="dtpRelevanceDate" runat="server" ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" IsButtonDeleteVisible="false" />
            </td>
            <td>
                <asp:RangeValidator
                    runat="server"
                    id="rvRelevanceDate"
                    controlToValidate="dtpRelevanceDate:txtDate"
                    Text="*"
                    errorMessage="Relevance date should be later or equal as today" 
                    Type="Date"
                    MinimumValue='2008-03-16'
                    MaximumValue='2035-03-25'/>&nbsp
                <asp:CompareValidator 
                    ID="cvRelevanceDate" 
                    runat="server"
                    ControlToValidate="dtpRelevanceDate:txtDate"
                    ControlToCompare="dtpWithdrawalDate:txtDate"
                    Operator="LessThanEqual"
                    Type="Date"
                    Text="*" 
                    ErrorMessage="The relevance date can not be after the withdrawal date"/>&nbsp
                <asp:RequiredFieldValidator ID="rvfRelevanceDate"
                    ControlToValidate="dtpRelevanceDate:txtDate"
                    runat="server"
                    Text="*"
                    ErrorMessage="Relevance date is mandatory" />
                <cc1:TooltipImage ID="ttiRelevanceDaten" 
                    runat="server" 
                    TooltipContent="The date that this withdrawal instruction is taken into account in the system and shown on the client portfolio page. This can not be after the withdrawal date." 
                    TooltipShadowWidth="5"
                    TooltipClickClose="true" 
                    TooltipDefaultImage="Balloon_Small" 
                    TooltipPadding="8"
                    IsTooltipAbove="true"
                    OffSetX="-17" />
            </td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblWithdrawalAmount" runat="server" Text="Withdrawal Amount" />
            </td>
            <td>
                <db:DecimalBox ID="dbWithdrawalAmount" runat="server" DecimalPlaces="2" />
                <asp:Literal ID="liCurLabel" runat="server" Text="€" />
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvWithdrawalAmount"
                    ControlToValidate="dbWithdrawalAmount:tbDecimal"
                    runat="server"
                    Text="*"
                    ErrorMessage="Withdrawal Amount is mandatory" />
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblCounterAccount" runat="server" Text="Counter Account" />
            </td>
            <td>
                <asp:DropDownList 
                    ID="ddlCounterAccount" DataSourceID="odsCounterAccount" DataTextField="DisplayName" DataValueField="Key" 
                    SkinID="broad" runat="server" /> 
                <asp:ObjectDataSource ID="odsCounterAccount" runat="server" 
                    SelectMethod="GetPublicCounterAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountids" SessionField="SelectedAccountIds" Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblTransferDescription" runat="server">Transfer Description</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtTransferDescription" SkinID="custom-width" Width="245px" runat="server" /> 
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>

          <tr>
            <td>&nbsp</td>
            <td>
                <asp:CheckBox ID="chkNoChargesWithdrawal" runat="server" Text="No Commission" Checked="true" />
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>
        </table>
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlClientDepartureInstructionProps" runat="server" Visible="False">
        <table cellpadding="1" cellspacing="1" border="0" >

          <tr>
            <td>
                <asp:Label ID="lblExecutionDateDeparture" runat="server" Text="Execution Date" />
            </td>
            <td>
                <uc2:Calendar ID="cldExecutionDateDeparture" runat="server" IsButtonDeleteVisible="false" Format="dd-MM-yyyy" />
            </td>
            <td>
                <asp:RangeValidator
                    runat="server"
                    id="rvExecutionDateDeparture"
                    controlToValidate="cldExecutionDateDeparture:txtCalendar"
                    Text="*"
                    errorMessage="Date should be later or equal as today" 
                    Type="Date"
                    MinimumValue='2008-03-16'
                    MaximumValue='2035-03-25'/>&nbsp
                <asp:RequiredFieldValidator ID="rfvExecutionDateDeparture"
                    ControlToValidate="cldExecutionDateDeparture:txtCalendar"
                    runat="server"
                    Text="*"
                    ErrorMessage="Date is mandatory" />
            </td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblCounterAccountDeparture" runat="server" Text="Counter Account" />
            </td>
            <td>
                <asp:DropDownList 
                    ID="ddlCounterAccountDeparture" DataSourceID="odsCounterAccount" DataTextField="DisplayName" DataValueField="Key" 
                    SkinID="broad" runat="server" /> 
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblTransferDescriptionDeparture" runat="server">Transfer Description</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtTransferDescriptionDeparture" SkinID="custom-width" Width="245px" runat="server" /> 
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>

          <tr>
            <td>&nbsp</td>
            <td>
                <asp:CheckBox ID="chkNoChargesDeparture" runat="server" Text="No Commission" Checked="true" />
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>
        </table>
        <br />
    </asp:Panel>
    <span class="padding" style="display:block">
        <asp:ValidationSummary ID="vsInstructionProps" runat="server" ForeColor="Red" Height="0px" Width="450px"/>
    </span>
    <br />
    
    <asp:Button ID="btnRebalance" runat="server" OnClick="btnRebalance_Click" Text="Create Rebalance" Visible="False" Width="120px" />&nbsp;
    <asp:Button ID="btnWithdrawal" runat="server" OnClick="btnWithdrawal_Click" Text="Create Withdrawal" Visible="False" Width="120px" />&nbsp;
    <asp:Button ID="btnDeparture" runat="server" OnClick="btnDeparture_Click" Text="Create Departure" Visible="False" Width="120px" />&nbsp;
    <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" Visible="False" CausesValidation="False" />

    <asp:Panel ID="pnlInstructions" runat="server" Visible="false">
        <br/>
        <asp:GridView 
            ID="gvInstructions"
            runat="server"
            CellPadding="0"
            AutoGenerateColumns="False"
            DataKeyNames="Key"
            Caption="Instructions"
            CaptionAlign="Left"
            DataSourceID="odsInstructions" 
            AllowSorting="True" 
            SkinID="custom-width"
            Width="900px">
            <Columns>
                <asp:TemplateField HeaderText="Type" SortExpression="InstructionType" >
                    <ItemTemplate>
                        <%# (InstructionTypes)DataBinder.Eval(Container.DataItem, "InstructionType")%>
                    </ItemTemplate>
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Created" SortExpression="CreationDate">
                    <HeaderStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                    <ItemStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                    <ItemTemplate>
                        <trunc:TruncLabel ID="lblCreationDate"
                            runat="server"
                            Width="35"
                            Text='<%# Eval("CreationDate", "{0:dd-MM-yyyy}") %>' 
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="DisplayStatus">
                    <HeaderStyle wrap="False" />
                    <ItemStyle horizontalalign="Left" wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Message" SortExpression="Message">
                    <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <ItemTemplate>
                        <trunc:TruncLabel ID="lblMessage"
                            runat="server"
                            Width="70"
                            Text='<%# DataBinder.Eval(Container.DataItem, "Message") %>' 
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Orders Created" SortExpression="OrdersGenerated">
                    <HeaderStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                    <ItemStyle HorizontalAlign="Right" Wrap="False" cssclass="alignright" />
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "OrdersGenerated") == DBNull.Value || (Int32)DataBinder.Eval(Container.DataItem, "OrdersGenerated") == 0 ? "" : DataBinder.Eval(Container.DataItem, "OrdersGenerated")%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsInstructions" runat="server" SelectMethod="GetAccountActiveInstructions"
            TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter" >
            <SelectParameters>
                <asp:ControlParameter ControlID="gvAccounts" Name="accountId" PropertyName="SelectedValue"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:Button ID="btnHideInstructions" runat="server" OnClick="btnHideInstructions_Click" Text="Hide" CausesValidation="False" />
    </asp:Panel>
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>

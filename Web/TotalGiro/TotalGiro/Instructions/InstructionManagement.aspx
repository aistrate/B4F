<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="InstructionManagement.aspx.cs" Inherits="InstructionManagement" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../UC/InstrumentsModelsSelector.ascx" TagName="InstrumentsModelsSelector" TagPrefix="uc3" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc4" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts.Instructions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
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
        Caption="Instructions"
        CaptionAlign="Left"
        DataSourceID="odsInstructions" 
        AllowSorting="True"
        Visible="false" 
        style="position: relative; top: -10px;"
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
                    <uc4:AccountLabel ID="ctlAccountLabel" 
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
            <asp:TemplateField HeaderText="Type" SortExpression="InstructionType">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (InstructionTypes)DataBinder.Eval(Container.DataItem, "InstructionType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="DoNotChargeCommission" HeaderText="Excl. Com." SortExpression="DoNotChargeCommission">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:CheckBoxField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status" >
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
            <asp:BoundField DataField="ExecutionDate" HeaderText="Execution" SortExpression="ExecutionDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                         ID="lbtnEditInstruction"
                         runat="server"
                         CommandName="EditInstruction"
                         Text="Edit"
                         Visible='<%# DataBinder.Eval(Container.DataItem, "IsEditable") %>' />
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewOrders" 
                        Text="Orders" 
                        CommandName="ViewOrders"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "HasOrders") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </cc1:MultipleSelectionGridView>
    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False" Font-Bold="True"></asp:Label><asp:Label
        ID="lblResult" runat="server" Font-Bold="True" ForeColor="Black" Visible="False"></asp:Label><br />
    <asp:Button ID="btnProcessInstructions" runat="server" Text="Process" OnClick="btnProcessInstructions_Click" Visible="false" />&nbsp
    <asp:Button ID="btnCheckPrices" runat="server" Text="Check Prices" OnClick="btnCheckPrices_Click" Visible="false" />&nbsp
    <asp:Button ID="btnCancelInstructions" runat="server" Text="Cancel Instructions" OnClick="btnCancelInstructions_Click" Visible="false" />
    <br />
    <br />
    <asp:Label ID="lblTolerance" runat="server" Text="Tolerance" Visible="false" />&nbsp 
    <asp:TextBox ID="txtTolerance" runat="server"  Visible="false" Width="49px" SkinID="small" />&nbsp 
    <asp:DropDownList ID="ddlPricingTypes" runat="server" DataSourceID="odsPricingTypes" DataTextField="Description" DataValueField="Key" AutoPostBack="False"  Visible="false" Width="56px" Height="12px" SkinID="custom-width" />
    <asp:ObjectDataSource ID="odsPricingTypes" runat="server" SelectMethod="GetPricingTypes" TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionManagementAdapter" />
    <br />
    <asp:ObjectDataSource ID="odsInstructions" runat="server" SelectMethod="GetInstructions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionManagementAdapter" SortParameterName="sortColumn">
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
        Width=500px
        Visible="False"> 
        <Fields>
            <asp:TemplateField HeaderStyle-Width="120px" >
                <HeaderTemplate>
                    <asp:Label ID="lblInstructionID" runat="server">InstructionID</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="InstructionID" runat="server" 
                        Text='<%# DataBinder.Eval(Container.DataItem, "InstructionID") %>'>
                    </asp:Label>
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="120px" >
                <HeaderTemplate>
                    <asp:Label ID="lblOrderActionType" runat="server">Action Type</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList 
                        ID="ddlOrderActionTypes" runat="server"
                        DataSource='<%# B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter.GetOrderActionTypes() %>' 
                        DataTextField="Description" DataValueField="Key"
                        SelectedValue='<%# DataBinder.Eval(Container.DataItem, "OrderActionType") %>'>
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="120px" >
                <HeaderTemplate>
                    <asp:Label ID="lblExecDate" runat="server">Execution Date</asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <uc2:DatePicker ID="dtpExecDate" runat="server" 
                        ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" 
                        IsButtonDeleteVisible="false"
                        SelectedDate ='<%# DataBinder.Eval(Container.DataItem, "ExecutionDate") %>' />&nbsp
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
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Width="120px" >
                <HeaderTemplate>
                    <asp:Label ID="lblNoCharges" runat="server"></asp:Label>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkNoCharges" runat="server" Text="No Commission" Checked='<%# DataBinder.Eval(Container.DataItem, "DoNotChargeCommission") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="chkExclude" runat="server" Text="Exclude instruments/submodels" AutoPostBack="true" Checked="false" OnCheckedChanged="chkExclude_CheckedChanged" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:Panel ID="pnlSelector" runat="server" Visible="false" >
                        <uc3:InstrumentsModelsSelector ID="ucInstrumentsModelsSelector" runat="server" 
                            Exclusions='<%# DataBinder.Eval(Container.DataItem, "Exclusions") %>' />
                    </asp:Panel>
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
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionManagementAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="Key" PropertyName="SelectedValue" />
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
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionManagementAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvInstructionOverview" Name="instructionId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


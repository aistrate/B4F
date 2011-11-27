<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="FeeRuleOverview.aspx.cs" 
    Inherits="DataMaintenance_Fee_FeeRuleOverview" Title="Fee Rules Overview" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc2" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="cmg" runat="server" />
    <table cellpadding="0" cellspacing="0" border="0"  style="border-color:black;" width="780">
        <tr>
            <td class="tblHeader">Fee Rule Overview</td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width:1000px">
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblFeeCalc" runat="server" Text="Fee Calculation"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlFeeCalculationFilter" runat="server" Width="200px" 
                                DataSourceID="odsFeeCalculation" DataTextField="Name" DataValueField="Key" />
                        </td>
                        <td colspan="2" />
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblStartDate" runat="server" Text="Start Date" />
                        </td>
                        <td style="width: 420px;">
                            <b4f:YearMonthPicker ID="ppStartPeriodFilter" runat="server" />
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblEndDate" runat="server" Text="End Date"></asp:Label></td>
                        <td style="width: 420px;">
                            <b4f:YearMonthPicker ID="ppEndPeriodFilter" runat="server" />
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblModelPortfolio" runat="server" Text="Model"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlModelPortfolio" runat="server" Width="200px" DataSourceID="odsModelPortfolio" 
                                DataTextField="ModelName" DataValueField="Key" />
                            <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios" 
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblAccount" runat="server" Text="Account" />
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="205px" DataSourceID="odsAccount"
                                DataTextField="DisplayNumberWithName" DataValueField="Key" OnDataBound="ddlAccount_DataBound">
                            </asp:DropDownList>
                            <asp:Button ID="btnFilterAccount" runat="server" CausesValidation="false" Text="Filter  >>" Height="22px"
                                OnClick="btnFilterAccount_Click"/>
                            <asp:Panel ID="pnlAccountFinder" runat="server" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Visible="False">
                                <uc2:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true" />
                            </asp:Panel>
                            <asp:ObjectDataSource ID="odsAccount" runat="server" SelectMethod="GetCustomerAccounts"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                                        Type="String" />
                                    <asp:Parameter  Name="propertyList" DefaultValue="Key, DisplayNumberWithName" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>

                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblIsDefault" runat="server" Text="Is Default" />
                        </td>
                        <td style="width: 420px;">
                            <asp:CheckBox ID="chkIsDefault" runat="server" />
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblHasEmployerRelationFilter" runat="server" Text="Connected to Employer"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:CheckBox ID="chkHasEmployerRelationFilter" runat="server" />
                        </td>
                    </tr>

                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblExecutionOnlyFilter" runat="server" Text="Execution-Only"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:CheckBox ID="chkExecutionOnlyFilter" runat="server" />
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="lblSendByPostFilter" runat="server" Text="Send By Post"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:CheckBox ID="chkSendByPostFilter" runat="server" />
                        </td>
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
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
    <br />
    <asp:GridView ID="gvFeeRules" runat="server" Caption="Fee Rules"
        AllowPaging="True" AllowSorting="True" DataSourceID="odsFeeRules" AutoGenerateColumns="False"
        PageSize="20" SkinID="custom-EmptyDataTemplate" Width="100%" 
        DataKeyNames="Key"
        OnRowDataBound="gvFeeRules_RowDataBound"
        OnRowUpdating="gvFeeRules_RowUpdating" >
        <Columns>
            <asp:TemplateField HeaderText="Calculation" SortExpression="FeeCalculation_Name">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "FeeCalculation_Name") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlFeeCalculationID" runat="server" Width="200px" 
                        DataSourceID="odsFeeCalculation" DataTextField="Name" DataValueField="Key" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="IsDefault" HeaderText="Default" SortExpression="IsDefault" ReadOnly="true" >
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Characteristics" SortExpression="DisplayRule">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlDisplayRule" 
                        runat="server"
                        cssclass="alignright"
                        Width="35"
                        Text='<%# DataBinder.Eval(Container.DataItem, "DisplayRule") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start Period" SortExpression="StartPeriod">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "StartPeriod")%>
                </ItemTemplate>
                <EditItemTemplate>
                    <b4f:YearMonthPicker ID="ppStartPeriod" runat="server" ListYearsBeforeCurrent="5" IsButtonDeleteVisible="true"
                        SelectedPeriod='<%# (int)DataBinder.Eval(Container.DataItem, "StartPeriod")%>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End Period" SortExpression="EndPeriod">
                <ItemTemplate>
                    <%# (int)DataBinder.Eval(Container.DataItem, "EndPeriod") == 0 ?
                        "" : DataBinder.Eval(Container.DataItem, "EndPeriod")%>
                </ItemTemplate>
                <EditItemTemplate>
                    <b4f:YearMonthPicker ID="ppEndPeriod" runat="server" ListYearsBeforeCurrent="5" IsButtonDeleteVisible="true"
                        SelectedPeriod='<%# (int)DataBinder.Eval(Container.DataItem, "EndPeriod")%>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" >
                <ItemStyle Wrap="False" />
            </asp:CommandField>
       </Columns>
        <EmptyDataTemplate>
            <asp:Label ID="lblEmptyDataTemplateCR" runat="server" Text="No Fee Rules" />
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFeeRules" runat="server" SelectMethod="GetFeeRules"
        UpdateMethod="UpdateFeeRule" OldValuesParameterFormatString="original_{0}"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeRuleOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlFeeCalculationFilter" Name="feeCalcId" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlModelPortfolio" Name="modelId"  PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlAccount" Name="accountId" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ppStartPeriodFilter" Name="startPeriod" PropertyName="SelectedPeriod" Type="Int32" />
            <asp:ControlParameter ControlID="ppEndPeriodFilter" Name="endPeriod" PropertyName="SelectedPeriod" Type="Int32" />
            <asp:ControlParameter ControlID="chkIsDefault" Name="isDefault" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkHasEmployerRelationFilter" Name="hasEmployerRelation"  PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkExecutionOnlyFilter" Name="executionOnly"  PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkSendByPostFilter" Name="sendByPost"  PropertyName="Checked" Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Panel ID="pnlFeeRuleEntry" runat="server" Visible="false">
        <table width="400px" cellpadding="0" cellspacing="0" style="border: 1px solid #000000" >
            <tr></tr>
            <tr>
                <td>
                    <asp:Label ID="lblFeeCalculation" runat="server" Text="Fee Calculation" />
                </td>
                <td>
                    <asp:DropDownList 
                        ID="ddlFeeCalculation" DataSourceID="odsFeeCalculation" DataTextField="Name" 
                            DataValueField="Key" runat="server" SkinID="broad" /> 
                    <asp:ObjectDataSource ID="odsFeeCalculation" runat="server" 
                        SelectMethod="GetActiveFeeCalculations"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeRuleOverviewAdapter">
                    </asp:ObjectDataSource>
                </td>
                <td style="width: 50px">
                    <asp:RequiredFieldValidator ID="rfvFeeCalculation" runat="server" ControlToValidate="ddlFeeCalculation"
                        ErrorMessage="Fee Calculation is mandatory" InitialValue="-2147483648" SetFocusOnError="True" Width="0px">*</asp:RequiredFieldValidator>
                </td>
            </tr>

            <tr>
                <td>&nbsp</td>
                <td>
                    <asp:CheckBox ID="chkExecutionOnly" runat="server" Text="Execution-Only" Checked="false" />
                </td>
                <td></td>
            </tr>

            <tr>
                <td>&nbsp</td>
                <td>
                    <asp:CheckBox ID="chkSendByPost" runat="server" Text="Send By Post" Checked="false" />
                </td>
                <td></td>
            </tr>

            <tr>
                <td>&nbsp</td>
                <td>
                    <asp:CheckBox ID="chkHasEmployerRelation" runat="server" Text="Is connected to Employer" Checked="false" />
                </td>
                <td></td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="lblStartPeriod" runat="server" Text="Start Period" />
                </td>
                <td>
                    <b4f:YearMonthPicker ID="ppStartPeriod" runat="server" ListYearsBeforeCurrent="1" DefaultToCurrentPeriod="true" IsButtonDeleteVisible="false" />
                </td>
                <td>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="lblEndPeriod" runat="server" Text="End Period" />
                </td>
                <td>
                    <b4f:YearMonthPicker ID="ppEndPeriod" runat="server" ListYearsBeforeCurrent="0"  />
                </td>
                <td>
                    <asp:CustomValidator ID="cvEndPeriod" ErrorMessage="The end period can not be before the start period"
                        ValidationGroup="CreateFeeRule" ControlToValidate="ppEndPeriod" runat="server" 
                        OnServerValidate="cvEndPeriod_ServerValidate">*</asp:CustomValidator>
                </td>
            </tr>
            <tr></tr>

        </table>
    </asp:Panel>
    <asp:Button ID="btnCreateFeeRule" runat="server" Text="Create Default Fee Rule" OnClick="btnCreateFeeRule_Click"
        CausesValidation="true" ValidationGroup="CreateFeeRule" />
    <asp:Button ID="btnCancelCreateFeeRule" runat="server" Text="Cancel" OnClick="btnCancelCreateFeeRule_Click"
        CausesValidation="false" Visible="false" />
    <br />
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="RuleOverview.aspx.cs" Inherits="RuleOverview" Title="Commission Rules Overview" %>

<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc2" %>
<%@ Register Src="../../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc3" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table cellpadding="0" cellspacing="0" border="0"  style="border-color:black;" width="780">
        <tr>
            <td class="tblHeader">Commission Rule Overview</td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width:1000px">
                    <tr style="height :25px;">
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="Label1" runat="server" Text="Commission Rule Name"></asp:Label></td>
                        <td style="width: 420px;">
                            <asp:TextBox ID="txtCommissionRuleName" runat="server" Width="194px"></asp:TextBox>
                            </td>
                        <td style="width: 300px;" align="right">
                            <asp:Label ID="Label6" runat="server" Text="Start Date"></asp:Label></td>
                        <td style="width: 600px;">
                            <uc1:DatePicker ID="DatePickerStartDate" runat="server" />
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label2" runat="server" Text="Commission Rule Type"></asp:Label></td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlCommissionRuleType" runat="server" Width="200px" 
                                DataSourceID="odsCommissionRuleType" DataTextField="Description" DataValueField="Key" />
                            <asp:ObjectDataSource ID="odsCommissionRuleType" runat="server" SelectMethod="GetCommRuleTypes"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter">
                            </asp:ObjectDataSource>
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label7" runat="server" Text="End Date"></asp:Label></td>
                        <td style="width: 420px;">
                            <uc1:DatePicker ID="DatePickerEndDate" runat="server" />
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right"><asp:Label ID="Label11" runat="server" Text="Model"></asp:Label></td>
                        <td style="width: 420px;"><asp:DropDownList ID="ddlModelPortfolio" runat="server" Width="200px" DataSourceID="odsModelPortfolio" DataTextField="ModelName" DataValueField="Key"></asp:DropDownList>
                        </td>
                        <td style="width: 200px;" align="right"><asp:Label ID="Label10" runat="server" Text="Account"></asp:Label></td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="205px" DataSourceID="odsAccount"
                                DataTextField="DisplayNumberWithName" DataValueField="Key" OnDataBound="ddlAccount_DataBound">
                                <asp:ListItem></asp:ListItem>
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
                        <td style="width: 200px;" align="right"><asp:Label ID="Label9" runat="server" Text="Exchange"></asp:Label></td>
                        <td style="width: 420px; ">
                            <asp:DropDownList ID="ddlExchange" runat="server" Width="200px" DataSourceID="odsExchange" DataTextField="ExchangeName" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 200px;" align="right"><asp:Label ID="Label8" runat="server" Text="Instrument"></asp:Label></td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlInstrument" SkinID="custom-width" runat="server" Width="205px" 
                                DataSourceID="odsInstrument" DataTextField="DisplayIsinWithName" DataValueField="Key">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button ID="btnFilterInstrument" runat="server" CausesValidation="false" Text="Filter  >>" Height="22px"
                                OnClick="btnFilterInstrument_Click" />
                            <asp:Panel ID="pnlInstrumentFinder" runat="server" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" Visible="False" >
                                <uc3:InstrumentFinder ID="ctlInstrumentFinder" runat="server" />
                            </asp:Panel>
                            <asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetTradeableInstruments"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                                        Type="Object" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="exchangeId" PropertyName="ExchangeId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label4" runat="server" Text="Buy/Sell"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlBuySell" runat="server" Width="80px"
                                DataSourceID="odsBuySell" DataTextField="Description" DataValueField="Key" />
                            <asp:ObjectDataSource ID="odsBuySell" runat="server" SelectMethod="GetBuySellOptions"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                            </asp:ObjectDataSource>                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label5" runat="server" Text="Security Category"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlSecCategory" runat="server" Width="370px" DataSourceID="odsSecCategories" DataTextField="Description" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label3" runat="server" Text="Commission Calculation"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlCommissionCalculation" runat="server" Width="200px" DataSourceID="odsCommissionCalculation" DataTextField="Name" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label13" runat="server" Text="Additional Calculation"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlAdditionalCalculation" runat="server" Width="200px" DataSourceID="odsCommissionCalculation" DataTextField="Name" DataValueField="Key">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label12" runat="server" Text="Order action type"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:DropDownList ID="ddlOrderActionType" runat="server" Width="370px"
                                DataSourceID="odsOrderActionType" DataTextField="Description" DataValueField="Key" />
                            <asp:ObjectDataSource ID="odsOrderActionType" runat="server" SelectMethod="GetOrderActionTypeOptions"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                            </asp:ObjectDataSource>
                        </td>
                        <td style="width: 200px;" align="right">
                            <asp:Label ID="Label14" runat="server" Text="Apply to all accounts"></asp:Label>
                        </td>
                        <td style="width: 420px;">
                            <asp:CheckBox ID="cbApplyToAllAccounts" runat="server" />
                        </td>
                    </tr>
                    <tr style="height :25px;">
                        <td style="height: 37px; width: 230px;" colspan="4">
                            <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Text="Refresh" />
                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" OnClick="btnResetFilter_Click" />
                        </td>
                    </tr>
                </table>
                <asp:ObjectDataSource ID="odsCommissionCalculation" runat="server" SelectMethod="GetCommissionCalculations"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges" 
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsSecCategories" runat="server" SelectMethod="GetSecCategories" 
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios" 
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="odsAccountType" runat="server" SelectMethod="GetAccountTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>

    <asp:GridView ID="gvCommissionRules" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsCommissionRules" PageSize="20" DataKeyNames="Key" OnRowCommand="gvCommissionRules_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="Rule Name" SortExpression="CommRuleName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlCommRuleName" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="35"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "CommRuleName") %>'
                        ForeColor='<%# ((bool)DataBinder.Eval(Container.DataItem, "IsAccountActive") ? System.Drawing.Color.Black : System.Drawing.Color.Gray) %>'
                        PermanentToolTip='<%# ((bool)DataBinder.Eval(Container.DataItem, "IsAccountActive") ? "" : "This Account is not Active!!") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            
            
            <asp:BoundField DataField="CommCalculation_Name" HeaderText="Calculation" SortExpression="CommCalculation_Name">
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
            <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                <ItemTemplate>
                    <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")) ?
                        "" : ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToString("dd MMM yyyy"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                <ItemTemplate>
                    <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) ?
                        "" : ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToString("dd MMM yyyy"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtEdit" 
                        Text="Edit" 
                        CommandName="EditRule"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDelete" 
                        Text="Delete"
                        CommandName="DeleteRule"
                        OnClientClick="return confirm('Delete rule?')"/>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCommissionRules" runat="server" SelectMethod="GetCommissionRules"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">    
        <SelectParameters>
            <asp:ControlParameter ControlID="txtCommissionRuleName" Name="commrulename"  PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ddlCommissionRuleType" Name="commRuleTypeId" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlModelPortfolio" Name="modelId"  PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlAccount" Name="accountId" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlInstrument" Name="instrumentId"  PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlBuySell" Name="buySell"  PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlSecCategory" Name="secCategoryId" PropertyName="SelectedValue" Type="int32" />
            <asp:ControlParameter ControlID="ddlExchange" Name="exchangeId" PropertyName="SelectedValue" Type="int32" />
            <asp:ControlParameter ControlID="DatePickerStartDate" Name="startdate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="DatePickerEndDate" Name="enddate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="ddlCommissionCalculation" Name="commcalcId" PropertyName="SelectedValue" Type="int32" />
            <asp:ControlParameter ControlID="ddlOrderActionType" Name="orderactiontype"  PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlAdditionalCalculation" Name="additionalCalcId" PropertyName="SelectedValue" Type="int32" />
            <asp:ControlParameter ControlID="cbApplyToAllAccounts" Name="applytoallaccounts"  PropertyName="Checked" Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add new" CausesValidation="false"/></asp:Content>

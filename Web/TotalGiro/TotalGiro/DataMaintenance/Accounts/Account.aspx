<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="Account.aspx.cs" Inherits="DataMaintenance_Account" Title="Account"
    EnableEventValidation="false" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="uc2" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc3" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="uc4" %>
<%@ Register Src="../../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc5" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="B4F.TotalGiro.Utils" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts.ManagementPeriods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="cmg" runat="server" />
    <asp:HiddenField ID="hfAssetMAnagerID" runat="server" />
    <asp:HiddenField ID="hfAccountCategory" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
        <ContentTemplate>
            <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td colspan="20">
                <uc3:BackButton ID="BackButton" runat="server" />
                <uc2:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton2" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnlErrorMess" runat="server" Visible="false">
            <tr>
                <td colspan="20" style="color: Red; height: 30px;">
                    <asp:Label ID="lblMess" Font-Bold="true" runat="server" />
                </td>
            </tr>
        </asp:Panel>
    </table>
    <br />
    
    <table width="1000px" cellpadding="0" cellspacing="0" style="border: 1px solid #000000">
        <tr>
            <td colspan="20" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                border-bottom: solid 1px black;">
                <asp:Label ID="lblAccount" Font-Bold="true" runat="server" Text="Account Details" />
            </td>
        </tr>
        <tr>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
            <td style="width: 50px">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: right; height: 25px;">
                <asp:Label ID="lblAssetManagerLabel" runat="server" Text="Asset Manager:"></asp:Label>
            </td>
            <td colspan="1" style="height: 25px;">
            </td>
            <td colspan="4" style="height: 25px;">
                <asp:Label ID="lblAssetManager" Font-Bold="True" Font-Size="Large" runat="server"></asp:Label>
            </td>
            <td colspan="1">
            </td>
            <td colspan="6" style="height: 24px">
                <asp:Label ID="lblNarDetails" Font-Bold="true" runat="server" Text="Contact NAR Details are IN Order" />
            </td>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: right; height: 25px;">
                <asp:Label ID="lblAccountFullNameLabel" runat="server" Text="Account Full Name:"></asp:Label>
            </td>
            <td colspan="1" style="height: 25px;">
            </td>
            <td colspan="14" style="height: 25px;">
                <asp:TextBox ID="tbFullName" SkinID="custom-width" Width="750px" MaxLength="150"
                    runat="server"></asp:TextBox>
            </td>
            <td colspan="1" style="height: 25px;">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: right; height: 25px;">
                <asp:Label ID="lblAccountShortNameLabel" runat="server" Text="Account Short Name:"></asp:Label>
            </td>
            <td colspan="1" style="height: 25px;">
            </td>
            <td colspan="5" style="height: 25px;">
                <asp:TextBox ID="tbShortName" SkinID="custom-width" Width="244px" runat="server"></asp:TextBox>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblAccountNumberHeader" runat="server" Text="Account Number:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="4" style="height: 24px">
                <uc5:AccountLabel ID="ctlAccountLabel" RetrieveData="false" runat="server" Font-Bold="True" Width="120px" NavigationOption="PortfolioView" />
            </td>
            <td colspan="1">
            </td>
        </tr>

        <tr>
            <td colspan="4" style="text-align: right; height: 25px;">
                <asp:Label ID="lblAccountFamily" runat="server" Text="Account Family:"></asp:Label>
            </td>
            <td colspan="1" style="height: 25px;">
            </td>
            <td colspan="5" style="height: 25px;">
                <asp:DropDownList ID="ddlAccountFamily" DataSourceID="odsAccountFamily" DataTextField="AccountPrefix"
                    DataValueField="Key" SkinID="broad" runat="server" />
                <asp:ObjectDataSource ID="odsAccountFamily" runat="server" SelectMethod="GetAccountFamilies"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountOverviewAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountID" SessionField="accountnrid" Type="Int32" DefaultValue="0" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
            <td colspan="1">
                <asp:RequiredFieldValidator 
                    ID="rfvAccountFamily" 
                    runat="server"
                    ControlToValidate="ddlAccountFamily"
                    SetFocusOnError="True"
                    InitialValue="-2147483648" 
                    ErrorMessage="Account Family is mandatory">*</asp:RequiredFieldValidator>
            </td>
            <td colspan="4">
            </td>
            <td colspan="4">
                <asp:CheckBox ID="chkIsJointAccount" runat="server" Text="Is Joint Account" />
            </td>
            <td colspan="1">
            </td>
        </tr>
        <asp:Panel ID="pnlLifecycle" runat="server"  Visible="false" >
            <tr>
                <td colspan="4" style="text-align: right; height: 24px;">
                    <asp:Label ID="lblLifecycle" runat="server" Text="Lifecycle:"></asp:Label>
                </td>
                <td colspan="1" style="height: 24px;">
                </td>
                <td colspan="5" style="height: 24px;">
                    <asp:DropDownList ID="ddlLifecycle" SkinID="broad" runat="server" AutoPostBack="true"
                        DataTextField="Name" DataValueField="Key" DataSourceID="odsLifecycle"
                        OnSelectedIndexChanged="ddlLifecycle_SelectedIndexChanged" >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsLifecycle" runat="server" SelectMethod="GetLifecycles"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountOverviewAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="hfAssetMAnagerID" PropertyName="Value" Name="assetManagerId" Type="Int32" DefaultValue="0" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
                <td colspan="10">
                </td>
            </tr>
        </asp:Panel>  
        <tr>
            <td colspan="4" style="text-align: right; height: 24px;">
                <asp:Label ID="lblModelLabel" runat="server" Text="Model:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="5" style="height: 24px;">
                <asp:DropDownList ID="ddlModelPortfolio" SkinID="broad" runat="server" AutoPostBack="true"
                    DataTextField="ModelName" DataValueField="Key" DataSourceID="odsModelPortfolio"
                    OnSelectedIndexChanged="ddlModelPortfolio_SelectedIndexChanged" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hfAssetMAnagerID" PropertyName="Value" Name="assetManagerId" Type="Int32" DefaultValue="0" />
                        <asp:Parameter Name="activityFilter" Type="Int32" DefaultValue="1"  />
                    </SelectParameters>
                </asp:ObjectDataSource>
                
            </td>
            <td colspan="5">
            </td>
            <td colspan="4">
                <asp:CheckBox ID="chkIsExecOnlyCustomer" runat="server" Text="Is Execution-Only Customer" />
            </td>
            <td colspan="1">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblCounterAccount" runat="server" Text="Counter Account:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:DropDownList ID="ddlCounterAccount" DataSourceID="odsCounterAccount" DataTextField="DisplayName"
                    DataValueField="Key" SkinID="broad" runat="server" />
                <asp:ObjectDataSource ID="odsCounterAccount" runat="server" SelectMethod="GetAttachedCounterAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountID" SessionField="accountnrid" Type="Int32" DefaultValue="0" />
                        <asp:SessionParameter Name="contactID" SessionField="contactid" Type="Int32" DefaultValue="0" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblFirstDeposit" runat="server" Text="First (promised) Deposit:">
                </asp:Label>
            </td>
            <td colspan="1">
            </td>
            <td colspan="4">
                <uc4:DecimalBox ID="dbFirstDeposit" runat="server" DecimalPlaces="2" />
                <asp:Label ID="lblFirstDepositLabel" runat="server" Text="€">
                </asp:Label>
            </td>
            <td colspan="1">
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblVerpandSoortLabel" runat="server" Text="Verpand Soort:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:UpdatePanel ID="updVerpandSoort" runat="server" >
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlVerpandSoort" SkinID="broad" DataSourceID="odsVerpandSoort"
                            DataTextField="Description" DataValueField="Key" AutoPostBack="true" runat="server" 
                            OnSelectedIndexChanged="ddlVerpandSoort_SelectedIndexChanged"/>
                        <asp:RequiredFieldValidator 
                            ID="rfvVerpandSoort" 
                            runat="server"
                            Enabled="false" 
                            ControlToValidate="ddlVerpandSoort"
                            SetFocusOnError="True"
                            InitialValue="-2147483648" 
                            ErrorMessage="Verpand soort is mandatory when pandhouder is selected">*</asp:RequiredFieldValidator>
                        <asp:ObjectDataSource ID="odsVerpandSoort" runat="server" SelectMethod="GetVerpandSoort"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblCreationDateLabel" runat="server" Text="Account Created on:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="4" style="height: 24px;">
                <asp:Label ID="lblCreationDate" runat="server" />
            </td>
            <td colspan="1" style="height: 24px">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblPandhouderLabel" runat="server" Text="Pandhouder:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:UpdatePanel ID="updPandhouder" runat="server" >
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlPandhouder" SkinID="custom-width" Width="200px" DataSourceID="odsPandhouder"
                            DataTextField="Name" DataValueField="Key" AutoPostBack="true" runat="server"
                            OnSelectedIndexChanged="ddlPandhouder_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator 
                            ID="rfvPandhouder" 
                            runat="server"
                            Enabled="false" 
                            ControlToValidate="ddlPandhouder"
                            SetFocusOnError="True"
                            InitialValue="-2147483648" 
                            ErrorMessage="Pandhouder is mandatory when verpand soort is selected">*</asp:RequiredFieldValidator>
                        <asp:ObjectDataSource ID="odsPandhouder" runat="server" SelectMethod="GetPandHouders"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblLastupdatedLabel" runat="server" Text="Account LastUpdated on:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="4" style="height: 24px">
                <asp:Label ID="lblLastupdated" runat="server" />
            </td>
            <td colspan="1">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblTradeability" runat="server" Text="Tradeability:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:DropDownList ID="ddlTradeability" runat="server" DataTextField="Name" DataValueField="Key"
                    SkinID="custom-width" Width="200px" DataSourceID="odsAccountTradeabilityStatuses">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsAccountTradeabilityStatuses" runat="server" SelectMethod="GetAccountTradeabilityStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                </asp:ObjectDataSource>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblLastDateTradeabilityChangedLabel" runat="server" Text="Date Tradeability Changed:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="4" style="height: 24px">
                <asp:Label ID="lblLastDateTradeabilityChanged" runat="server" />
            </td>
            <td colspan="1">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblStatusLabel" runat="server" Text="Status:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:DropDownList ID="ddlStatus" runat="server" DataTextField="Name" DataValueField="Key"
                    DataSourceID="odsStatus" SkinID="custom-width" Width="200px">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsStatus" runat="server" SelectMethod="GetAccountStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                </asp:ObjectDataSource>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblLastDateStatusChangedLabel" runat="server" Text="Date Status Changed:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="4" style="height: 24px">
                <asp:Label ID="lblLastDateStatusChanged" runat="server" />
            </td>
            <td colspan="1">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblFirstManagementStartDate" runat="server" Text="First Start Date:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:UpdatePanel ID="updFirstManagementStartDate" runat="server" >
                    <ContentTemplate>
                        <uc1:DatePicker ID="dpFirstManagementStartDate" runat="server" IsButtonDeleteVisible="false" Enabled="false" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="5" style="height: 24px;">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:CheckBox ID="chkUseManagementFee" runat="server" Text="Use Management Fee" Checked="true" />
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblFinalManagementEndDate" runat="server" Text="Final End Date:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:UpdatePanel ID="updFinalManagementEndDate" runat="server"  >
                    <ContentTemplate>
                        <uc1:DatePicker ID="dpFinalManagementEndDate" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="10" style="height: 24px">
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlExitFeePayer" Visible="false">
            <tr>
                <td colspan="4" style="height: 24px; text-align: right">
                    <asp:Label ID="lblExitFeePayer" runat="server" Text="Different Exit-Fee Payer:"></asp:Label>
                </td>
                <td colspan="1" style="height: 24px">
                </td>
                <td colspan="5" style="height: 24px">
                    <asp:DropDownList ID="ddlExitFeePayer" DataSourceID="odsExitFeePayer" DataTextField="DisplayNumberWithName"
                        DataValueField="Key" SkinID="broad" runat="server" />
                    <asp:ObjectDataSource ID="odsExitFeePayer" runat="server" SelectMethod="GetOtherOwnAccounts"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                        <SelectParameters>
                            <asp:SessionParameter Name="accountID" SessionField="accountnrid" Type="Int32" DefaultValue="0" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
                <td colspan="10" style="height: 24px">
                </td>
            </tr>
        </asp:Panel>

        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblRemisier" runat="server" Text="Remisier:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:UpdatePanel ID="updRemisier" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlRemisier" runat="server" SkinID="broad" DataSourceID="odsRemisier" AutoPostBack="true"
                            DataValueField="Key" DataTextField="DisplayName" OnSelectedIndexChanged="ddlRemisier_SelectedIndexChanged" >
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsRemisier" runat="server" SelectMethod="GetRemisiers"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter"></asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblKickbackPercentage" runat="server" Text="Kickback Percentage:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="2" style="height: 24px">
                <asp:UpdatePanel ID="updKickbackPercentage" runat="server" UpdateMode="Conditional" >
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRemisierEmployee" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkUseKickBack" EventName="CheckedChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <uc4:DecimalBox ID="dbKickbackPercentage" runat="server" DecimalPlaces="3" Width="30px" />
                        <asp:Label ID="lblKickbackPercentageSign" runat="server" Text="%" />&nbsp
                        <asp:RangeValidator ID ="rvKickbackPercentage" runat="server" 
                            ControlToValidate="dbKickbackPercentage:tbDecimal"  
                            MaximumValue="15"
                            Type="Double"
                            Text="*"
                            ErrorMessage="Kick back must be a valid percentage" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="2">
                <asp:UpdatePanel ID="updUseKickBack" runat="server" UpdateMode="conditional" >
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRemisierEmployee" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRemisier" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:CheckBox ID="chkUseKickBack" runat="server" Text="Use Kickback" Enabled="false"
                            OnCheckedChanged="chkUseKickBack_CheckedChanged" AutoPostBack="true" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="1">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblRemisierEmployee" runat="server" Text="Remisier Employee:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:UpdatePanel ID="updRemisierEmployee" runat="server" UpdateMode="conditional" >
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRemisier" EventName="SelectedIndexChanged" />
                        <asp:PostBackTrigger ControlID="dpFirstManagementStartDate"/>
                        <asp:PostBackTrigger ControlID="dpFinalManagementEndDate"/>
                    </Triggers>
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlRemisierEmployee" runat="server" SkinID="broad" DataValueField="Key" 
                            DataTextField="Employee_FullNameLastNameFirst" DataSourceID="odsRemisierEmployee" 
                            OnSelectedIndexChanged="ddlRemisierEmployee_SelectedIndexChanged" AutoPostBack="true" >
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsRemisierEmployee" runat="server" SelectMethod="GetRemisierEmployees"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                            <SelectParameters>
                                <asp:ControlParameter Name="remisierID" ControlID="ddlRemisier" PropertyName="SelectedValue" Type="Int32"/>
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblIntroductionFee" runat="server" Text="Kosten initiële/vervolg storting:" />
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="2" style="height: 24px">
                <uc4:DecimalBox ID="dbIntroductionFee" runat="server" DecimalPlaces="3" Width="30px" />
                <asp:Label ID="lblIntroductionFeePercentageSign" runat="server" Text="%" />&nbsp
                <asp:RangeValidator ID ="rvIntroductionFee" runat="server" 
                    ControlToValidate="dbIntroductionFee:tbDecimal"  
                    MaximumValue="15"
                    Type="Double"
                    Text="*"
                    ErrorMessage="Kosten initiële storting must be a valid percentage" />
            </td>
            <td colspan="3" style="height: 24px">
                <uc4:DecimalBox ID="dbSubsequentDepositFee" runat="server" DecimalPlaces="3" Width="30px" />
                <asp:Label ID="lblSubsequentDepositFeePercentageSign" runat="server" Text="%" />&nbsp
                <asp:RangeValidator ID ="rvSubsequentDepositFee" runat="server" 
                    ControlToValidate="dbSubsequentDepositFee:tbDecimal"  
                    MaximumValue="15"
                    Type="Double"
                    Text="*"
                    ErrorMessage="Kosten vervolg storting must be a valid percentage" />
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblEmployerRelationship" runat="server" Text="Employee relationship:"></asp:Label>
            </td>
            <td colspan="1" style="height: 24px">
            </td>
            <td colspan="5" style="height: 24px">
                <asp:DropDownList ID="ddlEmployerRelationship" runat="server" SkinID="broad"
                    DataValueField="Key" DataTextField="Description" 
                    DataSourceID="odsEmployerRelationship" 
                    onselectedindexchanged="ddlEmployerRelationship_SelectedIndexChanged" 
                    AutoPostBack="true" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsEmployerRelationship" runat="server" SelectMethod="GetEmployerRelationshipList"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                </asp:ObjectDataSource>
            </td>
            <td colspan="4" style="height: 24px; text-align: right">
                <asp:Label ID="lblIntroductionFeeReduction" runat="server" Text="Afslag initiële/vervolg storting:" />
            </td>
            <td colspan="1" style="height: 24px;">
            </td>
            <td colspan="2" style="height: 24px">
                <uc4:DecimalBox ID="dbIntroductionFeeReduction" runat="server" DecimalPlaces="3" Width="30px" />
                <asp:Label ID="lblIntroductionFeeReductionPercentageSign" runat="server" Text="%" />&nbsp
                <asp:CompareValidator 
                    ID="cvIntroductionFees" 
                    runat="server"
                    ControlToValidate="dbIntroductionFeeReduction:tbDecimal"
                    ControlToCompare="dbIntroductionFee:tbDecimal"
                    Operator="LessThanEqual"
                    Type="Double"
                    Text="*" 
                    ErrorMessage="Afslag initiële storting should be less than the kosten initiële storting"/>&nbsp
            </td>
            <td colspan="3" style="height: 24px">
                <uc4:DecimalBox ID="dbSubsequentDepositFeeReduction" runat="server" DecimalPlaces="3" Width="30px" />
                <asp:Label ID="lblSubsequentDepositFeeReductionPercentageSign" runat="server" Text="%" />&nbsp
                <asp:CompareValidator 
                    ID="cvSubsequentDepositFeeReduction" 
                    runat="server"
                    ControlToValidate="dbSubsequentDepositFeeReduction:tbDecimal"
                    ControlToCompare="dbSubsequentDepositFee:tbDecimal"
                    Operator="LessThanEqual"
                    Type="Double"
                    Text="*" 
                    ErrorMessage="Afslag vervolg storting should be less than the kosten vervolg storting"/>&nbsp
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlRelatedEmployee" Visible="false">
            <tr>
                <td colspan="4" style="height: 24px; text-align: right">
                    <asp:Label ID="lblRelatedEmployee" runat="server" Text="Related Employee:"></asp:Label>
                </td>
                <td colspan="1" style="height: 24px">
                </td>
                <td colspan="5" style="height: 24px">
                    <asp:DropDownList ID="ddlRelatedEmployee" runat="server" SkinID="broad"
                        DataValueField="Key" DataTextField="UserName" DataSourceID="odsRelatedEmployee" >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsRelatedEmployee" runat="server" SelectMethod="GetEmployees"
                        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                    </asp:ObjectDataSource>
                </td>
                <td colspan="10" style="height: 24px">
                </td>
            </tr>
        </asp:Panel>                    
    </table>

    <table border="0" cellpadding="0" cellspacing="0" style="width: 1000px">
        <tr style="font-size: xx-small; height: 3px;">
            <td colspan="20">
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:MultiView ID="mvwAccountSave" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwButtons" runat="server">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 1000px">
                            <tr>
                                <td>
                                    <asp:Button ID="btnSaveAccount" runat="server" OnClick="btnSaveAccount_Click" Text="Save Changes"
                                        Enabled="false" />&nbsp;&nbsp;
                                </td>
                                <td>
                                </td>
                                <td td align="right">
                                    <asp:Button ID="btnShowPortfolio" runat="server" OnClick="btnShowPortfolio_Click" CausesValidation="false"
                                        Text="Show Portfolio" />&nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="vwQuestion" runat="server">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 1000px">
                            <tr>
                                <td colspan="4">
                                    <asp:Label ID="lblWarningMessage" runat="server" ForeColor="Red" Width="700px">Are you sure you want to close this account?</asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnYes" runat="server" Width="60px" Text="Yes" OnClick="btnYes_Click" />
                                </td>
                                <td colspan="1">
                                </td>
                                <td>
                                    <asp:Button ID="btnNo" runat="server" Width="60px" Text="No" OnClick="btnNo_Click" />
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr>
            <td colspan="20" style="height: 24px">
            </td>
        </tr>

        <tr>
            <td colspan="20">
                <asp:GridView ID="gvAccountholders" Caption="AccountHolders" SkinID="custom-width"
                    DataKeyNames="Contact_Key" Width="100%" AutoGenerateColumns="False" runat="server"
                    DataSourceID="odsAccountholders" OnRowCommand="gvAccountholders_RowCommand" OnRowDataBound="gvAccountholders_RowDataBound">
                    <Columns>
                        <asp:BoundField HeaderText="FullName" DataField="Contact_FullName" SortExpression="Contact_FullName" />
                        <asp:BoundField HeaderText="Account Short Name" DataField="GiroAccount_ShortName"
                            SortExpression="GiroAccount_ShortName" />
                        <asp:BoundField DataField="CreationDate" HeaderText="Map Date" SortExpression="CreationDate" 
                            DataFormatString="{0:d MMM yyyy}" HtmlEncode="False" ReadOnly="true" />
                        <asp:BoundField HeaderText="PrimaryAccountHolder" DataField="IsPrimaryAccountHolder"
                            SortExpression="IsPrimaryAccountHolder" />
                        <asp:TemplateField>
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEdit" Visible="true" Enabled="false" runat="server" CausesValidation="false"
                                    ToolTip="Edit this contact" CommandName="EditContact" Text="Edit" />
                                <asp:LinkButton ID="lbtnDetach" Visible="true" Enabled="false" runat="server" CausesValidation="false"
                                    ToolTip="Detaches this contact as an accountholder." CommandName="DetachAccountHolder"
                                    Text="Detach" OnClientClick="if (this.disabled == false) return confirm('Detach Account Holder?');" />
                                <asp:LinkButton ID="lbtnMakePrimaryAccountHolder" 
                                    Visible='<%# !(bool)DataBinder.Eval(Container.DataItem, "IsPrimaryAccountHolder") %>'
                                    runat="server" CausesValidation="false"
                                    ToolTip="Makes this accountholder the primary accountholder" 
                                    CommandName="SetPrimary"
                                    Text="Set Primary" OnClientClick="return confirm('Set selected accountholder as Primary?');" />
                            </ItemTemplate>
                            <ControlStyle Width="40px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsAccountholders" runat="server" SelectMethod="GetAccountAccountHolders"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountId" SessionField="accountnrid" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvModelHistory" Caption="Model Portfolio History" SkinID="custom-width"
                    DataKeyNames="Key" Width="100%" AutoGenerateColumns="False" runat="server" DataSourceID="odsModelHistory"
                    OnRowCommand="gvModelHistory_RowCommand" 
                    OnRowDataBound="gvModelHistory_RowDataBound" 
                    onrowupdating="gvModelHistory_RowUpdating">
                    <Columns>
                        <asp:TemplateField HeaderText="Model" SortExpression="ModelPortfolio_ModelName">
                            <ItemTemplate>
                                <asp:Label ID="lblModel" runat="server" Text='<%# Bind("ModelPortfolio_ModelName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlModel" runat="server" DataSourceID="odsModels" DataTextField="ModelName" DataValueField="Key" />
                                <asp:ObjectDataSource ID="odsModels" runat="server" SelectMethod="GetModelPortfolios"
                                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter">
                                    <SelectParameters>
                                        <asp:FormParameter Name="assetManagerId" FormField="AssetManagerID" Type="Int32" DefaultValue="0" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ExecOnly" SortExpression="IsExecOnlyCustomer">
                            <ItemTemplate>
                                <asp:Label ID="lblExecOnly" runat="server" Text='<%# Bind("IsExecOnlyCustomer") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkExecOnly" runat="server" Checked='<%# Bind("IsExecOnlyCustomer") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="EmployerRelationship" SortExpression="EmployerRelationship">
                            <ItemTemplate>
                                <asp:Label ID="lblEmployerRelationship" runat="server" Text='<%# (AccountEmployerRelationship)DataBinder.Eval(Container.DataItem, "EmployerRelationship") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlEmployerRelationship" runat="server" SkinID="broad" DataValueField="Key" 
                                    DataTextField="Description" DataSourceID="odsEmployerRelationship" />
                                <asp:ObjectDataSource ID="odsEmployerRelationship" runat="server" SelectMethod="GetEmployerRelationshipList"
                                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Begin Date" SortExpression="ChangeDate">
                            <ItemTemplate>
                                <asp:Label ID="lblChangeDate" runat="server" Text='<%# Bind("ChangeDate", "{0:dd MMM yyyy}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:DatePicker ID="dtpChangeDate" runat="server" SelectedDate='<%# Bind("ChangeDate") %>'
                                    IsButtonDeleteVisible="false" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                            <ItemTemplate>
                                <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) ? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToString("dd MMM yyyy"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Employee" DataField="Employee_UserName" SortExpression="Employee_UserName"
                            ReadOnly="True" />
                        <asp:CommandField ShowEditButton="true" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnDelete" Visible="true" Enabled="false" runat="server" CausesValidation="false"
                                    ToolTip="Delete this item." CommandName="DeleteHistoryItem" Text="Delete" OnClientClick="if (this.disabled == false) return confirm('Delete Model History Item?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsModelHistory" runat="server" SelectMethod="GetModelHistory" 
                    UpdateMethod="UpdateModelHistoryItem" OldValuesParameterFormatString="original_{0}"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter" 
                    onupdated="odsData_Updated">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountId" SessionField="accountnrid" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>

        <tr>
            <td colspan="20">
                &nbsp;
            </td>
        </tr>

        <tr>
            <td colspan="20">
                <asp:GridView ID="gvCommissionRules" runat="server" Caption="Commission Rules ONLY for this Account"
                    AllowPaging="True" AllowSorting="True" DataSourceID="odsCommissionRules" AutoGenerateColumns="False"
                    PageSize="5" SkinID="custom-EmptyDataTemplate" Width="100%" DataKeyNames="Key"
                    OnRowCommand="gvCommissionRules_RowCommand" OnRowDataBound="gvCommissionRules_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="CommRuleName" HeaderText="Commission Rule" SortExpression="CommRuleName">
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CommCalculation_Name" HeaderText="Calculation" SortExpression="CommCalculation_Name">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                            <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                                <ItemTemplate>
                                    <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")) ?
                                        "" : ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToString("d MMMM yyyy"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                                <ItemTemplate>
                                    <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) ?
                                        "" : ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToString("d MMMM yyyy"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditCommissionRule" Text="Edit" CommandName="EditRule" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="lblEmptyDataTemplateCR" runat="server" Text="No Commission Rule" />
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsCommissionRules" runat="server" SelectMethod="GetAccountCommissionRules"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountID" SessionField="accountnrid" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr style="font-size: 3px; height: 3px;">
            <td colspan="20" style="height: 3px">
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height: 24px">
                <asp:Button ID="btnCreateCR" runat="server" Text="Create Commission Rule" OnClick="btnCreateCR_Click"
                    Width="185px" Enabled="False" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        
        <tr>
            <td colspan="20">
                <asp:GridView ID="gvFeeRules" runat="server" Caption="Fee Rules ONLY for this Account"
                    AllowPaging="True" DataSourceID="odsFeeRules" AutoGenerateColumns="False"
                    PageSize="5" SkinID="custom-EmptyDataTemplate" Width="100%" 
                    DataKeyNames="Key"
                    OnRowUpdating="gvFeeRules_RowUpdating" >
                    <Columns>
                        <asp:BoundField DataField="FeeCalculation_Name" HeaderText="Calculation" ReadOnly="true" >
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ExecutionOnly" HeaderText="Execution-Only" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SendByPost" HeaderText="Send By Post" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StartPeriod" HeaderText="Start Period" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="End Period" >
                            <ItemTemplate>
                                <%# (int)DataBinder.Eval(Container.DataItem, "EndPeriod") == 0 ?
                                    "" : DataBinder.Eval(Container.DataItem, "EndPeriod")%>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <b4f:YearMonthPicker ID="ppEndPeriod" runat="server" ListYearsBeforeCurrent="0" IsButtonDeleteVisible="true"
                                    SelectedPeriod='<%# (int)DataBinder.Eval(Container.DataItem, "EndPeriod")%>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" >
                            <ItemStyle Wrap="False" />
                        </asp:CommandField>
<%--                                <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditFeeRule" Text="Edit" CommandName="EditRule" 
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' 
                                    OnCommand="lbtnEditFeeRule_Command" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField> --%> 
                   </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="lblEmptyDataTemplateCR" runat="server" Text="No Fee Rules" />
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsFeeRules" runat="server" SelectMethod="GetAccountFeeRules"
                    UpdateMethod="UpdateAccountFeeRule" OldValuesParameterFormatString="original_{0}"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountId" SessionField="accountnrid" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="20">
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
                                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="accountId" SessionField="accountnrid" Type="Int32" />
                                    </SelectParameters>
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
                <asp:Button ID="btnCreateFeeRule" runat="server" Text="Create Fee Rule" OnClick="btnCreateFeeRule_Click"
                    CausesValidation="true" ValidationGroup="CreateFeeRule" />
                <asp:Button ID="btnCancelCreateFeeRule" runat="server" Text="Cancel" OnClick="btnCancelCreateFeeRule_Click"
                    CausesValidation="false" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvWithDrawals" Caption="Withdrawal Rule" SkinID="custom-EmptyDataTemplate"
                    DataKeyNames="Key" Width="100%" AutoGenerateColumns="False" runat="server" OnRowCommand="gvWithDrawals_RowCommand"
                    DataSourceID="odsWithdrawalRules" OnRowDataBound="gvWithDrawals_RowDataBound">
                    <Columns>
                        <asp:BoundField HeaderText="Amount" DataField="Amount_Quantity" SortExpression="Amount_Quantity" ReadOnly="true" />
                        <asp:BoundField HeaderText="Regularity" DataField="Regularity_Description" SortExpression="Regularity_Description" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Acc#" SortExpression="CounterAccount_DisplayNameShort">
                            <ItemTemplate>
                                <trunc:TruncLabel2 ID="lblCounterAccountNumber" runat="server" Width="75px" CssClass="padding"
                                    MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "CounterAccount_DisplayNameShort") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description" SortExpression="TransferDescription">
                            <ItemTemplate>
                                <trunc:TruncLabel2 ID="lblDescription" runat="server" Width="75px" CssClass="padding"
                                    MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "TransferDescription") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start" SortExpression="FirstDateWithdrawal">
                            <ItemTemplate>
                                <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "FirstDateWithdrawal")) ?
                                                                            "" : ((DateTime)DataBinder.Eval(Container.DataItem, "FirstDateWithdrawal")).ToString("dd-MM-yyyy"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End" SortExpression="EndDateWithdrawal">
                            <ItemTemplate>
                                <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDateWithdrawal")) ?
                                                                            "" : ((DateTime)DataBinder.Eval(Container.DataItem, "EndDateWithdrawal")).ToString("dd-MM-yyyy"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Created" SortExpression="CreationDate">
                            <ItemTemplate>
                                <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")) ?
                                                                            "" : ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("dd-MM-yyyy"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="CreatedBy" DataField="CreatedBy" SortExpression="CreatedBy" ReadOnly="true" />
                        <asp:TemplateField HeaderText="No Charges" SortExpression="DoNotChargeCommission">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkNoCharges" runat="server" Checked=<%# (bool)DataBinder.Eval(Container.DataItem, "DoNotChargeCommission") %> Enabled="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Active" SortExpression="IsActive">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkIsActive" runat="server" Checked=<%# (bool)DataBinder.Eval(Container.DataItem, "IsActive") %> Enabled="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEditWithdrawalRule" runat="server" Text="Edit" CommandName="EditWithdrawalRule" />
                                <asp:LinkButton ID="lbtnDeleteWithdrawalRule" runat="server" Text="Delete" CommandName="DeleteWithdrawalRule"
                                    OnClientClick="if (this.disabled == false) return confirm('Delete Withdrawal Rule?')" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="lblEmptyDataTemplateCR" runat="server" Text="No Withdrawal Rule" />
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsWithdrawalRules" runat="server" SelectMethod="GetWithdrawalRules"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountId" SessionField="accountnrid" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr style="font-size: 3px; height: 3px;">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnCreateWR" runat="server" CausesValidation="false" Text="Create Withdrawal Rule"
                    OnClick="btnCreateWR_Click" Width="185px" Enabled="false" />
            </td>
        </tr>
        <tr>
            <td colspan="20">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvManagementPeriods" Caption="Management Periods" SkinID="custom-width"
                    DataKeyNames="Key" Width="100%" AutoGenerateColumns="False" runat="server" DataSourceID="odsManagementPeriods"
                    OnRowCommand="gvManagementPeriods_RowCommand" 
                    OnRowDataBound="gvManagementPeriods_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Type" SortExpression="ManagementType">
                            <ItemTemplate>
                                <asp:Label ID="lblManagementType" runat="server" Text='<%# (ManagementTypes)DataBinder.Eval(Container.DataItem, "ManagementType") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate" >
                            <ItemTemplate>
                                <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("StartDate", "{0:dd MMM yyyy}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:DatePicker ID="dtpStartDate" runat="server" SelectedDate='<%# Bind("StartDate") %>'
                                    IsButtonDeleteVisible="false" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date" SortExpression="EndDateDisplayString">
                            <ItemTemplate>
                                <%# (string)DataBinder.Eval(Container.DataItem, "EndDateDisplayString")%>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:DatePicker ID="dtpEndDate" runat="server" SelectedDate='<%# Bind("EndDate") %>'
                                    IsButtonDeleteVisible="true" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMM yyyy}" HtmlEncode="False" ReadOnly="true" >
                            <ItemStyle wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Employee" DataField="Employee" SortExpression="Employee"
                            ReadOnly="True" />
                        <%--<asp:CommandField ShowEditButton="true" />--%>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEdit" Visible="true" Enabled="true" runat="server" CausesValidation="false"
                                    ToolTip="Edit this item" CommandName="EditManagementPeriod" Text="Edit" />
                                <asp:LinkButton ID="lbtnDelete" Visible="true" Enabled="false" runat="server" CausesValidation="false"
                                    ToolTip="Delete this item." CommandName="DeleteManagementPeriod" Text="Delete" OnClientClick="if (this.disabled == false) return confirm('Delete Management Period?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsManagementPeriods" runat="server" SelectMethod="GetManagementPeriods" 
                    UpdateMethod="UpdateManagementPeriod" OldValuesParameterFormatString="original_{0}"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter" 
                    onupdated="odsData_Updated">
                    <SelectParameters>
                        <asp:SessionParameter Name="accountId" SessionField="accountnrid" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>

                <asp:Panel ID="pnlCreateManagementPeriod" runat="server" Visible="False">
                    <br />
                    <table cellpadding="1" cellspacing="1" border="0" >
                      <tr>
                        <td style="width: 60px">
                            <asp:Label ID="lblManagementTypeLabel" runat="server">Type</asp:Label>
                        </td>
                        <td style="width: 150px">
                            <asp:DropDownList ID="ddlManagementTypes" runat="server" DataValueField="Key" 
                                DataTextField="Description" DataSourceID="odsManagementTypes" />
                            <asp:ObjectDataSource ID="odsManagementTypes" runat="server" SelectMethod="GetManagementTypes"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.AccountEditAdapter" />
                        </td>
                        <td style="width: 40px" align="left">&nbsp</td>
                      </tr>

                      <tr>
                        <td>
                            <asp:Label ID="lblMPStartDate" runat="server">Start</asp:Label>
                        </td>
                        <td>
                            <uc1:DatePicker ID="dtpMPStartDate" runat="server" IsButtonDeleteVisible="false" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvStartDate"
                                ControlToValidate="dtpMPStartDate:txtDate"
                                runat="server"
                                ValidationGroup="ManagementPeriod"
                                Text="*"
                                ErrorMessage="Start Date is mandatory" />
                        </td>
                      </tr>
                      
                      <asp:Panel ID="pnlMPEndDate" runat="server" Visible="false">
                          <tr>
                            <td>
                                <asp:Label ID="lblMPEndDate" runat="server">End</asp:Label>
                            </td>
                            <td colspan="2" >
                                <uc1:DatePicker ID="dtpMPEndDate" runat="server" IsButtonDeleteVisible="true" />
                                <asp:CompareValidator 
                                    ID="cvMPEndDate" 
                                    runat="server"
                                    ControlToValidate="dtpMPEndDate:txtDate"
                                    ControlToCompare="dtpMPStartDate:txtDate"
                                    Operator="GreaterThan"
                                    Type="Date"
                                    Text="*" 
                                    ErrorMessage="The end date can not be before the start date"/>
                            </td>
                          </tr>
                      </asp:Panel>

                    </table>
                    <br />
                </asp:Panel>
                <asp:Button ID="btnCreateMP" runat="server" CausesValidation="true" Text="Create Management Period" 
                    ValidationGroup="ManagementPeriod" OnClick="btnCreateMP_Click" />
                <asp:Button ID="btnEditMP" runat="server" CausesValidation="true" Text="Edit Management Period" Visible="false"
                    ValidationGroup="ManagementPeriod" OnClick="btnEditMP_Click" />
                <asp:Button ID="btnCancelMP" runat="server" CausesValidation="false" Text="Cancel" Visible="false" 
                    ValidationGroup="ManagementPeriod" OnClick="btnCancelMP_Click" />
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>
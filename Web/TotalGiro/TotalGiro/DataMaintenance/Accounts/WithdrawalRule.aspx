<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="WithdrawalRule.aspx.cs" Inherits="DataMaintenance_WithdrawalRule" Title="Withdrawal Rule" %>

<%@ Register Src="../../UC/FirstPreviousPageBackButton.ascx" TagName="FirstPreviousPageBackButton" TagPrefix="btn" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="dp" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Import Namespace="System" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <asp:ValidationSummary ID="valsum" runat="server" />
     <asp:Panel ID="pnlErr" Visible="false" runat="server" Width="125px">
        <table cellpadding="0" cellspacing="0" width="500">
           <tr>
                <td>
                    <asp:Label ID="lblErrorMess" runat="server" Font-Bold="true" ForeColor="red" />
                </td>
            </tr>
           <tr><td>&nbsp;</td></tr>
        </table>
    </asp:Panel>
    <table cellpadding="0" cellspacing="0" width="550" style="border: solid 1px black;">
        <asp:HiddenField ID="hfRuleID" runat="server" Value="" />
        <asp:HiddenField ID="hfRegularity" runat="server" Value="" />
        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
             <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                <asp:Label ID="lblAccountLabel" runat="server">Account</asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <asp:Label ID="lblAccount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td>
        </tr>
        <tr>
            <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                <asp:Label ID="lblRegularityLabel" runat="server">Regularity</asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <asp:MultiView ID="mvRegularity" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwRegularityReadOnly" runat="server">
                        <asp:Label ID="lblRegularity" runat="server"></asp:Label>
                    </asp:View>
                    <asp:View ID="vwRegularityInsert" runat="server">
                        <asp:DropDownList ID="ddlRegularity" 
                            runat="server" 
                            DataTextField="Description" DataValueField="Key" Width="300px"
                            DataSourceID="odsRegularities">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsRegularities" runat="server" SelectMethod="GetRegularities"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.WithdrawalRuleEditAdapter">
                        </asp:ObjectDataSource>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
            <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                <asp:Label ID="lblAmountLabel" runat="server">Amount</asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <asp:MultiView ID="mvAmount" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwAmountReadOnly" runat="server">
                        <asp:Label ID="lblAmount" runat="server"></asp:Label>
                    </asp:View>
                    <asp:View ID="vwAmountInsert" runat="server">
                        <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td style="width: 90%; vertical-align: bottom;">
                                        <db:DecimalBox ID="dbAmount" runat="server" DecimalPlaces="2" />
                                    </td>
                                    <td style="width: 10%; vertical-align: top;">
                                        <asp:RequiredFieldValidator ID="rfAmount"
                                            ControlToValidate="dbAmount:tbDecimal"
                                            runat="server"
                                            Text="*"
                                            ErrorMessage="Amount is mandatory" />
                                    </td>
                                </tr>
                            </table>
                        
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>

        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
            <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                <asp:Label ID="lblCounterAccountLabel" runat="server">Counter Account</asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <table cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td style="width: 90%; vertical-align: bottom;">
                                <asp:DropDownList 
                                    ID="ddlCounterAccount" DataSourceID="odsCounterAccount" DataTextField="DisplayName" DataValueField="Key" 
                                    SkinID="broad" runat="server" /> 
                                <asp:ObjectDataSource ID="odsCounterAccount" runat="server" 
                                    SelectMethod="GetCounterAccounts"
                                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.WithdrawalRuleEditAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="accountID" SessionField="accountnrid" Type="Int32" DefaultValue=0 />
                                        <asp:SessionParameter Name="contactID" SessionField="contactid" Type="Int32" DefaultValue=0 />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                    </table>
            </td>
        </tr>

        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <asp:Panel ID="pnlPandhouderPermission" Visible="false" runat="server" Width="125px">
            <tr>
                <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                    <asp:Label ID="lblPandhouderPermissionLabel" runat="server">Permission Pandhouder</asp:Label>
                </td>
                <td>
                    <asp:MultiView ID="mvPandhouderPermission" runat="server" ActiveViewIndex="0" EnableTheming="true">
                        <asp:View ID="vwPandhouderPermissionReadOnly" runat="server">
                            <asp:Label ID="lblPandhouderPermission" runat="server"></asp:Label>
                        </asp:View>
                        <asp:View ID="vwPandhouderPermissionInsert" runat="server">
                            <asp:CheckBox ID="cbPandhouderPermission" Checked="false"  runat="server" />
                            <asp:CustomValidator 
                                ID="cvPandhouderPermission" 
                                runat="server"
                                ClientValidationFunction="ClientValidate" 
                                OnServerValidate="cvPandhouderPermission_ServerValidate"
                                ErrorMessage="Pandhouder needs to give permission.">*</asp:CustomValidator>                        </asp:View>
                    </asp:MultiView>
                    
                </td>
            </tr>
            <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        </asp:Panel>

        <tr>
            <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                <asp:Label ID="lblTransferDescription" runat="server">Transfer Description</asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <table cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td style="width: 90%; vertical-align: bottom;">
                                <asp:TextBox ID="txtTransferDescription" SkinID="custom-width" Width="245px" runat="server" /> 
                            </td>
                        </tr>
                    </table>
            </td>
        </tr>
        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>

        <tr>
            <td style="width: 250px; text-align: right; padding-right: 5px; font-weight: bold; background-color: #CDD7DC;">
                <asp:Label ID="lblFirstDateLabel" runat="server">First Withdrawal Date</asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <asp:MultiView ID="mvFirstDate" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwFirstDateReadOnly" runat="server">
                        <asp:Label ID="lblFirstDate" runat="server"></asp:Label>
                    </asp:View>
                    <asp:View ID="vwFirstDateInsert" runat="server">
                         <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td style="width: 90%; vertical-align: bottom;">
                                        <dp:DatePicker ID="dpFirstDate" runat="server" />
                                    </td>
                                    <td style="width: 10%; vertical-align: top;">
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                                ControlToValidate="dpFirstDate:txtDate"
                                                runat="server"
                                                Text="*"
                                                ErrorMessage="Date is mandatory" />
                                    </td>
                                </tr>
                            </table>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
            <td style="text-align: right; padding-right: 5px; padding-top: 2px; font-weight: bold; background-color: #CDD7DC; vertical-align: top;">
                <asp:Label ID="lblEndDateWithdrawal" runat="server">End Withdrawal Date</asp:Label>
            </td>
            <td style="padding-left: 5px; vertical-align: bottom;">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="width: 90%; vertical-align: bottom;">
                            <dp:DatePicker ID="dpEndDate" runat="server" />
                        </td>
                        <td style="width: 10%; vertical-align: top;">
                            <asp:CustomValidator 
                                ID="cvEndDate" 
                                runat="server"
                                Enabled="false"
                                ErrorMessage="End Withdrawal Date can't be within the regular timespan (regularity)." 
                                OnServerValidate="cvEndDate_ServerValidate">*</asp:CustomValidator>
                       </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
            <td style="text-align: right; padding-right: 5px; padding-top: 2px; font-weight: bold; background-color: #CDD7DC; vertical-align: top;">
                &nbsp;
            </td>
            <td style="padding-left: 5px; vertical-align: bottom;">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="vertical-align: bottom;">
                            <asp:CheckBox ID="chkNoCharges" runat="server" Text="No Commission" Checked="true" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <asp:Panel ID="pnlIsActive" Visible="false" runat="server" Width="125px">
        <tr><td style="background-color: #CDD7DC;">&nbsp;</td><td>&nbsp;</td></tr>
        <tr>
            <td style="text-align: right; padding-right: 5px; padding-top: 2px; font-weight: bold; background-color: #CDD7DC; vertical-align: top;">
                &nbsp;
            </td>
            <td style="padding-left: 5px; vertical-align: bottom;">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="vertical-align: bottom;">
                            <asp:CheckBox ID="chkIsActive" runat="server" Text="Is Active?" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        </asp:Panel>
        <tr>
            <td style="background-color: #CDD7DC;">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="background-color: #CDD7DC;">&nbsp;</td>
            <td style="padding-left: 5px;">
                <asp:MultiView ID="mvSave" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwUpdate" runat="server">
                        <asp:LinkButton ID="lbtnUpdate" runat="server" OnClick="lbtnUpdate_Click">Update</asp:LinkButton>
                    </asp:View>
                    <asp:View ID="vwInsert" runat="server">
                        <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="true" OnClick="lbtnInsert_Click">Insert</asp:LinkButton>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr><tr>
            <td style="background-color: #CDD7DC;">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr style="font-size: 3px; height: 3px;"><td>&nbsp;</td></tr>
        <tr>
            <td>
                <btn:FirstPreviousPageBackButton ID="FirstPreviousPageBackButton1" runat="server" />
            </td>
        </tr>
    </table>
    <script type="text/javascript">
       function ClientValidate(source, arguments)
       {
            if (document.getElementById('ctl00_bodyContentPlaceHolder_cbPandhouderPermission').checked)
                arguments.IsValid=true;            
            else
                arguments.IsValid=false;
       }
    </script>
</asp:Content>


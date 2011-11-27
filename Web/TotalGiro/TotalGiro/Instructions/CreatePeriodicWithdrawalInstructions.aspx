<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="CreatePeriodicWithdrawalInstructions.aspx.cs" Inherits="Instructions_CreatePeriodicWithdrawalInstructions" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc2" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc3" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:RadioButtonList ID="rblCreatePeriodicWithdrawalsChoice" runat="server" OnSelectedIndexChanged="rblCreatePeriodicWithdrawalsChoice_SelectedIndexChanged" AutoPostBack="true">
        <asp:ListItem Text="Normal" Selected="True" />
        <asp:ListItem Text="Advanced"  />
    </asp:RadioButtonList>
    <br />
    <asp:MultiView ID="mlvCreatePeriodicWithdrawals" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweNormal" runat="server">
            <asp:Button ID="btnCreatePeriodicWithdrawals" runat="server" OnClick="btnCreatePeriodicWithdrawals_Click" Text="Create Periodic Withdrawals" />
        </asp:View>
        <asp:View ID="vweAdvanced" runat="server">
            <uc2:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true"/>
            <asp:GridView
                ID="gvWithDrawals"
                Caption="Withdrawal Rules"
                SkinID="custom-EmptyDataTemplate"
                DataKeyNames="Key"
                AllowPaging="True"
                PageSize="20" 
                AutoGenerateColumns="False"
                AllowSorting="True"
                runat="server" 
                DataSourceID="odsWithdrawalRules"
                OnRowCommand="gvWithDrawals_RowCommand"
                OnRowDataBound="gvWithDrawals_RowDataBound"
                Visible="false">
                 <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:Image ID="imgWarning" runat="server" 
                            ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                            Visible='<%# DataBinder.Eval(Container.DataItem, "IsInValid") %>'
                            ToolTip="This withdrawal rule does not contain all the necessary data"
                            />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Account#" SortExpression="Account_Number" >
                        <ItemTemplate>
                            <uc3:AccountLabel ID="ctlAccountLabel" 
                                runat="server" 
                                RetrieveData="false" 
                                Width="120px" 
                                NavigationOption="PortfolioView"
                                />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="Account_ShortName">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                                MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Model" SortExpression="Account_ModelPortfolioName">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblModel" runat="server" Width="100px" CssClass="padding"
                                MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "Account_ModelPortfolioName") %>' />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Amount" DataField="Amount_Quantity" SortExpression="Amount_Quantity">
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Regularity" DataField="Regularity_Description" SortExpression="Regularity_Description" >
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="No Charges" SortExpression="DoNotChargeCommission">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkNoCharges" runat="server" Checked=<%# (bool)DataBinder.Eval(Container.DataItem, "DoNotChargeCommission") %> Enabled="false" />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="First Date" DataField="FirstDateWithdrawal" SortExpression="FirstDateWithdrawal" DataFormatString="{0:dd MMM yyyy}" HtmlEncode="False" >
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Next Date" SortExpression="NextWithdrawalDate1">
                        <ItemTemplate>
                            <asp:Label ID="lblNextDate" runat="server">
                                <%#((DateTime)DataBinder.Eval(Container.DataItem, "NextWithdrawalDate1") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "NextWithdrawalDate1", "{0:dd MMM yyyy}") : "")%>
                            </asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False"  />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Max Date" SortExpression="MaxWithdrawalDate">
                        <ItemTemplate>
                            <asp:Label ID="lblMaxDate" runat="server" >
                                <%#((DateTime)DataBinder.Eval(Container.DataItem, "MaxWithdrawalDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "MaxWithdrawalDate", "{0:dd MMM yyyy}") : "")%>
                            </asp:Label>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton
                                 ID="lbtnViewInstructions"
                                 runat="server"
                                 Text="View"
                                 CommandName="ViewInstructions" />
                            <asp:LinkButton
                                 ID="lbtnCreateWithdrawal"
                                 runat="server"
                                 Text="Create Withdrawals"
                                 Visible='<%# !(bool)DataBinder.Eval(Container.DataItem, "IsInValid") %>'
                                 CommandName="CreateWithdrawals" />
                         </ItemTemplate>
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                    </asp:TemplateField>
                 </Columns>
                 <EmptyDataTemplate>
                    <asp:Label ID="lblEmptyDataTemplateWR" runat="server" Text="No Withdrawal Rules" />
                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="Gainsboro" />
            </asp:GridView>
            <asp:ObjectDataSource ID="odsWithdrawalRules" runat="server" SelectMethod="GetWithdrawalRules"
                TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.CreatePeriodicWithdrawalInstructionsAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                        Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>

            <asp:Panel ID="pnlCreateWithdrawals" runat="server" Visible="false">
                <br />
                <asp:Label ID="lblEndDate" runat="server" Text="Select Max Date up to which periodic withdrawals will be created." />
                <br />
               <table cellpadding="1" cellspacing="1" border="0" >
                  <tr>
                   <td width="175">
                        <uc1:DatePicker ID="dtpEndDate" runat="server" ListYearsAfterCurrent="2" ListYearsBeforeCurrent="0" IsButtonDeleteVisible="false" />&nbsp
                    </td>            
                    <td width="30px">
                        <asp:RangeValidator
                            runat="server"
                            id="rvEndDate"
                            controlToValidate="dtpEndDate:txtDate"
                            Text="*"
                            errorMessage="Date should be later or equal as today" 
                            Type="Date"
                            MinimumValue="3000-01-01"
                            MaximumValue="3000-01-01"/>&nbsp
                        <asp:RequiredFieldValidator ID="rfvEndDate"
                            ControlToValidate="dtpEndDate:txtDate"
                            runat="server"
                            Text="*"
                            ErrorMessage="Date is mandatory" />
                    </td>
                  </tr>
                </table>
                <br />
                <asp:Button ID="btnCreatePeriodicWithdrawals2" runat="server" OnClick="btnCreatePeriodicWithdrawals2_Click" Text="Create Periodic Withdrawals" />&nbsp
                <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="false" />
            </asp:Panel>

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
                <EmptyDataTemplate>
                    <asp:Label ID="lblEmptyDataTemplateW" runat="server" Text="No Withdrawals" />
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:Button ID="btnHideInstructions" runat="server" Text="Hide" Visible="false" OnClick="btnHideInstructions_Click" />&nbsp
            <asp:ObjectDataSource ID="odsInstructions" runat="server" SelectMethod="GetWithdrawalInstructions"
                TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.CreatePeriodicWithdrawalInstructionsAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvWithDrawals" Name="withdrawalRuleID" PropertyName="SelectedValue" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
    </asp:MultiView>
    <br />
    <asp:Label ID="lblMessage2" runat="server" Text="" /> 
    <br />
    <span class="padding" style="display:block">
        <asp:ValidationSummary ID="vsFuckUps" runat="server" ForeColor="Red" Height="0px" Width="450px"/>
    </span>
    
</asp:Content>


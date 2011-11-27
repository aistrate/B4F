<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="NewPortfolios.aspx.cs" Inherits="Orders_AssetManager_NewPortfolios" 
    Title="Manage Cash Deposits" Theme="Neutral" %>
<%@ Register Src="../../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <script type="text/javascript">
        function ConfirmBuyModelInstruction(showBuyModelDiffWarning, depositCashPositionDiff)
        {
            if (showBuyModelDiffWarning && depositCashPositionDiff != 0)
                return alert('There is a difference of ' + depositCashPositionDiff + ' between the cash position and the deposit(s). If you do not want to include this amount in this instruction uncheck the checkbox.');
            else
                return true;
        }
    </script>
    
    <asp:RadioButtonList ID="rblNewCashChoice" runat="server" 
        OnSelectedIndexChanged="rblNewCashChoice_SelectedIndexChanged" AutoPostBack="true">
        <asp:ListItem Text="First Deposit" Value="0" Selected="True" />
        <asp:ListItem Text="New Cash Deposit" Value="1" />
    </asp:RadioButtonList>
    <br />
    <asp:MultiView ID="mlvNewCashView" runat="server" ActiveViewIndex="0" 
        onactiveviewchanged="mlvNewCashView_ActiveViewChanged" >
        <asp:View ID="vweFirstDeposit" runat="server">
            <asp:GridView 
                ID="gvNewCustomers" 
                runat="server" 
                AllowPaging="True"
                PageSize="20" 
                AllowSorting="True" 
                DataKeyNames="Key"
                DataSourceID="odsNewAccounts"
                AutoGenerateColumns="False" 
                OnRowCommand="gvNewCustomers_RowCommand"
                OnRowDataBound="gvNewCustomers_RowDataBound"
                SkinID="custom-EmptyDataTemplate"
                Width="750px"
                Caption="First Deposits" 
                CaptionAlign="Left">
                <Columns>
                    <asp:ButtonField Visible="False" CommandName="Select" />
                    <asp:TemplateField HeaderText="Account#" SortExpression="Number">
                        <HeaderStyle Wrap="False" />
                        <ItemStyle HorizontalAlign="Left" Wrap="False" />
                        <ItemTemplate>
                            <uc1:AccountLabel ID="ctlAccountLabel" 
                                runat="server" 
                                RetrieveData="false" 
                                Width="120px" 
                                NavigationOption="PortfolioView"
                                AccountDisplayOption="DisplayNumber"
                                />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Account Name" SortExpression="ShortName">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="50px" CssClass="padding"
                                MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Model" SortExpression="ModelPortfolioName">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblModel" runat="server" Width="50px" CssClass="padding"
                                MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ModelPortfolioName") %>' />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="TotalCashAmount" HeaderText="Total Cash Amount" SortExpression="TotalCashAmount" >
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemStyle wrap="False" />
                        <ItemTemplate>
                            <asp:LinkButton 
                                runat="server" 
                                ID="lbtCreatePortfolio" 
                                Text="Create Portfolio" 
                                CommandName="CreatePortfolio"/>
<%--                            <asp:LinkButton 
                                runat="server" 
                                ID="lbtBuyFund" 
                                Text="BuyFund"
                                CommandName="BuyFund"/>
--%>                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <SelectedRowStyle BackColor="Gainsboro" />
                <EmptyDataTemplate>
                    <asp:Label ID="lblEmptyccountsNewCash" runat="server" Text="No first cash deposits found" Width="1000px" />
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsNewAccounts" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetNewAccounts" TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.NewPortfoliosAdapter">
            </asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="vweAccountsNewCash" runat="server">
            <asp:GridView 
                ID="gvAccountsNewCash" 
                runat="server" 
                AllowPaging="True"
                PageSize="15" 
                cssclass="padding"
                AllowSorting="True" 
                DataKeyNames="Key"
                DataSourceID="odsAccountsNewCash"
                AutoGenerateColumns="False" 
                Caption="New Cash Deposit" 
                CaptionAlign="Left"
                SkinID="custom-EmptyDataTemplate"
                Width="750px"
                OnRowCommand="gvAccountsNewCash_RowCommand" 
                OnRowDataBound="gvAccountsNewCash_RowDataBound">
                <Columns>
                    <asp:ButtonField Visible="False" CommandName="Select" />
                    <asp:TemplateField HeaderText="Account#" SortExpression="Number">
                        <HeaderStyle Wrap="False" />
                        <ItemStyle HorizontalAlign="Left" Wrap="False" />
                        <ItemTemplate>
                            <uc1:AccountLabel ID="ctlAccountLabel" 
                                runat="server" 
                                RetrieveData="false" 
                                Width="120px" 
                                NavigationOption="PortfolioView"
                                AccountDisplayOption="DisplayNumber"
                                />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Account Name" SortExpression="ShortName">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="50px" CssClass="padding"
                                MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Model" SortExpression="ModelPortfolioName">
                        <ItemTemplate>
                            <trunc:TruncLabel2 ID="lblModel" runat="server" Width="50px" CssClass="padding"
                                MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ModelPortfolioName") %>' />
                        </ItemTemplate>
                        <HeaderStyle wrap="False" />
                        <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CashPosition" HeaderText="Cash Position" SortExpression="CashPosition" DataFormatString="{0:###,##0.00}" HtmlEncode="false" >
                        <ItemStyle horizontalalign="Right" wrap="False" />
                        <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="TotalCashTransfers" HeaderText="Total Transfers" SortExpression="TotalCashTransfers" DataFormatString="{0:###,##0.00}" HtmlEncode="false" >
                        <ItemStyle horizontalalign="Right" wrap="False" />
                        <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="Transactions" HeaderText="#" SortExpression="Transactions" >
                        <ItemStyle horizontalalign="Right" wrap="False" />
                        <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False"/>
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemStyle wrap="False" />
                        <ItemTemplate>
                            <b4f:TooltipImage ID="ttiBuyModelNotification" runat="server"  TooltipShadowWidth="5" TooltipWidth="200"
                                TooltipClickClose="true" TooltipDefaultImage="Balloon_Small" TooltipPadding="8"
                                IsTooltipAbove="true" OffSetX="-17"
                                TooltipContent='<%# DataBinder.Eval(Container.DataItem, "BuyModelMessage") %>'
                                Visible='<%# !(bool)DataBinder.Eval(Container.DataItem, "IsBuyModelAllowed") %>' >
                            </b4f:TooltipImage>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle wrap="False" />
                        <ItemTemplate>
                            <asp:LinkButton 
                                runat="server" 
                                ID="lbtViewTransactions" 
                                Text="View"
                                CommandName="ViewTransactions"/>
                            <asp:LinkButton 
                                runat="server" 
                                ID="lbtRebalance" 
                                Text="Rebalance" 
                                CommandName="Rebalance"/>
                            <asp:LinkButton 
                                runat="server" 
                                ID="lbtBuyModel" 
                                Text="Buy Model" 
                                CommandName="BuyModel"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "DepositCashPositionDiff") %>'
                                Visible='<%# DataBinder.Eval(Container.DataItem, "IsBuyModelAllowed") %>'
                                OnClientClick='<%# string.Format("return ConfirmBuyModelInstruction({0}, {1});", ((bool)DataBinder.Eval(Container.DataItem, "ShowBuyModelDiffWarning")).ToString().ToLower(), ((decimal)DataBinder.Eval(Container.DataItem, "DepositCashPositionDiff")).ToString(System.Globalization.CultureInfo.InvariantCulture)) %>'
                                />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <SelectedRowStyle BackColor="Gainsboro" />
                <EmptyDataTemplate>
                    <asp:Label ID="lblEmptyccountsNewCash" runat="server" Text="No new cash deposits found" Width="1000px" />
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsAccountsNewCash" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetAccountsNewCashTransfers" TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.NewPortfoliosAdapter">
            </asp:ObjectDataSource>
            <br />
            <asp:GridView 
                ID="gvCashTransfers" 
                runat="server" 
                AutoGenerateColumns="False" 
                Caption="Cash Transfers"
                CaptionAlign="Left" 
                DataSourceID="odsCashTransfers" 
                DataKeyNames="Key"
                Visible="False"
                OnSelectedIndexChanging="gvCashTransfers_SelectedIndexChanging">
                <Columns>
                    <asp:BoundField DataField="Key" HeaderText="EntryID" >
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "CashTransferType")%>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreditDisplayString" HeaderText="Value" >
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Transaction Date" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" >
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" >
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:d MMMM yyyy hh:mm}" HtmlEncode="False">
                        <HeaderStyle wrap="False" />
                        <ItemStyle wrap="False" />
                    </asp:BoundField>
                    <asp:CommandField SelectText="Skip" ShowSelectButton="True">
                        <ItemStyle Wrap="False" />
                    </asp:CommandField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsCashTransfers" runat="server" 
                SelectMethod="GetNewCashTransfers" TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.NewPortfoliosAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvAccountsNewCash" Name="accountID" PropertyName="SelectedValue" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:Button ID="btnHideCashTransfers" runat="server" Text="Hide" OnClick="btnHideCashTransfers_Click" Visible="false" />&nbsp
        </asp:View>
    </asp:MultiView>
    <br />
    <asp:HiddenField ID="hdnAccountID" runat="server" />
    <asp:HiddenField ID="hdnAction" runat="server" />
    <asp:HiddenField ID="hdnDepositCashPositionDiff" runat="server" />
    <asp:Panel ID="pnlDoAction" runat="server" Visible="False">
        <table cellpadding="1" cellspacing="1" border="0" >
          <tr>
            <td style="width: 120px"><asp:Label ID="lblOrderActionTypes" runat="server" Text="Action Type"></asp:Label></td>
            <td style="width: 200px">
                <asp:DropDownList ID="ddlOrderActionTypes" runat="server" DataSourceID="odsOrderActionTypes" 
                    DataTextField="Description" Enabled="false" DataValueField="Key" SkinID="custom-width" Width="85px">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsOrderActionTypes" runat="server" SelectMethod="GetOrderActionTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter">
                </asp:ObjectDataSource>            
            </td>
            <td style="width: 40px" align="left">&nbsp</td>
          </tr>
          <tr>
            <td>&nbsp</td>
            <td>
                <asp:CheckBox ID="chkNoCommissionCharged" runat="server" Text="No commission is charged" Checked="False" />
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>

          <asp:Panel ID="pnlIncludeAllCash" runat="server" Visible="false" >
          <tr>
            <td>&nbsp</td>
            <td>
                <asp:CheckBox ID="chkIncludeAllCash" runat="server" Text="Include previous cash?" Checked="True" />
            </td>
            <td style="width: 50px">&nbsp</td>
          </tr>
          </asp:Panel>        
        </table>
        <br/>
        <asp:Button ID="btnOK" runat="server" OnClick="btnOK_Click" Text="OK"  />&nbsp;
        <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="False" />
    </asp:Panel>
    <br />
    <asp:Label ID="lblError" Font-Bold="true" Font-Size="Medium" runat="server" ForeColor="Red" Width="700px" />
    <br />
    <asp:Label ID="lblResult" Font-Bold="True" Font-Size="Medium" runat="server" ForeColor="Black" Width="700px" />
    <br />
</asp:Content>


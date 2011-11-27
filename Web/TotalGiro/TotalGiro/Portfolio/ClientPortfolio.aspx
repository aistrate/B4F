<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="ClientPortfolio.aspx.cs"
    Inherits="ClientPortfolio" Title="Client Portfolio" Theme="Neutral" %>

<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="smClientPortfolio" runat="server" />
    <div style="position: relative; top: -5px; left: 15px;">
        <div style="position: relative; left: -3px">
            <asp:Panel ID="pnlAccountFinder" runat="server" Width="870px">
                <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true"
                    ShowContactActiveCbl="true" />
            </asp:Panel>
        </div>
    </div>
    <asp:Panel ID="pnlSelectedAccount" runat="server" Visible="False" Width="870px">
        <hr style="width: 870px" />
        <table style="width: 366px">
            <tr style="height: 2px">
                <td style="width: 116px">
                </td>
                <td style="width: 250px">
                </td>
            </tr>
            <tr>
                <td style="height: 24px; white-space: nowrap">
                    <asp:Label ID="Label1" runat="server" Text="Selected Account:" Width="105px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:DropDownList ID="ddlSelectedAccount" SkinID="custom-width" runat="server" Width="200px"
                        DataSourceID="odsSelectedAccount" DataTextField="DisplayNumberWithName" DataValueField="Key"
                        OnSelectedIndexChanged="ddlSelectedAccount_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="odsSelectedAccount" runat="server" SelectMethod="GetCustomerAccounts"
            TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClientPortfolioAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                    Type="Boolean" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <asp:Panel ID="pnlClientPortfolio" runat="server" Visible="False">
        <table style="width: 793px">
            <tr style="height: 1px">
                <td style="width: 123px">
                </td>
                <td style="width: 210px">
                </td>
                <td style="width: 60px">
                </td>
                <td style="width: 140px">
                </td>
                <td style="width: 260px">
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblAccountNameLabel" runat="server" Text="Account Name:"></asp:Label>
                </td>
                <td style="white-space: nowrap;" colspan="2">
                    <asp:Label ID="lblAccountName" runat="server" Font-Bold="True" Width="250px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblModelNameLabel" runat="server" Text="Model:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblModelName" runat="server" Font-Bold="True" Width="250px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblContactStreetLabel" runat="server" Text="Address:"></asp:Label>
                </td>
                <td style="white-space: nowrap;" colspan="2">
                    <asp:Label ID="lblContactStreet" runat="server" Font-Bold="True" Width="250px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblAccountNumberLabel" runat="server" Text="Account Number:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <uc2:AccountLabel ID="ctlAccountLabel" RetrieveData="false" runat="server" Font-Bold="True" Width="120px" />
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblContactCityLabel" runat="server" Text="City:"></asp:Label>
                </td>
                <td style="white-space: nowrap" colspan="2">
                    <asp:Label ID="lblContactCity" runat="server" Font-Bold="True" Width="250px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblStatusLabel" runat="server" Text="Status:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblStatus" runat="server" Font-Bold="True" Width="150px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblRemisierLabel" runat="server" Text="Remisier:"></asp:Label>
                </td>
                <td style="white-space: nowrap" colspan="2">
                    <asp:Label ID="lblRemisier" runat="server" Font-Bold="True" Width="250px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblRemisierEmployeeLabel" runat="server" Text="Contact Person:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblRemisierEmployee" runat="server" Font-Bold="True" Width="150px"></asp:Label>
                </td>
            </tr>
            <tr style="height: 7px">
                <td colspan="5">
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblTotalCashLabel" runat="server" Text="Total Cash:"></asp:Label>
                </td>
                <td style="white-space: nowrap" align="right">
                    <asp:Label ID="lblTotalCash" runat="server" Font-Bold="True" Width="180px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblLastRebalanceLabel" runat="server" Text="Last Rebalance:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblLastRebalance" runat="server" Font-Bold="True" Width="150px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblTotalPositionsLabel" runat="server" Text="Total Positions:"></asp:Label>
                </td>
                <td style="white-space: nowrap" align="right">
                    <asp:Label ID="lblTotalPositions" runat="server" Font-Bold="True" Width="180px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblCurrentRebalanceLabel" runat="server" Text="Current Rebalance:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblCurrentRebalance" runat="server" Font-Bold="True" Width="150px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 21px; white-space: nowrap;">
                    <asp:Label ID="lblTotalLabel" runat="server" Text="Total:"></asp:Label>
                </td>
                <td style="white-space: nowrap;" align="right">
                    <asp:Label ID="lblTotal" runat="server" Font-Bold="True" Width="180px"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblFutureWithdrawalsLabel" runat="server" Text="Future Withdrawals:"></asp:Label>
                </td>
                <td style="white-space: nowrap;">
                    <asp:Label ID="lblFutureWithdrawals" runat="server" Font-Bold="True" Width="150px"></asp:Label>
                </td>
            </tr>
            <tr style="height: 6px">
                <td colspan="5">
                </td>
            </tr>
        </table>
        
        <cc1:MultipleSelectionGridView ID="gvPositions" runat="server" AllowSorting="True"
            AutoGenerateColumns="False" Caption="Positions" GridLines="None" CaptionAlign="Top"
            DataSourceID="odsPositions" AllowPaging="True" DataKeyNames="Key" 
            OnDataBinding="gvPositions_DataBinding"
            OnRowDataBound="gvPositions_RowDataBound" 
            OnDataBound="gvPositions_DataBound"
            OnRowCommand="gridView_RowCommand"
            SelectionBoxEnabledBy="IsCloseable"
            Width="870px" SkinID="custom-width" >
            <Columns>
                <asp:BoundField DataField="Isin" HeaderText="ISIN" SortExpression="Isin">
                    <ItemStyle Wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="InstrumentName" HeaderText="Instrument" SortExpression="InstrumentName">
                    <ItemStyle Wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Size" HeaderText="Size" SortExpression="Size">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="Percentage" HeaderText="Actual" SortExpression="Percentage"
                    DataFormatString="{0:##0.0000}%">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Model" SortExpression="ModelAllocation">
                    <ItemStyle HorizontalAlign="Right" Wrap="False" />
                    <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                    <ItemTemplate>
                        <%# ((decimal)DataBinder.Eval(Container.DataItem, "ModelAllocation") != 0m ? Eval("ModelAllocation", "{0:##0.0000}%"): "") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Price" SortExpression="Price">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Ex Rate" SortExpression="ExchangeRate">
                    <ItemStyle HorizontalAlign="Right" Wrap="False" />
                    <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                    <ItemTemplate>
                        <%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}"): "") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="AccruedInterest" HeaderText="Accr.Int." SortExpression="AccruedInterest" Visible="true" >
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                </asp:BoundField>
                <asp:CommandField SelectText="View" ShowSelectButton="True">
                    <ItemStyle Wrap="False" Width="29px" />
                </asp:CommandField>
            </Columns>
        </cc1:MultipleSelectionGridView>
        <asp:ObjectDataSource ID="odsPositions" runat="server" SelectMethod="GetPositions"
            TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClientPortfolioAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlSelectedAccount" DefaultValue="" Name="accountId"
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:Panel ID="pnlCash" runat="server">
            <br />
            <table width="870px">
                <tr>
                   <td align="right">
                        <asp:CheckBox ID="chkShowForeignCurrency" runat="server" Text="Show Foreign Currency"
                            OnCheckedChanged="chkBuyingPowerFilter_CheckedChanged" AutoPostBack="true" />&nbsp;
                        <asp:CheckBox ID="chkShowUnSettled" runat="server" Text="Show UnSettled Positions"
                            OnCheckedChanged="chkBuyingPowerFilter_CheckedChanged" AutoPostBack="true"  />
                   </td>
                </tr>
            </table>
            <asp:GridView ID="gvBuyingPower" runat="server" AllowPaging="True" AllowSorting="false"
                DataSourceID="odsBuyingPower" AutoGenerateColumns="False" Caption="Cash Overview"
                CaptionAlign="Left" DataKeyNames="Key" PageSize="20" 
                OnRowDataBound="gvBuyingPower_RowDataBound"
                OnRowCommand="gvBuyingPower_RowCommand"
                Width="870px" SkinID="custom-width">
                <Columns>
                    <asp:BoundField DataField="LineDescription" HeaderText="LineDescription" SortExpression="LineDescription">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status">
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Value">
                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                        <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                        <ItemTemplate>
                            <%# ((bool)DataBinder.Eval(Container.DataItem, "IsCashLine") || (bool)DataBinder.Eval(Container.DataItem, "IsCashFundLine") ? DataBinder.Eval(Container.DataItem, "ValueDisplay") : "") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ExRate">
                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                        <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                        <ItemTemplate>
                            <%# ((decimal)DataBinder.Eval(Container.DataItem, "ExRate") != 1m ? Eval("ExRate", "{0:##0.0000}") : "")%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="BaseValue">
                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                        <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                        <ItemTemplate>
                            <%#  DataBinder.Eval(Container.DataItem, "BaseValueDisplay")%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Right" wrap="False" />
                        <ItemTemplate>
                            <asp:LinkButton 
                                runat="server" 
                                ID="lbtViewCashDetails" 
                                Text="View" 
                                CommandName="ViewCashDetails"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SubPositionID") %>'
                                Visible='<%# DataBinder.Eval(Container.DataItem, "IsCashLine") %>'
                                />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsBuyingPower" runat="server" SelectMethod="GetBuyingPowerDisplayForClient"
                TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClientPortfolioAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlSelectedAccount" DefaultValue="" Name="accountId"
                        PropertyName="SelectedValue" Type="Int32" />
                    <asp:ControlParameter ControlID="chkShowForeignCurrency" Name="showForeignCurrency"
                        PropertyName="Checked" Type="Boolean" />
                    <asp:ControlParameter ControlID="chkShowUnSettled" Name="showUnSettledCash"
                        PropertyName="Checked" Type="Boolean" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
        </asp:Panel>
        <asp:MultiView ID="mvwButtons" runat="server" ActiveViewIndex="0" EnableTheming="False">
            <asp:View ID="vwMain" runat="server">
                <asp:Panel ID="pnlErrorMessage" runat="server" Width="860px" Visible="false">
                    <br />
                    <span class="padding" style="display: block">
                        <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Bold="True" Width="850px"></asp:Label>
                    </span>
                </asp:Panel>
                <table style="width: 870px">
                    <tr>
                        <td style="width: 450">
                            <asp:Button ID="btnClosePositions" runat="server" OnClick="btnClosePositions_Click"
                                Text="Close Positions" 
                                OnClientClick="return confirm('Are you sure you want to close positions?')"/>&nbsp;
                            <asp:Button ID="btnRebalance" runat="server" OnClick="btnRebalance_Click" Text="Rebalance" 
                                OnClientClick="return confirm('Are you sure you want to create a rebalance instruction?')"/>
                            <asp:CheckBox ID="chkNoCharges" runat="server" Text="No Commission" />
                            <asp:Button ID="btnLiquidate" runat="server" OnClick="btnLiquidate_Click" Text="Liquidate" 
                                OnClientClick="return confirm('Are you sure you want to liquidate this account?')"/>
                        </td>
                        <td style="width: 420px" align="right">
                            <asp:Button ID="btnAccountDetails" runat="server" OnClick="btnAccountDetails_Click"
                                Text="Account Details" Width="130px" />&nbsp;
                            <asp:Button ID="btnShowClosedPositions" runat="server" OnClick="btnShowClosedPositions_Click"
                                Text="Show Closed Positions" Width="170px" />
                        </td>
                    </tr>
                </table>
            </asp:View>
            <asp:View ID="vwQuestion" runat="server">
                <asp:Label ID="lblWarningMessages" runat="server" Width="800px"></asp:Label><br />
                <asp:Button ID="btnYes" runat="server" OnClick="btnYes_Click" Text="Yes" />&nbsp;
                <asp:Button ID="btnNo" runat="server" Text="No" /></asp:View>
        </asp:MultiView>
    </asp:Panel>
</asp:Content>

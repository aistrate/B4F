<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="PositionTransactions.aspx.cs" Inherits="PositionTransactions" 
         Title="Position Transactions" %>

<%@ Import Namespace="B4F.TotalGiro.Accounts.Portfolios.FundPositions" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Import Namespace="B4F.TotalGiro.Utils" %>
<%@ Register Src="../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <asp:Table ID="Table1" runat="server" Width="760px">
        <asp:TableRow ID="TableRow1" runat="server" Height="24px">
            <asp:TableCell ID="TableCell1" runat="server" Width="110px">Account:</asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" Width="650px" Font-Bold="True"><asp:Label ID="lblAccount" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server" Height="24px">
            <asp:TableCell ID="TableCell3" runat="server">Instrument:</asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" Font-Bold="True"><asp:Label ID="lblInstrument" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" Height="24px">
            <asp:TableCell ID="TableCell5" runat="server">Position Value:</asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server" Font-Bold="True"><asp:Label ID="lblValue" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:MultiView ID="mvwGridViews" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwPositionTxsSecurity" runat="server">
            <b4f:MultipleSelectionGridView ID="gvPositionTxsSecurity" runat="server" SkinID="custom-width" Width="1075px"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsPositionTxsSecurity" PageSize="20" 
                    Caption="Position Transactions" CaptionAlign="Left" DataKeyNames="Key" SelectionBoxVisibleBy="IsStornoable"
                    OnRowCommand="gvPositionTx_RowCommand" >
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton 
                                ID="imbDetails"
                                CommandName="Details"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TransactionId") %>'
                                runat="server" 
                                ImageUrl="~/layout/images/audit.gif"
                                ToolTip="View Audit Trail" 
                                Height="16px" 
                                Width="16px" />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="TransactionId" HeaderText="TradeID" SortExpression="TransactionId">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Size" HeaderText="Size" SortExpression="Size">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Price" SortExpression="Price">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Value Type" SortExpression="ValueType">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                        <ItemTemplate>
                            <%# (PositionsTxValueTypes)DataBinder.Eval(Container.DataItem, "ValueType")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tx Side" SortExpression="Side">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                        <ItemTemplate>
                            <%# (Side)DataBinder.Eval(Container.DataItem, "Side")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tx Type" SortExpression="TransactionTypeDisplay">
                        <ItemTemplate>
                            <%# Util.SplitCamelCase(((string)Eval("TransactionTypeDisplay"))) %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Description" HeaderText="Tx Description" SortExpression="Description">
                        <ItemStyle wrap="False" Width="325px" />
                        <HeaderStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Tx Date" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" 
                        SortExpression="TransactionDate">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionCreationDate" HeaderText="Creation Date" DataFormatString="{0:dd-MM-yyyy}" 
                        HtmlEncode="False" SortExpression="TransactionCreationDate">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                </Columns>
            </b4f:MultipleSelectionGridView>
            <asp:ObjectDataSource ID="odsPositionTxsSecurity" runat="server" SelectMethod="GetPositionTxsSecurity"
                                  TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.PositionTransactionsAdapter">
                <SelectParameters>
                    <asp:SessionParameter Name="positionId" SessionField="SelectedPositionId" Type="Int32" DefaultValue="0" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="vwPositionTxsCashBaseCurrency" runat="server">
            <%--<b4f:MultipleSelectionGridView ID="gvPositionTxsCashBaseCurrency" runat="server" SkinID="custom-width" Width="1050px"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsPositionTxsCashBaseCurrency" PageSize="20" 
                    Caption="Position Transactions" CaptionAlign="Left" DataKeyNames="Key" SelectionBoxVisibleBy="IsStornoable"
                    OnRowCommand="gvPositionTx_RowCommand" >
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton 
                                ID="imbDetails"
                                CommandName="Details"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ParentTransaction_Key") %>'
                                runat="server" 
                                ImageUrl="~/layout/images/audit.gif" 
                                Height="16px" 
                                Width="16px" />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ParentTransaction_Key" HeaderText="TradeID" SortExpression="ParentTransaction_Key">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Tx Type" SortExpression="ParentTransaction_TransactionType">
                        <ItemTemplate>
                            <%# Util.SplitCamelCase(((string)Eval("ParentTransaction_TransactionType"))) %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="FullDescription">
                        <ItemTemplate>
                            <%# ((string)Eval("FullDescription")).Replace("\n", "<br />") %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" Width="550px" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="DisplayAmount" HeaderText="Amount" SortExpression="DisplayAmount">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Tx Date" DataFormatString="{0:dd-MM-yyyy}" 
                                    HtmlEncode="False" SortExpression="TransactionDate">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ParentTransaction_CreationDate" HeaderText="Creation Date" DataFormatString="{0:dd-MM-yyyy}" 
                                    HtmlEncode="False" SortExpression="ParentTransaction_CreationDate">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                </Columns>
            </b4f:MultipleSelectionGridView>
            <asp:ObjectDataSource ID="odsPositionTxsCashBaseCurrency" runat="server" SelectMethod="GetPositionTxsCashBaseCurrency"
                                  TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.PositionTransactionsAdapter">
                <SelectParameters>
                    <asp:SessionParameter Name="positionId" SessionField="SelectedPositionId" Type="Int32" DefaultValue="0" />
                </SelectParameters>
            </asp:ObjectDataSource>--%>
        </asp:View>
        <asp:View ID="vwPositionTxsCashForeignCurrency" runat="server">
<%--            <b4f:MultipleSelectionGridView ID="gvPositionTxsCashForeignCurrency" runat="server" SkinID="custom-width" Width="1000px"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsPositionTxsCashForeignCurrency" PageSize="20" 
                    Caption="Position Transactions" CaptionAlign="Left" DataKeyNames="Key" SelectionBoxVisibleBy="IsStornoable"
                    OnRowCommand="gvPositionTx_RowCommand" >
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton 
                                ID="imbDetails"
                                CommandName="Details"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ParentTransaction_Key") %>'
                                runat="server" 
                                ImageUrl="~/layout/images/audit.gif" 
                                Height="16px" 
                                Width="16px" />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ParentTransaction_Key" HeaderText="TradeID" SortExpression="ParentTransaction_Key">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ExchangeRate" HeaderText="Ex Rate" SortExpression="ExchangeRate" DataFormatString="{0:###.0000}">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Value Type" SortExpression="IsCV">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                        <ItemTemplate>
                            <%# (CVType)DataBinder.Eval(Container.DataItem, "IsCV")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tx Side" SortExpression="Side">
                        <ItemStyle wrap="False" />
                        <HeaderStyle wrap="False" />
                        <ItemTemplate>
                            <%# (Side)DataBinder.Eval(Container.DataItem, "Side")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tx Type" SortExpression="ParentTransaction_TransactionType">
                        <ItemTemplate>
                            <%# Util.SplitCamelCase(((string)Eval("ParentTransaction_TransactionType"))) %>
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                        <HeaderStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Description" HeaderText="Tx Description" SortExpression="Description">
                        <ItemStyle wrap="False" Width="250px" />
                        <HeaderStyle wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionDate" HeaderText="Tx Date" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" 
                        SortExpression="TransactionDate">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ParentTransaction_CreationDate" HeaderText="Creation Date" DataFormatString="{0:dd-MM-yyyy}" 
                        HtmlEncode="False" SortExpression="ParentTransaction_CreationDate">
                        <ItemStyle wrap="False" horizontalalign="Right" />
                        <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                    </asp:BoundField>
                </Columns>
            </b4f:MultipleSelectionGridView>
            <asp:ObjectDataSource ID="odsPositionTxsCashForeignCurrency" runat="server" SelectMethod="GetPositionTxsCashForeignCurrency"
                                  TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.PositionTransactionsAdapter">
                <SelectParameters>
                    <asp:SessionParameter Name="positionId" SessionField="SelectedPositionId" Type="Int32" DefaultValue="0" />
                </SelectParameters>
            </asp:ObjectDataSource>--%>
        </asp:View>
    </asp:MultiView>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Panel ID="pnlStornoFields" runat="server" Height="50px" Width="752px" Visible="False">
        <table width="760px">
            <tr style="height:24px">
                <td style="width:110px">Storno Account:</td>
                <td style="width:650px">
                    <asp:DropDownList ID="ddlStornoAccount" SkinID="custom-width" runat="server" Width="250px" DataSourceID="odsStornoAccounts" 
                        DataTextField="DisplayNumberWithName" DataValueField="Key">
                    </asp:DropDownList>&nbsp;
                    <asp:HiddenField ID="hdnPositionTxId" runat="server" />
                    <asp:ObjectDataSource ID="odsStornoAccounts" runat="server" SelectMethod="GetStornoAccounts"
                        TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.PositionTransactionsAdapter">
                        <SelectParameters>
                            <asp:SessionParameter Name="positionId" SessionField="SelectedPositionId" Type="Int32" DefaultValue="0" />
                            <asp:ControlParameter ControlID="hdnPositionTxId" Name="positionTxId" PropertyName="Value" Type="Int32" DefaultValue="0" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
            <tr style="height:24px">
                <td >Reason:</td>
                <td>
                    <asp:TextBox ID="txtReason" runat="server" SkinID="custom-width" Width="450px"></asp:TextBox>
                    &nbsp; &nbsp;
                    <asp:RequiredFieldValidator ID="rfvReason" runat="server" ControlToValidate="txtReason"
                                                ErrorMessage="Field cannot be empty"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <br />
    </asp:Panel>
    <uc1:BackButton ID="ctlBackButton" runat="server" />
    <asp:Button ID="btnStorno" runat="server" OnClick="btnStorno_Click" Text="Storno" Width="80px" />&nbsp;
    <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" Width="80px" Visible="False" CausesValidation="False" />
</asp:Content>


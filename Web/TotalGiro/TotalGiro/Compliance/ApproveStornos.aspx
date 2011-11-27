<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ApproveStornos.aspx.cs" 
    Inherits="Compliance_ApproveStornos" Title="Approve Storno Transactions" Theme="Neutral" %>

<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <cc1:MultipleSelectionGridView ID="gvStornos" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" PageSize="25" Caption="Storno Transactions" CaptionAlign="Left" 
        DataKeyNames="Key" DataSourceID="odsStornos">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="TradeID" SortExpression="Key">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account#" SortExpression="AccountA_Number" >
                <ItemTemplate>
                    <uc1:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="true" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountID='<%#  DataBinder.Eval(Container.DataItem, "AccountA_Key") %>'
                        />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Name" SortExpression="AccountA_ShortName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                        MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "AccountA_ShortName") %>' />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Side" SortExpression="TxSide">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "TxSide")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="Description">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel id="TruncLabel1" 
                        runat="server"
                        width="40"
                        text='<%# DataBinder.Eval(Container.DataItem, "Description") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ValueSize_DisplayString" HeaderText="Value" SortExpression="ValueSize" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Price" SortExpression="Price" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CounterValueSize_DisplayString" HeaderText="Countervalue" SortExpression="CounterValueSize" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ex Rate" SortExpression="ExchangeRate">
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle cssclass="alignright" horizontalalign="Right" wrap="False" />
                <ItemTemplate>
                    <%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}"): "") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TransactionDate" HeaderText="Tx Date" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" 
                SortExpression="TransactionDate">
                <ItemStyle wrap="False" horizontalalign="Right" />
                <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Creation Date" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" 
                SortExpression="CreationDate">
                <ItemStyle wrap="False" horizontalalign="Right" />
                <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Reason" SortExpression="Reason">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel id="TruncLabel2" 
                        runat="server"
                        width="50"
                        text='<%# DataBinder.Eval(Container.DataItem, "Reason") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsStornos" runat="server" SelectMethod="GetStornoTransactions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Compliance.ApproveStornosAdapter"></asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClick="btnApprove_Click" />&nbsp;
    <asp:Button ID="btnDisapprove" runat="server" Text="Disapprove" OnClick="btnDisapprove_Click" OnClientClick="return confirm('Disapprove trades?')" />
</asp:Content>


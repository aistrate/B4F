<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="ApproveOrders.aspx.cs" Inherits="ApproveOrders" Theme="Neutral" %>

<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" />
    &nbsp;<br />
    <br />
        <cc1:MultipleSelectionGridView 
            ID="gvUnapprovedOrders" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataSourceID="odsUnapprovedOrders"
            OnRowCommand="gvUnapprovedOrders_RowCommand"
            OnRowDataBound="gvUnapprovedOrders_RowDataBound"
            cssclass="padding"
            AllowPaging="True" 
            PageSize="15" Caption="Unapproved Orders" CaptionAlign="Left" AllowSorting="True" DataKeyNames="AccountID">
            <Columns>
                <asp:TemplateField HeaderText="Account#" SortExpression="AccountNumber">
                    <HeaderStyle Wrap="False" />
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <ItemTemplate>
                        <uc2:AccountLabel ID="ctlAccountLabel" 
                            runat="server" 
                            RetrieveData="false" 
                            Width="120px" 
                            NavigationOption="PortfolioView"
                            AccountDisplayOption="DisplayNumber"
                            
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Account Name" SortExpression="AccountName">
                    <ItemTemplate>
                        <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                            MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "AccountName") %>' />
                    </ItemTemplate>
                    <HeaderStyle wrap="False" />
                    <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="NrOfOrders" HeaderText="Number of orders" SortExpression="NrOfOrders" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="Commission" HeaderText="Commission" SortExpression="Commission" DataFormatString="{0:###,##0.00}" HtmlEncode="False"  >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:CommandField SelectText="View" ShowSelectButton="True" >
                    <ItemStyle wrap="False" />
                </asp:CommandField>
            </Columns>
        </cc1:MultipleSelectionGridView>
    &nbsp;<br />
    <asp:Button ID="btnApprove" runat="server" OnClick="btnApprove_Click"
        Text="Approve" /><br />
    <br />
    <asp:ObjectDataSource ID="odsUnapprovedOrders" runat="server" SelectMethod="GetUnapprovedGroupedOrders" 
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.ApproveOrdersAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
</asp:Content>

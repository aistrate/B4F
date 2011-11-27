<%@ Page Title="Clients with Cash without Orders" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="ClientsCashNoOrders.aspx.cs" Inherits="ClientsCashNoOrders" %>

<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:GridView ID="gvPositions" runat="server" AllowSorting="True"
        AutoGenerateColumns="False" Caption="Positions" GridLines="None" CaptionAlign="Top"
        DataSourceID="odsPositions" AllowPaging="True" DataKeyNames="Key" Width="870px"
        SkinID="custom-width" PageSize="20"
        OnRowDataBound="gvPositions_RowDataBound" >
        <Columns>
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
                    <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                        MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TotalCashDisplay" HeaderText="TotalCashDisplay" SortExpression="TotalCashDisplay">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="TotalCashFundDisplay" HeaderText="TotalCashFundDisplay"
                SortExpression="TotalCashFundDisplay">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsPositions" runat="server" SelectMethod="GetAccountsWithCashNoOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClientsCashNoOrdersAdapter">
    </asp:ObjectDataSource>
</asp:Content>

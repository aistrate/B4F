<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="ClosedPositions.aspx.cs" Inherits="Portfolio_ClosedPositions" Title="Closed Positions" 
    Theme="Neutral" %>

<%@ Register Src="../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <asp:Table ID="Table1" runat="server" Width="709px" Height="8px">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell1" runat="server" Width="80px">Account:</asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" Font-Bold="True"><asp:Label ID="lblAccount" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Panel ID="pnlCash" runat="server" Width="125px">
        <asp:GridView ID="gvClosedCash" runat="server" AutoGenerateColumns="False" Caption="Closed Cash Positions" SkinID="custom-width"
            CaptionAlign="Left" PageSize="5" AllowPaging="True" AllowSorting="True" DataSourceID="odsCash" DataKeyNames="Key" Width="650px"
             OnRowCommand="gridView_RowCommand">
            <Columns>
                <asp:BoundField DataField="Instrument_DisplayName" HeaderText="Currency" SortExpression="Instrument_DisplayName">
                    <ItemStyle wrap="False" horizontalalign="Left" />
                    <HeaderStyle wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Ex Rate" SortExpression="ExchangeRate_Rate">
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle cssclass="alignright" horizontalalign="Right" wrap="False" />
                    <ItemTemplate>
                        <%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate_Rate") != 1m ? 
                            Eval("ExchangeRate_Rate", "{0:###.0000}") : "")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField SelectText="View" ShowSelectButton="True">
                    <ItemStyle Wrap="False" Width="29px" />
                </asp:CommandField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsCash" runat="server" SelectMethod="GetClosedCashPositions" 
            TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClosedPositionsAdapter" OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:SessionParameter DefaultValue="" Name="accountId" SessionField="SelectedAccountId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
    </asp:Panel>
    <asp:GridView ID="gvClosedPositions" runat="server" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" SkinID="custom-width"
        Caption="Closed Positions" CaptionAlign="Left" DataSourceID="odsPositions" AllowPaging="True" DataKeyNames="Key" Width="650px"
        OnDataBound="gvClosedPositions_DataBound" OnRowDataBound="gvClosedPositions_RowDataBound" OnRowCommand="gridView_RowCommand">
        <Columns>
            <asp:BoundField DataField="Isin" HeaderText="ISIN" SortExpression="Isin">
                <ItemStyle wrap="False" horizontalalign="Left" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="InstrumentName" HeaderText="Instrument" SortExpression="InstrumentName">
                <ItemStyle wrap="False" horizontalalign="Left" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Price" SortExpression="Price">
                <ItemStyle wrap="False" horizontalalign="Right" />
                <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ex Rate" SortExpression="ExchangeRate">
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle cssclass="alignright" horizontalalign="Right" wrap="False" />
                <ItemTemplate>
                    <%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}"): "") %>
</ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField SelectText="View" ShowSelectButton="True" >
                <ItemStyle Wrap="False" Width="29px" />
            </asp:CommandField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsPositions" runat="server" SelectMethod="GetClosedSecurityPositions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClosedPositionsAdapter">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="" Name="accountId" SessionField="SelectedAccountId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <uc1:BackButton ID="ctlBackButton" runat="server" />
</asp:Content>


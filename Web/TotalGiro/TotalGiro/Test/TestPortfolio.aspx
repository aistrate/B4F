<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="TestPortfolio.aspx.cs" Inherits="TestPortfolio" Theme="Neutral" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:DropDownList ID="ddlAccount" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged">
        <asp:ListItem Value="0">--</asp:ListItem>
        <asp:ListItem Value="354">EGVL010032 (Groen)</asp:ListItem>
    </asp:DropDownList><br />
    <br />
    <cc1:MultipleSelectionGridView id="MultipleSelectionGridView1" runat="server" AllowPaging="True" AllowSorting="True" 
        AutoGenerateColumns="False" Caption="Test Client Portfolio I" CaptionAlign="Left" DataSourceID="odsClientPortfolio" DataKeyNames="Key">
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
            <asp:BoundField DataField="Percentage" HeaderText="Percentage" SortExpression="Percentage" DataFormatString="{0:###,00}%">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ex Rate" SortExpression="ExchangeRate">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}"): "") %>'></asp:Label>
                
</ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:Button ID="btnUncheckSelectAll" runat="server" OnClick="btnUncheckSelectAll_Click"
        Text="Uncheck Select All" />
    <asp:Button ID="btnCheckSelectAll" runat="server" OnClick="btnCheckSelectAll_Click"
        Text="Check Select All" />
    <asp:Button ID="btnShowSelectAll" runat="server" 
        Text="Show Select All" OnClick="btnShowSelectAll_Click" />
    <asp:Label ID="lblShowSelectAll" runat="server"></asp:Label><br />
    <asp:Button ID="btnClearSelection" runat="server" OnClick="btnClearSelection_Click"
        Text="Clear Selection" />
    <asp:Button ID="btnSelectAllRecords" runat="server" 
        Text="Select All Records" OnClick="btnSelectAllRecords_Click" />
    <asp:Button ID="btnShowSelectedIds" runat="server" OnClick="btnShowSelectedIds_Click"
        Text="Show Selected IDs" /><br />
    <asp:Label ID="lblSelectedIds" runat="server"></asp:Label><br />
    <asp:ObjectDataSource ID="odsClientPortfolio" runat="server" SelectMethod="GetPositions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.ClientPortfolioAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAccount" DefaultValue="" Name="accountId" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:CheckBox ID="chkMultipleSelection" runat="server" AutoPostBack="True" Checked="True"
        OnCheckedChanged="chkMultipleSelection_CheckedChanged" Text="Multiple Selection" /><br />
    <br />
    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" 
        AutoGenerateColumns="False" Caption="Test Client Portfolio II" CaptionAlign="Left" DataSourceID="odsClientPortfolio" DataKeyNames="Key" OnRowCommand="GridView1_RowCommand">
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
            <asp:BoundField DataField="Percentage" HeaderText="Percentage" SortExpression="Percentage" DataFormatString="{0:###,00}%">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ex Rate" SortExpression="ExchangeRate">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}"): "") %>'>
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:CommandField SelectText="View" ShowSelectButton="True" >
                <ItemStyle Wrap="False" />
            </asp:CommandField>
        </Columns>
    </asp:GridView>
    &nbsp;<br />
</asp:Content>

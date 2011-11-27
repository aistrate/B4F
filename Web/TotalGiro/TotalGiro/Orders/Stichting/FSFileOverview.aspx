<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="FSFileOverview.aspx.cs" Inherits="FSFileOverview" Theme="Neutral" 
    Title="Fund Settle Export Files" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
   <table border="0">
    <tr>
        <td align="center">Show files from this date:<br/><asp:Calendar ID="calStart" runat="server" OnSelectionChanged="calStart_SelectionChanged"></asp:Calendar></td>
        <td align="center">Until(incl.) this date:<br/><asp:Calendar ID="calEnd" runat="server" OnSelectionChanged="calEnd_SelectionChanged"></asp:Calendar></td>
    </tr>
    <tr>
        <td align="center">Search FileID:&nbsp;
            <asp:TextBox ID="txtFile" runat="server" AutoPostBack="True" OnTextChanged="txtFile_TextChanged"></asp:TextBox></td>
        <td align="center">Search FsNumber:&nbsp;
            <asp:TextBox ID="txtFsNumber" runat="server" AutoPostBack="True" OnTextChanged="txtFsNumber_TextChanged"></asp:TextBox></td>
    </tr>
   </table>
   
    <asp:GridView 
        ID="gvFSFileOverview" 
        runat="server" 
        AutoGenerateColumns="False" 
        DataSourceID="odsFSFileOverview"
        CssClass="padding"
        AllowPaging="True" 
        Caption="Export files" 
        CaptionAlign="Left"
        AllowSorting="True" 
        DataKeyNames="Key"
        OnRowUpdated = "gvFSFileOverview_RowUpdated"
        OnRowEditing = "gvFSFileOverview_RowEditing"
        OnRowCancelingEdit = "gvFSFileOverview_RowCancelingEdit"
        OnRowDataBound="gvFSFileOverview_RowDataBound"
        OnRowCommand="gvFSFileOverview_RowCommand" PageSize="15">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="FileID" SortExpression="Key" ReadOnly="True" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="File Name" SortExpression="FileName">
                <ItemTemplate>
                    <a href="<%# Request.Url.ToString().Replace(Request.Url.Segments[Request.Url.Segments.Length-1], "DownloadFSFile.aspx" + QueryStringModule.Encrypt("fileid=" + DataBinder.Eval(Container.DataItem, "Key").ToString())) %>" ><%# DataBinder.Eval(Container.DataItem, "FileName").ToString() + DataBinder.Eval(Container.DataItem, "Key").ToString() + DataBinder.Eval(Container.DataItem, "FileExt").ToString()%></a>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Orders_Count" HeaderText="Orders" SortExpression="Orders_Count" ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="FSNumber" HeaderText="FS Number" SortExpression="FSNumber" >
                <ItemStyle Wrap="False" Width="145px"/>
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewOrders" 
                        Text="Orders"
                        CommandName="ViewOrders"
                        Visible="true"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtTotal" 
                        Text="Total"
                        CommandName="Total"
                        Visible="true"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDeleteFile" 
                        Text="Delete"
                        CommandName="DeleteFile"
                        Visible="false"
                        OnClientClick="return confirm('Delete file?')"/>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" >
                <ItemStyle Wrap="False" Width="82px" />
            </asp:CommandField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFSFileOverview" runat="server" SelectMethod="GetFSFileOverviewByCriteria" UpdateMethod="UpdateFSFileNumber"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.FSFileOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="calStart" Name="p_datStartDate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="calEnd" Name="p_datEndDate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="txtFile" Name="p_intFileID" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtFsNumber" Name="p_strFsNumber" PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:GridView 
        ID="gvOrderOverview" 
        runat="server" 
        AutoGenerateColumns="False" 
        CssClass="padding"
        AllowPaging="True" 
        PageSize="10" 
        Caption="Orders per export file" 
        CaptionAlign="Left" 
        AllowSorting="True"
        DataKeyNames="OrderID" DataSourceID="odsOrderOverview">
        <Columns>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrument_DisplayName">
                <ItemTemplate>
                    <trunc:TruncLabel
                        runat="server" ID="Instrument"
                        Width="40"
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Value_DisplayString" HeaderText="Value" SortExpression="Value" >
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" >
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status" >
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="OrderID" HeaderText="OrderID" SortExpression="OrderID" >
                <ItemStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsOrderOverview" runat="server" SelectMethod="GetOrdersPerExportFile"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.FSFileOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvFSFileOverview" Name="fileid" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:GridView 
        ID="gvCurrencyOverview" 
        runat="server" 
        AutoGenerateColumns="False" 
        CssClass="padding"
        AllowPaging="True" 
        PageSize="5" 
        Caption="Subtotals per currency" 
        CaptionAlign="Left" 
        AllowSorting="True"
        DataSourceID="odsCurrencyOverview">
        <Columns>
            <asp:BoundField DataField="CurrencyName" HeaderText="Currency" SortExpression="CurrencyName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="OrderCount" HeaderText="Orders" SortExpression="OrderCount">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCurrencyOverview" runat="server" SelectMethod="GetSubtotalsPerCurrency"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.FSFileOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvFSFileOverview" Name="fileid" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Button ID="btnToManualDesk" runat="server" OnClick="btnToManualDesk_Click" Text="FundSettle Desk" /><br />
</asp:Content>

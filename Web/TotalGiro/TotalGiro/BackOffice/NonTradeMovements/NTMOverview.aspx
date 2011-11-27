<%@ Page Title="NTM Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="NTMOverview.aspx.cs" Inherits="NTMOverview" %>

<%@ Register Src="../../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Orders.Transfers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <br />
    <asp:GridView ID="gvNTMTransfers" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsNTMTransfers" AutoGenerateColumns="False" Caption="NTM Transfers"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20" 
        SkinID="custom-width" Width="850px"
        OnRowDataBound="gvNTMTransfers_RowDataBound" >
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Key" SortExpression="Key">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Transfer Date" SortExpression="TransferDate">
                <ItemTemplate>
                    <asp:Label ID="lblTransferDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "TransferDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "TransferDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="OriginInternal" HeaderText="Origin Internal" SortExpression="OriginInternal">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account A" SortExpression="AccountNumberA">
                <ItemTemplate>
                    <uc1:AccountLabel ID="ctlAccountLabelA" 
                        runat="server" 
                        RetrieveData="false" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountDisplayOption="DisplayNumber"
                        AltText='<%# DataBinder.Eval(Container.DataItem, "AccountNumberA") %>'
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="DestinationInternal" HeaderText="Destination Internal"
                SortExpression="DestinationInternal">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account B" SortExpression="AccountNumberB">
                <ItemTemplate>
                    <uc1:AccountLabel ID="ctlAccountLabelB" 
                        runat="server" 
                        RetrieveData="false" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        AccountDisplayOption="DisplayNumber"
                        AltText='<%# DataBinder.Eval(Container.DataItem, "AccountNumberB") %>'
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>


            <asp:TemplateField HeaderText="Status" SortExpression="TransferStatus">
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <%# (TransferStatus)DataBinder.Eval(Container.DataItem, "TransferStatus")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDetails" runat="server" CausesValidation="False" Text="Details"
                        CommandName="ViewDetails" ToolTip="View Details" OnCommand="lbtDetails_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsNTMTransfers" runat="server" SelectMethod="GetNtmTransfers"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.TransferAdapter" OldValuesParameterFormatString="original_{0}">
    </asp:ObjectDataSource>
    <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="9"></asp:TableCell>
            <asp:TableCell ID="TableCell1" runat="server">
                <asp:Button ID="btnNewTransfer" runat="server" Text="New Transfer" CausesValidation="False"
                    OnClick="btnNewTransfer_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

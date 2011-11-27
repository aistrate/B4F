<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="RebalanceExcludeInstruments.aspx.cs" Inherits="RebalanceExcludeInstruments" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Register Src="../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:GridView 
        ID="gvInstrumentsToExclude"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="15" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        Caption="Instruments"
        CaptionAlign="Left"
        DataSourceID="odsInstruments" AllowSorting="True" >
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="ID" SortExpression="Key" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayName" HeaderText="Instrument" SortExpression="DisplayName">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Isin" HeaderText="ISIN" SortExpression="Isin">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="SecCategoryName" HeaderText="Category" SortExpression="SecCategoryName" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayCurrentPrice" HeaderText="Current Price" SortExpression="DisplayCurrentPrice" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Price Date" SortExpression="DisplayCurrentPriceDate">
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "DisplayCurrentPriceDate") != DateTime.MinValue ? 
                        ((DateTime)DataBinder.Eval(Container.DataItem, "DisplayCurrentPriceDate")).ToString("d MMMM yyyy") : "") %>
                </ItemTemplate>
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Button ID="btnOK" runat="server" Text="OK" Width="75px" Visible="false" />&nbsp
    <uc1:BackButton ID="ctlBackButton" runat="server" />
    <asp:ObjectDataSource ID="odsInstruments" runat="server" SelectMethod="GetInstrumentsFromInstructions"
        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionManagementAdapter" >
        <SelectParameters>
            <asp:SessionParameter Name="instructions" SessionField="SelectedInstructionIds" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
    &nbsp;<br />
    &nbsp;&nbsp;<br />
    <br />
    <br />
</asp:Content>


<%@ Page Title="Nav Calculations Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="NavCalculationsOverview.aspx.cs" Inherits="NavCalculationsOverview" %>

<%@ Register Src="~/UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <asp:HiddenField ID="hdnFundID" runat="server" />
    <asp:Table runat="server" Width="100%">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="r1c1" runat="server" ColumnSpan="1">
                <asp:Label ID="lblFundName" runat="server" Text="Fund Name"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="r1c2" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtFundName" runat="server" SkinID="custom-width" Width="600"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell Style="height: 24px">
                <asp:Label ID="Label2" runat="server" Text="Transaction Date From:"></asp:Label></asp:TableCell>
            <asp:TableCell Style="width: 210px">
                <uc1:DatePicker ID="dpNavDateFrom" runat="server" />
            </asp:TableCell>
            <asp:TableCell Style="width: 50px">
                <asp:Label ID="Label3" runat="server" Text="To:"></asp:Label></asp:TableCell>
            <asp:TableCell Style="width: 210px">
                <uc1:DatePicker ID="dpNavDateTo" runat="server" />
            </asp:TableCell>
            <asp:TableCell></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:GridView ID="gvCalculations" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsCalculations" AutoGenerateColumns="False" Caption="Calculations"
        CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:TemplateField HeaderText="ValuationDate" SortExpression="ValuationDate">
                <ItemTemplate>
                    <asp:Label ID="lblValuationDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "ValuationDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "ValuationDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TotalParticipationsAfterFill" HeaderText="TotalParticipationsAfterFill"
                SortExpression="TotalParticipationsAfterFill">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NavPerUnitDisplayString" HeaderText="NavPerUnitDisplayString"
                SortExpression="NavPerUnitDisplayString">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDetails" runat="server" CausesValidation="False" Text="Details"
                        CommandName="ViewDetails" ToolTip="View details of this calculation" OnCommand="lbtDetails_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCalculations" runat="server" SelectMethod="GetNavCalculations"
        TypeName="B4F.TotalGiro.ApplicationLayer.VirtualFunds.NavCalculationsOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="hdnFundID" Name="fundID" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="dpNavDateFrom" Name="navDateFrom" PropertyName="SelectedDate"
                Type="DateTime" />
            <asp:ControlParameter ControlID="dpNavDateTo" Name="navDateTo" PropertyName="SelectedDate"
                Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

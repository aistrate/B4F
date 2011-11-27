<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="AuditLogDetails.aspx.cs" Inherits="Auditing_AuditLogDetails" Title="Audit Log Details" 
    Theme="Neutral" %>

<%@ Register Src="../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <asp:Table ID="Table1" runat="server" Width="323px" Height="51px">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell1" runat="server" Width="100px">Entity Type:</asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" Font-Bold="True"><asp:Label ID="lblEntityType" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell3" runat="server">Key:</asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" Font-Bold="True"><asp:Label ID="lblKey" runat="server"></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:GridView ID="gvAuditLogEvents" runat="server" AllowPaging="True" AllowSorting="True" Caption="Events" CaptionAlign="Left" 
        DataSourceID="odsAuditLogEvents" AutoGenerateColumns="False" DataKeyNames="AuditLogEntryId" 
        OnSelectedIndexChanged="gvAuditLogEvents_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="EventName" HeaderText="Event" SortExpression="EventName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DateCreated" HeaderText="Date" SortExpression="DateCreated" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" HtmlEncode="False" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="UserName" HeaderText="User" SortExpression="UserName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:CommandField ShowSelectButton="True" SelectText="Fields" />
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAuditLogEvents" runat="server" SelectMethod="GetAuditLogEvents"
        TypeName="B4F.TotalGiro.ApplicationLayer.Auditing.AuditLogDetailsAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="entityClass" SessionField="EntityClass" Type="String" />
            <asp:SessionParameter Name="entityKey" SessionField="EntityKey" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <uc1:BackButton ID="btnBack" runat="server" />
    <br />
    <br />
    <asp:GridView ID="gvAuditLogFields" runat="server" AllowSorting="True" Caption="Fields" CaptionAlign="Left" 
        DataSourceID="odsAuditLogFields" Visible="False" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="FieldName" HeaderText="Field" SortExpression="FieldName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="OldValue" HeaderText="Old Value" SortExpression="OldValue">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NewValue" HeaderText="New Value" SortExpression="NewValue">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAuditLogFields" runat="server" SelectMethod="GetAuditLogFields" TypeName="B4F.TotalGiro.ApplicationLayer.Auditing.AuditLogDetailsAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvAuditLogEvents" Name="auditLogEntryId" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="AuditLog.aspx.cs" Inherits="Auditing_AuditLog" Title="Audit Log" Theme="Neutral" %>

<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table style="width: 610px">
        <tr>
            <td style="vertical-align: top">
                Entity Type:
            </td>
            <td style="vertical-align: top">
                <asp:TextBox ID="txtEntityType" runat="server"></asp:TextBox></td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td style="vertical-align: top">
                Key:
            </td>
            <td style="vertical-align: top">
                <asp:TextBox ID="txtKey" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="revKey" runat="server" ControlToValidate="txtKey"
                    ErrorMessage="Value of search field Key should be an integer." ValidationExpression="^\s*(-)?\d+\s*$">*</asp:RegularExpressionValidator></td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td style="width: 150px; vertical-align: top">
                Created From:
            </td>
            <td style="width: 205px; vertical-align: top">
                <uc1:DatePicker ID="dpCreatedFrom" runat="server" />
            </td>
            <td style="width: 50px; vertical-align: top">
                &nbsp; &nbsp;To:
            </td>
            <td style="width: 205px; vertical-align: top">
                <uc1:DatePicker ID="dpCreatedTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top">
                Created By:
            </td>
            <td style="vertical-align: top">
                <asp:TextBox ID="txtCreatedBy" runat="server"></asp:TextBox></td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td style="vertical-align: top">
                Last Updated From:
            </td>
            <td style="vertical-align: top">
                <uc1:DatePicker ID="dpLastUpdatedFrom" runat="server" />
            </td>
            <td style="vertical-align: top">
                &nbsp; &nbsp;To:
            </td>
            <td style="vertical-align: top">
                <uc1:DatePicker ID="dpLastUpdatedTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top">
                Last Updated By:
            </td>
            <td style="vertical-align: top">
                <asp:TextBox ID="txtLastUpdatedBy" runat="server"></asp:TextBox></td>
            <td colspan="2"></td>
        </tr>
    </table>
    <asp:ValidationSummary ID="vsValidation" runat="server" DisplayMode="List" />
    <br />
    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /><br />
    <br />
    <br />
    <asp:GridView ID="gvAuditLogEntities" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" Visible="false"
        DataSourceID="odsAuditLogEntities" Caption="Entities" CaptionAlign="Left" PageSize="20" DataKeyNames="EntityClass,EntityKey" 
        OnSelectedIndexChanged="gvAuditLogEntities_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="EntityClass" HeaderText="Entity Type" SortExpression="EntityClass">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="EntityKey" HeaderText="Key" SortExpression="EntityKey">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="LastUpdated" HeaderText="Last Updated" SortExpression="LastUpdated">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="LastUpdatedBy" HeaderText="Last Updated By" SortExpression="LastUpdatedBy">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="EventCount" HeaderText="Events" SortExpression="EventCount">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:CommandField SelectText="View Events" ShowSelectButton="True" >
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:CommandField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAuditLogEntities" runat="server" SelectMethod="GetAuditLogEntities"
        TypeName="B4F.TotalGiro.ApplicationLayer.Auditing.AuditLogAdapter" OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:SessionParameter Name="entityType" SessionField="EntityType" Type="String" ConvertEmptyStringToNull="False" />
            <asp:SessionParameter Name="key" SessionField="AuditEntityKey" Type="Int32" />
            <asp:SessionParameter Name="createdFrom" SessionField="CreatedFrom" Type="DateTime" />
            <asp:SessionParameter Name="createdTo" SessionField="CreatedTo" Type="DateTime" />
            <asp:SessionParameter Name="createdBy" SessionField="CreatedBy" Type="String" ConvertEmptyStringToNull="False" />
            <asp:SessionParameter Name="lastUpdatedFrom" SessionField="LastUpdatedFrom" Type="DateTime" />
            <asp:SessionParameter Name="lastUpdatedTo" SessionField="LastUpdatedTo" Type="DateTime" />
            <asp:SessionParameter Name="lastUpdatedBy" SessionField="LastUpdatedBy" Type="String" ConvertEmptyStringToNull="False" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


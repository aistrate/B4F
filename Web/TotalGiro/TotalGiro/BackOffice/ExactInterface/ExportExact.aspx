<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="ExportExact.aspx.cs"
    Inherits="ExportExact" Title="Export to Exact" Theme="Neutral" %>

<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <table>
        <tr>
            <td style="height: 30px; width: 80px">
                Date up to:
            </td>
            <td style="height: 30px; width: 210px">
                <uc1:DatePicker ID="dpDateUntil" runat="server" IsButtonDeleteVisible="false" />
            </td>
            <td style="height: 30px">
                <asp:Button ID="btnExportToExact" runat="server" Text="Export to Exact" OnClick="btnExportToExact_Click"
                    OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerText = '\nExporting...'" />
            </td>
        </tr>
        <%--        <tr>
            <td style="height: 30px; width: 80px">
                Date up to:
            </td>
            <td style="height: 30px; width: 210px">
                <uc1:DatePicker ID="dpCreateDateUntil" runat="server" IsButtonDeleteVisible="false" />
            </td>
            <td style="height: 30px">
                <asp:Button ID="btnCreateEntriesToExport" runat="server" Text="Create Entries To Export"
                    OnClick="btnCreateEntriesToExport_Click" OnClientClick="document.getElementById('ctl00_bodyContentPlaceHolder_lblErrorMessage').innerText = '\ncreating...'" />
            </td>
        </tr>--%>
    </table>
    <br />
    <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell1" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell3" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell5" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell7" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell13" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell15" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell17" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell18" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell19" runat="server"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="20" runat="server">
                <asp:GridView ID="gvExportedFiles" runat="server" AllowPaging="True" AllowSorting="True"
                    DataSourceID="odsExportedFiles" AutoGenerateColumns="False" Caption="Exported Files"
                    CaptionAlign="Left" DataKeyNames="Key" PageSize="20" OnRowCommand="gvExportedFiles_OnRowCommand">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FullPathName" HeaderText="FullPathName" SortExpression="FullPathName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreationDate" HeaderText="CreationDate" SortExpression="CreationDate">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemStyle Wrap="False" />
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtViewFile" Text="Records" CommandName="ViewRecords" />
                                <asp:LinkButton runat="server" ID="lbtViewFileRecords" Text="Undo" CommandName="UndoFile" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsExportedFiles" runat="server" SelectMethod="GetExportedFiles"
                    TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.ExportExactAdapter" OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="20"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ID="TableCell20" ColumnSpan="20" runat="server">
                <asp:GridView ID="gvLedgerEntriesinFile" runat="server" AllowPaging="True" AllowSorting="True"
                    DataSourceID="odsLedgerEntriesinFile" AutoGenerateColumns="False" Caption="Entries in File"
                    CaptionAlign="Left" DataKeyNames="Key" PageSize="20" Visible="false">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FullPathName" HeaderText="FullPathName" SortExpression="FullPathName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreationDate" HeaderText="CreationDate" SortExpression="CreationDate">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsLedgerEntriesinFile" runat="server" SelectMethod="GetLedgerEntriesinFile"
                    TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.ExportExactAdapter" OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="gvExportedFiles" Name="fileID" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
</asp:Content>

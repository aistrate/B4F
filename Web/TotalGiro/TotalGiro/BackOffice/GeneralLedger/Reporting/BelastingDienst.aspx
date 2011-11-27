<%@ Page Title="Belasting Dienst" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="BelastingDienst.aspx.cs" Inherits="BelastingDienst" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:Table ID="Table1" runat="server" Width="100%">
        <asp:TableRow ID="TableRow1" runat="server" Width="100%">
            <asp:TableCell ID="TableCell1" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell3" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell5" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell7" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell13" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell15" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell17" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell18" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell19" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell20" runat="server">            
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableCell ID="TableCell22" runat="server" ColumnSpan="1">
            </asp:TableCell>
            <asp:TableCell ID="TableCell23" ColumnSpan="3" runat="server">
                <asp:Label runat="server" ID="lblLastFileCreated" Text="Last File Created:" />
            </asp:TableCell><asp:TableCell ID="TableCell24" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell25" ColumnSpan="5" runat="server">
                <asp:TextBox runat="server" ID="txtLastFileCreated" ReadOnly="true" />
            </asp:TableCell><asp:TableCell ID="TableCell26" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell27" runat="server" ColumnSpan="3">
                <asp:Label runat="server" ID="lblNextFinancialYear" Text="Next Financial Year:" />
            </asp:TableCell><asp:TableCell ID="TableCell28" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell29" runat="server" ColumnSpan="5">
                <asp:TextBox runat="server" ID="txtNextFinancialYear" ReadOnly="true" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableCell ID="TableCell30" runat="server" ColumnSpan="1">
            </asp:TableCell>
            <asp:TableCell ID="TableCell31" ColumnSpan="3" runat="server">
                <asp:Label runat="server" ID="lblCurrentDate" Text="Current Date:" />
            </asp:TableCell><asp:TableCell ID="TableCell32" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell33" ColumnSpan="5" runat="server">
                <asp:TextBox runat="server" ID="txtCurrentDate" ReadOnly="true" />
            </asp:TableCell><asp:TableCell ID="TableCell34" runat="server" ColumnSpan="5">
            </asp:TableCell><asp:TableCell ID="TableCell37" runat="server" ColumnSpan="5">
                <asp:Button ID="btnCreateDividWep" runat="server" Text="Create Divid WEP" 
                    CausesValidation="False"
                    Enabled="True" OnClick="btnCreateDividWep_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table runat="server" ID="Table2" Width="100%">
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell21" runat="server" ColumnSpan="20">
                <asp:GridView ID="gvDividWepFiles" runat="server" AutoGenerateColumns="False" PageSize="25"
                    AllowPaging="True" Caption="DividWep File Reportiing" CaptionAlign="Left" AllowSorting="True"
                    OnRowCommand="gvDividWepFiles_OnRowCommand" DataSourceID="odsDividWepFiles" SkinID="spreadsheet"
                    DataKeyNames="Key" Visible="True" Width="100%">
                    <Columns>
                        <asp:BoundField DataField="FinancialYear" HeaderText="FinancialYear" SortExpression="FinancialYear">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FileName" HeaderText="FileName" SortExpression="FileName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CodeFinance" HeaderText="CodeFinance" SortExpression="CodeFinance">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotalWep" HeaderText="TotalWep" SortExpression="TotalWep">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="InstelRecord" HeaderText="InstelRecord" SortExpression="InstelRecord">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemStyle Wrap="False" />
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtViewFile" Text="View" CommandName="ViewFile" />
                                <asp:LinkButton runat="server" ID="lbtViewFileRecords" Text="Records" CommandName="ViewFileRecords" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsDividWepFiles" runat="server" SelectMethod="GetDividWepFiles"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Communicator.BelastingdienstAdapter">
                </asp:ObjectDataSource>
            </asp:TableCell></asp:TableRow>
    </asp:Table>
    <asp:MultiView ID="mlvFileView" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweFileDetails" runat="server">
            <asp:DetailsView ID="RecordView" DataSourceID="odsSingleRecord" runat="Server" AllowPaging="True"
                PagerSettings-Position="Top" RowStyle-VerticalAlign="Top" AutoGenerateRows="true"
                Visible="false">
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsSingleRecord" runat="server" SelectMethod="GetDividWepFile"
                TypeName="B4F.TotalGiro.ApplicationLayer.Communicator.BelastingdienstAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvDividWepFiles" Name="dividWepFileID" PropertyName="SelectedValue"
                        Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="vweFileRecords" runat="server">
        </asp:View>
    </asp:MultiView>
    <%--    <table width="1061px">
        <tr>
            <td style="width: 300px">
            </td>
            <td align="right">
                <asp:Panel ID="pnlActionButtons" runat="server">
&nbsp;
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height: 40px">

            </td>
        </tr>
    </table>--%>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="GLDSTDOverview.aspx.cs"
    Inherits="GLDSTDOverview" Title="Exported GLDSTD Overview" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <table border="0" cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td width="100%">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnlErrorMess" runat="server" Visible="false">
            <tr>
                <td width="100%" style="color: Red; height: 30px;">
                    <asp:Label ID="lblMess" Font-Bold="true" runat="server" />
                </td>
            </tr>
        </asp:Panel>
    </table>
    <table border="0">
        <tr>
            <td align="center">
                Show files from this date:<br />
                <asp:Calendar ID="calStart" runat="server" OnSelectionChanged="calStart_SelectionChanged">
                </asp:Calendar>
            </td>
            <td align="center">
                Until(incl.) this date:<br />
                <asp:Calendar ID="calEnd" runat="server" OnSelectionChanged="calEnd_SelectionChanged">
                </asp:Calendar>
            </td>
        </tr>
        <tr>
            <td align="center">
                Search Reference:&nbsp;
                <asp:TextBox ID="txtReference" runat="server" AutoPostBack="True"></asp:TextBox>
            </td>
            <%--            <td align="center">
                Search FsNumber:&nbsp;
                <asp:TextBox ID="txtFsNumber" runat="server" AutoPostBack="True" OnTextChanged="txtFsNumber_TextChanged"></asp:TextBox>
            </td>--%>
        </tr>
    </table>
    <asp:GridView ID="gvGLDSTDFileOverview" runat="server" AutoGenerateColumns="False"
        DataSourceID="odsGLDSTDFileOverview" CssClass="padding" AllowPaging="True" Caption="Export files"
        CaptionAlign="Left" AllowSorting="True" DataKeyNames="Key" PageSize="15" 
        onselectedindexchanged="gvGLDSTDFileOverview_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="FileID" SortExpression="Key" ReadOnly="True">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="FullFileName" HeaderText="FileName" SortExpression="FullFileName"
                ReadOnly="True">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <%--            <asp:TemplateField HeaderText="File Name" SortExpression="FullFileName">
                <ItemTemplate>
                    <a href="<%# Request.Url.ToString().Replace(Request.Url.Segments[Request.Url.Segments.Length-1], "DownloadFSFile.aspx?fileid=" + DataBinder.Eval(Container.DataItem, "Key").ToString()) %>">
                        <%# DataBinder.Eval(Container.DataItem, "FullFileName").ToString() + DataBinder.Eval(Container.DataItem, "Key").ToString()%></a>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>--%>
            <asp:BoundField DataField="Records_Count" HeaderText="NoOfRecords" SortExpression="Records_Count"
                ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate"
                ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <%--<asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="lbtViewOrders" Text="Orders" CommandName="ViewOrders"
                        Visible="true" />
                    <asp:LinkButton runat="server" ID="lbtTotal" Text="Total" CommandName="Total" Visible="true" />
                    <asp:LinkButton runat="server" ID="lbtDeleteFile" Text="Delete" CommandName="DeleteFile"
                        Visible="false" OnClientClick="return confirm('Delete file?')" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>--%>
            <asp:CommandField ShowSelectButton="True">
                <ItemStyle Wrap="False" Width="82px" />
            </asp:CommandField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsGLDSTDFileOverview" runat="server" SelectMethod="GetGLDSTDFileOverviewByCriteria"
        UpdateMethod="UpdateFSFileNumber" TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.GLDSTDExportOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="calStart" Name="p_datStartDate" PropertyName="SelectedDate"
                Type="DateTime" />
            <asp:ControlParameter ControlID="calEnd" Name="p_datEndDate" PropertyName="SelectedDate"
                Type="DateTime" />
            <asp:ControlParameter ControlID="txtReference" Name="p_strReference" PropertyName="Text"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:GridView ID="gvFileOverview" runat="server" AutoGenerateColumns="False" CssClass="padding"
        AllowPaging="True" PageSize="10" Caption="Records per export file" CaptionAlign="Left"
        AllowSorting="True" DataKeyNames="Key" DataSourceID="odsFileOverview" 
        onselectedindexchanged="gvFileOverview_SelectedIndexChanged" Visible="false">
        <Columns>
            <asp:BoundField DataField="Reference" HeaderText="Reference" SortExpression="Reference">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DebetAcctNr" HeaderText="DebetAcctNr" SortExpression="DebetAcctNr">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NarBenef1" HeaderText="NarBenef1" SortExpression="NarBenef1">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="GroundForPayment1" HeaderText="GroundForPayment1" SortExpression="GroundForPayment1">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:CommandField ShowSelectButton="True">
                <ItemStyle Wrap="False" Width="82px" />
            </asp:CommandField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFileOverview" runat="server" SelectMethod="GetGLDSTDRecordsByFile"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.GLDSTDExportOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvGLDSTDFileOverview" Name="theFileID" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:DetailsView ID="RecordView" DataSourceID="odsSingleRecord" runat="Server" AllowPaging="True"
        PagerSettings-Position="Top" RowStyle-VerticalAlign="Top" AutoGenerateRows="true" Visible="false">
       <%-- <Fields>
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            <asp:BoundField DataField="Reference" HeaderText="Reference" />
            <asp:BoundField DataField="CurrencyCode" HeaderText="CurrencyCode" />
            <asp:BoundField DataField="NarDebet1" HeaderText="NarDebet1" />
            <asp:BoundField DataField="NarDebet2" HeaderText="NarDebet2" />
            
            <asp:BoundField DataField="NarDebet3" HeaderText="NarDebet3" />
            <asp:BoundField DataField="NarDebet4" HeaderText="NarDebet4" />
            <asp:BoundField DataField="DebetAcctNr" HeaderText="DebetAcctNr" />
            <asp:BoundField DataField="NarCorrespondentBank1" HeaderText="NarCorrespondentBank1" />
            <asp:BoundField DataField="NarCorrespondentBank2" HeaderText="NarCorrespondentBank2" />
            
            <asp:BoundField DataField="NarCorrespondentBank3" HeaderText="NarCorrespondentBank3" />
            <asp:BoundField DataField="NarCorrespondentBank4" HeaderText="NarCorrespondentBank4" />
            <asp:BoundField DataField="SwiftCorrespondentBank" HeaderText="SwiftCorrespondentBank" />
            <asp:BoundField DataField="NarBenefBank1" HeaderText="NarBenefBank1" />
            <asp:BoundField DataField="NarBenefBank2" HeaderText="NarBenefBank2" />
            
            <asp:BoundField DataField="NarBenefBank3" HeaderText="NarBenefBank3" />
            <asp:BoundField DataField="NarBenefBank4" HeaderText="NarBenefBank4" />
            <asp:BoundField DataField="SwiftBenefBank" HeaderText="SwiftBenefBank" />
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
            <asp:BoundField DataField="PriorityCode" HeaderText="PriorityCode" />
        </Fields>--%>
    </asp:DetailsView>
    <asp:ObjectDataSource ID="odsSingleRecord" runat="server" SelectMethod="GetGLDSTDRecord"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.GLDSTDExportOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvFileOverview" Name="recordID" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

<%@ Page Title="End Year Management" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="EndYearManagement.aspx.cs" Inherits="EndYearManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:Table runat="server" Width="100%">
        <asp:TableRow runat="server" Width="100%">
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">            
            </asp:TableCell>
        </asp:TableRow>
        <%--    </asp:Table>
    <asp:Table runat="server" Caption="End Term Management" Width="100%">--%>
        <%--        <tr>
            <td style="width: 300px">
            </td>
            <td align="right">
                <asp:Panel ID="pnlActionButtons" runat="server">
                    <asp:Button ID="btnCloseCurrentYear" runat="server" Text="Close Current Year" CausesValidation="False"
                        Enabled="True" OnClick="btnCloseCurrentYear_Click" />&nbsp;
                </asp:Panel>
            </td>
        </tr>--%>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="1">
            </asp:TableCell>
            <asp:TableCell ColumnSpan="3" runat="server">
                <asp:Label runat="server" ID="lblLastEndTerm" Text="Last Period Stored:" />
            </asp:TableCell><asp:TableCell runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ColumnSpan="5" runat="server">
                <asp:TextBox runat="server" ID="txtLastEndTermDescription" ReadOnly="true" />
            </asp:TableCell><asp:TableCell runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell runat="server" ColumnSpan="3">
                <asp:Label runat="server" ID="lblEndTermDate" Text="End Term Date:" />
            </asp:TableCell><asp:TableCell ID="TableCell1" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell runat="server" ColumnSpan="5">
                <asp:TextBox runat="server" ID="txtEmdTermDate"  ReadOnly="true"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell ID="TableCell2" runat="server" ColumnSpan="1">
            </asp:TableCell>
            <asp:TableCell ID="TableCell3" ColumnSpan="3" runat="server">
                <asp:Label runat="server" ID="Label1" Text="Current Date:" />
            </asp:TableCell><asp:TableCell ID="TableCell4" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell5" ColumnSpan="5" runat="server">
                <asp:TextBox runat="server" ID="txtCurrentDate" ReadOnly="true" />
            </asp:TableCell><asp:TableCell ID="TableCell6" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell7" runat="server" ColumnSpan="3">
                <asp:Label runat="server" ID="Label2" Text="Next Possible Period:"  ReadOnly="true"/>
            </asp:TableCell><asp:TableCell ID="TableCell8" runat="server" ColumnSpan="1">
            </asp:TableCell><asp:TableCell ID="TableCell9" runat="server" ColumnSpan="5">
                <asp:TextBox runat="server" ID="txtNextPossiblePeriod" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell runat="server" ColumnSpan="5">
            </asp:TableCell>
            <asp:TableCell ColumnSpan="5" runat="server">
                <asp:Button ID="btnCloseCurrentYear" runat="server" Text="Close Current Year" CausesValidation="False"
                    Enabled="True" OnClick="btnCloseCurrentYear_Click" />
            </asp:TableCell>
            <asp:TableCell runat="server" ColumnSpan="1"></asp:TableCell>
            <asp:TableCell runat="server" ColumnSpan="9">
                <asp:Label runat="server" ID="lblInformation" Text="Next Possible Period:" />
            </asp:TableCell>        
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table runat="server" ID="Table2" Width="100%">
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell21" runat="server" ColumnSpan="20">
                <asp:GridView ID="gvPeriodicReporting" runat="server" AutoGenerateColumns="False"
                    PageSize="25" AllowPaging="True" Caption="Reported Periods" CaptionAlign="Left"
                    AllowSorting="True" DataSourceID="odsPeriodicReporting" SkinID="spreadsheet"
                    DataKeyNames="Key" Visible="True" Width="100%">
                    <Columns>
                        <asp:TemplateField HeaderText="EndTermYear" SortExpression="EndTermYear">
                            <ItemTemplate>
                                <asp:Label ID="lblLineNumber" runat="server" Width="15px">
                    <%# DataBinder.Eval(Container.DataItem, "EndTermYear")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" HorizontalAlign="Center" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description" SortExpression="Description">
                            <ItemTemplate>
                                <asp:Label ID="lblAccount" runat="server" Width="200px" CssClass="padding">
                                <%# DataBinder.Eval(Container.DataItem, "Description")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="EndDate" SortExpression="EndDate">
                            <ItemTemplate>
                                <asp:Label ID="lblAccountShortName" runat="server" Width="200px" CssClass="padding">
                                <%# DataBinder.Eval(Container.DataItem, "EndDate")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CreatedOn" SortExpression="CreatedOn">
                            <ItemTemplate>
                                <asp:Label ID="lblDebit" runat="server" Width="150px">
                    <%# DataBinder.Eval(Container.DataItem, "CreatedOn")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" HorizontalAlign="Center" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CreatedBy" SortExpression="CreatedBy">
                            <ItemTemplate>
                                <asp:Label ID="lblCredit" runat="server" Width="150px">
                    <%# DataBinder.Eval(Container.DataItem, "CreatedBy")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsPeriodicReporting" runat="server" SelectMethod="GetPeriodicReporting"
                    TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.EndYearManagementAdapter">
                </asp:ObjectDataSource>
            </asp:TableCell></asp:TableRow>
    </asp:Table>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label></asp:Content>

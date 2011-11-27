<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Reports.FinancialReports" Codebehind="FinancialReports.aspx.cs" %>

<%@ Register Src="~/UC/PortfolioNavigationBar.ascx" TagName="PortfolioNavigationBar" TagPrefix="b4f" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <b4f:PortfolioNavigationBar ID="ctlPortfolioNavigationBar" runat="server" />
    
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="lblAccountLabel" runat="server" Text="Rekening:" Width="80px"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" 
                    Width="300px" DataSourceID="odsAccount" 
                    DataTextField="DisplayNumberWithName" DataValueField="Key" 
                    AutoPostBack="True" onselectedindexchanged="ddlAccount_SelectedIndexChanged" >
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsAccount" runat="server" SelectMethod="GetContactAccounts"
                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Common.CommonAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="contactId" SessionField="ContactId" Type="Int32" DefaultValue="0" /> 
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr style="height: 15px" >
            <td style="width: 155px"></td>
            <td style="width: 360px"></td>
        </tr>
    </table>
    
    <asp:GridView ID="gvFinancialReportDocuments" runat="server" AllowPaging="True" AllowSorting="True" 
                  DataSourceID="odsFinancialReportDocuments" DataKeyNames="Key" SkinID="custom-width" Width="550px"
                  Caption="Rapportages" CaptionAlign="Top" PageSize="30" AutoGenerateColumns="False">
        <Columns>
            <asp:TemplateField HeaderText="Rapportage" SortExpression="YearAndType">
                <ItemTemplate>
                    <b4f:ArrowsLinkButton ID="lkbYearAndType" runat="server"
                        PostBackUrl='<%# Eval("Key", "~/Reports/DocViewer.aspx?id={0}#toolbar=1") %>' 
                        Text='<%# Eval("YearAndType") %>'>
                    </b4f:ArrowsLinkButton>
                </ItemTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="CreationDate" HeaderText="Datum" SortExpression="CreationDate"
                DataFormatString="{0:dd-MM-yyyy}" >
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Concern" HeaderText="Betreft" SortExpression="Concern">
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:HyperLinkField Text="View" DataNavigateUrlFields="Key" 
                DataNavigateUrlFormatString="~/Reports/DocViewer.aspx?id={0}#toolbar=1">
                <ItemStyle Wrap="False" />
            </asp:HyperLinkField>
            <asp:HyperLinkField Text="Download" DataNavigateUrlFields="Key" 
                DataNavigateUrlFormatString="~/Reports/DocViewer.aspx?id={0}&download=true">
                <ItemStyle Wrap="False" />
            </asp:HyperLinkField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFinancialReportDocuments" runat="server" SelectMethod="GetFinancialReportDocuments"
        TypeName="B4F.TotalGiro.ClientApplicationLayer.Reports.FinancialReportsAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAccount" Name="accountId" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


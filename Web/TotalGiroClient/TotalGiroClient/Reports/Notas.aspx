<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Reports.Notas" Codebehind="Notas.aspx.cs" %>

<%@ Register Src="~/UC/PortfolioNavigationBar.ascx" TagName="PortfolioNavigationBar" TagPrefix="b4f" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <b4f:PortfolioNavigationBar ID="ctlPortfolioNavigationBar" runat="server" />
    
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="lblAccount" runat="server" Text="Rekening:" Width="80px"></asp:Label>
            </td>
            <td colspan="3" style="white-space: nowrap">
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
            <td style="white-space: nowrap">
                <%--<asp:Button ID="btnSearch" runat="server" Text="Search" Width="98px" />--%>
            </td>
        </tr>
        <%--<tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="lblDateFrom" runat="server" Text="Date From:" Width="80px"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:TextBox ID="txtDateFrom" runat="server" SkinID="custom-width" Width="80px"></asp:TextBox>
            </td>
            <td style="white-space: nowrap">
                <asp:Label ID="lblDateTo" runat="server" Text="Date To:" Width="60px"></asp:Label>
            </td>
            <td colspan="2" style="white-space: nowrap">
                <asp:TextBox ID="txtDateTo" runat="server" SkinID="custom-width" Width="80px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="lblNotaType" runat="server" Text="Type:" Width="80px"></asp:Label>
            </td>
            <td colspan="3" style="white-space: nowrap">
                <asp:DropDownList ID="ddlNotaTypes" SkinID="custom-width" runat="server" Width="200px" >
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>--%>
        <tr style="height: 15px">
            <td style="width: 155px"></td>
            <td style="width: 120px"></td>
            <td style="width: 85px"></td>
            <td style="width: 200px"></td>
            <td style="width: 120px"></td>
        </tr>
    </table>
    
    <asp:GridView ID="gvNotaDocuments" runat="server" AllowPaging="True" AllowSorting="True" 
                  DataSourceID="odsNotaDocuments" DataKeyNames="Key" SkinID="custom-width" Width="550px"
                  Caption="Afschriften" CaptionAlign="Top" PageSize="30" AutoGenerateColumns="False">
        <Columns>
            <asp:TemplateField HeaderText="Afschriftnummer" SortExpression="FirstNotaNumber">
                <ItemTemplate>
                    <b4f:ArrowsLinkButton ID="lkbNotaNumber" runat="server"
                        PostBackUrl='<%# Eval("Key", "~/Reports/DocViewer.aspx?id={0}#toolbar=1") %>' 
                        Text='<%# Eval("FirstNotaNumber") %>'>
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
            <asp:BoundField DataField="FirstNotaTitle" HeaderText="Soort afschrift" SortExpression="FirstNotaTitle">
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NotaCount" HeaderText="Inhoud" SortExpression="NotaCount">
                <HeaderStyle HorizontalAlign="Right" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
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
    <asp:ObjectDataSource ID="odsNotaDocuments" runat="server" SelectMethod="GetNotaDocuments"
        TypeName="B4F.TotalGiro.ClientApplicationLayer.Reports.NotasAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAccount" Name="accountId" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


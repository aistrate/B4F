<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" Inherits="Test" CodeFile="Test.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    
    <asp:GridView ID="gvResults" runat="server" DataSourceID="odsResults" AutoGenerateColumns="True" DataKeyNames="Key" 
                  AllowPaging="False" AllowSorting="True" Caption="Results" CaptionAlign="Top" PageSize="20">
        <Columns>
        </Columns>
    </asp:GridView>
    
    <asp:ObjectDataSource ID="odsResults" runat="server" SelectMethod="GetResults"
                          TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.TestAdapter">
        <SelectParameters>
        </SelectParameters>
    </asp:ObjectDataSource>
    
</asp:Content>

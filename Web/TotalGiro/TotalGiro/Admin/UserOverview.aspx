<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="UserOverview.aspx.cs" Inherits="UserOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:GridView ID="gvUsers" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsUsers" PageSize="20" Caption="Users" CaptionAlign="Left" DataKeyNames="UserName" OnRowCommand="gvUsers_RowCommand">
        <Columns>
            <asp:CheckBoxField DataField="IsApproved" HeaderText="Active" SortExpression="IsApproved">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:CheckBoxField>
            <asp:BoundField DataField="UserName" HeaderText="Name" SortExpression="UserName">
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Email" HeaderText="E-mail" SortExpression="Email">
                <ItemStyle Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Comment" HeaderText="Description" SortExpression="Comment">
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Creation Date" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtEdit" 
                        Text="Edit" 
                        CommandName="EditUser"/>&nbsp;
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDelete" 
                        Text="Delete"
                        CommandName="DeleteUser"
                        OnClientClick="return confirm('Delete user?')"/>&nbsp;
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtResetPassword" 
                        Text="Reset password"
                        CommandName="ResetPassword"
                        Visible='<%# Eval("IsApproved") %>'
                        OnClientClick="return confirm('Reset user password?')"/>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsUsers" runat="server" SelectMethod="GetUsers" 
        TypeName="B4F.TotalGiro.ApplicationLayer.Admin.UserOverviewAdapter">
    </asp:ObjectDataSource>
    
    <br />
    <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="Add new" />
    <br />
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px" Font-Bold="false"></b4f:ErrorLabel>
    
</asp:Content>

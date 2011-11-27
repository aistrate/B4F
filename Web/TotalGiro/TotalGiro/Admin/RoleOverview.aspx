<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="RoleOverview.aspx.cs" Inherits="RoleOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:GridView ID="gvRoles" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" Caption="Roles" CaptionAlign="Left" DataSourceID="odsRoles"
        PageSize="30" OnRowCommand="gvRoles_RowCommand" DataKeyNames="RoleName">
        <Columns>
            <asp:BoundField DataField="RoleName" HeaderText="Role name" SortExpression="RoleName">
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="NumberOfUsers" HeaderText="Number of users" SortExpression="NumberOfUsers">
                <ItemStyle CssClass="bigrightpadding" HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtUsers" 
                        Text="Users"
                        CommandName="EditRole"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDelete" 
                        Text="Delete"
                        CommandName="DeleteRole"
                        OnClientClick="return confirm('Delete role?')"/>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsRoles" runat="server" SelectMethod="GetRoles" 
        TypeName="B4F.TotalGiro.ApplicationLayer.Admin.RoleOverviewAdapter">
    </asp:ObjectDataSource>
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px" Font-Bold="true"></b4f:ErrorLabel>
    <br />
    <asp:MultiView ID="mvwEditRole" runat="server">
        <asp:View ID="vwNoEdit" runat="server">
            <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="Add new" /><br />
        </asp:View>
        <asp:View ID="vwAdd" runat="server">
            <asp:Label ID="Label1" runat="server" Text="Role Name"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtRoleName" runat="server"></asp:TextBox>&nbsp;
            <asp:RequiredFieldValidator ID="reqRoleName" runat="server" ControlToValidate="txtRoleName" 
                    SetFocusOnError="True">*</asp:RequiredFieldValidator><br />
            <br />
            <asp:Button ID="btnSaveAdd" runat="server" OnClick="btnSaveAdd_Click" Text="Save" />&nbsp;
            <asp:Button ID="btnCancelAdd" runat="server" OnClick="btnCancel_Click" Text="Cancel" CausesValidation="False" /><br />
        </asp:View>
        <asp:View ID="vwModify" runat="server">
            <asp:GridView ID="gvUsersInRole" runat="server" AllowPaging="True" 
                AllowSorting="True" AutoGenerateColumns="False" Caption="Users In Role" 
                          DataSourceID="odsUsersInRole" PageSize="25" CaptionAlign="Left" DataKeyNames="UserName" 
                          OnPageIndexChanging="gvUsersInRole_PageIndexChanging" OnSorting="gvUsersInRole_Sorting" 
                          onrowcommand="gvUsersInRole_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Is in role" SortExpression="IsInRole">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbIsInRole" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsInRole") %>' />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" CssClass="aligncenter" Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="UserName" HeaderText="User name" SortExpression="UserName">
                        <HeaderStyle Wrap="False" />
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsUsersInRole" runat="server" SelectMethod="GetUsersInRole"
                TypeName="B4F.TotalGiro.ApplicationLayer.Admin.RoleOverviewAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvRoles" ConvertEmptyStringToNull="False" Name="roleName"
                        PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
            <asp:Button ID="btnSaveModify" runat="server" Text="Save" OnClick="btnSaveModify_Click" />&nbsp;
            <asp:Button ID="btnCancelModify" runat="server" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="False" /><br />
        </asp:View>
    </asp:MultiView>
    <br />
</asp:Content>

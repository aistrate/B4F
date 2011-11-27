<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/EG.master" CodeFile="RemisierOverview.aspx.cs"
    Inherits="RemisierOverview" Theme="Neutral" Title="Securities Overview" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.Stichting.Remisier" %>
<%@ Import Namespace="B4F.TotalGiro.StaticData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hfAssetMAnagerID" runat="server" />
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblAssetManagerLabel" runat="server" Text="Asset Manager:"></asp:Label>
            </td>
            <td style="width: 190px">
                <asp:MultiView ID="mvwAssetManager" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwAssetManager" runat="server">
                        <asp:Label ID="lblAssetManager" runat="server" Font-Bold="True"></asp:Label></asp:View>
                    <asp:View ID="vwStichting" runat="server">
                        <asp:DropDownList ID="ddlAssetManager" runat="server" Width="165px" DataSourceID="odsAssetManager"
                            DataTextField="CompanyName" DataValueField="Key" >
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                            TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter"></asp:ObjectDataSource>
                    </asp:View>
                </asp:MultiView>
            </td>
        
        </tr>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblRemisierName" runat="server" Text="Remisier Name:"></asp:Label></td>
            <td style="width: 190px">
                <asp:TextBox ID="txtRemisierName" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="lblRemisierActive" runat="server" Text="Status:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlRemisierActive" runat="server" DataSourceID="odsRemisierActive"
                    DataTextField="Status" DataValueField="ID" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsRemisierActive" runat="server" SelectMethod="GetAccountStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                   CausesValidation="False" Width="90px" />
            </td>
        </tr>
    </table>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red" />
    <br />
    <br />
    <asp:GridView ID="gvRemisiers" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsRemisiers" 
        DataKeyNames="Key" Caption="Remisiers"
        CaptionAlign="Left" PageSize="20" 
        Visible="false"
        OnRowCommand="gvRemisiers_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="InternalRef" HeaderText="Ref" SortExpression="InternalRef" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Type" SortExpression="RemisierType">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlRemisierType" 
                        runat="server"
                        cssclass="alignright"
                        Width="8"
                        Text='<%# (RemisierTypes)DataBinder.Eval(Container.DataItem, "RemisierType")%>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Address" SortExpression="OfficeAddress_DisplayAddress">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlAddress" 
                        runat="server"
                        cssclass="alignright"
                        Width="25"
                        Text='<%# DataBinder.Eval(Container.DataItem, "OfficeAddress_DisplayAddress") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Telephone" HeaderText="Telephone" SortExpression="Telephone" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <%--<asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>--%>
            <asp:TemplateField HeaderText="Contact" SortExpression="ContactPerson_FullName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlContactPerson" 
                        runat="server"
                        cssclass="alignright"
                        Width="15"
                        Text='<%# DataBinder.Eval(Container.DataItem, "ContactPerson_FullName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NummerAFM" HeaderText="Nummer AFM" SortExpression="NummerAFM" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <%--<asp:CheckBoxField HeaderText="Internal" SortExpression="IsInternal" DataField="IsInternal" >
                <HeaderStyle wrap="False" />
            </asp:CheckBoxField>--%>
            <asp:CheckBoxField HeaderText="Active" SortExpression="IsActive" DataField="IsActive" >
                <HeaderStyle wrap="False" />
            </asp:CheckBoxField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnEditRemisier" runat="server" 
                        CommandName="EditRemisier" 
                        Text="Edit" />
                    <asp:LinkButton
                         ID="lbtnDeleteRemisier"
                         runat="server"
                         CausesValidation="false"
                         CommandName="DeleteRemisier"
                         Text="Deactive" 
                         Visible='<%# DataBinder.Eval(Container.DataItem, "IsActive") %>' 
                         OnClientClick="return confirm('Are you sure you want to deactivate this remisier?')" />
                    <asp:LinkButton ID="lbtnEmployees" runat="server" 
                        CommandName="ViewEmployees" 
                        Text="Employees" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsRemisiers" runat="server" SelectMethod="GetRemisiers"
        TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAssetManager" Name="assetManagerId" PropertyName="SelectedValue"
                Type="Int32" DefaultValue="0" />
            <asp:ControlParameter ControlID="txtRemisierName" Name="remisierName" PropertyName="Text"
                Type="String" />
            <asp:ControlParameter ControlID="ddlRemisierActive" Name="activeStatus" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:Parameter DefaultValue="Key, Name, InternalRef, RemisierType, IsInternal, IsActive, OfficeAddress.DisplayAddress, Telephone, Email, ContactPerson.FullName, NummerAFM" Name="propertyList" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Button ID="btnCreate" runat="server" Text="Create Remisier" OnClick="btnCreate_Click" />
    <br />
    <br />
    <asp:GridView
        ID="gvEmployees"
        runat="server"
        AutoGenerateColumns="false"
        Visible="false"
        AllowPaging="True" AllowSorting="True"
        Caption="Remisier Employees"
        CaptionAlign="Left" PageSize="20" 
        SkinID="custom-width"
        Width="900px"
        DataSourceID="odsEmployees"
        DataKeyNames="Key"
        OnRowCommand="gvEmployees_RowCommand" >
        <Columns>
            <asp:BoundField
                 DataField="Employee_FullName"
                 HeaderText="Name"
                 SortExpression="Employee_LastName" />
            <asp:TemplateField HeaderText="Gender" SortExpression="Employee_Gender">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <asp:Label ID="lblGender" runat="server">
                        <%#((Gender)DataBinder.Eval(Container.DataItem, "Employee_Gender") == Gender.Male ? "M" : ((Gender)DataBinder.Eval(Container.DataItem, "Employee_Gender") == Gender.Female ? "F" : ""))%>
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Role" SortExpression="Role">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <asp:Label ID="lblRole" runat="server">
                        <%# (EmployeeRoles)DataBinder.Eval(Container.DataItem, "Role") %>
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField
                 DataField="Employee_Telephone_Number"
                 HeaderText="Telephone"
                 SortExpression="Employee_Telephone_Number" />
            <asp:BoundField
                 DataField="Employee_Mobile_Number"
                 HeaderText="Mobile"
                 SortExpression="Employee_Mobile_Number" />
            <asp:TemplateField HeaderText="Email" SortExpression="Employee_Email">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlEMail" 
                        runat="server"
                        cssclass="alignright"
                        Width="28"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Employee_Email") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField
                 DataField="IsActive"
                 HeaderText="Active"
                 SortExpression="IsActive" />
           <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnEditEmployee" runat="server" 
                        CommandName="EditEmployee" 
                        Text="Edit" />
                    <asp:LinkButton
                         ID="lbtnDeleteEmployee"
                         runat="server"
                         CausesValidation="false"
                         CommandName="DeleteEmployee"
                         Text="Delete"
                         Visible='<%# DataBinder.Eval(Container.DataItem, "IsActive") %>' 
                         OnClientClick="return confirm('Are you sure you want to delete this employee?')" />
                </ItemTemplate>
            </asp:TemplateField>
         </Columns>
         <EmptyDataTemplate>
            &nbsp;
         </EmptyDataTemplate>
    </asp:GridView>
    <asp:ObjectDataSource 
        ID="odsEmployees" 
        runat="server" 
        SelectMethod="GetEmployees" 
        TypeName="B4F.TotalGiro.ApplicationLayer.Remisers.RemisierAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvRemisiers" Name="remisierID" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlRemisierActive" Name="activeStatus" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Button ID="btnCreateEmployee" runat="server" Text="Create Employee" Visible="false" OnClick="btnCreateEmployee_Click" />
    <asp:Button ID="btnHideEmployees" runat="server" Text="Hide" Visible="false" OnClick="btnHideEmployees_Click" Width="60px" />
</asp:Content>

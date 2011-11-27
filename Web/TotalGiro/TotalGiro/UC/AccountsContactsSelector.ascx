<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountsContactsSelector.ascx.cs" Inherits="AccountsContactsSelector" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.ApplicationLayer.UC" %>
<table cellpadding="1" cellspacing="1" border="0" >
    <tr>
        <td colspan="3" >
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
        </td>
    </tr>

    <tr>
        <td >
            <asp:RadioButtonList ID="rblSelectorType" runat="server" RepeatDirection="Horizontal" 
                OnSelectedIndexChanged="rblSelectorType_SelectedIndexChanged" AutoPostBack="true" >
                <asp:ListItem Value="0" Text="Contacts" Selected="True" />
                <asp:ListItem Value="1" Text="Accounts" />
            </asp:RadioButtonList>
        </td>
        <td colspan="2" />
    </tr>

    <tr>
        <td >
            <asp:Label ID="lblFilter" runat="server" Text="Filter" />
            <asp:TextBox ID="txtFilter" runat="server" />
            <asp:Button ID="btnFilter" runat="server" Text="..." OnClick="btnFilter_Click" CausesValidation="false" />
        </td>
        <td colspan="2" />
    </tr>


    <tr>
        <td style="width: 450px">
            <asp:MultiView ID="mvwSelection" runat="server" ActiveViewIndex="0" EnableTheming="True">
                <asp:View ID="vwSelectedContacts" runat="server">
                    <asp:GridView ID="gvContacts" runat="server"
                    CellPadding="0"
                    DataSourceID="odsContacts" 
                    AllowPaging="true" PageSize="5"
                    AllowSorting="true"
                    AutoGenerateColumns="False"
                    Caption="Select Contacts"
                    CaptionAlign="Left"
                    DataKeyNames="Key"
                    SkinID="custom-EmptyDataTemplate" 
                    Width="450px"
                    OnSelectedIndexChanging="gvExclusion_SelectedIndexChanging" >
                    <EmptyDataTemplate>
                        <table cellpadding="0" cellspacing="0" border="0" width="450px">
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                        </table>
                    </EmptyDataTemplate>                    
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BSN" HeaderText="BSN/KvK" SortExpression="BSN">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:CommandField ShowSelectButton="true" />
                    </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsContacts" runat="server" SelectMethod="GetContacts" OnSelecting="odsContacts_Selecting"
                        TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountsContactsSelectorAdapter" >
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtFilter" Name="filter" PropertyName="Text" Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:View>
                <asp:View ID="vwSelectedAccounts" runat="server">
                    <asp:GridView ID="gvAccounts" runat="server"
                    CellPadding="0"
                    DataSourceID="odsAccounts" 
                    AllowPaging="true" PageSize="5"
                    AllowSorting="true"
                    AutoGenerateColumns="False"
                    Caption="Select Accounts"
                    CaptionAlign="Left"
                    DataKeyNames="Key"
                    SkinID="custom-EmptyDataTemplate" 
                    Width="450px"
                    OnSelectedIndexChanging="gvExclusion_SelectedIndexChanging" >
                    <EmptyDataTemplate>
                        <table cellpadding="0" cellspacing="0" border="0" width="450px">
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                        </table>
                    </EmptyDataTemplate>                    
                    <Columns>
                        <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ShortName" HeaderText="ShortName" SortExpression="ShortName">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:CommandField ShowSelectButton="true" />
                    </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetAccounts" OnSelecting="odsAccounts_Selecting"
                        TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountsContactsSelectorAdapter" >
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtFilter" Name="filter" PropertyName="Text" Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:View>
            </asp:MultiView>
        </td>
        <td style="width: 40px"> 
            <asp:Label ID="lblMoveTo" runat="server" Text="<->" />
        </td>
        <td style="width: 380px; vertical-align: top">
            <asp:GridView ID="gvSelection" runat="server"
            CellPadding="0"
            AutoGenerateColumns="False"
            AllowPaging="true" PageSize="5"
            AllowSorting="true"
            Caption="Selection"
            CaptionAlign="Left"
            SkinID="custom-EmptyDataTemplate" 
            Width="380px"
            DataKeyNames="Key"
            OnRowDeleting="gvSelection_RowDeleting"
            OnSorting="gvSelection_Sorting"
            OnPageIndexChanging="gvSelection_PageIndexChanging" >
            <EmptyDataTemplate>
                <table cellpadding="0" cellspacing="0" border="0" width="350px">
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                </table>
            </EmptyDataTemplate>                    
            <Columns>
                <asp:TemplateField HeaderText="Name" SortExpression="SelectedType">
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <asp:Label ID="lblType"
                            runat="server"
                            Width="38"
                            Text='<%# (AccountContactSelectedTypes)DataBinder.Eval(Container.DataItem, "SelectedType") %>' 
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="Description">
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <trunc:TruncLabel ID="lblMessage"
                            runat="server"
                            Width="38"
                            Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>' 
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowDeleteButton="true" />
            </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>


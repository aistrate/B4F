<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Accounts.ascx.cs" Inherits="UC_Accounts" %>
<asp:GridView 
    ID="gvAccounts"
    DataKeyNames="AccountKey,ContactKey"
    SkinID="custom-width" 
    AutoGenerateColumns="False" 
    AllowPaging="True"
    PageSize="5" 
    AllowSorting="True"
    runat="server"
    OnRowDataBound="gvAccounts_RowDataBound"
    DataSourceID="odsContactAccounts" 
    OnRowCommand="gvAccounts_RowCommand">
    <Columns>
        <asp:BoundField HeaderText="Number" DataField="Number" SortExpression="Number" />
        <asp:BoundField HeaderText="Account ShortName" DataField="ShortName" SortExpression="ShortName" />
        <asp:TemplateField HeaderText="Primary AccountHolder">
            <ItemTemplate>
               <asp:Label ID="lblPrimaryAH" runat="server" Text='<%# Eval("PrimaryAhName") %>' />
            </ItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton
                     ID="lbtnEditAccount"
                     runat="server"
                     CausesValidation="false"
                     ToolTip="View Account data"
                     CommandName="Edit"
                     Text="Edit Account" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton
                     ID="lbtnDetach"
                     Enabled="false"
                     runat="server"
                     CausesValidation="false"
                     ToolTip="Detaches this contact as accountholder and will randomly make another accountholder, if present, the primary accountholder."
                     CommandName="Detach"
                     Text="Detach" />
            </ItemTemplate>
        </asp:TemplateField>
     </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsContactAccounts" runat="server" SelectMethod="GetAccountInfo"
        TypeName="B4F.TotalGiro.ApplicationLayer.UC.ucAccountsEditAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="contactID" SessionField="contactid" Type="Int32" />
        </SelectParameters>
</asp:ObjectDataSource>
<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" CodeFile="ContactOverview.aspx.cs" Inherits="DataMaintenance_ContactOverview" Title="Contact Overview" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table>
        <tr>
            <td>
                <uc1:AccountFinder  
                    ID="ctlAccountFinder"
                    ShowContactActiveCbl="true"
                    AccountNameLabel="Contact Name:"
                    ShowBsN_KvK="true"
                    runat="server"  />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlContacts" runat="server" Visible="false">
                    <asp:GridView 
                        ID="gvContactOverview"
                        DataSourceID="odsContactOverview"
                        PageSize="25"
                        EnableViewState="false"
                        AutoGenerateColumns="false"
                        AllowSorting="true"
                        AllowPaging="true"
                        Caption="Contacts"
                        CaptionAlign="Left"
                        OnRowCommand="gvContactOverview_RowCommand"
                        DataKeyNames="Key"
                        runat="server" ondatabinding="gvContactOverview_DataBinding">
                            <Columns>
                                <asp:TemplateField HeaderText="Contact Name" SortExpression="CurrentNAW_Name">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                    <ItemTemplate>
                                        <trunc:TruncLabel ID="lblName" 
                                            runat="server"
                                            Width="30"
                                            Text='<%# DataBinder.Eval(Container.DataItem, "FullName")%>'
                                        />
                                   </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField ReadOnly="true" 
                                            DataField="GetBSN" 
                                            HeaderText="BSN / KVK" 
                                            SortExpression="GetBSN">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Birth/Founding" SortExpression="GetBirthFounding">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                        <ItemTemplate>
                                            <trunc:TruncLabel ID="lblBirthFoundingDate"
                                                runat="server"
                                                Width="35"
                                                Text='<%# (DataBinder.Eval(Container.DataItem, "GetBirthFounding") == System.DBNull.Value || (DateTime)DataBinder.Eval(Container.DataItem, "GetBirthFounding") == DateTime.MinValue ? "" : Eval("GetBirthFounding", "{0:dd MMM yyyy}")) %>' 
                                                />
                                        </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Address" 
                                            SortExpression="CurrentNAW_ResidentialAddress_DisplayAddress">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                    <ItemTemplate>
                                        <trunc:TruncLabel ID="lblStreet" 
                                            runat="server"
                                            Width="40"
                                            Text='<%# DataBinder.Eval(Container.DataItem, "CurrentNAW_ResidentialAddress_DisplayAddress")%>'
                                        />
                                   </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Type" SortExpression="ContactType">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                    <ItemTemplate>
                                           <%# (B4F.TotalGiro.CRM.ContactTypeEnum)DataBinder.Eval(Container.DataItem, "ContactType")%>
                                   </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Active" SortExpression="IsActive">
                                    <ItemStyle Wrap="False" Width="50px" />
                                    <HeaderStyle Wrap="False" />
                                    <ItemTemplate>
                                        <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive")) == true ? "Active" : "Inactive"%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status NAR" SortExpression="StatusNAR">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                    <ItemTemplate>
                                           <%# (B4F.TotalGiro.CRM.EnumStatusNAR)DataBinder.Eval(Container.DataItem, "StatusNAR")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                    <ItemTemplate>
                                        <asp:LinkButton
                                             ID="lbtnEdit"
                                             runat="server"
                                             CommandName="Edit"
                                             Text='<%# (UserHasEditRights ? "Edit" : "View") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                    </asp:GridView>       
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlButtons" runat="server" Width="600px" Visible="false">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="5">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnAddPerson" runat="server" Text="Create Person" OnClick="btnAddPerson_Click" />&nbsp;
                            </td>
                            <td>&nbsp;</td>
                            <td>
                                <asp:Button ID="btnAddCompany" runat="server" Text="Create Company" OnClick="btnAddCompany_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
                        
    <asp:ObjectDataSource ID="odsContactOverview" runat="server" SelectMethod="GetContacts" 
        UpdateMethod="UpdateStatusContact"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ContactOverviewAdapter" 
        OnUpdating="odsContactOverview_Updating">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerID" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="bsN_KvK" PropertyName="BsN_KvK"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactActive" PropertyName="ContactActive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactInactive" PropertyName="ContactInactive"
                Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
</asp:Content>


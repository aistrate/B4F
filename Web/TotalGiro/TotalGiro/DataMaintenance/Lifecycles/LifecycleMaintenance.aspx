<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="LifecycleMaintenance.aspx.cs"
    Inherits="LifecycleMaintenance" Title="Lifecycle Maintenance" %>

<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hdnIsLifecycleInsert" runat="server" Value="false" />
    <asp:HiddenField ID="hdnIsLifecycleLineInsert" runat="server" Value="false" />
    <asp:ScriptManagerProxy ID="cmg" runat="server" />
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="Label1" runat="server" Text="Lifecycle Name:"></asp:Label></td>
            <td style="width: 190px">
                <asp:TextBox ID="txtLifecycleName" runat="server"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="lblLifecycleActive" runat="server" Text="Status:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlLifecycleActive" runat="server" DataSourceID="odsLifecycleActive"
                    DataTextField="Status" DataValueField="ID" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsLifecycleActive" runat="server" SelectMethod="GetAccountStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                   CausesValidation="False" Width="90px" />
            </td>
        </tr>
    </table>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
    <table cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td colspan="20">
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" ValidationGroup="UpdateModel" />
            </td>
        </tr>
    </table>
    <table width="1000px" cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="20">
                <asp:GridView ID="gvLifecycles" runat="server" AutoGenerateColumns="false" Caption="Lifecycles"
                    CaptionAlign="Top" PageSize="10" DataSourceID="odsLifecycles" AllowPaging="True"
                    DataKeyNames="Key" AllowSorting="True" 
                    SkinID="custom-EmptyDataTemplate" Width="100%"
                    OnRowCommand="gvLifecycles_RowCommand"
                    OnRowDataBound="gvLifecycles_RowDataBound"
                    OnRowUpdating="gvLifecycles_RowUpdating"
                    OnRowUpdated="gvLifecycles_RowUpdated" >
                    <SelectedRowStyle BackColor="Gainsboro" />
                    <Columns>
                        <asp:BoundField HeaderText="Lifecycle Name" DataField="Name" SortExpression="Name">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Created" SortExpression="LatestVersion_LatestVersionDate">
                            <ItemTemplate>
                                <%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate") != DateTime.MinValue ?
                                    ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("d MMMM yyyy") : "")%>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Creator" DataField="CreatedBy" SortExpression="CreatedBy" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Last Updated" SortExpression="LastUpdated">
                            <ItemTemplate>
                                <%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastUpdated") != DateTime.MinValue ?
                                    ((DateTime)DataBinder.Eval(Container.DataItem, "LastUpdated")).ToString("d MMMM yyyy") : "")%>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Active" SortExpression="IsActive">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkIsActive" runat="server"
                                    Checked='<%# DataBinder.Eval(Container.DataItem, "IsActive") %>' 
                                    Enabled="false"
                                    />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton 
                                    ID="lbtViewLines" 
                                    runat="server" 
                                    CausesValidation="False" 
                                    Text="View Lines"
                                    CommandName="ViewLines"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                    OnCommand="lbtViewLines_Command" />
                                <asp:LinkButton 
                                    ID="lbtEditCycle" 
                                    runat="server" 
                                    CausesValidation="False" 
                                    Text="Edit"
                                    CommandName="EditCycle"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                    OnCommand="lbtEditCycle_Command" />
                                <asp:LinkButton 
                                    ID="lbtDeleteCycle" 
                                    runat="server" 
                                    CausesValidation="False" 
                                    Text="Delete"
                                    CommandName="DeleteCycle"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                    OnCommand="lbtDeleteCycle_Command"
                                    OnClientClick="return confirm('Delete line?')" />
                                <asp:LinkButton 
                                    ID="lbtUpdateCycle" 
                                    runat="server" 
                                    CausesValidation="True" 
                                    Text='<%# ((int)DataBinder.Eval(Container.DataItem, "Key") != 0 ? "Update" : "Insert") %>'
                                    CommandName="Update"
                                    Visible="False" />
                                <asp:LinkButton 
                                    ID="lbtCancelCycle" 
                                    runat="server" 
                                    CausesValidation="False" 
                                    Text="Cancel"
                                    CommandName="Cancel"
                                    Visible="False" />
                                <asp:LinkButton 
                                    runat="server" 
                                    ID="lbtDeActivateCycle" 
                                    Text="DeActivate" 
                                    CommandName="DeActivateCycle"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                                    Visible='<%# DataBinder.Eval(Container.DataItem, "IsActive") %>'
                                    OnCommand="lbtDeActivateCycle_Command" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <asp:Label ID="lblEmptyLifecycles" runat="server" Text="No Lifecycles found" />
                    </EmptyDataTemplate>
                </asp:GridView>

                <asp:ObjectDataSource ID="odsLifecycles" runat="server" SelectMethod="GetLifecycles" 
                    UpdateMethod="UpdateLifecycle" OldValuesParameterFormatString="original_{0}"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.LifecycleMaintenanceAdapter"
                    onupdated="odsLifecycles_Updated" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtLifecycleName" Name="LifecycleName" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ddlLifecycleActive" Name="activeStatus" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="hdnIsLifecycleInsert" DefaultValue="False" Name="isInsert"
                            PropertyName="Value" Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:Button ID="btnCreateNewLifecycle" runat="server" Text="Create New Lifecycle" Visible="true"
                    OnClick="btnCreateNewLifecycle_Click" />
                <asp:Button ID="btnUpdateLifecycleModelToAge" runat="server" Text="Update Lifecycle Model's To Age" Visible="true"
                    OnClick="btnUpdateLifecycleModelToAge_Click" />
            </td>
        </tr>
    </table>
    <br />

    <asp:Panel ID="pnlLifecycleLines" runat="server" Visible="false">

        <asp:GridView ID="gvLifecycleLines" runat="server" AutoGenerateColumns="false" Caption="Lifecycle Lines"
            CaptionAlign="Top" PageSize="10" DataSourceID="odsLifecycleLines" AllowPaging="True"
            DataKeyNames="Key" AllowSorting="True" 
            SkinID="custom-EmptyDataTemplate" Width="100%"
            OnRowCommand="gvLifecycleLines_RowCommand"
            OnRowDataBound="gvLifecycleLines_RowDataBound"
            OnRowUpdating="gvLifecycleLines_RowUpdating"
            OnRowUpdated="gvLifecycleLines_RowUpdated" >
            <SelectedRowStyle BackColor="Gainsboro" />
            <Columns>
                <asp:TemplateField HeaderText="Age From" SortExpression="AgeFrom">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "AgeFrom")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <db:DecimalBox ID="dbAgeFrom" runat="server" AllowNegativeSign="false" 
                            DecimalPlaces="0" Width="30px"
                            Text='<%# DataBinder.Eval(Container.DataItem, "AgeFrom") %>' />
                    </EditItemTemplate>
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Model" SortExpression="ModelName">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ModelName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlModel" runat="server" DataSourceID="odsModels"
                            DataTextField="ModelName" DataValueField="Key" SkinID="broad"
                            SelectedValue='<%# DataBinder.Eval(Container.DataItem, "ModelID") %>' />
                        <asp:ObjectDataSource ID="odsModels" runat="server" SelectMethod="GetModelPortfolios"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.LifecycleMaintenanceAdapter" >
                        </asp:ObjectDataSource>
                    </EditItemTemplate>
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Created" SortExpression="LatestVersion_LatestVersionDate">
                    <ItemTemplate>
                        <%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate") != DateTime.MinValue ?
                            ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("d MMMM yyyy") : "")%>
                    </ItemTemplate>
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField HeaderText="Creator" DataField="CreatedBy" SortExpression="CreatedBy" ReadOnly="true" >
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Last Updated" SortExpression="LastUpdated">
                    <ItemTemplate>
                        <%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastUpdated") != DateTime.MinValue ?
                            ((DateTime)DataBinder.Eval(Container.DataItem, "LastUpdated")).ToString("d MMMM yyyy") : "")%>
                    </ItemTemplate>
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton 
                            ID="lbtEditLine" 
                            runat="server" 
                            CausesValidation="False" 
                            Text="Edit"
                            CommandName="EditLine"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                            OnCommand="lbtEditLine_Command" />
                        <asp:LinkButton 
                            ID="lbtDeleteLine" 
                            runat="server" 
                            CausesValidation="False" 
                            Text="Delete"
                            CommandName="DeleteLine"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                            OnCommand="lbtDeleteLine_Command"
                            OnClientClick="return confirm('Delete line?')" />
                        <asp:LinkButton 
                            ID="lbtUpdateLine" 
                            runat="server" 
                            CausesValidation="True" 
                            Text='<%# ((int)DataBinder.Eval(Container.DataItem, "Key") != 0 ? "Update" : "Insert") %>'
                            CommandName="Update"
                            Visible="False" />
                        <asp:LinkButton 
                            ID="lbtCancelLine" 
                            runat="server" 
                            CausesValidation="False" 
                            Text="Cancel"
                            CommandName="Cancel"
                            Visible="False" />
                    </ItemTemplate>
                    <ItemStyle Wrap="False" HorizontalAlign="Left" />
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblEmptyLifecycleLines" runat="server" Text="No Lines" />
            </EmptyDataTemplate>
        </asp:GridView>

        <asp:ObjectDataSource ID="odsLifecycleLines" runat="server" SelectMethod="GetLifecycleLines" 
            UpdateMethod="UpdateLifecycleLine" OldValuesParameterFormatString="original_{0}"
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.LifecycleMaintenanceAdapter"
            onupdated="odsLifecycles_Updated" >
            <SelectParameters>
                <asp:ControlParameter ControlID="gvLifecycles" Name="lifecycleId" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:ControlParameter ControlID="hdnIsLifecycleLineInsert" DefaultValue="False" Name="isInsert"
                    PropertyName="Value" Type="Boolean" />
            </SelectParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="gvLifecycles" Name="lifecycleId" PropertyName="SelectedValue"
                    Type="Int32" />
            </UpdateParameters>
        </asp:ObjectDataSource>

        <asp:Button ID="btnCreateNewLifecycleLine" runat="server" Text="Create New Line" Visible="true"
            OnClick="btnCreateNewLifecycleLine_Click" />
    
    </asp:Panel>

    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


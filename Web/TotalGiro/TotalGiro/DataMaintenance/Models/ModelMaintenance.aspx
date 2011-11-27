<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ModelMaintenance.aspx.cs"
    Inherits="ModelMaintenance" Title="Model Maintenance" %>

<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<%@ Register Src="../../UC/ModelFinder.ascx" TagName="ModelFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="cmg" runat="server" />
    <uc1:ModelFinder ID="ctlModelFinder" runat="server" />
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
    <br />
    <table width="1000px" cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="20">
                <asp:GridView ID="gvModels" runat="server" AutoGenerateColumns="false" Caption="Current Models"
                    CaptionAlign="Top" PageSize="10" DataSourceID="odsModels" AllowPaging="True"
                    DataKeyNames="Key" AllowSorting="True" OnRowDataBound="gvModels_RowDataBound"
                    OnRowCommand="gvModels_RowCommand" OnSelectedIndexChanged="gvModels_SelectedIndexChanged"
                    OnPageIndexChanged="gvModels_PageIndexChanged" SkinID="custom-EmptyDataTemplate"
                    Width="100%">
                    <SelectedRowStyle BackColor="Gainsboro" />
                    <Columns>
                        <asp:BoundField HeaderText="Model Name" DataField="ModelName" SortExpression="ModelName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Latest Version Nr" DataField="LatestVersion_VersionNumber"
                            SortExpression="LatestVersion_VersionNumber">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Last Updated" SortExpression="LatestVersion_LatestVersionDate">
                            <ItemTemplate>
                                <%# ((DateTime)DataBinder.Eval(Container.DataItem, "LatestVersion_LatestVersionDate") != DateTime.MinValue ?
                     ((DateTime)DataBinder.Eval(Container.DataItem, "LatestVersion_LatestVersionDate")).ToString("d MMMM yyyy") : "")%>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Model Owner" DataField="AssetManager_CompanyName" SortExpression="AssetManager_CompanyName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Version by" DataField="LatestVersion_CreatedBy_UserName"
                            SortExpression="LatestVersion_CreatedBy_UserName">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:CheckBoxField HeaderText="Common" SortExpression="IsPublic" DataField="IsPublic" >
                            <HeaderStyle wrap="False" />
                        </asp:CheckBoxField>
                        <asp:CheckBoxField HeaderText="SubModel" SortExpression="IsSubModel" DataField="IsSubModel" >
                            <HeaderStyle wrap="False" />
                        </asp:CheckBoxField>
                        <asp:CheckBoxField HeaderText="Active" SortExpression="IsActive" DataField="IsActive" >
                            <HeaderStyle wrap="False" />
                        </asp:CheckBoxField>
                        <asp:CommandField ShowSelectButton="True" SelectText="ViewVersionHistory">
                            <ItemStyle Wrap="False" Width="82px" />
                        </asp:CommandField>
                        <asp:TemplateField>
                            <ItemStyle Wrap="False" />
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtEditModel" Text="Edit" CommandName="EditModel"
                                    Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsModels" runat="server" SelectMethod="GetModels" TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctlModelFinder" Name="modelName" PropertyName="ModelName"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctlModelFinder" Name="activeStatus" PropertyName="ActiveStatus"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:Button ID="btnCreateNewModel" runat="server" Text="Create New Model" Visible="true"
                    OnClick="btnCreateNewModel_Click" />
            </td>
        </tr>
    </table>
    <br />
    <asp:MultiView ID="mlvVersion" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweViewVersions" runat="server">
            <table width="1000px" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:GridView ID="gvModelVersions" runat="server" AutoGenerateColumns="false" Caption="List of Versions"
                            CaptionAlign="Top" PageSize="5" DataSourceID="odsModelVersions" AllowPaging="True"
                            DataKeyNames="Key" AllowSorting="True" Visible="false" OnSelectedIndexChanged="gvModelVersions_SelectedIndexChanged"
                            SkinID="custom-EmptyDataTemplate" Width="100%" OnPageIndexChanged="gvModelVersions_PageIndexChanged">
                            <SelectedRowStyle BackColor="Gainsboro" />
                            <Columns>
                                <asp:BoundField HeaderText="Version Number" DataField="VersionNumber" SortExpression="VersionNumber">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Last Updated" SortExpression="LatestVersionDate">
                                    <ItemTemplate>
                                        <%# ((DateTime)DataBinder.Eval(Container.DataItem, "LatestVersionDate") != DateTime.MinValue ?
                                             ((DateTime)DataBinder.Eval(Container.DataItem, "LatestVersionDate")).ToString("d MMMM yyyy") : "")%>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Version by" DataField="CreatedBy_UserName" SortExpression="CreatedBy_UserName">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:CommandField ShowSelectButton="True" SelectText="View Components">
                                    <ItemStyle Wrap="False" />
                                </asp:CommandField>
                            </Columns>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="odsModelVersions" runat="server" SelectMethod="GetModelVersions"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="gvModels" Name="modelID" PropertyName="SelectedValue"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table><br />
            
            <table width="1000px" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:GridView ID="gvModelComponents" runat="server" AutoGenerateColumns="false" Caption="sModel Components"
                            CaptionAlign="Top" PageSize="20" DataSourceID="odsModelComponents" AllowPaging="True"
                            DataKeyNames="Key" AllowSorting="True" Visible="false" SkinID="custom-EmptyDataTemplate"
                            Width="100%">
                            <Columns>
                                <asp:BoundField HeaderText="ComponentName" DataField="ComponentName" SortExpression="ComponentName">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Component Type">
                                    <ItemTemplate>
                                        <%# (ModelComponentType)DataBinder.Eval(Container.DataItem, "ModelComponentType")%>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Allocation" DataField="DisplayAllocation" SortExpression="DisplayAllocation">
                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Allocation" DataField="Allocation" SortExpression="Allocation"
                                    Visible="false">
                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="odsModelComponents" runat="server" SelectMethod="GetModelComponents"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="gvModelVersions" Name="modelVersionID" PropertyName="SelectedValue"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table><br />
            
            <table width="1000px" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:GridView ID="gvModelInstruments" runat="server" AutoGenerateColumns="false"
                            Caption="Model Instruments" CaptionAlign="Top" PageSize="20" DataSourceID="odsModelInstruments"
                            AllowPaging="True" DataKeyNames="Key" AllowSorting="True" Visible="false" SkinID="custom-EmptyDataTemplate"
                            Width="100%">
                            <Columns>
                                <asp:BoundField HeaderText="Instrument" DataField="Component_Name" SortExpression="Component_Name">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Isin" DataField="Component_DisplayIsin" SortExpression="Component_DisplayIsin">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Allocation" DataField="DisplayAllocation" SortExpression="DisplayAllocation">
                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                    <HeaderStyle Wrap="False" HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Allocation" DataField="Allocation" SortExpression="Allocation"
                                    Visible="false">
                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="odsModelInstruments" runat="server" SelectMethod="GetModelInstruments"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="gvModelVersions" Name="modelVersionID" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="vweEditVersions" runat="server">
            <br />
            <table width="1000px" cellpadding="0" cellspacing="0" style="border: 1px solid #000000" >
                <tr>
                    <td colspan="8" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                        border-bottom: solid 1px black;">
                        <asp:Label ID="lblEditVersion" Font-Bold="true" runat="server" Text="Editing Model" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblModelName" runat="server" Text="Model Name:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:TextBox ID="tbModelName" MaxLength="80" runat="server" SkinID="broad" ></asp:TextBox>
                        <asp:CheckBox ID="chkIsSubModel" Text="Is SubModel" runat="server" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:RequiredFieldValidator ID="rfvModelName" ErrorMessage="Model Name"
                            ControlToValidate="tbModelName" 
                            ValidationGroup="UpdateModel"
                            runat="server">*</asp:RequiredFieldValidator>
                    </td>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblModelShortName" runat="server" Text="Short Name:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:TextBox ID="tbModelShortName" MaxLength="15" runat="server"></asp:TextBox>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblModelDescription" runat="server" Text="Model Description:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:TextBox ID="tbDescription" MaxLength="255" runat="server" SkinID="custom-width" Width="300px" ></asp:TextBox>
                    </td>
                    <td colspan="3" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:CheckBox ID="chkIsPublic" Text="Model is Common" runat="server" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                </tr>

                <tr>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblModelNotes" runat="server" Text="Model Notes:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:TextBox ID="tbNotes" MaxLength="200" runat="server" SkinID="custom-width" Width="300px" ></asp:TextBox>
                    </td>
                    <td colspan="3" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:CheckBox ID="chkIsActive" Text="Is Active" runat="server" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblExecutionOptions" runat="server" Text="Execution Only Options:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;"></td>
                    <td style="height: 25px;">
                        <asp:DropDownList ID="ddlExecutionOption" runat="server" AutoPostBack="true" DataTextField="Option" DataSourceID="odsExecutionOption" DataValueField="ID"></asp:DropDownList>
                        <asp:ObjectDataSource ID="odsExecutionOption" runat="server" SelectMethod="GetExecutionOptions"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                        </asp:ObjectDataSource>
                    </td>
                    <td style="height: 25px;">
                    </td>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblExpectedReturn" runat="server" Text="Expected Return:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <db:DecimalBox ID="dbExpectedReturn" runat="server" DecimalPlaces="2" AllowNegativeSign="false" />&nbsp;%
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblModelType" runat="server" Text="Type:"></asp:Label></td>
                    <td colspan="1" style="height: 25px;"></td>
                    <td style="height: 25px;">
                        <asp:DropDownList ID="ddlModelDetail" runat="server" DataSourceID="odsModelDetails"
                            DataTextField="Description" DataValueField="Key" AutoPostBack="true" 
                            OnSelectedIndexChanged="ddlModelDetail_SelectedIndexChanged" />
                        <asp:ObjectDataSource ID="odsModelDetails" runat="server" SelectMethod="GetModelDetailData"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                        </asp:ObjectDataSource>
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:RequiredFieldValidator ID="rfvModelDetail" runat="server" ControlToValidate="ddlModelDetail"
                            ErrorMessage="Type is mandatory" InitialValue="-2147483648" 
                            ValidationGroup="UpdateModel"
                            SetFocusOnError="True" Width="0px">*</asp:RequiredFieldValidator>
                    </td>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblCashFundAlternative" runat="server" Text="Cash Fund Alternative:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:DropDownList ID="ddlCashFundAlternative" runat="server" SkinID="broad" 
                            DataTextField="Name" DataValueField="Key" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblVersion" runat="server" Text="Next Version:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:TextBox ID="tbVersion" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="text-align: right; height: 25px;">
                        <asp:Label ID="lblTotalAllocations" runat="server" Text="Total Allocations:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td style="height: 25px;">
                        <asp:TextBox ID="tbAllocations" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:CustomValidator ID="customTotalAllocations" ErrorMessage="The model MUST be 100% Allocated."
                            ControlToValidate="tbAllocations" runat="server" 
                            ValidationGroup="UpdateModel"
                            OnServerValidate="customTotalAllocations_ServerValidate">*</asp:CustomValidator>
                    </td>
                </tr>
            </table><br />              

            <table width="1000px" cellpadding="0" cellspacing="0" >
                <tr>
                    <td colspan="3">
                        <asp:GridView ID="gvModelEditComponents" runat="server" AllowPaging="True" 
                                AllowSorting="True" AutoGenerateColumns="false" Caption="Model Components" 
                                CaptionAlign="Top" DataKeyNames="Key" DataSourceID="odsModelEditComponents" 
                                OnRowUpdated="gvModelEditComponents_RowUpdated" 
                                OnRowUpdating="gvModelEditComponents_RowUpdating" PageSize="20" 
                                SkinID="custom-EmptyDataTemplate" Visible="true" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="ComponentName" HeaderText="ComponentName" 
                                        ReadOnly="true" SortExpression="ComponentName">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Component Type">
                                        <ItemTemplate>
                                            <%# (ModelComponentType)DataBinder.Eval(Container.DataItem, "ModelComponentType")%>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Allocation" SortExpression="Allocation">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAllocation" runat="server">
                                            <%# DataBinder.Eval(Container.DataItem, "DisplayAllocation")%></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <db:DecimalBox ID="dbAllocation" runat="server" DecimalPlaces="7" 
                                                MaximumValue="1" 
                                                Value='<%# DataBinder.Eval(Container.DataItem, "Allocation")%>' />
                                        </EditItemTemplate>
                                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                                        <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ComponentID" HeaderText="ComponentID" 
                                        ReadOnly="true" SortExpression="ComponentID" Visible="false">
                                        <ItemStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:CommandField ShowEditButton="True">
                                        <ItemStyle HorizontalAlign="Right" Wrap="False" />
                                    </asp:CommandField>
                                    <asp:TemplateField>
                                        <ItemStyle Wrap="False" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbtnDeleteLine" runat="server" CausesValidation="False" 
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' 
                                                CommandName="DeleteLine" OnCommand="lbtnDelete_Command" Text="Delete" 
                                                Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="lblEmptyDataTemplateModelEditComponents" runat="server" 
                                        Text="No Lines" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="odsModelEditComponents" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="GetModelComponents" 
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelComponentHelper" 
                                UpdateMethod="UpdateModelComponent"></asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnUpdateModel" runat="server" OnClick="btnUpdateModel_Click" 
                                Text="Save Changes" ValidationGroup="UpdateModel" Visible="true" />
                            <asp:Button ID="btnCancelChanges" runat="server" CausesValidation="false" 
                                OnClick="btnCancelChanges_Click" Text="Cancel Changes" Visible="true" />
                        </td>
                        <td>
                        </td>
                        <td align="right" style="float:right;">
                            <asp:Button ID="btnAddNewComponentLine" runat="server" 
                                OnClick="btnAddNewComponentLine_Click" Text="Add New Line" Visible="true" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlEditComponent" runat="server" Visible="false">
                                <table cellpadding="0" cellspacing="0" style="border: 1px solid #000000" 
                                    width="100%">
                                    <tr>
                                        <td colspan="7" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                                        border-bottom: solid 1px black;">
                                            <asp:Label ID="lblNewComponentLine" runat="server" Font-Bold="true" 
                                                Text="Create new Component Line" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="7"></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblAddModelComponentType" runat="server" Text="Type:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlComponentType" runat="server" AutoPostBack="true" 
                                                OnSelectedIndexChanged="ddlComponentType_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblModelComponent" runat="server" Text="Component:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlComponent" runat="server" 
                                                DataSourceID="odsAddInstrument" DataTextField="Name" DataValueField="Key" 
                                                SkinID="broad">
                                            </asp:DropDownList>
                                            <asp:ObjectDataSource ID="odsAddInstrument" runat="server" 
                                                SelectMethod="GetInstruments" 
                                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                                            </asp:ObjectDataSource>
                                            <asp:ObjectDataSource ID="odsAddModel" runat="server" 
                                                SelectMethod="GetModelsExcludedCurrentModel" 
                                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="gvModels" Name="modelID" 
                                                        PropertyName="SelectedValue" Type="Int32" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblAddAllocation" runat="server" Text="Allocation:"></asp:Label>
                                        </td>
                                        <td>
                                            <db:DecimalBox ID="tbAddAllocation" runat="server" DecimalPlaces="7" 
                                                MaximumValue="1" />
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td colspan="5"></td>
                                        <td>
                                            <asp:Label ID="lblSuggestion" runat="server" BackColor="Yellow" 
                                                Text="e.g., 23,4000% = 0,23400"></asp:Label>
                                        </td>
                                        <td></td></tr>
                                    <tr><td></td></tr>
                                </table>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnUpdateNewComponentLine" runat="server" 
                                                OnClick="btnUpdateNewComponentLine_Click" Text="Update Line" Visible="true" />
                                            <asp:Button ID="btnCancelAddNewComponentLine" runat="server" 
                                                CausesValidation="false" OnClick="btnCancelAddNewComponentLine_Click" 
                                                Text="Cancel" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr><td style="text-align:center"></td></tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3">
                            <asp:GridView ID="gvModelEditPerformances" runat="server" AllowPaging="True" 
                                AllowSorting="True" AutoGenerateColumns="False" Caption="Model Benchmark performances"
                                CaptionAlign="Top" DataKeyNames="Key" DataSourceID="odsModelEditPerformances"
                                OnRowUpdating="gvModelEditPerformances_RowUpdating"
                                PageSize="4" SkinID="custom-EmptyDataTemplate" Visible="true" Width="100%" >
                                <Columns>
                                    <asp:BoundField DataField="PerformanceYear" HeaderText="PerformanceYear" SortExpression="PerformanceYear" ReadOnly="true" >
                                        <ItemStyle Wrap="False" CssClass="aligncenter" Width="130px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Quarter" HeaderText="Quarter" ReadOnly="true" >
                                        <ItemStyle Wrap="False" CssClass="aligncenter" Width="60px" />                      
                                    </asp:BoundField>                                    
                                    <asp:BoundField DataField="IBoxxTarget" DataFormatString="{0:#,##0.00}" HeaderText="IBoxxTarget (%)" ReadOnly="true">
                                        <ItemStyle Wrap="True" CssClass="aligncenter" Width="115px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MSCIWorldTarget" DataFormatString="{0:#,##0.00}" HeaderText="MSCIWorldTarget (%)" ReadOnly="true" >
                                        <ItemStyle Wrap="True" CssClass="aligncenter" Width="150px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CompositeTarget" DataFormatString="{0:#,##0.00}" HeaderText="CompositeTarget (%)" ReadOnly="true" >
                                        <ItemStyle Wrap="True" CssClass="aligncenter" Width="150px"/>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="BenchMarkValue (%)" SortExpression="BenchMarkValue" >
                                        <ItemTemplate >
                                            <asp:Label ID="lblBenchMarkValue" runat="server" >
                                                <%# DataBinder.Eval(Container.DataItem, "BenchMarkValue")%></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <db:DecimalBox ID="dbBenchMarkValue" runat="server" AlignLeft="true" DecimalPlaces="5" MaximumValue="0" AllowNegativeSign="true" Value='<%# DataBinder.Eval(Container.DataItem, "BenchMarkValue")%>' />
                                        </EditItemTemplate>
                                        <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                        <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                                    </asp:TemplateField>
                                    <asp:CommandField ShowEditButton="True" ><ItemStyle Wrap="False" CssClass="alignright"/></asp:CommandField>
                                   
                                    <asp:TemplateField>
                                        <ItemStyle Wrap="False" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbtnDeleteModelBenchMarkPerformanceLine" runat="server" CausesValidation="False" 
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' 
                                                CommandName="DeleteLine" OnCommand="lbtnDeleteModelBenchMarkPerformanceLine_Command" Text="Delete" 
                                                Visible="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="lblEmptyDataTemplateModelEditPerformances" runat="server" Text="No Modelbenchmarkperformance Lines" />
                                </EmptyDataTemplate>                                
                            </asp:GridView>
                            <asp:ObjectDataSource ID="odsModelEditPerformances" runat="server" SelectMethod="GetModelPerformances"
                                UpdateMethod="UpdateModelPerformance" OldValuesParameterFormatString="original_{0}"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="gvModels" Name="modelID" PropertyName="SelectedValue" Type="Int32" />
                                    <asp:Parameter Name="quarter" Type="Int32" />
                                    <asp:Parameter Name="yyyy" Type="Int32" />                                                                        
                                </SelectParameters>
                            </asp:ObjectDataSource>      
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="float:right;" align="right" >
                            <asp:Button ID="btnAddNewModelPerformanceLine" runat="server" OnClick="btnAddNewModelPerformanceLine_Click" Text="Add New Line" Visible="true" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlEditModelBenchmarkPerformance" runat="server" Visible="false">
                                <asp:UpdatePanel ID="updatePnl" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>                                
                                        <table cellpadding="0" cellspacing="0" style="border: 1px solid #000000" width="100%">
                                        <tr>
                                            <td colspan="10" style="padding-left: 5px; background-color: #AAB9C2; height: 20px; border-bottom: solid 1px black;">
                                                <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Create new ModelPerformance Line" />
                                            </td>
                                        </tr>
                                        <tr><td colspan="10"></td></tr>
                                        
                                        <tr>
                                            <td style="text-align: right; height: 25px;">
                                                <asp:Label ID="lblBoxxTarget" runat="server" Text="IBoxx target:"></asp:Label></td>
                                            <td colspan="1"></td>
                                            <td>
                                                <db:DecimalBox ID="dbBoxxTarget" runat="server" DecimalPlaces="2" AllowNegativeSign="false" />&nbsp;%</td>
                                            <td colspan="1"></td>
                                            <td style="text-align: right">
                                                <asp:Label ID="lblBenchmarkPerformance" runat="server" Text="Benchmark performance:"></asp:Label></td>
                                            <td colspan="1"></td>
                                            <td style="padding-right:100px">
                                                <db:DecimalBox ID="dbBenchmarkPerformance" runat="server" DecimalPlaces="5" AllowNegativeSign="true" />&nbsp;%</td>
                                            <td colspan="1"></td>
                                            <td><asp:Label ID="lblYear" runat="server" Text="Year and Quarter:"></asp:Label></td>
                                            <td><b4f:YearMonthPicker ID="YearOfBenchmarkPerformance" runat="server" 
                                                    DefaultToCurrentPeriod="true" IsButtonDeleteVisible="false" 
                                                    ListYearsBeforeCurrent="1"  ListQuarterForCurrent="4"/>
                                            </td>                                    
                                        </tr>
                                        <tr>
                                            <td style="text-align: right; height: 25px;">
                                                <asp:Label ID="lblMSCIWorldTarget" runat="server" Text="MSCIWorld target:"></asp:Label></td>
                                            <td colspan="1"></td>
                                            <td colspan="8">
                                                <db:DecimalBox ID="dbMSCIWorldTarget" runat="server" DecimalPlaces="2" AllowNegativeSign="false" />&nbsp;%</td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right; height: 25px;">
                                                <asp:Label ID="lblCompositeTarget" runat="server" Text="CompositeTarget:"></asp:Label></td>
                                            <td colspan="1"></td>
                                            <td colspan="8">
                                                <db:DecimalBox ID="dbCompositeTarget" runat="server" DecimalPlaces="2" AllowNegativeSign="false" />&nbsp;%</td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right; height: 25px;">
                                                <asp:Label ID="lblCategoryWeighting" runat="server" Text="Total Category weighting:"></asp:Label></td>
                                            <td colspan="1" style="height: 25px;"></td>
                                            <td colspan="1" style="height: 25px;">
                                                <db:DecimalBox ID="dbCategoryWeighting" runat="server" DecimalPlaces="2" AllowNegativeSign="false" Enabled="false" BackColor="LightGray"/>&nbsp;%</td>
                                            <td colspan="7"><asp:Label ID="lblMessage" runat="server" Visible="false" ForeColor="red" /></td>
                                            
                                        </tr>
                                    </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:Panel>
                            <asp:Button ID="btnSaveNewModelPerformance" runat="server" OnClick="btnSaveNewModelPerformance_Click" Text="Save New ModelPerformance" width="185px" Visible="false" />
                            <asp:Button ID="btnCancleNewModelPerformance" runat="server" OnClick="btnCancleNewModelPerformance_Click" Text="Cancle New ModelPerformance" width="185px"  Visible="false"/>
                        </td>
                    </tr>
                    <tr><td></td></tr>
                    <tr>
                        <td colspan="3">
                            <asp:GridView ID="gvCommissionRules" runat="server" AllowPaging="True" 
                                AllowSorting="True" AutoGenerateColumns="False" Caption="Commission Rules" 
                                DataKeyNames="Key" DataSourceID="odsCommissionRules" 
                                OnRowCommand="gvCommissionRules_RowCommand" 
                                OnRowDataBound="gvCommissionRules_RowDataBound" PageSize="5" 
                                SkinID="custom-EmptyDataTemplate" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="CommRuleName" HeaderText="Commission Rule" 
                                        SortExpression="CommRuleName">
                                        <HeaderStyle Wrap="False" />
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CommCalculation_Name" HeaderText="Calculation" 
                                        SortExpression="CommCalculation_Name">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                                        <ItemTemplate>
                                            <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")) ?
                                            "" : ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToString("d MMMM yyyy"))%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                                        <ItemTemplate>
                                            <%# (B4F.TotalGiro.Utils.Util.IsNullDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) ?
                                            "" : ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToString("d MMMM yyyy"))%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbtnEditCommissionRule" runat="server" 
                                                CommandName="EditRule" Text="Edit" />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="lblEmptyDataTemplateCR" runat="server" 
                                        Text="No Commission Rules" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="odsCommissionRules" runat="server" 
                                SelectMethod="GetModelCommissionRules" 
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleOverviewAdapter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="gvModels" Name="modelID" 
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Button ID="btnCreateCR" runat="server" Enabled="False" OnClick="btnCreateCR_Click" Text="Create Commission Rule" Width="185px" /></td>
                    </tr>
                    <tr><td></td></tr>

                    <tr>
                        <td colspan="3">
                            <asp:GridView ID="gvFeeRules" runat="server" AllowPaging="True" 
                                AllowSorting="True" AutoGenerateColumns="False" Caption="Fee Rules" 
                                DataKeyNames="Key" DataSourceID="odsFeeRules" 
                                OnRowUpdating="gvFeeRules_RowUpdating" PageSize="5" 
                                SkinID="custom-EmptyDataTemplate" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="FeeCalculation_Name" HeaderText="Calculation" 
                                        ReadOnly="true" SortExpression="FeeCalculation_Name">
                                        <HeaderStyle Wrap="False" />
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ExecutionOnly" HeaderText="Execution-Only" 
                                        ReadOnly="true" SortExpression="ExecutionOnly">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SendByPost" HeaderText="Send By Post" 
                                        ReadOnly="true" SortExpression="SendByPost">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="StartPeriod" HeaderText="Start Period" 
                                        ReadOnly="true" SortExpression="StartPeriod">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="End Period" SortExpression="EndPeriod">
                                        <ItemTemplate>
                                            <%# (int)DataBinder.Eval(Container.DataItem, "EndPeriod") == 0 ?
                                            "" : DataBinder.Eval(Container.DataItem, "EndPeriod")%>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <b4f:YearMonthPicker ID="ppEndPeriod" runat="server" 
                                                IsButtonDeleteVisible="true" ListYearsBeforeCurrent="4" 
                                                SelectedPeriod='<%# (int)DataBinder.Eval(Container.DataItem, "EndPeriod")%>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField ShowEditButton="True">
                                        <ItemStyle Wrap="False" />
                                    </asp:CommandField>
                                    <%--<asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="lbtnEditFeeRule" Text="Edit" CommandName="EditRule" 
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' 
                                            OnCommand="lbtnEditFeeRule_Command" />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField> --%>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="lblEmptyDataTemplateCR" runat="server" Text="No Fee Rules" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="odsFeeRules" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="GetModelFeeRules" 
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter" 
                                UpdateMethod="UpdateModelFeeRule">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="gvModels" Name="modelID" PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlFeeRuleEntry" runat="server" Visible="false">
                                <table cellpadding="0" cellspacing="0" style="border: 1px solid #000000" 
                                    width="400px">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblFeeCalculation" runat="server" Text="Fee Calculation" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlFeeCalculation" runat="server" 
                                                DataSourceID="odsFeeCalculation" DataTextField="Name" DataValueField="Key" 
                                                SkinID="broad" />
                                            <asp:ObjectDataSource ID="odsFeeCalculation" runat="server" 
                                                SelectMethod="GetActiveFeeCalculations" 
                                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ModelMaintenanceAdapter">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="gvModels" Name="modelID" 
                                                        PropertyName="SelectedValue" Type="Int32" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </td>
                                        <td style="width: 50px">
                                            <asp:RequiredFieldValidator ID="rfvFeeCalculation" runat="server" 
                                                ControlToValidate="ddlFeeCalculation" 
                                                ErrorMessage="Fee Calculation is mandatory" InitialValue="-2147483648" 
                                                SetFocusOnError="True" Width="0px">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;</td>
                                        <td>
                                            <asp:CheckBox ID="chkExecutionOnly" runat="server" Checked="false" 
                                                Text="Execution-Only" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;</td>
                                        <td>
                                            <asp:CheckBox ID="chkSendByPost" runat="server" Checked="false" 
                                                Text="Send By Post" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblStartPeriod" runat="server" Text="Start Period" />
                                        </td>
                                        <td>
                                            <b4f:YearMonthPicker ID="ppStartPeriod" runat="server" 
                                                DefaultToCurrentPeriod="true" IsButtonDeleteVisible="false" 
                                                ListYearsBeforeCurrent="1" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEndPeriod" runat="server" Text="End Period" />
                                        </td>
                                        <td>
                                            <b4f:YearMonthPicker ID="ppEndPeriod" runat="server" 
                                                ListYearsBeforeCurrent="0" />
                                        </td>
                                        <td>
                                            <asp:CustomValidator ID="cvEndPeriod" runat="server" 
                                                ControlToValidate="ppEndPeriod"
                                                ErrorMessage="The end period can not be before the start period" 
                                                OnServerValidate="cvEndPeriod_ServerValidate" ValidationGroup="CreateFeeRule">*</asp:CustomValidator>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Button ID="btnCreateFeeRule" runat="server" CausesValidation="true" OnClick="btnCreateFeeRule_Click" Text="Create Fee Rule" ValidationGroup="CreateFeeRule" />
                            <asp:Button ID="btnCancelCreateFeeRule" runat="server" CausesValidation="false" OnClick="btnCancelCreateFeeRule_Click" Text="Cancel" Visible="false" />
                        </td>
                    </tr>
                
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>

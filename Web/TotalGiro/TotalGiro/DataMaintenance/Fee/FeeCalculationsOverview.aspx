<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="FeeCalculationsOverview.aspx.cs"
    Inherits="FeeCalculationsOverview" Title="Fee Calculations Overview" %>

<%@ Import Namespace="B4F.TotalGiro.Fees" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/FlatSlab.ascx" TagName="FlatSlabControl" TagPrefix="Custom" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="cmg" runat="server" />
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblCalcName" runat="server" Text="Calculation Name:" />
            </td>
            <td style="width: 190px">
                <asp:TextBox ID="txtCalcName" runat="server" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblFeeTypeSelector" runat="server" Text="Fee Type:" />
            </td>
            <td style="width: 190px">
                <asp:DropDownList ID="ddlFeeTypeSelector" runat="server" DataSourceID="odsFeeTypes"
                    DataTextField="Name" DataValueField="Key" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsFeeTypes" runat="server" SelectMethod="GetFeeTypes"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeCalculationsOverviewAdapter">
                </asp:ObjectDataSource>
            </td>
            <td></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="lblModelActive" runat="server" Text="Status:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlActiveStatus" runat="server" DataSourceID="odsActiveStati"
                    DataTextField="Status" DataValueField="ID" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsActiveStati" runat="server" SelectMethod="GetAccountStatuses"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter">
                </asp:ObjectDataSource>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                   CausesValidation="False" Width="90px" />
            </td>
        </tr>
    </table>
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
    <table cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td colspan="20">
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <table width="1000px" cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="20">
                <asp:GridView ID="gvCalculations" runat="server" AutoGenerateColumns="false" Caption="Fee Calculations"
                    CaptionAlign="Top" 
                    SkinID="custom-EmptyDataTemplate"
                    DataSourceID="odsCalculations" 
                    PageSize="10" AllowPaging="True"
                    DataKeyNames="Key" AllowSorting="True" 
                    OnRowCommand="gvCalculations_RowCommand"
                    OnSelectedIndexChanged="gvCalculations_SelectedIndexChanged"
                    OnPageIndexChanged="gvCalculations_PageIndexChanged"
                    OnRowEditing="gvCalculations_RowEditing"
                    OnRowCancelingEdit="gvCalculations_RowCancelingEdit"
                    OnRowUpdating="gvCalculations_RowUpdating"
                    OnRowDataBound="gvCalculations_RowDataBound"
                    Width="100%">
                    <SelectedRowStyle BackColor="Gainsboro" />
                    <Columns>
                        <asp:BoundField HeaderText="Name" DataField="Name" SortExpression="Name" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Fee Type" SortExpression="FeeType_Key">
                            <ItemStyle wrap="False" />
                            <ItemTemplate>
                                <%# (FeeTypes)DataBinder.Eval(Container.DataItem, "FeeType_Key")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Currency" DataField="FeeCurrency_AltSymbol" 
                            SortExpression="FeeCurrency_AltSymbol" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                         <asp:BoundField HeaderText="Latest Version Nr" DataField="LatestVersion_VersionNumber"
                            SortExpression="LatestVersion_VersionNumber" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Last Updated" SortExpression="LatestVersion_LastUpdated">
                            <ItemTemplate>
                                <%# (DataBinder.Eval(Container.DataItem, "LatestVersion_LastUpdated") != System.DBNull.Value ? ((DateTime)DataBinder.Eval(Container.DataItem, "LatestVersion_LastUpdated") != DateTime.MinValue ?
                                 ((DateTime)DataBinder.Eval(Container.DataItem, "LatestVersion_LastUpdated")).ToString("d MMMM yyyy") : "") : "") %>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Version by" DataField="LatestVersion_CreatedBy"
                            SortExpression="LatestVersion_CreatedBy" ReadOnly="true" >
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:CheckBoxField HeaderText="Active" SortExpression="IsActive" DataField="IsActive" ReadOnly="true" >
                            <HeaderStyle wrap="False" />
                        </asp:CheckBoxField>
                        <asp:TemplateField  >
                            <EditItemTemplate>
                                <b4f:YearMonthPicker ID="ppEnd" runat="server" ListYearsBeforeCurrent="0" IsButtonDeleteVisible="true" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:CommandField ShowSelectButton="True" SelectText="History">
                            <ItemStyle Wrap="False" Width="82px" />
                        </asp:CommandField>
                        <asp:TemplateField>
                            <ItemStyle Wrap="False" />
                            <ItemTemplate>
                                <asp:LinkButton
                                    ID="lbtEditCalc" runat="server" 
                                    Text="Edit" 
                                    CommandName="EditCalc" />
                                <%--<asp:LinkButton
                                    ID="lbtDeleteCalc" runat="server" 
                                    Text="Delete" 
                                    Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "IsActive") %>'
                                    CommandName="DelCalc" />--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" EditText="Delete" >
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                        </asp:CommandField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsCalculations" runat="server" SelectMethod="GetFeeCalculations" UpdateMethod="CancelFeeCalculation"
                    TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeCalculationsOverviewAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCalcName" Name="calcName" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ddlFeeTypeSelector" Name="feeType" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ddlActiveStatus" Name="activeStatus" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:Parameter  Name="propertyList" DefaultValue="Key, Name, FeeType.Key, FeeCurrency.AltSymbol, LatestVersion.VersionNumber, LatestVersion.LastUpdated, LatestVersion.CreatedBy, IsActive" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <asp:Button ID="btnCreateNewCalc" runat="server" Text="Create New Calculation" Visible="true"
                    OnClick="btnCreateNewCalc_Click" />
            </td>
        </tr>
    </table>
    <br />
    <asp:MultiView ID="mlvVersion" runat="server" ActiveViewIndex="0">
        <asp:View ID="vweViewVersions" runat="server">
            <table width="1000px" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:GridView ID="gvCalcVersions" runat="server" AutoGenerateColumns="false" Caption="List of Versions"
                            CaptionAlign="Top" PageSize="5" DataSourceID="odsCalcVersions" AllowPaging="True"
                            DataKeyNames="Key" AllowSorting="True" Visible="false" 
                            SkinID="custom-EmptyDataTemplate" Width="100%" 
                            OnPageIndexChanged="gvCalcVersions_PageIndexChanged"
                            OnSelectedIndexChanged="gvCalcVersions_SelectedIndexChanged" >
                            <SelectedRowStyle BackColor="Gainsboro" />
                            <Columns>
                                <asp:TemplateField HeaderText="Calc Type" SortExpression="FeeCalcType">
                                    <ItemStyle wrap="False" />
                                    <ItemTemplate>
                                        <%# (FeeCalcTypes)DataBinder.Eval(Container.DataItem, "FeeCalcType")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Version Number" DataField="VersionNumber" SortExpression="VersionNumber">
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
                                <asp:BoundField HeaderText="Version by" DataField="CreatedBy" SortExpression="CreatedBy">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Start" DataField="StartPeriod" SortExpression="StartPeriod">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="End" SortExpression="EndPeriod">
                                    <ItemTemplate>
                                        <%# ((int)DataBinder.Eval(Container.DataItem, "EndPeriod") == 0 ? "" : DataBinder.Eval(Container.DataItem, "EndPeriod"))%>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Setup" DataField="FixedSetup_DisplayString" SortExpression="FixedSetup_DisplayString">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField >
                                    <ItemTemplate>
                                        <asp:LinkButton 
                                            ID="lbtnLines" runat="server" 
                                            Text="View Lines"
                                            Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "FeeCalcType") != 3 ? true : false) %>'
                                            CommandName="Select"
                                            />
                                    </ItemTemplate>                   
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="odsCalcVersions" runat="server" SelectMethod="GetFeeCalcVersions"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeCalculationsOverviewAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="gvCalculations" Name="calcID" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:Parameter  Name="propertyList" DefaultValue="Key, FeeCalcType, VersionNumber, LastUpdated, StartPeriod, EndPeriod, FixedSetup.DisplayString, CreatedBy" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
            <br />
            <table width="1000px" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:GridView ID="gvCalcLines" runat="server" AutoGenerateColumns="false" Caption="Fee Calc Lines"
                            CaptionAlign="Top" PageSize="20" DataSourceID="odsCalcLines" AllowPaging="True"
                            DataKeyNames="Key" AllowSorting="True" Visible="false" SkinID="custom-EmptyDataTemplate" >
                            <Columns>
                                <asp:BoundField HeaderText="Serial Number" DataField="SerialNo" SortExpression="SerialNo">
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Range" DataField="DisplayRange" SortExpression="DisplayRange" HtmlEncode="false" >
                                    <ItemStyle Wrap="False" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Fee %" DataField="FeePercentage" SortExpression="FeePercentage">
                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="StaticCharge" DataField="StaticCharge_DisplayString" SortExpression="StaticCharge_DisplayString" >
                                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                                    <HeaderStyle Wrap="False" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="odsCalcLines" runat="server" SelectMethod="GetFeeCalcLines"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeCalculationsOverviewAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="gvCalcVersions" Name="versionID" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:Parameter  Name="propertyList" DefaultValue="Key, SerialNo, DisplayRange, FeePercentage, StaticCharge.DisplayString" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="vweEditVersions" runat="server">
            <br />
            <table width="750px" cellpadding="0" cellspacing="0" style="border: 1px solid #000000" >
                <tr>
                    <td colspan="13" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                        border-bottom: solid 1px black;">
                        <asp:Label ID="lblEditCalculation" Font-Bold="true" runat="server" Text="Editing Calculation" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblCalcNameLabel" runat="server" Text="Calculation Name:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <asp:TextBox ID="tbCalcName" MaxLength="80" runat="server" SkinID="broad" ></asp:TextBox>
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:RequiredFieldValidator ID="rfvCalcName" ErrorMessage="Calculation Name"
                            ControlToValidate="tbCalcName" 
                            runat="server">*</asp:RequiredFieldValidator>
                    </td>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblFeeType" runat="server" Text="Fee Type:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="2" style="height: 25px;">
                        <asp:DropDownList ID="ddlFeeType" runat="server" DataSourceID="odsMgtFeeTypes"
                            DataTextField="Name" DataValueField="Key" />
                        <asp:ObjectDataSource ID="odsMgtFeeTypes" runat="server" SelectMethod="GetFeeTypes"
                            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee.FeeCalculationsOverviewAdapter">
                            <SelectParameters>
                                <asp:Parameter Name="managementType" DefaultValue="1" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:RequiredFieldValidator ID="rfvFeeType" ErrorMessage="Fee Type"
                            ControlToValidate="ddlFeeType"
                            InitialValue="-2147483648"
                            runat="server">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblCurrency" runat="server" Text="Fee Currency:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <asp:DropDownList ID="ddlCurrency" runat="server" SkinID="custom-width" Width="75px"
                            AutoPostBack="true" DataTextField="Symbol" DataValueField="Key" DataSourceID="odsCurrencies">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsCurrencies" runat="server" SelectMethod="GetCurrencies"
                            TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.MoneyTransferOrderAdapter">
                        </asp:ObjectDataSource>
                    </td>
                    <td colspan="4" style="height: 25px;">
                    </td>
                    <td colspan="2" style="height: 25px;">
                        <asp:CheckBox ID="chkIsActive" Text="Is Active" runat="server" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                </tr>

                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblStartPeriod" runat="server" Text="Start Period:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <b4f:YearMonthPicker ID="ppStartPeriod" runat="server" ListYearsBeforeCurrent="4" IsButtonDeleteVisible="true" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:RequiredFieldValidator ID="rfvStartPeriod" runat="server" ControlToValidate="ppStartPeriod"
                            ErrorMessage="Start Period is mandatory" 
                            SetFocusOnError="True" Width="0px">*</asp:RequiredFieldValidator>
                    </td>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblEndPeriod" runat="server" Text="End Period:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="2" style="height: 25px;">
                        <b4f:YearMonthPicker ID="ppEndPeriod" runat="server" ListYearsBeforeCurrent="4"  />
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:CustomValidator ID="cvEndPeriod" ErrorMessage="The end period can not be before the start period"
                            ControlToValidate="ppEndPeriod" runat="server" 
                            OnServerValidate="cvEndPeriod_ServerValidate">*</asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="13" />
                </tr>
                <tr>
                    <td colspan="13" style="padding-left: 5px; background-color: #AAB9C2; height: 20px;
                        border-bottom: solid 1px black;">
                        <asp:Label ID="lblConstraints" Font-Bold="true" runat="server" Text="Constraints" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblBasedOn" runat="server" Text="Based On:"/>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <asp:RadioButtonList 
                            ID="rbSlabFlat"
                            OnSelectedIndexChanged="rbSlabFlat_SelectedIndexChanged" 
                            RepeatDirection="Horizontal"
                            runat="server"
                            AutoPostBack="True">
                                <%--<asp:ListItem Value="2">Slab</asp:ListItem>--%>
                                <asp:ListItem Value="1" Selected="True">Flat</asp:ListItem>
                                <asp:ListItem Value="3">Simple</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td colspan="1" style="height: 25px;">
                         <asp:RequiredFieldValidator 
                            ID="reqSlabFlat"
                            runat="server"
                            ControlToValidate="rbSlabFlat"
                            Text="*" 
                            ErrorMessage="Choice required."/>
                    </td>
                    <td colspan="6" />
                </tr>
                <tr>
                    <td colspan="13">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <asp:CheckBox ID="chkNoFees" runat="server" Text="No Fees" AutoPostBack="true" OnCheckedChanged="chkNoFees_CheckedChanged" />
                    </td>
                    <td colspan="7" />
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblSetUp" runat="server" Text="Set up:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <db:DecimalBox ID="dbSetUp" runat="server" DecimalPlaces="2" Width="85px" />
                        <asp:Literal ID="litSetUpEuroSign" runat="server">€</asp:Literal> 
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:RangeValidator ID="rangeValSetUp" runat="server"
                            Type="Double"
                            ControlToValidate="dbSetUp:tbDecimal"
                            Text="*"
                            MinimumValue="0"
                            MaximumValue="100000000000"
                            ErrorMessage="Enter a number, format: 100.000.000.000,00.">
                        </asp:RangeValidator>
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:CustomValidator 
                            ID="custSetUp" 
                            runat="server"
                            ValidateEmptyText="true"
                            ControlToValidate="dbSetUp:tbDecimal"
                            OnServerValidate="CustSetUp"
                            Text="*" 
                            ErrorMessage="Enter either setup fee and/or Staffel fee. Or a setup fee with value 0."/>
                    </td>
                    <td colspan="5" />
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblMinValue" runat="server" Text="Minimum Value:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <db:DecimalBox ID="dbMinValue" runat="server" DecimalPlaces="2" Width="85px" />
                        <asp:Literal ID="litMinValueEuroSign" runat="server">€</asp:Literal> 
                    </td>
                    <td colspan="7" />
                </tr>

                <tr>
                    <td colspan="2" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblMaxValue" runat="server" Text="Maximum Value:"></asp:Label>
                    </td>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <db:DecimalBox ID="dbMaxValue" runat="server" DecimalPlaces="2" Width="85px" />
                        <asp:Literal ID="litMaxValueEuroSign" runat="server">€</asp:Literal> 
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:CompareValidator 
                            ID="cvMaxValue" 
                            runat="server"
                            ControlToValidate="dbMaxValue:tbDecimal"
                            ControlToCompare="dbMinValue:tbDecimal"
                            Operator="GreaterThan"
                            Type="Double"
                            Text="*" 
                            ErrorMessage="Maximum value should be greater than the minimum value"/>&nbsp
                    </td>
                    <td colspan="6" />
                </tr>

                <tr>
                    <td colspan="1" style="text-align: right; height: 25px;">
                        <asp:Label ID="lblStaffel" runat="server" Font-Bold="True" Text="Staffel"></asp:Label>
                    </td>
                    <td colspan="2" style="height: 25px;">
                    </td>
                    <td colspan="3" style="height: 25px;">
                        <asp:Button ID="btnAddNewFlatSlabRange" runat="server" Text="+" CausesValidation="false" OnClick="btnAddNewFlatSlabRange_Click" />
                        <asp:Button ID="btnDeleteFlatSlabRange" runat="server" Text="-" CausesValidation="false" OnClick="btnDeleteFlatSlabRange_Click" />
                    </td>
                    <td colspan="1" style="height: 25px;">
                        <asp:CustomValidator 
                            ID="custValRanges" 
                            runat="server"
                            Visible="true"
                            OnServerValidate="checkRanges"
                            Text="*" 
                            ErrorMessage="Ranges are not defined correctly. A range should start at an amount greater than the previous one."/>
                    </td>
                    <td colspan="6" />
                </tr>
                <tr>
                    <td colspan="1" style="height: 25px;">
                    </td>
                    <td colspan="12">
                        <asp:PlaceHolder ID="FlatSlabPlaceHolder" runat="server" EnableViewState="True"></asp:PlaceHolder>
                    </td>
                </tr>
            </table> 
            <br />
            <asp:Button ID="btnUpdateVersion" runat="server" Text="Save Changes" Visible="true" 
                OnClick="btnUpdateVersion_Click" />
            <asp:Button ID="btnCancelChanges" runat="server" Text="Cancel Changes" Visible="true" CausesValidation="false"
                OnClick="btnCancelChanges_Click" />
        </asp:View>
    </asp:MultiView>
</asp:Content>

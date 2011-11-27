<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="KickbackOverview.aspx.cs" Inherits="KickbackOverview" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc3" %>
<%@ Register Src="../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.ManagementPeriodUnits" %>
<%@ Import Namespace="B4F.TotalGiro.Fees" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <asp:HiddenField ID="hfYear" runat="server" />
    <asp:HiddenField ID="hfQuarter" runat="server" />
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowRemisier="true" ShowRemisierEmployee="true" ShowModelPortfolio="true" ShowSearchButton="false" ShowContactActiveCbl="true" ContactActiveAll="true" />
    <table>
        <tr>
            <td width="0.15px"> </td>
            <td width="128px">
                <asp:Label ID="lblYear" runat="server" Text="Year" />
            </td>
            <td width="150px">
                <asp:DropDownList ID="ddlYear" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblQuarter" runat="server" Text="Quarter" />
            </td>
            <td width="100px">
                <asp:DropDownList ID="ddlQuarter" runat="server" SkinID="custom-width" Width="50px" >
                    <asp:ListItem Text="Q1" Value="1"  />
                    <asp:ListItem Text="Q2" Value="2" />
                    <asp:ListItem Text="Q3" Value="3" />
                    <asp:ListItem Text="Q4" Value="4" />
                </asp:DropDownList>
            </td>
        </tr>

        <tr style="height: 25px;" >
            <td></td>
            <td>
                <asp:Label ID="lblContinuationStatus" runat="server" Text="Is Leaving?"  />
            </td>
            <td colspan="2">
                <asp:DropDownList ID="ddlContinuationStatus" runat="server" >
                    <asp:ListItem Text="Leaving" Value="1" />
                    <asp:ListItem Text="Current" Value="2" />
                    <asp:ListItem Text="All" Value="0" Selected="True" />
                </asp:DropDownList>
            </td>
            <td>
                <table width="240px" >
                    <tr>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" Style="float: left; position: relative; top: -2px" Width="90px" 
                                OnClick="btnSearch_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkShowSummary" runat="server" Text="Show Summary" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <asp:Panel ID="pnlKickbackUnitOverviewSummary" runat="server" Visible="false" >
        <asp:GridView 
            ID="gvKickbackUnitOverviewSummary"
            runat="server" 
            Width="200px"
            SkinID="custom-width"
            EnableViewState="False"
            DataKeyNames="Period"
            Caption="Kickback Fee Summary"
            DataSourceID="odsKickbackUnitOverviewSummary"
            AutoGenerateColumns="False"
            Visible="true">
            <Columns>
                <asp:BoundField DataField="Period" HeaderText="Period" >
                    <ItemStyle wrap="False" />
                    <HeaderStyle wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="TotalValue" HeaderText="Total Value" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="TotalFee" HeaderText="Total Fee" >
                    <ItemStyle horizontalalign="Right" wrap="False" />
                    <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                </asp:BoundField>
            </Columns>
            <EmptyDataTemplate>
                &nbsp;
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsKickbackUnitOverviewSummary" runat="server" SelectMethod="GetUnitFeeOverviewSummary"
            TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeOverviewAdapter" >
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierId" PropertyName="RemisierId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierEmployeeId" PropertyName="RemisierEmployeeId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ddlYear" Name="year" PropertyName="SelectedValue" 
                    Type="Int32" />
                <asp:ControlParameter ControlID="ddlQuarter" Name="quarter" PropertyName="SelectedValue" 
                    Type="Int32" />
                <asp:ControlParameter ControlID="ddlContinuationStatus" Name="continuationStatus" PropertyName="SelectedValue" 
                    Type="Int32" />
                <asp:Parameter Name="managementType" DefaultValue="2" Type="Int32" />
                <asp:Parameter Name="tradeStatus" DefaultValue="-1" Type="Int32" />
                <asp:Parameter Name="includeStornoedUnits" DefaultValue="false" Type="Boolean" />
            </SelectParameters>
        </asp:ObjectDataSource>
    <br />
    </asp:Panel>
    
    
    <asp:GridView 
        ID="gvKickbackUnitOverview"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="Key"
        Caption="Kickback Fee Overview"
        CaptionAlign="Left"
        DataSourceID="odsKickbackUnitOverview" 
        AllowSorting="True"
        Visible="false"
        OnRowCommand="gvKickbackUnitOverview_OnRowCommand"
        OnRowCreated="gvKickbackUnitOverview_RowCreated"
        OnRowDataBound="gvKickbackUnitOverview_RowDataBound"
        style="position: relative; top: -10px; z-index:2;">
        <Columns>
            <%-- <asp:BoundField DataField="Key" HeaderText="Key" SortExpression="Key">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField> --%> 
            <asp:TemplateField HeaderText="Remisier" SortExpression="Account_RemisierEmployee_Remisier_Name">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlRemisier" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="15"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "Account_RemisierEmployee_Remisier_Name") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Account_RemisierEmployee_Remisier_Name") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Employee" SortExpression="Account_RemisierEmployee_Employee_FullName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlRemisierEmployee" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="15"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "Account_RemisierEmployee_Employee_FullName") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Account_RemisierEmployee_Employee_FullName") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account#" SortExpression="Account_Number" >
                <ItemTemplate>
                    <uc3:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="false" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        ForeColor='<%# ((AccountStati)DataBinder.Eval(Container.DataItem, "Account_Status") == AccountStati.Inactive ? System.Drawing.Color.Gray : System.Drawing.Color.Black) %>' 
                        />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account Name" SortExpression="Account_ShortName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlAccountName" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="15"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>'
                        ForeColor='<%# ((AccountStati)DataBinder.Eval(Container.DataItem, "Account_Status") == AccountStati.Inactive ? System.Drawing.Color.Gray : System.Drawing.Color.Black) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Current Model" SortExpression="Account_ModelPortfolio_ShortName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlModel" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="25"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "Account_ModelPortfolio_ShortName") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Account_ModelPortfolio_ShortName") %>' 
                        ForeColor='<%# ((AccountStati)DataBinder.Eval(Container.DataItem, "Account_Status") == AccountStati.Inactive ? System.Drawing.Color.Gray : System.Drawing.Color.Black) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End Date" SortExpression="ManagementEndDate">
                <HeaderStyle HorizontalAlign="Left" Wrap="False" cssclass="alignleft" />
                <ItemStyle HorizontalAlign="Left" Wrap="False" cssclass="alignleft" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="lblLastRebalanceDate"
                        runat="server"
                        Width="35"
                        Text='<%# (DataBinder.Eval(Container.DataItem, "ManagementEndDate") == System.DBNull.Value || (DateTime)DataBinder.Eval(Container.DataItem, "ManagementEndDate") == DateTime.MinValue ? "" : Eval("ManagementEndDate", "{0:dd-MMM-yyyy}")) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField HeaderText="Exp." DataField="IsKickBackExported" SortExpression="IsKickBackExported" />
            <asp:BoundField DataField="MgtPeriod1_TotalValue_DisplayString" HeaderText="Avg Value 1" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="MgtPeriod1_FeeAmount_DisplayString" HeaderText="Fee 1" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" SortExpression="MgtPeriod1_HasMessage" >
                <ItemTemplate>
                    <asp:Image ID="imgWarning1" runat="server" 
                    ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                    Visible='<%# (DataBinder.Eval(Container.DataItem, "MgtPeriod1_HasMessage") == System.DBNull.Value ? false : DataBinder.Eval(Container.DataItem, "MgtPeriod1_HasMessage")) %>'
                    ToolTip ='<%# DataBinder.Eval(Container.DataItem, "MgtPeriod1_Message") %>'
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="MgtPeriod2_TotalValue_DisplayString" HeaderText="Avg Value 2" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="MgtPeriod2_FeeAmount_DisplayString" HeaderText="Fee 2" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" SortExpression="MgtPeriod2_HasMessage" >
                <ItemTemplate>
                    <asp:Image ID="imgWarning2" runat="server" 
                    ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                    Visible='<%# (DataBinder.Eval(Container.DataItem, "MgtPeriod2_HasMessage") == System.DBNull.Value ? false : DataBinder.Eval(Container.DataItem, "MgtPeriod2_HasMessage")) %>'
                    ToolTip ='<%# DataBinder.Eval(Container.DataItem, "MgtPeriod2_Message") %>'
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="MgtPeriod3_TotalValue_DisplayString" HeaderText="Avg Value 3" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="MgtPeriod3_FeeAmount_DisplayString" HeaderText="Fee 3" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" SortExpression="MgtPeriod3_HasMessage" >
                <ItemTemplate>
                    <asp:Image ID="imgWarning3" runat="server" 
                    ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                    Visible='<%# (DataBinder.Eval(Container.DataItem, "MgtPeriod3_HasMessage") == System.DBNull.Value ? false : DataBinder.Eval(Container.DataItem, "MgtPeriod3_HasMessage")) %>'
                    ToolTip ='<%# DataBinder.Eval(Container.DataItem, "MgtPeriod3_Message") %>'
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                         ID="lbtnViewDetails"
                         runat="server"
                         CommandName="ViewDetails"
                         Text="Details" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsKickbackUnitOverview" runat="server" SelectMethod="GetUnitsOverview"
        TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeOverviewAdapter" SortParameterName="sortColumn">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierId" PropertyName="RemisierId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierEmployeeId" PropertyName="RemisierEmployeeId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ddlYear" Name="year" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:ControlParameter ControlID="ddlQuarter" Name="quarter" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:ControlParameter ControlID="ddlContinuationStatus" Name="continuationStatus" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:Parameter Name="managementType" DefaultValue="2" Type="Int32" />
            <asp:Parameter Name="tradeStatus" DefaultValue="-1" Type="Int32" />
            <asp:Parameter Name="includeStornoedUnits" DefaultValue="false" Type="Boolean" />
            <asp:ControlParameter ControlID="gvKickbackUnitOverview" Name="maximumRows" PropertyName="PageSize" 
                Type="Int32" />
            <asp:ControlParameter ControlID="gvKickbackUnitOverview" Name="pageIndex" PropertyName="PageIndex" 
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False" Font-Bold="True"></asp:Label>
    <asp:Label ID="lblResult" runat="server" Font-Bold="True" ForeColor="Black" Visible="False"></asp:Label>
    <br />
    <asp:Button ID="btnExport" runat="server" Text="Export Kickback" OnClick="btnExport_Click" Visible="false" 
        OnClientClick="return confirm('Are you sure you want to export the kickback data?')" />
    <br />
    
    <asp:GridView 
        ID="gvManagementPeriodUnits"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="Key"
        DataSourceID="odsManagementPeriodUnits"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="20" 
        AllowSorting="True"
        Caption="Unit Details"
        Visible="False"
        OnDataBound="gvManagementPeriodUnits_DataBound" 
        onrowdatabound="gvManagementPeriodUnits_RowDataBound"
        OnRowCommand="gvManagementPeriodUnits_OnRowCommand">
        <Columns>
            <asp:BoundField DataField="UnitParent_Period" HeaderText="Period" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="UnitParent_StartDate" HeaderText="Start Date" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>            
            <asp:TemplateField HeaderText="End Date" >
                <HeaderStyle HorizontalAlign="Left" Wrap="False" cssclass="alignleft" />
                <ItemStyle HorizontalAlign="Left" Wrap="False" cssclass="alignleft" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="lblLastRebalanceDate"
                        runat="server"
                        Width="35"
                        Text='<%# (DataBinder.Eval(Container.DataItem, "UnitParent_EndDate") == System.DBNull.Value || (DateTime)DataBinder.Eval(Container.DataItem, "UnitParent_EndDate") == DateTime.MinValue ? "" : Eval("UnitParent_EndDate", "{0:dd-MMM-yyyy}")) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UnitParent_TotalValue_DisplayString" HeaderText="Total Value" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Model" >
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlModel" 
                        runat="server"
                        CssClass="alignright"
                        Width="25"
                        Text='<%# DataBinder.Eval(Container.DataItem, "ModelPortfolio_ModelName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Fee_1" HeaderText="Fee 1">
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Fee_2" HeaderText="Fee 2">
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Fee Calculated" >
                <ItemTemplate>
                    <%# (FeesCalculatedStates)DataBinder.Eval(Container.DataItem, "FeesCalculated")%>
                </ItemTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="RulesFound" HeaderText="Rules Found" >
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsStornoed" HeaderText="IsStornoed" >
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Message" >
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlMessage" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Message") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                         ID="lbtnViewAverageHoldings"
                         runat="server"
                         CommandName="ViewHoldings"
                         Text="Holdings" 
                         />
                    <asp:LinkButton
                         ID="lbtnRecalculate"
                         runat="server"
                         CommandName="Recalculate"
                         Visible='<%# DataBinder.Eval(Container.DataItem, "IsEditable") %>'
                         Text="Recalculate" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsManagementPeriodUnits" runat="server" 
        SelectMethod="GetManagementPeriodUnits"
        TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeOverviewAdapter" >
        <SelectParameters>
            <asp:ControlParameter ControlID="gvKickbackUnitOverview" Name="managementPeriodID" PropertyName="SelectedValue" Type="Int32" />
            <asp:SessionParameter SessionField="KickbackYear" Name="year" Type="Int32" />
            <asp:SessionParameter SessionField="KickbackQuarter" Name="quarter" Type="Int32" />
            <asp:Parameter Name="managementType" DefaultValue="2" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:Panel ID="pnlAverageHoldingFees" runat="server" Visible="false">
        <br />
        <asp:GridView 
            ID="gvAverageHoldingFees"
            runat="server" 
            EnableViewState="False"
            DataKeyNames="Key"
            DataSourceID="odsAverageHoldingFees"
            AutoGenerateColumns="False"
            AllowPaging="True"
            PageSize="20" 
            AllowSorting="True"
            Caption="Fees per average holdings"
            Visible="false"
            OnDataBound="gvAverageHoldingFees_DataBound" 
            onrowdatabound="gvAverageHoldingFees_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Period" HeaderText="Period"  >
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>            
                <asp:BoundField DataField="Instrument_Name" HeaderText="Instrument"  >
                    <ItemStyle Wrap="False" />
                    <HeaderStyle Wrap="False" />
                </asp:BoundField>            
                <asp:BoundField DataField="BeginDate" HeaderText="Start Date"  DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                </asp:BoundField>            
                <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                </asp:BoundField>            
                <asp:BoundField DataField="AverageValue_DisplayString" HeaderText="Avg Value" >
                    <HeaderStyle CssClass="alignright" Wrap="False" />
                    <ItemStyle HorizontalAlign="Right" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Fee_1" HeaderText="Fee 1" >
                    <HeaderStyle CssClass="alignleft" Wrap="False" />
                    <ItemStyle HorizontalAlign="Right" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Fee_2" HeaderText="Fee 2" >
                    <HeaderStyle CssClass="alignleft" Wrap="False" />
                    <ItemStyle HorizontalAlign="Right" Wrap="False" />
                </asp:BoundField>
               <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                </asp:BoundField>            
                <asp:TemplateField HeaderText="Message" >
                    <ItemTemplate>
                        <trunc:TruncLabel ID="trlMessage" 
                            runat="server"
                            Width="40"
                            Text='<%# DataBinder.Eval(Container.DataItem, "DisplayMessage") %>' 
                            />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" CssClass="alignleft" Wrap="False" />
                    <HeaderStyle CssClass="alignleft" Wrap="False" />
                </asp:TemplateField>
            </Columns>
            <SelectedRowStyle BackColor="Gainsboro" />
        </asp:GridView>
        <asp:ObjectDataSource ID="odsAverageHoldingFees" runat="server" 
            SelectMethod="GetAverageHoldingFees"
            TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeOverviewAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="gvManagementPeriodUnits" Name="unitID" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="managementType" DefaultValue="2" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>        
    <asp:Button ID="btnHideManagementPeriodUnits" runat="server" Text="Hide" OnClick="btnHideManagementPeriodUnits_Click" Visible="false" />&nbsp
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


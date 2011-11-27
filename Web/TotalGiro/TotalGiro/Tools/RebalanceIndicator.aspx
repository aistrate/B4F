<%@ Page Title="Rebalance Indicator" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="RebalanceIndicator.aspx.cs" Inherits="Tools_RebalanceIndicator" %>
<%@ Register Src="../UC/ModelFinder.ascx" TagName="ModelFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc2" %>
<%@ Register Src="../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblModelName" runat="server" Text="Model Name:"></asp:Label>
            </td>
            <td style="width: 190px">
                <asp:TextBox ID="txtModelName" runat="server"  />
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblDepositDate" runat="server" Text="Deposit Date:"></asp:Label>
            </td>
            <td style="width: 190px">
                <uc2:Calendar ID="ctlDepositDate" runat="server" MaximumDate='<%# DateTime.Today %>' IsButtonDeleteVisible="false" Format="dd-MM-yyyy" />
                <asp:RangeValidator
                    runat="server"
                    id="rvDepositDate"
                    controlToValidate="ctlDepositDate:txtCalendar"
                    Text="*"
                    errorMessage="Deposit Date should be before today" 
                    Type="Date" />
            </td>
            <td style="width: 70px">
                <asp:Label ID="lblEndDate" runat="server" Text="End Date:"></asp:Label>
            </td>
            <td style="width: 70px">
                <uc2:Calendar ID="ctlEndDate" runat="server" Format="dd-MM-yyyy" />
                <asp:RangeValidator
                    runat="server"
                    id="rvEndDate"
                    controlToValidate="ctlEndDate:txtCalendar"
                    Text="*"
                    errorMessage="End Date should be before today" 
                    Type="Date" />
                <asp:CompareValidator ID="cvCompareDates"
                    controlToValidate="ctlEndDate:txtCalendar"
                    ControlToCompare="ctlDepositDate:txtCalendar"
                    Operator="GreaterThan"
                    Type="Date"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="End Date should be after the Deposit date" />
            </td>
        </tr>
        <tr>
            <td style="width: 130px; height: 24px">
                <asp:Label ID="lblStartBalance" runat="server" Text="Start Balance:"></asp:Label>
            </td>
            <td style="width: 190px">
                <db:DecimalBox ID="dbStartBalance" runat="server" Value="100000" DecimalPlaces="2" Width="50px" />
                <asp:Label ID="lblStartBalanceCurrency" runat="server" Text="€"></asp:Label>
                <asp:RequiredFieldValidator ID="rfvStartBalance"
                    controlToValidate="dbStartBalance:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Start Balance is mandatory" />
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td style="height: 24px">
                <asp:Label ID="lblMaxDeviation" runat="server" Text="Max. Deviation:"></asp:Label>
            </td>
            <td>
                <db:DecimalBox ID="dbMaxDeviation" runat="server" DecimalPlaces="2" Width="50px" />
                <asp:Label ID="lblMaxDeviationSuffix" runat="server" Text="%"></asp:Label>
                <asp:RequiredFieldValidator ID="rfvMaxDeviation"
                    controlToValidate="dbMaxDeviation:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Max Deviation is mandatory" />
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:CheckBox ID="chkIncludeInactiveModels" runat="server" Checked="false" Text="Incl. Inactive Models" />
            </td>
            <td colspan="2" ></td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:CheckBox ID="chkIncludeModelChanges" runat="server" Checked="true" Text="Incl. Model Changes" />
            </td>
            <td>
                <asp:Button ID="btnCalculate" runat="server" OnClick="btnCalculate_Click" Text="Calculate" 
                    style="position: relative; top: -2px" Width="90px" />
            </td>
            <td></td>
        </tr>
    </table>
    <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
    <asp:ValidationSummary DisplayMode="BulletList" HeaderText="The rebalance indication could not be calculated:"
        ID="valSum" runat="server" />
    <asp:GridView ID="gvOverview" runat="server" AutoGenerateColumns="false" Caption="Overview"
        CaptionAlign="Top" PageSize="10" AllowPaging="True"
        DataKeyNames="Key" AllowSorting="True" SkinID="custom-EmptyDataTemplate" Width="75%"
        OnSelectedIndexChanged="gvOverview_SelectedIndexChanged"
        OnSorting="gvOverview_Sorting" OnPageIndexChanging="gvOverview_PageIndexChanging" >
        <SelectedRowStyle BackColor="Gainsboro" />
        <Columns>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:Image ID="imgWarning1" runat="server" 
                    ImageUrl='<%# ((byte)DataBinder.Eval(Container.DataItem, "WarningLevel") == 2 ? "~/layout/images/images_ComponentArt/pager/priority_high.gif" : "~/layout/images/images_ComponentArt/pager/priority_medium.GIF") %>'
                    Visible='<%# ((byte)DataBinder.Eval(Container.DataItem, "WarningLevel") == 0 ? false : true) %>'
                    ToolTip='<%# ((byte)DataBinder.Eval(Container.DataItem, "WarningLevel") == 2 ? "Warning level high" : "Warning level medium") %>'
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Model" SortExpression="ModelName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblModelName" runat="server" Width="75px" CssClass="padding"
                        MaxLength="50" LongText='<%# DataBinder.Eval(Container.DataItem, "ModelName") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField HeaderText="Total Amount" DataField="TotalAmount"
                SortExpression="TotalAmountQuantity">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <%--<asp:BoundField HeaderText="WarningLevel" DataField="WarningLevel"
                SortExpression="WarningLevel">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>--%>
            <asp:CommandField ShowSelectButton="True" SelectText="View Details" >
                <ItemStyle Wrap="False" Width="82px" />
            </asp:CommandField>
        </Columns>
    </asp:GridView>

    <br/>
    <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="false" Caption="Details per Model"
        CaptionAlign="Top" PageSize="10" AllowPaging="True"
        DataKeyNames="Key" AllowSorting="True" SkinID="custom-EmptyDataTemplate" Width="100%" >
        <SelectedRowStyle BackColor="Gainsboro" />
        <Columns>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:Image ID="imgWarning1" runat="server" 
                    ImageUrl='<%# ((byte)DataBinder.Eval(Container.DataItem, "WarningLevel") == 2 ? "~/layout/images/images_ComponentArt/pager/priority_high.gif" : "~/layout/images/images_ComponentArt/pager/priority_medium.GIF") %>'
                    Visible='<%# ((byte)DataBinder.Eval(Container.DataItem, "WarningLevel") == 0 ? false : true) %>'
                    ToolTip='<%# ((byte)DataBinder.Eval(Container.DataItem, "WarningLevel") == 2 ? "Warning level high" : "Warning level medium") %>'
                    />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField HeaderText="Instrument" DataField="InstrumentName" SortExpression="InstrumentName">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Size" DataField="Size" SortExpression="SizeQuantity">
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Amount" DataField="NewAmount"
                SortExpression="NewAmountQuantity">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Buy Price" DataField="OldPrice"
                SortExpression="OldPriceQuantity">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Last Price" DataField="LastPrice"
                SortExpression="LastPriceQuantity">
                    <ItemStyle Wrap="False" HorizontalAlign="Right" />
                    <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField DataField="LastPriceDate" HeaderText="Last Price Date" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" 
                SortExpression="LastPriceDate" > 
                <ItemStyle wrap="False" horizontalalign="Right" />
                <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Actual" DataField="ActualAllocation" SortExpression="ActualAllocation"
                DataFormatString="{0:##0.00###}%" >
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>
            <asp:BoundField HeaderText="Model" DataField="ModelAllocation" SortExpression="ModelAllocation"
                DataFormatString="{0:##0.00###}%" >
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
            </asp:BoundField>            
            <%--<asp:BoundField HeaderText="WarningLevel" DataField="WarningLevel"
                SortExpression="WarningLevel">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>--%>
        </Columns>
    </asp:GridView>
    
</asp:Content>


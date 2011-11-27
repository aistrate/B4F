<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="CalculationsOverview.aspx.cs" Inherits="CalculationsOverview" Title="Calculations Overview" %>
<%@ Import Namespace="B4F.TotalGiro.Fees" %>
<%@ Import Namespace="B4F.TotalGiro.Fees.CommCalculations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table cellpadding="0" cellspacing="0" border="0"  style="border-color:black" width="780">
        <tr>
            <td class="tblHeader">Commission Calculations Overview</td>
        </tr>
        <tr>
            <td>
                <table border="1" style="width:100%">
                    <tr>
                        <td style="width: 120px; height: 35px;" align="left">
                            <asp:Label ID="lblCalculationName" runat="server" Text="Calculation Name"></asp:Label>
                        </td>
                        <td style="width: 420px; height: 35px;" align="left" >
                            <asp:TextBox ID="txtCalculationName" runat="server" Width="194px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="height: 37px; width: 230px;" colspan="4">
                <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Text="Refresh" />
            </td>
        </tr>
    </table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>

    <asp:GridView 
        ID="gvCalcOverview"
        Width="200px" 
        runat="server" 
        DataKeyNames="Key" 
        DataSourceID="odsCommissionCalcs" 
        AutoGenerateColumns="False"
        AllowSorting="true"
        OnRowCommand="gvCalcOverview_RowCommand"
        OnPageIndexChanged="gvCalcOverview_PageIndexChanged"
        OnSelectedIndexChanged="gvCalcOverview_SelectedIndexChanged"        
        PageSize="20"
        AllowPaging="True">
        <SelectedRowStyle BackColor="Gainsboro" />
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="MinValue_DisplayString" HeaderText="Minimum" SortExpression="MinValue_DisplayString" />
            <asp:BoundField DataField="MaxValue_DisplayString" HeaderText="Maximum" SortExpression="MaxValue_DisplayString" />
            <asp:BoundField DataField="FixedSetup_DisplayString" HeaderText="Setup" SortExpression="FixedSetup_DisplayString" />
            <asp:TemplateField HeaderText="Type" SortExpression="CalcType">
                <ItemTemplate>
                    <%# (FeeCalcTypes)DataBinder.Eval(Container.DataItem, "CalcType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton 
                        ID="lbtnLines" runat="server" 
                        Text="View Lines"
                        Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "CalcType") != 3 ? true : false) %>'
                        CommandName="Select" />
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtEdit" 
                        Text="Edit" 
                        CommandName="EditCalc"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDelete" 
                        Text="Delete"
                        CommandName="DeleteCalc"
                        OnClientClick="return confirm('Delete rule?')"/>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCommissionCalcs" runat="server" SelectMethod="GetCommissionCalculationsOverview" 
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.CalculationsOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtCalculationName" Name="calcname"  PropertyName="Text" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
    <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add new" />
    <br />
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
            <asp:BoundField HeaderText="Fee" DataField="Fee" SortExpression="FeeQuantity" >
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField HeaderText="StaticCharge" DataField="StaticCharge" SortExpression="StaticCharge" DataFormatString="{0:###,##0.00}" HtmlEncode="false" >
                <ItemStyle Wrap="False" HorizontalAlign="Right" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCalcLines" runat="server" SelectMethod="GetCommCalcLines"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.CalculationsOverviewAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvCalcOverview" Name="calcID" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:Parameter  Name="propertyList" DefaultValue="Key, SerialNo, DisplayRange, FeePercentage, StaticCharge" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>    

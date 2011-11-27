<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="OrderDetails.aspx.cs" Inherits="Orders_Common_OrderDetails" Title="Order Details" Theme="Neutral" %>

<%@ Register Src="~/UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Import Namespace="B4F.TotalGiro.Utils" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <uc1:BackButton ID="BackButton1" runat="server" />
    <asp:Button ID="Button1" runat="server" OnClick="btnAuditLogDetails_Click"
        Text="Audit Log Details..." />
    <br /><br />
    <asp:DetailsView ID="dvOrder" runat="server" AutoGenerateRows="False" Caption="Order"
        CaptionAlign="Left" DataSourceID="odsOrder" Width="125px" DataKeyNames="ClassName,Key"
        OnItemCommand="dvOrder_ItemCommand">
        <Fields>
            <asp:BoundField DataField="ClassName" HeaderText="Type">
                <ItemStyle Wrap="False" BackColor="Gainsboro" />
                <HeaderStyle BackColor="LightSteelBlue" />
            </asp:BoundField>
            <asp:BoundField DataField="Key" HeaderText="OrderID">
                <ItemStyle Wrap="False" BackColor="Gainsboro" />
                <HeaderStyle BackColor="LightSteelBlue" />
            </asp:BoundField>
            <asp:BoundField DataField="AccountNumber" HeaderText="Account Number">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="AccountShortName" HeaderText="Account Name">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Action Type">
                <ItemTemplate>
                    <%# (OrderActionTypes)DataBinder.Eval(Container.DataItem, "ActionType") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Allocation Date">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "AllocationDate")) %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Amount" HeaderText="Amount">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Approved" HeaderText="Approved">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Cancel Status">
                <ItemTemplate>
                    <%# (OrderCancelStati)DataBinder.Eval(Container.DataItem, "CancelStatus")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Commission" HeaderText="Commission">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CommissionInfo" HeaderText="Commission Info">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ServiceCharge" HeaderText="Service Charge">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Creation Date">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")) %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date Closed">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "DateClosed"))%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Error">
                <ItemTemplate>
                    <%# (OrderErrors)DataBinder.Eval(Container.DataItem, "Err")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="ErrDescription" HeaderText="Error Description">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="EstimatedAmount" HeaderText="Estimated Amount">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ExRate" HeaderText="Exchange Rate">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="FilledValue" HeaderText="Filled Value">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="GrossAmount" HeaderText="Gross Amount">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsAggregateOrder" HeaderText="Is Aggregate">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsClosed" HeaderText="Is Closed">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsCompleteFilled" HeaderText="Is Completely Filled">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Is Fillable">
                <ItemTemplate>
                    <%# (OrderFillability)DataBinder.Eval(Container.DataItem, "IsFillable")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="IsMonetary" HeaderText="Is Monetary">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsNetted" HeaderText="Is Netted">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsSecurity" HeaderText="Is Security">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsStgOrder" HeaderText="Is Stichting">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="IsUnApproveable" HeaderText="Is Unapproveable">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Last Updated">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "LastUpdated"))%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="OpenAmount" HeaderText="Open Amount">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="OpenValue" HeaderText="Open Value">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="OrderCurrency" HeaderText="Order Currency">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Order Type">
                <ItemTemplate>
                    <%# (OrderTypes)DataBinder.Eval(Container.DataItem, "OrderType")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="PlacedValue" HeaderText="Placed Value">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Price" HeaderText="Price">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="RequestedInstrument" HeaderText="Requested Instrument">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side">
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <%# (OrderStati)DataBinder.Eval(Container.DataItem, "Status")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TopParentDisplayStatus" HeaderText="Top Parent Status">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Top Parent">
                <ItemTemplate>
                    <%# (IOrder)DataBinder.Eval(Container.DataItem, "TopParentOrder") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:LinkButton 
                        ID="lbtnOrder" runat="server" 
                        Text="Show Parent" 
                        Visible='<%# (DataBinder.Eval(Container.DataItem, "ParentOrder_Key") == System.DBNull.Value || (Int32)DataBinder.Eval(Container.DataItem, "ParentOrder_Key") == 0 ? false : true) %>'
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ParentOrder_Key") %>'
                        />
                </ItemTemplate>                   
            </asp:TemplateField>
        </Fields>
        <FieldHeaderStyle Wrap="False" Height="14px" />
    </asp:DetailsView>
    <asp:ObjectDataSource ID="odsOrder" runat="server" SelectMethod="GetOrderDetails"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Common.OrderDetailsAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="orderId" SessionField="OrderId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


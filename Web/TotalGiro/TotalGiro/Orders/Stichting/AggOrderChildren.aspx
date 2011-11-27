<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AggOrderChildren.aspx.cs" Inherits="AggOrderChildren" Title="View Children Aggregated Order" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:GridView 
        ID="gvAggregatedChildOrders"
        Caption="Details Aggregate Order"
        CaptionAlign="Left"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="OrderID"
        OnRowCommand="gvAggregatedChildOrders_RowCommand" 
        DataSourceID="odsAggOrderChildren"
        AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" SortExpression="Key" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="Account_Number" HeaderText="Account No." SortExpression="Account_Number" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account Name" SortExpression="Account_ShortName">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel1" 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="RequestedInstrument_DisplayName">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel2" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "RequestedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
                <ItemStyle wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value" SortExpression="Value">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel3" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Value_DisplayString") %>' 
                        />
                </ItemTemplate>
                <ItemStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                <HeaderStyle cssclass="alignright" />
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAggOrderChildren" runat="server" 
        SelectMethod="GetAggregatedChildOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.AggOrderChildrenAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="parentOrderId" SessionField="StgAggOrderEditID"
                Type="Int32"  />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <uc1:BackButton ID="ctlBackButton" runat="server" />
    <br />
    <br />
</asp:Content>


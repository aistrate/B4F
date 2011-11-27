<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AggOrderChildren.aspx.cs" Inherits="AggOrderChildren" Title="View Children Aggregated Order" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:GridView 
        ID="gvAggregatedChildOrders"
        Caption="Details Aggregate Order"
        CaptionAlign="Left"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="OrderID"
        DataSourceID="odsAggOrderChildren"
        AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="Account_Number" HeaderText="Account Number">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Account Name">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel1" 
                        runat="server"
                        CssClass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Instrument">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel2" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "RequestedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side">
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value">
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel3" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Value_DisplayString") %>' 
                        />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
                <HeaderStyle CssClass="alignright" />
            </asp:TemplateField>
            <asp:BoundField DataField="Commission_DisplayString" HeaderText="Commission">
                <HeaderStyle CssClass="alignright" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAggOrderChildren" runat="server" 
        SelectMethod="GetAggregatedChildOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.AggOrderChildrenAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="parentOrderId" SessionField="AggOrderEditID"
                Type="Int32"  />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <uc1:BackButton ID="ctlBackButton" runat="server" />
    <br />
    <br />
</asp:Content>


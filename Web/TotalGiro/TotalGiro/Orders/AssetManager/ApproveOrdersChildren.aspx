<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ApproveOrdersChildren.aspx.cs" Inherits="ApproveOrdersChildren"  %>

<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Register Src="../../UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:GridView 
        ID="gvApproveOrdersChildren"
        Caption="Approve Orders Children"
        CaptionAlign="Left"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="OrderID"
        OnRowCommand="gvApproveOrdersChildren_OnRowCommand"
        DataSourceID="odsApproveOrdersChildren"
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
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
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
            <asp:BoundField DataField="DisplayStatus" HeaderText="Status">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayInstructionKey" HeaderText="Instruction">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False">
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
	            <ItemTemplate>
		            <asp:LinkButton ID="lbtnCancel" runat="server" CommandName="Cancel" Text="Cancel" 
		                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
			            OnClientClick="return confirm('Are you sure you want to cancel this order?');" />
	            </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsApproveOrdersChildren" runat="server" 
        SelectMethod="GetOrdersByAccount"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.ApproveOrdersChildrenAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="accountId" SessionField="ApproveOrdersSelectedID"
                Type="Int32"  />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <uc1:BackButton ID="ctlBackButton" runat="server" />
    <br />
    <br />
</asp:Content>


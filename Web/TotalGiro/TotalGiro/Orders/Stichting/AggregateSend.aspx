<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="AggregateSend.aspx.cs" Inherits="AggregateSend" %>

<%@ Import Namespace="System" %>
<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="smAggregateSend" runat="server" />
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvApprovedOrders" 
        runat="server" 
        CellPadding="0"
        AutoGenerateColumns="False"
        DataSourceID="odsApprovedOrders"
        DataKeyNames="Key"
        cssclass="padding"
        AllowPaging="True"
        Caption="Approved/New Orders" 
        CaptionAlign="Left"
        PageSize="10" 
        AllowSorting="True">
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
                    <trunc:TruncLabel 
                        runat="server"
                        cssclass="alignright"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' 
                        />
                </ItemTemplate>
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrument_DisplayName">
                <ItemTemplate>
                    <trunc:TruncLabel 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
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
                    <trunc:TruncLabel 
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
    </cc1:MultipleSelectionGridView>
    <asp:Label ID="lblErrorAggregate" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Button ID="btnAggregate" runat="server" OnClick="btnAggregate_Click" Text="Aggregate on Stichting" Width="190px" />&nbsp;
    <asp:Button ID="btnAggregateSpecial" runat="server" OnClick="btnAggregateSpecial_Click" Text="Aggregate Special" />&nbsp;
    <asp:Button ID="btnUnApprove" runat="server" OnClick="btnUnApprove_Click" Text="Unapprove" 
        OnClientClick="return confirm('Unapprove orders?')" />
    <asp:ObjectDataSource ID="odsApprovedOrders" runat="server" SelectMethod="GetStichtingUnAggregatedChildOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.AggregateSendAdapter"></asp:ObjectDataSource>
    <br />
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvAggregatedOrders"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="10" 
        AutoGenerateColumns="False"
        DataKeyNames="OrderID"
        OnRowCommand="gvAggregatedOrders_OnRowCommand"
        Caption="Aggregated Orders"
        CaptionAlign="Left"
        DataSourceID="odsAggregatedOrders"
        AllowSorting="True"
        SelectionBoxEnabledBy="IsSendable">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="OrderID" SortExpression="Key">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Right" wrap="False" />
                <HeaderStyle cssclass="alignright" wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="ChildOrders_Count" HeaderText="Child Orders" SortExpression="ChildOrders_Count">
                <ItemStyle cssclass="bigrightpadding" horizontalalign="Right" wrap="False" />
                <HeaderStyle cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Instrument" SortExpression="TradedInstrument_DisplayName">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel2" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "TradedInstrument_DisplayName") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Side" SortExpression="Side">
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (Side)DataBinder.Eval(Container.DataItem, "Side") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value" SortExpression="PlacedValue_Quantity">
                <ItemStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
                <HeaderStyle cssclass="alignright" />
                <ItemTemplate>
                    <trunc:TruncLabel ID="TruncLabel3" 
                        runat="server"
                        Width="20"
                        Text='<%# DataBinder.Eval(Container.DataItem, "PlacedValue_DisplayString") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayIsSizeBased" HeaderText="Type" SortExpression="DisplayIsSizeBased" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Route_Name" HeaderText="Route" SortExpression="Route_Name">
                <itemstyle wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:d MMMM yyyy}" HtmlEncode="False" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtViewChildren" 
                        Text="View" 
                        CommandName="ViewChildren"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDeAggregate" 
                        Text="De-Aggregate"
                        CommandName="DeAggregate"
                        OnClientClick="return confirm('De-Aggregate order?')"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtConvert" 
                        Text="Convert" 
                        CommandName="ConvertOrder"/>
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtConvertFX" 
                        Text="Convert FX" 
                        Visible='<%# DataBinder.Eval(Container.DataItem, "NeedsCurrencyConversion") %>' 
                        CommandName="ConvertFX"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cc1:MultipleSelectionGridView>
    <asp:Label ID="lblErrorAggregatedOrders" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:Button ID="btnNett" runat="server" Text="Nett" OnClick="btnNett_Click" />&nbsp;
    <asp:Button ID="btnSend" runat="server" Text="Send to Exchange" OnClick="btnSend_Click" />&nbsp;
    <asp:ObjectDataSource ID="odsAggregatedOrders" runat="server" SelectMethod="GetAggregatedStgOrders"
        TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.AggregateSendAdapter">
    </asp:ObjectDataSource>
    <br />
    <asp:MultiView ID="mvwOrderEditting" runat="server" ActiveViewIndex="0" Visible="false" >
        <asp:View ID="vwOrderEdit" runat="server">
            <asp:DetailsView ID="dvOrderEdit" runat="server" AutoGenerateRows="False"
            Caption="Edit Order" CaptionAlign="Left" DataKeyNames="OrderID" DataSourceID="odsDetailsOrderEdit"
            Width="350px"
            OnItemCommand="dvOrderEdit_ItemCommand"
            OnDataBound="dvOrderEdit_DataBound" >
                <Fields>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblOrderID" runat="server">OrderID</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="OrderID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderID") %>'>
                            </asp:Label>
                            <asp:HiddenField ID="hdnIsBondOrder" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IsBondOrder") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblPrice" runat="server">Price</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <db:DecimalBox ID="dbPrice" runat="server" AutoPostBack="true" 
                                DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "NumberOfDecimals") %>'
                                Value='<%# DataBinder.Eval(Container.DataItem, "Price") %>' />
                            <asp:Label ID="lblPriceCur" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PriceSymbol") %>'
                                Width='18'></asp:Label>
                            <asp:RequiredFieldValidator ID="reqValPrice" runat="server" ControlToValidate="dbPrice:tbDecimal"
                                ErrorMessage="Price is an obligatory field" Text="*"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblTransactionDate" runat="server">Transaction Date</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <uc1:DatePicker ID="dpTransactionDate" runat="server"
                                SelectedDate='<%# (DateTime)System.DateTime.Today %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblExpectedSettlementDate" runat="server">Settlement Date</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <uc1:DatePicker ID="dpExpectedSettlementDate" runat="server" 
                                SelectedDate='<%# (DateTime)DataBinder.Eval(Container.DataItem, "ExpectedSettlementDate") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtEOSaveOrder" runat="server" CommandName="EOSaveOrder" Enabled="true"
                                Text="OK" Visible="true"></asp:LinkButton>
                            <asp:LinkButton ID="lbtEOCancelEdit" runat="server" CausesValidation="false" CommandName="EOCancelEdit"
                                Text="Cancel" Visible="true"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsDetailsOrderEdit" runat="server" SelectMethod="GetOrderEditData"
                TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.AggregateSendAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvAggregatedOrders" Name="orderId" PropertyName="SelectedValue" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="vwOrderFXConvert" runat="server">
            <asp:DetailsView ID="dvOrderFXConvert" runat="server" AutoGenerateRows="False"
                Caption="Convert Order FX" CaptionAlign="Left" DataKeyNames="OrderID" DataSourceID="odsOrderFXConvert"
                OnItemCommand="dvOrderFXConvert_ItemCommand" Width="350px" >
                <Fields>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblOrderID" runat="server">OrderID</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="OrderID" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderID") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblExRate" runat="server">ExRate</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <db:DecimalBox ID="dbExRate" runat="server" AutoPostBack="true" 
                                Value='<%# DataBinder.Eval(Container.DataItem, "ExRate") %>' 
                                DecimalPlaces="5" />
                            <asp:RequiredFieldValidator ID="reqExRate" runat="server" ControlToValidate="dbExRate:tbDecimal"
                                ErrorMessage="ExRate is an obligatory field" Text="*"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblConvertedAmount" runat="server">Converted Amount</asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <db:DecimalBox ID="dbConvertedAmount" runat="server" AutoPostBack="true" 
                                Value='<%# DataBinder.Eval(Container.DataItem, "ConvertedAmount") %>' 
                                DecimalPlaces="2" />
                            <asp:Label ID="lblAmountCur" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AmountSymbol") %>'
                                Width='18'></asp:Label>
                            <asp:RequiredFieldValidator ID="reqValConvertedAmount" runat="server" ControlToValidate="dbConvertedAmount:tbDecimal"
                                ErrorMessage="Converted Amount is an obligatory field" Text="*"></asp:RequiredFieldValidator>
                            <asp:HiddenField ID="hdfOriginalAmount" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "OriginalAmount") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtFxSaveOrder" runat="server" CommandName="FXSaveOrder" Enabled="true"
                                Text="OK" Visible="true"></asp:LinkButton>
                            <asp:LinkButton ID="lbtFxCancelEdit" runat="server" CausesValidation="false" CommandName="FXCancelEdit"
                                Text="Cancel" Visible="true"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsOrderFXConvert" runat="server" SelectMethod="GetOrderFxConvertData"
                TypeName="B4F.TotalGiro.ApplicationLayer.Orders.Stichting.AggregateSendAdapter">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvAggregatedOrders" Name="orderId" PropertyName="SelectedValue" />
                </SelectParameters>
            </asp:ObjectDataSource>

        </asp:View>
    </asp:MultiView>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


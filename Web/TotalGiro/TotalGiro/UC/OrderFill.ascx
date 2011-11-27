<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderFill.ascx.cs" Inherits="OrderFill"
            TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register Src="DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>

<asp:ScriptManagerProxy ID="smOrderFill" runat="server"  />
<b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="550px"></b4f:ErrorLabel>
<div class="padding" style="display:block">
    <asp:ValidationSummary ID="vsOrderFill" runat="server" ForeColor="Red" Height="0px" Width="450px"/>
</div>
<br />
<br />
<br />
<asp:DetailsView 
    ID="dvOrderFill" 
    runat="server" 
    DataSourceID="odsDetailsOrderFill" 
    AutoGenerateRows="False"
    DataKeyNames="OrderID"
    Caption="Fill Order" 
    CaptionAlign="Left"
    OnItemCommand="dvOrderFill_ItemCommand" OnItemUpdating="dvOrderFill_ItemUpdating" DefaultMode="Edit" 
    OnItemUpdated="dvOrderFill_ItemUpdated" OnDataBound="dvOrderFill_DataBound" > 
    <Fields>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblOrderIdHeader" runat="server">OrderID</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblOrderId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrderID") %>'>
                </asp:Label>
                <asp:HiddenField ID="hdnIsSizeBased" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IsSizeBased") %>' />
            </ItemTemplate>                
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblExchange" runat="server">Exchange</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:DropDownList ID="ddlExchange" runat="server" Width="165px" DataSourceID="odsExchange" 
                    DataTextField="ExchangeName" DataValueField="Key" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsExchange" runat="server" SelectMethod="GetExchanges"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.InstrumentFinderAdapter"></asp:ObjectDataSource>
                <asp:RequiredFieldValidator ID="rfvExchange" runat="server" ControlToValidate="ddlExchange"
                        Text="*" ErrorMessage="Exchange is mandatory" InitialValue="-2147483648" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblTransactionDate" runat="server">Date</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <table style="margin: 0px 0px 0px -3px">
                    <tr>
                        <td>
                            <uc1:DatePicker ID="dpTransactionDate" runat="server" IsButtonDeleteVisible="false" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvTransactionDate"
                                ControlToValidate="dpTransactionDate:txtDate"
                                runat="server"
                                Text="*"
                                ErrorMessage="Date is mandatory" />
                            <asp:RangeValidator
                                runat="server"
                                id="rvTransactionDate"
                                controlToValidate="dpTransactionDate:txtDate"
                                Text="*"
                                errorMessage="Date should be no later than today" 
                                Type="Date"
                                MinimumValue='<%# DateTime.MinValue.ToShortDateString() %>'
                                MaximumValue='<%# DateTime.Today.ToShortDateString() %>' />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblSettlementDate" runat="server">Settlement Date</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <table style="margin: 0px 0px 0px -3px">
                    <tr>
                        <td>
                            <uc1:DatePicker ID="dpSettlementDate" runat="server" IsButtonDeleteVisible="false" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvSettlementDate"
                                ControlToValidate="dpSettlementDate:txtDate"
                                runat="server"
                                Text="*"
                                ErrorMessage="Settlement Date is mandatory" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblTransactionTime" runat="server">Time</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <table style="margin: 0px 0px 0px -3px">
                    <tr>
                        <td>
                            <b4f:TimePicker ID="tpTime" runat="server" SelectedHour="10" SelectedMinute="0" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvTransactionTimeHour"
                                ControlToValidate="tpTime:HOUR"
                                runat="server"
                                Text="*"
                                ErrorMessage="Transaction Time (hour) is mandatory" />
                            <asp:RequiredFieldValidator ID="rfvTransactionTimeMinute"
                                ControlToValidate="tpTime:MINUTE"
                                runat="server"
                                Text="*"
                                ErrorMessage="Transaction Time (minute) is mandatory" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblExRate" runat="server">ExRate</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <db:DecimalBox ID="dbExRate" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces="5"
                    Value='<%# DataBinder.Eval(Container.DataItem, "ExchangeRate") %>'
                    Width="145px" 
                    AlignLeft="true" />
                <asp:RequiredFieldValidator ID="rfvExRate"
                    ControlToValidate="dbExRate:tbDecimal"
                    runat="server"
                    Text="*"
                    ErrorMessage="Exchange Rate is mandatory" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblPrice" runat="server">Price</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <db:DecimalBox ID="dbPrice" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces="5"
                    Width="145px" 
                    AlignLeft="true" />
                <asp:Label ID="lblPriceCurrency" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PriceSymbol") %>' />
                <asp:RequiredFieldValidator ID="rfvPrice"
                    ControlToValidate="dbPrice:tbDecimal"
                    runat="server"
                    Text="*"
                    ErrorMessage="Price is mandatory" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblSize" runat="server">Size</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <db:DecimalBox ID="dbSize" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "SizeDecimals") %>'
                    Text='<%# ((bool)DataBinder.Eval(Container.DataItem, "IsSizeBased") ? DataBinder.Eval(Container.DataItem, "DisplaySize") : "") %>'
                    Width="145px" 
                    AlignLeft="true" />
                <asp:Label ID="lblSizeCurrency" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SizeSymbol") %>' />
                <asp:RequiredFieldValidator
                    ID="rfvSize"
                    ControlToValidate="dbSize:tbDecimal"
                    SetFocusOnError="true"
                    runat="server"
                    Text="*"
                    ErrorMessage="Size is mandatory" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblAmount" runat="server">Amount</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <db:DecimalBox ID="dbAmount" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "AmountDecimals") %>'
                    Text='<%# ((bool)DataBinder.Eval(Container.DataItem, "IsAmountBased") ? DataBinder.Eval(Container.DataItem, "DisplayAmount") : "") %>'
                    Width="145px" 
                    AlignLeft="true" />
                <asp:Label ID="lblAmountCurrency" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AmountSymbol") %>' />
                <asp:RequiredFieldValidator
                    ID="rfvAmount"
                    ControlToValidate="dbAmount:tbDecimal"
                    runat="server"
                    Text="*"
                    ErrorMessage="Amount is mandatory" />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField >
            <HeaderTemplate>
                <asp:Label ID="lblAccruedInterest" runat="server">Accrued Interest</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <db:DecimalBox ID="dbAccruedInterest" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "AmountDecimals") %>'
                    Text='<%# ((bool)DataBinder.Eval(Container.DataItem, "IsSizeBased") ? DataBinder.Eval(Container.DataItem, "DisplayAccruedInterestAmount") : "") %>'
                    Width="145px" 
                    AlignLeft="true" />
                <asp:Label ID="lblAccruedInterestCurrency" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AmountSymbol") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField >
            <HeaderTemplate>
                <asp:Label ID="lblServiceCharge" runat="server">Service Charge</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HiddenField ID="hdnInitialServiceChargePercentage" runat="server" 
                    Value='<%# DataBinder.Eval(Container.DataItem, "DisplayServiceChargePercentage") %>' />
                <db:DecimalBox ID="dbServiceChargePercentage" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces=4
                    Text='<%# DataBinder.Eval(Container.DataItem, "DisplayServiceChargePercentage") %>'
                    ToolTip='<%# DataBinder.Eval(Container.DataItem, "ServiceChargeDisplayInfo") %>'
                    Width="45px" 
                    Enabled="false" 
                    AlignLeft="true" />
                <asp:Label ID="lblServiceChargePercentSign" SkinID="custom-width" Width='8' runat="server" Text="%" />
                <db:DecimalBox ID="dbServiceChargeAmount" 
                    runat="server"
                    AutoPostBack="true" 
                    DecimalPlaces='<%# DataBinder.Eval(Container.DataItem, "AmountDecimals") %>'
                    Text='<%# DataBinder.Eval(Container.DataItem, "DisplayServiceChargeAmount") %>'
                    ToolTip='<%# DataBinder.Eval(Container.DataItem, "ServiceChargeDisplayInfo") %>'
                    Width="74px" 
                    Enabled='<%# (string)DataBinder.Eval(Container.DataItem, "DisplayServiceChargePercentage") != string.Empty %>'
                    AlignLeft="true" />
                <asp:Label ID="lblServiceChargeCurrency" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AmountSymbol") %>' />
<%--                <asp:RequiredFieldValidator ID="rfvServiceChargePercentage"
                    ControlToValidate="dbServiceChargePercentage:tbDecimal"
                    runat="server"
                    Text="*"
                    ErrorMessage="Service Charge Percentage is mandatory" />--%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblCounterparty" runat="server">Counterparty</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:DropDownList ID="ddlCounterpartyAccount" runat="server" Width="155px" DataSourceID="odsCounterpartyAccount" 
                                  DataTextField="DisplayNumberWithName" DataValueField="Key" AutoPostBack="False" >
                </asp:DropDownList>&nbsp;
                <asp:CheckBox ID="chkUseNostro" runat="server" Text="Nostro" Checked="false" 
                              AutoPostBack="true" />
                <asp:ObjectDataSource ID="odsCounterpartyAccount" runat="server" SelectMethod="GetCounterparties" 
                                      TypeName="B4F.TotalGiro.ApplicationLayer.UC.OrderFillAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="chkUseNostro" Name="nostro" PropertyName="Checked" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Label ID="lblIsCompleteFill" runat="server">Complete Fill?</asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkIsCompleteFill" runat="server" Checked="false" Enabled="false"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <EditItemTemplate>
                <asp:LinkButton ID="lbtOk" runat="server" CausesValidation="True" CommandName="Update"
                    Text="OK"></asp:LinkButton>
                <asp:LinkButton ID="lbtCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                    Text="Cancel"></asp:LinkButton>
            </EditItemTemplate>
        </asp:TemplateField>
    </Fields>
</asp:DetailsView>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransferPositionDetailsEditor.ascx.cs"
    Inherits="TransferPositionDetailsEditor" %>
<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<asp:HiddenField ID="hdnTransferID" runat="server" Value="0" />
<asp:HiddenField ID="hdnIsLineInsert" runat="server" Value="false" />
<asp:HiddenField ID="hdnAccountAPortfolioExists" runat="server" Value="0" />
<asp:HiddenField ID="hdnIsManual" runat="server" Value="0" />
<%@ Import Namespace="B4F.TotalGiro.Orders.Transfers" %>
<asp:GridView ID="gvNTMTransfer" runat="server" AllowPaging="True" AllowSorting="True"
    SkinID="spreadsheet-custom-width" Width="1000px" DataSourceID="odsNTMTransfer"
    AutoGenerateColumns="False" Caption="Historical Position" CaptionAlign="Left"
    DataKeyNames="Key" PageSize="20" Visible="true" ShowFooter="true" OnRowUpdating="gvNTMTransfer_RowUpdating"
    OnRowDataBound="gvNTMTransfer_RowDataBound">

    <Columns>
        <asp:TemplateField HeaderText="TxDirection" SortExpression="TxDirection">
            <ItemStyle Wrap="False" HorizontalAlign="Center" />
            <ItemTemplate>
                <%# (TransferDirection)DataBinder.Eval(Container.DataItem, "TxDirection") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlTxDirection" runat="server" AutoPostBack="true" DataSourceID="odsSelectedDirection"
                    DataTextField="Description" OnSelectedIndexChanged="ddlTxDirection_SelectedIndexChanged" DataValueField="Key">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsSelectedDirection" runat="server" SelectMethod="GetTxDirection"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.TransferPositionDetailsAdapter" OldValuesParameterFormatString="original_{0}">
                </asp:ObjectDataSource>
            </EditItemTemplate>
        </asp:TemplateField>        
        <asp:TemplateField HeaderText="Instrument Description" SortExpression="InstrumentDescription">
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "InstrumentDescription")%>
                
            </ItemTemplate>
            <EditItemTemplate>
            <asp:HiddenField ID="hdnInstrumentId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "InstrumentID").ToString() %>' />
                <asp:DropDownList ID="ddlInstrumentOfPosition" runat="server" AutoPostBack="true"
                    DataSourceID="odsInstrumentOfPosition" SkinID="custom-width" Width="275" 
                    DataTextField="Description" OnSelectedIndexChanged="ddlInstrumentOfPosition_SelectedIndexChanged"
                    DataValueField="Key">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrumentOfPosition" 
                    runat="server" 
                    SelectMethod="GetInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.TransferPositionDetailsAdapter" 
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnTransferID" DefaultValue="0" Name="positionTransferID"
                            PropertyName="Value" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlTxDirection" Name="txDirection" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="hdnInstrumentId" DefaultValue="0" Name="instrumentID"
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Size" SortExpression="Size">
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" />
            <EditItemTemplate>
                <db:DecimalBox ID="dbSize" DecimalPlaces="6" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" />
            </EditItemTemplate>
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "Size")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actual Price" SortExpression="ActualPriceShortDisplayString">
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "ActualPriceShortDisplayString")%>
            </ItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Transfer Price" SortExpression="Price" Visible="false">
            <EditItemTemplate>
                <db:DecimalBox ID="dbPriceQuantity" DecimalPlaces="4" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" />
            </EditItemTemplate>
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "TransferPriceShortDisplayString")%>
            </ItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ex Rate" FooterText="Total:" SortExpression="ExchangeRate" FooterStyle-Font-Bold="True">
            <ItemStyle HorizontalAlign="Right" Wrap="False" />
            <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
            <ItemTemplate>
                <%# ((decimal)DataBinder.Eval(Container.DataItem, "ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}"): "") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="ValueinEuro" SortExpression="ValueinEuroQty" FooterStyle-Font-Bold="True"
            FooterStyle-HorizontalAlign="Right">
            <ItemStyle HorizontalAlign="Right" Wrap="False" />
            <HeaderStyle CssClass="alignright" HorizontalAlign="Right" Wrap="False" />
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "ValueinEuro")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:Panel ID="pnlButtons" runat="server" CssClass="padding">
                    <asp:LinkButton ID="lbtEdit" runat="server" CausesValidation="False" Text="Edit"
                        CommandName="EditLine" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        OnCommand="lbtEdit_Command" Visible='<%# DataBinder.Eval(Container.DataItem, "IsEditable") %>' />
                    <asp:LinkButton ID="lbtDelete" runat="server" CausesValidation="False" Text="Delete"
                        CommandName="DeleteLine" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        OnCommand="lbtDelete_Command" Visible='<%# DataBinder.Eval(Container.DataItem, "IsDeletable") %>'
                        OnClientClick="return confirm('Delete line?')" />
                    <asp:LinkButton ID="lbtUpdate" runat="server" CausesValidation="True" Text='<%# ((int)DataBinder.Eval(Container.DataItem, "Key") != 0 ? "Update" : "Insert") %>'
                        CommandName="Update" Visible="False" />
                </asp:Panel>
            </ItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Left" />
        </asp:TemplateField>
    </Columns>
<%--    <EmptyDataTemplate>
        <table cellpadding="0" cellspacing="0" border="0" width="1048px">
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
        </table>
    </EmptyDataTemplate>--%>
</asp:GridView>
<asp:ObjectDataSource ID="odsNTMTransfer" runat="server" SelectMethod="GetPositionTransferDetails"
    UpdateMethod="UpdatePositionTransferDetails" TypeName="B4F.TotalGiro.ApplicationLayer.UC.TransferPositionDetailsAdapter"
    DataObjectTypeName="B4F.TotalGiro.ApplicationLayer.UC.TransferPositionDetailsEditView"
    OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdnTransferID" DefaultValue="0" Name="positionTransferID"
            PropertyName="Value" Type="Int32" />
        <asp:ControlParameter ControlID="hdnIsLineInsert" DefaultValue="False" Name="isInsert"
            PropertyName="Value" Type="Boolean" />
    </SelectParameters>
</asp:ObjectDataSource>

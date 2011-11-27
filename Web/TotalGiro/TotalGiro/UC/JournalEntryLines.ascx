<%@ Control Language="C#" AutoEventWireup="true" CodeFile="JournalEntryLines.ascx.cs" Inherits="JournalEntryLines" %>

<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="~/UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Import Namespace="B4F.TotalGiro.GeneralLedger.Journal" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments" %>

<%--<asp:UpdatePanel ID="upGiroAccount" runat="server">
<ContentTemplate>--%>
<asp:HiddenField ID="hdnJournalEntryId" runat="server" Value="0" />
<asp:HiddenField ID="hdnAllowGiroAccountsDataBind" runat="server" Value="false" />
<asp:HiddenField ID="hdnIsLineInsert" runat="server" Value="false" />
<asp:HiddenField ID="hdnStornoedLineId" runat="server" Value="0" />
<asp:HiddenField ID="hdnVirtualFundID" runat="server" Value="0" />
<asp:HiddenField ID="hdnShowManualAllowedGLAccountsOnly" runat="server" Value="false" />
<asp:ScriptManagerProxy ID="scriptman" runat="server" />
<asp:GridView ID="gvLines" runat="server" AutoGenerateColumns="False" PageSize="25" AllowPaging="True" Caption="Lines" CaptionAlign="Left"
    AllowSorting="True" DataSourceID="odsJournalEntryLines" OnRowDataBound="gvLines_RowDataBound" SkinID="spreadsheet-custom-width"
    DataKeyNames="Key" Visible="True" OnRowCommand="gvLines_RowCommand" OnRowUpdated="gvLines_RowUpdated" 
    OnRowUpdating="gvLines_RowUpdating" OnDataBinding="gvLines_DataBinding" >
    <Columns>
        <asp:TemplateField HeaderText="#" SortExpression="LineNumber">
            <ItemTemplate>
                <asp:Label ID="lblLineNumber" runat="server" Width="15px">
                    <%# DataBinder.Eval(Container.DataItem, "LineNumber")%></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status" SortExpression="Status">
            <ItemTemplate>
                <asp:Label ID="lblStatus" runat="server" Width="43px">
                    <%# (JournalEntryLineStati)DataBinder.Eval(Container.DataItem, "Status")%></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="GL Account" SortExpression="GLAccount_FullDescription">
            <ItemTemplate>
                <trunc:TruncLabel2 ID="lblGLAccount" runat="server" Width="192px" CssClass="padding"
                                   MaxLength="32" LongText='<%# DataBinder.Eval(Container.DataItem, "GLAccount_FullDescription") %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlGLAccount" runat="server" SkinID="custom-width" Width="202px" DataSourceID="odsGLAccounts" 
                    DataTextField="FullDescription" DataValueField="Key" AutoPostBack="true" 
                    OnSelectedIndexChanged="ddlGLAccount_SelectedIndexChanged" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsGLAccounts" runat="server" SelectMethod="GetGLAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.JournalEntryLinesAdapter">
                    <SelectParameters>
                        <asp:ControlParameter name="showAllowedManualOnly" ControlID="hdnShowManualAllowedGLAccountsOnly" PropertyName="Value" />
                        <asp:ControlParameter name="isLineInsert" ControlID="hdnIsLineInsert" PropertyName="Value" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="CUR" SortExpression="Currency" >
            <ItemTemplate>
                <asp:Label ID="lblCurrency" runat="server" Width="15px" >
                    <%# DataBinder.Eval(Container.DataItem, "Currency")%></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlCurrency" runat="server" SkinID="custom-width" Width="47px" DataSourceID="odsCurrencies" 
                    DataTextField="Symbol" DataValueField="Key" AutoPostBack="true" 
                    OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsCurrencies" runat="server" SelectMethod="GetActiveCurrencies"
                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.JournalEntryLinesAdapter">
                </asp:ObjectDataSource>
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="ExRate" SortExpression="ExchangeRate">
            <ItemTemplate>
                <asp:Label ID="lblExchangeRate" runat="server" Width="50px">
                    <%# DataBinder.Eval(Container.DataItem, "ExchangeRate", "{0:###0.00000}")%></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <db:DecimalBox ID="dbExRateQuantity" runat="server" SkinID="custom-width" Width="50px" DecimalPlaces="5" Enabled="false" />
            </EditItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Debit" SortExpression="Debit">
            <ItemTemplate>
                <asp:Label ID="lblDebit" runat="server" Width="71px">
                    <%# DataBinder.Eval(Container.DataItem, "DebitDisplayString") %></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <db:DecimalBox ID="dbDebitQuantity" runat="server" SkinID="custom-width" Width="75px" />
            </EditItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Credit" SortExpression="Credit">
            <ItemTemplate>
                <asp:Label ID="lblCredit" runat="server" Width="71px">
                    <%# DataBinder.Eval(Container.DataItem, "CreditDisplayString")%></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <db:DecimalBox ID="dbCreditQuantity" runat="server" SkinID="custom-width" Width="75px" />
            </EditItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Right" />
            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Giro Account" SortExpression="GiroAccount_Number">
            <ItemTemplate>
                <uc2:AccountLabel ID="ctlAccountLabel" 
                    runat="server" 
                    RetrieveData="false" 
                    Width="120px" 
                    NavigationOption="PortfolioView"
                    AccountDisplayOption="DisplayNumber"
                    ShowFeeDetails='<%# DataBinder.Eval(Container.DataItem, "IsRelevantForDepositFee") %>'
                    ToolTip='<%# DataBinder.Eval(Container.DataItem, "GiroAccount_ShortName") %>'
                    />
            </ItemTemplate>
            <EditItemTemplate>
                <div style="position: relative; width: 109px">
                    <asp:TextBox ID="txtGiroAccount" runat="server" autocomplete="off" 
                                 SkinID="custom-width" Width="93px"></asp:TextBox>
                    <div style="position: absolute; left: 92px; top: 0px">
                        <asp:Button 
                            ID="btnFindAccount" runat="server" Text="..." CausesValidation="False" Width="18px" 
                            Height="19px" CommandName="FindAccount" Font-Bold="true" />
                    </div>
                    <div style="position: absolute; left: 0px; top: 19px; background-color: White">
                        <asp:Panel ID="pnlAccountList" runat="server" BorderStyle="None" Width="200px" Visible="False" >
                            <asp:ListBox ID="lboGiroAccount" runat="server" Width="250px" Height="250px" AutoPostBack="true"
                                         DataSourceID="odsSelectedAccount" DataTextField="DisplayNumberWithName" 
                                         DataValueField="Number" OnSelectedIndexChanged="lboGiroAccount_SelectedIndexChanged">
                            </asp:ListBox>
                            <asp:ObjectDataSource ID="odsSelectedAccount" runat="server" SelectMethod="GetCustomerAccounts"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter" 
                                OnSelecting="odsSelectedAccount_Selecting" OldValuesParameterFormatString="original_{0}">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                                        Type="String" />
                                    <asp:Parameter Name="retrieveNostroAccounts" DefaultValue="True" Type="Boolean" />
                                    <asp:ControlParameter ControlID="hdnVirtualFundId" Name="virtualFundId" PropertyName="Value" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <div style="position: absolute; left: 0px; top: 234px; width: 248px; height: 42px; border-style: solid; border-width: 1px; border-color: Gray; background-color: #F0F0F0">
                                <div style="position: absolute; left: 5px; top: 14px">
                                    <asp:Button ID="btnShowAccountFinder" runat="server" Text="Change Filter" CausesValidation="False" 
                                                OnClick="btnShowAccountFinder_Click" CommandName="ShowAccountFinder" Width="110px" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                    <div style="position: absolute; left: 0px; top: 296px; background-color: White">
                        <asp:Panel ID="pnlAccountFinder" runat="server" BorderColor="Gray" BorderStyle="Solid" 
                                   BorderWidth="1px" Width="410px" BackColor="White" Visible="false"  >
                            <uc1:AccountFinder ID="ctlAccountFinder" runat="server" />
                        </asp:Panel>
                    </div>
                </div>
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Description" SortExpression="Description">
            <ItemTemplate>
                <trunc:TruncLabel2 ID="lblDescription" runat="server" Width="124px" CssClass="padding"
                                   MaxLength="22" LongText='<%# DataBinder.Eval(Container.DataItem, "Description") %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtDescription" runat="server" autocomplete="off" SkinID="custom-width" Width="128px"></asp:TextBox>
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Original Description" SortExpression="OriginalDescription">
            <ItemTemplate>
               <trunc:TruncLabel2 ID="lblOriginalDescription" runat="server" Width="217px" CssClass="padding"
                                   MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "OriginalDescription") %>' />
            </ItemTemplate>
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:Panel ID="pnlButtons" runat="server" CssClass="padding">
                    <asp:LinkButton 
                        ID="lbtEdit" 
                        runat="server" 
                        CausesValidation="False" 
                        Text="Edit"
                        CommandName="EditLine"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        OnCommand="lbtEdit_Command"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "IsEditable") %>' />
                    <asp:LinkButton 
                        ID="lbtDelete" 
                        runat="server" 
                        CausesValidation="False" 
                        Text="Delete"
                        CommandName="DeleteLine"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        OnCommand="lbtDelete_Command"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "IsDeletable") %>'
                        OnClientClick="return confirm('Delete line?')" />
                    <asp:LinkButton 
                        ID="lbtStorno" 
                        runat="server" 
                        CausesValidation="False" 
                        Text="Storno"
                        CommandName="StornoLine"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        OnCommand="lbtStorno_Command"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "IsStornoable") %>' />
                    <asp:LinkButton 
                        ID="lbtUpdate" 
                        runat="server" 
                        CausesValidation="True" 
                        Text='<%# ((int)DataBinder.Eval(Container.DataItem, "Key") != 0 ? "Update" : "Insert") %>'
                        CommandName="Update"
                        Visible="False" />
                    <asp:LinkButton 
                        ID="lbtCancel" 
                        runat="server" 
                        CausesValidation="False" 
                        Text="Cancel"
                        CommandName="Cancel"
                        Visible="False" />
                    <asp:LinkButton 
                        ID="lbtAddTransferFee" 
                        runat="server" 
                        CausesValidation="False" 
                        Text="Add Transfer Fee"
                        CommandName="AddTransferFee"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        OnCommand="lbtAddTransferFee_Command"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "IsAllowedToAddTransferFee") %>' />
                </asp:Panel>
            </ItemTemplate>
            <ItemStyle Wrap="False" HorizontalAlign="Left" />
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <table cellpadding="0" cellspacing="0" border="0" width="1048px">
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr><td colspan="3">&nbsp;</td></tr>
            <tr><td colspan="3">&nbsp;</td></tr>
        </table>
    </EmptyDataTemplate>
</asp:GridView>
<%--</ContentTemplate>
</asp:UpdatePanel>--%>
<asp:ObjectDataSource ID="odsJournalEntryLines" runat="server" SelectMethod="GetJournalEntryLines" UpdateMethod="UpdateJournalEntryLine"
    TypeName="B4F.TotalGiro.ApplicationLayer.UC.JournalEntryLinesAdapter"
    DataObjectTypeName="B4F.TotalGiro.ApplicationLayer.UC.JournalEntryLineEditView" OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdnJournalEntryId" Name="journalEntryId" PropertyName="Value" Type="Int32" />
        <asp:ControlParameter ControlID="hdnIsLineInsert" DefaultValue="False" Name="isInsert"
            PropertyName="Value" Type="Boolean" />
        <asp:ControlParameter ControlID="hdnStornoedLineId" DefaultValue="0" Name="stornoedLineId"
            PropertyName="Value" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AccountsByInstrument.aspx.cs" Inherits="Portfolio_AccountsByInstrument" Theme="Neutral" Title="Accounts by Instrument" %>

<%@ Register Src="../UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" />
    <br />
    <asp:Panel ID="pnlSelectedInstrument" runat="server" Visible="False" Width="700px">
        <hr style="position: relative; top: -20px; width: 778px;" />
        <table style="width: 480px; position: relative; top: -10px">
            <tr>
                <td style="width: 120px; height: 24px">
                    <asp:Label ID="Label1" runat="server" Text="Sel. Instrument:"></asp:Label></td>
                <td style="width: 315px; height: 24px">
                    <asp:DropDownList ID="ddlSelectedInstrument" SkinID="custom-width" runat="server" Width="300px" DataSourceID="odsSelectedInstrument" DataTextField="DisplayIsinWithName" 
                        DataValueField="Key" OnSelectedIndexChanged="ddlSelectedInstrument_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="odsSelectedInstrument" runat="server" SelectMethod="GetInstruments"
            TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.AccountsByInstrumentAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                    Type="Object" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="exchangeId" PropertyName="ExchangeId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
    <asp:Panel ID="pnlAccounts" runat="server" Width="125px" Visible="False">
        <table style="width: 644px">
            <tr>
                <td style="width: 143px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="Label7" runat="server" Text="Total Size:"></asp:Label></td>
                <td style="width: 166px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="lblTotalSize" runat="server" Font-Bold="True" Width="150px"></asp:Label></td>
                <td style="width: 45px; height: 21px;white-space:nowrap;"></td>
                <td style="width: 140px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="Label3" runat="server" Text="Price:"></asp:Label></td>
                <td style="width: 150px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="lblPrice" runat="server" Font-Bold="True" Width="150px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 143px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="Label5" runat="server" Text="Total Value:"></asp:Label></td>
                <td style="width: 166px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="lblTotalValue" runat="server" Font-Bold="True" Width="150px"></asp:Label></td>
                <td style="width: 45px; height: 21px;white-space:nowrap;"></td>
                <td style="width: 140px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="Label4" runat="server" Text="Exchange Rate:"></asp:Label></td>
                <td style="width: 150px; height: 21px;white-space:nowrap;">
                    <asp:Label ID="lblExchangeRate" runat="server" Font-Bold="True" Width="150px"></asp:Label></td>
            </tr>
        </table>
        <br />
        <cc1:MultipleSelectionGridView ID="gvPositions" runat="server" AutoGenerateColumns="False" DataSourceID="odsPositions" 
            AllowPaging="True" AllowSorting="True" PageSize="15" Caption="Positions" CaptionAlign="Left" DataKeyNames="Key" 
            OnDataBound="gvPositions_DataBound" OnRowDataBound="gvPositions_RowDataBound" OnRowCommand="gvPositions_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Account#" SortExpression="Account_Number">
                    <HeaderStyle Wrap="False" />
                    <ItemStyle HorizontalAlign="Left" Wrap="False" />
                    <ItemTemplate>
                        <uc2:AccountLabel ID="ctlAccountLabel" 
                            runat="server" 
                            RetrieveData="false" 
                            Width="120px" 
                            NavigationOption="PortfolioView"
                            AccountDisplayOption="DisplayNumber"
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Account Name" SortExpression="Account_ShortName">
                    <ItemTemplate>
                        <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                            MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "Account_ShortName") %>' />
                    </ItemTemplate>
                    <HeaderStyle wrap="False" />
                    <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="Size_Quantity" HeaderText="Size" SortExpression="Size_Quantity">
                    <ItemStyle wrap="False" horizontalalign="Right" />
                    <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:BoundField DataField="CurrentBaseValue" HeaderText="Value" SortExpression="CurrentBaseValue">
                    <ItemStyle wrap="False" horizontalalign="Right" />
                    <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                </asp:BoundField>
                <asp:CommandField SelectText="View" ShowSelectButton="True">
                    <ItemStyle Wrap="False" Width="29px" />
                </asp:CommandField>
            </Columns>
        </cc1:MultipleSelectionGridView>
        <asp:ObjectDataSource ID="odsPositions" runat="server" SelectMethod="GetPositions" 
            TypeName="B4F.TotalGiro.ApplicationLayer.Portfolio.AccountsByInstrumentAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlSelectedInstrument" Name="instrumentId" PropertyName="SelectedValue"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <asp:MultiView ID="mvwButtons" runat="server" ActiveViewIndex="0" EnableTheming="False">
            <asp:View ID="vwMain" runat="server">
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="700px"/>
                <br />
                <table width="280px" >
                    <tr>
                        <td>
                            <asp:Button ID="btnClosePositions" runat="server" OnClick="btnClosePositions_Click" Text="Close Positions" 
                                OnClientClick="return confirm('Are you sure you want to close positions?')"/>
                        </td>
                        <td align="left"  >
                            <asp:CheckBox ID="chkNoCharges" runat="server" Text="No Commission" />
                        </td>
                    </tr>
                </table>
            </asp:View>
            <asp:View ID="vwQuestion" runat="server">
                <asp:Label ID="lblWarningMessages" runat="server" Width="700px"></asp:Label><br />
                <asp:Button ID="btnYes" runat="server" OnClick="btnYes_Click" Text="Yes" />&nbsp;
                <asp:Button ID="btnNo" runat="server" Text="No" /></asp:View>
        </asp:MultiView>
    </asp:Panel>
    <br />
    <br />
</asp:Content>


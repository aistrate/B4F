<%@ Page Title="Nav Calculation Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="NavCalculationDetails.aspx.cs" Inherits="NavCalculationDetails" %>

<%@ Register Src="~/UC/JournalEntryLines.ascx" TagName="JournalEntryLines" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hdnCalculationId" runat="server" />
    <asp:Table runat="server" Width="100%" Caption="Fund Details">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell1" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell2" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell3" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell4" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell5" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell6" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell7" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell8" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell9" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell10" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell23" runat="server" Width="10%" />
            <asp:TableCell ID="TableCell24" runat="server" Width="10%" />
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell ID="r1c1" runat="server" ColumnSpan="3">
                <asp:Label ID="lblFundName" runat="server" Text="Fund Name"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="r1c2" runat="server" ColumnSpan="9">
                <asp:TextBox ID="txtFundName" runat="server" SkinID="custom-width" Width="600"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell11" runat="server" ColumnSpan="3">
                <asp:Label ID="lblHoldingsAccount" runat="server" Text="Holdings Account"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtHoldingsAccount" runat="server" SkinID="custom-width" Width="250"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell13" runat="server" ColumnSpan="3">
                <asp:Label ID="lblTradingAccount" runat="server" Text="Trading Account"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtTradingAccount" runat="server" SkinID="custom-width" Width="280"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableCell ID="TableCell15" runat="server" ColumnSpan="3">
                <asp:Label ID="lblValuationDate" runat="server" Text="Valuation Date"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtValuationDate" runat="server" SkinID="custom-width" Width="250"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell17" runat="server" ColumnSpan="3">
                <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell18" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtStatus" runat="server" SkinID="custom-width" Width="280"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow7" runat="server">
            <asp:TableCell ID="TableCell19" runat="server" ColumnSpan="3">
                <asp:Label ID="lblGrossAssetValue" runat="server" Text="Gross Asset Value"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell20" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtGrossAssetValue" runat="server" SkinID="custom-width" Width="250"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell21" runat="server" ColumnSpan="3">
                <asp:Label ID="lblParticipationsBefore" runat="server" Text="Participations Before"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell22" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtParticipationsBefore" runat="server" SkinID="custom-width" Width="280"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow8" runat="server">
            <asp:TableCell ID="TableCell25" runat="server" ColumnSpan="3">
                <asp:Label ID="lblNettAssetValue" runat="server" Text="Nett Asset Value"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell26" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtNettAssetValue" runat="server" SkinID="custom-width" Width="250"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell27" runat="server" ColumnSpan="3">
                <asp:Label ID="lblParticipationsNow" runat="server" Text="Subscribed/Redeemed"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell28" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtParticipationsNow" runat="server" SkinID="custom-width" Width="280"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow9" runat="server">
            <asp:TableCell ID="TableCell29" runat="server" ColumnSpan="3">
                <asp:Label ID="lblNAVperUnit" runat="server" Text="NAV per Unit"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell30" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtNAVperUnit" runat="server" SkinID="custom-width" Width="250"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell31" runat="server" ColumnSpan="3">
                <asp:Label ID="lblParticipationsAfter" runat="server" Text="Participations After"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell32" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtParticipationsAfter" runat="server" SkinID="custom-width" Width="280"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow10" runat="server">
            <asp:TableCell ID="TableCell33" runat="server" ColumnSpan="3">
                <asp:Label ID="lblPublicOfferPrice" runat="server" Text="Public Offer Price"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell34" runat="server" ColumnSpan="3">
                <asp:TextBox ID="txtPublicOfferPrice" runat="server" SkinID="custom-width" Width="250"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell ID="TableCell35" runat="server" ColumnSpan="3">
                <asp:Label ID="Label2" runat="server" Text="Participations After"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell36" runat="server" ColumnSpan="3">
                <asp:TextBox ID="TextBox2" runat="server" SkinID="custom-width" Width="280"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table runat="server" Caption="Positions at Calculation" Width="100%">
        <asp:TableRow runat="server">
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Button ID="btnImportOrders" runat="server" Text="Import Orders" OnClick="btnImportOrders_Click" />
    <asp:Table ID="Table1" runat="server" Caption="New Orders" Width="100%">
        <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableCell runat="server" ColumnSpan="12">
                <asp:GridView ID="gvNewOrders" runat="server" AllowPaging="True" AllowSorting="True"
                    DataSourceID="odsNewOrders" AutoGenerateColumns="False" Caption="New Orders"
                    CaptionAlign="Left" DataKeyNames="Key" PageSize="20" SkinID="custom-width" Width="100%">
                    <Columns>
                        <asp:BoundField DataField="OrderID" HeaderText="OrderID" SortExpression="OrderID">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PlacedValue_DisplayString" HeaderText="PlacedValue" SortExpression="PlacedValue_DisplayString">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="ApprovalDate" SortExpression="ApprovalDate">
                            <ItemTemplate>
                                <asp:Label ID="lblApprovalDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "ApprovalDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "ApprovalDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DisplayStatus" HeaderText="Status" SortExpression="Status">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsNewOrders" runat="server" SelectMethod="GetOrdersFromCalculation"
                    TypeName="B4F.TotalGiro.ApplicationLayer.VirtualFunds.NavCalculationDetailsAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnCalculationId" Name="CalcID" PropertyName="Value"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table ID="Table2" runat="server" Caption="Fees Charged" Width="100%">
        <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableCell ID="TableCell37" runat="server" ColumnSpan="12">
                <uc1:JournalEntryLines ID="ctlJournalEntryLines" runat="server" ShowOriginalDescription="false" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <%--                        <asp:TableCell runat="server" Width="300px">
                <asp:Button ID="btnJournals" runat="server" Text="Journals" CausesValidation="False"
                    PostBackUrl="~/BackOffice/BankStatementJournals.aspx" />&nbsp;
                <asp:Button ID="btnStatements" runat="server" Text="Statements" CausesValidation="False"
                    OnClick="btnStatements_Click" />
            </asp:TableCell>--%>
            <asp:TableCell ID="TableCell38" runat="server" align="right">
                <asp:Panel ID="pnlActionButtons" runat="server">
                    <asp:Button ID="btnNewLine" runat="server" Text="New Line" CausesValidation="False" OnClick="btnNewLine_Click"
                         />&nbsp;
                    <asp:Button ID="btnBook" runat="server" Text="Book" CausesValidation="False" Enabled="False"
                         OnClientClick="return confirm('Are you sure you want to book this Bank Statement?')" />
                <asp:Label ID="lblErrorMessageLedger" runat="server" ForeColor="Red" Height="0px"></asp:Label>
                </asp:Panel>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table ID="Table3" runat="server" Caption="Post NAV Bookings" Width="100%">
        <asp:TableRow ID="TableRow6" runat="server">
        </asp:TableRow>
    </asp:Table>
    <asp:Button ID="btnBookNav" runat="server" Text="Book NAV" OnClick="btnBookNav_Click" />
</asp:Content>

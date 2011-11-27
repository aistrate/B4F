<%@ Page Title="BonusDistribution Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="BonusDistributionDetails.aspx.cs" Inherits="BonusDistributionDetails" %>

<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hdnInstrumentHistoryID" runat="server" Value="0" />
    <asp:Table runat="server" Width="1000px" CellSpacing="0" BorderStyle="Solid" Caption="Dividend Details">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblChooseInstrument" runat="server" Text="Choose Instrument:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:DropDownList ID="ddlInstrument" runat="server" AutoPostBack="true" DataSourceID="odsInstrument"
                    SkinID="custom-width" Width="275" DataTextField="Description" DataValueField="Key"
                    Enabled="false" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetTradeableInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.BonusDistributionAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvInstrument" runat="server" Enabled="false" ControlToValidate="ddlInstrument"
                    SetFocusOnError="True" InitialValue="0" ErrorMessage="Instrument is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblChooseChangeDate" runat="server" Text="Choose Bonus Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <uc1:DatePicker ID="dpBonusDate" runat="server" IsButtonDeleteVisible="false" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="Label1" runat="server" Text="Choose Distribution Account:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell3" runat="server">
                <asp:DropDownList ID="ddlAccounts" runat="server" AutoPostBack="true" DataSourceID="odsAccounts"
                    SkinID="custom-width" Width="275" DataTextField="Description" DataValueField="Key"
                    Enabled="false">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetInternalAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.BonusDistributionAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Left">
            </asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:Label ID="lblSizeToDistibute" runat="server" Text="Size To Distibute:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server">
                            <asp:TextBox ID="txtSizeToDistibute" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblTotalCustomerHoldings" runat="server" Text="Total Customer Holdings:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtTotalCustomerHoldings" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblExternalDescription" runat="server" Text="External Description:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:TextBox ID="txtExternalDescription" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell runat="server">                
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:GridView ID="gvBonusDistributionDetails" runat="server" AllowPaging="True" AllowSorting="True"
                    DataSourceID="odsBonusDistributionDetails" AutoGenerateColumns="False" Caption="Bonus Distribution Details"
                    CaptionAlign="Left" DataKeyNames="Key" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="Account" HeaderText="Account" SortExpression="Account">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PreviousSize" HeaderText="Previous Size" SortExpression="PreviousSize">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BonusPercentageDisplay" HeaderText="Bonus Percentage"
                            SortExpression="BonusPercentage">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <%-- <asp:BoundField DataField="DividendAmount" HeaderText="Dividend Amount" SortExpression="DividendAmount">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TaxAmount" HeaderText="Tax Amount" SortExpression="TaxAmount">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>--%>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsBonusDistributionDetails" runat="server" SelectMethod="GetBonusDistributionList"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.BonusDistributionAdapter"
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnInstrumentHistoryID" DefaultValue="0" Name="instrumentHistoryID"
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>--%>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

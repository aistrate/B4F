<%@ Page Title="Dividend Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="DividendDetails.aspx.cs" Inherits="DividendDetails" %>

<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hdnInstrumentHistoryID" runat="server" Value="0" />
    <asp:Table ID="Table2" runat="server" Width="1000px" CellSpacing="0" BorderStyle="Solid" BorderWidth="1px" BorderColor="Black"
        Caption="Dividend Details">
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell3" runat="server">
                <asp:Label ID="lblChooseInstrument" runat="server" Text="Choose Instrument:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server">
                <asp:DropDownList ID="ddlInstrumentOfPosition" runat="server" AutoPostBack="true"
                    DataSourceID="odsInstrumentOfPosition" SkinID="custom-width" Width="275" DataTextField="Description"
                    DataValueField="Key" Enabled="false" OnSelectedIndexChanged="ddlInstrumentOfPosition_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrumentOfPosition" runat="server" SelectMethod="GetTradeableInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.DividendAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server" HorizontalAlign="Left" ColumnSpan="2" >
                <asp:RequiredFieldValidator ID="rfvInstrumentOfPosition" runat="server" Enabled="false"
                    ControlToValidate="ddlInstrumentOfPosition" SetFocusOnError="True" InitialValue="-2147483648"
                    ErrorMessage="Instrument is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell ID="TableCell13" runat="server">
                <asp:Label ID="Label3" runat="server" Text="ISIN StockDiv:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server">
                <asp:TextBox ID="txtIsin" runat="server" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell17" runat="server" HorizontalAlign="Left" ColumnSpan="2" >
                <asp:RequiredFieldValidator ID="rfvIsin" runat="server" Enabled="false"
                    ControlToValidate="txtIsin" SetFocusOnError="True"
                    ErrorMessage="Isin is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow7" runat="server">
            <asp:TableCell ID="TableCell18" runat="server">
                <asp:Label ID="Label4" runat="server" Text="Total Dividend Deposited:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell19" runat="server">
                <db:DecimalBox ID="dbTotalDividendDeposited" runat="server" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell20" runat="server" HorizontalAlign="Left" ColumnSpan="2" >
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableCell ID="TableCell1" runat="server">
                <asp:Label ID="lblChooseExDate" runat="server" Text="Choose Ex-Dividend Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server">
                <uc1:Calendar ID="dpExDividendDate" runat="server" IsButtonDeleteVisible="false"
                    Enabled="false" Format="dd-MM-yyyy" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server" HorizontalAlign="Left" ColumnSpan="2" >
                <asp:RequiredFieldValidator ID="rfvExDividendDate"
                    controlToValidate="dpExDividendDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="ExDividend Date is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableCell ID="TableCell5" runat="server">
                <asp:Label ID="Label1" runat="server" Text="Choose Payment Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server">
                <uc1:Calendar ID="dpPaymentDate" runat="server" IsButtonDeleteVisible="false" 
                Enabled="false" Format="dd-MM-yyyy" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server" ColumnSpan="2" >
                <asp:RequiredFieldValidator ID="rfvPaymentDate"
                    controlToValidate="dpPaymentDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Payment Date is Mandatory" />
                <asp:CompareValidator 
                    ID="cvPaymentDate" 
                    runat="server"
                    ControlToValidate="dpPaymentDate:txtCalendar"
                    ControlToCompare="dpExDividendDate:txtCalendar"
                    Operator="GreaterThanEqual"
                    Type="Date"
                    Text="*" 
                    ErrorMessage="The payment date can not be before the ex-dividend date"/>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow11" runat="server">
            <asp:TableCell ID="TableCell28" runat="server">
                <asp:Label ID="Label6" runat="server" Text="Type of Dividend:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell29" runat="server">
                <asp:RadioButtonList ID="rblDividendType" runat="server" Width="120px" RepeatDirection="Horizontal"
                    OnSelectedIndexChanged="rblDividendType_SelectedIndexChanged" AutoPostBack="true" >
                    <asp:ListItem Value="1" Selected="True" >Cash</asp:ListItem>
                    <asp:ListItem Value="2" >Stock</asp:ListItem>
                </asp:RadioButtonList>
            </asp:TableCell>
            <asp:TableCell ID="TableCell30" runat="server" ColumnSpan="2" >
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow runat="server">
            <asp:TableCell ID="TableCell7" runat="server">
                <asp:Label ID="Label2" runat="server" Text="Price:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <db:DecimalBox ID="dbPriceQuantity" DecimalPlaces="4" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" Enabled="false" />
                <asp:Label ID="lblPriceCurrency" runat="server" Text="EUR"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server" ColumnSpan="2" >
                <asp:RequiredFieldValidator ID="rfvPriceQuantity"
                    controlToValidate="dbPriceQuantity:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Price is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow10" runat="server">
            <asp:TableCell ID="TableCell25" runat="server">
                <asp:Label ID="Label5" runat="server" Text="Stock Div. Ratio:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell26" runat="server">
                <db:DecimalBox ID="dbScripRatio" DecimalPlaces="4" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" Enabled="false" />
                <asp:Label ID="lblScripRatioPercentage" runat="server" Text="%"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell27" runat="server" ColumnSpan="2" >
                <asp:RequiredFieldValidator ID="rfvScripRatio"
                    controlToValidate="dbScripRatio:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Ratio is Mandatory"
                    Enabled="false" />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableCell runat="server">
                <asp:Label ID="lblTaxType" runat="server" Text="Type Of Dividend Tax:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:DropDownList ID="ddlDividendTaxStyle" runat="server" AutoPostBack="true" DataSourceID="odsDividendTaxStyle"
                    SkinID="custom-width" Width="275" DataTextField="Description" DataValueField="Key"
                    OnSelectedIndexChanged="ddlDividendTaxStyle_SelectedIndexChanged" Enabled="true">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsDividendTaxStyle" runat="server" SelectMethod="GetDividendTaxStyles"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.DividendAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
                <asp:RequiredFieldValidator ID="rfvDividendTaxStyle" runat="server" Enabled="false"
                    ControlToValidate="ddlDividendTaxStyle" SetFocusOnError="True" InitialValue="-2147483648"
                    ErrorMessage="Type of Tax is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Left">
                <asp:Label ID="lblTaxPercentage" runat="server" Text="Tax Percentage:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Left">
                <db:DecimalBox ID="dbTaxPercentage" DecimalPlaces="7" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" Enabled="false" />
                <asp:CompareValidator ID="cvTaxPercentage"
                    controlToValidate="dbTaxPercentage:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ValueToCompare="0"
                    Type="Double"
                    Operator="GreaterThan"
                    EnableClientScript="false"
                    ErrorMessage="Tax Percentage has to be greater than 0." />
                <asp:RequiredFieldValidator ID="rfvTaxPercentage"
                    controlToValidate="dbTaxPercentage:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    Enabled="false"
                    ErrorMessage="Tax Percentage is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell8" runat="server">
                <asp:Label ID="lblExternalDescription" runat="server" Text="External Description:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server">
                <asp:TextBox ID="txtExternalDescription" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell15" runat="server" ColumnSpan="2" >
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="3" >
                <asp:Button ID="btnSaveDetails" runat="server" Text="Save Dividend Details" Enabled="false"
                    OnClick="btnSaveDetails_Click" />
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnInitialise" runat="server" Text="Initialise" Enabled="false" CausesValidation="False"
                    OnClick="btnInitialise_Click" />
            </asp:TableCell>
            <asp:TableCell  runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnExecute" runat="server" Text="Execute" Enabled="false" CausesValidation="False"
                    OnClick="btnExecute_Click" />
            </asp:TableCell>
            <asp:TableCell runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnLichten" runat="server" Text="Lichten" Enabled="false" CausesValidation="False" Visible="false"
                    OnClick="btnLichten_Click" />
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow ID="TableRow8" runat="server">
            <asp:TableCell ID="TableCell21" runat="server" ColumnSpan="3" >
                <asp:Label ID="lblTotalUnitsInPossessionLbl" runat="server" Text="Total# Units In Possession:" Font-Bold="true" ></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell22" runat="server" HorizontalAlign="Center">
                <asp:Label ID="lblTotalUnitsInPossession" runat="server" ></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow9" runat="server">
            <asp:TableCell ID="TableCell23" runat="server" ColumnSpan="3" >
                <asp:Label ID="lblTotalDivAmountLbl" runat="server" Text="Total Div Amount:" Font-Bold="true" ></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell24" runat="server" HorizontalAlign="Center">
                <asp:Label ID="lblTotalDivAmount" runat="server" ></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </asp:TableCell>
            <asp:TableCell runat="server"></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <asp:GridView ID="gvDividendDetails" runat="server" AllowPaging="True" AllowSorting="True"
                    DataSourceID="odsDividendDetails" AutoGenerateColumns="False" Caption="Dividend Details"
                    CaptionAlign="Left" DataKeyNames="Key" PageSize="20"
                    OnRowDataBound="gvDividendDetails_RowDataBound" >
                    <Columns>
                        <asp:TemplateField HeaderText="Account#" SortExpression="AccountNumber">
                            <HeaderStyle Wrap="False" />
                            <ItemStyle HorizontalAlign="Left" Wrap="False" />
                            <ItemTemplate>
                                <uc2:AccountLabel ID="ctlAccountLabel" 
                                    runat="server" 
                                    RetrieveData="false" 
                                    Width="120px" 
                                    NavigationOption="PortfolioView"
                                    AccountDisplayOption="DisplayNumberName"
                                    />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CheckBoxField DataField="IsStockDiv" HeaderText="IsStockDiv" SortExpression="IsStockDiv" >
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:CheckBoxField>
                        <asp:BoundField DataField="UnitsInPossession" HeaderText="Units In Possession" SortExpression="UnitsInPossession">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DividendAmount" HeaderText="Dividend Amount" SortExpression="DividendAmount">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TaxAmount" HeaderText="Tax Amount" SortExpression="TaxAmount">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsDividendDetails" runat="server" SelectMethod="GetAccountDividendDetails"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.DividendAdapter"
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnInstrumentHistoryID" DefaultValue="0" Name="instrumentHistoryID"
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" ></asp:Label>
</asp:Content>

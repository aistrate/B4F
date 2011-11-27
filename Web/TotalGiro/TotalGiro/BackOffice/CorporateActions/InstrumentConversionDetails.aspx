<%@ Page Title="Instrument Conversion Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="InstrumentConversionDetails.aspx.cs" Inherits="InstrumentConversionDetails" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hdnConvertedInstrumentID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnInstrumentConversionID" runat="server" Value="0" />
    <asp:Table ID="Table2" runat="server" Width="1000px" CellSpacing="0" BorderStyle="Solid" BorderWidth="1px" BorderColor="Black"
        Caption="Conversion Details">
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell3" runat="server">
                <asp:Label ID="lblChooseInstrument" runat="server" Text="Choose Instrument:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server">
                <asp:DropDownList ID="ddlInstrument" runat="server" AutoPostBack="true"
                    DataSourceID="odsInstrument" SkinID="custom-width" Width="275" DataTextField="Description"
                    DataValueField="Key" Enabled="false" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetTradeableInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.InstrumentConversionAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnInstrumentConversionID" DefaultValue="0" Name="instrumentConversionID"
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvInstrument" runat="server" 
                    ControlToValidate="ddlInstrument" SetFocusOnError="True" InitialValue="-2147483648"
                    ErrorMessage="Instrument is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow12" runat="server">
            <asp:TableCell ID="TableCell31" runat="server">
                <asp:Label ID="Label7" runat="server" Text="Choose/Create New Parent Instrument:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell32" runat="server">
                <asp:DropDownList ID="ddlNewInstrument" runat="server" AutoPostBack="true"
                    DataSourceID="odsNewInstrument" SkinID="custom-width" Width="275" DataTextField="Description"
                    DataValueField="Key" Enabled="false" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsNewInstrument" runat="server" SelectMethod="GetParentInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.InstrumentConversionAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnConvertedInstrumentID" DefaultValue="0" Name="convertedInstrumentId"
                            PropertyName="Value" Type="Int32" />
                        <asp:ControlParameter ControlID="hdnInstrumentConversionID" DefaultValue="0" Name="instrumentConversionID"
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <asp:Button ID="btnCreateNewInstrument" runat="server" Text="..." Enabled="false" CausesValidation="false"
                    OnClick="btnCreateNewInstrument_Click" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell33" runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvNewInstrument" runat="server" 
                    ControlToValidate="ddlNewInstrument" SetFocusOnError="True" InitialValue="-2147483648"
                    ErrorMessage="New Instrument is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow10" runat="server">
            <asp:TableCell ID="TableCell7" runat="server">
                <asp:Label ID="lblOldChildRatio" runat="server" Text="Old Instrument Ratio :"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCel21" runat="server">
                <db:DecimalBox ID="dbOldChildRatio" DecimalPlaces="7" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" Enabled="false" Value="1" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server">
                <asp:RequiredFieldValidator ID="rfvOldChildRatio"
                    controlToValidate="dbOldChildRatio:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Old Instrument Ratio is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow13" runat="server">
            <asp:TableCell ID="TableCell25" runat="server">
                <asp:Label ID="lblNewParentRatio" runat="server" Text="New Parent Ratio:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell26" runat="server">
                <db:DecimalBox ID="dbNewParentRatio" DecimalPlaces="0" AllowNegativeSign="false" runat="server"
                    SkinID="custom-width" Width="75px" Enabled="false" Value="1" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell27" runat="server">
                <asp:RequiredFieldValidator ID="rfvNewParentRatio"
                    controlToValidate="dbNewParentRatio:tbDecimal"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="New Parent Ratio is Mandatory"
                    />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell ID="TableCell13" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server">
                <asp:CheckBox ID="chkIsSpinOff" runat="server" Text="IsSpinOff" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell17" runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableCell ID="TableCell1" runat="server">
                <asp:Label ID="lblChangeDate" runat="server" Text="Change Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server">
                <uc1:Calendar ID="dpChangeDate" runat="server" IsButtonDeleteVisible="false"
                    Enabled="false" Format="dd-MM-yyyy" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvChangeDate"
                    controlToValidate="dpChangeDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Change Date Date is Mandatory" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trExecutionDate" runat="server" Visible="false" >
            <asp:TableCell ID="TableCell5" runat="server">
                <asp:Label ID="lblExecutionDate" runat="server" Text="Execution Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server">
                <uc1:Calendar ID="dpExecutionDate" runat="server" IsButtonDeleteVisible="false" 
                Enabled="false" Format="dd-MM-yyyy" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server">
                <asp:RequiredFieldValidator ID="rfvExecutionDate"
                    controlToValidate="dpExecutionDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Execution Date is Mandatory" />
                <asp:CompareValidator 
                    ID="cvExecutionDate" 
                    runat="server"
                    ControlToValidate="dpExecutionDate:txtCalendar"
                    ControlToCompare="dpChangeDate:txtCalendar"
                    Operator="GreaterThanEqual"
                    Type="Date"
                    Text="*" 
                    ErrorMessage="The execution date can not be before the change date"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="3" >
                <asp:Button ID="btnSaveDetails" runat="server" Text="Save Conversion Details" Enabled="false"
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
        </asp:TableRow>
        
        <asp:TableRow ID="trTotalOriginalSize" runat="server" Visible="false" >
            <asp:TableCell ID="TableCell21" runat="server" ColumnSpan="3" >
                <asp:Label ID="lblTotalOriginalSizeLbl" runat="server" Text="Total Original Size:" Font-Bold="true" ></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell22" runat="server" HorizontalAlign="Center">
                <asp:Label ID="lblTotalOriginalSize" runat="server" ></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trTotalConvertedSize" runat="server" Visible="false" >
            <asp:TableCell ID="TableCell23" runat="server" ColumnSpan="3" >
                <asp:Label ID="lblTotalConvertedSizeLbl" runat="server" Text="Total Converted Size:" Font-Bold="true" ></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell24" runat="server" HorizontalAlign="Center">
                <asp:Label ID="lblTotalConvertedSize" runat="server" ></asp:Label>
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
                <asp:GridView ID="gvInstrumentConversionTxDetails" runat="server" AllowPaging="True" AllowSorting="True"
                    DataSourceID="odsInstrumentConversionTxDetails" AutoGenerateColumns="False" Caption="Tx Details"
                    CaptionAlign="Left" DataKeyNames="Key" PageSize="20" Visible="false"
                    OnRowDataBound="gvInstrumentConversionTxDetails_RowDataBound" >
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
                        <asp:BoundField DataField="ValueSize" HeaderText="Original Size" SortExpression="ValueSizeQuantity">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ConvertedInstrumentSize" HeaderText="Converted Size" SortExpression="ConvertedInstrumentSizeQuantity">
                            <ItemStyle Wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Tx Date" SortExpression="TransactionDate">
                            <ItemTemplate>
                                <asp:Label ID="lblTransactionDate" runat="server">
                                    <%# DataBinder.Eval(Container.DataItem, "TransactionDate", "{0:dd-MM-yyyy}")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description" SortExpression="Description">
                            <ItemTemplate>
                                <trunc:TruncLabel2 ID="lblDescription" runat="server" Width="75px" CssClass="padding"
                                    MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "Description") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:CheckBoxField DataField="Approved" HeaderText="Approved" SortExpression="Approved">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:CheckBoxField>
                        <asp:TemplateField HeaderText="Created" SortExpression="CreationDate">
                            <ItemTemplate>
                                <asp:Label ID="lblCreationDate" runat="server">
                                    <%# DataBinder.Eval(Container.DataItem, "CreationDate", "{0:dd-MM-yyyy}")%></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsInstrumentConversionTxDetails" runat="server" SelectMethod="GetInstrumentConversionTxDetails"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.InstrumentConversionAdapter"
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnInstrumentConversionID" DefaultValue="0" Name="instrumentConversionId"
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" ></asp:Label>
</asp:Content>

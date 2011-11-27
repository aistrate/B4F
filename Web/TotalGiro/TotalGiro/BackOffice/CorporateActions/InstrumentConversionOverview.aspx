<%@ Page Title="Instrument Conversions Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="InstrumentConversionOverview.aspx.cs" Inherits="InstrumentConversionOverview" %>

<%@ Register TagPrefix="trunc" Namespace="Trunc" %>
<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register Src="~/UC/InstrumentFinder.ascx" TagName="InstrumentFinder" TagPrefix="uc1" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Instruments.CorporateAction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <uc1:InstrumentFinder ID="ctlInstrumentFinder" runat="server" SecCategoryFilter="Securities" ShowExchange="false" ShowSecCategory="true" ShowActivityFilter="false" />

    <br />
    <asp:GridView ID="gvConversions" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsConversions" AutoGenerateColumns="False" Caption="Conversions" CaptionAlign="Left"
        DataKeyNames="Key" PageSize="20" Visible="false" >
        <Columns>
            <asp:TemplateField HeaderText="Instrument" SortExpression="InstrumentName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblInstrumentName" runat="server" Width="75px" CssClass="padding"
                        MaxLength="45" LongText='<%# DataBinder.Eval(Container.DataItem, "InstrumentName") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Converted Instrument" SortExpression="NewInstrumentName">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblNewInstrumentName" runat="server" Width="75px" CssClass="padding"
                        MaxLength="45" LongText='<%# DataBinder.Eval(Container.DataItem, "NewInstrumentName") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Change Date" SortExpression="ChangeDate">
                <ItemTemplate>
                    <asp:Label ID="lblChangeDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "ChangeDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "ChangeDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Executed" SortExpression="ExecutionDate">
                <ItemTemplate>
                    <asp:Label ID="lblExecutionDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "ExecutionDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "ExecutionDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Ratio" HeaderText="Conversion" SortExpression="Ratio">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Description" SortExpression="Description">
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="lblDescription" runat="server" Width="75px" CssClass="padding"
                        MaxLength="20" LongText='<%# DataBinder.Eval(Container.DataItem, "Description") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Created" SortExpression="CreationDate">
                <ItemTemplate>
                    <asp:Label ID="lblCreationDate" runat="server">
                        <%# DataBinder.Eval(Container.DataItem, "CreationDate", "{0:dd-MM-yyyy}")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="False" />
                <ItemTemplate>
                    <asp:LinkButton ID="lbtDetails" runat="server" CausesValidation="False" Text="Details"
                        CommandName="ViewDetails" ToolTip="View Details" OnCommand="lbtDetails_Command"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
        <asp:ObjectDataSource ID="odsConversions" runat="server" SelectMethod="GetInstrumentConversions"
            TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.InstrumentConversionAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="isin" PropertyName="Isin"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="instrumentName" PropertyName="InstrumentName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="currencyNominalId" PropertyName="CurrencyNominalId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlInstrumentFinder" Name="secCategoryId" PropertyName="SecCategoryId"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    <br />
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="tcNewStuff" runat="server">
                <asp:Button ID="btnNewInstrumentConversion" runat="server" Text="Create New Conversion" 
                    OnClick="btnNewConversion_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell ID="tcValidationSummary" runat="server">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>

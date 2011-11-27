<%@ Page Title="BonusDistribution Overview" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" 
CodeFile="BonusDistributionOverview.aspx.cs" 
Inherits="BonusDistributionOverview" %>

<%@ Register Src="../../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hdnInstrumentID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnStartDate" runat="server" />
    <asp:HiddenField ID="hdnEndDate" runat="server" />
    <br />
    <br />
    <asp:GridView ID="gvBonusDistributions" runat="server" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsBonusDistributions" AutoGenerateColumns="False" Caption="Bonus Distributions" CaptionAlign="Left"
        DataKeyNames="Key" PageSize="20">
        <Columns>
            <asp:BoundField DataField="InstrumentName" HeaderText="InstrumentName" SortExpression="InstrumentName">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Change Date" SortExpression="ChangeDate">
                <ItemTemplate>
                    <asp:Label ID="lblChangeDate" runat="server">
                        <%#((DateTime)DataBinder.Eval(Container.DataItem, "ChangeDate") > DateTime.MinValue ? DataBinder.Eval(Container.DataItem, "ChangeDate", "{0:dd-MM-yyyy}") : "")%></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="UnitsAllocated" HeaderText="Units Allocated" SortExpression="UnitsAllocated">
                <ItemStyle Wrap="False" HorizontalAlign="Left" />
                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
            </asp:BoundField>
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
    <asp:ObjectDataSource ID="odsBonusDistributions" runat="server" SelectMethod="GetBonusDistributions"
        TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.BonusDistributionAdapter"
        OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:ControlParameter ControlID="hdnInstrumentID" DefaultValue="0" Name="instrumentKey"
                PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="hdnStartDate" Name="startDate" PropertyName="Value"
                Type="DateTime" />
            <asp:ControlParameter ControlID="hdnEndDate" Name="endDate" PropertyName="Value"
                Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />
        <asp:Table ID="Table1" runat="server" Width="1000px" CellSpacing="0" BorderStyle="Solid" BorderWidth="1px" BorderColor="Black"
            Caption="New Distribution Details">
        <asp:TableRow  runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="lblChooseInstrument" runat="server" Text="Choose Instrument:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:DropDownList ID="ddlInstrument" runat="server" AutoPostBack="true" DataSourceID="odsInstrument"
                    SkinID="custom-width" Width="275" DataTextField="Description" DataValueField="Key"
                    Enabled="true" >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetTradeableInstruments"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.BonusDistributionAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell  runat="server" HorizontalAlign="Left">
                <asp:RequiredFieldValidator ID="rfvInstrument" runat="server" Enabled="false" ControlToValidate="ddlInstrument"
                    SetFocusOnError="True" InitialValue="0" ErrorMessage="Instrument is Mandatory">*</asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow  runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="lblChooseChangeDate" runat="server" Text="Choose Bonus Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell5" runat="server">
                <uc1:DatePicker ID="dpBonusDate" runat="server" IsButtonDeleteVisible="false" Enabled="true" />
            </asp:TableCell>
            <asp:TableCell  runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="Label1" runat="server" Text="Choose Distribution Account:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:DropDownList ID="ddlAccounts" runat="server" AutoPostBack="true" DataSourceID="odsAccounts"
                    SkinID="custom-width" Width="275" DataTextField="Description" DataValueField="Key"
                    Enabled="true">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetInternalAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions.BonusDistributionAdapter"
                    OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
            </asp:TableCell>
            <asp:TableCell  runat="server" HorizontalAlign="Left">
            </asp:TableCell>
            <asp:TableCell   runat="server">
                <asp:Label ID="lblSizeToDistibute" runat="server" Text="Size To Distibute:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell  runat="server">
                            <asp:TextBox ID="txtSizeToDistibute" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell  runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow  runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="lblTotalCustomerHoldings" runat="server" Text="Total Customer Holdings:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:TextBox ID="txtTotalCustomerHoldings" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell  runat="server" HorizontalAlign="Left">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow  runat="server">
            <asp:TableCell  runat="server">
                <asp:Label ID="lblExternalDescription" runat="server" Text="External Description:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:TextBox ID="txtExternalDescription" runat="server" SkinID="custom-width" MaxLength="50"
                    Width="275" Enabled="false" />
            </asp:TableCell>
            <asp:TableCell  runat="server">                
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
        <asp:Table  runat="server">
        <asp:TableRow  runat="server">
            <asp:TableCell  runat="server"></asp:TableCell>
            <asp:TableCell  runat="server">
                <asp:Button ID="btnNewDistribution" runat="server" Text="Create New Distribution" Enabled="false"
                    CausesValidation="False" OnClick="btnNewDistribution_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow  runat="server">
            <asp:TableCell  runat="server">
                <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
                    ID="valSum" runat="server" />
            </asp:TableCell>
            <asp:TableCell  runat="server"></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
        <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Height="0px"></asp:Label>
</asp:Content>


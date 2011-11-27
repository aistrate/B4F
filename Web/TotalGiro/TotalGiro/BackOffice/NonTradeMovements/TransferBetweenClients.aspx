<%@ Page Title="Transfer Between Clients" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="TransferBetweenClients.aspx.cs" Inherits="TransferBetweenClients" %>

<%@ Register Src="~/UC/TransferPositionDetailsEditor.ascx" TagName="TransferPositionDetailsEditor" TagPrefix="uc2" %>
<%@ Register Src="~/UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="~/UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hdnCurrentStatus" runat="server" Value="0" />
    <asp:HiddenField ID="hdnShowSideA" runat="server" Value="True" />
    <asp:HiddenField ID="hdnTransferID" runat="server" Value="True" />
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server" />
    <br />
    <asp:Table runat="server" Width="1000px" CellSpacing="0">
        <asp:TableRow runat="server">
            <asp:TableCell ID="TableCell1" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell3" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell5" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell7" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell13" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell15" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell17" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell18" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell19" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell20" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell21" runat="server"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server" ID="rowFrom">
            <asp:TableCell ID="TableCell38" runat="server" ColumnSpan="2">
                <asp:CheckBox ID="chkAIsInternal" runat="server" AutoPostBack="true" OnCheckedChanged="chkAIsInternal_OnCheckedChanged"
                    Text="Internal Transfer" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:Label ID="lblAccountALabel" runat="server" Text="Account FROM:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="3" runat="server">
                <asp:UpdatePanel ID="updAccountsAList" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountAFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlAccountA" SkinID="custom-width" runat="server" Width="200px"
                            DataSourceID="odsSelectedAccountA" DataTextField="DisplayNumberWithName" DataValueField="Key"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlAccountA_SelectedIndexChanged"
                            OnDataBound="ddlAccountA_DataBound" EnableViewState="true" TabIndex="1">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;
                        <asp:ObjectDataSource ID="odsSelectedAccountA" runat="server" SelectMethod="GetCustomerAccounts"
                            TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.SingleOrderAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ctlAccountAFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ctlAccountAFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ctlAccountAFinder" Name="accountNumber" PropertyName="AccountNumber"
                                    Type="String" />
                                <asp:ControlParameter ControlID="ctlAccountAFinder" Name="accountName" PropertyName="AccountName"
                                    Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:UpdatePanel ID="updFilterAccountAButton" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Button ID="btnFilterAccountA" runat="server" CausesValidation="false" Text="Filter  >>"
                            OnClick="btnFilterAccountA_Click" TabIndex="2" Width="90px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell runat="server">
                <asp:UpdateProgress ID="upSpinnerA" runat="server" DisplayAfter="500">
                    <ProgressTemplate>
                        <asp:Image ID="imgWheel" runat="server" ImageUrl="~/layout/images/wheel.gif" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </asp:TableCell>
            <asp:TableCell ID="TableCell22" runat="server">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server" ID="rowFilterFrom">
            <asp:TableCell runat="server" ColumnSpan="3"></asp:TableCell>
            <asp:TableCell ID="TableCell28" runat="server" ColumnSpan="10">
                <asp:UpdatePanel ID="updAccountAFinder" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnFilterAccountA" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Panel ID="pnlAccountFinderA" runat="server" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" Visible="False" Width="440px">
                            <uc1:AccountFinder ID="ctlAccountAFinder" runat="server" ShowModelPortfolio="true"
                                TabIndex="3" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell ID="TableCell30" ColumnSpan="9" runat="server"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell ID="TableCell39" runat="server" ColumnSpan="2">
                <asp:CheckBox ID="chkBIsInternal" runat="server" AutoPostBack="true" OnCheckedChanged="chkBIsInternal_OnCheckedChanged"
                    Text="Internal Transfer" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell23" runat="server">
                <asp:Label ID="lblAccountB" runat="server" Text="Account TO:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell24" ColumnSpan="3" runat="server">
                <asp:UpdatePanel ID="updAccountBList" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctlAccountBFinder" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlAccountB" SkinID="custom-width" runat="server" Width="200px"
                            DataSourceID="odsSelectedAccountB" DataTextField="DisplayNumberWithName" DataValueField="Key"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlAccountB_SelectedIndexChanged"
                            OnDataBound="ddlAccountB_DataBound" EnableViewState="true" TabIndex="1">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;
                        <asp:ObjectDataSource ID="odsSelectedAccountB" runat="server" SelectMethod="GetCustomerAccounts"
                            TypeName="B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.SingleOrderAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ctlAccountBFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ctlAccountBFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ctlAccountBFinder" Name="accountNumber" PropertyName="AccountNumber"
                                    Type="String" />
                                <asp:ControlParameter ControlID="ctlAccountBFinder" Name="accountName" PropertyName="AccountName"
                                    Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell ID="TableCell25" runat="server">
                <asp:UpdatePanel ID="updFilterAccountBButton" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Button ID="btnFilterAccountB" runat="server" CausesValidation="false" Text="Filter  >>"
                            OnClick="btnFilterAccountB_Click" TabIndex="2" Width="90px" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell ID="TableCell26" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell27" ColumnSpan="2" runat="server">
            </asp:TableCell>
            <asp:TableCell ID="TableCell29" runat="server">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell ID="TableCell40" runat="server" ColumnSpan="3"></asp:TableCell>
            <asp:TableCell runat="server" ColumnSpan="10">
                <asp:UpdatePanel ID="updAccountBFinder" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnFilterAccountB" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Panel ID="pnlAccountFinderTo" runat="server" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" Visible="False" Width="440px">
                            <uc1:AccountFinder ID="ctlAccountBFinder" runat="server" ShowModelPortfolio="true"
                                TabIndex="3" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="9" runat="server"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell ID="TableCell45" runat="server" ColumnSpan="2">
            </asp:TableCell>
            <asp:TableCell ID="TableCell11" runat="server">
                <asp:Label ID="lblTransferDate" runat="server" Text="Transfer Date:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell43" ColumnSpan="3" runat="server">
                <uc1:Calendar ID="dpDateOfPortfolio" runat="server" IsButtonDeleteVisible="false" Format="dd-MM-yyyy" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow8" runat="server">
            <asp:TableCell ID="TableCell46" runat="server" ColumnSpan="2">
            </asp:TableCell>
            <asp:TableCell ID="TableCell47" runat="server">
                <asp:Label ID="lblCurrentStatus" runat="server" Text="Current Status:"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="cellStatusValue" ColumnSpan="2" runat="server">
                <asp:Label ID="lblStatusValue" runat="server" Font-Bold="true"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell48" runat="server" ColumnSpan="15">
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell31" runat="server" ColumnSpan="20">
                <asp:UpdatePanel ID="pnlTransferChoice" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Table ID="Table1" runat="server" Width="858px" BorderStyle="Solid">
                            <asp:TableRow ID="TableRow3" runat="server">
                                <asp:TableCell ID="TableCell32" runat="server" ColumnSpan="10">
                                    <asp:RadioButton runat="server" ID="rvFull"  Text="Full Transfer" AutoPostBack="true"
                                        OnCheckedChanged="rvAmount_CheckedChanged" GroupName="rblTransferChoice" Visible="true" />
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell33" runat="server" ColumnSpan="10"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow4" runat="server">
                                <asp:TableCell ID="TableCell34" runat="server" ColumnSpan="10">
                                    <asp:RadioButton runat="server" ID="rvAmount" Text="Transfer a Set Amount in €'s"
                                        AutoPostBack="true" OnCheckedChanged="rvAmount_CheckedChanged" GroupName="rblTransferChoice" />
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell35" runat="server" ColumnSpan="5">
                                    <asp:Label ID="lblAmountToTransfer" runat="server" Text="Amount To Transfer:" Visible="false"></asp:Label>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell36" runat="server" ColumnSpan="5">
                                    <db:DecimalBox ID="dbAmountToTransfer" runat="server" Visible="false" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow5" runat="server">
                                <asp:TableCell ID="TableCell37" runat="server" ColumnSpan="10">
                                    <asp:RadioButton runat="server" ID="rvManual" Text="Complete Manual Transfer" AutoPostBack="true"
                                        OnCheckedChanged="rvAmount_CheckedChanged" GroupName="rblTransferChoice" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell ID="TableCell41" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnInitialize" runat="server" CausesValidation="false" Text="Initialize"
                    OnClick="btnInitialize_Click" TabIndex="2" Width="90px" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell44" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnSaveDetails" runat="server" CausesValidation="false" Text="Save Details"
                    OnClick="btnSaveDetails_Click" TabIndex="2" Width="90px" />
            </asp:TableCell>
            <asp:TableCell ID="TableCell49" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnExecute" runat="server" CausesValidation="false" Text="Execute"
                    OnClick="btnExecute_Click" TabIndex="2" Width="90px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow7" runat="server">
            <asp:TableCell ID="TableCell42" runat="server" ColumnSpan="20">
                <uc2:TransferPositionDetailsEditor ID="ctlTransferPositions" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow9" runat="server">
            <asp:TableCell ID="TableCell50" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnNewLine" runat="server" CausesValidation="false" Text="Add New Line"
                    OnClick="btnNewLine_Click" TabIndex="2" Width="180px" Enabled="false"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server" ColumnSpan="20">
                <asp:GridView ID="gvPortfolioView" runat="server" AllowPaging="True" AllowSorting="True"
                    SkinID="spreadsheet-custom-width" Width="1000px" DataSourceID="odsPortfolioView"
                    AutoGenerateColumns="False" Caption="Before and After view of Portfolio" CaptionAlign="Left"
                    DataKeyNames="Key" PageSize="20" Visible="false" OnRowDataBound="gvPortfolioView_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="AccountDescription" HeaderText="Account" SortExpression="AccountDescription">
                            <HeaderStyle Wrap="False" />
                            <ItemStyle HorizontalAlign="Left" Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Instrument" HeaderText="Instrument" SortExpression="Instrument">
                            <HeaderStyle Wrap="False" />
                            <ItemStyle HorizontalAlign="Left" Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Price" SortExpression="ActualPriceShortDisplayString">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "ActualPriceShortDisplayString")%>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Size BeforeA" SortExpression="BeforeQuantity">
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                            <HeaderStyle Wrap="False" />
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "BeforeQuantity")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ValueinEuroBefore" HeaderText="Value Before" SortExpression="ValueinEuroBefore">
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                            <HeaderStyle Wrap="False" HorizontalAlign="Right" CssClass="alignright" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Size After" SortExpression="AfterPositionSizeDisplayString">
                            <ItemStyle Wrap="False" HorizontalAlign="Right" />
                            <HeaderStyle Wrap="False" />
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "AfterPositionSizeDisplayString")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsPortfolioView" runat="server" SelectMethod="ShowPortfolio"
                    TypeName="B4F.TotalGiro.ApplicationLayer.BackOffice.TransferAdapter" OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdnTransferID" DefaultValue="0" Name="positionTransferID"
                            PropertyName="Value" Type="Int32" />
                        <asp:ControlParameter ControlID="hdnShowSideA" DefaultValue="True" Name="showSideA"
                            PropertyName="Value" Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell runat="server" colspan="2" Style="height: 40px">
                <asp:Label ID="lblErrorMessageMain" runat="server" ForeColor="Red" Height="0px"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>

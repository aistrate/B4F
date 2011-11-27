<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" Theme="Neutral" CodeFile="ManagementFeeCorrections.aspx.cs" Inherits="ManagementFeeCorrections" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc3" %>
<%@ Register Src="../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<%@ Import Namespace="B4F.TotalGiro.ManagementPeriodUnits" %>
<%@ Import Namespace="B4F.TotalGiro.Fees" %>
<%@ Import Namespace="B4F.TotalGiro.Accounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <asp:HiddenField ID="hfYear" runat="server" />
    <asp:HiddenField ID="hfQuarter" runat="server" />
    <uc1:AccountFinder ID="ctlAccountFinder" runat="server" ShowModelPortfolio="true" ShowSearchButton="false" ShowContactActiveCbl="true" />
    <table>
        <tr>
            <td width="0.15px"> </td>
            <td width="128px">
                <asp:Label ID="lblYear" runat="server" Text="Year" />
            </td>
            <td width="150px">
                <asp:DropDownList ID="ddlYear" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblQuarter" runat="server" Text="Quarter" />
            </td>
            <td width="100px">
                <asp:DropDownList ID="ddlQuarter" runat="server" SkinID="custom-width" Width="50px" >
                    <asp:ListItem Text="" Value="0" Selected="True" />
                    <asp:ListItem Text="Q1" Value="1"  />
                    <asp:ListItem Text="Q2" Value="2" />
                    <asp:ListItem Text="Q3" Value="3" />
                    <asp:ListItem Text="Q4" Value="4" />
                </asp:DropDownList>
            </td>
        </tr>

        <tr >
            <td></td>
            <td>
                <asp:Label ID="lblOpenCloseStatus" runat="server" Text="Is Open?"  />
            </td>
            <td colspan="2">
                <asp:DropDownList ID="ddlOpenCloseStatus" runat="server" >
                    <asp:ListItem Text="Open" Value="0" Selected="True" />
                    <asp:ListItem Text="Closed" Value="1" />
                    <asp:ListItem Text="All" Value="-1" />
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="Search" Style="float: left; position: relative; top: -2px" Width="90px" 
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
    <br />
    <cc1:MultipleSelectionGridView 
        ID="gvMgtFeeCorrections"
        runat="server"
        CellPadding="0"
        AllowPaging="True"
        PageSize="20" 
        AutoGenerateColumns="False"
        DataKeyNames="AverageHolding_Key"
        Caption="Management Fee Corrections"
        CaptionAlign="Left"
        DataSourceID="odsMgtFeeCorrections" 
        AllowSorting="True"
        SelectionBoxVisibleBy="IsOpen"
        Visible="false"
        OnRowCommand="gvMgtFeeCorrections_OnRowCommand"
        OnRowDataBound="gvMgtFeeCorrections_RowDataBound"
        style="position: relative; top: -10px; z-index:2;">
        <Columns>
            <asp:TemplateField HeaderText="Account#" SortExpression="AverageHolding_Account_Number" >
                <ItemTemplate>
                    <uc3:AccountLabel ID="ctlAccountLabel" 
                        runat="server" 
                        RetrieveData="false" 
                        Width="120px" 
                        NavigationOption="PortfolioView"
                        ForeColor='<%# ((AccountStati)DataBinder.Eval(Container.DataItem, "AverageHolding_Account_Status") == AccountStati.Inactive ? System.Drawing.Color.Gray : System.Drawing.Color.Black) %>' 
                        />
                </ItemTemplate>
                <HeaderStyle wrap="False" />
                <ItemStyle wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Account Name" SortExpression="AverageHolding_Account_ShortName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlAccountName" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="20"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "AverageHolding_Account_ShortName") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "AverageHolding_Account_ShortName") %>'
                        ForeColor='<%# ((AccountStati)DataBinder.Eval(Container.DataItem, "AverageHolding_Account_Status") == AccountStati.Inactive ? System.Drawing.Color.Gray : System.Drawing.Color.Black) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Current Model" SortExpression="Unit_UnitParent_Account_ModelPortfolio_ModelName">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlModel" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="25"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "Unit_UnitParent_Account_ModelPortfolio_ModelName") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Unit_UnitParent_Account_ModelPortfolio_ModelName") %>' 
                        ForeColor='<%# ((AccountStati)DataBinder.Eval(Container.DataItem, "AverageHolding_Account_Status") == AccountStati.Inactive ? System.Drawing.Color.Gray : System.Drawing.Color.Black) %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AverageHolding_Period" HeaderText="Period" SortExpression="AverageHolding_Period" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="AverageHolding_BeginDate" HeaderText="Start Date" SortExpression="AverageHolding_BeginDate" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="AverageHolding_EndDate" HeaderText="End Date" SortExpression="AverageHolding_EndDate" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>            
            <asp:TemplateField HeaderText="Instrument" SortExpression="AverageHolding_Instrument_Name">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel2 ID="trlInstrumentName" 
                        runat="server"
                        cssclass="alignright"
                        MaxLength="20"
                        LongText='<%# DataBinder.Eval(Container.DataItem, "AverageHolding_Instrument_Name") %>'
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "AverageHolding_Instrument_Name") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AverageHolding_AverageValue_DisplayString" HeaderText="New Value" SortExpression="AverageHolding_AverageValue_DisplayString" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="AverageHolding_PreviousHolding_AverageValue_DisplayString" HeaderText="Old Value" SortExpression="AverageHolding_PreviousHolding_AverageValue_DisplayString" >
                <ItemStyle horizontalalign="Right" wrap="False" />
                <HeaderStyle horizontalalign="Right" cssclass="alignright" wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Skip" HeaderText="Skip" SortExpression="Skip" >
                <ItemStyle wrap="False" />
            </asp:BoundField>
            <%--<asp:BoundField DataField="Transaction_Key" HeaderText="TradeID" SortExpression="Transaction_Key" >
                <ItemStyle wrap="False" />
            </asp:BoundField>--%>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                         ID="lbtnViewDetails"
                         runat="server"
                         CommandName="ViewDetails"
                         Text="Details" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </cc1:MultipleSelectionGridView>
    <asp:ObjectDataSource ID="odsMgtFeeCorrections" runat="server" SelectMethod="GetManagementFeeCorrections"
        TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeCorrectionsAdapter" >
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ddlYear" Name="year" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:ControlParameter ControlID="ddlQuarter" Name="quarter" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:ControlParameter ControlID="ddlOpenCloseStatus" Name="openCloseStatus" PropertyName="SelectedValue" 
                Type="Int32" />
            <asp:Parameter Name="managementType" DefaultValue="1" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False" Font-Bold="True"></asp:Label>
    <asp:Label ID="lblResult" runat="server" Font-Bold="True" ForeColor="Black" Visible="False"></asp:Label>
    <br />
    <asp:Button ID="btnSkip" runat="server" Text="Skip Correction" OnClick="btnSkip_Click" Visible="false" />
    <br />
    <asp:GridView 
        ID="gvTransactions"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="Key"
        DataSourceID="odsTransactions"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="20" 
        AllowSorting="True"
        Caption="Management Fee Transaction Details"
        OnRowDataBound="gvTransactions_RowDataBound"
        OnDataBound="gvTransactions_DataBound" 
        Visible="false">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Key" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="TransactionDate" HeaderText="Tx Date" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="ValueSize_DisplayString" HeaderText="Amount" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Tax_DisplayString" HeaderText="Tax" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="SettleDifference" HeaderText="Settle Difference" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="StornoTransaction_Key" HeaderText="Storno" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="Description" HeaderText="Description" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="CreationDate" HeaderText="Created" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>            
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
        <EmptyDataTemplate>
            &nbsp;
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsTransactions" runat="server" 
        SelectMethod="GetManagementFeeTransactionData"
        TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeCorrectionsAdapter">
        <SelectParameters>
            <asp:SessionParameter SessionField="TradeId" Name="tradeId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <br />    
    <asp:GridView 
        ID="gvAverageHoldingFees"
        runat="server" 
        EnableViewState="False"
        DataKeyNames="Key"
        DataSourceID="odsAverageHoldingFees"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="20" 
        AllowSorting="True"
        Caption="Fees per average holdings"
        Visible="false">
        <Columns>
            <asp:BoundField DataField="FeeType" HeaderText="Type" SortExpression="FeeType"  >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
            <asp:BoundField DataField="CalculatedAmount_DisplayString" SortExpression="CalculatedAmount_Quantity" HeaderText="Calculated Amount" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="PreviousCalculatedFeeAmount_DisplayString" SortExpression="PreviousCalculatedFeeAmount_DisplayString" HeaderText="Prev Calc. Amount" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Amount_DisplayString" SortExpression="Amount_Quantity" HeaderText="Charged Amount" >
                <HeaderStyle CssClass="alignright" Wrap="False" />
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="FeeCalcSource_DisplayString" HeaderText="Fee Calculation" SortExpression="FeeCalcSource_DisplayString"  >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>            
           <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate" DataFormatString="{0:dd-MMM-yyyy}" HtmlEncode="False" >
                <ItemStyle HorizontalAlign="Left" Wrap="False" />
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
            </asp:BoundField>            
            <asp:TemplateField HeaderText="Message" SortExpression="DisplayMessage" >
                <ItemTemplate>
                    <trunc:TruncLabel ID="trlMessage" 
                        runat="server"
                        Width="40"
                        Text='<%# DataBinder.Eval(Container.DataItem, "DisplayMessage") %>' 
                        />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" CssClass="alignleft" Wrap="False" />
                <HeaderStyle CssClass="alignleft" Wrap="False" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
        <EmptyDataTemplate>
            &nbsp;
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAverageHoldingFees" runat="server" 
        SelectMethod="GetAverageHoldingFees"
        TypeName="B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeCorrectionsAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="gvMgtFeeCorrections" Name="averageHoldingID" PropertyName="SelectedValue" Type="String" />
            <asp:Parameter Name="managementType" DefaultValue="1" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>


</asp:Content>


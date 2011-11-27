<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestManagementFee.aspx.cs" Inherits="Test_TestManagementFee" 
         Title="Test Management Fee" Theme="Neutral" %>

<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label1" runat="server" Text="Account:"></asp:Label>
                &nbsp; &nbsp;<asp:DropDownList ID="ddlAccount" runat="server" Width="225px" DataSourceID="odsAccounts" 
                                               DataTextField="DisplayNumberWithName" DataValueField="Key">
                </asp:DropDownList><asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetCustomerAccounts"
                    TypeName="B4F.TotalGiro.ApplicationLayer.Test.TestManagementFeeAdapter"></asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td style="width: 300px">
                <asp:Label ID="Label2" runat="server" Text="Date:"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;<uc1:DatePicker ID="dpDateFrom" runat="server"/><asp:HiddenField ID="hdnDateFrom" runat="server" />
            </td>
            <td style="width: 300px">
                to:&nbsp;&nbsp;<uc1:DatePicker ID="dpDateTo" runat="server"/><asp:HiddenField ID="hdnDateTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="gvFeeNotas" runat="server" Caption="Management Fee Notas" CaptionAlign="Left"
        DataSourceID="odsFeeNotas">
    </asp:GridView>
    <asp:ObjectDataSource ID="odsFeeNotas" runat="server" SelectMethod="GetFeeNotas"
        TypeName="B4F.TotalGiro.ApplicationLayer.Test.TestManagementFeeAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAccount" DefaultValue="" Name="accountId" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter ControlID="hdnDateFrom" DefaultValue="" Name="dateFrom" PropertyName="Value"
                Type="DateTime" />
            <asp:ControlParameter ControlID="hdnDateTo" Name="dateTo" PropertyName="Value" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <br />
    <asp:GridView ID="gvValuations" runat="server" Caption="Valuations" CaptionAlign="Left"
        DataSourceID="odsValuations">
    </asp:GridView>
    <asp:ObjectDataSource ID="odsValuations" runat="server" SelectMethod="GetValuations"
        TypeName="B4F.TotalGiro.ApplicationLayer.Test.TestManagementFeeAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAccount" DefaultValue="" Name="accountId" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter ControlID="hdnDateFrom" DefaultValue="" Name="dateFrom" PropertyName="Value"
                Type="DateTime" />
            <asp:ControlParameter ControlID="hdnDateTo" Name="dateTo" PropertyName="Value" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>


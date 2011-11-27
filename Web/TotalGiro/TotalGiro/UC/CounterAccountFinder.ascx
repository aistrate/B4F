<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CounterAccountFinder.ascx.cs" Inherits="CounterAccountFinder" %>
<table style="width: 500px">
<tr>
<td style="width: 450px">
<table style="width: 450px">
    <tr>
        <td style="width: 170px; height: 24px">
            <asp:Label ID="Label1" runat="server" Text="Asset Manager:"></asp:Label></td>
        <td style="width: 225px; height: 24px">
            <asp:MultiView ID="mvwAssetManager" runat="server" ActiveViewIndex="0" EnableTheming="True">
                <asp:View ID="vwAssetManager" runat="server">
                    <asp:Label ID="lblAssetManager" runat="server" Font-Bold="True"></asp:Label></asp:View>
                <asp:View ID="vwStichting" runat="server">
                    <asp:DropDownList ID="ddlAssetManager" runat="server" Width="165px" DataSourceID="odsAssetManager" 
                        DataTextField="CompanyName" DataValueField="Key" AutoPostBack="False">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                        TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
                </asp:View>
            </asp:MultiView></td>
    </tr>
    <asp:Panel ID="pnlCounterAccountNumber" runat="server" Visible="false">
        <tr>
            <td style="width: 170px; height: 0px">
                <asp:Label ID="lblCounterAccountNumber" runat="server" Text="Counter Account Number:"></asp:Label></td>
            <td style="width: 225px; height: 0px">
                <asp:TextBox ID="txtCounterAccountNumber" runat="server" Width="165px" SkinID="custom-width" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlCounterAccountName" runat="server" Visible="false">
        <tr>
            <td style="width: 170px; height: 0px">
                <asp:Label ID="lblCounterAccountName" runat="server" Text="Counter Account Name:"></asp:Label></td>
            <td style="width: 225px; height: 0px">
                <asp:TextBox ID="txtCounterAccountName" runat="server" Width="165px" SkinID="custom-width" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlContactName" runat="server" Visible="true">
        <tr>
            <td style="width: 170px; height: 24px">
                <asp:Label ID="lblContactName" runat="server" Text="Contact Name:"></asp:Label></td>
            <td style="width: 225px; height: 24px">
                <asp:TextBox ID="txtContactName" runat="server" Width="165px" SkinID="custom-width" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlAccountNumber" runat="server" Visible="true">
        <tr>
            <td style="width: 170px; height: 24px">
                <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number:"></asp:Label></td>
            <td style="width: 225px; height: 24px">
                <asp:TextBox ID="txtAccountNumber" runat="server" Width="165px" SkinID="custom-width" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlCblContactActive" runat="server" Visible="false">
        <tr>
            <td style="width: 170px; height: 0px">
                <asp:Label ID="lblContactActive" runat="server" Text="Status:"></asp:Label></td>
            <td style="width: 225px; height: 0px; position:relative; left:-3px;">
                <asp:CheckBoxList ID="cblContactActive" runat="server" RepeatDirection="Horizontal" Width="160px" >
                    <asp:ListItem Value="ACTIVE" Selected="true">Active</asp:ListItem>
                    <asp:ListItem Value="INACTIVE">Inactive</asp:ListItem>
                </asp:CheckBoxList>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlIsPublic" runat="server" Visible="false">
        <tr>
            <td style="width: 170px; height: 0px">
                <asp:Label ID="lblType" runat="server" Text="Type:"></asp:Label></td>
            <td style="width: 225px; height: 0px;">
				<asp:CheckBox ID="chkIsPublic" runat="server" Text="Is Public"  />
			</td>
        </tr>
    </asp:Panel>
    
</table>
</td>
<td style="width: 30px"></td>
<td style="width: 80px; vertical-align: bottom">
    <div style="position: relative">
        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                    CommandName="SearchAccounts" CausesValidation="False" />
    </div>
</td>
</tr>
</table>
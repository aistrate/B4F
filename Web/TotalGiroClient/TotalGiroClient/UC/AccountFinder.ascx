<%@ Control Language="C#" 
            Inherits="B4F.TotalGiro.Client.Web.UC.AccountFinder" Codebehind="AccountFinder.ascx.cs" %>

<table cellpadding="4" cellspacing="0">
<tr>
<td>
<table cellpadding="0" cellspacing="0">
    <tr>
        <td style="width: 140px; height: 28px">
            <asp:Label ID="lblAssetManager" runat="server" Text="Asset Manager:"></asp:Label>
        </td>
        <td style="width: 260px">
            <asp:MultiView ID="mvwAssetManager" runat="server" ActiveViewIndex="0" EnableTheming="True">
                <asp:View ID="vwAssetManagerName" runat="server">
                    <asp:Label ID="lblAssetManagerName" runat="server" Font-Bold="True"></asp:Label>
                </asp:View>
                <asp:View ID="vwAssetManagerList" runat="server">
                    <asp:DropDownList ID="ddlAssetManager" runat="server" Width="250px" SkinID="custom-width" DataSourceID="odsAssetManager" 
                                      DataTextField="CompanyName" DataValueField="Key" AutoPostBack="False" 
                                      OnSelectedIndexChanged="ddlAssetManager_SelectedIndexChanged" >
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                        TypeName="B4F.TotalGiro.ClientApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
                </asp:View>
            </asp:MultiView>
        </td>
    </tr>
    <asp:Panel ID="pnlRemisier" runat="server" Visible="false">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblRemisier" runat="server" Text="Remisier:"></asp:Label>
            </td>
            <td>
                <asp:MultiView ID="mvwRemisier" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwRemisierName" runat="server">
                        <asp:Label ID="lblRemisierName" runat="server" Font-Bold="True"></asp:Label>
                    </asp:View>
                    <asp:View ID="vwRemisierList" runat="server">
                        <asp:DropDownList ID="ddlRemisier" runat="server" Width="250px" SkinID="custom-width" DataSourceID="odsRemisier"
                                          DataTextField="DisplayNameAndRefNumber" DataValueField="Key" 
                                          OnSelectedIndexChanged="ddlRemisier_SelectedIndexChanged" >
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsRemisier" runat="server" SelectMethod="GetRemisiers"
                            TypeName="B4F.TotalGiro.ClientApplicationLayer.UC.AccountFinderAdapter" OldValuesParameterFormatString="original_{0}">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                                      PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlRemisierEmployee" runat="server" Visible="false">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblRemisierEmployee" runat="server" Text="Remisier Employee:"></asp:Label>
            </td>
            <td>
                <asp:MultiView ID="mvwRemisierEmployee" runat="server" ActiveViewIndex="0" EnableTheming="True">
                    <asp:View ID="vwRemisierEmployeeName" runat="server">
                        <asp:Label ID="lblRemisierEmployeeName" runat="server" Font-Bold="True"></asp:Label>
                    </asp:View>
                    <asp:View ID="vwRemisierEmployeeList" runat="server">
                        <asp:DropDownList ID="ddlRemisierEmployee" runat="server" Width="250px" SkinID="custom-width" DataSourceID="odsRemisierEmployee"
                                          DataTextField="FullNameLastNameFirst" DataValueField="Key" >
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsRemisierEmployee" runat="server" SelectMethod="GetRemisierEmployees"
                            TypeName="B4F.TotalGiro.ClientApplicationLayer.UC.AccountFinderAdapter" >
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlRemisier" DefaultValue="0" Name="remisierId"
                                                      PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlModelPortfolio" runat="server" Visible="false">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblModelPortfolio" runat="server" Text="Model Portfolio:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlModelPortfolio" runat="server" Width="250px" SkinID="custom-width" DataSourceID="odsModelPortfolio" 
                    DataTextField="ModelName" DataValueField="Key">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios"
                    TypeName="B4F.TotalGiro.ClientApplicationLayer.UC.AccountFinderAdapter" OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                              PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlAccountNumber" runat="server" Visible="true">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number:"></asp:Label>
             </td>
            <td>
                <asp:TextBox ID="txtAccountNumber" runat="server" Width="246px" SkinID="custom-width"></asp:TextBox>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlAccountName" runat="server" Visible="true">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblAccountName" runat="server" Text="Account Name:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtAccountName" runat="server" Width="246px" SkinID="custom-width"></asp:TextBox>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlAccountStatus" runat="server" Visible="false">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblAccountStatus" runat="server" Text="Account Status:"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="cbAccountStatusActive" runat="server" Text="Active" Width="70px" Checked="true" />
                <asp:CheckBox ID="cbAccountStatusInactive" runat="server" Text="Inactive" Width="80px" Checked="false" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlEmailStatus" runat="server" Visible="false">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblEmailStatus" runat="server" Text="E-mail:"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="cbEmailStatusYes" runat="server" Text="Yes" Width="70px" Checked="true" />
                <asp:CheckBox ID="cbEmailStatusNo" runat="server" Text="No" Width="80px" Checked="true" />
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="pnlLoginStatus" runat="server" Visible="false">
        <tr>
            <td style="height: 28px">
                <asp:Label ID="lblLoginStatus" runat="server" Text="Login Status:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlLoginStatus" runat="server" Visible="True" Width="250px" SkinID="custom-width">
                    <asp:ListItem Value="Any" Selected="True">Any</asp:ListItem>
                    <asp:ListItem Value="NoLogin">No Login</asp:ListItem>
                    <asp:ListItem Value="LoginSentNoPassword">Login Sent - No Password</asp:ListItem>
                    <asp:ListItem Value="PasswordSentActive">Password Sent - Active</asp:ListItem>
                    <asp:ListItem Value="PasswordSentInactive">Password Sent - Inactive</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </asp:Panel>
    
</table>
</td>
<td style="width: 20px">
</td>
<td style="width: 110px; vertical-align: bottom">
    <div style="position: relative">
        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" style="position: relative; top: -2px"
                    Width="100px" CommandName="SearchAccounts" CausesValidation="False" />
    </div>
</td>
</tr>
</table>

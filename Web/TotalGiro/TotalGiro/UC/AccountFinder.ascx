<%@ Control Language="C#" CodeFile="AccountFinder.ascx.cs" Inherits="AccountFinder" %>
<table width="440px">
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width: 130px; height: 24px">
                        <asp:Label ID="Label1" runat="server" Text="Asset Manager:"></asp:Label>
                    </td>
                    <td style="width: 190px">
                        <asp:MultiView ID="mvwAssetManager" runat="server" ActiveViewIndex="0" EnableTheming="True">
                            <asp:View ID="vwAssetManager" runat="server">
                                <asp:Label ID="lblAssetManager" runat="server" Font-Bold="True"></asp:Label>
                            </asp:View>
                            <asp:View ID="vwStichting" runat="server">
                                <asp:DropDownList ID="ddlAssetManager" runat="server" DataSourceID="odsAssetManager"
                                    DataTextField="CompanyName" DataValueField="Key" AutoPostBack="False">
                                </asp:DropDownList>
                                <asp:ObjectDataSource ID="odsAssetManager" runat="server" SelectMethod="GetAssetManagers"
                                    TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
                <asp:Panel ID="pnlRemisier" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblRemisier" runat="server" Text="Remisier:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlRemisier" runat="server" DataSourceID="odsRemisier"
                                DataTextField="DisplayNameAndRefNumber" DataValueField="Key" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsRemisier" runat="server" SelectMethod="GetRemisiers"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlRemisierEmployee" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblRemisierEmployee" runat="server" Text="Remisier Employee:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlRemisierEmployee" runat="server" DataSourceID="odsRemisierEmployee"
                                DataTextField="Employee_FullNameLastNameFirst" DataValueField="Key" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsRemisierEmployee" runat="server" SelectMethod="GetRemisierEmployees"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlRemisier" DefaultValue="0" Name="remisierId"
                                        PropertyName="SelectedValue" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlLifecycle" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblLifecycle" runat="server" Text="Lifecycle:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlLifecycle" runat="server" DataSourceID="odsLifecycle"
                                DataTextField="Name" DataValueField="Key" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsLifecycle" runat="server" SelectMethod="GetLifecycles"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter" >
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                        PropertyName="SelectedValue" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlContactActive" DefaultValue="1" Name="activityFilter"
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlModelPortfolio" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblModelPortfolio" runat="server" Text="Model Portfolio:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlModelPortfolio" runat="server" DataSourceID="odsModelPortfolio"
                                DataTextField="ModelName" DataValueField="Key" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter" OldValuesParameterFormatString="original_{0}">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlAssetManager" DefaultValue="0" Name="assetManagerId"
                                        PropertyName="SelectedValue" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlContactActive" DefaultValue="1" Name="activityFilter"
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlAccountNumber" runat="server" Visible="true">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblAccountNumber" runat="server" Text="Account Number:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAccountNumber" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlAccountName" runat="server" Visible="true">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblAccountName" runat="server" Text="Account Name:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAccountName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlBsN_KvK" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblBsN_KvK" runat="server" Text="BSN / KvK:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBsN_KvK" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlTegenrekening" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblTegenrekening" runat="server" Text="Tegenrekening:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTegenrekening" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlCblContactActive" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblContactActive" runat="server" Text="Status:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlContactActive" runat="server" DataSourceID="odsContactActive"
                                DataTextField="Status" DataValueField="ID" AutoPostBack="true" OnSelectedIndexChanged="ddlContactActive_SelectedIndexChanged" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsContactActive" runat="server" SelectMethod="GetAccountStatuses"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlddlTradeability" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblAccountTradeable" runat="server" Text="Tradeability:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTradeability" runat="server" DataSourceID="odsTradeability"
                                DataTextField="Tradeability" DataValueField="ID" AutoPostBack="False">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsTradeability" runat="server" SelectMethod="GetAccountTradeability"
                                TypeName="B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter"></asp:ObjectDataSource>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlYear" runat="server" Visible="false">
                    <tr>
                        <td style="height: 24px">
                            <asp:Label ID="lblYear" runat="server" Text="Account active in:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlYear" runat="server" />
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </td>
        <td style="width: 100px; vertical-align: bottom">
            <div style="position: relative">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Style="position: relative; top: -2px" CommandName="SearchAccounts" CausesValidation="False"
                    Width="90px" />
            </div>
        </td>
    </tr>
</table>

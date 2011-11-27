<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" CodeFile="PortfolioPositions.aspx.cs" Inherits="PortfolioPositions" %>

<%@ Register Src="~/UC/PortfolioNavigationBar.ascx" TagName="PortfolioNavigationBar" TagPrefix="b4f" %>
<%@ Import Namespace="B4F.TotalGiro.Utils" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />
    
    <b4f:PortfolioNavigationBar ID="ctlPortfolioNavigationBar" runat="server" ShowPortfolio="false" />
    
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="lblAccountLabel" runat="server" Text="Rekening:"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="300px" DataSourceID="odsAccount" 
                    DataTextField="DisplayNumberWithName" DataValueField="Key" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" 
                    AutoPostBack="True" OnDataBound="ddlAccount_DataBound">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsAccount" runat="server" SelectMethod="GetContactAccounts"
                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Common.CommonAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="contactId" SessionField="ContactId" Type="Int32" DefaultValue="0" /> 
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr style="height: 4px" >
            <td style="width: 155px"></td>
            <td style="width: 360px"></td>
        </tr>
    </table>
    
    <asp:Panel ID="pnlPortfolio" runat="server" Visible="False" Width="780px">
        <hr style="width: 780px"/>
        <table cellpadding="0" cellspacing="0">
            <tr style="height: 2px" >
                <td style="width: 155px"></td>
                <td style="width: 600px"></td>
            </tr>
            <tr>
                <td style="height: 25px; white-space: nowrap">
                    <asp:Label ID="Label1" runat="server" Text="Rekening:"></asp:Label></td>
                <td style="white-space: nowrap">
                    <asp:Label ID="lblAccountNumberWithName" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 25px; white-space: nowrap">
                    <asp:Label ID="Label4" runat="server" Text="Startdatum beheer:"></asp:Label></td>
                <td style="white-space: nowrap">
                    <asp:Label ID="lblManagementStartDate" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 25px; white-space: nowrap">
                    <asp:Label ID="lblAccountHoldersLabel" runat="server" Text="Rekeninghouder:"></asp:Label></td>
                <td style="white-space: nowrap">
                    <asp:Label ID="lblPrimaryAccountHolder" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <asp:Panel ID="pnlSecondaryAccountHolder" runat="server" Visible="false">
                <tr>
                    <td style="height: 25px; white-space: nowrap"></td>
                    <td style="white-space: nowrap">
                        <asp:Label ID="lblSecondaryAccountHolder" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
            </asp:Panel>
            <tr>
                <td style="height: 25px; white-space: nowrap">
                    <asp:Label ID="Label8" runat="server" Text="Adres:"></asp:Label></td>
                <td style="white-space: nowrap">
                    <asp:Label ID="lblStreetAddressLine" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 25px; white-space: nowrap"></td>
                <td style="white-space: nowrap">
                    <asp:Label ID="lblCityAddressLine" runat="server" Font-Bold="True"></asp:Label></td>
            </tr>
            <asp:Panel ID="pnlCountryAddressLine" runat="server" Visible="false">
                <tr>
                    <td style="height: 25px; white-space: nowrap"></td>
                    <td style="white-space: nowrap">
                        <asp:Label ID="lblCountryAddressLine" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlVerpandSoort" runat="server" Visible="false">
                <tr>
                    <td style="height: 25px; white-space: nowrap">
                        <asp:Label ID="Label3" runat="server" Text="Verpand soort:"></asp:Label></td>
                    <td style="white-space: nowrap">
                        <asp:Label ID="lblVerpandSoort" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlPandhouder" runat="server" Visible="false">
                <tr>
                    <td style="height: 25px; white-space: nowrap">
                        <asp:Label ID="Label9" runat="server" Text="Pandhouder:"></asp:Label></td>
                    <td style="white-space: nowrap">
                        <asp:Label ID="lblPandhouder" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlRemisier" runat="server" Visible="false">
                <tr>
                    <td style="height: 25px; white-space: nowrap">
                        <asp:Label ID="Label10" runat="server" Text="Remisier:"></asp:Label></td>
                    <td style="white-space: nowrap">
                        <asp:Label ID="lblRemisier" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlRemisierEmployee" runat="server" Visible="false">
                <tr>
                    <td style="height: 25px; white-space: nowrap">
                        <asp:Label ID="Label12" runat="server" Text="Remisier employee:"></asp:Label></td>
                    <td style="white-space: nowrap">
                        <asp:Label ID="lblRemisierEmployee" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
            </asp:Panel>
            <tr style="height: 1px" >
                <td colspan="2"></td>
            </tr>
        </table>
        <hr style="width: 780px"/>

        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="width: 470px; text-align: left; vertical-align: top">
                    <table cellpadding="0" cellspacing="0">
                        <tr style="height: 2px" >
                            <td style="width: 155px"></td>
                            <td style="width: 300px"></td>
                        </tr>
                        <tr>
                            <td style="height: 25px; white-space: nowrap">
                                <asp:Label ID="Label6" runat="server" Text="Model:"></asp:Label></td>
                            <td style="white-space: nowrap">
                                <asp:Label ID="lblModelName" runat="server" Font-Bold="True" ></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="height: 25px; white-space: nowrap">
                                <asp:Label ID="Label11" runat="server" Text="Laatste herbalancering:"></asp:Label></td>
                            <td style="white-space: nowrap">
                                <asp:Label ID="lblLastRebalance" runat="server" Font-Bold="True"></asp:Label></td>
                        </tr>
                        <asp:Panel ID="pnlCurrentRebalance" runat="server" Visible="false">
                            <tr>
                                <td style="height: 25px; white-space: nowrap">
                                    <asp:Label ID="Label15" runat="server" Text="Huidige herbalancering:"></asp:Label></td>
                                <td style="white-space: nowrap">
                                    <asp:Label ID="lblCurrentRebalance" runat="server" Font-Bold="True"></asp:Label></td>
                            </tr>
                        </asp:Panel>
                    </table>
                </td>
        
                <td style="width: 250px; text-align: left; vertical-align: top">
                    <table cellpadding="0" cellspacing="0">
                        <tr style="height: 2px" >
                            <td style="width: 120px"></td>
                            <td style="width: 120px"></td>
                        </tr>
                        <asp:Panel ID="pnlPositionValueDetails" runat="server" Visible="false">
                            <tr>
                                <td style="height: 25px; white-space: nowrap">
                                    <asp:Label ID="Label7" runat="server" Text="Contante waarde:"></asp:Label></td>
                                <td style="white-space: nowrap" align="right">
                                    <asp:Label ID="lblTotalCash" runat="server" Font-Bold="True"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="height: 25px; white-space: nowrap">
                                    <asp:Label ID="Label5" runat="server" Text="Waarde portefeuille:"></asp:Label></td>
                                <td style="white-space: nowrap" align="right">
                                    <asp:Label ID="lblTotalPositions" runat="server" Font-Bold="True"></asp:Label></td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td style="height: 25px; white-space: nowrap">
                                <asp:Label ID="Label2" runat="server" Text="Totale waarde:"></asp:Label></td>
                            <td style="white-space: nowrap" align="right">
                                <asp:Label ID="lblTotal" runat="server" Font-Bold="True"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="height: 15px" >
                <td colspan="2"></td>
            </tr>
        </table>
        
        <table cellpadding="0" cellspacing="0">
            <tr>
                <asp:Panel ID="pnlShowOpenPositions" runat="server" Visible="false">
                    <td style="white-space: nowrap">
                        <b4f:ArrowsLinkButton ID="lnkShowOpenPositions" runat="server" SkinID="padding" CommandArgument="0"
                                              OnCommand="lnkShowActivePortfolioView_Command">Toon huidige posities</b4f:ArrowsLinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </asp:Panel>
                <asp:Panel ID="pnlShowPortfolioComponents" runat="server" Visible="true">
                    <td style="white-space: nowrap">
                        <b4f:ArrowsLinkButton ID="lnkShowPortfolioComponents" runat="server" SkinID="padding" CommandArgument="1"
                                              OnCommand="lnkShowActivePortfolioView_Command">Toon model opbouw</b4f:ArrowsLinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </asp:Panel>
                <asp:Panel ID="pnlShowClosedPositions" runat="server" Visible="true">
                    <td style="white-space: nowrap">
                        <b4f:ArrowsLinkButton ID="lnkShowClosedPositions" runat="server" SkinID="padding" CommandArgument="2"
                                              OnCommand="lnkShowActivePortfolioView_Command">Toon gesloten posities</b4f:ArrowsLinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </asp:Panel>
                <td style="white-space: nowrap">
                    <b4f:ArrowsLinkButton ID="lnkCashMutations" runat="server" SkinID="padding" 
                                          PostBackUrl="~/Portfolio/PositionTxsCash.aspx">Verwerkte geldboekingen</b4f:ArrowsLinkButton>
                </td>
            </tr>
            <tr>
                <td style="height: 12px"></td>
                <td></td>
                <td></td>
            </tr>
        </table>
        
        <table cellpadding="0" cellspacing="0">
        <tr><td style="height: 450px; vertical-align: top">
        
        <asp:MultiView ID="mvwPositions" runat="server" ActiveViewIndex="0">
            
            <asp:View ID="vwOpenPositions" runat="server">
            
                <asp:GridView ID="gvOpenPositions" runat="server" AllowSorting="True" AutoGenerateColumns="False" Caption="Huidige posities" 
                              CaptionAlign="Left" DataSourceID="odsOpenPositions" PageSize="25" AllowPaging="True" DataKeyNames="Key" 
                              SkinID="custom-width" Width="780px" >
                    <Columns>
                        <asp:TemplateField HeaderText="Fonds" SortExpression="InstrumentName">
                            <ItemStyle wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle wrap="False" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <b4f:ArrowsLinkButton ID="lnkInstrument" runat="server" CommandArgument='<%# Eval("Key") %>'
                                                      CommandName="PositionTxsSecurities" OnCommand="lnkInstrument_Command">
                                    <%# Eval("InstrumentName")%></b4f:ArrowsLinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Size" HeaderText="Aantal" SortExpression="Size" DataFormatString="{0:#,##0.000000}">
                            <ItemStyle wrap="False" horizontalalign="Right" />
                            <HeaderStyle wrap="False" horizontalalign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Percentage" HeaderText="Procentueel" SortExpression="Percentage" DataFormatString="{0:##0.00}%">
                            <ItemStyle wrap="False" horizontalalign="Right" />
                            <HeaderStyle wrap="False" horizontalalign="Right" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Model" SortExpression="ModelAllocation">
                            <ItemStyle horizontalalign="Right" wrap="False" />
                            <HeaderStyle horizontalalign="Right" wrap="False" />
                            <ItemTemplate>
                                <%# ((decimal)Eval("ModelAllocation") != 0m ? Eval("ModelAllocation", "{0:##0.00}%") : "")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Koers" SortExpression="Price">
                            <ItemStyle wrap="False" horizontalalign="Right" />
                            <HeaderStyle wrap="False" horizontalalign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PriceDate" HeaderText="Koersdatum" DataFormatString="{0:dd-MM-yyyy}" 
                                        HtmlEncode="False" SortExpression="PriceDate">
                            <ItemStyle wrap="False" horizontalalign="Right" />
                            <HeaderStyle wrap="False" horizontalalign="Right" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Verrekenkoers" SortExpression="ExchangeRate">
                            <ItemStyle horizontalalign="Right" wrap="False" />
                            <HeaderStyle horizontalalign="Right" wrap="False" />
                            <ItemTemplate>
                                <%# ((decimal)Eval("ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}") : "")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Value" HeaderText="Waarde" SortExpression="Value">
                            <ItemStyle wrap="False" horizontalalign="Right" Font-Bold="True" />
                            <HeaderStyle wrap="False" horizontalalign="Right" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsOpenPositions" runat="server" SelectMethod="GetOpenFundPositions"
                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.PortfolioPositionsAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlAccount" DefaultValue="0" Name="accountId" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                
            </asp:View>
            
            <asp:View ID="vwPortfolioComponents" runat="server">

                <asp:UpdatePanel ID="updPortfolioComponents" runat="server">
                    <ContentTemplate>
                        
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                        
                                    <asp:GridView ID="gvPortfolioComponents" runat="server" AllowSorting="False" AutoGenerateColumns="False" 
                                                  Caption="Model opbouw" CaptionAlign="Left" DataSourceID="odsPortfolioComponents" 
                                                  AllowPaging="False" DataKeyNames="LineNumber" SkinID="custom-width" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Component">
                                                <ItemStyle wrap="False" HorizontalAlign="Left" CssClass="no-padding" />
                                                <HeaderStyle wrap="False" HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <table cellpadding="0" cellspacing="0">
                                                        <tr class="multiline">
                                                            <%# "<td style='width: 17px' class='treeview'></td>".RepeatString(TreeViewNodes.GetNode((int)Eval("LineNumber")).Depth) %>
                                                            
                                                            <td style="width: 17px" class="treeview">
                                                                <asp:ImageButton ID="ibtExpand" runat="server" CssClass="plain"
                                                                                 ImageUrl="~/Images/Tree/Expand.gif"
                                                                                 Visible='<%# TreeViewNodes.GetNode((int)Eval("LineNumber")).HasChildren && !TreeViewNodes.GetNode((int)Eval("LineNumber")).Expanded %>' 
                                                                                 CommandArgument='<%# (int)Eval("LineNumber") %>'
                                                                                 OnCommand="ibtExpandCollapse_Command" />
                                                                <asp:ImageButton ID="ibtCollapse" runat="server" CssClass="plain"
                                                                                 ImageUrl="~/Images/Tree/Collapse.gif"
                                                                                 Visible='<%# TreeViewNodes.GetNode((int)Eval("LineNumber")).HasChildren && TreeViewNodes.GetNode((int)Eval("LineNumber")).Expanded %>' 
                                                                                 CommandArgument='<%# (int)Eval("LineNumber") %>'
                                                                                 OnCommand="ibtExpandCollapse_Command" />
                                                            </td>
                                                            
                                                            <td style="height: 17px" class="treeview-text">
                                                                <asp:Label ID="lblModel" runat="server" Font-Bold="True"
                                                                           Visible='<%# (bool)Eval("IsModel") %>'>
                                                                    <%# (string)Eval("ComponentName") %>
                                                                </asp:Label>
                                                                <b4f:ArrowsLinkButton ID="lnkInstrument" runat="server" 
                                                                                      CommandArgument='<%# Eval("PositionId") %>'
                                                                                      CommandName="PositionTxsSecurities" 
                                                                                      OnCommand="lnkInstrument_Command"
                                                                                      Visible='<%# (bool)Eval("IsInstrument") %>'>
                                                                    <%# (string)Eval("ComponentName") %>
                                                                </b4f:ArrowsLinkButton>
                                                                <b4f:ArrowsLinkButton ID="lnkCash" runat="server" 
                                                                                      PostBackUrl="~/Portfolio/PositionTxsCash.aspx"
                                                                                      Visible='<%# (bool)Eval("IsCash") %>'>
                                                                    Geld</b4f:ArrowsLinkButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div style="height: 0px; width: 350px"></div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Procentueel">
                                                <ItemStyle horizontalalign="Right" wrap="False" />
                                                <HeaderStyle horizontalalign="Right" wrap="False" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPercentage" runat="server" 
                                                               Font-Underline='<%# (bool)Eval("IsModel") && TreeViewNodes.GetNode((int)Eval("LineNumber")).Expanded %>'>
                                                        <%# ((decimal)Eval("Percentage") != 0m ? ((decimal)Eval("Percentage")).ToString("##0.00%") : "") %>
                                                    </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Model">
                                                <ItemStyle horizontalalign="Right" wrap="False" />
                                                <HeaderStyle horizontalalign="Right" wrap="False" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblModelAllocation" runat="server"
                                                               Font-Underline='<%# (bool)Eval("IsModel") && TreeViewNodes.GetNode((int)Eval("LineNumber")).Expanded %>'>
                                                        <%# ((decimal)Eval("ModelAllocation") != 0m ? ((decimal)Eval("ModelAllocation")).ToString("##0.00%") : "") %>
                                                    </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Waarde">
                                                <ItemStyle horizontalalign="Right" wrap="False" Font-Bold="True" />
                                                <HeaderStyle horizontalalign="Right" wrap="False" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblValue" runat="server"
                                                               Font-Underline='<%# (bool)Eval("IsModel") && TreeViewNodes.GetNode((int)Eval("LineNumber")).Expanded %>'>
                                                        <%# Eval("Value") %>
                                                    </asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    
                                    <asp:ObjectDataSource ID="odsPortfolioComponents" runat="server" SelectMethod="GetPortfolioComponents"
                                        TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.PortfolioPositionsAdapter"
                                        OnSelected="odsPortfolioComponents_Selected">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="ddlAccount" DefaultValue="0" Name="accountId" 
                                                                  PropertyName="SelectedValue" Type="Int32" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                
                                </td>
                                <td style="width: 100px; white-space: nowrap; text-align: right; vertical-align: top">
                                    <asp:UpdateProgress ID="upSpinner" runat="server" DisplayAfter="250">
                                        <ProgressTemplate>
                                            <asp:Image ID="imgWheel" runat="server" CssClass="plain" 
                                                ImageUrl="~/Images/Wheel.gif" />
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                </td>
                            </tr>
                        </table>
                        
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </asp:View>
                
            <asp:View ID="vwClosedPositions" runat="server">
            
                <asp:GridView ID="gvClosedPositions" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                    Caption="Gesloten posities" CaptionAlign="Left" DataSourceID="odsClosedPositions" PageSize="25" AllowPaging="True" 
                    DataKeyNames="Key" SkinID="custom-width" Width="550px">
                    <Columns>
                        <asp:TemplateField HeaderText="Fonds" SortExpression="InstrumentName">
                            <ItemStyle wrap="False" HorizontalAlign="Left" />
                            <HeaderStyle wrap="False" HorizontalAlign="Left" />
                            <ItemTemplate>
                                <b4f:ArrowsLinkButton ID="lnkInstrument" runat="server" CommandArgument='<%# Eval("Key") %>'
                                                      CommandName="PositionTxsSecurities" OnCommand="lnkInstrument_Command">
                                    <%# Eval("InstrumentName")%></b4f:ArrowsLinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Koers" SortExpression="Price">
                            <ItemStyle wrap="False" horizontalalign="Right" />
                            <HeaderStyle wrap="False" horizontalalign="Right" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Verrekenkoers" SortExpression="ExchangeRate">
                            <ItemStyle horizontalalign="Right" wrap="False" />
                            <HeaderStyle horizontalalign="Right" wrap="False" />
                            <ItemTemplate>
                                <%# ((decimal)Eval("ExchangeRate") != 1m ? Eval("ExchangeRate", "{0:###.0000}") : "")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="odsClosedPositions" runat="server" SelectMethod="GetClosedFundPositions"
                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.PortfolioPositionsAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlAccount" DefaultValue="0" Name="accountId" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                
            </asp:View>
            
        </asp:MultiView>
        
        </td></tr>
        </table>
        
        <br />
        <p class="info" style="width: 770px" >
            De getoonde waarde van uw portefeuille wordt berekend op basis van de laatste gepubliceerde koersen van de 
            verschillende fondsen. Deze koersen zijn altijd enkele dagen vertraagd. In beweeglijke markten kan de huidige 
            waarde sterk afwijken van de getoonde waarde.
        </p>
        
    </asp:Panel>
    
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>
    
</asp:Content>


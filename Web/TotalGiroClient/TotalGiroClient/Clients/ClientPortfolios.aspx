<%@ Page Title="" Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Clients.ClientPortfolios" Codebehind="ClientPortfolios.aspx.cs" %>

<%@ Register Src="~/UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="b4f" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="B4F.TotalGiro.ClientApplicationLayer.Clients" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <div style="position: relative">
        <div style="position: relative; left: -4px; top: -9px">
            <asp:Panel ID="pnlAccountFinder" runat="server" Width="900px">
                <b4f:AccountFinder ID="ctlAccountFinder" runat="server" AccountNameLabel="Contact Name:"
                                   ShowRemisier="true" ShowRemisierEmployee="true" />
            </asp:Panel>
        </div>
    </div>
    
    <div style="height: 3px"></div>
    <asp:Panel ID="pnlClients" runat="server" Visible="false">
        <b4f:MultipleSelectionGridView ID="gvClients" runat="server" DataSourceID="odsClients" AutoGenerateColumns="False" DataKeyNames="Key" 
                      AllowPaging="True" AllowSorting="True" Caption="Clients" CaptionAlign="Top" PageSize="20" 
                      SkinID="custom-width" Width="950px" OnDataBound="gvClients_DataBound" MultipleSelection="false" >
            <Columns>
                <asp:TemplateField HeaderText="Contact" SortExpression="ShortName">
                    <ItemStyle wrap="False" HorizontalAlign="Left"  Width="300px" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <b4f:TruncatedLabel id="lblFullName" runat="server" LongText='<%# Eval("FullName")%>' MaxLength="60" 
                                            CustomToolTip="True" ToolTip='<%# Eval("FullAddress") %>' >
                        </b4f:TruncatedLabel>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DisplayContactType" HeaderText="Type" SortExpression="DisplayContactType">
                    <ItemStyle wrap="False" HorizontalAlign="Left" Width="65px" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Accounts">
                    <ItemStyle Wrap="False" HorizontalAlign="Left" Width="250px" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label id="lblActiveAccountNumbers" runat="server"
                                   Text='<%# FormatAccountNumbersByColor(GetActiveAccountNumbers((int)Eval("Key"))) %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemStyle Wrap="False" HorizontalAlign="Left" Width="0px" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <%# ShowFlagsPerContact ? FormatTextByColor("", GetContactColor((int)Eval("Key"))) : ""%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtPortfolio" runat="server" CommandName="Portfolio" CommandArgument='<%# Eval("Key") %>'
                                        OnClick="linkButtonField_Click" Text="Portefeuille" ToolTip="View portfolio for this contact">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtCharts" runat="server" CommandName="Charts" CommandArgument='<%# Eval("Key") %>'
                                        OnClick="linkButtonField_Click" Text="Grafieken" ToolTip="View charts for this contact">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtPlanner" runat="server" CommandName="Planner" CommandArgument='<%# Eval("Key") %>'
                                        OnClick="linkButtonField_Click" Text="Monitor Portefeuille" 
                                        ToolTip="View financial planner for this contact">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtNotas" runat="server" CommandName="Notas" CommandArgument='<%# Eval("Key") %>'
                                        OnClick="linkButtonField_Click" Text="Afschriften" ToolTip="View notas for this contact">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtFinancialReports" runat="server" CommandName="FinancialReports" CommandArgument='<%# Eval("Key") %>'
                                        OnClick="linkButtonField_Click" Text="Rapportages" ToolTip="View financial reports for this contact">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </b4f:MultipleSelectionGridView>
        
        <asp:ObjectDataSource ID="odsClients" runat="server" SelectMethod="GetClientContacts"
                              TypeName="B4F.TotalGiro.ClientApplicationLayer.Clients.ClientPortfoliosAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierId" PropertyName="RemisierId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierEmployeeId" PropertyName="RemisierEmployeeId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactName" PropertyName="AccountName"
                    Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </asp:Panel>
        
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>
    
</asp:Content>


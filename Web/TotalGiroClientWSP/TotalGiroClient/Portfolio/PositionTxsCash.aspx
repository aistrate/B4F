<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" CodeFile="PositionTxsCash.aspx.cs" Inherits="PositionTxsCash" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="2">
                <b4f:ArrowsLinkButton ID="lnkPortfolioPositions" runat="server" SkinID="padding"
                                      PostBackUrl="~/Portfolio/PortfolioPositions.aspx">Terug naar Portefeuille</b4f:ArrowsLinkButton>
            </td>
        </tr>
        <tr style="height: 20px" >
            <td style="width: 135px"></td>
            <td style="width: 500px"></td>
        </tr>
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="Label1" runat="server">Rekening:</asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:Label ID="lblAccount" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="Label2" runat="server">Contante waarde:</asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:Label ID="lblValue" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr style="height: 16px" >
            <td colspan="2"></td>
        </tr>
    </table>
    
    <asp:GridView ID="gvCashMutations" runat="server" AllowPaging="True" AllowSorting="True" DataSourceID="odsCashMutations" PageSize="50" 
                  Caption="Verwerkte geldboekingen" CaptionAlign="Left" DataKeyNames="Key" SkinID="custom-width" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="TransactionDate" HeaderText="Datum" DataFormatString="{0:dd-MM-yyyy}" 
                            HtmlEncode="False" SortExpression="TransactionDate">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" Width="75px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Omschrijving" SortExpression="FullDescription">
                <ItemTemplate>
                    <%# ((string)Eval("FullDescription")).Replace("\n", "<br />") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" Width="575px" />
            </asp:TemplateField>
            <asp:BoundField DataField="DisplayAmount" HeaderText="Bedrag" SortExpression="DisplayAmount">
                <ItemStyle wrap="False" horizontalalign="Right" Font-Bold="True" />
                <HeaderStyle wrap="False" horizontalalign="Right" Width="100px" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsCashMutations" runat="server" SelectMethod="GetCashMutations"
        TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.PositionTxsCashAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="accountId" SessionField="AccountId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>
</asp:Content>


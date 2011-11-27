<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" CodeFile="PositionTxsSecurities.aspx.cs" Inherits="PositionTxsSecurities" %>

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
                <asp:Label ID="Label2" runat="server">Fonds:</asp:Label>
            </td>
            <td style="white-space: nowrap; padding-left: 5px">
                <asp:DropDownList ID="ddlInstrument" SkinID="custom-width" runat="server" Width="350px" DataSourceID="odsInstrument" 
                    DataTextField="InstrumentDescription" DataValueField="Key" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged" 
                    AutoPostBack="True" OnDataBound="ddlInstrument_DataBound">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetRelatedAccountFundPositions"
                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.PositionTxsSecuritiesAdapter">
                    <SelectParameters>
                        <asp:SessionParameter Name="selectedPositionId" SessionField="PositionId" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="Label3" runat="server">Waarde positie:</asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:Label ID="lblValue" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr style="height: 16px" >
            <td colspan="2"></td>
        </tr>
    </table>
    
    <asp:GridView ID="gvPositionTransactions" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsPositionTransactions" PageSize="50" Caption="Mutaties" CaptionAlign="Left" 
        DataKeyNames="Key" SkinID="custom-width" >
        <Columns>
            <asp:BoundField DataField="TransactionDate" HeaderText="Datum" DataFormatString="{0:dd-MM-yyyy}" 
                            HtmlEncode="False" SortExpression="TransactionDate">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" Width="75px" />
            </asp:BoundField>
            <asp:BoundField DataField="Description" HeaderText="Omschrijving" SortExpression="Description">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" Width="350px" />
            </asp:BoundField>
            <asp:BoundField DataField="Size" HeaderText="Aantal" SortExpression="Size">
                <ItemStyle wrap="False" horizontalalign="Right" />
                <HeaderStyle wrap="False" horizontalalign="Right" Width="100px" />
            </asp:BoundField>
            <asp:BoundField DataField="PriceShortDisplayString" HeaderText="Koers" SortExpression="Price">
                <ItemStyle wrap="False" horizontalalign="Right" />
                <HeaderStyle wrap="False" horizontalalign="Right" Width="100px" />
            </asp:BoundField>
            <asp:BoundField DataField="Value" HeaderText="Waarde" SortExpression="Value">
                <ItemStyle wrap="False" horizontalalign="Right" Font-Bold="True" />
                <HeaderStyle wrap="False" horizontalalign="Right" Width="100px" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsPositionTransactions" runat="server" SelectMethod="GetFundPositionTxs"
        TypeName="B4F.TotalGiro.ClientApplicationLayer.Portfolio.PositionTxsSecuritiesAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlInstrument" DefaultValue="0" Name="positionId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="OrderCreationTest.aspx.cs" Inherits="OrderCreationTest" Title="Single Order Test" Theme="Neutral" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:Panel ID="pnlOrder" runat="server" Height="62px" Width="634px">
        <table style="width: 100%">
            <tr>
                <td style="width: 20%; height: 66px;">
                    <asp:Label ID="lblChooseBankAccount" runat="server">Account</asp:Label>
                </td>
                <td style="width: 40%; height: 66px;">
                    <asp:DropDownList ID="ddlAccounts" runat="server" DataTextField="Number" DataValueField="Key" AutoPostBack="True" OnSelectedIndexChanged="ddlAccounts_SelectedIndexChanged">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="408">EGVL010111</asp:ListItem>
                        <asp:ListItem Value="379">EGVL010103</asp:ListItem>
                        <asp:ListItem Value="419">EGVL010152</asp:ListItem>
                        <asp:ListItem Value="648">EGVL010425</asp:ListItem>
                        <asp:ListItem Value="346">EGVL010055</asp:ListItem>
                        <asp:ListItem Value="343">EGVL010052</asp:ListItem>
                    </asp:DropDownList>
                    
                    <!--<asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetAccounts" 
                        TypeName="B4F.TotalGiro.ApplicationLayer.Test.OrderCreationTestAdapter"></asp:ObjectDataSource>-->
                </td>
                <td style="width: 40%; height: 66px;">
                    <asp:Label ID="lblShowBankAccount" runat="server" Font-Bold="true"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlAccounts"
                        ErrorMessage="Required"></asp:RequiredFieldValidator><asp:Button ID="btnClearOrders" runat="server" CausesValidation="false" OnClick="btnClearOrders_Click" Text="Clear Orders" /><br />
                    </td>
                
            </tr>
            <tr>
                <td style="width: 20%">
                    <asp:Label ID="lblCashPosLabel" runat="server">Cash Position</asp:Label></td>
                <td style="width: 40%">
                    <asp:Label ID="lblCashPos" runat="server"></asp:Label></td>
                <td style="width: 40%">
                </td>
            </tr>
            <tr>
                <td style="width: 20%">
                    <asp:Label ID="lblOpenOrderAmount" runat="server">Open Orders</asp:Label></td>
                <td style="width: 40%">
                    <asp:Label ID="lblOpenOrderCash" runat="server"></asp:Label></td>
                <td style="width: 40%">
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:RadioButtonList ID="rblFundMP" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rblFundMP_SelectedIndexChanged" RepeatDirection="Horizontal">
                        <asp:ListItem>Fund</asp:ListItem>
                        <asp:ListItem>Modelportfolio</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="rblFundMP"
                        ErrorMessage="Required"></asp:RequiredFieldValidator>
                </td>
             </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlModelPortfolio" runat="server" Height="83px" Width="634px" Visible="False">
        <table style="width: 100%">
            <tr>
                <td style="width: 20%">
                    <asp:Label ID="lblModelPortfolioLabel" runat="server" Text="Modelportfolio"></asp:Label>
                </td>
                <td style="width: 40%">
                    <asp:Label ID="lblModelportfolio" runat="server"></asp:Label>
                 </td>
                <td style="width: 40%">
                 </td>
            </tr>
        </table>
        <asp:Label ID="lblErrorMP" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <asp:Button ID="btnBuyMP" runat="server" Text="Buy Modelportfolio" OnClick="btnBuyMP_Click" /></asp:Panel>
    <asp:Panel ID="pnlFund" runat="server" Height="83px" Width="634px" Visible="False">
        <table style="width: 100%">
            <tr>
                <td style="width: 20%">
                    <asp:Label ID="lblInstrument" runat="server" Text="Instrument"></asp:Label></td>
                <td style="width: 40%">
                    <asp:DropDownList
                        ID="ddInstrument" runat="server" DataTextField="Name"
                        DataValueField="Key" Width="166px" OnSelectedIndexChanged="ddInstrument_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Value="225">LU0120308787</asp:ListItem>
                        <asp:ListItem Value="222">LU0099730524</asp:ListItem>
                        <asp:ListItem Value="184">LU0083949973</asp:ListItem>
                        <asp:ListItem Value="200">LU0067888072</asp:ListItem>
                        <asp:ListItem Value="197">GB0002769429</asp:ListItem>
                        <asp:ListItem Value="404">IE0002270589</asp:ListItem>
                        <asp:ListItem Value="711">XS0136338786</asp:ListItem>
                        <asp:ListItem Value="703">XS0133722172</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="width: 40%">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddInstrument"
                        ErrorMessage="Required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="height: 47px">
                </td>
                <td style="height: 47px">
                    <asp:RadioButtonList ID="rblSizeAmount" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rblSizeAmount_SelectedIndexChanged"
                        RepeatDirection="Horizontal">
                        <asp:ListItem>Size</asp:ListItem>
                        <asp:ListItem>Amount</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="height: 47px">
                </td>
            </tr>
            <tr>
                <td style="height: 47px">
                </td>
                <td style="height: 47px">
                    <asp:RadioButtonList ID="rblBuySell" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rblBuySell_SelectedIndexChanged">
        <asp:ListItem>Buy</asp:ListItem>
        <asp:ListItem>Sell</asp:ListItem>
    </asp:RadioButtonList></td>
                <td style="height: 47px">
                    <asp:Label ID="lblBuySellError" runat="server" EnableViewState="False" ForeColor="Red"
                        Text="Combination Size/Buy not possible" Visible="False"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
    <asp:Label ID="lblAmount" runat="server" Text="Amount"></asp:Label>
                </td>
                <td>
    <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="ddlCurrency" runat="server">
                    </asp:DropDownList></td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtQuantity"
                        ErrorMessage="Formaat: ####,##" ValidationExpression="^\d+(\,\d{1,2})?$"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="lblSize" runat="server" Text="Size"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="txtSize" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtSize"
                        ErrorMessage="Formaat: ####,######" ValidationExpression="^\d+(\,\d{1,6})?$"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td style="height: 47px">
                    <asp:Label ID="lblInclExclComm" runat="server" Text="Commission" Visible="False"></asp:Label></td>
                <td style="height: 47px">
                    <asp:RadioButtonList ID="rblInclExclCom" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
                        Visible="False" Width="187px">
                        <asp:ListItem Selected="True">Inclusive</asp:ListItem>
                        <asp:ListItem>Exclusive</asp:ListItem>
                    </asp:RadioButtonList></td>
                <td style="height: 47px">
                </td>
            </tr>
        </table>
        <asp:Label ID="lblErrorPlaceOrder" runat="server" ForeColor="Red"></asp:Label><br />
        &nbsp;<asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click" />&nbsp;<br />
        &nbsp;<asp:ObjectDataSource ID="odsInstrument" runat="server" SelectMethod="GetInstruments"
            TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission.RuleEditAdapter"></asp:ObjectDataSource>
        <br />
        <asp:ObjectDataSource ID="odsCashPos" runat="server"></asp:ObjectDataSource>
        &nbsp;&nbsp;<br />
        &nbsp;&nbsp; &nbsp;
        &nbsp;<br />
        <br />
    </asp:Panel>
    &nbsp;<br />
    &nbsp; &nbsp;<br />
    <asp:GridView ID="GridView1" runat="server" AllowSorting="true" DataSourceID="odsOrders" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="Account_Number" HeaderText="Accountnumber" SortExpression="Account_Number" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Account_ShortName" HeaderText="AccountName" SortExpression="Account_ShortName" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TradedInstrument_DisplayName" HeaderText="TradedInstrument" SortExpression="TradedInstrument_DisplayName" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="DisplayTradedInstrumentIsin" HeaderText="ISIN" SortExpression="DisplayTradedInstrumentIsin" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Side" HeaderText="Side" SortExpression="Side" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Value_DisplayString" HeaderText="Value" SortExpression="Value_DisplayString" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="CommissionInfo" HeaderText="CommissionInfo" SortExpression="CommissionInfo" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="OrderID" HeaderText="OrderID" SortExpression="OrderID" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:ObjectDataSource ID="odsOrders" runat="server" SelectMethod="GetOrders" 
        TypeName="B4F.TotalGiro.ApplicationLayer.Test.OrderCreationTestAdapter"></asp:ObjectDataSource>
    &nbsp;
    <br />

</asp:Content>

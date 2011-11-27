<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="AttachCommRuleToAccount.aspx.cs" Inherits="DataMaintenance_AttachCommRuleToAccount" Title="Attach Commission Rule To Account" %>
<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td style="text-align:left; font-weight: bold;"><div>Account to attach commission rule to: 
                <asp:Label ID="lblPerson" runat="server" /></div>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="600px" style="border: solid 1px black;">
                    <tr>
                        <td style=" background-color: #AAB9C2; height: 20px; border-bottom: solid 1px black;">
                            <asp:Label ID="lblAccountFinder" Font-Bold="true" runat="server" Text="Search Existing Commission Rules" />
                        </td>
                    </tr>
                    <tr><td>&nbsp;</td></tr>
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td style="text-align:right;">Assetmanager</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <select id="ddAM">
                                            <option>Vierlander</option>
                                            <option>Cross Border</option>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:right;">Name</td>
                                    <td>&nbsp;</td>
                                    <td><input type="text" runat="server" value="Bla" /></td>
                                </tr>
                                <tr>
                                    <td>Type</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <input id="Text1" type="text" value="Bla" />
                                    </td>                               
                                </tr>
                                <tr>
                                    <td>Modelportfolio</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <select id="ddMod">
                                            <option>Monitor - Profiel 1</option>
                                            <option>Monitor - Profiel 2</option>
                                        </select>
                                    </td>                               
                                </tr><tr>
                                    <td>Exchange</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <select id="ddExchange">
                                            <option>Euronext</option>
                                            <option>FundSettle</option>
                                        </select>
                                    </td>                               
                                </tr><tr>
                                    <td>Instrument</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <select id="ddInstrument">
                                            <option>Euro</option>
                                            <option>Amerikaanse Dollar</option>
                                        </select>
                                    </td>                               
                                </tr>
                            </table>
                        </td>
                    </tr>                
                </table>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <asp:GridView 
                      ID="gvAccounts" 
                      runat="server" 
                      AllowPaging="True" 
                      AllowSorting="True"
                      SkinID="custom-width"
                      Width="600px" 
                      AutoGenerateColumns="False"
                      DataKeyNames="Key" 
                      Caption="Accounts" CaptionAlign="Left" PageSize="15" 
                      OnRowCommand="gvAccounts_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Number" HeaderText="Account Number" SortExpression="Number" ReadOnly="True">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ShortName" HeaderText="Account Name" SortExpression="ShortName" ReadOnly="True">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="AccountOwner_CompanyName" HeaderText="Asset Manager" SortExpression="AccountOwner_CompanyName" ReadOnly="True">
                            <ItemStyle Wrap="False" />
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton
                                     ID="lbtnAdd"
                                     runat="server"
                                     CommandName="AddAccount"
                                     Text="Add" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td>
                <asp:Button ID="btnAddNewAccount" runat="server" Text="Add New Account" />
            </td>
        </tr>
    </table>

    

</asp:Content>


<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TelephoneDetails.ascx.cs"
    Inherits="TelephoneDetails" %>
<table style="width: 200px">
    <tr>
        <td style="width: 100px; height: 24px; white-space: nowrap;">
            <asp:Label ID="lblTitle" runat="server"></asp:Label>
        </td>
        <td style="width: 125px; height: 24px; white-space: nowrap;">
            <asp:Label ID="lblDisplay" runat="server" Width="125"></asp:Label>
        </td>
        <td style="width: 100px; height: 24px">
            <asp:Button ID="btnEditDetails" runat="server" CausesValidation="false" Text="..."
                OnClick="btnEditDetails_Click" /></td>
    </tr>
    <tr>
        <td colspan="3" style="height: 102px">
            <asp:Panel ID="pnlTelephoneEdit" runat="server" BorderColor="Silver" BorderStyle="Solid"
                BorderWidth="1px" Visible="False" Width="225px">
                <table style="width: auto">
                    <tr>
                        <td style="width: 85px; height: 24px; white-space: nowrap;">
                            <asp:Label ID="lblCountry" runat="server" Text="Land"></asp:Label>
                        </td>
                        <td style="width: 140px; height: 24px; white-space: nowrap;">
                            <asp:TextBox ID="txtCountryID" runat="server">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 85px; height: 24px; white-space: nowrap;">
                            <asp:Label ID="lblCity" runat="server" Text="City"></asp:Label>
                        </td>
                        <td style="width: 140px; height: 24px; white-space: nowrap;">
                            <asp:TextBox ID="txtCityID" runat="server">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 85px; height: 24px; white-space: nowrap;">
                            <asp:Label ID="lblNumber" runat="server" Text="Number:"></asp:Label>
                        </td>
                        <td style="width: 140px; height: 24px; white-space: nowrap;">
                            <asp:TextBox ID="txtNumberID" runat="server">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                    <td></td>
                        <td style="width: 140px; height: 24px; white-space: nowrap;">
                            <asp:Button ID="btnConfirm" runat="server" CausesValidation="false" Text="Confirm" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>

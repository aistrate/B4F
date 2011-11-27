<%@ Control Language="C#" EnableTheming="true" AutoEventWireup="true" CodeFile="Address.ascx.cs" Inherits="UC_Address" %>

<table border="0" cellpadding="0" cellspacing="0" width="770px">
     <tr>
        <td style="vertical-align: top;">
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td style="width: 20px;">
                        <img runat="server" style="vertical-align: bottom;" src="~/layout/images/1pixel_black.gif" width="20" alt="" height="1"/>
                        <img runat="server" style=" vertical-align: top;" src="~/layout/images/1pixel_black.gif" width="1" alt="" height="14"/>
                    </td>
                    <td style="text-align: left; vertical-align: middle; width: 170px; height: 12px;">
                       <asp:Label runat="server" ID="lblResidentialAddress" Font-Bold="true" BackColor="White" Text="Residential Address"/>
                    </td>
                    <td style="height: 12px; width: 600px; text-align: right;">
                        <img runat="server" style="vertical-align: bottom;" src="~/layout/images/1pixel_black.gif" width="600" alt="" height="1"/>
                        <img runat="server" style="vertical-align: top; text-align: right;" src="~/layout/images/1pixel_black.gif" width="1" alt="" height="14"/>
                    </td>
                </tr>
                 <tr>
                        <td colspan="3">
                            <table style="border-left: solid 1px black; border-bottom: solid 1px black; border-right: solid 1px black;" cellpadding="0" cellspacing="0" width="770px">
                                <tr>
                                    <td style="width: 513px; text-align: right; height: 18px; vertical-align: middle;">
                                        <asp:Label ID="lblStreet" runat="server" Text="Street"></asp:Label></td>
                                    <td style="width: 256px; vertical-align: middle; text-align: left; height: 18px;">
                                        <asp:TextBox ID="tbStreet" runat="server" MaxLength="50"></asp:TextBox></td>
                                    <td style="vertical-align: middle; width: 114px; height: 18px; text-align: left">
                                        <asp:RequiredFieldValidator ErrorMessage="Street" ID="reqStreet" ControlToValidate="tbStreet" runat="server">*</asp:RequiredFieldValidator></td>
                                    <td style="width: 358px; text-align: right; vertical-align: middle; height: 18px;">
                                        <asp:Label ID="lblHouseNumber" runat="server" Text="HouseNumber"></asp:Label></td>
                                    <td style="width: 340px; vertical-align: middle; text-align: left; height: 18px;">
                                        <asp:TextBox ID="tbHouseNumber" runat="server" SkinID="small" MaxLength="20"></asp:TextBox></td>
                                    <td style="vertical-align: middle; width: 210px; height: 18px; text-align: left; ">
                                        <asp:RequiredFieldValidator ErrorMessage="HouseNumber" ControlToValidate="tbHouseNumber" ID="reqHouseNumber" runat="server">*</asp:RequiredFieldValidator>
                                        <asp:RangeValidator ID="rangeHouseNumber" ControlToValidate="tbHouseNumber" ErrorMessage="HouseNumber format: integer" runat="server" Type="Integer" MinimumValue="1" MaximumValue="10000000">*</asp:RangeValidator>
                                    </td>
                                    <td style="height: 10px"></td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: middle; width: 513px; height: 23px; text-align: right">
                                        <asp:Label ID="lblHouseNumberSuffix" runat="server" Text="HouseNumber Suffix"></asp:Label></td>
                                    <td style="vertical-align: middle; width: 256px; height: 23px; text-align: left">
                                        <asp:TextBox ID="tbHouseNumberSuffix" runat="server" MaxLength="7"></asp:TextBox>
                                    </td>
                                    <td style="vertical-align: middle; width: 114px; height: 23px; text-align: left">
                                    </td>
                                    <td style="vertical-align: middle; width: 358px; height: 23px; text-align: right">
                                        <asp:Label ID="lblPostCode" runat="server" Text="PostalCode"></asp:Label>
                                    </td>
                                    <td style="vertical-align: middle; width: 340px; height: 23px; text-align: left">
                                        <asp:TextBox ID="tbPostCode" Width="45" MaxLength="7" runat="server"></asp:TextBox>
                                    </td>
                                    <td style="vertical-align: middle; width: 210px; height: 23px; text-align: left">
                                        <asp:RequiredFieldValidator ErrorMessage="PostalCode"  ID="reqPostCodeNumbers" ControlToValidate="tbPostCode" runat="server">*</asp:RequiredFieldValidator>
                                       <asp:RegularExpressionValidator 
                                            id="regexPostcode" runat="server"
                                            Enabled="true"
                                            ControlToValidate="tbPostCode"
                                            ErrorMessage="PostalCode format: four digits plus two characters or five till seven digits."
                                            ValidationExpression="([0-9]{4}\s?[a-zA-Z]{2}|[0-9]{5,7})"
                                            Display="Static">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: middle; width: 513px; height: 23px; text-align: right">
                                        <asp:Label ID="lblCity" runat="server" Text="City"></asp:Label></td>
                                    <td style="vertical-align: middle; width: 256px; height: 23px; text-align: left">
                                        <asp:TextBox ID="tbCity" runat="server" MaxLength="30"></asp:TextBox></td>
                                    <td style="vertical-align: middle; width: 114px; height: 23px; text-align: left">
                                        <asp:RequiredFieldValidator ErrorMessage="City" ControlToValidate="tbCity" ID="reqCity" runat="server">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="vertical-align: middle; width: 358px; height: 23px; text-align: right">
                                        <asp:Label ID="lblCountry" runat="server" Text="Country"></asp:Label>
                                    </td>
                                    <td style="vertical-align: middle; width: 340px; height: 23px; text-align: left">
                                        <asp:DropDownList ID="ddCountry" runat="server" AutoPostBack="true"
                                            onselectedindexchanged="ddCountry_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                    <td style="vertical-align: middle; width: 210px; height: 23px; text-align: left">
                                        <asp:RangeValidator ErrorMessage="Country" ID="rangeCountry" Type="Integer" MinimumValue="0" MaximumValue="10000000" ControlToValidate="ddCountry" runat="server">*</asp:RangeValidator>
                                    </td>

                                </tr>
                                <tr><td colspan="7">&nbsp;</td></tr>
                            </table>
                        </td>
                    </tr>
                </table>
        </td>
     </tr>
    <asp:Panel ID="pnlPostalAddress" runat="server" Visible="true">
     <tr style="height: 1px; font-size: 1px;">
        <td>&nbsp;</td>
     </tr>
     <tr>
       <td>
           <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td style="width: 20px;">
                        <img runat="server" style="vertical-align: bottom;" src="~/layout/images/1pixel_black.gif" width="20" alt="" height="1"/>
                        <img runat="server" style=" vertical-align: top;" src="~/layout/images/1pixel_black.gif" width="1" alt="" height="14"/>
                    </td>
                    <td style="text-align: left; vertical-align: middle; width: 170px; height: 12px;">
                       <asp:Label runat="server" ID="lblPostalAddress" Font-Bold="true" BackColor="White" Text="Postal Address"/>
                    </td>
                    <td style="height: 12px; width: 600px; text-align: right;">
                        <img runat="server" style="vertical-align: bottom;" src="~/layout/images/1pixel_black.gif" width="600" alt="" height="1"/>
                        <img runat="server" style="vertical-align: top; text-align: right;" src="~/layout/images/1pixel_black.gif" width="1" alt="" height="14"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                       <table style="border-left: solid 1px black; border-bottom: solid 1px black; border-right: solid 1px black; " cellpadding="0" cellspacing="0" width="770px">
                            <tr>
                                <td style="vertical-align: middle; width: 477px; height: 25px; text-align: right">
                                    <asp:Label ID="lblPostalStreet" runat="server" Text="Street"></asp:Label>
                                </td>
                                <td style="vertical-align: middle; width: 210px; height: 25px; text-align: left">
                                    <asp:TextBox ID="tbPostalStreet" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                                <td style="vertical-align: middle; width: 109px; height: 25px; text-align: left">
                                </td>
                                <td style="vertical-align: middle; width: 350px; height: 25px; text-align: right">
                                    <asp:Label ID="lblPostalHouseNumber" runat="server">HouseNumber</asp:Label>
                                </td>
                                <td style="vertical-align: middle; width: 310px; height: 25px; text-align: left">
                                    <asp:TextBox ID="tbPostalHouseNumber" SkinID="small" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                                  <td style="vertical-align: middle; width: 210px; height: 25px; text-align: left">
                                        <asp:RangeValidator  ID="rangePostalHouseNumber" ControlToValidate="tbPostalHouseNumber" ErrorMessage="HouseNumber format: integer" runat="server" Type="Integer" MinimumValue="1" MaximumValue="10000000">*</asp:RangeValidator>
                                  </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: middle; width: 477px; height: 25px; text-align: right">
                                    <asp:Label ID="lblPostalHouseNumberSuffix" runat="server" Text="HouseNumber Suffix"></asp:Label></td>
                                <td style="vertical-align: middle; width: 210px; height: 25px; text-align: left">
                                    <asp:TextBox ID="tbPostalHouseNumberSuffix" runat="server" SkinID="small" MaxLength="7"></asp:TextBox></td>
                                <td style="vertical-align: middle; width: 109px; height: 25px; text-align: left">
                                
                                </td>
                                <td style="vertical-align: middle; width: 350px; height: 25px; text-align: right">
                                    <asp:Label ID="lblPostalPostCode" runat="server" Text="PostalCode"></asp:Label></td>
                                <td style="vertical-align: middle; width: 310px; height: 25px; text-align: left">
                                    <asp:TextBox ID="tbPostalPostCode" Width="45" runat="server" MaxLength="7" ></asp:TextBox>
                                </td>
                                <td style="vertical-align: middle; width: 210px; height: 25px; text-align: left">
                                   <asp:RegularExpressionValidator 
                                        id="regexPostalPostCode" runat="server"
                                        Enabled="true"
                                        ControlToValidate="tbPostalPostCode"
                                        ErrorMessage="PostalCode format: four digits plus two characters or five till seven digits."
                                        ValidationExpression="([0-9]{4}\s?[a-zA-Z]{2}|[0-9]{5,7})"
                                        Display="Static">*</asp:RegularExpressionValidator> 
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: middle; width: 477px; height: 25px; text-align: right">
                                    <asp:Label ID="lblPostalCity" runat="server" Text="City"></asp:Label></td>
                                <td style="vertical-align: middle; width: 210px; height: 25px; text-align: left">
                                    <asp:TextBox ID="tbPostalCity" runat="server" MaxLength="30"></asp:TextBox></td>
                                <td style="vertical-align: middle; width: 109px; height: 25px; text-align: left">
                                </td>
                                <td style="vertical-align: middle; width: 350px; height: 25px; text-align: right">
                                    <asp:Label ID="lblPostalCountry" runat="server" Text="Country"></asp:Label></td>
                                <td style="vertical-align: middle; width: 310px; height: 25px; text-align: left">
                                    <asp:DropDownList ID="ddPostalCountry" runat="server" AutoPostBack="true"
                                            onselectedindexchanged="ddPostalCountry_SelectedIndexChanged">
                                    </asp:DropDownList></td>
                                <td style="vertical-align: middle; width: 210px; height: 25px; text-align: left">
                                </td>
                              </tr>
                                <tr><td colspan="7">&nbsp;</td></tr>
                            </table>
                    </td>
                </tr>
           </table>
       </td>
    </tr>
    </asp:Panel>
</table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContactDetails.ascx.cs" Inherits="UC_ContactDetails" %>
<table border="0" cellpadding="0" cellspacing="0">
    <asp:HiddenField ID="hfRemisierID" runat="server" Value="" />
    <tr>
        <td style="width: 693px">
            <table border="0" cellpadding="0" cellspacing="0" width="693px">
<%--                <tr>
                    <td style="vertical-align: middle; width: 141px; height: 13px; text-align: right">
                        <asp:Label ID="lblVwRemisier" runat="server" Text="Remisier"></asp:Label>
                    </td>
                    <td style="width: 180px; height: 24px;">
                        <asp:DropDownList ID="ddlRemisier" Enabled="true" runat="server" 
                            Width="165px" DataSourceID="odsRemisier" DataTextField="Name" DataValueField="Key" 
                            AutoPostBack="True">
                        </asp:DropDownList>

                        <asp:ObjectDataSource 
                            ID="odsRemisier" runat="server" 
                            SelectMethod="GetRemisiers"
                            TypeName="B4F.TotalGiro.ApplicationLayer.UC.ContactDetailsAdapter">
                        </asp:ObjectDataSource>
                    </td>
                    <td style="padding-left: 5px; vertical-align: middle; width: 4px; height: 13px;
                        text-align: left">
                    </td>
                    <td style="vertical-align: middle; width: 126px; height: 13px; text-align: right">
                        <asp:Label ID="lblRemisierEmployee" Visible="false" runat="server" Text="Remisier Employee"></asp:Label>&nbsp;</td>
                    <td style="vertical-align: middle; width: 173px; height: 13px; text-align: left">
                        <asp:DropDownList   ID="ddlRemisierEmployee"
                                            DataSourceID="odsEmployees"
                                            Enabled="true"
                                            runat="server"
                                            Width="165px"
                                            DataTextField="FullName" 
                                            DataValueField="Key" 
                                            Visible="false"
                                            OnDataBound="ddlRemisierEmployee_DataBound">
                                            
                        </asp:DropDownList>
                        <asp:ObjectDataSource 
                            ID="odsEmployees" 
                            runat="server" SelectMethod="GetRemisierEmployees" TypeName="B4F.TotalGiro.ApplicationLayer.UC.ContactDetailsAdapter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlRemisier" Name="remisierID" PropertyName="SelectedValue"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td style="vertical-align: middle; width: 13px; height: 13px;
                        text-align: left">
                        &nbsp;
                    </td>
                </tr>--%>
                <tr>
                    <td style="width: 141px; text-align: right; height: 13px; vertical-align: middle;">
                        <asp:Label ID="lblInternet" runat="server" Text="Internet Enabled"></asp:Label>&nbsp;</td>
                    <td style="width: 169px; vertical-align: top; text-align: left; height: 13px;">
                        <asp:RadioButtonList ID="rbInternet" runat="server" RepeatDirection="Horizontal" />
                    </td>
                    <td style="vertical-align: middle; width: 4px; height: 13px; text-align: left; padding-left: 5px;">
                       <asp:RequiredFieldValidator ID="rfInternet" runat="server" ErrorMessage="Internet Enabled" ControlToValidate="rbInternet">*</asp:RequiredFieldValidator>
                    </td>
                    <td style="vertical-align: middle; width: 126px; height: 13px; text-align: right">
                        <asp:Label ID="lblEmail" runat="server" Text="Email Address"></asp:Label>
                    </td>
                    <td style="vertical-align: middle; width: 173px; height: 13px; text-align: left">
                        <asp:TextBox ID="tbEmail" SkinID="broad" runat="server"></asp:TextBox>
                    </td>
                    <td style="vertical-align: middle; height: 13px; text-align: left; width: 13px;">
                        <asp:RequiredFieldValidator EnableClientScript="true" ID="reqEmail" runat="server" ErrorMessage="Email" 
                                                    ControlToValidate="tbEmail">*</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="regexpEmail" runat="server" ControlToValidate="tbEmail"	
                                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                                        ErrorMessage="Email format is incorrect.">*</asp:RegularExpressionValidator>
                    </td>
                 </tr>
                 

                <tr>
                    <td colspan="3"></td>
                    <td style="text-align: right"><asp:Label ID="lblSendNewsItem" runat="server" Text="Send news item"></asp:Label></td>
                    <td colspan="2" style="float:left; text-align:left;">
                        <asp:CheckBox ID="chkSendNewsItem" runat="server" Checked="false" AutoPostBack="true" />
                    </td>
                </tr>                 
                 
                 
                <tr>
                <td style="vertical-align: middle; width: 141px; height: 26px; text-align: right">
                        <asp:Label ID="lblTelephone" runat="server" Text="Telephone"></asp:Label></td>
                    <td style="vertical-align: middle; width: 169px; height: 26px; text-align: left">
                        <asp:TextBox ID="tbTelephone" SkinID="custom-size" Width="145px" runat="server"></asp:TextBox>
                    </td>
                    <td style="height: 26px; width: 4px; padding-left: 5px;">
                        &nbsp;<asp:RequiredFieldValidator 
                            ID="reqTelephone" 
                            runat="server"
                            ErrorMessage="Telephone" 
                            ControlToValidate="tbTelephone">*</asp:RequiredFieldValidator><br />
                        <asp:RegularExpressionValidator 
                           id="regexTelephone" runat="server" 
                           ControlToValidate="tbTelephone" 
                           ValidationExpression="^((((0031)|(\+31))(\-|\s)?6(\-)?[0-9]{8})|(06(\-\|\s)?[0-9]{8})|(((0031)|(\+31))(\-|\s)?[1-9]{1}(([0-9](\-|\s)?[0-9]{7})|([0-9]{2}(\-|\s)?[0-9]{6})))|([0]{1}[1-9]{1}(([0-9](\-|\s)?[0-9]{7})|([0-9]{2}(\-|\s)?[0-9]{6}))))|(((0031)|(\+31)|([0]{1}))(\s)?[6]{1}[-\s]*[0-9]{8})$"
                           ErrorMessage="Not a valid dutch telephone number">*</asp:RegularExpressionValidator>&nbsp;
                    </td>
                    <td style="vertical-align: middle; width: 126px; height: 26px; text-align: right">
                        <asp:Label ID="lblTelephoneAH" runat="server" Text="Tel.  After Hours"></asp:Label>
                    </td>
                    <td style="vertical-align: middle; width: 173px; height: 26px; text-align: left">
                        <asp:TextBox ID="tbTelephoneAH" runat="server"></asp:TextBox></td>
                    <td style="height: 26px;">
                        &nbsp;<asp:RegularExpressionValidator 
                           id="regexTelephoneAH" runat="server" 
                           ControlToValidate="tbTelephoneAH" 
                           ValidationExpression="^((((0031)|(\+31))(\-|\s)?6(\-)?[0-9]{8})|(06(\-\|\s)?[0-9]{8})|(((0031)|(\+31))(\-|\s)?[1-9]{1}(([0-9](\-|\s)?[0-9]{7})|([0-9]{2}(\-|\s)?[0-9]{6})))|([0]{1}[1-9]{1}(([0-9](\-|\s)?[0-9]{7})|([0-9]{2}(\-|\s)?[0-9]{6}))))|(((0031)|(\+31)|([0]{1}))(\s)?[6]{1}[-\s]*[0-9]{8})$"
                           ErrorMessage="Not a valid dutch home telephone number">*</asp:RegularExpressionValidator>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: middle; width: 141px; height: 26px; text-align: right">
                        <asp:Label ID="lblFax" runat="server" Text="Fax"></asp:Label></td>
                    <td style="vertical-align: middle; width: 169px; height: 26px; text-align: left">
                        <asp:TextBox ID="tbFax" runat="server"></asp:TextBox></td>
                    <td style="padding-left: 5px; width: 4px; height: 26px">
                        <asp:RegularExpressionValidator 
                           id="regexFax" runat="server" 
                           ControlToValidate="tbFax" 
                           ValidationExpression="^((((0031)|(\+31))(\-|\s)?6(\-)?[0-9]{8})|(06(\-\|\s)?[0-9]{8})|(((0031)|(\+31))(\-|\s)?[1-9]{1}(([0-9](\-|\s)?[0-9]{7})|([0-9]{2}(\-|\s)?[0-9]{6})))|([0]{1}[1-9]{1}(([0-9](\-|\s)?[0-9]{7})|([0-9]{2}(\-|\s)?[0-9]{6}))))$"
                           ErrorMessage="Not a valid dutch fax number">*</asp:RegularExpressionValidator>
                     </td>
                    <td style="vertical-align: middle; width: 126px; height: 26px; text-align: right">
                        <asp:Label ID="lblMobile" runat="server" Text="Mobile"></asp:Label></td>
                    <td style="vertical-align: middle; width: 173px; height: 26px; text-align: left">
                        <asp:TextBox ID="tbMobile" runat="server"></asp:TextBox></td>
                    <td style="height: 26px">
                          <asp:RegularExpressionValidator 
                            ID="regexMobile"
                            ControlToValidate="tbMobile" 
                            runat="server"
                            ValidationExpression="^(((0031)|(\+31)|([0]{1}))(\s)?[6]{1}[-\s]*[0-9]{8})$"
                           ErrorMessage="Not a valid dutch mobile number">*</asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5" style="float:left; text-align:left;">
                        <asp:CheckBox ID="chkTelephoneNumberCheck" runat="server" Text="Telephone number check" Checked="true" 
                            AutoPostBack="true" OnCheckedChanged="chkTelephoneNumberCheck_CheckedChanged"/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

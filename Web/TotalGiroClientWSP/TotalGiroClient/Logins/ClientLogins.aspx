<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" CodeFile="ClientLogins.aspx.cs" Inherits="ClientLogins" %>

<%@ Register Src="~/UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="b4f" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="B4F.TotalGiro.Security" %>
<%@ Import Namespace="B4F.TotalGiro.ClientApplicationLayer.Logins" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <asp:ScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
    
    <div style="position: relative">
        <div style="position: relative; left: -4px; top: -9px">
            <asp:Panel ID="pnlAccountFinder" runat="server" Width="900px">
                <b4f:AccountFinder ID="ctlAccountFinder" runat="server" AccountNameLabel="Contact Name:"
                                   ShowRemisier="true" ShowRemisierEmployee="true" 
                                   ShowAccountStatus="true" ShowEmailStatus="true" ShowLoginStatus="true" />
            </asp:Panel>
        </div>
    </div>
    
    <div style="height: 3px"></div>
    <asp:Panel ID="pnlContacts" runat="server" Visible="false">
        <b4f:MultipleSelectionGridView ID="gvContacts" runat="server" DataSourceID="odsContacts" AutoGenerateColumns="False" 
                      DataKeyNames="PersonKey" AllowPaging="True" AllowSorting="True" Caption="Clients" CaptionAlign="Top" PageSize="15" 
                      SkinID="no-stripes" Width="970px" SelectionBoxVisibleBy="NeedsHandling" OnDataBound="gvContacts_DataBound"> 
            <Columns>
                <asp:TemplateField HeaderText="Contact" SortExpression="ShortName">
                    <ItemStyle wrap="False" backcolor="#F3F3F3" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <b4f:TruncatedLabel id="lblFullName" runat="server" LongText='<%# Eval("FullName")%>' MaxLength="40" 
                                            CustomToolTip="True" ToolTip='<%# Eval("FullAddress") %>' 
                                            ForeColor='<%# ((bool)Eval("IsActive") ? Color.Black : Color.Gray) %>'
                                            Font-Italic='<%# ((bool)Eval("IsActive") ? false : true) %>'>
                        </b4f:TruncatedLabel>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DisplayContactType" HeaderText="Type" SortExpression="DisplayContactType">
                    <ItemStyle wrap="False" backcolor="#F3F3F3" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Email" SortExpression="Email">
                    <ItemStyle wrap="False" backcolor="#F3F3F3" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <b4f:TruncatedLabel id="lblEmail" runat="server" LongText='<%# Eval("Email")%>' MaxLength="50">
                        </b4f:TruncatedLabel>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Accounts">
                    <ItemStyle wrap="False" backcolor="#F3F3F3" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label id="lblAccountNumbers" runat="server"
                                   Text='<%# Utility.FormatAccountNumbersByActive(ClientLoginsAdapter.GetAccountNumbers((int)Eval("PersonKey"), !ctlAccountFinder.AccountStatusInactive)) %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="By post" SortExpression="SendByPost">
                    <ItemStyle horizontalalign="Left" wrap="False" backcolor="#F3F3F3" CssClass="checkbox-col" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <b4f:QuestionCheckBox id="cboSendByPost" runat="server" Checked='<%# Eval("SendByPost") %>'
                            QuestionText='<%# string.Format(@"Are you sure you want to turn {0} sending by Post for this contact?", 
                                                            (bool)Eval("SendByPost") ? "OFF" : "ON") %>'
                            OnCheckedChanged="cboSendByPost_CheckedChanged">
                        </b4f:QuestionCheckBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="By email" SortExpression="SendByEmail">
                    <ItemStyle horizontalalign="Left" wrap="False" backcolor="#F3F3F3" CssClass="checkbox-col" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <b4f:QuestionCheckBox id="cboSendByEmail" runat="server" Checked='<%# Eval("SendByEmail") %>'
                            QuestionText='<%# string.Format(@"Are you sure you want to turn {0} sending by Email for this contact?", 
                                                            (bool)Eval("SendByEmail") ? "OFF" : "ON") %>'
                            OnCheckedChanged="cboSendByEmail_CheckedChanged">
                        </b4f:QuestionCheckBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LoginUserName" HeaderText="Login" SortExpression="LoginUserName">
                    <ItemStyle wrap="False" HorizontalAlign="Left" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Active" SortExpression="LoginPersonStatus">
                    <ItemStyle horizontalalign="Left" wrap="False" CssClass="checkbox-col" />
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <b4f:QuestionCheckBox id="cboLoginActive" runat="server" Visible='<%# Eval("HasLogin") %>'
                                              Checked='<%# Eval("IsLoginActive") %>' Enabled='<%# Eval("PasswordSent") %>'
                                              QuestionText='<%# string.Format(@"Are you sure you want to {0}activate this contact&#39;s login?", 
                                                                              (bool)Eval("IsLoginActive") ? "in" : "") %>'
                                              OnCheckedChanged="cboLoginActive_CheckedChanged">
                        </b4f:QuestionCheckBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Password" SortExpression="LoginPersonStatus" >
                    <ItemStyle wrap="False" HorizontalAlign="Left"/>
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtResetPassword" onclick="lbtResetPassword_Click" runat="server" Text="Reset" 
                                        Visible='<%# (bool)Eval("IsLoginActive") && (bool)Eval("PasswordSent") %>' 
                                        ToolTip="Reset password for this login" 
                                        OnClientClick="return confirm('Are you sure you want to reset the password for this login?')" 
                                        CommandArgument='<%# Eval("PersonKey") %>'>
                        </asp:LinkButton>
                        <asp:Label id="lblPasswordSent" runat="server" Text='<%# ((bool)Eval("PasswordSent") ? "Sent" : "NOT sent") %>' 
                                   ForeColor="DarkGray" Visible='<%# (bool)Eval("HasLogin") && !(bool)Eval("IsLoginActive") %>' 
                                   Font-Italic="True">
                        </asp:Label> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle wrap="False" HorizontalAlign="Left"/>
                    <HeaderStyle wrap="False" HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton id="lbtDeleteLogin" runat="server" CommandArgument='<%# Eval("PersonKey") %>' Visible='<%# Eval("HasLogin") %>'
                                        OnClick="lbtDeleteLogin_Click" Text="Delete" ToolTip="Delete login for this contact"
                                        OnClientClick="return confirm('Are you sure you want to delete this contact\'s login?')">
                        </asp:LinkButton>
                        <asp:LinkButton id="lbtUnlockLogin" runat="server" CommandArgument='<%# Eval("PersonKey") %>' 
                                        Visible='<%# Eval("LoginUserName") != null && SecurityManager.IsUserLockedOut((string)Eval("LoginUserName")) %>'
                                        OnClick="lbtUnlockLogin_Click" Text="Unlock" ToolTip="Unlock login for this contact"
                                        OnClientClick="return confirm('Are you sure you want to unlock this contact\'s login?')">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </b4f:MultipleSelectionGridView>
        <asp:ObjectDataSource ID="odsContacts" runat="server" SelectMethod="GetClientContacts"
                              TypeName="B4F.TotalGiro.ClientApplicationLayer.Logins.ClientLoginsAdapter">
            <SelectParameters>
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierId" PropertyName="RemisierId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="remisierEmployeeId" PropertyName="RemisierEmployeeId"
                    Type="Int32" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactName" PropertyName="AccountName"
                    Type="String" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountStatusActive" PropertyName="AccountStatusActive"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountStatusInactive" PropertyName="AccountStatusInactive"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="emailStatusYes" PropertyName="EmailStatusYes"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="emailStatusNo" PropertyName="EmailStatusNo"
                    Type="Boolean" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="hasLogin" PropertyName="HasLogin"
                    Type="Boolean" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="passwordSent" PropertyName="PasswordSent"
                    Type="Boolean" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="ctlAccountFinder" Name="isLoginActive" PropertyName="IsLoginActive"
                    Type="Boolean" ConvertEmptyStringToNull="true" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <asp:Button ID="btnSendLoginName" runat="server" Text="Send Login Names" Width="150px" OnClick="btnSendLoginName_Click" />&nbsp;
        <asp:Button ID="btnSendPassword" runat="server" Text="Send Passwords" Width="150px" OnClick="btnSendPassword_Click" />
    </asp:Panel>
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>

</asp:Content>

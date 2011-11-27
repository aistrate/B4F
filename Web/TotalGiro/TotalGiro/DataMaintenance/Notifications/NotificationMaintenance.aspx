<%@ Page Title="Notification Maintenance" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="NotificationMaintenance.aspx.cs" Inherits="NotificationMaintenance" %>

<%@ Register Src="../../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../../UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc2" %>
<%@ Register Src="../../UC/AccountsContactsSelector.ascx" TagName="AccContSelector" TagPrefix="uc3" %>
<%@ Import Namespace="B4F.TotalGiro.Notifications" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <uc1:AccountFinder  
        ID="ctlAccountFinder"
        ShowContactActiveCbl="true"
        AccountNameLabel="Contact Name:"
        ShowBsN_KvK="true"
        ShowSearchButton="false"
        runat="server"  />

    <table width="440px" style="position:relative; top:-8px;" >
        <tr>
            <td>
                <table>
                    <tr>
                        <td style="width: 130px; height: 24px">
                            <asp:Label ID="lblNotificationType" runat="server" Text="Notification Type:" />
                        </td>
                        <td style="width: 190px">
                            <asp:DropDownList ID="ddlNotificationType" runat="server" DataSourceID="odsNotificationType"
                                DataTextField="Description" DataValueField="Key" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsNotificationType" runat="server" SelectMethod="GetNotificationTypes"
                                TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.NotificationMaintenanceAdapter" />
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 100px; vertical-align: bottom">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CausesValidation="False" Width="90px" 
                    Style="position: relative; top: -5px;" OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="gvNotifications" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="odsNotifications" 
        DataKeyNames="Key" Caption="Notifications"
        CaptionAlign="Left" PageSize="20" 
        Visible="false" >
        <Columns>
            <asp:TemplateField>
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <b4f:TooltipImage ID="ttiNotification" runat="server" TooltipClickClose="true" 
                         TooltipShadowWidth="5" TooltipPadding="8" IsTooltipAbove="true" OffSetX="-17"
                        TooltipContent='<%# DataBinder.Eval(Container.DataItem, "DisplayMessage") %>'
                        TooltipDefaultImage='<%# (NotificationTypes)DataBinder.Eval(Container.DataItem, "NotificationType") == NotificationTypes.Warning ? TooltipImage.DefaultImage.ExclamationMark : TooltipImage.DefaultImage.Balloon_Small  %>' >
                    </b4f:TooltipImage>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Type" SortExpression="NotificationType">
                <ItemTemplate>
                    <%# (NotificationTypes)DataBinder.Eval(Container.DataItem, "NotificationType") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Message" SortExpression="Message">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        ID="trlMessage"
                        runat="server"
                        cssclass="alignright"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "Message") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Who" SortExpression="ToWho">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel 
                        ID="trlToWho" 
                        runat="server"
                        cssclass="alignright"
                        Width="30"
                        Text='<%# DataBinder.Eval(Container.DataItem, "ToWho") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:CheckBoxField>
            <asp:TemplateField HeaderText="Start" SortExpression="StartDate">
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToString("d MMMM yyyy") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Due" SortExpression="DueDate">
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "DueDate") != DateTime.MinValue ?
                                                                     ((DateTime)DataBinder.Eval(Container.DataItem, "DueDate")).ToString("d MMMM yyyy") : "")%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" >
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Created" SortExpression="CreationDate">
                <ItemTemplate>
                    <%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("d MMMM yyyy") %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton
                        ID="lbtnEditNotification"
                        runat="server"
                        CommandName="EditNotification"
                        Text="Edit"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' 
                        OnCommand="lbtEdit_Command" />
                    <asp:LinkButton 
                        runat="server" 
                        ID="lbtDeActivate" 
                        Text="DeActivate" 
                        CommandName="DeActivate"
                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                        Visible='<%# DataBinder.Eval(Container.DataItem, "IsActive") %>'
                        OnCommand="lbtDeActivate_Command" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsNotifications" runat="server" SelectMethod="GetNotifications"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.NotificationMaintenanceAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerID" PropertyName="AssetManagerId"
                Type="Int32" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="contactName" PropertyName="AccountName"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="bsN_KvK" PropertyName="BsN_KvK"
                Type="String" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showActive" PropertyName="ContactActive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ctlAccountFinder" Name="showInactive" PropertyName="ContactInactive"
                Type="Boolean" />
            <asp:ControlParameter ControlID="ddlNotificationType" Name="notificationTypeId" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Button ID="btnCreateNewNotification" runat="server" Text="Create" OnClick="btnCreateNewNotification_Click" Width="90px" Visible="false" />

    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
    <asp:ValidationSummary DisplayMode="BulletList" HeaderText="THE PAGE WAS NOT SAVED. Correct the following fields:"
        ID="valSum" runat="server" />
    <br />
    <asp:Panel ID="pnlDetails" runat="server" Visible="false" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Width="885px" >
        <table cellpadding="1" cellspacing="1" border="0" >
          <tr>
            <td style="width: 120px">
                <asp:Label ID="lblNotificationType_Edit" runat="server" Text="Notification Type:" />
            </td>
            <td style="width: 150px">
                <asp:DropDownList ID="ddlNotificationType_Edit" runat="server" DataSourceID="odsNotificationType" 
                    DataTextField="Description" DataValueField="Key" SkinID="custom-width" Width="95px">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvNotificationType"
                    controlToValidate="ddlNotificationType_Edit"
                    runat="server"
                    InitialValue="-2147483648" 
                    Text="*"
                    ErrorMessage="Date is mandatory" />
            </td>
            <td style="width: 100px" align="left">
                <asp:Label ID="lblMessage" runat="server" Text="Message:" />
            </td>
            <td style="width: 505px" align="left" valign="baseline" rowspan="3" >
                <asp:TextBox ID="txtMessage" runat="server" SkinID="CustomMultiLine" Width="505px" Height="85px" Wrap="true" TextMode="MultiLine"  />
            </td>
            <td style="width: 10px" align="left">
                <asp:RequiredFieldValidator ID="rfvMessage"
                    controlToValidate="txtMessage"
                    runat="server"
                    Text="*"
                    ErrorMessage="Message is mandatory" />
            </td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblStartDate" runat="server" Text="Start Date" />
            </td>
            <td>
                <uc2:Calendar ID="cldStartDate" runat="server" IsButtonDeleteVisible="false" Format="dd-MM-yyyy" />
            </td>
            <td align="left">
                <asp:RequiredFieldValidator ID="rfvExecDate"
                    controlToValidate="cldStartDate:txtCalendar"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Date is mandatory" />
            </td>
            <td colspan="2" ></td>
          </tr>

          <tr>
            <td>
                <asp:Label ID="lblDueDate" runat="server" Text="Due Date" />
            </td>
            <td>
                <uc2:Calendar ID="cldDueDate" runat="server" IsButtonDeleteVisible="true" Format="dd-MM-yyyy" />
            </td>
            <td>
                <asp:CompareValidator ID="cvDueDate"
                    controlToValidate="cldDueDate:txtCalendar"
                    ControlToCompare="cldStartDate:txtCalendar"
                    Operator="GreaterThan"
                    Type="Date"
                    runat="server"
                    Text="*"
                    SetFocusOnError="true"
                    ErrorMessage="Due Date should be after the start date" />
            </td>
            <td colspan="2" ></td>
          </tr>
        </table>
        <uc3:AccContSelector ID="ctlAccContSelector" runat="server" />
   </asp:Panel>
    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" Width="90px" Visible="false" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false" Width="90px" Visible="false" />
    <br />
    <asp:HiddenField ID="hdnScrollToBottom" runat="server" />
</asp:Content>


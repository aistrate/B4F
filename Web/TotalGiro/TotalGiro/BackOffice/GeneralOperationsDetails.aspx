<%@ Page Title="General Operations Details" Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true"
    CodeFile="GeneralOperationsDetails.aspx.cs" Inherits="GeneralOperationsDetails" %>

<%@ Register Src="~/UC/BackButton.ascx" TagName="BackButton" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Utils" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    <br />
    <uc1:BackButton ID="BackButton1" runat="server" />
    <asp:Button ID="Button1" runat="server" OnClick="btnAuditLogDetails_Click" Text="Audit Log Details..." />
    <br />
    <br />
    <asp:DetailsView ID="dvTrade" runat="server" AutoGenerateRows="False" Caption="General Operations Booking"
        CaptionAlign="Left" DataSourceID="odsGop" Width="125px" DataKeyNames="AuditLogClass,AuditLogKey">
        <Fields>
            <asp:BoundField DataField="ClassName" HeaderText="Type">
                <ItemStyle Wrap="False" BackColor="Gainsboro" />
                <HeaderStyle BackColor="LightSteelBlue" />
            </asp:BoundField>
            <asp:BoundField DataField="Key" HeaderText="BookingID">
                <ItemStyle Wrap="False" BackColor="Gainsboro" />
                <HeaderStyle BackColor="LightSteelBlue" />
            </asp:BoundField>
            <asp:BoundField DataField="AccountName" HeaderText="Account">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Description" HeaderText="Description">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Booking Date">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "BookDate"))%>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="TotalAmount" HeaderText="Total Amount">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="TaxPercentage" HeaderText="Tax Percentage">
                <ItemStyle Wrap="False" />
            </asp:BoundField>            
            <asp:TemplateField HeaderText="Creation Date">
                <ItemTemplate>
                    <%# Util.DateTimeToString((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")) %>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="IsStorno" HeaderText="Is Storno">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="StornoBookingID" HeaderText="StornoBookingID">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="BookNotaID" HeaderText="BookNotaID">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="JournalEntryID" HeaderText="JournalEntryID">
                <ItemStyle Wrap="False" />
            </asp:BoundField>
        </Fields>
        <FieldHeaderStyle Wrap="False" Height="14px" />
    </asp:DetailsView>
    <asp:ObjectDataSource ID="odsGop" runat="server" SelectMethod="GetBookingDetails"
        TypeName="B4F.TotalGiro.ApplicationLayer.GeneralLedger.GLBookingsAdapter">
        <SelectParameters>
            <asp:SessionParameter Name="bookingId" SessionField="BookingId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

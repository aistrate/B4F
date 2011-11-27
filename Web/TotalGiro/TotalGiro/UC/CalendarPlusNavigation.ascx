<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CalendarPlusNavigation.ascx.cs" Inherits="CalendarPlusNavigation" %>
<%@ Register Src="Calendar.ascx" TagName="Calendar" TagPrefix="ucCalendar" %>

<span class="padding" style="display:inline; float:left;" >
<asp:Table runat="server" id="tblDate" CellSpacing="0" Width="80px" >
    <asp:TableRow>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px;" align="left" >
            <asp:ImageButton ID="btnPrev" runat="server" ImageAlign="Top" ImageUrl="~/layout/images/prev.gif" CausesValidation="False" OnClick="btnPrev_Click" />
        </asp:TableCell>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px" align="left" >
            <ucCalendar:Calendar ID="cldDate" runat="server" Format="dd-MM-yyyy" IsButtonDeleteVisible="false" AutoPostBack="true" />
        </asp:TableCell>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px" align="left" >
            <asp:ImageButton ID="btnNext" runat="server" ImageAlign="Top" ImageUrl="~/layout/images/next.gif" CausesValidation="False" OnClick="btnNext_Click" />
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
</span>
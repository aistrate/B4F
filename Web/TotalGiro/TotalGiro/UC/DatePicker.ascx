<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatePicker.ascx.cs" Inherits="DatePicker" %>
<asp:Table runat="server" id="tblDate" CellSpacing="0">
    <asp:TableRow>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px;" align="left" >
            <asp:TextBox ID="txtDate" runat="server" Enabled="False" Width="80px" />
        </asp:TableCell>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px" align="left" >
            <asp:ImageButton ID="imbCalendar" runat="server" ImageUrl="~/layout/images/calendar.gif" ImageAlign="Top" OnClick="imbCalendar_Click" CausesValidation="False" />
        </asp:TableCell>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px" align="left" >
            <asp:ImageButton ID="imbDelete" runat="server" ImageAlign="Top" ImageUrl="~/layout/images/delete.gif" OnClick="imbDelete_Click" CausesValidation="False" />
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Panel ID="pnlCalendar" runat="server" Width="201px" Visible="False">
    <div style="position: absolute; z-index:5">
    <asp:DropDownList SkinID="custom-width" Font-Bold="true" Width="100px" id="ddlMonth" Runat="Server" AutoPostBack="True" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
    </asp:DropDownList><asp:DropDownList SkinID="custom-width" Font-Bold="true" Width="100px" id="ddlYear" Runat="Server" AutoPostBack="True" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
    </asp:DropDownList><asp:Calendar ID="cldDate" runat="server" BackColor="White" BorderColor="#999999"
        CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt"
        ForeColor="Black" Height="180px" Width="200px" OnSelectionChanged="cldDate_SelectionChanged" OnVisibleMonthChanged="cldDate_VisibleMonthChanged">
        <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
        <TodayDayStyle BackColor="Silver" ForeColor="Black" />
        <SelectorStyle BackColor="#CCCCCC" />
        <WeekendDayStyle BackColor="#DDDDDD" />
        <OtherMonthDayStyle ForeColor="Gray" />
        <NextPrevStyle VerticalAlign="Bottom" />
        <DayHeaderStyle Font-Bold="True" Font-Size="7pt" />
        <TitleStyle BackColor="#AAB9C2" BorderColor="#999999" Font-Bold="True" BorderStyle="Solid" BorderWidth="1px" />
    </asp:Calendar>
</div>
</asp:Panel>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Calendar.ascx.cs" Inherits="UC_Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Table runat="server" id="tblDate" CellSpacing="0" Width="80px">
    <asp:TableRow>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px;" align="left" >
            <asp:TextBox ID="txtCalendar" runat="server" Width="70px" SkinID="custom-width"
                OnTextChanged="cldxCalender_ClientDateSelectionChanged"
                AutoPostBack="false" 
                EnableViewState="true" />
            <ajaxToolkit:CalendarExtender ID="cldxCalender" runat="server" TargetControlID="txtCalendar" PopupButtonID="imgCalender" Animated="true" ClearTime="true" Format="dd/MM/yyyy" 
                EnableViewState="true" />        

           <ajaxToolkit:MaskedEditExtender
                ID="meeMaskEditExtender"
                TargetControlID="txtCalendar"
                runat="server" 
                MessageValidatorTip="true" 
                OnFocusCssClass="MaskedEditFocus" 
                OnInvalidCssClass="MaskedEditError"
                MaskType="Date"
                Mask="99/99/9999"
                CultureName="en-GB"
                ClearMaskOnLostFocus="true"
                UserDateFormat="DayMonthYear" 
                InputDirection="LeftToRight" 
                ErrorTooltipEnabled="True"
                DisplayMoney="Left" 
                AcceptNegative="Left"
                Enabled="false" />

            <asp:CompareValidator ID="cvValidDate" runat="server" 
                ControlToValidate="txtCalendar" Display="None" Text="*" ErrorMessage="Not a valid date" 
                Operator="DataTypeCheck" Type="Date" SetFocusOnError="true" Enabled="false" />
            <ajaxToolkit:ValidatorCalloutExtender ID="cvValidDateCalloutExtender" PopupPosition="BottomLeft"
                            runat="server" Enabled="True" TargetControlID="cvValidDate" />

            <asp:RangeValidator ID="rvCalendar" runat="server" Display="None" SetFocusOnError="true"
                            ControlToValidate="txtCalendar" ErrorMessage="Date out of range" Type="Date" Enabled="false" />
            <ajaxToolkit:ValidatorCalloutExtender ID="rvCalendar_ValidatorCalloutExtender" PopupPosition="BottomLeft" 
                            runat="server" Enabled="True" TargetControlID="rvCalendar" />

        </asp:TableCell>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px" align="left" >
            <img alt="Icon" runat="server" src="~/layout/images/calendar.gif" id="imgCalender" />
        </asp:TableCell>
        <asp:TableCell style="border-left-width:thin; border-right-width:thin; border-spacing:0px; border-width:thin; margin-left:0px; margin-right:0px" align="left" >
            <%--onclick="emptyCalendar(this)"--%>
            <img alt="Icon" runat="server" src="~/layout/images/delete.gif" id="imgDelete" />
            <asp:ImageButton ID="imbDelete" runat="server" ImageAlign="Top" ImageUrl="~/layout/images/delete.gif" OnClick="imbDelete_Click" CausesValidation="False" Visible="false" />
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
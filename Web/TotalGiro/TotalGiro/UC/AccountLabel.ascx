<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountLabel.ascx.cs" Inherits="AccountLabel" %>

<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>

<asp:HyperLink ID="lbtnAccount" runat="server" ForeColor="Black" />
<cc1:TooltipImage ID="ttiAccountStatus" runat="server" TooltipShadowWidth="5" ImageAlign="AbsBottom"
    TooltipClickClose="true" TooltipDefaultImage="EmergencyExit" TooltipPadding="8"
    IsTooltipAbove="true" OffSetX="-17" />
<cc1:TooltipImage ID="ttiFeeDetails" runat="server" TooltipShadowWidth="5"
    TooltipClickClose="true" TooltipDefaultImage="Coins" TooltipPadding="8"
    IsTooltipAbove="true" OffSetX="-17" Visible="false" />
<cc1:TooltipImage ID="ttiNotification" runat="server" TooltipShadowWidth="5"
    TooltipClickClose="true" TooltipDefaultImage="Balloon_Small" TooltipPadding="8"
    IsTooltipAbove="true" OffSetX="-17" />

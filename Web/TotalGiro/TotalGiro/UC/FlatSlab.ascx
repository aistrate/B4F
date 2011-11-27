<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FlatSlab.ascx.cs" Inherits="FlatSlab" %>
<%@ Register Src="../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>
<asp:Label ID="lblFrom" runat="server" Text="More than"></asp:Label>&nbsp;
<db:DecimalBox ID="dbFrom" runat="server" DecimalPlaces="2" Value="0" Width="60px" />&nbsp;
<asp:Literal ID="tbEurot" runat="server">€</asp:Literal>
<asp:RangeValidator
    id="FromRangeValidator"
    runat="server"
    Type="Double"
    ControlToValidate="dbFrom:tbDecimal"
    ErrorMessage="Enter a number between 0,00 and 10000000,00."
    Text="*"
    MinimumValue="0"
    MaximumValue="10000000"
    />
<asp:Label ID="lblUc" runat="server" Text="Commission"></asp:Label>&nbsp;
<db:DecimalBox ID="dbPercent" runat="server" DecimalPlaces="5" Width="60px" />&nbsp;
<asp:Literal ID="litPercent" runat="server">%</asp:Literal>&nbsp;
<asp:RangeValidator 
    ID="rangeValPercent"
    Type="Double"
    MinimumValue="0,00" 
    MaximumValue="10000000,00"
    runat="server"
    ControlToValidate="dbPercent:tbDecimal"
    Text="*"
    ErrorMessage="Enter a number, format: 1000000,00."/>&nbsp;
<asp:CustomValidator 
    ID="custVal" 
    runat="server"
    Visible="false"
    OnServerValidate="checkFields"
    Text="*" 
    ErrorMessage="Enter either an amount and/or a percentage of at least '0,001'."/>&nbsp;

<db:DecimalBox ID="dbTariff" runat="server" DecimalPlaces="4" Width="60px" Visible="false" />&nbsp;
<asp:Label ID="lblTariffCurrency" runat="server" Text="€" Visible="false" />&nbsp;
<asp:RangeValidator 
    ID="rvTariff"
    Type="Double"
    MinimumValue="0,00" 
    MaximumValue="10000000,00"
    runat="server"
    ControlToValidate="dbTariff:tbDecimal"
    Text="*"
    Enabled="false"
    ErrorMessage="Enter a number, format: 1000000,00."/>&nbsp;
<asp:CustomValidator 
    ID="cvTariff" 
    runat="server"
    Visible="false"
    OnServerValidate="checkFields"
    Text="*" 
    Enabled="false"
    ErrorMessage="Enter either an amount and/or a percentage of at least '0,001'."/>&nbsp;

<asp:Label ID="lblStaticCharge" runat="server" Text="Static Charge" Visible="false" />&nbsp;
<db:DecimalBox ID="dbStaticCharge" runat="server" DecimalPlaces="2" Value="0" Width="60px" Visible="false"  />&nbsp;
<asp:Literal ID="tbEurot2" runat="server" Visible="false"  >€</asp:Literal>
<asp:RangeValidator
    id="rvStaticCharge"
    runat="server"
    Enabled="false"
    Type="Double"
    ControlToValidate="dbStaticCharge:tbDecimal"
    ErrorMessage="Enter a number between 0,00 and 10000000,00."
    Text="*"
    MinimumValue="0"
    MaximumValue="10000000"
    />
<br />


    
    


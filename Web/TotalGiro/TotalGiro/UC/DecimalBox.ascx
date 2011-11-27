<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DecimalBox.ascx.cs" Inherits="DecimalBox" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:TextBox ID="tbDecimal" 
    Style="text-align: right;" 
    SkinID="custom-width" 
    Width="80px" 
    autocomplete="off"
    runat="server" 
    OnTextChanged="tbDecimal_TextChanged"
    onkeyup="javascript:checkDecimalSeparator(event);" 
    onkeydown="javascript:checkInput(event);" 
    onchange="javascript:checkNumber(event);" />
<asp:HiddenField ID="hdfDecimalSeparator" runat="server" />
<asp:HiddenField ID="hdfNumberGroupSeparator" runat="server" />
<asp:HiddenField ID="hdfDecimalPlaces" runat="server" />
<ajaxToolkit:FilteredTextBoxExtender ID="ftbeDecimal" runat="server"
    TargetControlID="tbDecimal"         
    FilterType="Custom, Numbers"
    ValidChars=".," />
<asp:RangeValidator ID="rvDecimal" runat="server" ControlToValidate="tbDecimal" SetFocusOnError="true" Enabled="false" />
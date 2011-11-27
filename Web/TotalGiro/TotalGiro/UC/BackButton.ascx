<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BackButton.ascx.cs" Inherits="BackButton" %>
<input id="btnBack" type="button" value="Back"
    onclick="history.go(parseInt(document.getElementById('hdnClientCounter').value))" style="width: 80px"/>
<asp:HiddenField ID="hdnServerCounter" runat="server" Value="0" />

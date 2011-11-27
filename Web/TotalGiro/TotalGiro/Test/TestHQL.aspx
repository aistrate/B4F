<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestHQL.aspx.cs" Inherits="Test_TestHQL" Theme="Neutral" %>

<%@ Import Namespace="B4F.TotalGiro.Orders" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <br />
    <div>
        <div style="font-size: 1.2em; font-weight: bold; padding-bottom: 1px">HQL:</div>
        <asp:TextBox TextMode="MultiLine" ID="txtHQL" runat="server" Height="250px" Width="950px" SkinID="custom-size"></asp:TextBox>
        <br />
        <br />
        <div style="font-size: 1.2em; font-weight: bold; padding-bottom: 1px">Fields:</div>
        <asp:TextBox ID="txtFields" runat="server" Height="15px" Width="950px" SkinID="custom-size"></asp:TextBox>
        <asp:RadioButtonList ID="rblSource" runat="server" RepeatDirection="Horizontal"
            Width="426px">
            <asp:ListItem Selected="True">From Business Objects</asp:ListItem>
            <asp:ListItem>From Hibernate List</asp:ListItem>
        </asp:RadioButtonList><br />
        <asp:Button ID="btnGo" runat="server" Text="GO" OnClick="btnGo_Click" Width="103px" /><br />
        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label><br />
        <asp:Label ID="lblObjectCount" runat="server" Font-Bold="True"></asp:Label><br />
        <br />
        <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False">
            <RowStyle Wrap="False" />
        </asp:GridView>
    </div>
    <br />
</asp:Content>

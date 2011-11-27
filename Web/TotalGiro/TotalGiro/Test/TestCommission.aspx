<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestCommission.aspx.cs" Inherits="TestCommission" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Rule Overview</asp:LinkButton><br />
        <br />
        <asp:LinkButton ID="LinkButton2" runat="server" OnClick="LinkButton2_Click">Approve Orders</asp:LinkButton>&nbsp;<br />
        <br />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label><br />
        <br />
        <br />
        <br />
        <asp:LinkButton ID="LinkButton3" runat="server" OnClick="LinkButton3_Click">Change Password</asp:LinkButton></div>
    </form>
</body>
</html>

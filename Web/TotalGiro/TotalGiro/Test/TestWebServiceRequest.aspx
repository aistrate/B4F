<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestWebServiceRequest.aspx.cs" Inherits="Test_TestWebServiceRequest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnSendRequest" runat="server" Text="Send request" OnClick="btnSendRequest_Click" />
        <asp:Button ID="btnCreateTestFile" runat="server" Text="Create Test File" OnClick="btnCreateTestFile_Click" />
        <asp:Button ID="btnTestText" runat="server" OnClick="btnTestText_Click" Text="Retrieve OPAL Text" /></div>
        <asp:Label ID="lblError" runat="server"></asp:Label>
        <asp:Label ID="lblResponse" runat="server"></asp:Label>
    </form>
</body>
</html>

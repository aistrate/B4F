<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestGraphics.aspx.cs" Inherits="TestGraphics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        &nbsp;&nbsp;<br />
        <br />
        <asp:ObjectDataSource ID="odsPerformance" runat="server"></asp:ObjectDataSource>
        <br />
        <br />
        <asp:GridView ID="gvHistPos" runat="server" DataSourceID="odsHistPos">
        </asp:GridView>
        <br />
        <asp:ObjectDataSource ID="odsHistPos" runat="server" SelectMethod="GetHistPositions" 
            TypeName="B4F.TotalGiro.ApplicationLayer.Test.TestGraphicsAdapter"></asp:ObjectDataSource>
        &nbsp;&nbsp;
        <br />
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="paerelleven.aspx.cs" Inherits="Export_paerelleven"
    ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Paerel Leven Prijzen</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ValidationSummary DisplayMode="BulletList" HeaderText="The file could not be created:"
            ID="valSum" runat="server" />
        <asp:Label ID="lblPriceDate" Text="Price Date" runat="server" />
        <br />
        <asp:Calendar ID="calPricedate" runat="server"></asp:Calendar>
        <asp:CustomValidator ID="cvPricedate" runat="server" ErrorMessage="Price Date is mandatory / should be a weekday"
            OnServerValidate="cvDate_ServerValidate">*</asp:CustomValidator>
        <br />
        <asp:Label ID="lblShowDate" Text="Date to Show" runat="server" />
        <br />
        <asp:Calendar ID="calShowDate" runat="server"></asp:Calendar>
        <asp:CustomValidator ID="cvShowDate" runat="server" ErrorMessage="Show Date is mandatory / should be a weekday"
            OnServerValidate="cvDate_ServerValidate">*</asp:CustomValidator>
        <br />

        <asp:Label ID="Label1" Text="External Interface" runat="server" />
                <br />
        <asp:DropDownList ID="ddlExternalInterfaces" SkinID="broad" runat="server" AutoPostBack="true"
            DataTextField="Name" DataValueField="Key" DataSourceID="odsExternalInterfaces">
        </asp:DropDownList>
        <asp:ObjectDataSource ID="odsExternalInterfaces" runat="server" SelectMethod="GetExternalInterfaces"
            TypeName="B4F.TotalGiro.ApplicationLayer.Communicator.Export.PriceExportAdapter">
        </asp:ObjectDataSource>
        <br />
        <asp:Button ID="btnExportPrices" runat="server" Text="Download Prices" CausesValidation="true"
            OnClick="btnExportPrices_Click" />&nbsp;<br />
    </div>
    </form>
</body>
</html>

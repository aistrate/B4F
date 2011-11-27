<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="JobConsole.aspx.cs" Inherits="DataMaintenance_JobConsole" Title="Job Console" Theme="Neutral" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click"/><br />
    <asp:Label ID="lblStatus" runat="server" Visible="False" Font-Bold="True"></asp:Label><br />
    <asp:Label ID="lblHeartBeat" runat="server" Visible="False" Font-Bold="True"></asp:Label><br />
    <br />
    <asp:GridView ID="gvJobs" SkinID="spreadsheet" runat="server" AutoGenerateColumns="False" AllowSorting="True" AutoGenerateSelectButton="True" Caption="Jobs" OnSelectedIndexChanged="gvJobs_SelectedIndexChanged" PageSize="15">
        <RowStyle Wrap="False" />
        <SelectedRowStyle BorderStyle="Solid" BackColor="#AAB9C2" />
    </asp:GridView>
    <br />
    <asp:Button ID="btnStartJob" runat="server" Text="Start Job" OnClick="btnStartJob_Click" Visible="False"/>
    <asp:Button ID="btnStopJob" runat="server" Text="Stop Job" OnClick="btnStopJob_Click" Visible="False"/>
    <br />
    <asp:GridView ID="gvJobComponents" SkinID="spreadsheet" runat="server" AutoGenerateColumns="False" AllowSorting="True" Caption="Job Components" >
        <RowStyle Wrap="False" />
        <SelectedRowStyle BorderStyle="Solid" BackColor="#AAB9C2"  />
    </asp:GridView>
    <br />
    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label><br />
</asp:Content>


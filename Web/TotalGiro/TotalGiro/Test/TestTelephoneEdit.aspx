<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="TestTelephoneEdit.aspx.cs" Inherits="Test_TestTelephoneEdit" Title="Untitled Page" %>

<%@ Register Src="../UC/TelephoneDetails.ascx" TagName="TelephoneDetails" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <div style="position: relative">
        <div style="position: relative; left: -3px">
            <asp:Panel ID="pnlAccountFinder" runat="server" Width="870px">
                <uc1:TelephoneDetails ID="ctlAccountFinder" runat="server"  />
            </asp:Panel>
        </div>
    </div>
</asp:Content>


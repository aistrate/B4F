<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Settings.Settings" Codebehind="Settings.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

    <asp:Panel ID="pnlHelpText" runat="server" Visible="false">
        <p class="info">
            <b>Per post</b> means notas/reports will be printed on paper and sent to you by letter.
            These settings will only affect accounts for which you are the <i>primary</i> account holder.
        </p>
        <p class="info">
            <b>Per e-mail</b> means you will be notified (by e-mail) when new notas/reports are ready for viewing online.
        </p>
        <p class="info">
            Regardless of the settings you choose, you can always view <i>all</i> of your notas/reports online by going to 
            the <b>Afschriften</b> and <b>Rapportages</b> links on the main menu.
        </p>
        <p class="info">
            Be sure to press <b>Bewaren</b> after making any changes.
        </p>
    </asp:Panel>
    
    <table cellpadding="0" cellspacing="0">
        <tr style="height: 9px" >
            <td colspan="3"></td>
        </tr>
        <tr style="height: 0px" >
            <td style="width: 270px"></td>
            <td style="width: 100px"></td>
            <td style="width: 180px"></td>
        </tr>
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="Label1" runat="server" Text="Ontvang afschriften en kwartaalrapportage:"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:CheckBox ID="cbNotasAndQuarterlyReportsByPost" runat="server" Text="Per post" />
            </td>
            <td style="white-space: nowrap">
                <asp:CheckBox ID="cbNotasAndQuarterlyReportsByEmail" runat="server" Text="Per e-mail" />
            </td>
        </tr>
        <tr>
            <td style="height: 25px; white-space: nowrap">
                <asp:Label ID="Label3" runat="server" Text="Ontvang fiscaal jaaropgave:"></asp:Label>
            </td>
            <td style="white-space: nowrap">
                <asp:CheckBox ID="cbYearlyReportsByPost" runat="server" Text="Per post" Enabled="false" />
            </td>
            <td style="white-space: nowrap">
                <asp:CheckBox ID="cbYearlyReportsByEmail" runat="server" Text="Per e-mail" Enabled="false" />
            </td>
        </tr>
        <tr style="height:20px" >
            <td colspan="3"></td>
        </tr>
        <tr>
            <td colspan="3" style="height: 25px; padding-left: 5px">
                <asp:Button ID="btnSave" runat="server" Text="Bewaren" Width="98px" 
                    OnClick="btnSave_Click" />
            </td>
        </tr>
    </table>
    <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>

</asp:Content>


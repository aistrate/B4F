<%@ Page Language="C#" MasterPageFile="~/EG.master" Theme="Neutral" AutoEventWireup="true" CodeFile="testReportViewer.aspx.cs" Inherits="Reports_testReportViewer" Title="ReportViewer" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" style="border-color:black" width="100%">
        <tr>
            <td>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" Font-Names="Verdana" Font-Size="8pt" Height="251px" Width="1000px">
                    <ServerReport ReportServerUrl="http://145.7.20.130/ReportServer/" />
                </rsweb:ReportViewer>
            </td>
        </tr>
        
        <tr>
            <td>
                <asp:Button ID="btnShowReport" runat="server" Text="ShowReport" OnClick="btnShowReport_Click" />
            </td>
        
        </tr>
        
    </table>
</asp:Content>


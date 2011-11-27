<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ReportLetter.aspx.cs" Theme="Neutral" Inherits="Reports_ReportLetter" Title="ReportLetter" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
  <asp:ScriptManagerProxy id="sm" runat="Server"></asp:ScriptManagerProxy>
  
   <uc1:accountfinder id="ctlAccountFinder" runat="server" showmodelportfolio="false" ShowYear="false" ShowAccountNumber="false" ShowAccountName ="false" ShowTegenrekening="false" ShowSearchButton="false" ></uc1:accountfinder>
     <asp:UpdatePanel ID="upLastReportLetter" runat="server"> 
       <ContentTemplate>
         <table cellpadding="0" cellspacing="0" border="0"  style="border-color:Gray" width="100%">
           <tr>
             <td style="width: 144px"><asp:Label ID="lblKindsOfReport" runat="server" Text="Kinds of report:"></asp:Label></td>
             <td style="width: 500px"><asp:DropDownList ID="ddlReport" runat="server" Width="165px" SkinID="custom-width" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged" AutoPostBack="true" /></td>
             <td colspan="2">&nbsp</td>             
           </tr>
           <tr>
             <td style="width: 144px"><asp:Label ID="lblYear" runat="server" Text="Report year:"></asp:Label></td>
             <td style="width: 500px"><asp:DropDownList ID="ddlYear" runat="server" Width="165px" SkinID="custom-width" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" AutoPostBack="true" /></td>
             <td colspan="2">&nbsp</td>      
           </tr>      

           <tr>
            <td colspan="3" style="height:20px"></td>
           </tr>

           <tr>
             <td style="width: 144px"><asp:Label ID="lblConcerning" runat="server" Text="Concerning:"></asp:Label></td>
             <td style="width: 600px"><asp:TextBox ID="txtConcerning" runat="server" SkinID="custom-width" Enabled="false" width="600px"></asp:TextBox></td>
             <td colspan="2">&nbsp</td>     
           </tr>
                      
           <tr>
             <td style="width: 144px"><asp:Label ID="lblDescription" runat="server" Text="Description:"></asp:Label></td>
             <td style="width: 600px"><asp:TextBox ID="txtDescription" runat="server" SkinID="CustomMultiLine" Enabled="false" TextMode="MultiLine" Height="370px" width="600px"></asp:TextBox></td>
             <td style="width: 100px">
                <table>
                    <tr>
                        <td style="padding-top:5px"><asp:Label ID="lblBeginText" runat="server" Text="Begin of page"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="padding-top:320px"><asp:Label ID="lblEndText" runat="server" Text="End of page"></asp:Label></td>
                    </tr>
                </table>
             </td>     
             <td>&nbsp</td>         
           </tr>
           
           <tr>
            <td colspan="3" style="height:20px"></td>
           </tr>
                      
           <tr >
             <td colspan="3" style="width: 764px; padding-left:160px"><asp:Button ID="btnNew" runat="server" Text="New" OnClick="btnNew_Click" Width="100px" />&nbsp   
                <asp:Button ID="btnSave" runat="server" Text="Save" Enabled="false" OnClick="btnSave_Click" Width="100px" />&nbsp
                <asp:Button ID="btnPreview" runat="server" Text="Preview" OnClick="btnPreview_Click" Width="100px" />&nbsp
                <asp:Button ID="btnTest" runat="server" Text="Test" Visible="false" OnClick="btnTest_Click" Width="100px" />&nbsp
                <asp:Label ID="lblStatus" runat="server" Visible="false" Font-Italic="true"  ></asp:Label>
             </td>             
           </tr>       
         </table>
       </ContentTemplate>
     </asp:UpdatePanel>
    
</asp:Content>


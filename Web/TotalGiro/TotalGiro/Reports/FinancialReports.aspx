<%@ Page Language="C#" MasterPageFile="~/EG.master" Theme="Neutral" AutoEventWireup="true" CodeFile="FinancialReports.aspx.cs" Inherits="Reports_ReportsVB" Title="Reporting" %>
<%@ Register Src="../UC/Calendar.ascx" TagName="Calendar" TagPrefix="uc2" %>
<%@ Register Assembly="B4F.Web.WebControls" Namespace="B4F.Web.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../UC/AccountFinder.ascx" TagName="AccountFinder" TagPrefix="uc1" %>
<%@ Register Src="../UC/AccountLabel.ascx" TagName="AccountLabel" TagPrefix="uc4" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
   <asp:ScriptManagerProxy id="sm" runat="Server"></asp:ScriptManagerProxy>
   <uc1:accountfinder id="ctlAccountFinder" runat="server" showmodelportfolio="true" ShowYear="true"></uc1:accountfinder>
   <asp:Panel ID="pnlSelectedAccounts" runat="server" Visible="false" Width="100%">

   <table cellpadding="0" cellspacing="0" border="0" style="border-color:black" width="100%">
      <!-- Rows02  : MultiGrid - Use Preselected Dates | Start and EndDates - rapportages - Betreft | Omschrijving - RadioButton - Buttons -->          
     <tr><td colspan ="4">
       <table cellpadding="1" cellspacing="1" border="0" style="border-color:Yellow" width="100%">
       <!-- Row03  : MultiGrid -->                  
         <tr>
            <td colspan="4">
                <cc1:MultipleSelectionGridView 
                    ID="gvAccounts" 
                    runat="server" 
                    AllowPaging="True" 
                    AllowSorting="True"
                    AutoGenerateColumns="False" 
                    Caption="Accounts" 
                    CaptionAlign="Left" 
                    DataKeyNames="Key"
                    DataSourceID="odsAccounts"
                    OnRowDataBound="gvAccounts_RowDataBound"
                    PageSize="10" SelectionBoxEnabledBy="HasPrimaryAH">
                    <Columns>
                        <asp:TemplateField HeaderText="Account#" SortExpression="Number" >
                            <ItemTemplate>
                                <uc4:AccountLabel ID="ctlAccountLabel" 
                                    runat="server" 
                                    RetrieveData="false" 
                                    Width="120px" 
                                    NavigationOption="PortfolioView"
                                    />
                            </ItemTemplate>
                            <HeaderStyle wrap="False" />
                            <ItemStyle wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Account Name" SortExpression="ShortName">
                            <ItemTemplate>
                                <trunc:TruncLabel2 ID="lblShortName" runat="server" Width="100px" CssClass="padding"
                                    MaxLength="30" LongText='<%# DataBinder.Eval(Container.DataItem, "ShortName") %>' />
                            </ItemTemplate>
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:TemplateField>
                         <asp:BoundField DataField="AccountOwner_CompanyName" HeaderText="Asset Manager" ReadOnly="True" SortExpression="AccountOwner_CompanyName">
                           <itemstyle wrap="False" />
                           <headerstyle wrap="False" />
                         </asp:BoundField>
                         <asp:TemplateField HeaderText="Model" SortExpression="ModelPortfolioName">
                           <itemstyle width="250px" wrap="False" />
                           <headerstyle wrap="False" />
                           <itemtemplate>
                           <%# DataBinder.Eval(Container.DataItem, "ModelPortfolioName")%>
                           </ItemTemplate>
                         </asp:TemplateField>
                    </Columns>
                </cc1:MultipleSelectionGridView>
            </td>
          </tr>
             
              <!-- Row04  : Use preSelected Dates | Start and EndDates -->      
          <tr>
            <td colspan="4">
              <asp:UpdatePanel ID="upDetails" runat="server">
              <ContentTemplate>
                <table cellpadding="1" cellspacing="1" border="0" style="border-color:black" width="100%">
                  <tr>
                    <td style="width: 330px"><asp:CheckBox ID="cbUsePreselectedDates" runat="server" Text="Use Preselected Dates:" Checked="true" OnCheckedChanged="cbUsePreselectedDates_CheckedChanged" AutoPostBack="true" Width="170px" /></td>
                    <td style="width: 210px"><asp:DropDownList ID="ddlQuarter" runat="server" Width="100px" SkinID="custom-width" OnSelectedIndexChanged="ddlQuarter_SelectedIndexChanged"  AutoPostBack="true" /></td>
                    <td style="width: 30px">&nbsp</td>
                    <td><asp:DropDownList ID="ddlYear" runat="server" Width="100px" SkinID="custom-width" Visible="false"/></td>
                  </tr>

                  <tr>
                    <td style="width: 330px;"><asp:Label ID="lblFrom" runat="server" Text="From:" Visible="false" width="170px"/></td>
                    <td style="width: 210px" ><uc2:Calendar ID="ctlBeginDate" runat="server" IsButtonDeleteVisible="false" Visible="false" Format="dd-MM-yyyy" /></td>
                    <td style="width: 30px"><asp:Label ID="lblTo" runat="server" Text="To:" Visible="false" /></td>               
                    <td><uc2:Calendar ID="ctlEndDate" runat="server" IsButtonDeleteVisible="false" Visible="false" Format="dd-MM-yyyy" /></td>
                  </tr>
                </table>
              </ContentTemplate>
              </asp:UpdatePanel>
            </td>
          </tr>

          <!-- Row05  : rapportages -->     
          <tr>
            <td colspan="4">
              <asp:UpdatePanel ID="upKindsOfReports" runat="server"> 
              <ContentTemplate>
                <table cellpadding="1" cellspacing="1" border="0" style="border-color:Blue" width="100%">
                  <tr><td style="width: 330px">Portefeuille ontwikkeling over de periode:</td>
                      <td style="width: 100px"><asp:CheckBox ID="chkPortfolioDevelopment" runat="server" Checked="true" OnCheckedChanged="chkPortfolioDevelopment_CheckedChanged" AutoPostBack="true" /></td>
                      <td style="width: 20px">&nbsp</td>
                      <td style="width: 130px">Fiscaal jaaroverzicht:</td>
                      <td><asp:CheckBox ID="chkFiscaalJaaroverzicht" runat="server" Checked="false" Enabled="false" OnCheckedChanged="chkFiscaalJaaroverzicht_CheckedChanged" AutoPostBack="true" /></td>                        
                  </tr>
                  <tr><td style="width: 330px;">Portefeuille samenvatting per rapport datum:</td>
                      <td colspan= "4"><asp:CheckBox ID="chkPortfolioSummary" runat="server" Checked="true" OnCheckedChanged="chkPortfolioSummary_CheckedChanged" AutoPostBack="true" /></td>
                  </tr>
                  <tr><td style="width: 330px">Portefeuille overzicht per rapport datum:</td>
                      <td colspan= "4"><asp:CheckBox ID="chkPortfolioOverview" runat="server" Checked="true" OnCheckedChanged="chkPortfolioOverview_CheckedChanged" AutoPostBack="true" /></td>
                  </tr>
                  <tr><td style="width: 330px">Specificatie van transacties en gerealiseerde resultaten:</td>
                      <td colspan= "4"><asp:CheckBox ID="chkTransaction" runat="server" Checked="true" OnCheckedChanged="chkTransaction_CheckedChanged" AutoPostBack="true" /></td>
                  </tr>
                  <tr><td style="width: 330px">Specificatie van geldmutaties:</td>
                      <td colspan= "4"><asp:CheckBox ID="chkMoneyMutations" runat="server" Checked="true" OnCheckedChanged="chkMoneyMutations_CheckedChanged" AutoPostBack="true" /></td>
                  </tr>
                </table>
              </ContentTemplate>
              </asp:UpdatePanel>

            </td>                
          </tr>

          <!-- Row06  : Betreft | Omschrijving-->
          <tr>
             <td colspan="4">
               <asp:UpdatePanel ID="upLastReportLetter" runat="server"> 
               <ContentTemplate>
               <table cellpadding="1" cellspacing="1" border="0" style="border-color:Red" width="100%">
                 <tr>
                   <td style="width: 144px"><asp:Label ID="lblConcerning" runat="server" Text="Concerning:"></asp:Label></td>
                   <td style="width: 600px" colspan="3"><asp:TextBox ID="txtConcerning" runat="server" SkinID="custom-width" Enabled="false" width="600px"></asp:TextBox></td>
                  </tr>
                  
                 <tr>
                   <td style="width: 144px"><asp:Label ID="lblDescription" runat="server" Text="Description:"></asp:Label></td>
                   <td style="width: 600px" colspan="3"><asp:TextBox ID="txtDescription" runat="server" SkinID="CustomMultiLine" Enabled="false" TextMode="MultiLine" Height="310px" width="600px"></asp:TextBox></td>
                 </tr> 
               </table>
               </ContentTemplate>
              </asp:UpdatePanel>                   
             </td>
          </tr>              

          <!-- Row07  : Formal Cover page | Chart cover page-->
          <tr>
             <td colspan="4">
               <asp:UpdatePanel ID="upKindsOfCaverpages" runat="server">
                 <ContentTemplate>
                   <table cellpadding="1" cellspacing="1" border="0" style="border-color:Blue" width="100%">                    
                     <tr>
                       <td style="width: 150px">
                       <asp:Label ID="lblFormatCoverPage" runat="server" Text="Formal cover page:" Width="150px"></asp:Label></td>
                       <td style="width: 150px"><asp:RadioButton ID="rdoCover1" runat="server" Checked="false" GroupName="CoverPage" ValidationGroup="CoverPage" AutoPostBack="true" Width="300px" /></td>
                     </tr>
                     <tr>
                       <td style="width: 150px"><asp:Label ID="lblChartCoverPage" runat="server" Text="Chart cover page:"></asp:Label></td>
                       <td style="width: 150px" colspan="3"><asp:RadioButton ID="rdoCover2" runat="server" Checked="true" GroupName="CoverPage" ValidationGroup="CoverPage" AutoPostBack="true" Width="300px" /></td>                         
                     </tr>
                   </table>
                 </ContentTemplate>
               </asp:UpdatePanel>
             </td>
          </tr>
          
           <!-- Row08  : Buttons -->
		   <tr>
             <td colspan="4" style="height: 30px">
                <b4f:ErrorLabel ID="elbErrorMessage" runat="server" Width="750px"></b4f:ErrorLabel><br />
                <table cellpadding="1" cellspacing="1" border="0">
                    <tr>
                        <td style="width: 630px">
                            <asp:Button ID="btnPrint" runat="server" Text="Print" OnClick="btnPrint_Click" Width="130px" />&nbsp;
                            <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" Width="130px" />
                        </td>
                        <td style="width: 150px; text-align: right">
                            <asp:Button ID="btnViewResult" runat="server" Text="View Summary" OnClick="btnViewResult_Click" Width="130px" />
                        </td>
                    </tr>
                </table>
            </td>
          </tr>

        </table>

       </td>
     </tr>
   </table>

  </asp:Panel>

  <asp:ObjectDataSource ID="odsAccounts" runat="server"
    SelectMethod="GetCustomerAccounts" TypeName="B4F.TotalGiro.ApplicationLayer.Reports.FinancialReportAdapter">
    <SelectParameters>
      <asp:ControlParameter ControlID="ctlAccountFinder" Name="assetManagerId" PropertyName="AssetManagerId" Type="Int32" />
      <asp:ControlParameter ControlID="ctlAccountFinder" Name="modelPortfolioId" PropertyName="ModelPortfolioId" Type="Int32" />
      <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountNumber" PropertyName="AccountNumber" Type="String" />
      <asp:ControlParameter ControlID="ctlAccountFinder" Name="accountName" PropertyName="AccountName" Type="String" />
      <asp:ControlParameter ControlID="ctlAccountFinder" Name="year" PropertyName="Year" Type="Int32" />
      <asp:Parameter Name="propertyList" Type="String" 
                     DefaultValue="Key, ShortName, Number, AccountOwner.CompanyName, ModelPortfolioName, LastDateStatusChanged, HasPrimaryAH" />
    </SelectParameters>
 </asp:ObjectDataSource>
 
</asp:Content>

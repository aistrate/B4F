<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Planning.FinancialPlanner" Codebehind="FinancialPlanner.aspx.cs" %>

<%@ Register Src="~/UC/PortfolioNavigationBar.ascx" TagName="PortfolioNavigationBar" TagPrefix="b4f" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />
    
    <b4f:PortfolioNavigationBar ID="ctlPortfolioNavigationBar" runat="server" ShowPlanner="false" />
    
    <asp:UpdatePanel ID="updPlanner" runat="server">
        <ContentTemplate>
            
            <asp:Panel ID="pnlAccountList" runat="server" Visible="false" Width="855px">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="lblAccountLabel" runat="server" Text="Rekening:"></asp:Label>
                        </td>
                        <td style="white-space: nowrap">
                            <asp:DropDownList ID="ddlAccount" SkinID="custom-width" runat="server" Width="300px" DataSourceID="odsAccount" 
                                DataTextField="DisplayNumberWithName" DataValueField="Key" AutoPostBack="True" TabIndex="10"
                                OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" OnDataBound="ddlAccount_DataBound">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsAccount" runat="server" SelectMethod="GetContactAccounts"
                                                  TypeName="B4F.TotalGiro.ClientApplicationLayer.Planning.FinancialPlannerAdapter">
                                <SelectParameters>
                                    <asp:Parameter Name="hasEmptyFirstRow" Type="Boolean" DefaultValue="False" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                        <td style="white-space: nowrap; text-align: left">
                            <b4f:ArrowsLinkButton ID="lnkClearAll" runat="server" SkinID="padding" TabIndex="11" Visible="false"
                                                  OnCommand="lnkClearAll_Command" Text="Clear all">
                            </b4f:ArrowsLinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 155px; height: 4px"></td>
                        <td style="width: 340px"></td>
                        <td style="width: 150px"></td>
                    </tr>
                </table>
                
                <hr style="width: 855px"/>
                
            </asp:Panel>
    
            <asp:Panel ID="pnlGivenFields" runat="server" Visible="true" Width="855px">
                
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width: 155px; height: 2px"></td>
                        <td style="width: 300px"></td>
                        <td style="width: 300px"></td>
                        <td style="width: 50px"></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label1" runat="server" Text="Bekende gegevens:" Font-Bold="true"
                                       Font-Size="1.1em"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label2" runat="server" Text="Model:"></asp:Label>
                        </td>
                        <td colspan="2" style="white-space: nowrap">
                            <asp:MultiView ID="mvwModelPortfolio" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwModelPortfolio0" runat="server">
                                    <asp:DropDownList ID="ddlModelPortfolio" runat="server" Width="250px" SkinID="custom-width"
                                        DataSourceID="odsModelPortfolio" DataTextField="ModelName" DataValueField="Key" TabIndex="12"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlModelPortfolio_SelectedIndexChanged">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsModelPortfolio" runat="server" SelectMethod="GetModelPortfolios"
                                                          TypeName="B4F.TotalGiro.ClientApplicationLayer.Planning.FinancialPlannerAdapter">
                                        <SelectParameters>
                                            <asp:Parameter Name="hasEmptyFirstRow" Type="Boolean" DefaultValue="False" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </asp:View>
                                <asp:View ID="vwModelPortfolio1" runat="server">
                                    <asp:Label ID="lblModelName" runat="server" Font-Bold="True"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                        <td rowspan="9" style="text-align: right; vertical-align: bottom">
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="250">
                                <ProgressTemplate>
                                    <asp:Image ID="imgPlanner" runat="server" ImageUrl="~/Images/Wheel.gif"/>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label3" runat="server" Text="Huidige waarde:"></asp:Label>
                        </td>
                        <td colspan="2" style="white-space: nowrap">
                            <asp:MultiView ID="mvwPresentValue" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwPresentValue0" runat="server">
                                    <asp:TextBox ID="txtPresentValue" runat="server" TabIndex="13" CssClass="alignright">
                                    </asp:TextBox>&nbsp;&nbsp;€
                                    <asp:RangeValidator ID="rvPresentValue" runat="server" 
                                        ControlToValidate="txtPresentValue"
                                        MinimumValue="0" MaximumValue="2000000000" Type="Double" SetFocusOnError="true" 
                                        Width="0px" ValidationGroup="GivenFields">*</asp:RangeValidator>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="fltPresentValue" runat="server" 
                                        TargetControlID="txtPresentValue"
                                        FilterType="Custom, Numbers" ValidChars=","></ajaxToolkit:FilteredTextBoxExtender>
                                </asp:View>
                                <asp:View ID="vwPresentValue1" runat="server">
                                    <asp:Label ID="lblPresentValue" runat="server" Font-Bold="True" Width="130px"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label4" runat="server" Text="Prognoserendement:"></asp:Label>
                        </td>
                        <td colspan="2" style="white-space: nowrap">
                            <asp:MultiView ID="mvwExpectedReturn" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwExpectedReturn0" runat="server">
                                    <asp:TextBox ID="txtExpectedReturn" runat="server" TabIndex="14" CssClass="alignright">
                                    </asp:TextBox>&nbsp;&nbsp;%
                                    <asp:RequiredFieldValidator ID="rfvExpectedReturn" runat="server" 
                                        ControlToValidate="txtExpectedReturn"
                                        InitialValue="" SetFocusOnError="True" Width="0px"
                                        ValidationGroup="GivenFields">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvExpectedReturn" runat="server" 
                                        ControlToValidate="txtExpectedReturn"
                                        MinimumValue="0" MaximumValue="2000000000" Type="Double" SetFocusOnError="true"
                                        Width="0px" ValidationGroup="GivenFields">*</asp:RangeValidator>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="fltExpectedReturn" runat="server" 
                                        TargetControlID="txtExpectedReturn"
                                        FilterType="Custom, Numbers" ValidChars=","></ajaxToolkit:FilteredTextBoxExtender>
                                </asp:View>
                                <asp:View ID="vwExpectedReturn1" runat="server">
                                    <asp:Label ID="lblExpectedReturn" runat="server" Font-Bold="True"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label17" runat="server" Text="Volatiliteit:"></asp:Label>
                        </td>
                        <td colspan="2" style="white-space: nowrap">
                            <asp:MultiView ID="mvwStandardDeviation" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwStandardDeviation0" runat="server">
                                    <asp:TextBox ID="txtStandardDeviation" runat="server" TabIndex="15" CssClass="alignright">
                                    </asp:TextBox>&nbsp;&nbsp;%
                                    <asp:RequiredFieldValidator ID="rfvStandardDeviation" runat="server" 
                                        ControlToValidate="txtStandardDeviation"
                                        InitialValue="" SetFocusOnError="True" Width="0px"
                                        ValidationGroup="GivenFields">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvStandardDeviation" runat="server" 
                                        ControlToValidate="txtStandardDeviation"
                                        MinimumValue="0,001" MaximumValue="2000000000" Type="Double" SetFocusOnError="true"
                                        Width="0px" ValidationGroup="GivenFields">*</asp:RangeValidator>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="fltStandardDeviation" runat="server" 
                                        TargetControlID="txtStandardDeviation"
                                        FilterType="Custom, Numbers" ValidChars=","></ajaxToolkit:FilteredTextBoxExtender>
                                </asp:View>
                                <asp:View ID="vwStandardDeviation1" runat="server">
                                    <asp:Label ID="lblStandardDeviation" runat="server" Font-Bold="True"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label5" runat="server" Text="Doelvermogen:"></asp:Label>
                        </td>
                        <td style="white-space: nowrap">
                            <asp:MultiView ID="mvwTargetValue" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwTargetValue0" runat="server">
                                    <asp:TextBox ID="txtTargetValue" runat="server" TabIndex="16" CssClass="alignright">
                                    </asp:TextBox>&nbsp;&nbsp;€
                                    <asp:RequiredFieldValidator ID="rfvTargetValue" runat="server" 
                                        ControlToValidate="txtTargetValue"
                                        InitialValue="" SetFocusOnError="True" Width="0px"
                                        ValidationGroup="GivenFields">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvTargetValue" runat="server" ControlToValidate="txtTargetValue"
                                        MinimumValue="1" MaximumValue="2000000000" Type="Double" SetFocusOnError="true" 
                                        Width="0px" ValidationGroup="GivenFields">*</asp:RangeValidator>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="fltTargetValue" runat="server" TargetControlID="txtTargetValue"
                                        FilterType="Custom, Numbers" ValidChars=","></ajaxToolkit:FilteredTextBoxExtender>
                                </asp:View>
                                <asp:View ID="vwTargetValue1" runat="server">
                                    <asp:Label ID="lblTargetValue" runat="server" Font-Bold="True" Width="130px"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                        <td style="text-align: left; vertical-align: bottom">
                            <asp:Panel ID="pnlTargetUpdate" runat="server" Visible="false">
                                <div style="position: relative" class="screen-only">
                                    <asp:MultiView ID="mvwTargetUpdate" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                        <asp:View ID="vwTargetUpdate0" runat="server">
                                            <b4f:ArrowsLinkButton ID="lnkUpdateTarget" runat="server" TabIndex="20"
                                                                  OnCommand="lnkUpdateTarget_Command" Text="Doelvermogen aanpassen"
                                                                  style="position: relative; top: 0px"
                                                                  CausesValidation="false">
                                            </b4f:ArrowsLinkButton>
                                        </asp:View>
                                        <asp:View ID="vwTargetUpdate1" runat="server">
                                            <b4f:ArrowsLinkButton ID="lnkSaveTarget" runat="server" TabIndex="21"
                                                                  OnCommand="lnkSaveTarget_Command" Text="Doelvermogen opslaan"
                                                                  style="position: relative; top: 0px"
                                                                  CausesValidation="true" ValidationGroup="GivenFields">
                                            </b4f:ArrowsLinkButton>
                                            <span style="display: inline-block; width: 20px;"></span>
                                            <b4f:ArrowsLinkButton ID="lnkCancelUpdateTarget" runat="server" TabIndex="22"
                                                                  OnCommand="lnkCancelUpdateTarget_Command" Text="Annuleer"
                                                                  style="position: relative; top: 0px"
                                                                  CausesValidation="false">
                                            </b4f:ArrowsLinkButton>
                                        </asp:View>
                                    </asp:MultiView>
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label6" runat="server" Text="Einde looptijd:"></asp:Label>
                        </td>
                        <td colspan="2" style="white-space: nowrap">
                            <asp:MultiView ID="mvwTargetEndDate" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwTargetEndDate0" runat="server">
                                
                                    <asp:DropDownList ID="ddlMonth" SkinID="custom-width" runat="server" Width="90px" DataSourceID="odsMonth" 
                                        DataTextField="Value" DataValueField="Key" AutoPostBack="False" TabIndex="17">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsMonth" runat="server" SelectMethod="GetMonths"
                                                          TypeName="B4F.TotalGiro.ClientApplicationLayer.Planning.FinancialPlannerAdapter">
                                        <SelectParameters>
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    
                                    <asp:DropDownList ID="ddlYear" SkinID="custom-width" runat="server" Width="55px" DataSourceID="odsYear" 
                                        DataTextField="Value" DataValueField="Key" AutoPostBack="False" TabIndex="18">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsYear" runat="server" SelectMethod="GetYears"
                                                          TypeName="B4F.TotalGiro.ClientApplicationLayer.Planning.FinancialPlannerAdapter">
                                        <SelectParameters>
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    
                                    &nbsp;
                                    <asp:RequiredFieldValidator ID="rfvMonth" runat="server" ControlToValidate="ddlMonth"
                                        InitialValue="0" SetFocusOnError="True" Width="0px"
                                        ValidationGroup="GivenFields">*/</asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rfvYear" runat="server" ControlToValidate="ddlYear"
                                        InitialValue="0" SetFocusOnError="True" Width="0px"
                                        ValidationGroup="GivenFields">/*</asp:RequiredFieldValidator>
                                </asp:View>
                                <asp:View ID="vwTargetEndDate1" runat="server">
                                    <asp:Label ID="lblTargetEndDate" runat="server" Font-Bold="True" Width="130px"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px; white-space: nowrap">
                            <asp:Label ID="Label7" runat="server" Text="U stort nu:"></asp:Label>
                        </td>
                        <td style="white-space: nowrap">
                            <asp:MultiView ID="mvwDepositPerYear" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                <asp:View ID="vwDepositPerYear0" runat="server">
                                    <asp:TextBox ID="txtDepositPerYear" runat="server" TabIndex="19" CssClass="alignright">
                                    </asp:TextBox>&nbsp;&nbsp;€ / jaar
                                    <asp:RangeValidator ID="rvDepositPerYear" runat="server" ControlToValidate="txtDepositPerYear"
                                        MinimumValue="0" MaximumValue="2000000000" Type="Double" SetFocusOnError="true" 
                                        Width="0px" ValidationGroup="GivenFields">*</asp:RangeValidator>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="fltDepositPerYear" runat="server" TargetControlID="txtDepositPerYear"
                                        FilterType="Custom, Numbers" ValidChars=","></ajaxToolkit:FilteredTextBoxExtender>
                                </asp:View>
                                <asp:View ID="vwDepositPerYear1" runat="server">
                                    <asp:Label ID="lblDepositPerYear" runat="server" Font-Bold="True" Width="130px"></asp:Label>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                        <td style="text-align: left; vertical-align: top">
                            <div style="position: relative; height: 25px" class="screen-only">
                                <asp:Button ID="btnCalculate" runat="server" OnClick="btnCalculate_Click" Text="Bereken" TabIndex="23"
                                            style="position: relative; top: -1px" Width="110px" CausesValidation="true"
                                            ValidationGroup="GivenFields"/>
                                <div style="position: absolute; left: 117px; top: 5px">
                                    <asp:ValidationSummary ID="vsGivenFields" runat="server" Height="0px"
                                        ValidationGroup="GivenFields" HeaderText="* Deze gegevens zijn verplicht" />
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 4px" colspan="3"></td>
                    </tr>
                </table>
                
            </asp:Panel>
            
            <b4f:ErrorLabel ID="elbGivenFields" runat="server" PrecedingNewline="true" Width="700px"
                            Font-Bold="false"></b4f:ErrorLabel>
            
            <asp:Panel ID="pnlFutureValue" runat="server" Visible="false" Width="855px" Height="780px">
                
                <hr style="width: 855px; padding: 0px; margin: 0px"/>
                
                <table cellpadding="0" cellspacing="0">
                    <asp:Panel ID="pnlErrorFutureValue" runat="server" Visible="false">
                        <tr>
                            <td colspan="2" style="height: 0px">
                                <b4f:ErrorLabel ID="elbFutureValue" runat="server" PrecedingNewline="false" Width="700px"
                                                Font-Bold="true"></b4f:ErrorLabel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="height: 7px"></td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td style="width: 580px; height: 2px"></td>
                        <td style="width: 110px"></td>
                        <td style="width: 160px"></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; height: 245px">
                
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="width: 330px; height: 0px"></td>
                                    <td style="width: 200px"></td>
                                </tr>
                                <tr>
                                    <td style="height: 25px; white-space: nowrap">
                                        <asp:Label ID="Label8" runat="server" Text="Verwachte eindwaarde zonder aanpassing:" 
                                                   Font-Bold="true" Font-Size="1.1em"></asp:Label>
                                    </td>
                                    <td style="white-space: nowrap">
                                        <asp:Label ID="lblFutureValueBeforeAdjust" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                </tr>
                                <asp:Panel ID="pnlEndOfPeriod" runat="server" Visible="true">
                                    <tr>
                                        <td style="height: 25px; white-space: nowrap">
                                            <asp:Label ID="Label11" runat="server" Text="Einde looptijd (in hele jaren vanaf huidige maand):"></asp:Label>
                                        </td>
                                        <td style="white-space: nowrap">
                                            <asp:Label ID="lblEndOfPeriod" runat="server" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                </asp:Panel>
                                <tr>
                                    <td style="height: 12px" colspan="2"></td>
                                </tr>
                                <tr>
                                    <td style="height: 25px; white-space: nowrap">
                                        <asp:Label ID="Label12" runat="server" Text="Kans op halen doelstellingen:" 
                                                   Font-Bold="true" Font-Size="1.1em"></asp:Label>
                                    </td>
                                    <td style="white-space: nowrap">
                                        <asp:Label ID="lblChanceOfMeetingTarget" runat="server" 
                                                   Font-Bold="True" Font-Size="1.1em"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="width: 330px; height: 12px"></td>
                                    <td style="width: 200px"></td>
                                </tr>
                                <tr>
                                    <td style="height: 25px; white-space: nowrap">
                                        <asp:Label ID="lblChoiceOfExtras" runat="server" Font-Bold="True" Font-Size="1.1em"></asp:Label>
                                    </td>
                                    <td style="white-space: nowrap">
                                        <b4f:ArrowsLinkButton ID="lnkChoiceOfExtras" runat="server" SkinID="padding" TabIndex="101"
                                                              OnCommand="lnkChoiceOfExtras_Command"></b4f:ArrowsLinkButton>
                                    </td>
                                </tr>
                            </table>
                                                                
                            <asp:MultiView ID="mvwExtras" runat="server" ActiveViewIndex="0" EnableTheming="True">
                                
                                <asp:View ID="vwExtras0" runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="width: 330px; height: 3px"></td>
                                            <td style="width: 10px"></td>
                                            <td style="width: 10px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px; white-space: nowrap">
                                                <asp:Label ID="Label13" runat="server">Extra periodieke inleg:</asp:Label>
                                            </td>
                                            <td style="white-space: nowrap" align="right">
                                                <asp:Label ID="lblProposedPeriodical" runat="server" Font-Bold="True"></asp:Label>
                                            </td>
                                            <td style="white-space: nowrap" align="left">
                                                &nbsp;/ jaar
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px; white-space: nowrap">
                                                <asp:Label ID="Label14" runat="server">OF extra eenmalige inleg:</asp:Label>
                                            </td>
                                            <td style="white-space: nowrap" align="right">
                                                <asp:Label ID="lblProposedInitial" runat="server" Font-Bold="True"></asp:Label>
                                            </td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </asp:View>
                                
                                <asp:View ID="vwExtras1" runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="width: 177px; height: 3px"></td>
                                            <td style="width: 15px"></td>
                                            <td style="width: 150px"></td>
                                            <td style="width: 15px"></td>
                                            <td style="width: 25px"></td>
                                            <td style="width: 140px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px; white-space: nowrap">
                                                <asp:Label ID="Label9" runat="server">Extra periodieke inleg:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblExtraPeriodicalMin" runat="server" Text="0"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtExtraPeriodical" runat="server" Text="0"
                                                             AutoPostBack="true"></asp:TextBox>
                                            </td>
                                            <td style="text-align: right">
                                                <asp:Label ID="lblExtraPeriodicalMax" runat="server"></asp:Label>
                                            </td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txtExtraPeriodical_BoundControl" runat="server" CssClass="alignright" 
                                                             AutoPostBack="true" OnTextChanged="txtExtra_TextChanged" TabIndex="102"
                                                             SkinID="custom-width" Width="60px"></asp:TextBox>&nbsp;&nbsp;€ / jaar
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px; white-space: nowrap">
                                                <asp:Label ID="Label10" runat="server">Extra eenmalige inleg:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblExtraInitialMin" runat="server" Text="0"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtExtraInitial" runat="server" Text="0"
                                                             AutoPostBack="true"></asp:TextBox>
                                            </td>
                                            <td style="text-align: right">
                                                <asp:Label ID="lblExtraInitialMax" runat="server"></asp:Label>
                                            </td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txtExtraInitial_BoundControl" runat="server" CssClass="alignright" 
                                                             AutoPostBack="true" OnTextChanged="txtExtra_TextChanged" TabIndex="103"
                                                             SkinID="custom-width" Width="60px"></asp:TextBox>&nbsp;&nbsp;€
                                            </td>
                                        </tr>
                                    </table>
                                        
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="width: 330px; height: 15px"></td>
                                            <td style="width: 10px"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px; white-space: nowrap">
                                                <asp:Label ID="Label15" runat="server" Text="Verwachte eindwaarde met aanpassing:" 
                                                           Font-Bold="true" Font-Size="1.1em"></asp:Label>
                                            </td>
                                            <td style="white-space: nowrap; text-align: right">
                                                <asp:Label ID="lblFutureValueAfterAdjust" runat="server" Font-Bold="True"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 25px; white-space: nowrap">
                                                <asp:Label ID="Label16" runat="server" Text="Doelvermogen:" 
                                                           Font-Bold="true" Font-Size="1.1em"></asp:Label>
                                            </td>
                                            <td style="white-space: nowrap; text-align: right">
                                                <asp:Label ID="lblTargetValueExtras" runat="server" Font-Bold="True"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>

                                    <ajaxToolkit:SliderExtender ID="sldExtraPeriodical" runat="server" Length="150"
                                        BehaviorID="txtExtraPeriodical" TargetControlID="txtExtraPeriodical" 
                                        BoundControlID="txtExtraPeriodical_BoundControl" RaiseChangeOnlyOnMouseUp="true"
                                        Minimum="0" Maximum="0" EnableHandleAnimation="false" >
                                    </ajaxToolkit:SliderExtender>
                                    
                                    <ajaxToolkit:SliderExtender ID="sldExtraInitial" runat="server" Length="150"
                                        BehaviorID="txtExtraInitial" TargetControlID="txtExtraInitial" 
                                        BoundControlID="txtExtraInitial_BoundControl" RaiseChangeOnlyOnMouseUp="true"
                                        Minimum="0" Maximum="0" EnableHandleAnimation="false" >
                                    </ajaxToolkit:SliderExtender>
                                </asp:View>
                                
                            </asp:MultiView>
                        
                        </td>
                        
                        <td style="vertical-align: top">
                            <asp:Panel ID="pnlTrafficLight" runat="server">
                                <table cellpadding="0" cellspacing="0" style="float:right">
                                    <tr>
                                        <td style="width: 10px; height: 10px; border: Solid 2px Black">
                                            <asp:Image ID="imgTrafficLight" runat="server" Height="136px" CssClass="plain"
                                                       BorderStyle="Solid" BorderColor="White" BorderWidth="1px"/>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        
                        <td style="vertical-align: top; text-align: right">
                            <input id="btnPrint" type="button" value="Afdrukken..." class="push-button screen-only" style="width: 110px"
                                   onclick="window.print(); return false;" tabindex="110" />
                        </td>
                    </tr>
                </table>
                
                <dundas:Chart ID="chPortfolioFutureValue" runat="server" BorderLineColor="26, 59, 105" EnableViewState="true" 
                              ViewStateContent="All" Palette="DundasDark" Width="850px" Height="500px" AntiAliasing="Graphics" 
                              BorderLineWidth="0">
                    <Legend Alignment="Far" AutoFitText="False" BackColor="#F8FCFF" BorderColor="26, 59, 105" Reversed="True"
                            Docking="Top" EquallySpacedItems="False" LegendStyle="Row" Font="Trebuchet MS, 12px"
                            Name="Default" ShadowOffset="2" TableStyle="Wide" FontColor="#313457">
                    </Legend>
                    <BorderSkin FrameBackGradientEndColor="CornflowerBlue" PageColor="Window" FrameBorderColor="100, 0, 0, 0"
                        FrameBorderWidth="2" FrameBackColor="CornflowerBlue"></BorderSkin>
                    <ChartAreas>
                        <dundas:ChartArea BorderColor="26, 59, 105" BackColor="White" BorderStyle="Solid" ShadowOffset="2"
                            Name="Default">
                            <AxisX Margin="False" LabelsAutoFitMaxFontSize="9" LabelsAutoFitMinFontSize="7" IntervalAutoMode="VariableCount">
                                <MinorGrid LineColor="Silver"></MinorGrid>
                                <LabelStyle Format="MMM\\nyyyy" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                            IntervalType="Auto" Font="Trebuchet MS, 12px" FontColor="#313457"></LabelStyle>
                                <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                           IntervalType="Years"></MajorGrid>
                                <MinorTickMark Size="2"></MinorTickMark>
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Years" />
                            </AxisX>
                            <Area3DStyle WallWidth="10" Light="Realistic" RightAngleAxes="False"></Area3DStyle>
                            <AxisY LabelsAutoFitMinFontSize="7" LabelsAutoFitMaxFontSize="9" StartFromZero="true">
                                <MinorGrid LineColor="Silver"></MinorGrid>
                                <LabelStyle Format="C0" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                            Font="Trebuchet MS, 12px" FontColor="#313457" IntervalType="Auto"></LabelStyle>
                                <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                    IntervalType="Auto"></MajorGrid>
                                <MinorTickMark Size="2"></MinorTickMark>
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisY>
                            <AxisX2>
                                <MinorGrid LineColor="Silver"></MinorGrid>
                                <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                    IntervalType="Auto"></MajorGrid>
                                <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisX2>
                            <AxisY2>
                                <MinorGrid LineColor="Silver"></MinorGrid>
                                <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                    IntervalType="Auto"></MajorGrid>
                                <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                            </AxisY2>
                        </dundas:ChartArea>
                    </ChartAreas>
                    <Titles>
                        <dundas:Title Font="Trebuchet MS, 14px, style=Bold" Name="PortfolioFutureValue" Text="" 
                                      Alignment="TopLeft" Color="#313457">
                            <Position Height="10" Width="90" X="3" Y="3" />
                        </dundas:Title>
                    </Titles>
                </dundas:Chart>
                
                <p class="print-only info" style="width: 770px">
                    <i>* Bovenstaande berekening is louter indicatief. Aan deze berekening kunnen geen rechten worden ontleend.</i>
                </p>
            
            </asp:Panel>
            
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

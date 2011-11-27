<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="CalculationsEdit.aspx.cs" Inherits="Calculations" Title="Edit Calculation" %>

<%@ Register TagPrefix="Custom" TagName="FlatSlabControl" Src="../../UC/FlatSlab.ascx" %>
<%@ Register TagPrefix="ValTb" Namespace="CustomControls" %> 
<%@ Register Src="../../UC/DecimalBox.ascx" TagName="DecimalBox" TagPrefix="db" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <asp:ScriptManagerProxy ID="sm" runat="server" />
    <table cellpadding="0" cellspacing="0" border="1" bordercolor="black">
        <asp:HiddenField ID="hidIdValue" runat="server" EnableViewState="False" />
        <tr>
            <td class="tblHeader" colspan="4">Commission calculation</td>
        </tr>
        <tr>
            <td colspan="4" style="height: 10px;"></td>
        </tr>
        <tr>
            <td style="text-align: right; width: 21%; height: 30px;">
                <asp:Label 
                    ID="lblCalculation"
                    runat="server"
                    Font-Bold="true" 
                    Text="Calculation">
                </asp:Label>
            </td>
            <td style="width: 24%; height: 30px; text-align: right;">
            </td>
            <td style="width: 30px">
            
            </td>
        </tr>        
        <tr>
            <td style="text-align: right; width: 20%; height: 30px;">
                <asp:Label ID="lblCalcName" runat="server" Text="Name"></asp:Label>
            </td>
            <td style="width: 24%; height: 30px;">
                <asp:TextBox ID="tbCalcName" runat="server" MaxLength="40"></asp:TextBox>
            </td>
            <td style="width: 30px">
                <asp:RequiredFieldValidator 
                    ID="reqFieldVal" 
                    runat="server"
                    ControlToValidate="tbCalcName"
                    Text="*" 
                    ErrorMessage="Required field."/>
            </td>
            <td style="width: 55%; vertical-align: top;" rowspan="6">
                <table width="300" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <asp:ValidationSummary 
                                ID="valSum" 
                                runat="server" 
                                Font-Bold="True" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="text-align: right; width: 21%; height: 17px;">
                <asp:Label ID="lblBasedOn" runat="server" Text="Based On"/>
            </td>
            <td style="width: 30%; height: 17px;">
                <asp:RadioButtonList 
                    ID="rbSlabFlat"
                    OnSelectedIndexChanged="rbSlabFlat_SelectedIndexChanged" 
                    RepeatDirection="Horizontal"
                    runat="server"
                    AutoPostBack="True">
                        <asp:ListItem Value="2" Selected="True" >Slab</asp:ListItem>
                        <asp:ListItem Value="1">Flat</asp:ListItem>
                        <asp:ListItem Value="4">Size-Based</asp:ListItem>
                        <asp:ListItem Value="3">Simple</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td style="height: 17px; width: 30px;">
                 <asp:RequiredFieldValidator 
                    ID="reqSlabFlat"
                    runat="server"
                    ControlToValidate="rbSlabFlat"
                    Text="*" 
                    ErrorMessage="Choice required."/>
            </td>
        </tr>
        
        <tr>
            <td style="text-align: right; width: 21%; height: 30px;">
                <asp:Label 
                    ID="lblCommission"
                    runat="server"
                    Font-Bold="true" 
                    Text="Constraints">
                </asp:Label>
            </td>
            <td style="width: 24%; height: 30px; text-align: right;">
            </td>
            <td style="width: 30px">
            
            </td>
        </tr>
        <tr>
            <td style="text-align: right; width: 21%; height: 30px;">
                <asp:Label ID="lblMinimum" runat="server" Text="Minimum"></asp:Label></td>
            <td style="width: 24%; height: 30px;">
                <db:DecimalBox ID="dbMinimum" runat="server" DecimalPlaces="2" Width="145px" />
            </td>
            <td style="width: 30px">
                <asp:Literal ID="tbEuroSign01" runat="server">€</asp:Literal>
                <asp:RangeValidator ID="rangeValMin" runat="server"
                    Type="Double"
                    ControlToValidate="dbMinimum:tbDecimal"
                    MinimumValue="0"
                    MaximumValue="100000000000"
                    Text="*"
                    ErrorMessage="Enter a number, format: 100.000.000.000,00." />
                <%--<asp:RequiredFieldValidator ID="rfvMinimum"
                    ControlToValidate="dbMinimum:tbDecimal"
                    runat="server"
                    Text="*"
                    ErrorMessage="Minimum is mandatory" />--%>
            </td>
        </tr>
        <tr>
            <td style="text-align: right; width: 21%; height: 30px;">
                <asp:Label ID="lblMaximum" runat="server" Text="Maximum"></asp:Label></td>
            <td style="width: 24%; height: 30px;">
                <db:DecimalBox ID="dbMaximum" runat="server" DecimalPlaces="2" Width="145px" />
            <td style="width: 30px">
                <table border="0" cellpadding="0" cellspacing="0" width="30">
                    <tr>
                        <td>
                            <asp:Literal ID="tbEuroSign02" runat="server">€</asp:Literal>
                        </td>
                        <td>
                            <asp:RangeValidator ID="rangeValMax" runat="server"
                                Type="Double"
                                ControlToValidate="dbMaximum:tbDecimal"
                                Text="*"
                                MinimumValue="0"
                                MaximumValue="100000000000"
                                ErrorMessage="Enter a number, format: 100.000.000.000,00.">
                            </asp:RangeValidator>
                        </td>
                        <td style="width: 9px">
                            <asp:CompareValidator 
                                ID="cvMaximum" 
                                runat="server"
                                ControlToValidate="dbMaximum:tbDecimal"
                                ControlToCompare="dbMinimum:tbDecimal"
                                Operator="GreaterThanEqual"
                                Type="Double"
                                Text="*" 
                                ErrorMessage="Enter a greater Maximum value than the Minimum value. "/>
                            <%--<asp:RequiredFieldValidator ID="rfvMaximum"
                                ControlToValidate="dbMaximum:tbDecimal"
                                runat="server"
                                Text="*"
                                ErrorMessage="Maximum is mandatory" />--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="text-align: right; width: 21%; height: 30px;">
                <asp:Label ID="lblSetUp" runat="server" Text="Set up"></asp:Label></td>
            <td style="width: 24%">
                <db:DecimalBox ID="dbSetUp" runat="server" DecimalPlaces="2" Width="145px" />
            </td>
            <td style="width: 30px">
                <table border="0" cellpadding="0" cellspacing="0" width="30">
                    <tr>
                        <td>
                           <asp:Literal ID="tbEuroSign03" runat="server">€</asp:Literal> 
                        </td>
                        <td>
                            <asp:RangeValidator ID="rangeValSetUp" runat="server"
                                Type="Double"
                                ControlToValidate="dbSetUp:tbDecimal"
                                Text="*"
                                MinimumValue="0"
                                MaximumValue="100000000000"
                                ErrorMessage="Enter a number, format: 100.000.000.000,00.">
                            </asp:RangeValidator>
                        </td>
                        <td>
                            <asp:CustomValidator 
                                ID="custSetUp" 
                                runat="server"
                                ValidateEmptyText="true"
                                ControlToValidate="dbSetUp:tbDecimal"
                                OnServerValidate="CustSetUp"
                                Text="*" 
                                ErrorMessage="Enter either setup fee and/or Staffel fee. Or a setup fee with value 0."/>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="text-align: right; width: 21%; height: 30px;">
                <asp:Label ID="lblStaffel" runat="server" Font-Bold="True" Text="Staffel"></asp:Label>
            </td>
            <td style="width: 24%">
                <asp:Button ID="btnAddNewFlatSlabRange" runat="server" Text="+" CausesValidation="false" OnClick="btnAddNewFlatSlabRange_Click" />
                <asp:Button ID="btnDeleteFlatSlabRange" runat="server" Text="-" CausesValidation="false" OnClick="btnDeleteFlatSlabRange_Click" />
            </td>
            <td style="width: 30px"><asp:CustomValidator 
                    ID="custValRanges" 
                    runat="server"
                    Visible="true"
                    OnServerValidate="checkRanges"
                    Text="*" 
                    ErrorMessage="Ranges are not defined correctly. A range should start at an amount greater than the previous one."/>
            </td>
        </tr>
        <tr>
            <td></td>
            <td colspan="3">
                <asp:PlaceHolder ID="FlatSlabPlaceHolder" runat="server" EnableViewState="True"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <br />
    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:Button ID="btnSave" runat="server"  Text="Save" OnClick="btnSave_Click" />
    <asp:Button ID="btnCancel" CausesValidation="false" runat="server" OnClick="btnCancel_Click" Text="Cancel" /><br />
    <br />
    <br />

    </asp:Content>

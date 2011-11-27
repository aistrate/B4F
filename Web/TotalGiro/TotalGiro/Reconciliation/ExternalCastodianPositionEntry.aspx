<%@ Page Language="C#" MasterPageFile="~/EG.master" AutoEventWireup="true" CodeFile="ExternalCastodianPositionEntry.aspx.cs" 
    Inherits="ExternalCastodianPositionEntry" Title="External Castodian Position Entry" Theme="Neutral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    
        External party:
        <asp:DropDownList ID="ddlCustodians" runat="server" Width="140px" DataSourceID="dsCustName" DataValueField="Key" DataTextField="Name" >
        </asp:DropDownList>
    <asp:RequiredFieldValidator ID="ExPartVal" runat="server" ControlToValidate="ddlCustodians"
        ErrorMessage="Veld Verplicht"></asp:RequiredFieldValidator><br />
        Date: &nbsp;&nbsp; &nbsp;<asp:TextBox ID="txtDate" runat="server" Enabled="false"
            Text="<%# DateTime.Now.ToShortDateString() %>"></asp:TextBox>
        <asp:Button ID="btnCalendar" runat="server" CausesValidation="false" Height="20px"
            OnClick="btnCalendar_Click" Text="..." />&nbsp;
    <asp:RequiredFieldValidator ID="DateVal" runat="server" ControlToValidate="txtDate"
        ErrorMessage="Veld verplicht"></asp:RequiredFieldValidator><br />
    <br />
    <asp:Button ID="btnGo" runat="server" OnClick="btnGo_Click" Text="Open" /><br />
    <asp:ObjectDataSource ID="dsCustName" runat="server" SelectMethod="GetCustodians"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ExternalCustodianPositionEntryAdapter">
    </asp:ObjectDataSource>
        <br />
        <asp:Calendar ID="cldDate" runat="server" OnSelectionChanged="cldrDate_SelectionChanged"
            SelectionMode="Day" TodayDayStyle-ForeColor="blue" Visible="false">
            <TodayDayStyle ForeColor="Blue" />
        </asp:Calendar>
    &nbsp;<br />
    
    &nbsp;<asp:GridView ID="grdPositions" runat="server" DataSourceID="odsPositions" AutoGenerateColumns="False"
        Visible="False" DataKeyNames="Key" AllowPaging="True" AllowSorting="True" Caption="Quantity" CaptionAlign="Left" PageSize="20">
   
   <Columns>
     <asp:BoundField HeaderText="ISIN" DataField="Isin" SortExpression="Isin" ReadOnly="True">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
        <asp:BoundField HeaderText="Instrument" DataField="InstrumentName" SortExpression="InstrumentName" 
                    ReadOnly="True">
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
        </asp:BoundField>
           <asp:BoundField HeaderText="Quantity" DataField="Size" SortExpression="Size" DataFormatString="{0:###,###}">
                <ItemStyle wrap="False" horizontalalign="Right" Width="145px"/>
                <HeaderStyle wrap="False" horizontalalign="Right" cssclass="alignright" />
                 </asp:BoundField>
        <asp:CommandField ShowEditButton="True">
                <ItemStyle Wrap="False" Width="100px"/>
        </asp:CommandField>
   </Columns>
    </asp:GridView>
    
    <asp:ObjectDataSource ID="odsPositions" runat="server" SelectMethod="GetExtCustodianPositions" UpdateMethod="UpdateExtPosition" OnUpdating="odsPositions_Updating"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.ExternalCustodianPositionEntryAdapter"  OldValuesParameterFormatString="original_{0}" >
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlCustodians" Name="custodianID" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter ControlID="txtDate" DefaultValue="" Name="date" PropertyName="Text"
                Type="DateTime" />
        </SelectParameters>
        <UpdateParameters>
            <asp:ControlParameter ControlID="txtDate" DefaultValue="" Name="date" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="ddlCustodians" DefaultValue="0" Name="custodianID" PropertyName="SelectedValue" Type="Int32" />
        </UpdateParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
</asp:Content>

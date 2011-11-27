<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InstrumentsModelsSelector.ascx.cs" Inherits="InstrumentsModelsSelector" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>
<table cellpadding="1" cellspacing="1" border="0" >
    <tr>
        <td colspan="3" >
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
        </td>
    </tr>

    <tr>
        <td >
            <asp:RadioButtonList ID="rblExclusionType" runat="server" RepeatDirection="Horizontal" 
                OnSelectedIndexChanged="rblExclusionType_SelectedIndexChanged" AutoPostBack="true" >
                <asp:ListItem Value="0" Text="Instruments" Selected="True" />
                <asp:ListItem Value="1" Text="Sub-Models" />
            </asp:RadioButtonList>
        </td>
        <td colspan="2" />
    </tr>

    <tr>
        <td >
            <asp:Label ID="lblFilter" runat="server" Text="Filter" />
            <asp:TextBox ID="txtFilter" runat="server" />
            <asp:Button ID="btnFilter" runat="server" Text="..." OnClick="btnFilter_Click" />
        </td>
        <td colspan="2" />
    </tr>


    <tr>
        <td style="width: 450px">
            <asp:MultiView ID="mvwExclusions" runat="server" ActiveViewIndex="0" EnableTheming="True">
                <asp:View ID="vwExcludeInstruments" runat="server">
                    <asp:GridView ID="gvSelectInstrumentsToExclude" runat="server"
                    CellPadding="0"
                    DataSourceID="odsSelectInstrumentsToExclude" 
                    AllowPaging="true" PageSize="5"
                    AllowSorting="true"
                    AutoGenerateColumns="False"
                    Caption="Select Instruments to Exclude"
                    CaptionAlign="Left"
                    DataKeyNames="Key"
                    SkinID="custom-EmptyDataTemplate" 
                    Width="450px"
                    OnSelectedIndexChanging="gvExclusion_SelectedIndexChanging" >
                    <EmptyDataTemplate>
                        <table cellpadding="0" cellspacing="0" border="0" width="450px">
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                        </table>
                    </EmptyDataTemplate>                    
                    <Columns>
                        <asp:BoundField DataField="Isin" HeaderText="Isin" SortExpression="Isin">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DisplayName" HeaderText="Name" SortExpression="DisplayName">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:CommandField ShowSelectButton="true" />
                    </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsSelectInstrumentsToExclude" runat="server" SelectMethod="GetInstruments" OnSelecting="odsSelectInstrumentsToExclude_Selecting"
                        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter" >
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtFilter" Name="filter" PropertyName="Text" Type="String" />
                            <asp:Parameter Name="propertyList" DefaultValue="Key, DisplayName, Isin" Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:View>
                <asp:View ID="vwExcludeSubModels" runat="server">
                    <asp:GridView ID="gvSelectModelsToExclude" runat="server"
                    CellPadding="0"
                    DataSourceID="odsSelectModelsToExclude" 
                    AllowPaging="true" PageSize="5"
                    AllowSorting="true"
                    AutoGenerateColumns="False"
                    Caption="Select Models to Exclude"
                    CaptionAlign="Left"
                    DataKeyNames="Key"
                    SkinID="custom-EmptyDataTemplate" 
                    Width="450px"
                    OnSelectedIndexChanging="gvExclusion_SelectedIndexChanging" >
                    <EmptyDataTemplate>
                        <table cellpadding="0" cellspacing="0" border="0" width="450px">
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                        </table>
                    </EmptyDataTemplate>                    
                    <Columns>
                        <asp:BoundField DataField="ModelName" HeaderText="Name" SortExpression="ModelName">
                            <HeaderStyle wrap="False" />
                            <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                        </asp:BoundField>
                        <asp:CommandField ShowSelectButton="true" />
                    </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsSelectModelsToExclude" runat="server" SelectMethod="GetModels" OnSelecting="odsSelectModelsToExclude_Selecting"
                        TypeName="B4F.TotalGiro.ApplicationLayer.Instructions.InstructionEntryAdapter" >
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtFilter" Name="filter" PropertyName="Text" Type="String" />
                            <asp:Parameter Name="propertyList" DefaultValue="Key, ModelName" Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:View>
            </asp:MultiView>
        </td>
        <td style="width: 40px"> 
            <asp:Label ID="lblMoveTo" runat="server" Text="<->" />
        </td>
        <td style="width: 350px; vertical-align: top">
            <asp:GridView ID="gvExclusions" runat="server"
            CellPadding="0"
            AutoGenerateColumns="False"
            AllowPaging="true" PageSize="5"
            AllowSorting="true"
            Caption="Exclusions"
            CaptionAlign="Left"
            SkinID="custom-EmptyDataTemplate" 
            Width="350px"
            DataKeyNames="Key"
            OnRowDeleting="gvExclusions_RowDeleting"
            OnSorting="gvExclusions_Sorting"
            OnPageIndexChanging="gvExclusions_PageIndexChanging" >
            <EmptyDataTemplate>
                <table cellpadding="0" cellspacing="0" border="0" width="350px">
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                </table>
            </EmptyDataTemplate>                    
            <Columns>
                <asp:BoundField DataField="ComponentType" HeaderText="Type" SortExpression="ComponentType">
                    <HeaderStyle wrap="False" />
                    <ItemStyle cssclass="bigrightpadding" horizontalalign="Left" wrap="False" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Name" SortExpression="ComponentName">
                    <ItemStyle wrap="False" />
                    <ItemTemplate>
                        <trunc:TruncLabel ID="lblMessage"
                            runat="server"
                            Width="38"
                            Text='<%# DataBinder.Eval(Container.DataItem, "ComponentName") %>' 
                            />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowDeleteButton="true" />
            </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>

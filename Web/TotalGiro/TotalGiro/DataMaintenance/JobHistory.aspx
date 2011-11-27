<%@ Page Language="C#" MasterPageFile="~/EG.master" CodeFile="JobHistory.aspx.cs" Theme="Neutral" Inherits="DataMaintenance_JobHistory" 
    Title="Job History Page" %>
<%@ Register Src="../UC/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Import Namespace="B4F.TotalGiro.Jobs" %>
<%@ Register TagPrefix="trunc" Namespace="Trunc"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">

   <table border="0">
    <tr>
        <td align="left">Search Job Name:&nbsp;
            <br /><asp:TextBox ID="txtJob" runat="server" AutoPostBack="True" /></td>
        <td align="left">Search Job Component:&nbsp;
            <br /><asp:TextBox ID="txtComponent" runat="server" AutoPostBack="True" /></td>
    </tr>
    <tr>
        <td align="left">Show history from this date:&nbsp;
            <br/><uc1:DatePicker ID="dpDateFrom" runat="server" /></td>
        <td align="left">Until(incl.) this date:&nbsp;
            <br/><uc1:DatePicker ID="dpDateTo" runat="server" /></td>
    </tr>
   </table>
    <br />
   
    <asp:GridView 
        ID="gvJobHistory"
        SkinID="spreadsheet" 
        runat="server" 
        AutoGenerateColumns="False" 
        DataSourceID="odsJobHistory"
        CssClass="padding"
        AllowPaging="True" 
        Caption="Job History Details" 
        CaptionAlign="Left"
        AllowSorting="True" 
        DataKeyNames="Key"
        OnRowCommand="gvJobHistory_OnRowCommand"
        PageSize="15">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Key" SortExpression="Key" ReadOnly="True" >
                <ItemStyle Wrap="False" />
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="Job" HeaderText="Job Name" SortExpression="Job" >
                <ItemStyle Wrap="False"/>
                <HeaderStyle Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="JobComponent" SortExpression="JobComponent">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel id="trlJobComponent" 
                        runat="server"
                        width="20"
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "JobComponent") %>' 
                        text='<%# DataBinder.Eval(Container.DataItem, "JobComponent") %>' 
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StartTime" HeaderText="StartTime" SortExpression="StartTime" DataFormatString="{0:dd MMM yy hh:mm}" HtmlEncode="False">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="EndTime" HeaderText="EndTime" SortExpression="EndTime" DataFormatString="{0:dd MMM yy hh:mm}" HtmlEncode="False" ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Status" SortExpression="Status"  >
                <ItemStyle wrap="False" />
                <ItemTemplate>
                    <%# (WorkerResultStatus)DataBinder.Eval(Container.DataItem, "Status")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Details" SortExpression="Details">
                <ItemStyle wrap="False" />
                <HeaderStyle wrap="False" />
                <ItemTemplate>
                    <trunc:TruncLabel id="trlDetails" 
                        runat="server"
                        width="40"
                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "Details") %>' 
                        text='<%# DataBinder.Eval(Container.DataItem, "Details") %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CreationDate" HeaderText="Created" SortExpression="CreationDate"  DataFormatString="{0:dd MMM yy hh:mm}" HtmlEncode="False" ReadOnly="True">
                <ItemStyle HorizontalAlign="Right" Wrap="False" />
                <HeaderStyle HorizontalAlign="Right" CssClass="alignright" Wrap="False" />
            </asp:BoundField>
            <asp:CommandField SelectText="View" ShowSelectButton="True" >
                <ItemStyle wrap="False" />
            </asp:CommandField>
        </Columns>
        <SelectedRowStyle BackColor="Gainsboro" />
    </asp:GridView>
    <asp:ObjectDataSource ID="odsJobHistory" runat="server" SelectMethod="GetJobHistoryDetails"
        TypeName="B4F.TotalGiro.ApplicationLayer.DataMaintenance.JobHistoryAdapter">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtJob" Name="jobName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtComponent" Name="componentName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="dpDateFrom" Name="startDate" PropertyName="SelectedDate" Type="DateTime" />
            <asp:ControlParameter ControlID="dpDateTo" Name="endDate" PropertyName="SelectedDate" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="lblDetails" runat="server"></asp:Label>
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
    <br />
</asp:Content>


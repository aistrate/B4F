<%@ Page Language="C#" Theme="Neutral" MasterPageFile="~/EG.master" AutoEventWireup="true" 
CodeFile="ImportFundPrices.aspx.cs" Inherits="DataMaintenance_ImportFundPrices" Title="Import Fund Prices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" Runat="Server">
    <div>
        <div id="divStart" style="width: 600px; height: 320px" runat="server">
          <asp:Calendar ID="calPrice" runat="server" OnInit="calPrice_Init"></asp:Calendar><br />
          <asp:Label ID="lblFile" runat="server" Text="File :" Width="50px"></asp:Label><asp:FileUpload ID="ctrUpload" runat="server" /><br /><br /> 
          <asp:Button ID="cmdNext" runat="server" Text="Next" OnClick="cmdNext_Click" Width="100px" />
          <asp:Button ID="cmdCancel" runat="server" OnClick="cmdCancel_Click" Text="Cancel" Visible="False" Width="100px" /></div><br />
          <asp:Label ID="lblErrorFundPrices" runat="server" Text="" Visible="False" Font-Bold="True" Font-Size="Larger" ForeColor="Red"></asp:Label><br /><br /> 
          <asp:Label ID="lblDat" runat="server" Text="Label" Visible="False" Font-Bold="True"></asp:Label><br />
        
        <div id="divRes" runat="server" visible="false" style="width: 597px; height: 550px; overflow-x: hidden; overflow-y: scroll; border-width:1px; border-color:Black; border-style:solid"  >
            <asp:DataGrid ID="grdList" runat="server" AutoGenerateColumns="False" Width="580px" 
                 OnItemDataBound="grdList_ItemDataBound">
                <Columns>
                    <asp:BoundColumn HeaderText="&lt;B&gt;Skip&lt;/B&gt;" DataField="Skip" />
                    
                    <asp:BoundColumn HeaderText="&lt;B&gt;Instrument&lt;/B&gt;" DataField="Instrument" />
                    <asp:BoundColumn HeaderText="&lt;B&gt;ISIN&lt;/B&gt;" DataField="ISIN"  />
                    <asp:BoundColumn HeaderText="&lt;B&gt;Price&lt;/B&gt;" DataField="Price" HeaderStyle-HorizontalAlign="Right">
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundColumn>
                    <asp:BoundColumn HeaderText="&lt;B&gt;PrevPrice&lt;/B&gt;" DataField="PrevPrice" HeaderStyle-HorizontalAlign="Right">
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundColumn>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <asp:Image ID="imgWarning" runat="server" 
                            ImageUrl="~/layout/images/images_ComponentArt/pager/priority_high.gif" 
                            Visible='<%# (DataBinder.Eval(Container.DataItem, "Message") == System.DBNull.Value ? false : true) %>'
                            ToolTip ='<%# DataBinder.Eval(Container.DataItem, "Message") %>'
                            />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateColumn>
                </Columns>
                <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" />
            </asp:DataGrid>
        </div><br />
        <asp:Button ID="cmdImport" runat="server" OnClick="cmdImport_Click" Text="Import" Visible="False" Width="100px" />
        <asp:Button ID="cmdAbort" runat="server" OnClick="cmdCancel_Click" Text="Cancel" Visible="False" Width="100px" /><br /><br />
    </div>
     
</asp:Content>
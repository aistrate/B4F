<%@ Page Language="C#" MasterPageFile="~/TotalGiroClient.master" 
         Inherits="B4F.TotalGiro.Client.Web.Charts.Charts" Codebehind="Charts.aspx.cs" %>

<%@ Register Src="~/UC/PortfolioNavigationBar.ascx" TagName="PortfolioNavigationBar" TagPrefix="b4f" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="Server">
    
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1" />
    
    <%-- This is redundant, but needs to be here in order to override the default stylesheet of the Tabs control --%>
    <link href="../App_Themes/Neutral/Neutral.css" type="text/css" rel="stylesheet"/>
    <link href="../App_Themes/Print/Print.css" type="text/css" rel="stylesheet" media="print"/>
    
    <style type="text/css">
        .ajax__tab_xp .ajax__tab_tab { width: 180px; }
    </style>
    
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td style="width: 600px; height: 27px; white-space: nowrap">
                <asp:Panel ID="pnlPortfolioNavigationBar" runat="server">
                    <b4f:PortfolioNavigationBar ID="ctlPortfolioNavigationBar" runat="server" ShowCharts="false" />
                </asp:Panel>
            </td>
            <td rowspan="2" style="width: 245px; white-space: nowrap; text-align: right; vertical-align: middle">
                <asp:UpdateProgress ID="upSpinner" runat="server" DisplayAfter="250">
                    <ProgressTemplate>
                        <asp:Image ID="imgWheel" runat="server" ImageUrl="~/Images/Wheel.gif" CssClass="plain" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
            <td rowspan="2" style="width: 15px">
            </td>
        </tr>
        <tr>
            <td style="height: 5px">
            </td>
        </tr>
    </table>
    
    <asp:UpdatePanel ID="updTabs" runat="server">
        <ContentTemplate>
            <b4f:ErrorLabel ID="elbErrorMessage" runat="server"></b4f:ErrorLabel>
            <div style="height: 7px"></div>
            
            <ajaxtoolkit:TabContainer runat="server" ID="tbcChartType" AutoPostBack="true" ActiveTabIndex="0" 
                                      Width="860px" OnActiveTabChanged="tbcChartType_ActiveTabChanged"
                                      CssClass="ajax__tab_xp">
                
                <ajaxToolkit:TabPanel runat="server" ID="tbpAccountValuations" HeaderText="Portefeuille">
                    <ContentTemplate>

                        <div style="min-height: 532px">
                            <asp:Panel ID="pnlAccountValuations" runat="server" Visible="false">
                            
                                <asp:CheckBoxList ID="cblAccounts" runat="server" DataSourceID="odsAccounts" Height="22px"
                                    DataTextField="DisplayNumberWithName" DataValueField="Key" AutoPostBack="True"
                                    RepeatDirection="Horizontal" RepeatColumns="3" 
                                    OnDataBound="cblAccounts_DataBound"
                                    OnSelectedIndexChanged="cblAccounts_SelectedIndexChanged">
                                </asp:CheckBoxList>
                                <asp:ObjectDataSource ID="odsAccounts" runat="server" SelectMethod="GetContactAccounts"
                                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Common.CommonAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="contactId" SessionField="ContactId" Type="Int32" DefaultValue="0" /> 
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                
                                <div style="height: 510px">
                                    <dundas:Chart ID="chAccountValuations" runat="server" BorderLineColor="26, 59, 105"
                                        Palette="DundasDark" Width="850px" Height="500px" AntiAliasing="Graphics" BorderLineWidth="0"
                                        EnableViewState="false" ViewStateContent="All">
                                        <Legend Alignment="Far" AutoFitText="False" BackColor="#F8FCFF" BorderColor="26, 59, 105"
                                                Docking="Top" EquallySpacedItems="True" LegendStyle="Row" Font="Trebuchet MS, 12px"
                                                Name="Default" ShadowOffset="2" TableStyle="Wide" FontColor="#313457">
                                        </Legend>
                                        <BorderSkin FrameBackGradientEndColor="CornflowerBlue" PageColor="Window" FrameBorderColor="100, 0, 0, 0"
                                            FrameBorderWidth="2" FrameBackColor="CornflowerBlue"></BorderSkin>
                                        <ChartAreas>
                                            <dundas:ChartArea BorderColor="26, 59, 105" BackColor="White" BorderStyle="Solid" ShadowOffset="2"
                                                Name="Default">
                                                <AxisX Margin="False" LabelsAutoFitMaxFontSize="9" LabelsAutoFitMinFontSize="7">
                                                    <MinorGrid LineColor="Silver"></MinorGrid>
                                                    <LabelStyle Format="MMM yy" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                                Font="Trebuchet MS, 12px" FontColor="#313457" IntervalType="Auto"></LabelStyle>
                                                    <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                                        IntervalType="Auto"></MajorGrid>
                                                    <MinorTickMark Size="2"></MinorTickMark>
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisX>
                                                <Area3DStyle WallWidth="10" Light="Realistic" RightAngleAxes="False"></Area3DStyle>
                                                <AxisY LabelsAutoFitMinFontSize="7" LabelsAutoFitMaxFontSize="9">
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
                                    </dundas:Chart>
                                </div>

                            </asp:Panel>
                        </div>
                        
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                
                <ajaxToolkit:TabPanel runat="server" ID="tbpPositionValuations" HeaderText="Posities" Height="510px">
                    <ContentTemplate>
                    
                        <div style="min-height: 532px">
                            <asp:Panel ID="pnlPositionValuations" runat="server" Visible="false">
                            
                                <asp:RadioButtonList ID="rblPositionAccounts" runat="server" AutoPostBack="True" Height="22px"
                                    DataSourceID="odsPositionAccounts" DataTextField="DisplayNumberWithName" DataValueField="Key"
                                    RepeatDirection="Horizontal" RepeatColumns="3"
                                    OnSelectedIndexChanged="rblPositionAccounts_SelectedIndexChanged">
                                </asp:RadioButtonList>
                                <asp:ObjectDataSource ID="odsPositionAccounts" runat="server" SelectMethod="GetContactAccounts"
                                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Common.CommonAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="contactId" SessionField="ContactId" Type="Int32" DefaultValue="0" /> 
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                
                                <div style="height: 8px"></div>
                                <asp:CheckBoxList ID="cblPositionInstruments" runat="server" DataSourceID="odsPositionInstruments"
                                    DataTextField="InstrumentDescription" DataValueField="InstrumentId" AutoPostBack="True"
                                    OnDataBound="cblPositionInstruments_DataBound" 
                                    OnSelectedIndexChanged="cblPositionInstruments_SelectedIndexChanged"
                                    RepeatDirection="Vertical" RepeatColumns="2">
                                </asp:CheckBoxList>
                                <asp:ObjectDataSource ID="odsPositionInstruments" runat="server" SelectMethod="GetAccountPositions"
                                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Charts.ChartsAdapter">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="rblPositionAccounts" Name="accountId" PropertyName="SelectedValue" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                
                                <div style="height: 510px">
                                    <dundas:Chart ID="chPositionValuations" runat="server" BorderLineColor="26, 59, 105"
                                        Palette="DundasDark" Width="850px" Height="500px" AntiAliasing="Graphics" BorderLineWidth="0"
                                        EnableViewState="false" ViewStateContent="All">
                                        <Legend Alignment="Far" BackColor="#F8FCFF" BorderColor="26, 59, 105" Docking="Top" LegendStyle="Table"
                                                EquallySpacedItems="true" Font="Trebuchet MS, 12px" Name="Default" ShadowOffset="2"
                                                TextWrapThreshold="0" TableStyle="Wide" FontColor="#313457" >
                                            <Position Height="12" Width="67" X="30" Y="3" />
                                        </Legend>
                                        <BorderSkin FrameBackColor="CornflowerBlue" FrameBackGradientEndColor="CornflowerBlue"
                                            FrameBorderColor="100, 0, 0, 0" FrameBorderWidth="2" PageColor="Window" />
                                        <ChartAreas>
                                            <dundas:ChartArea BorderColor="26, 59, 105" BackColor="White" BorderStyle="Solid" ShadowOffset="2"
                                                Name="Default">
                                                <AxisX Margin="False" LabelsAutoFitMaxFontSize="9" LabelsAutoFitMinFontSize="7">
                                                    <MinorGrid LineColor="Silver" />
                                                    <LabelStyle Format="MMM yy" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                                Font="Trebuchet MS, 12px" FontColor="#313457" IntervalType="Auto" />
                                                    <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                                        LineColor="Silver" />
                                                    <MinorTickMark Size="2" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisX>
                                                <Area3DStyle Light="Realistic" RightAngleAxes="False" WallWidth="10" />
                                                <AxisY LabelsAutoFitMinFontSize="7" LabelsAutoFitMaxFontSize="9">
                                                    <MinorGrid LineColor="Silver" />
                                                    <LabelStyle Format="C0" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                                Font="Trebuchet MS, 12px" FontColor="#313457" IntervalType="Auto" />
                                                    <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                                        LineColor="Silver" />
                                                    <MinorTickMark Size="2" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisY>
                                                <AxisX2>
                                                    <MinorGrid LineColor="Silver" />
                                                    <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                                        LineColor="Silver" />
                                                    <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisX2>
                                                <AxisY2>
                                                    <MinorGrid LineColor="Silver" />
                                                    <MajorGrid Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto"
                                                        LineColor="Silver" />
                                                    <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisY2>
                                                <Position Height="83" Width="94" X="3" Y="17" />
                                            </dundas:ChartArea>
                                        </ChartAreas>
                                    </dundas:Chart>
                                </div>
                            
                            </asp:Panel>
                        </div>
                        
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                
                <ajaxToolkit:TabPanel runat="server" ID="tbpAllocation" HeaderText="Verdeling beleggingen">
                    <ContentTemplate>
                    
                        <div style="min-height: 532px">
                            <asp:Panel ID="pnlAllocation" runat="server" Visible="false">
                            
                                <asp:RadioButtonList ID="rblAllocationAccounts" runat="server" AutoPostBack="True" Height="22px"
                                    DataSourceID="odsAllocationAccounts" DataTextField="DisplayNumberWithName" DataValueField="Key"
                                    RepeatDirection="Horizontal" RepeatColumns="3"
                                    OnSelectedIndexChanged="rblAllocationAccounts_SelectedIndexChanged">
                                </asp:RadioButtonList>
                                <asp:ObjectDataSource ID="odsAllocationAccounts" runat="server" SelectMethod="GetContactAccounts"
                                    TypeName="B4F.TotalGiro.ClientApplicationLayer.Common.CommonAdapter">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="contactId" SessionField="ContactId" Type="Int32" DefaultValue="0" /> 
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                
                                <div style="height: 510px">
                                    <dundas:Chart ID="chAllocation" runat="server" AntiAliasing="Graphics" BorderLineWidth="0"
                                        BackGradientEndColor="White" BackGradientType="DiagonalLeft" BorderLineColor="26, 59, 105"
                                        DataSourceID="odsAllocation" Height="500px" Palette="DundasDark" Width="850px"
                                        EnableViewState="false" ViewStateContent="All">
                                        <Legend AutoFitText="true" BackColor="#F8FCFF" BorderColor="26, 59, 105" Font="Trebuchet MS, 12px" 
                                                Alignment="Far" Docking="Top" DockInsideChartArea="false"
                                                LegendStyle="Column" Name="Default" ShadowOffset="2" TableStyle="Wide" FontColor="#313457" TextWrapThreshold="0">
                                            <CellColumns>
                                                <dundas:LegendCellColumn Alignment="TopLeft" ColumnType="SeriesSymbol" Name="Color">
                                                    <Margins Left="15" Right="15" />
                                                </dundas:LegendCellColumn>
                                                <dundas:LegendCellColumn Alignment="TopLeft" Name="Instrument" Text="#LEGENDTEXT">
                                                    <Margins Left="15" Right="15" />
                                                </dundas:LegendCellColumn>
                                                <dundas:LegendCellColumn Alignment="TopRight" Name="Percentage" Text="#PERCENT{P2}">
                                                    <Margins Left="15" Right="15" />
                                                </dundas:LegendCellColumn>
                                                <dundas:LegendCellColumn Alignment="TopRight" Name="Value" Text="#VAL{C2}">
                                                    <Margins Left="15" Right="15" />
                                                </dundas:LegendCellColumn>
                                            </CellColumns>
                                        </Legend>
                                        <Series>
                                            <dundas:Series BorderColor="26, 59, 105" ChartType="Pie" 
                                                CustomAttributes="PieLabelStyle=Outside, PieDrawingStyle=Concave, CollectedSliceExploded=False, 
                                                                  CollectedThresholdUsePercent=True, CollectedThreshold=2, CollectedLabel=Overigens, 
                                                                  CollectedLegendText=Overigens"
                                                Font="Trebuchet MS, 12px" Name="PositionValues" ShadowOffset="2" ValueMemberX="InstrumentName"
                                                ValueMembersY="CurrentBaseValueQuantity" YValuesPerPoint="2" BackGradientEndColor="120, 190, 220"
                                                Label="#VALX" FontColor="#313457">
                                            </dundas:Series>
                                        </Series>
                                        <BorderSkin FrameBackColor="CornflowerBlue" FrameBackGradientEndColor="CornflowerBlue"
                                            PageColor="Window" />
                                        <ChartAreas>
                                            <dundas:ChartArea BackColor="Transparent" BorderColor="Transparent" BorderStyle="Solid" Name="Default">
                                                <AxisX>
                                                    <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                                        IntervalType="Auto" />
                                                    <MinorGrid LineColor="Silver" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                        IntervalType="Auto" />
                                                    <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisX>
                                                <Area3DStyle XAngle="60" YAngle="15" />
                                                <AxisY>
                                                    <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                                        IntervalType="Auto" />
                                                    <MinorGrid LineColor="Silver" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                        IntervalType="Auto" />
                                                    <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisY>
                                                <AxisX2>
                                                    <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                                        IntervalType="Auto" />
                                                    <MinorGrid LineColor="Silver" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                        IntervalType="Auto" />
                                                    <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisX2>
                                                <AxisY2>
                                                    <MajorGrid LineColor="Silver" Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto"
                                                        IntervalType="Auto" />
                                                    <MinorGrid LineColor="Silver" />
                                                    <MajorTickMark Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" 
                                                        IntervalType="Auto" />
                                                    <LabelStyle Interval="Auto" IntervalOffset="Auto" IntervalOffsetType="Auto" IntervalType="Auto" />
                                                </AxisY2>
                                                <Position Height="80" Width="100" X="0" Y="20" />
                                                <InnerPlotPosition Height="70" Width="100" X="0" Y="15" />
                                            </dundas:ChartArea>
                                        </ChartAreas>
                                    </dundas:Chart>
                                    <asp:ObjectDataSource ID="odsAllocation" runat="server" SelectMethod="GetAllocationByInstrument"
                                        TypeName="B4F.TotalGiro.ClientApplicationLayer.Charts.ChartsAdapter" OnSelecting="odsAllocation_Selecting">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="rblAllocationAccounts" Name="accountId" PropertyName="SelectedValue"
                                                Type="Int32" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </div>
                    
                            </asp:Panel>
                        </div>
                    
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            
            </ajaxtoolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>

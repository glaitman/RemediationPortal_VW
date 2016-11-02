<%@ Page Title="Analytical Chart" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AnalyticalCharts.aspx.cs" Inherits="AnalyticalCharts" 
    EnableEventValidation="false" StylesheetTheme="Default" MaintainScrollPositionOnPostback="true"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <ajax:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajax:ToolkitScriptManager>
  
    <asp:UpdatePanel ID="uCharts" runat="server" RenderMode="Block" UpdateMode="Always" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div style="margin-bottom:800px;">
                <asp:FormView 
                    ID="fvChart" 
                    runat="server"
                    AllowPaging="true"       
                    PagerSettings-Visible="false"                
                    OnPageIndexChanging="fvChart_PageIndexChanging"
                    OnDataBound="fvChart_DataBound">
                    <EmptyDataTemplate>
                        <div class="chartparameters_hdr">
                            <asp:Image runat="server" ImageUrl="~/images/settings_icon.png" ImageAlign="Left" /> 
                            <asp:LinkButton ID="lbParameters" runat="server" Text="Chart Parameters" 
                                OnClick="lbParameters_Click" />
                        </div>
                    </EmptyDataTemplate>
                    <EmptyDataRowStyle
                        BackColor="#dcdbd8"
                        ForeColor="#333333"
                        BorderStyle="Solid"
                        BorderColor="#B5C7DE"
                        BorderWidth="1px"
                        VerticalAlign="Middle" />
                    <ItemTemplate>
                        <asp:Chart 
                            ID="Chart1" 
                            runat="server"                            
                            SkinID="Chart"                         
                            OnCustomize="Chart_Customize" 
                            OnCustomizeLegend="Chart_CustomizeLegend">
                            <Series></Series>
                            <ChartAreas></ChartAreas>
                            <Legends></Legends>
                            <Titles></Titles>
                        </asp:Chart>
                    </ItemTemplate>
                    <FooterTemplate>
                        <table width="100%">
                            <tr>
                                <td style="width:20%;">
                                    <div class="chartparameters_hdr">
                                        <asp:Image 
                                            ID="Image1" 
                                            runat="server" 
                                            ImageUrl="~/images/settings_icon.png" 
                                            ImageAlign="Left" /> 
                                        <asp:LinkButton 
                                            ID="lbParameters" 
                                            runat="server" 
                                            Text="Chart Parameters" 
                                            OnClick="lbParameters_Click" />
                                    </div>                                
                                </td>
                                <td style="width:25%; padding-left:1em;">
                                    <asp:LinkButton
                                        ID="lbFirst"
                                        runat="server"
                                        CommandName="Page"
                                        CommandArgument="First"
                                        Text="&lt;&lt;"
                                        ToolTip="First" />
                                    <asp:LinkButton
                                        ID="lbPrev"
                                        runat="server"
                                        CommandName="Page"
                                        CommandArgument="Prev"
                                        Text="Previous"
                                        ToolTip="Previous" />
                                        <span>|</span>
                                    <asp:LinkButton
                                        ID="lbNext"
                                        runat="server"
                                        CommandName="Page"
                                        CommandArgument="Next"
                                        Text="Next"
                                        ToolTip="Next" />
                                    <asp:LinkButton
                                        ID="lbLast"
                                        runat="server"
                                        CommandName="Page"
                                        CommandArgument="Last"
                                        Text="&gt;&gt;"
                                        ToolTip="Last" />
                                    <span>Page</span>
                                    <asp:Label 
                                        ID="lblPageNumber"
                                        runat="server" />
                                    <span>of</span>
                                    <asp:Label
                                        ID="lblTotal"
                                        runat="server" />
                                </td>
                                <td style="width:40%; text-align:center;">
                                    <span style="font-weight:bold;">Tide Prediction:</span>
                                    <asp:CheckBox
                                        ID="chkTideY1"
                                        runat="server"
                                        Text="Y1-Axis"
                                        TextAlign="Right"
                                        AutoPostBack="true" 
                                        OnCheckedChanged="chk_CheckedChanged"
                                        OnPreRender="chk_PreRender" />
                                    <asp:CheckBox
                                        ID="chkTideY2"
                                        runat="server"
                                        Text="Y2-Axis"
                                        TextAlign="Right"
                                        AutoPostBack="true" 
                                        OnCheckedChanged="chk_CheckedChanged"
                                        OnPreRender="chk_PreRender" />
                                </td>
                                <td style="width:15%; text-align:right; padding-right:1em;">
                                    <asp:ImageButton
                                        ID="ibExport"
                                        runat="server"
                                        ImageUrl="~/images/icon_pdf.gif"
                                        ToolTip="Export all charts to PDF"
                                        CommandName="Export"
                                        CommandArgument="ChartExport.aspx"
                                        OnCommand="ib_Command"
                                        OnPreRender="ibExport_AddTrigger" />
                                    <asp:LinkButton
                                        ID="lbExport"
                                        runat="server"
                                        Text="Export Charts"
                                        ToolTip="Export all charts to PDF"
                                        CommandName="Export"
                                        CommandArgument="ChartExport.aspx" 
                                        OnCommand="ib_Command"
                                        OnPreRender="lbExport_AddTrigger" />
                                </td>
                            </tr>
                        </table>
                    </FooterTemplate>
                    <FooterStyle 
                        BackColor="#dcdbd8"
                        ForeColor="#333333"
                        BorderStyle="Solid"
                        BorderColor="#B5C7DE"
                        BorderWidth="1px" />
                </asp:FormView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnMpe" runat="server" style="display:none;" />
    <div id="pnlParameters" class="chartparameters_menu">
        <div class="alignright">              
            <input type="image" src="images/icon_close_button.gif" id="btnClose" />
        </div>
        <asp:Panel ID="pnlErrors" runat="server" Visible="false">
            <span class="bold" style="color:Red;">Please correct the following:</span>
            <asp:BulletedList 
                ID="blErrors" 
                runat="server"
                ForeColor="Red" 
                BulletStyle="Disc"
                DisplayMode="Text">
            </asp:BulletedList>
            <br />
            <br />
        </asp:Panel>
        <asp:Panel ID="pnlWells" runat="server" GroupingText="Select Well" CssClass="pnl_wells">
            <asp:CheckBoxList ID="chkWellID" runat="server" DataSourceID="sdsWellID" DataValueField="WellID"
                DataTextField="WellInfo" RepeatColumns="5">
            </asp:CheckBoxList>        
        </asp:Panel>
        <asp:CheckBox ID="chkSelectAll" runat="server" Text="Check All" TextAlign="Right" AutoPostBack="true"
            ToolTip="Select all wells" CssClass="checkall" OnCheckedChanged="chkSelectAll_CheckedChanged" />
        <br />
        <br />
        <table>
            <tr>
                <th colspan="2" class="header">Start Date/Time Parameters</th>
                <th class="separator"></th>
                <th colspan="2" class="header">End Date/Time Parameters</th>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblStartDt" runat="server" AssociatedControlID="cbStartDt" Text="Date:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <ajax:ComboBox ID="cbStartDt" runat="server" DataSourceID="sdsDate" DataTextField="LogDate" DropDownStyle="DropDownList"
                        DataValueField="LogDate" AppendDataBoundItems="true" AutoCompleteMode="Suggest" DataTextFormatString="{0:MM/dd/yyyy}"
                        CssClass="comboBoxInsideModalPopup">
                        <asp:ListItem Value="">Select start date...</asp:ListItem>
                    </ajax:ComboBox>                
                </td>

                <td class="separator"></td>

                <td>
                    <asp:Label ID="lblEndDt" runat="server" AssociatedControlID="cbEndDt" Text="Date:" Font-Bold="true"></asp:Label>
                </td>
                <td>     
                    <ajax:ComboBox ID="cbEndDt" runat="server" DataSourceID="sdsDate" DataTextField="LogDate" DropDownStyle="DropDownList"
                        DataValueField="LogDate" AppendDataBoundItems="true" AutoCompleteMode="Suggest" DataTextFormatString="{0:MM/dd/yyyy}"
                        CssClass="comboBoxInsideModalPopup">
                        <asp:ListItem Value="">Select end date...</asp:ListItem>    
                    </ajax:ComboBox> 
                </td>
            </tr>
            <tr>
                <td colspan="2" class="bold" style="padding-top:18px;">            
                    <asp:Label ID="lblStarTime" runat="server" Text="Time"></asp:Label>
                </td>

                <td class="separator"></td>

                <td colspan="2" class="bold" style="padding-top:18px;">            
                    <asp:Label ID="lblEndTime" runat="server" Text="Time"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Hours:</td>
                <td>
                    <asp:TextBox runat="server" ID="txtSHours" MaxLength="2" Width="30px" />
                </td> 

                <td class="separator"></td>

                <td>Hours:</td>
                <td>
                    <asp:TextBox runat="server" ID="txtEHours" MaxLength="2" Width="30px" />
                </td>
            </tr>
            <tr>
                <td>Minutes:</td>
                <td>                
                    <asp:TextBox runat="server" ID="txtSMinutes" MaxLength="2" Width="30px" />
                </td>

                <td class="separator"></td>

                <td>Minutes:</td>
                <td>
                    <asp:TextBox runat="server" ID="txtEMinutes" MaxLength="2" Width="30px" />
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table title="Measurement Category" summary="Select up to two measurement categories to generate your chart." width="100%"> 
            <tr>
                <th colspan="2"><span style="text-decoration:underline;">Measurement Category</span></th>
            </tr>  
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCategory" AssociatedControlID="ddlCategory" Text="Axis Y1:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCategory" runat="server" AppendDataBoundItems="true" DataSourceID="sdsCategory" 
                        DataValueField="ListKey" DataTextField="ListValue" Width="100%">
                        <asp:ListItem Value="">Select a category...</asp:ListItem> 
                    </asp:DropDownList>
                </td>
                <td rowspan="2" style="width:60%;">
                    <div style="padding-left:100px;">
                        <asp:RadioButtonList ID="rblGraphType" runat="server">
                            <asp:ListItem Value="0" Selected="True">1 Graph/Well</asp:ListItem>
                            <asp:ListItem Value="1">All Wells on 1 Graph</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </td>
            </tr>   
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCategory2" AssociatedControlID="ddlCategory2" Text="Axis Y2:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCategory2" runat="server" AppendDataBoundItems="true" DataSourceID="sdsCategory" 
                        DataValueField="ListKey" DataTextField="ListValue" Width="100%">
                        <asp:ListItem Value="">Select a category...</asp:ListItem> 
                    </asp:DropDownList>
                </td>
            </tr>     
        </table>
        <br />
        <div id="divSubmit" class="pnl_wells" style="text-align:center;">
            <asp:Button ID="btnUpdate" runat="server" Text="Submit" 
                onclick="btnUpdate_Click" />            
        </div>
    </div>

    <ajax:ModalPopupExtender 
        ID="mpeChartParameters"
        runat="server"
        PopupControlID="pnlParameters"
        TargetControlID="btnMpe"
        CancelControlID="btnClose"        
        BackgroundCssClass="modalbg">
    </ajax:ModalPopupExtender>

    <asp:SqlDataSource ID="sdsWellID" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
            SELECT DISTINCT
	            WellID,
	            'WellInfo' = CAST((WellID - 31) as VarChar) + ' ' + WellLabel
            FROM VancouverLogData
            ORDER BY WellID
        "></asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsDate" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
            SELECT DISTINCT
	            [LogDate] = CONVERT(Date, LogDate)
            FROM VancouverLogData
            ORDER BY LogDate
        "></asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsTime" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
        SELECT DISTINCT
	        [LogTime] = CONVERT(VarChar, LogDate, 108)
        FROM VancouverLogData
        ORDER BY LogTime
        "></asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsCategory" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
            SELECT DISTINCT
                ListKey,
                ListValue
            FROM ListManager
            WHERE Category = @Category
            AND IsNULL(NotVisible, 0) = 0 
            ORDER BY ListValue
        ">
        <SelectParameters>
            <asp:Parameter Name="Category" DefaultValue="Measurement" />
        </SelectParameters>    
    </asp:SqlDataSource>
</asp:Content>


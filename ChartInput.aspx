<%@ Page Title="Analytical Charts | Input Data" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ChartInput.aspx.cs" Inherits="ChartInput"
    EnableEventValidation="false" StylesheetTheme="Default" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

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
                    DataValueField="LogDate" AppendDataBoundItems="true" AutoCompleteMode="Suggest" DataTextFormatString="{0:MM/dd/yyyy}">
                    <asp:ListItem Value="">Select start date...</asp:ListItem>
                </ajax:ComboBox>                
            </td>

            <td class="separator"></td>

            <td>
                <asp:Label ID="lblEndDt" runat="server" AssociatedControlID="cbEndDt" Text="Date:" Font-Bold="true"></asp:Label>
            </td>
            <td>     
                <ajax:ComboBox ID="cbEndDt" runat="server" DataSourceID="sdsDate" DataTextField="LogDate" DropDownStyle="DropDownList"
                    DataValueField="LogDate" AppendDataBoundItems="true" AutoCompleteMode="Suggest" DataTextFormatString="{0:MM/dd/yyyy}">
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
    <table title="Measurement Category" summary="Select up to two measurement categories to generate your chart."> 
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


<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false"
	AutoEventWireup="true" CodeFile="logviewer.aspx.cs" Inherits="logviewer" StylesheetTheme="Default"
	Title="Web Portal | Application Log" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <p>
	    <asp:Label ID="lblError" runat="server" CssClass="error" EnableViewState="false"></asp:Label>
	</p>
	<table width="100%">
        <tr>
            <td style="text-align:center;">
                <b>View Log for</b> 
                <asp:DropDownList ID="ddlDate" runat="server" AppendDataBoundItems="true" AutoPostBack="true" 
                    DataSourceID="sdsDateList" DataTextField="date" DataValueField="date">
                    <asp:ListItem Value="">Please select...</asp:ListItem>
                    <asp:ListItem Value="all">All Dates</asp:ListItem>
                </asp:DropDownList>

            </td>
        </tr>
	<tr>
		<td>
			<lfr:SecureGridView ID="gvLog" runat="server" AutoGenerateColumns="False" 
				DataSourceID="sdsLog" EmptyDataText="" PageSize="15">
				<Columns>
					<asp:BoundField DataField="Date" SortExpression="Date" HeaderText="Date" ItemStyle-Width="20%" />
					<asp:BoundField DataField="Username" SortExpression="Username" HeaderText="User" ItemStyle-Width="15%" />
					<asp:BoundField DataField="Msg" SortExpression="Msg" HeaderText="Msg" ItemStyle-Width="19%" />
					<asp:BoundField DataField="URL" SortExpression="URL" HeaderText="URL or User-Agent" ItemStyle-Width="36%" />
					<asp:BoundField DataField="IP" SortExpression="IP" HeaderText="IP" ItemStyle-Width="10%" />
				</Columns>
			</lfr:SecureGridView>
        </td>
	</tr>
    </table>

    <asp:Panel ID="pnlSearch" runat="server">
        <table border="0" cellpadding="0" cellspacing="3" width="100%">
        <tr>
            <td width="25%">
            </td>
            <td width="45%">
                Search:
                <asp:TextBox ID="txtSearch" runat="server" Width="75%" SkinID="Search"></asp:TextBox> 
                <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/search-icon2.gif" 
                    ToolTip="Search" OnClick="btnSearch_Click" />
            </td>
            <td width="30%" align="right">
                <asp:Literal ID="litRecords" runat="server">Total Records: 0</asp:Literal>
            </td>
        </tr>
        </table>
    </asp:Panel>
    
	<asp:SqlDataSource ID="sdsDateList" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
            SELECT DISTINCT 'date' = CONVERT(varchar, Date, 101)
				, 'dateSort' = CAST(CONVERT(varchar, Date, 101) AS smalldatetime)
            FROM ApplicationLog
            WHERE SiteID = @SiteID
            ORDER BY 'dateSort' DESC
		">
		<SelectParameters>
			<asp:SessionParameter Name="SiteID" SessionField="SiteID" />
		</SelectParameters>
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="sdsLog" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
			SELECT al.date,al.url,al.msg,al.ip,
				'username' = ISNULL(m.firstname + ' ', '') + m.lastname
			FROM ApplicationLog al
				LEFT OUTER JOIN externalauthentication.dbo.membership m on m.userid = al.username
			WHERE 1 = CASE WHEN @date = 'all' THEN 1 WHEN CONVERT(varchar, al.date, 101) = @date THEN 1 ELSE 0 END
				AND al.siteid = @siteid
			ORDER BY al.date DESC
		" OnSelected="DisplayTotalRecords">
		<SelectParameters>
			<asp:SessionParameter Name="siteid" SessionField="SiteID" />
			<asp:ControlParameter Name="date" ControlID="ddlDate" PropertyName="SelectedValue" />
		</SelectParameters>
	</asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsTextSearch" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
			SELECT al.date,al.url,al.msg,al.ip,
				'username' = ISNULL(m.firstname + ' ', '') + m.lastname
			FROM ApplicationLog al
				LEFT OUTER JOIN externalauthentication.dbo.membership m on m.userid = al.username
			WHERE 1 = CASE WHEN @date = 'all' THEN 1 WHEN CONVERT(varchar, al.date, 101) = @date THEN 1 ELSE 0 END
				AND al.siteid = @siteid
				AND (
					al.username like '%{0}%'
					or al.url like '%{0}%'
					or al.msg like '%{0}%'
				)
			ORDER BY al.date DESC
        " OnSelected="DisplayTotalRecords" OnSelecting="sdsTextSearch_Selecting">
		<SelectParameters>
			<asp:SessionParameter Name="siteid" SessionField="SiteID" />
			<asp:ControlParameter Name="date" ControlID="ddlDate" PropertyName="SelectedValue" />
		</SelectParameters>
    </asp:SqlDataSource>
 
</asp:Content>

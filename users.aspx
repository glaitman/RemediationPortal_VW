<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master"
	CodeFile="users.aspx.cs" Inherits="users" Title="Web Portal | Users" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType virtualpath="~/Masterpage.Master" %>

<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <lfr:SecureGridView ID="gvUsers" runat="server" DataSourceID="sdsUsers" DataKeyNames="membershipsiteid" Width="100%"
        AutoGenerateColumns="false" EmptyDataText="There are no users to display."
        AllowPaging="true" AllowSorting="true" PageSize="10" 
        DesiredDelete="false" DesiredInsert="false" DesiredUpdate="false">
        <Columns>
            <asp:CommandField ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/images/select_button.gif" ItemStyle-Width="4%" />
            <asp:BoundField DataField="username" SortExpression="username" HeaderText="E-Mail" ItemStyle-Width="36%" />
            <asp:BoundField DataField="firstname" SortExpression="firstname" HeaderText="First Name" ItemStyle-Width="20%" />
            <asp:BoundField DataField="lastname" SortExpression="lastname" HeaderText="Last Name" ItemStyle-Width="20%" />
            <asp:BoundField DataField="company" SortExpression="company" HeaderText="Company" ItemStyle-Width="20%" />
        </Columns>
    </lfr:SecureGridView>
	<asp:Panel ID="pnlSearch" runat="server">
		<table border="0" cellpadding="0" cellspacing="3" width="100%">
			<tr>
				<td style="width: 25%;"></td>
				<td style="width: 45%;">
					Search:
					<asp:TextBox ID="txtSearch" runat="server" Width="75%" SkinID="Search" ValidationGroup="SearchGroup"></asp:TextBox>
					<asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/search-icon2.gif"
						ToolTip="Search" OnClick="btnSearch_Click" ValidationGroup="SearchGroup" />
				</td>
				<td style="text-align: right; width: 30%;">
					<asp:Literal ID="litRecords" runat="server">Total Records: 0</asp:Literal>
				</td>
			</tr>
		</table>
	</asp:Panel>
    <br />


    <table width="100%" cellspacing="0" cellpadding="0">
    <tr valign="top">
    
        <td width="60%">
            <p>
                <asp:Label ID="lblError" runat="server" CssClass="error" EnableViewState="false"></asp:Label>
                <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false"></asp:Label>
            </p>

            <asp:Panel ID="pnlAddUser" runat="server" GroupingText="Add Existing User" Visible="false" CssClass="existingUser">
                <asp:Label ID="lblUserExists" runat="server"></asp:Label>
                <br />
                <asp:Button ID="btnOK" runat="server" Text="OK" OnClick="btnOK_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </asp:Panel>
            
			<asp:Panel ID="pnlDetail" runat="server" Width="100%" GroupingText="User Detail">
			<div align="left" style="padding:10px 10px 10px 10px;">
		        <lfr:SecureDetailsView ID="dvUser" runat="server" AutoGenerateRows="false" Width="100%"
			        DataSourceID="sdsUserDetail" DataKeyNames="membershipsiteid" FieldHeaderStyle-Width="130px"
			        DesiredDelete="true" DesiredInsert="false" DesiredUpdate="true" OnDataBound="DetailsView_DataBound"
			        OnItemUpdated="dvUser_ItemUpdated" OnItemCommand="dvUser_ItemCommand" >
		        <Fields>
                    <asp:CommandField ShowEditButton="true" ShowInsertButton="true" NewText="Lock Account" />
                    
                    <lfr:BetterBoundField DataField="Username" HeaderText="User Name" MaxLength="128" 
                        Required="true" RequiredErrorMessage="Enter a user name"
                        ControlStyle-Width="80%" />
                    <lfr:BetterBoundField DataField="FirstName" HeaderText="First Name" MaxLength="64" 
                        Required="true" RequiredErrorMessage="Enter a first name"
                        ControlStyle-Width="80%" />
                    <lfr:BetterBoundField DataField="LastName" HeaderText="Last Name" MaxLength="64" 
                        Required="true" RequiredErrorMessage="Enter an E-Mail"
                        ControlStyle-Width="80%" />
                    <lfr:BetterBoundField DataField="Email" HeaderText="E-Mail" MaxLength="254" 
                        Required="true" RequiredErrorMessage="Enter a last name"
                        RegularExpressionValidationString="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        RegularExpressionValidationErrorMessage="Please enter a valid e-mail address."
                        ControlStyle-Width="80%" />
                    
			        <asp:CheckBoxField DataField="lockedout" HeaderText="Locked Out" ReadOnly="true" />
                    <asp:BoundField DataField="homedirectory" HeaderText="Home Directory" ControlStyle-Width="80%"
                        NullDisplayText="[default]"  />
                    <asp:TemplateField HeaderText="Reset Password">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPasswordReset" runat="server" TextMode="Password"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:CommandField ShowDeleteButton="true" />
		        </Fields>
		        </lfr:SecureDetailsView>
	        </div>
			</asp:Panel>
			
			<br />
			
			<asp:Panel ID="pnlPermission" runat="server" Width="100%" GroupingText="Permissions">
			<div align="left" style="padding:10px 10px 10px 10px;">
		        <lfr:SecureGridView ID="gvPermission" runat="server" AutoGenerateColumns="False" Width="100%"
			        DataSourceID="sdsPermission" DataKeyNames="membershipsitepermissionid"
			        DesiredDelete="true" DesiredInsert="false" DesiredUpdate="true" 
			        AllowSorting="true" AllowPaging="true" PageSize="6"
			        OnRowDataBound="GridView_RowDataBound" >
		        <Columns>
			        <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" DeleteText="Remove" ItemStyle-Width="30%" />
			        <asp:BoundField DataField="PermissionName" HeaderText="Name" SortExpression="PermissionName" ReadOnly="true" ItemStyle-Width="40%" />
			        <asp:CheckBoxField DataField="CanWrite" HeaderText="Write" SortExpression="CanWrite" ItemStyle-Width="10%" />
			        <asp:CheckBoxField DataField="CanRead" HeaderText="Read" SortExpression="CanRead" ItemStyle-Width="10%" />
			        <asp:CheckBoxField DataField="CanCreate" HeaderText="Create" SortExpression="CanCreate" ItemStyle-Width="10%" />
		        </Columns>
		        </lfr:SecureGridView>
		        <br />
		        New Permission:
		        <asp:DropDownList ID="ddlInsertPermission" runat="server"
		            DataSourceID="sdsPermissionList" DataValueField="sitepermissionid" DataTextField="permissionname">
		        </asp:DropDownList>
		        <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click" /><br />
		        
		    </div>
			</asp:Panel>
        </td>
        
		<td width="40%" style="padding:0 0 0 15px;">
            <asp:Panel ID="pnlNewUser" runat="server" GroupingText="New User">
                <br />
                <table width="90%" cellspacing="0" cellpadding="0" style="margin:0 0 0 10px;">
                <tr>
                    <td width="80">First Name:</td>
                    <td>
                        <asp:TextBox ID="txtFirstName" runat="server" Width="70%" ToolTip="First Name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rvFirstName" runat="server" Text="*" 
                            ControlToValidate="txtFirstName" EnableClientScript="true" Display="Dynamic" ValidationGroup="NewUser"
                            ErrorMessage="Please enter a first name." EnableViewState="false">
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Last Name:</td>
                    <td><asp:TextBox ID="txtLastName" runat="server" Width="70%" ToolTip="Last Name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rvLastName" runat="server" Text="*" 
                            ControlToValidate="txtLastName" EnableClientScript="true" Display="Dynamic" ValidationGroup="NewUser"
                            ErrorMessage="Please enter a last name." EnableViewState="false">
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Company:</td>
                    <td><asp:TextBox ID="txtCompany" runat="server" Width="70%" ToolTip="Company"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rvCompany" runat="server" Text="*" 
                            ControlToValidate="txtCompany" EnableClientScript="true" Display="Dynamic" ValidationGroup="NewUser"
                            ErrorMessage="Please enter a company." EnableViewState="false">
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>E-Mail:</td>
                    <td><asp:TextBox ID="txtEmail" runat="server" Width="80%" ToolTip="E-Mail"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rvEmail" runat="server" Text="*" 
                            ControlToValidate="txtEmail" EnableClientScript="true" Display="Dynamic" ValidationGroup="NewUser"
                            ErrorMessage="Please enter an e-mail address." EnableViewState="false">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" Text="*"
                            ControlToValidate="txtEmail" EnableClientScript="true" Display="Dynamic" ValidationGroup="NewUser"
                            ErrorMessage="Please enter a valid e-mail address." EnableViewState="false"
                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
                        </asp:RegularExpressionValidator>
                    </td>
                </tr>
                </table>
                
                <br />
                <div align="center">
                    <asp:Button ID="btnCreate" runat="server" OnClick="btnCreate_Click" Text="Create User" ValidationGroup="NewUser"
                        OnClientClick="return confirm('This will send an e-mail confirmation to the account above. Continue?')" />
                </div>
                
                <br />
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="true" ValidationGroup="NewUser" EnableViewState="false" />
            </asp:Panel>
            
            <br />
            
            
			<asp:Panel ID="pnlInstructions" runat="server" Width="100%" GroupingText="Need Help?">
				<div style="padding:10px 10px 10px 10px;">
				    To create a new user, enter a first name, last name, and e-mail address above, then
				    click the "Create User" button. Once you click the button an e-mail will be sent to the 
				    e-mail address you specified with a randomly-generated password and instructions on how
				    to find and use this site. You cannot create a new user without sending them this
				    e-mail.
				    <br /><br />
				    If the user already exists, an message will appear stating this. Instead, go to
				    the "Assign User" area to give the existing user permission to access your site. This
				    way, he can reuse his existing account information for various sites he goes to.
				    <br /><br />
				    Once the user is created, you can add or change individual permissions in the area on the
				    left. Select the user from the top grid, then use the "New Permission" drop-down to add
				    permissions to the user. Use the "Edit" link next to new permissions to change the checkbox
				    values, and click the "Update" link when finished.
				</div>
			</asp:Panel>
			
		</td>
		
    </tr>
    </table>
    
    <asp:SqlDataSource ID="sdsUsers" runat="server" ConnectionString="<%$ ConnectionStrings:MembershipProvider %>"
        SelectCommand="
			select ms.membershipsiteid,m.firstname,m.lastname,m.company,
				'username' = case when (m.lockedout is null or m.lockedout = 0) then m.username else m.username + ' [Locked Out]' end
			from membershipsite ms
				join membership m on m.userid = ms.userid
			where ms.siteid = @siteid
			order by m.username
        "
        OnSelected="DisplayTotalRecords">
		<SelectParameters>
			<asp:SessionParameter Name="siteid" SessionField="SiteID" />
		</SelectParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="sdsUserDetail" runat="server" ConnectionString="<%$ ConnectionStrings:MembershipProvider %>"
        SelectCommand="
            select ms.membershipsiteid,ms.homedirectory,m.lockedout,m.username,m.firstname,m.lastname,m.company, m.email
            from membershipsite ms
            join membership m on m.userid = ms.userid
            where ms.membershipsiteid = @membershipsiteid
        "
        DeleteCommand="
            delete from membershipsite
            where membershipsiteid = @membershipsiteid
        "
        UpdateCommand="
            update membershipsite set
            homedirectory = @homedirectory
            where membershipsiteid = @membershipsiteid;
            
            update membership set
            username = @username,
            firstname = @firstname,
            Email = @Email,
            lastname = @lastname,
            company = @company
            from membership
            join membershipsite ms on ms.userid = membership.userid
            where ms.membershipsiteid = @membershipsiteid
        "
        OnSelected="HandleDataException" OnDeleted="sdsUserDetail_Status" OnUpdated="sdsUserDetail_Status">
		<SelectParameters>
			<asp:ControlParameter Name="membershipsiteid" ControlID="gvUsers" PropertyName="SelectedValue" />
		</SelectParameters>
		<DeleteParameters>
			<asp:ControlParameter Name="membershipsiteid" ControlID="dvUser" PropertyName="SelectedValue" />
		</DeleteParameters>
		<UpdateParameters>
		    <asp:Parameter Name="username" />
		    <asp:Parameter Name="firstname" />
		    <asp:Parameter Name="lastname" />
		    <asp:Parameter Name="Email" />
		    <asp:Parameter Name="company" />
			<asp:Parameter Name="homedirectory" />
			<asp:ControlParameter Name="membershipsiteid" ControlID="dvUser" PropertyName="SelectedValue" />
		</UpdateParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="sdsPermissionList" runat="server" ConnectionString="<%$ ConnectionStrings:MembershipProvider %>"
        SelectCommand="
            select sitepermissionid,permissionname
            from sitepermission
            where siteid = @siteid
            and (disabled is null or disabled = 0)
            order by permissionname
        "
        OnSelected="HandleDataException">
		<SelectParameters>
			<asp:SessionParameter Name="siteid" SessionField="SiteID" />
		</SelectParameters>
    </asp:SqlDataSource>
    
	<asp:SqlDataSource ID="sdsPermission" runat="server" ConnectionString="<%$ ConnectionStrings:MembershipProvider %>"
		SelectCommand="
            select msp.membershipsitepermissionid,sp.permissionname,msp.canread,msp.canwrite,msp.cancreate
            from membershipsitepermission msp
            join sitepermission sp on sp.sitepermissionid = msp.sitepermissionid
            where msp.membershipsiteid = @membershipsiteid
            ORDER BY sp.permissionname
		"
		DeleteCommand="
			delete from membershipsitepermission
			where membershipsitepermissionid = @membershipsitepermissionid
		"
		UpdateCommand="
			update membershipsitepermission set
			canread = @canread,
			canwrite = @canwrite,
			cancreate = @cancreate
			where membershipsitepermissionid = @membershipsitepermissionid
		"
		InsertCommand="sp_InsertSitePermission" InsertCommandType="StoredProcedure"
		OnSelected="HandleDataException">
		<SelectParameters>
			<asp:ControlParameter Name="membershipsiteid" ControlID="gvUsers" PropertyName="SelectedValue" />
		</SelectParameters>
		<DeleteParameters>
			<asp:ControlParameter Name="membershipsitepermissionid" ControlID="gvPermission" PropertyName="SelectedValue" />
		</DeleteParameters>
		<UpdateParameters>
			<asp:Parameter Name="CanRead" />
			<asp:Parameter Name="CanWrite" />
			<asp:Parameter Name="CanCreate" />
			<asp:ControlParameter Name="membershipsitepermissionid" ControlID="gvPermission" PropertyName="SelectedValue" />
		</UpdateParameters>
		<InsertParameters>
			<asp:ControlParameter Name="membershipsiteid" ControlID="gvUsers" PropertyName="SelectedValue" />
			<asp:ControlParameter Name="sitepermissionid" ControlID="ddlInsertPermission" PropertyName="SelectedValue" />
		</InsertParameters>
	</asp:SqlDataSource>
	
    <asp:SqlDataSource ID="sdsTextSearch" runat="server" ConnectionString="<%$ ConnectionStrings:MembershipProvider %>"
        SelectCommand="
			select ms.membershipsiteid,m.firstname,m.lastname,m.company,
				'username' = case when (m.lockedout is null or m.lockedout = 0) then m.username else m.username + ' [Locked Out]' end
			from membershipsite ms
				join membership m on m.userid = ms.userid
			where ms.siteid = @siteid
				AND (
					m.firstname LIKE '%{0}%'
					OR m.lastname LIKE '%{0}%'
					OR m.company LIKE '%{0}%'
					OR m.username LIKE '%{0}%'
				)
			order by m.username
        "
        OnSelected="DisplayTotalRecords" OnSelecting="sdsTextSearch_Selecting">
		<SelectParameters>
			<asp:SessionParameter Name="siteid" SessionField="SiteID" />
		</SelectParameters>
    </asp:SqlDataSource>

</asp:Content>

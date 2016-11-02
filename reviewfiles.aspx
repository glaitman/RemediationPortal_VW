<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="reviewfiles.aspx.cs"
	Inherits="reviewfiles" EnableEventValidation="false" Title="Review Files" StylesheetTheme="Default"
    MaintainScrollPositionOnPostback="true"	 %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" ShowSummary="false" 
		ShowMessageBox="true" HeaderText="Please correct the following:" />
	<table>
		<tr class="gridrowhiglight">
			<td>Category</td>
			<td>
				<asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true"
					OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
					<asp:ListItem Value="">Please select...</asp:ListItem>
					<asp:ListItem Value="Regulated Waste$">Regulated Waste</asp:ListItem>
					<asp:ListItem Value="Site Remediation$">Site Remediation</asp:ListItem>
					<asp:ListItem Value="Waste &amp; Recycling$">Waste &amp; Recycling</asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
	</table>
	
	<asp:MultiView ID="mvMain" runat="server">
	<asp:View ID="vwWaste" runat="server">
		<lfr:SecureGridView ID="gvFile" runat="server" AutoGenerateColumns="False" Width="100%"
			DataSourceID="sdsFile" DataKeyNames="upWasteFilesID"
			EmptyDataText="There are no files to display." PageSize="50">
		<Columns>
			<asp:CommandField ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/Images/select_button.gif" ItemStyle-Width="4%" />
			<asp:TemplateField HeaderText="File" ItemStyle-Width="4%" 
				ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:ImageButton ID="ibIcon" runat="server" ImageUrl="~/Images/icon_document.gif" OnCommand="ib_Command"
						CommandName="View" CommandArgument='<%# Eval("upWasteFilesID") %>' ToolTip="Download this file"></asp:ImageButton>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:BoundField DataField="Filename" SortExpression="Filename" HeaderText="Filename" ItemStyle-Width="23%" />
			<asp:BoundField DataField="FileSize" SortExpression="FileSize" HeaderText="Size" ItemStyle-Width="10%" />
			<asp:BoundField DataField="ShippedDate" SortExpression="ShippedDate" HeaderText="Date Shipped" 
				HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="15%" />
			<asp:BoundField DataField="TrackingNumber" SortExpression="TrackingNumber" HeaderText="Tracking" ItemStyle-Width="10%" />
			<asp:BoundField DataField="UploadedBy" SortExpression="UploadedBy" HeaderText="Uploaded By" ItemStyle-Width="18%" />
			<asp:BoundField DataField="UploadDate" SortExpression="UploadDate" HeaderText="Upload Date" 
				HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ItemStyle-Width="16%" />
		</Columns>
		</lfr:SecureGridView>

		<br />
		<asp:Panel ID="pnlDetail" runat="server" Width="100%">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr valign="top">
				<td width="60%">
					<h3><asp:Label ID="lblDetailName" runat="server" Text="Detail" /></h3>
					<lfr:SecureDetailsView ID="dvFile" runat="server" DataSourceID="sdsFileDetail"
						DataKeyNames="upWasteFilesID" AutoGenerateRows="False" Width="100%">
					<FieldHeaderStyle Width="140px" />
					<Fields>
						<asp:CommandField />
						<asp:TemplateField HeaderText="File">
							<InsertItemTemplate>
								<asp:FileUpload ID="fuInsert" runat="server" Width="100%" />
							</InsertItemTemplate>
							<ItemTemplate>
								<%# Eval("filename") %>
							</ItemTemplate>
						</asp:TemplateField>
				        
						<lfr:BetterBoundField DataField="Filename" HeaderText="Filename" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="Category" HeaderText="Category" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="ContentType" HeaderText="ContentType" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="FileSize" HeaderText="FileSize" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="ShippedDate" HeaderText="Date Shipped" MaxLength="32"
							HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ApplyFormatInEditMode="true"
							Type="Date" TypeErrorMessage="Enter a date value for 'Date Shipped'"
							Required="true" RequiredErrorMessage="Enter a value for 'Date Shipped'"
							ControlStyle-Width="45%" />
						<lfr:BetterBoundField DataField="TrackingNumber" HeaderText="Tracking Number" MaxLength="30" 
							Required="true" RequiredErrorMessage="Enter a value for 'Tracking Number'"
							ControlStyle-Width="60%" />
						<lfr:BetterBoundField DataField="Source" HeaderText="Source" MaxLength="255" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="UploadedBy" HeaderText="Uploaded By" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="UploadDate" HeaderText="Upload Date" MaxLength="32"
							HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ApplyFormatInEditMode="true"
							ControlStyle-Width="45%" />

						<asp:CommandField />
					</Fields>
					</lfr:SecureDetailsView>
				</td>
				<td width="40%" style="padding:0 0 0 15px;">
					<br />
				</td>
			</tr>
			</table>
		</asp:Panel>
	</asp:View>
	<asp:View ID="vwRemediation" runat="server">
		<lfr:SecureGridView ID="gvRemediation" runat="server" AutoGenerateColumns="False" Width="100%"
			DataSourceID="sdsRemediation" DataKeyNames="upRemediationFilesID"
			EmptyDataText="There are no files to display." PageSize="50">
		<Columns>
			<asp:CommandField ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/Images/select_button.gif" ItemStyle-Width="4%" />
			<asp:TemplateField HeaderText="File" ItemStyle-Width="4%" 
				ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:ImageButton ID="ibIcon" runat="server" ImageUrl="~/Images/icon_document.gif" OnCommand="ib_Command"
						CommandName="View" CommandArgument='<%# Eval("upRemediationFilesID") %>' ToolTip="Download this file"></asp:ImageButton>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:BoundField DataField="Filename" SortExpression="Filename" HeaderText="Filename" ItemStyle-Width="45%" />
			<asp:BoundField DataField="FileSize" SortExpression="FileSize" HeaderText="Size" ItemStyle-Width="8%" />
			<asp:BoundField DataField="UploadedBy" SortExpression="UploadedBy" HeaderText="Uploaded By" ItemStyle-Width="23%" />
			<asp:BoundField DataField="UploadDate" SortExpression="UploadDate" HeaderText="Upload Date" 
				HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ItemStyle-Width="16%" />
		</Columns>
		</lfr:SecureGridView>

		<br />
		<asp:Panel ID="pnlRemediationDetail" runat="server" Width="100%">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr valign="top">
				<td width="60%">
					<h3><asp:Label ID="lbRemediationDetailName" runat="server" Text="Detail" /></h3>
					<lfr:SecureDetailsView ID="gvRemediationDetail" runat="server" DataSourceID="sdsRemediationDetail"
						DataKeyNames="upRemediationFilesID" AutoGenerateRows="False" Width="100%">
					<FieldHeaderStyle Width="140px" />
					<Fields>
						<asp:CommandField />
						<asp:TemplateField HeaderText="File">
							<InsertItemTemplate>
								<asp:FileUpload ID="fuInsert" runat="server" Width="100%" />
							</InsertItemTemplate>
							<ItemTemplate>
								<%# Eval("filename") %>
							</ItemTemplate>
						</asp:TemplateField>
				        
						<lfr:BetterBoundField DataField="Filename" HeaderText="Filename" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="Category" HeaderText="Category" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="ContentType" HeaderText="ContentType" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="FileSize" HeaderText="FileSize" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="Source" HeaderText="Source" MaxLength="255" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="UploadedBy" HeaderText="Uploaded By" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="UploadDate" HeaderText="Upload Date" MaxLength="32"
							HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ApplyFormatInEditMode="true"
							ControlStyle-Width="45%" />

						<asp:CommandField />
					</Fields>
					</lfr:SecureDetailsView>
				</td>
				<td width="40%" style="padding:0 0 0 15px;">
					<br />
				</td>
			</tr>
			</table>
		</asp:Panel>
	</asp:View>
	</asp:MultiView>

	<asp:SqlDataSource ID="sdsFile" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
		    SELECT upWasteFilesID, Filename, FileSize, ShippedDate, TrackingNumber, UploadedBy, UploadDate
            FROM up_WasteFiles
            ORDER BY upWasteFilesID DESC
	    ">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="sdsFileDetail" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
	    OnInserting="test"
		SelectCommand="
		    SELECT *
            FROM up_WasteFiles
            WHERE upWasteFilesID = @upWasteFilesID
	    "
	>
	<SelectParameters>
		<asp:ControlParameter Name="upWasteFilesID" ControlID="gvFile" PropertyName="SelectedValue" />
	</SelectParameters>
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="sdsRemediation" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
		    SELECT upRemediationFilesID, Filename, FileSize, UploadedBy, UploadDate
            FROM up_RemediationFiles
            ORDER BY upRemediationFilesID DESC
	    ">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="sdsRemediationDetail" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
	    OnInserting="test"
		SelectCommand="
		    SELECT *
            FROM up_RemediationFiles
            WHERE upRemediationFilesID = @upRemediationFilesID
	    "
	>
	<SelectParameters>
		<asp:ControlParameter Name="upRemediationFilesID" ControlID="gvRemediation" PropertyName="SelectedValue" />
	</SelectParameters>
	</asp:SqlDataSource>
	
</asp:Content>


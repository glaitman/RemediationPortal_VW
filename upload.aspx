<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false"
	AutoEventWireup="true" CodeFile="upload.aspx.cs" Inherits="upload" StylesheetTheme="Default"
	Title="Web Portal | Upload Page" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" ShowSummary="false" 
    ShowMessageBox="true" HeaderText="Please correct the following:" />

<asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
<asp:View ID="vwUpload" runat="server">
	<table width="100%" cellpadding="0" cellspacing="0">
	<tr valign="top">
		<td>
			<h3>Select Upload Parameters</h3>
			<table width="100%" class="grid" border="1">
				<tr class="gridheader">
					<td colspan="2"><br /></td>
				</tr>
				<tr class="gridrowhiglight">
					<td>Category</td>
					<td>
                        <asp:DropDownList 
                            ID="ddlCategory" 
                            runat="server"
                            DataSourceID="xdsUploadCategory"
                            DataValueField="ID"
                            DataTextField="DisplayName"
                            AppendDataBoundItems="true">                            
							<asp:ListItem Value="">Please select...</asp:ListItem>							
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>Spreadsheet to Upload</td>
					<td>
						<asp:FileUpload ID="fu1" runat="server" Width="300px" 
							ToolTip="Please select the data (XLS, CSV, TXT) to upload." />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" 
                            ControlToValidate="fu1" ErrorMessage="Choose a data file to upload." Text="*"
                            EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr class="gridrowhiglight">
					<td>Column Names in 1st Row</td>
					<td>
						<asp:CheckBox ID="chkColumnNames" runat="server" Checked="false" />
					</td>
				</tr>
				<tr class="gridheader">
					<td colspan="2"><br /></td>
				</tr>
			</table>
			
			<div align="center">
				<asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
			</div>
			
			<br />
			<asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>						
		</td>
		<td width="40%" style="padding:0 0 0 15px;">
			<asp:Panel ID="Panel2" runat="server" Width="100%" GroupingText="Instructions">
				<div class="instructions">
					<ol>
					<%--<li>Choose the category of data to upload from the Category dropdown.</li>--%>
					<li>Click Browse to select an Excel spreadsheet to upload. Browse will search 
					your local harddrive.</li>
					<li>CLOSE THE SPREADSHEET IN EXCEL BEFORE UPLOADING.</li>
					<li>If the spreadsheet does not have column names in the first row, 
						uncheck this box before continuing.</li>
					<li>Click the "Upload" button to continue.</li>
					</ol>
					<%--<br />
					If there are no values in the Category dropdown, please contact an administrator 
					to recieve the correct permissions.--%>
				</div>
			</asp:Panel>
		</td>
	</tr>
	    
	</table>

</asp:View>
<asp:View ID="vwPreview" runat="server">
	<table width="100%" cellpadding="0" cellspacing="0">
	<tr valign="top">
		<td width="60%">
			<h3>Preview</h3>
<%--
			<lfr:SecureGridView ID="GridView1" runat="server"></lfr:SecureGridView>
			<br />
--%>			
			<table width="100%" class="grid" border="1">
				<tr class="gridheader">
					<td><asp:Literal ID="litCount" runat="server"></asp:Literal></td>
				</tr>
			</table>
			<lfr:SecureDetailsView ID="DetailsView1" runat="server"></lfr:SecureDetailsView>
			<table width="100%" class="grid">
				<tr class="gridheader">
					<td><br /></td>
				</tr>
			</table>

			<div align="center">
				<asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_Click" />
				<asp:Button ID="btnContinue3" runat="server" Text="Continue" OnClick="btnContinue2_Click" Visible="false" />
				<asp:Button ID="Button1" runat="server" Text="Back" OnCommand="ib_Command" CommandName="Back" />
			</div>
		</td>
		<td width="40%" style="padding:0 0 0 15px;">
			<asp:Panel ID="Panel3" runat="server" Width="100%" GroupingText="Instructions">
				<div class="instructions">
					<ol>
					<li>This is a preview of the data. If this doesn't look correct click "Back" to select 
						a different file or change your settings.</li>
					<li>Click "Continue" to proceed.</li>
					</ol>
				</div>
			</asp:Panel>
		</td>
	</tr>
	</table>            	
</asp:View>
<asp:View ID="vwMapFields" runat="server">
	<table width="100%" cellpadding="0" cellspacing="0">
	<tr valign="top">
		<td width="60%">
			<h3>Map Fields</h3>
			<asp:Panel ID="pnlMapFields" runat="server">
			</asp:Panel>

			<div align="center">
				<asp:Button ID="btnContinue2" runat="server" Text="Continue" OnClick="btnContinue2_Click" />
				<asp:Button ID="Button2" runat="server" Text="Back" OnCommand="ib_Command" CommandName="Back" />
			</div>
		</td>
		<td width="40%" style="padding:0 0 0 15px;">
			<asp:Panel ID="Panel4" runat="server" Width="100%" GroupingText="Instructions">
				<div class="instructions">
					<ol>
					<li>If you need to map the fields from your spreadsheet to the destination
						table, you will be prompted here.</li>
					<li>Click "Continue" to proceed.</li>
					</ol>
				</div>
			</asp:Panel>
		</td>
	</tr>
	</table>		
</asp:View>
<asp:View ID="vwAttachDocs" runat="server">
	<table width="100%" cellpadding="0" cellspacing="0">
	<tr valign="top">
		<td width="60%">
			<h3>Supporting Documents Upload</h3>
			<table width="100%" class="grid" border="1">
			<tr class="gridheader">
				<td colspan="2"><br /></td>
			</tr>
			<tr class="gridrowhiglight">
			<td>Attached File</td>
			<td>
				<asp:FileUpload ID="fu2" runat="server" Width="300px" 
					ToolTip="Please select a single file or zipfile containing documents you want to attach." /><br />
				<em>(Optional)</em>
			</td>
			</tr>
			<tr class="gridheader">
				<td colspan="2"><br /></td>
			</tr>
			</table>

			<div align="center">
				<asp:Button ID="btnSubmit" runat="server" Text="Continue" OnClick="btnSubmit_Click"
					OnClientClick="if(!confirm('This will now add the spreadsheet data rows and supporting files to the staging database. Continue?')) return false;" />
				<asp:Button ID="btnBack" runat="server" Text="Back" OnCommand="ib_Command" CommandName="Back" />
			</div>
		</td>
		<td width="40%" style="padding:0 0 0 15px;">
			<asp:Panel ID="pnlFileUploadGeneral" runat="server" Width="100%" GroupingText="Instructions">
				<div class="instructions">
					<ol>
					<li>This is a place to submit supporting documents such as waste manifests and certificates
						of recycling (waste &amp; reycling) and consultant reports (site remediation).</li>
					<li>If you have a number of documents to attach, zip them up into a single zipfile before
						selecting them here.</li>
					<li>Click "Continue" to upload the data to IDEA.</li>
					<li>File names in general must follow the convention MAILCODE_DESCRIPTION_YYYY_MM_DD. 
						Please use the full MAILCODE, e.g. AZ4-104. DESCRIPTION can be any descriptive text.
						For example, CA3-100_QuarterlyMonitoringReport_2008_03_15. Use the date of the
						report as the date of the file.</li>
					<li>You may use YYYY-MM-DD or YYYYMMDD as alternate layouts for the date of the report, 
						but YYYY_MM_DD is preferred.</li>
					</ol>
				</div>
			</asp:Panel>
			<asp:Panel ID="pnlFileUploadWaste" runat="server" Width="100%" GroupingText="Instructions" Visible="false">
				<div class="instructions">
					<ol>
					<li>This is a place to submit supporting documents such as waste manifests and certificates
						of recycling.</li>
					<li>If you have a number of documents to attach, zip them up into a single zipfile before
						selecting them here.</li>
					<li>Click "Continue" to upload the data to IDEA.</li>
					<li>Disposal document filenames must follow a particular convention. Please use 
						DOCTYPE_TRACKINGNUMBER_YYYY-MM-DD, where DOCTYPE is one of the abbreviations below, 
						TRACKINGNUMBER is a unique number associated with the document, and DATE is 
						the date shipped. For example, BOL_X112F0_2008-03-15.pdf</li>
					<li>Use the list below as an order of priority when
						assigning a Tracking Number (e.g. if a manifest number is available, use it in
						favor of an invoice number). Tracking Numbers only apply to disposal documents.
						<br /><br />
						<b>BOL</b>- Manifest / Bill of Lading<br />
						<b>RC</b>- Certificate of Recycling<br />
						<b>INV</b>- Invoice / Receipt<br />
						<b>WT</b>- Weight Ticket<br />
						</li>
					<li>You may use YYYY_MM_DD or YYYYMMDD as alternate layouts for the DATESHIPPED, 
						but YYYY-MM-DD is preferred.</li>
					</ol>
				</div>
			</asp:Panel>
		</td>
	</tr>
	</table>		
</asp:View>
<asp:View ID="vwMapDocs" runat="server">
	<table width="100%" cellpadding="2" cellspacing="0">
	<tr valign="top">
		<td width="60%">
			<h3>Map Documents to Issue</h3>
			<asp:Panel ID="pnlMapDocs" runat="server">
				<asp:Table ID="tblMapDocs" runat="server" CssClass="grid" BorderWidth="1px">
					<asp:TableRow CssClass="gridheader">
						<asp:TableCell Width="120px"></asp:TableCell>
						<asp:TableCell></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell><b>Mail Code</b></asp:TableCell>
						<asp:TableCell><asp:Literal ID="litMailCode" runat="server"></asp:Literal></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow CssClass="gridrowhiglight">
						<asp:TableCell><b>File</b></asp:TableCell>
						<asp:TableCell><asp:Literal ID="litFileName" runat="server"></asp:Literal></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell><b>Related Issue</b></asp:TableCell>
						<asp:TableCell>
							<%--<asp:DropDownList ID="ddlInventory" runat="server"></asp:DropDownList>--%>
							<asp:RadioButtonList ID="rblInventory" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow CssClass="gridrowhiglight">
						<asp:TableCell><b>Title</b></asp:TableCell>
						<asp:TableCell><asp:TextBox ID="tbTitle" runat="server" MaxLength="128" Width="95%"></asp:TextBox></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell><b>Description</b></asp:TableCell>
						<asp:TableCell>
							<asp:DropDownList id="ddlCategoryInsert" runat="server"
								ToolTip="Description">
								<asp:ListItem Value="">Please select...</asp:ListItem>
								<asp:ListItem>Asbestos Survey Report</asp:ListItem>
								<asp:ListItem>Assessment / Investigation</asp:ListItem>
								<asp:ListItem>Audit</asp:ListItem>
								<asp:ListItem>Communication</asp:ListItem>
								<asp:ListItem>Corrective Action / Remediation</asp:ListItem>
								<asp:ListItem>Data</asp:ListItem>
								<asp:ListItem>Floor/Site Plans</asp:ListItem>
								<asp:ListItem>LEED Documentation</asp:ListItem>
								<asp:ListItem>Legal Documents</asp:ListItem>
								<asp:ListItem>Photos</asp:ListItem>
								<asp:ListItem>Permit / Registration</asp:ListItem>
								<asp:ListItem>Plan</asp:ListItem>
								<asp:ListItem>Proposal</asp:ListItem>
								<asp:ListItem>Regulatory Correspondence</asp:ListItem>
								<asp:ListItem>Specification</asp:ListItem>
								<asp:ListItem>Training/Presentation</asp:ListItem>
								<asp:ListItem>Waste Documentation</asp:ListItem>
								<asp:ListItem>Other</asp:ListItem>
							</asp:DropDownList>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow CssClass="gridrowhiglight">
						<asp:TableCell><b>Date Created</b></asp:TableCell>
						<asp:TableCell>
							<asp:TextBox ID="tbDateCreated" runat="server" MaxLength="32"></asp:TextBox>
						<%--	<asp:CompareValidator ID="cvDateCreated" Type="Date" runat="server" ControlToValidate="tbDateCreated" 
								ErrorMessage="Enter a date value for 'Date Created'" Text="*" />--%>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell><b>Prepared For</b></asp:TableCell>
						<asp:TableCell><asp:TextBox ID="tbPreparedFor" runat="server" MaxLength="64" Width="60%"></asp:TextBox></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow CssClass="gridrowhiglight">
						<asp:TableCell><b>Prepared By</b></asp:TableCell>
						<asp:TableCell><asp:TextBox ID="tbPreparedBy" runat="server" MaxLength="64" Width="60%"></asp:TextBox></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell><b>Notes</b></asp:TableCell>
						<asp:TableCell><asp:TextBox ID="tbNotes" runat="server" Width="95%"></asp:TextBox></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow CssClass="gridheader">
						<asp:TableCell></asp:TableCell>
						<asp:TableCell></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:Panel>
			<asp:HiddenField ID="hFileID" runat="server" />
			<div align="center">
				<asp:Button ID="btnSubmitMapDocs" runat="server" Text="Next" OnClick="btnSubmitMapDocs_Click" />
				<asp:Button ID="btnBackMapDocs" runat="server" Text="Back" OnCommand="ib_Command" CommandName="BackMapDocs" />
			</div>
		</td>
		<td width="40%" style="padding:0 0 0 15px;">
			<asp:Panel ID="Panel5" runat="server" Width="100%" GroupingText="Instructions">
				<div class="instructions">
					<ol>
					<li>Uploaded documents can be linked to specific issues from the uploaded spreadsheet.</li>
					<li>To link the document to a issue, select a radio button next to the correct item 
						in the Related Issue list. If the document should not be linked to any of the listed issues, select the 
						"None" option. If the Mail Code of the document does not match any of the Mail Codes in the uploaded
						spreadsheet, "None" will be the only option.</li>
					<li>Fill in any other relevant information for the document.</li>
					<li>Click "Next" to save the data and proceed.</li>
					</ol>
				</div>
			</asp:Panel>
		</td>
	</tr>
	</table>
</asp:View>
<asp:View ID="vwComplete" runat="server">
	<h3><asp:Literal ID="litCompleteHeader" runat="server">Import Complete</asp:Literal></h3>
	<div style="width:70%;margin:0 auto;">
		<table class="grid">
			<tr class="gridrowhiglight">
				<td style="padding:24px 16px;">
					<asp:Label ID="lblSuccess" runat="server" EnableViewState="false"></asp:Label>
					
					<div id="divErrs" runat="server" visible="false" style="width:100%;">
						 <asp:Label ID="lbError" runat="server" CssClass="error" EnableViewState="false">The following errors have occurred.</asp:Label> 
						 <%--<br />Please review the file considering the errors listed below.--%>
						 <br />If corrections can be made, please do so and try to upload the file again.
						 <br /><u>NOTE:</u> <asp:literal ID="litFileNameFail" runat="server" Text="The file" /> was not uploaded.<br /><br />
						<table border="1">
							<tr>
								<th>Row</th><th>Source Field</th><th>Destination</th><th>Description</th>
							</tr>
							<asp:Literal ID="litErrors" runat="server" EnableViewState="false" />
						</table>
						
					</div>
				</td>
			</tr>
		</table>
	</div>
	
	<br /><br />
	<div align="center">
		<asp:Button ID="btnUploadAnother" runat="server" Text="Upload Another" OnClick="btnUploadAnother_Click" />
	</div>
</asp:View>
</asp:MultiView>

<asp:XmlDataSource ID="xdsDataSchema" runat="server" DataFile="~/App_Data/XML/schema.xml" XPath="Root/Table">
</asp:XmlDataSource>
<asp:XmlDataSource ID="xdsFileSchema" runat="server" DataFile="~/App_Data/XML/schema_files.config" XPath="Root/Table">
</asp:XmlDataSource>

    <asp:XmlDataSource 
        ID="xdsUploadCategory" 
        runat="server" 
        DataFile="~/App_Data/XML/schema.xml" 
        XPath="Root/Table">
    </asp:XmlDataSource>
</asp:Content>


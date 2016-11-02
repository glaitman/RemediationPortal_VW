<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false"
	AutoEventWireup="true" CodeFile="uploadfiles.aspx.cs" Inherits="uploadfiles" StylesheetTheme="Default"
	Title="Web Portal | Upload Files Only" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableViewState="false" ShowSummary="false" 
    ShowMessageBox="true" HeaderText="Please correct the following:" />

<asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
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
					<td>Category</td>
					<td>
						<asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true"
							OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
							<asp:ListItem Value="">Please select...</asp:ListItem>
							<asp:ListItem Value="Site Remediation$">Site Remediation</asp:ListItem>
							<asp:ListItem Value="Waste &amp; Recycling$">Waste &amp; Recycling</asp:ListItem>
							<asp:ListItem Value="Waste &amp; Recycling JXO$">Waste &amp; Recycling JXO</asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>Attached File</td>
					<td>
						<asp:FileUpload ID="fu2" runat="server" Width="300px" 
							ToolTip="Please select a single file or zipfile containing documents you want to attach." />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" 
                            ControlToValidate="fu2" ErrorMessage="Choose a file to upload." Text="*"
                            EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr class="gridheader">
					<td colspan="2"><br /></td>
				</tr>
			</table>

			<div align="center">
				<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click"
					OnClientClick="if(!confirm('This will add the files to the staging database. Continue?')) return false;" />
			</div>
		</td>
		<td width="40%" style="padding:0 0 0 15px;">
			<asp:Panel ID="pnlGeneralInstructions" runat="server" Width="100%" GroupingText="Instructions" Visible="false">
				<div class="instructions">
					<ol>
					<li>This is a place to submit supporting documents such as waste manifests and certificates
						of recycling (waste &amp; reycling) and consultant reports (site remediation).</li>
					<li>If you have a number of documents to attach, zip them up into a single zipfile before
						selecting them here.</li>
					<li>Click "Submit" to upload the data to IDEA.</li>
					<li>File names in general must follow the convention MAILCODE_DESCRIPTION_YYYY_MM_DD. 
						Please use the full MAILCODE, e.g. AZ4-104. DESCRIPTION can be any descriptive text.
						For example, CA3-100_QuarterlyMonitoringReport_2008_03_15. Use the date of the
						report as the date of the file.</li>
					<li>You may use YYYY-MM-DD or YYYYMMDD as alternate layouts for the date of the report, 
						but YYYY_MM_DD is preferred.</li>
					</ol>
				</div>
			</asp:Panel>
			<asp:Panel ID="pnlWasteInstructions" runat="server" Width="100%" GroupingText="Instructions" Visible="false">
				<div class="instructions">
					<ol>
					<li>This is a place to submit supporting documents such as waste manifests and certificates
						of recycling.</li>
					<li>If you have a number of documents to attach, zip them up into a single zipfile before
						selecting them here.</li>
					<li>Click "Submit" to upload the data to IDEA.</li>
					<li>File names must follow the convention DOCTYPE_TRACKINGNUMBER_DATESHIPPED, where 
						DOCTYPE is one of the abbreviations below, TRACKINGNUMBER is a unique number
						associated with the document, and DATESHIPPED is the date shipped in the format 
						YYYY-MM-DD.</li>
					<li>Please use the list below as an order of priority when
						assigning a Tracking Number (e.g. if a manifest number is available, use it in
						favor of an invoice number).
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

</asp:Content>


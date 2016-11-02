<%@ Page Title="Web Portal | Measurement Settings" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" 
    CodeFile="MeasurementCategory.aspx.cs" Inherits="MeasurementCategory" StylesheetTheme="Default" %>

<%@ MasterType virtualpath="~/Masterpage.Master" %>
<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />

	<lfr:SecureGridView ID="gvMain" runat="server" AutoGenerateColumns="False" PageSize="10"
		DataSourceID="sdsMain" DataKeyNames="ListManagerID" Caption="<b>Edit</b>">
		<Columns>
			<asp:CommandField ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/Images/select_button.gif" ItemStyle-Width="4%" />
            <asp:BoundField DataField="ListKey" SortExpression="ListKey" HeaderText="List Key" ItemStyle-Width="9%" 
                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="ListValue" SortExpression="ListValue" HeaderText="List Value" ItemStyle-Width="30%"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="ListValueDescription" SortExpression="ListValueDescription" HeaderText="List Value Description" 
                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="45%" />
			<asp:BoundField DataField="ModifiedDate" HeaderText="Modified Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="12%" />           
		</Columns>
	</lfr:SecureGridView>
     <asp:Panel ID="pnlSearch" runat="server" CssClass="pnlSearch">
		<asp:Panel ID="pnlNewRecord" runat="server" CssClass="newrecord"></asp:Panel>
		<div class="divSearch">
<%--                Search:
                <asp:TextBox ID="txtSearch" runat="server" Width="75%" SkinID="Search" ValidationGroup="SearchGroup"></asp:TextBox> 
                <asp:ImageButton ID="btnSearch" runat="server" ImageUrl="~/images/search-icon2.gif" 
                    ToolTip="Search" OnClick="btnSearch_Click" ValidationGroup="SearchGroup" />--%>
		</div>
		<div class="litRecordCount">
                <asp:Literal ID="litRecords" runat="server">Total Records: 0</asp:Literal>
		</div>
    </asp:Panel>
    <div id="content">
		<div id="left">
			<h3><asp:Label ID="lblDetailName" runat="server" Text="Detail" /></h3>
			<lfr:SecureDetailsView ID="dvDetail" runat="server" AutoGenerateRows="False"
				DataKeyNames="ListManagerID" DataSourceID="sdsDetail"
				DesiredDelete="True" DesiredInsert="True" DesiredUpdate="True"
				OnDataBound="DetailsView_DataBound" OnItemInserted="DetailsView_ItemInserted">
				<FieldHeaderStyle Width="220px" />
				<Fields>
					<asp:CommandField ShowEditButton="true" ShowInsertButton="true" ButtonType="Button" 
						ControlStyle-CssClass="command_button" />

                    <lfr:BetterBoundField DataField="ListKey" HeaderText="List Key" MaxLength="128"
                        Required="true" RequiredErrorMessage="Enter a value for 'List Key'"
                        ControlStyle-Width="95%"></lfr:BetterBoundField>

                    <lfr:BetterBoundField DataField="ListValue" HeaderText="List Value" MaxLength="128"
						Required="true" RequiredErrorMessage="Enter a value for 'List Value'"
                        ControlStyle-Width="95%" />

                    <lfr:BetterBoundField DataField="ListValueDescription" HeaderText="List Value Description" MaxLength="128"
                        ControlStyle-Width="95%" />
					
					<lfr:BetterBoundField DataField="ModifiedDate" HeaderText="Modified Date" MaxLength="32" 
                        HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ApplyFormatInEditMode="true"
                        ReadOnly="true" InsertVisible="false" ControlStyle-Width="45%" />
                        
					<asp:CommandField ShowDeleteButton="true" ButtonType="Button" 
						ControlStyle-CssClass="command_button" />
				</Fields>
			</lfr:SecureDetailsView>
		</div>
	</div>	
	<div id="right">
		<br />
		<asp:Panel ID="pnlInstructions" runat="server" Width="100%" GroupingText="Need Help?">
		<div style="padding:12px 8px 12px 8px;">
			To add a new value, click on the "New" button to the left. 
			Enter a name for it and click "Insert" to save your changes. Enter a description as an internal
			reminder for the item. The description will not appear anywhere else in IDEA.
			Once you enter the new information and save the changes, the item will appear on 
			all dropdowns associated with these values. Use the "Edit" button to rename a value or to update
			a description. Click the "Update" button to save your changes. Click the "Cancel" button at any time 
			to leave a form without saving the changes.			
		</div>
		</asp:Panel>
	</div>	

    <asp:SqlDataSource ID="sdsMain" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
			SELECT ListManagerID, ListKey, ListValue, ListValueDescription, ModifiedDate
			FROM ListManager
			WHERE Category = @Category
			ORDER BY ListValue
		" OnSelected="DisplayTotalRecords">
		<SelectParameters>
			<asp:QueryStringParameter Name="Category" QueryStringField="category" />
		</SelectParameters>
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="sdsDetail" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
			SELECT [ListManagerID], ListKey, ListValue, ListValueDescription, ModifiedDate
			FROM ListManager
			WHERE ListManagerID = @ListManagerID
		"
		DeleteCommand="
			DELETE FROM ListManager
			WHERE ListManagerID = @ListManagerID
		" 
		UpdateCommand="
		    UPDATE ListManager SET
                ListKey = @ListKey,
				ListValue = @ListValue,
				ListValueDescription = @ListValueDescription,
				ModifiedDate = GETDATE()
			WHERE ListManagerID = @ListManagerID
		"
		InsertCommand="
		    INSERT INTO ListManager
				(Category, ListKey, ListValue, ListValueDescription)
		    SELECT @Category, @ListKey, @ListValue, @ListValueDescription
            WHERE NOT EXISTS (
                SELECT 1 FROM ListManager
                WHERE Category = @Category
                AND ListKey = @ListKey
                AND ListValue = @ListValue 
            )
		    ; SELECT @ID = SCOPE_IDENTITY()
		" OnInserted="sdsDetail_Inserted" OnUpdated="RefreshList" OnDeleted="RefreshList">
		<SelectParameters>
			<asp:ControlParameter Name="ListManagerID" ControlID="gvMain" PropertyName="SelectedValue" />
		</SelectParameters>
		<DeleteParameters>
			<asp:ControlParameter Name="ListManagerID" ControlID="gvMain" PropertyName="SelectedValue" />
		</DeleteParameters>
		<UpdateParameters>
            <asp:Parameter Name="ListKey" />
			<asp:Parameter Name="ListValue" />
			<asp:Parameter Name="ListValueDescription" />
			<asp:ControlParameter Name="ListManagerID" ControlID="dvDetail" PropertyName="SelectedValue" />
		</UpdateParameters>
		<InsertParameters>
			<asp:Parameter Name="ID" Direction="Output" Type="int32" />
            <asp:QueryStringParameter Name="Category" QueryStringField="category" />
            <asp:Parameter Name="ListKey" />		
			<asp:Parameter Name="ListValue" />
			<asp:Parameter Name="ListValueDescription" />
		</InsertParameters>
	</asp:SqlDataSource>

</asp:Content>


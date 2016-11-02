<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="reviewdata.aspx.cs"
	Inherits="reviewdata" EnableEventValidation="false" Title="Review Data" StylesheetTheme="Default"
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
                <asp:DropDownList 
                    ID="ddlCategory" 
                    runat="server"
                    DataSourceID="xdsUploadCategory"
                    DataValueField="ID"
                    DataTextField="DisplayName"
                    AppendDataBoundItems="true"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">                            
				    <asp:ListItem Value="">Please select...</asp:ListItem>							
			    </asp:DropDownList>
			</td>
		</tr>
	</table>
	
	<asp:MultiView ID="mvMain" runat="server">
	<asp:View ID="vwLogData" runat="server">
		<lfr:SecureGridView ID="gvLogData" runat="server" AutoGenerateColumns="False" Width="100%"
			DataSourceID="ldsPumpLogger" DataKeyNames="WellID, WellLabel, Category, LogDate"
			EmptyDataText="There are no files to display." PageSize="25" AllowPaging="true" 
            onprerender="gv_PreRender">
		<Columns>
			<asp:CommandField ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/Images/select_button.gif" ItemStyle-Width="4%" />
			<asp:BoundField DataField="WellID" SortExpression="WellID" HeaderText="Well ID" ItemStyle-Width="6%" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="WellLabel" SortExpression="WellLabel" HeaderText="Well Label" ItemStyle-Width="9%" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="Category" SortExpression="Category" HeaderText="Measurement Type" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="LogDate" SortExpression="LogDate" HeaderText="Log Date" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="LogValue" SortExpression="LogValue" HeaderText="Value" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Right" />
            <asp:BoundField DataField="Units"   SortExpression="Units" HeaderText="Units" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="UploadedBy" SortExpression="UploadedBy" HeaderText="Uploaded By" ItemStyle-Width="11%" ItemStyle-HorizontalAlign="Center" />
			<asp:BoundField DataField="ModifiedDate" SortExpression="ModifiedDate" HeaderText="Upload Date" ItemStyle-HorizontalAlign="Center"
				HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ItemStyle-Width="16%" />
		</Columns>
		</lfr:SecureGridView>

        <asp:LinqDataSource
            ID="ldsPumpLogger"
            runat="server"
            ContextTypeName="Data_Access.PumpLogDataContext"
            TableName="PumpLogDataItems"
            Select="new(WellID, WellLabel, Category, LogDate, LogValue, Units, UploadedBy, ModifiedDate)"
            AutoPage="true"
            AutoSort="true">
        </asp:LinqDataSource>                       
		<br />
<%--		
		<asp:Panel ID="pnlSearchLogData" runat="server">
			<table border="0" cellpadding="0" cellspacing="3" width="100%">
			<tr>
				<td width="25%">
				</td>
				<td width="75%">
					Search:
					<asp:TextBox ID="txtSearchLogData" runat="server" Width="75%" SkinID="Search"></asp:TextBox> 
					<asp:ImageButton ID="btnSearchLogData" runat="server" ImageUrl="~/images/search-icon2.gif" 
						ToolTip="Search" OnCommand="btnSearch_Click" CommandName="LogData" />
				</td><%--
				<td width="30%" align="right">
					<asp:Literal ID="litRecordsGreenCleaning" runat="server">Total Records: 0</asp:Literal>
				</td>
			</tr>
			</table>
		</asp:Panel>--%>

		<br />
    <%-- 
		<asp:Panel ID="pnlGreenCleaningDetail" runat="server" Width="100%">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr valign="top">
				<td width="60%">
					<h3><asp:Label ID="lblGreenCleaningDetail" runat="server" Text="Detail" /></h3>
					<lfr:SecureDetailsView ID="dvGreenCleaningDetail" runat="server" DataSourceID="sdsGreenCleaningDetail"
						DataKeyNames="UpGreenCleaningID" AutoGenerateRows="False" Width="100%">
					<FieldHeaderStyle Width="200px" />
					<Fields>
						<asp:CommandField />
						<lfr:BetterBoundField DataField="MailCode" HeaderText="Mail Code" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="VendorName" HeaderText="Vendor Name" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="ProductType" HeaderText="Product Type" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="ProductUse" HeaderText="Product Use" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="Manufacturer" HeaderText="Manufacturer" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="ManufacturerURL" HeaderText="Link to Manufacturer Website" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="Model" HeaderText="Model/Product Name" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="ReportingDate" HeaderText="Reporting Date (MM/DD/YYYY)" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ApplyFormatInEditMode="true" ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="ReportingFrequency" HeaderText="Reporting Frequency" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="LEEDCompliant" HeaderText="LEED Compliant?" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="LEEDCriteriaMet" HeaderText="LEED Criteria Met" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="TotalCost" HeaderText="Total Cost" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="DatePurchased" HeaderText="Date Purchased (Equipment)" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ApplyFormatInEditMode="true" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="QuantityUsed" HeaderText="Quantity On Hand (Equipment)" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="PostConsumerContent" HeaderText="Post-Consumer Content (%) (Paper)" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="PreConsumerContent" HeaderText="Pre-Consumer Content (%) (Paper)" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="ProductTypeNotes" HeaderText="Product Type Notes" ControlStyle-Width="95%"/>
						<lfr:BetterBoundField DataField="Source" HeaderText="Source" MaxLength="255" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="UploadedBy" HeaderText="Uploaded By" MaxLength="128" 
							ControlStyle-Width="95%" />
						<lfr:BetterBoundField DataField="ModifiedDate" HeaderText="Upload Date" MaxLength="32"
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
--%>
	</asp:View>
    <asp:View ID="vwTideData" runat="server">
        <lfr:SecureGridView ID="gvTideData" runat="server" 
            AutoGenerateColumns="False" Width="100%"
			    DataSourceID="ldsTideData" DataKeyNames="TideDataID"
			    EmptyDataText="There is no data to display." PageSize="25" AllowPaging="true" 
            onprerender="gv_PreRender">
		    <Columns>
			    <asp:CommandField ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/Images/select_button.gif" ItemStyle-Width="4%" />
			     <asp:BoundField DataField="Date_Time" SortExpression="Date_Time" HeaderText="Date Time" ItemStyle-HorizontalAlign="Center"
				    HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ItemStyle-Width="16%" />
			    <asp:BoundField DataField="Height" SortExpression="Height" HeaderText="Height" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Right"
                    DataFormatString="{0:N1}" />
			    <asp:BoundField DataField="UploadedBy" SortExpression="UploadedBy" HeaderText="Uploaded By" ItemStyle-Width="11%" ItemStyle-HorizontalAlign="Center" />
			    <asp:BoundField DataField="ModifiedDate" SortExpression="ModifiedDate" HeaderText="Upload Date" ItemStyle-HorizontalAlign="Center"
				    HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy hh:mm}" ItemStyle-Width="16%" />
		    </Columns>
		</lfr:SecureGridView>

        <asp:LinqDataSource
            ID="ldsTideData"
            runat="server"
            ContextTypeName="Data_Access.PumpLogDataContext"
            TableName="TidalPredictionItems"
            AutoPage="true"
            AutoSort="true">
        </asp:LinqDataSource>
		<br />
    </asp:View>
	</asp:MultiView>

	
	<asp:SqlDataSource ID="sdsPumpLogData" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
		SelectCommand="
		SELECT [WellID]
              ,[WellLabel]
              ,[Category]
              ,[LogDate]
              ,[LogValue]
              ,[Units]
              ,[ModifiedDate]
              ,[Source]
              ,[UploadedBy]
              ,[UploadID]
          FROM [VancouverLogData]
          ORDER BY
               [WellID]
              ,[WellLabel]
              ,[Category]
              ,[LogDate]
	    ">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="sdsPumpLogDataDetail" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
	    OnInserting="test"
		SelectCommand="
		SELECT [WellID]
              ,[WellLabel]
              ,[Category]
              ,[LogDate]
              ,[LogValue]
              ,[Units]
              ,[ModifiedDate]
              ,[Source]
              ,[UploadedBy]
              ,[UploadID]
          FROM [VancouverLogData]
            WHERE [WellID] = @WellID
            AND [WellLabel] = @WellLabel
            AND [Category] = @Category
            AND [LogDate] = @LogDate
	    " OnSelecting="sdsPumpLogData_Selecting">
	<SelectParameters>
		<asp:Parameter Name="WellID" DbType="Int32" />
        <asp:Parameter Name="WellLabel" DbType="String" />
        <asp:Parameter Name="Category" DbType="String" />
        <asp:Parameter Name="LogDate" DbType="DateTime" />
	</SelectParameters>
	</asp:SqlDataSource>

    <asp:SqlDataSource ID="sdsTideData" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
        	SELECT [TideDataID]
                  ,[DateTime]
                  ,[Height]
                  ,[ModifiedDate]
                  ,[Source]
                  ,[UploadedBy]
                  ,[UploadID]
              FROM [VancouverTideData]
              ORDER BY
                   [TideDataID]
                  ,[DateTime]">        
    </asp:SqlDataSource>

     <asp:SqlDataSource ID="sdsTextSearch" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
        SelectCommand="
		    SELECT [WellID]
                  ,[WellLabel]
                  ,[Category]
                  ,[LogDate]
                  ,[LogValue]
                  ,[Units]
                  ,[ModifiedDate]
                  ,[Source]
                  ,[UploadedBy]
                  ,[UploadID]
              FROM [VancouverLogData]
            ORDER BY WellID, LogDate DESC
        "
        FilterExpression="UploadedBy LIKE '%{0}%' OR Source LIKE '%{0}%'">
    <FilterParameters>
		<asp:Parameter Name="UploadedBy" />
		<asp:Parameter Name="Source" />
    </FilterParameters>
    </asp:SqlDataSource>

    <asp:XmlDataSource 
        ID="xdsUploadCategory" 
        runat="server" 
        DataFile="~/App_Data/XML/schema.xml" 
        XPath="Root/Table">
    </asp:XmlDataSource>
</asp:Content>


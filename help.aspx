<%@ Page Language="C#" AutoEventWireup="true" CodeFile="help.aspx.cs" Inherits="help" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Help</title>
	<style type="text/css">
	    body {
	        margin:15px 15px 15px 15px;
	    	background-color:#ECE9D8;
	    }
	    em {
	        font-style:normal;
	        font-weight:normal;
	        background-color:white;
	        padding:1px 5px 2px 4px;
	        border:solid 1px #666;
	        
	    }
	</style>
</head>
<body>
	<form id="form1" runat="server">
		<div id="content">
		    <table width="100%">
		    <tr>
		        <td width="70%">
			        <b>How do I view events?</b><br />
			        <br />
			        Scroll the screen to view the next 10 working days. To see a different date range,
			        click the calendar icon <img border="0" src="images/calendar_icon.gif" /> in the 
			        top left corner and pick a new start date. Events
			        appear on the calendar as white boxes with a colored bar on the left. Hover your
			        mouse cursor over an event to see more information. Clicking the event will load
			        the detail panel. To close the panel without making changes, click the <em>Cancel</em>
			        button.<br />
			        <br />
		        </td>
		        <td width="30%">
			        <table width="100%">
		            <tr valign="middle">
		                <td align="right">
    		                Field
		                </td>
		                <td>
		                    <img src="images/help_event_field.gif" />
		                </td>
		            </tr>
		            <tr valign="middle">
		                <td align="right">
    		                Office
		                </td>
		                <td>
		                    <img src="images/help_event_office.gif" />
		                </td>
		            </tr>
		            <tr valign="middle">
		                <td align="right">
    		                Vacation
		                </td>
		                <td>
		                    <img src="images/help_event_vacation.gif" />
		                </td>
		            </tr>
			        </table>
		        </td>
		    </tr>
		    </table>
			
			<br /><hr /><br />
			<b>How do I edit an event?</b><br />
			<br />
			Locate the event you want to edit and click on it. This will load the detail panel.
			Here you can make the changes you want. Click the <em>Update</em> button, or click
			<em>Cancel</em> to close the panel without making changes.<br />
			<br /><hr /><br />
			<b>How do I enter a new event?</b><br />
			<br />
			Click on an empty cell <img src="images/help_cell.gif" /> to add a new event. 
			The date, time, and staff name will be set to the cell you clicked on.
			Enter the event information and click the <em>Update</em> button,
			or the <em>Cancel</em> button to exit without saving.<br />
			<br /><hr /><br />
			<b>How do I delete an event?</b><br />
			<br />
			Locate the event you want to delete and click on it. This will load the detail panel.
			Click the <em>Delete</em> button to permanently remove the event from the database.
			You will be prompted to make sure you really want to delete. Click <em>OK</em> to
			confirm, or click <em>Cancel</em> to exit without deleting.<br />
			<br /><hr /><br />
			<b>How do I reassign an event to someone else?</b><br />
			<br />
			Click on an event to edit it. Use the drop-down list at the top of the detail panel
			to select a new staff member. Click the <em>Update</em> button, or click <em>Cancel</em>
			to close the panel without making changes.<br />
			<br /><hr /><br />
			<b>How do I reassign my workload to someone else?</b><br />
			<br />
			Better talk to your supervisor. I'm just a field schedule.<br />
			<br /><hr /><br />
			<b>How do I set up a recurring event?</b><br />
			<br />
			Click on an event to edit it. In the detail panel, check the Recurring Event box.
			This loads the recurring event panel. Choose from the recurring options here and
			click <em>Update</em> when finished, or <em>Cancel</em> to exit without saving. Use
			this process to change a single event to a recurring event, and vice-versa.<br />
			<br /><hr /><br />
			<b>How do I edit a recurring event?</b><br />
			<br />
			Clicking on a recurring event will ask you whether you want to <b>Open this occurrence</b>
			(one event) or <b>Open the series</b> (many events). If you choose the occurrence,
			your changes will only modify the event you clicked on. If you choose the entire
			series, all events will be changed. For example, if you want to move a regular event
			from Friday afternoon to Friday morning, select the <b>Open the series</b> option,
			change the start date and end date options if necessary, and click <em>Update</em>.
			You can make more changes as elaborate as you want. Keep in mind, however, that
			any change to the series will <b>override all previous changes</b> to the occurrences.<br />
			<br /><hr /><br />
			<b>Can I change the color scheme?</b><br />
			<br />
			Not now. Look for this feature in a future release.<br />
			<br /><hr /><br />
			<b>What browsers are compatible with this site?</b><br />
			<br />
			This site has been tested successfully on Internet Explorer 6, Firefox 1.5, and
			Opera 9 for Windows, as well as Safari 2.0 and Firefox 1.5 for the Macintosh. 
			It is not compatible with Netscape 4 or Lynx. It is absolutely <b>not</b> compatible with Mosaic.<br />
            <div align="center">
                <img src="images/mosaic.gif" />
            </div>
			<br /><hr /><br />
		</div>
        <div align="center">
            <input type="button" value="Close" onclick="window.close()" />
        </div>
	</form>
</body>
</html>

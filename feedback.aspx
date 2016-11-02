<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="feedback.aspx.vb" 
    Inherits="_feedback" Title="Web Portal | Feedback" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table width="100%" cellpadding="0" cellspacing="0">
    <tr valign="top">
        <td width="60%">
            <table width="100%" cellpadding="4">
            <tr>
                <td></td>
                <td style="text-align:center">Feedback</td>
            </tr>
            <tr>
                <td>Your E-Mail Address</td>
                <td><asp:TextBox ID="Email" runat="server" Width="300px" Enabled="false"></asp:TextBox></td>
            </tr>
            <tr>
                <td>URL</td>
                <td><asp:TextBox ID="URL" runat="server" Width="300px"></asp:TextBox></td>
            </tr>
            <tr>
                <td valign="top">Question</td>
                <td><asp:TextBox ID="Message" runat="server" Height="240px" TextMode="MultiLine" Width="318px"></asp:TextBox></td>
            </tr>
            <tr>
                <td></td>
                <td style="text-align:center"><asp:Button ID="Submit" runat="server" Text="Submit" Height="24px" /></td>
            </tr>
            </table>
            
            <br />
            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
        </td>
	    <td width="40%" align="center" style="padding:15px 0 0 15px;">
		    <asp:Panel ID="pnlInstructions" runat="server" Width="100%" GroupingText="Need Help?">
			    <div align="left" style="padding:10px 10px 10px 10px;">
			        This is a place to send questions and feedback to the development team.
			    </div>
		    </asp:Panel>
	    </td>
    </tr>
    
    </table>
</asp:Content>

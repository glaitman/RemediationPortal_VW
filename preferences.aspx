<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master"
	CodeFile="preferences.aspx.cs" Inherits="preferences" Title="Web Portal | Preferences" 
	MaintainScrollPositionOnPostback="true" %>
<%@ MasterType virtualpath="~/Masterpage.Master" %>

<%@ Register Assembly="SecureControls" Namespace="SecureControls" TagPrefix="lfr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table width="400" cellpadding="2" cellspacing="0" class="password_table">
    <tr>
        <td colspan="2" align="center" class="password_header">Change Your Password</td>
    </tr>
    <tr>
        <td colspan="2"><br /></td>
    </tr>
    <tr>
        <td align="right">Password: </td>
        <td width="200">
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" 
                ControlToValidate="txtPassword" ErrorMessage="Please enter a password." Text="*"
                EnableClientScript="true"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td align="right">New Password: </td>
        <td>
            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" 
                ControlToValidate="txtNewPassword" ErrorMessage="Please enter a new password." Text="*"
                EnableClientScript="true"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td align="right">Confirm New Password: </td>
        <td>
            <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" 
                ControlToValidate="txtNewPassword" ErrorMessage="Please confirm the new password." Text="*"
                EnableClientScript="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic"
                ControlToValidate="txtNewPassword" ControlToCompare="txtConfirm"
                ErrorMessage="New passwords don't match." Text="*"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <br />
            <asp:Button ID="btnChange" runat="server" Text="Change Password" OnClick="btnChange_Click" Height="24px" Width="130px" />
            <br /><br />
        </td>
    </tr>
    </table>
    <br />
    Theme: 
    <asp:DropDownList ID="ddlTheme" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTheme_SelectedIndexChanged">
        <asp:ListItem>Default</asp:ListItem>
    </asp:DropDownList>

    
    <asp:Label ID="lblMessage" runat="server"></asp:Label><br />
    <br />

    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
</asp:Content>
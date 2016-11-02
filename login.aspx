<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Web Portal</title>
</head>
<body>
<form id="form1" runat="server">
    <div class="top">
    </div>
    <div style="margin-top:15px;margin-left:1in;">
        <asp:Login ID="Login1" runat="server" TitleText="Please Log In"
			DisplayRememberMe="false" RememberMeSet="false" 
            OnAuthenticate="Login1_Authenticate" OnLoggedIn="Login1_LoggedIn" >
        </asp:Login>

    </div>
    
	<asp:SqlDataSource ID="sdsHomeDirectory" runat="server" ConnectionString="<%$ ConnectionStrings:MembershipProvider %>"
	SelectCommand="
	    select homedirectory
	    from membershipsite
	    where userid = @userid
	    and siteid = @siteid
	">
	<SelectParameters>
		<asp:SessionParameter Name="userid" SessionField="UserID" />
		<asp:SessionParameter Name="siteid" SessionField="SiteID" />
	</SelectParameters>
    </asp:SqlDataSource>


</form>
</body>
</html>

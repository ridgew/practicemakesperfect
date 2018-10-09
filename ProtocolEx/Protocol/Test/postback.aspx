<%@Page Language="C#"%>
<HTML>
<BODY BGCOLOR="Silver">
<form runat="server">
	<input type="text" runat="server" value="12" id="Data"><br>
	<input type="Submit" runat="server"><hr>
	<%= Request.Form %>
</form>
</BODY>
</HTML>
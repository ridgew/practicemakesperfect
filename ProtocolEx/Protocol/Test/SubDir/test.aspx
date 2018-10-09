<%@Page Language="C#"%>
<HTML>
<BODY BGCOLOR="Silver">
<DIV STYLE="color:green;">Hey There buddy. The Current Time is <%= DateTime.Now %></DIV><hr>
<a href="test2.aspx">Test2</a><br>
<a href="postback.aspx">Postback</a><br>
<%= Request.QueryString["g"] %><br>
<img src="newbkglogo_03.jpg"><hr>
<img src="http://localhost/images/newbkglogo_03.jpg"><hr>
<img src="403-3.gif"><hr>

</BODY>
</HTML>
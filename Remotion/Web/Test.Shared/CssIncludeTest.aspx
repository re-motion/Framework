<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CssIncludeTest.aspx.cs" Inherits="Remotion.Web.Test.CssIncludeTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
<style type="text/css">
    <%
      for (int i = 0; i < 64; i++)
      {
        Response.Write ("@import url('CssHandler.ashx?Class=Class"+(i+1)+"');");
        if (i%31 == 0)
        {
        Response.Write ("</style>\r\n<style type=\"text/css\">");
        }
      } %>
      </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <%
      for (int i = 0; i < 64; i++)
      {
        Response.Write (@"<div class=Class" + (i + 1) + ">Class" + (i + 1) + "</div>");
        Response.Write (@"<div class=Class" + (i + 1) + "a>Class" + (i + 1) + "a</div>");
      } %>
   
    </div>
    </form>
</body>
</html>

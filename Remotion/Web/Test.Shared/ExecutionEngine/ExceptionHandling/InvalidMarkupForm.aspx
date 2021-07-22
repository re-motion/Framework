<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvalidMarkupForm.aspx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.ExceptionHandling.InvalidMarkupForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Invalid Markup Form</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:Literal runat="server">Will result in the following error message: "The Controls collection cannot be modified because the control contains code blocks (i.e. &lt;% ... &gt;%)."</asp:Literal>
      <% Response.Write ("Fail!"); %>
    </div>
    </form>
</body>
</html>

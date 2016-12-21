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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DropDownMenu.aspx.cs" Inherits="Remotion.Web.Test.DropDownMenu" %>
<% %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <remotion:HtmlHeadContents ID="HeadContents" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
    <tr>
    <td><remotion:DropDownMenu ID="EditableEnabled" runat="server" TitleText="" /></td>
    </tr>
    <tr>
    <td>
    <span style="display:inline-block; border:solid 1px black"><a href="#"></a><span>X</span></span>
    </td>
    </tr>
    </table>
    </div>
    </form>
</body>
</html>

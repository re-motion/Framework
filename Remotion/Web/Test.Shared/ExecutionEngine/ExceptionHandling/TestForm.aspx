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

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestForm.aspx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.ExceptionHandling.TestForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="TheForm" runat="server">
    <div>
      <p>
        <asp:Button runat="server" ID="ThrowExceptionButton" OnClick="ThrowExceptionButton_Click" Text="Throw Exception" />
      </p>
      <p>
        <asp:Button runat="server" ID="ThrowHttpExceptionButton" OnClick="ThrowHttpExceptionButton_Click" Text="Throw HttpException" />
      </p>
      <p>
        <asp:Button runat="server" ID="ThrowExceptionFromSubFunctionButton" OnClick="ThrowExceptionFromSubFunctionButton_Click" Text="Throw Exception from SubFunction" />
      </p>
      <p>
        <asp:Button runat="server" ID="ThrowExceptionForMissingPageButton" OnClick="ThrowExceptionForMissingPageButton_Click" Text="Throw Exception for missing page" />
      </p>
      <p>
        <asp:Button runat="server" ID="ThrowExceptionForMissingUserControlButton" OnClick="ThrowExceptionForMissingUserControlButton_Click" Text="Throw Exception for missing user control" />
      </p>
      <p>
        <asp:Button runat="server" ID="ThrowExceptionForInvalidMarkupButton" OnClick="ThrowExceptionForInvalidMarkupButton_Click" Text="Throw Exception for page containing invalid markup" />
      </p>
      <p>
        <asp:Button runat="server" ID="OpenSubFunctionButton" OnClick="OpenSubFunctionButton_Click" Text="Open SubFunction" />
      </p>
    </div>
    <div>
      <asp:Literal runat="server" ID="Stack"></asp:Literal>
    </div>
  </form>
</body>
</html>

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

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestForm.aspx.cs" Inherits="Remotion.Web.Test.ErrorHandling.TestForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Test Form</title>
</head>
<body>
  <form id="TheForm" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager" EnablePartialRendering="True" />

<%--    <script type="text/javascript">
      Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
      function EndRequestHandler(sender, args)
      {
        if (args.get_error() != undefined && args.get_error().httpStatusCode == '500')
        {
          var errorMessage = args.get_error().message;
          args.set_errorHandled(true);
          var errorSection = document.getElementById('ErrorSection');
          errorSection.innerHTML = errorMessage;
          errorSection.style.display = 'block';
        }
      }
    </script>--%>

    <div>
      <asp:HyperLink runat="server" NavigateUrl="TestForm.aspx">Restart</asp:HyperLink>
      <br />
      <asp:Button runat="server" ID="SynchronousPostbackErrorButton" Text="Throw synchronous exception" OnClick="SynchronousPostbackErrorButton_Click" />
      <br />
      <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
          <asp:Button runat="server" ID="AsynchronousPostbackErrorButton" Text="Throw asynchronous exception" OnClick="AsynchronousPostbackErrorButton_Click" />
          <br />
          <asp:Button runat="server" ID="AsynchronousPostbackButton" Text="Asynchronous postback" />
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
  </form>
  <div id="ErrorSection" style="display: none; position: fixed; top: 0; left: 0; bottom: 0; right: 0; background: white;"></div>
</body>
</html>

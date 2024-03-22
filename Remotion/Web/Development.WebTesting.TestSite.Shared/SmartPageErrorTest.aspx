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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/SmartPageErrorTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.SmartPageErrorTest" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<html lang="en">
<head>
    <title>SmartPage Error Test</title>
    <remotion:htmlheadcontents id="HtmlHeadContents" runat="server" />
    <style>
      body
      {
        padding: 0;
        margin: 0;
        width: 100%;
        height: 100%;
      }
    </style>
  </head>
<body>
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" AsyncPostBackTimeout="3600" runat="server"/>
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <label for="statusCode">StatusCode:</label>
            <asp:TextBox runat="server" ID="statusCode"></asp:TextBox>
            <br/>
            <asp:Button ID="TriggerAsyncResponseWithPartialRenderingErrorButton" runat="server" OnClick="AsyncOnClick" Text="Error response with partial rendering result"></asp:Button>
            <br/>
            <asp:Button ID="TriggerAsyncResponseWithFullPageRenderingErrorButton" Text="Error response with full rendering result" runat="server"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</form>
</body>
</html>

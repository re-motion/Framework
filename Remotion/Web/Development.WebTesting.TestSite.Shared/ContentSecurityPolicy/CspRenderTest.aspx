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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/ContentSecurityPolicy/CspRenderTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.ContentSecurityPolicy.CspRenderTest" %>
<%@ Register TagPrefix="app" Namespace="Remotion.Web.Development.WebTesting.TestSite.Shared.ContentSecurityPolicy" Assembly="Remotion.Web.Development.WebTesting.TestSite.Shared" %>

<html xmlns:asp="http://www.w3.org/1999/html">
<head runat="server">

</head>
<body>

<h1>CSP Render test</h1>

<p>
 This test page tests that our CSP inline script and inline event handler logic works correctly.
 It also tests that async postbacks work with CSP.
</p>

<p>
 The following output is filled with string constants by different scripts.
 Depending on CSP, some of the scripts might get blocked and thus do not appear in the output.
</p>

<b>Output:</b>
<div id="output">

</div>

<!-- Log for disposes -->
<b>Dispose Output:</b>
<div id="disposeLog">
</div>

<p>
 The first time is only updated on sync postbacks,
 while the second time is updated on async postbacks initiated by the button below.
 This is used to trigger an
</p>

<div id="syncDate"><%= DateTime.Now.TimeOfDay.ToString("g") %></div>

<form runat="server" id="form1">
 <asp:ScriptManager runat="server" EnablePartialRendering="true" />
 <asp:UpdatePanel runat="server">
  <ContentTemplate>
   <app:CspRenderTestControl runat="server"/>
   <div id="asyncDate"><%= DateTime.Now.TimeOfDay.ToString("g") %></div>
   <asp:button id="asyncPostback" Text="Async postback" runat="server" />
   <app:CspRenderTestControlWithDispose runat="server"/>
  </ContentTemplate>
 </asp:UpdatePanel>
</form>

<!-- Triggers inline event -->
<img src="" onerror="document.getElementById('output').append('; INLINE EVENT HANDLER')" />

<!-- Triggers inline script -->
<script>
document.getElementById("output").append("; INLINE SCRIPT");
</script>

</body>
</html>

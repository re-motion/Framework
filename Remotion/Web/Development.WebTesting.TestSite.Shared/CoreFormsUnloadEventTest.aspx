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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CoreFormsUnloadEventTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.CoreFormsUnloadEventTest" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="CoreForms.Web.Extensions, Version=10.0.2.0, Culture=neutral, PublicKeyToken=4775604ccf360c09" %>

<html>
<head runat="server">

</head>
<body>

<h1>CoreForms unload event test</h1>

<p>
 This test page tests that SmartPage.EnableCoreFormsClientScriptUnloadEvent correctly toggles
 whether CoreForms uses the (deprecated) unload event for its client script dispose logic.
</p>

<!-- Changes on every request; used to detect that a postback has completed and the page has reloaded. -->
<div id="loadMarker"><%= DateTime.Now.Ticks %></div>

<!-- Filled in by the readAndClearUnloadMarker startup script with 'true' or 'false'. -->
<b>Unload event result:</b>
<div id="unloadEventResult"></div>

<form runat="server" id="form1">
 <asp:ScriptManager runat="server" />
 <asp:button id="PostBackButton" Text="PostBack" runat="server" />
</form>

<script nonce="testsite">
const storageKey = "__unloadFired";
document.getElementById("unloadEventResult").textContent = String(window.localStorage.getItem(storageKey) !== null);
window.localStorage.removeItem(storageKey);

let isPostBack = false;
document.getElementById("PostBackButton").addEventListener("click", () => {
    isPostBack = true;
});

// Creating a component is the easiest way to get a disposing script without any async postback stuff.
// The dispose script will only run if the ASP.NET Web Forms infrastructure has the unload event enabled.
const component = new window.Sys.Component();
component.add_disposing(() => {
    if (isPostBack) {
        localStorage.setItem(storageKey, true);
    }
});
</script>

</body>
</html>

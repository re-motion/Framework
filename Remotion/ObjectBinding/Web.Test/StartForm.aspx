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
<%@ Page Trace="false" language="c#" Codebehind="StartForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.StartForm" %>
<%@ Import Namespace="Remotion" %>
<%@ Import Namespace="Remotion.ServiceLocation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
    <title>Start Form</title>
    <remotion:htmlheadcontents id="HtmlHeadContents" runat="server" />
<script type="text/javascript">
  function OpenClientWindow(url)
  {
    var clientWindow = window.open(url, 'ClientWindow', 'menubar=yes,toolbar=yes,location=yes,status=yes');
  }
</script>

  </head>
<body>
<form id="TheForm" method="post" runat="server">
<p>Wxe-Enabled Tests for individual Business Object Controls<br />
<a href="IndividualControlTest.wxe?UserControl=BocListUserControl.ascx&WxeReturnToSelf=True&TabbedMenuSelection=IndividualControlTests,BocList">IndividualControlTest.wxe</a></p>

<p>Wxe-Enabled Tests containing all the Business Object 
Controls in a single Form or User Control<br /><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest" >WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest</A></p>
<p>Wxe-Enabled Test for a Tabbed Form<br /><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&amp;ReadOnly=false" >WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&amp;ReadOnly=false</A></p>
<p>Wxe-Enabled Tests for individual Business Object Controls wrapped in an iframe<br /><A href="IFrameContainer.html" >IFrameContainer</A></p>
<p>Test Tree View<br /><A href="SingleTestTreeView.aspx" >SingleTestTreeView.aspx</A></p>
 <p><A href="javascript:OpenClientWindow ('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=false');" >OpenClientWindow 
('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=false')</A></p>
<p><a href="javascript:OpenClientWindow ('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=true');" >OpenClientWindow 
('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=true')</a></p>
<p><a href="javascript:OpenClientWindow ('ClientFormFrameset.htm');">OpenClientWindow ('ClientFormFrameset.htm')</a></p>
<p>Design Test<br /><a 
href="WxeHandler.ashx?WxeFunctionType=OBWTest.Design.DesignTestFunction,OBWTest">WxeHandler.ashx?WxeFunctionType=OBWTest.Design.DesignTestFunction,OBWTest</a></p>
<p>Tests for SingleView and TabbedMultiView Layout<br />
<a href="ViewLayoutTest.wxe?WxeReturnToSelf=True&TabbedMenuSelection=ViewLayoutTests">ViewLayoutTest.wxe</a></p>
<p>Tests for layout compatibility between the different controls<br />
<a href="ControlLayoutTest.wxe?WxeReturnToSelf=True&TabbedMenuSelection=ViewLayoutTests">ControlLayoutTest.wxe</a></p>
<p>Tests for reference data sources<br />
<a href="ReferenceDataSourceTest.wxe?WxeReturnToSelf=True">ReferenceDataSourceTest.wxe</a></p>
<p>BOC-validation tests<br /><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.Validation.BocValidationTestWxeFunction,OBWTest" >WxeHandler.ashx?WxeFunctionType=OBWTest.Validation.BocValidationTestWxeFunction,OBWTest</A></p>
  <ul>
  <% foreach (var inst in SafeServiceLocator.Current.GetAllInstances<ITestInterface>())
     { %>
      <li><%= inst.GetOutput() %>
  <% } %>
  </ul>
</form>
  </body>
</html>

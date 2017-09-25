﻿<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
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
<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Web.Development.WebTesting.TestSite</title>
  </head>
  <body>
    <form id="HtmlForm" runat="server">
      <div>
        <label>Specific tests</label>
        <table style="margin-bottom: 20px">
          <tr><td><a href="AnchorTest.wxe">AnchorTest</a></td></tr>
          <tr><td><a href="CommandTest.wxe">CommandTest</a></td></tr>
          <tr><td><a href="DropDownListTest.wxe">DropDownListTest</a></td></tr>
          <tr><td><a href="DropDownMenuTest.wxe">DropDownMenuTest</a></td></tr>
          <tr><td><a href="Empty.aspx">Empty</a></td></tr>
          <tr><td><a href="FileDownloadTest.aspx">FileDownloadTest</a></td></tr>
          <tr><td><a href="FormGridTest.aspx">FormGridTest</a></td></tr>
          <tr><td><a href="ImageTest.aspx">ImageTest</a></td></tr>
          <tr><td><a href="ImageButtonTest.wxe">ImageButtonTest</a></td></tr>
          <tr><td><a href="InfrastructureTests.wxe">InfrastructureTests</a></td></tr>
          <tr><td><a href="MouseTest.aspx">MouseTest</a></td></tr>
          <tr><td><a href="LabelTest.aspx">LabelTest</a></td></tr>
          <tr><td><a href="ListMenuTest.wxe">ListMenuTest</a></td></tr>
          <tr><td><a href="ScopeTest.aspx">ScopeTest</a></td></tr>
          <tr><td><a href="ElementScopeTest.wxe">ElementScopeTest</a></td></tr>
          <tr><td><a href="ScreenshotTest.wxe">ScreenshotTest</a></td></tr>
          <tr><td><a href="ScrollTest.aspx">ScrollTest</a></td></tr>
          <tr><td><a href="SingleViewTest.aspx">SingleViewTest</a></td></tr>
          <tr><td><a href="TabbedMenuTest.wxe">TabbedMenuTest</a></td></tr>
          <tr><td><a href="TabbedMultiViewTest.wxe">TabbedMultiViewTest</a></td></tr>
          <tr><td><a href="TextBoxTest.wxe">TextBoxTest</a></td></tr>
          <tr><td><a href="TreeViewTest.wxe">TreeViewTest</a></td></tr>
          <tr><td><a href="WebButtonTest.wxe">WebButtonTest</a></td></tr>
          <tr><td><a href="WebTabStripTest.wxe">WebTabStripTest</a></td></tr>
          <tr><td><a href="WebTreeViewTest.aspx">WebTreeViewTest</a></td></tr>
          <tr><td><a href="MultiWindowTest/Main.wxe">MultiWindowTest</a></td></tr>
          <tr><td><a href="PlatformTest.wxe">PlatformTest</a></td></tr>
          <tr><td><a href="BrowserSessionTest.wxe">BrowserSessionTest</a></td></tr>
        </table>
        
        <label>Generic tests</label>
        <table>
          <tr><td><a href="GenericTest.wxe?control=anchor">AnchorTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=command">CommandTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=dropDownList">DropDownListTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=dropDownMenu">DropDownMenuTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=formGrid">FormGridTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=imageButton">ImageButtonTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=image">ImageTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=label">LabelTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=listMenu">ListMenuTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=scope">ScopeTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=singleView">SingleViewTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=tabbedMenu">TabbedMenuTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=tabbedMultiView">TabbedMultiViewTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=textBox">TextBoxTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=treeView">TreeViewTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=webButton">WebButtonTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=webTabStrip">WebTabStripTest</a></td></tr>
          <tr><td><a href="GenericTest.wxe?control=webTreeView">WebTreeViewTest</a></td></tr>
        </table>
      </div>
    </form>
  </body>
</html>
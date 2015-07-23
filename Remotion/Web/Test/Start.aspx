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
<%@ Page language="c#" Codebehind="Start.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Web.Test.Start" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Start</title>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SampleWxeFunction">
          WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SampleWxeFunction
        </a>
      </p>
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SessionWxeFunction&amp;ReadOnly=True">
          WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SessionWxeFunction&amp;ReadOnly=True
        </a>
      </p>
      <p>
        <a href="ExecutionEngine/RedirectHandler.ashx?RedirectTo=~/ShowUserControl.wxe" target="newWindow">
          RedirectTo ShowUserControl.wxe
        </a>
      </p>
      <p>
        <a href="session.wxe?ReadOnly=True">session.wxe?ReadOnly=True</a>
      </p>
      <p>
        <a href="MultiplePostbackCatching/SutForm.aspx">Multiple Postback Catcher Tests</a>
      </p>
      <p>
        <a href="MultiplePostbackCatching/UpdatePanelSutForm.aspx">Multiple Postback Catcher Tests for Update Panel</a>
      </p>
      <p>
        <a href="UpdatePanelTests/Sut.wxe">UpdatePanel SUT</a>
      </p>
      <p>
        <a href="../../../prereq/Tools/Selenium/Core/TestRunner.html?test=../../../../Remotion/Web/Test/MultiplePostbackCatching/TestSuiteForm.aspx">Remotion.Web.Tests.MultiplePostBackCatching</a>
      </p>
      <p>
        <a href="../../../prereq/Tools/Selenium/Core/TestRunner.html?test=../../../../Remotion/Web/Test/MultiplePostbackCatching/UpdatePanelTestSuiteForm.aspx">Remotion.Web.Tests.MultiplePostBackCatching for UpdatePanel</a>
      </p>
      <p>
        <a href="PostBackFocus.aspx">PostBackFocus.aspx</a>
      </p>      
      <p>
        <a href="redirected.wxe">redirected.wxe</a>
      </p>
      <p>
        <a href="ShowUserControl.wxe">ShowUserControl.wxe</a>
      </p>      
      <p>
        <a href="WebButtonDisabledCssTest.aspx">WebButtonDisabledCssTest.aspx</a>
      </p>     
      <h3>Theming</h3>
      <p>
        <a href="Theming/SingleViewTest.aspx">SingleView</a>
        <a href="Theming/TabbedMultiViewTest.aspx">Tabbed MultiView</a>
      </p>
      <h3>Error Handling</h3>
      <p>
        <a href="ErrorHandling/TestForm.aspx">Test for handling errors in synchronous and asynchronous requests</a>
      </p>
      <p>
        <a href="ExceptionHandlingTest.wxe">Test for propagtion of exceptions thrown on WXE Pages</a>
      </p>
      <p>
        Current functions: <asp:Label runat="server" ID="CurrentFunctionCountLabel" Text="###"/>
      </p>
      <asp:Button id="ResetSessionButton" runat="server" Text="Reset Session" OnClick="ResetSessionButton_Click"></asp:Button>
    </form>
  </body>
</html>

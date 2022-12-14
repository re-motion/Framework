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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Frame.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Frame" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>MyFrame</title>
  </head>
  <body>
    <form id="form1" runat="server">
      <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" AsyncPostBackTimeout="3600" runat="server" />
      <div>
        <asp:UpdatePanel ID="UpdatePanel" UpdateMode="Always" runat="server">
          <ContentTemplate>
            <p>Frame.aspx running in FrameFunction</p>
            <p><asp:Label ID="FrameLabel" Text="FrameLabel" ViewStateMode="Disabled" runat="server" /></p>
            <p><remotion:WebButton ID="SimplePostBack" Text="Simple PostBack" runat="server"/></p>
            <p><remotion:WebButton ID="NextStep" Text="Next step (= finish function => destroys frame!)" runat="server"/></p>
            <p><remotion:WebButton ID="LoadWindowFunctionInNewWindow" Text="Load WindowFunction in new Window" runat="server"/></p>
            <p><remotion:WebButton ID="RefreshMainUpdatePanel" Text="Refresh Main UpdatePanel" runat="server"/></p>
            <p><testsite:TestEditableTextBox ID="_MyTextBox" runat="server"></testsite:TestEditableTextBox></p>
          </ContentTemplate>
        </asp:UpdatePanel>
      </div>
    </form>
  </body>
</html>
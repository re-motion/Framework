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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserControlForm.aspx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.UserControlForm" %>
<%@ Register Src="FirstControl.ascx" TagName="FirstControl" TagPrefix="uc1" %>
<%@ Register TagPrefix="webTest" TagName="ZeroControl" Src="ZeroControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <style>
    div
    {
      border: solid 1px black;
      margin-bottom: .5em;
      padding: .5em;
    }
  </style>
</head>
<body>
  <form id="TheForm" runat="server">
  <a href="ShowUserControl.wxe">Restart</a>
  <div>
    Page
    <p>
      Last postback on page:
      <asp:Label ID="PageLabel" runat="server" /><br />
      <remotion:WebButton ID="PageButton" runat="server" Text="Postback to Page" OnClick="PageButton_Click" />
      <remotion:WebButton ID="ExecuteSecondUserControlButton" runat="server" Text="Execute Second User Control" OnClick="ExecuteSecondUserControlButton_Click" />
      <br />
      ViewState: <asp:Label ID="ViewStateLabel" runat="server" /> (Must increment by two upon execute/return)<br />
      ControlState: <asp:Label ID="ControlStateLabel" runat="server" /> (Must increment by two upon execute/return)<br />
      <remotion:ControlMock ID="SubControlWithState" runat="server" /><br />
      <asp:TextBox ID="SubControlWithFormElement" runat="server" EnableViewState="false" />
    </p>
  </div>
  <asp:PlaceHolder ID="FirstControlPlaceHoder" runat="server" />
  <webTest:ZeroControl ID="TheUserControl" runat="server" />
  <div>
    Stack:
    <p>
      <asp:Label ID="StackLabel" runat="server" />
    </p>
  </div>
  </form>
</body>
</html>

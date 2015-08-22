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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmartForm.aspx.cs" Inherits="Remotion.Web.Test.DoublePostBackHandling.SmartForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnablePartialRendering="True" />
    <div>
      <asp:UpdatePanel runat="server">
        <ContentTemplate>
          Async:
          <asp:TextBox runat="server" id="AsyncTextBox" AutoPostBack="True" OnTextChanged="AsyncTextBox_OnTextChanged"/>
          <asp:Button runat="server" id="AsyncSubmitButton" Text="Async Button" OnClick="AsyncSubmitButton_OnClick"/>
          <asp:LinkButton runat="server" id="AsyncLinkButton" Text="Async Link" OnClick="AsyncLinkButton_OnClick"/>
        </ContentTemplate>
      </asp:UpdatePanel>
      Sync:
      <asp:TextBox runat="server" id="SyncTextBox" AutoPostBack="True" OnTextChanged="SyncTextBox_OnTextChanged"/>
      <asp:Button runat="server" id="SyncSubmitButton" Text="Sync Button" OnClick="SyncSubmitButton_OnClick"/>
      <asp:LinkButton runat="server" id="SyncLinkButon" Text="Sync Link" OnClick="SyncLinkButton_OnClick"/>
      <asp:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
          Counter: <asp:TextBox runat="server" ID="CounterTextBox" EnableViewState="False"/>
          <div>
             <asp:Label runat="server" ID="StatusLabel"/>
          </div>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>

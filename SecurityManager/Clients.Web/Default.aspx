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
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.DefaultPage" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Security Manager</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server">
  </remotion:HtmlHeadContents>
</head>
<body>
  <form id="ThisForm" runat="server">
    <p>
      <a href="UserList.wxe">Aufbauorganisation verwalten</a>
    </p>
    <p>
      <a href="SecurableClassDefinitionList.wxe?WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
    <p>
      <remotion:BindableObjectDataSourceControl ID="UserDataSource" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.User, Remotion.SecurityManager" />
      <remotion:BocReferenceValue runat="server" ID="UsersField" DataSourceControl="UserDataSource" OnSelectionChanged="UsersField_SelectionChanged">
        <DropDownListStyle AutoPostBack="True" />
      </remotion:BocReferenceValue>
    </p>
  </form>
</body>
</html>

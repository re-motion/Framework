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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocTreeViewUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocTreeViewUserControlTestOutput" %>
<table border="1">
  <tr><td>Selected node (normal):</td><td colspan="3"><asp:Label ID="NormalSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected node (no top level expander):</td><td colspan="3"><asp:Label ID="NoTopLevelExpanderSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected node (no look ahead evaluation):</td><td colspan="3"><asp:Label ID="NoLookAheadEvaluationSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected node (no property identifier):</td><td colspan="3"><asp:Label ID="NoPropertyIdentifierSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr>
    <td>Action performed:</td>
    <td><asp:Label ID="ActionPerformedSenderLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedParameterLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
  </tr>
</table>
<style>
  #body_DataEditControl_NoTopLevelExpander > div
  {
    width: 200px;
    height: 220px;
  }
</style>
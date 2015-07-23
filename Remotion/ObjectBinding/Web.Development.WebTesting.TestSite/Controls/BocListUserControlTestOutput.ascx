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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocListUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocListUserControlTestOutput" %>
<table border="1">
  <tr><td>Selected indices (normal):</td><td colspan="4"><asp:Label ID="SelectedIndicesLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected view (normal):</td><td colspan="4"><asp:Label ID="SelectedViewLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Edit mode (normal):</td><td colspan="4"><asp:Label ID="EditModeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr>
    <td>Action performed:</td>
    <td><asp:Label ID="ActionPerformedSenderLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedSenderRowLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedParameterLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
  </tr>
</table>
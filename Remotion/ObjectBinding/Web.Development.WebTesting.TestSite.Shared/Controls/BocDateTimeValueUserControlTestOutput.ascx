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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocDateTimeValueUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocDateTimeValueUserControlTestOutput" %>
<table border="1">
  <tr><td>Current value (normal):</td><td><asp:Label ID="NormalCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (no auto postback):</td><td><asp:Label ID="NoAutoPostBackCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (date-only):</td><td><asp:Label ID="DateOnlyCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (with seconds):</td><td><asp:Label ID="WithSecondsCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (DateOnly data type):</td><td><asp:Label ID="DateOnlyTypeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
</table>
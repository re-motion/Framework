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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocEnumValueUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocEnumValueUserControlTestOutput" %>
<table border="1">
  <tr><td>Current value (DropDownList, normal):</td><td><asp:Label ID="DropDownListNormalCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (DropDownList, no auto postback):</td><td><asp:Label ID="DropDownListNoAutoPostBackCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (ListBox, normal):</td><td><asp:Label ID="ListBoxNormalCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (ListBox, no auto postback):</td><td><asp:Label ID="ListBoxNoAutoPostBackCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, normal):</td><td><asp:Label ID="RadioButtonListNormalCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, no auto postback):</td><td><asp:Label ID="RadioButtonListNoAutoPostBackCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, multi-column):</td><td><asp:Label ID="RadioButtonListMultiColumnCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, flow):</td><td><asp:Label ID="RadioButtonListFlowCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, ordered list):</td><td><asp:Label ID="RadioButtonListOrderedListCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, unordered list):</td><td><asp:Label ID="RadioButtonListUnorderedListCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (RadioButtonList, label left):</td><td><asp:Label ID="RadioButtonListLabelLeftCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
</table>
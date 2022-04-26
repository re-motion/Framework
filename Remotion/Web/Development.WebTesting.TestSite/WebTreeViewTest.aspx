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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="WebTreeViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.WebTreeViewTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>WebTreeView1</h3>
  <remotion:WebTreeView ID="MyWebTreeView" runat="server"/>
  <div id="scope">
    <h3>WebTreeView2</h3>
    <remotion:WebTreeView ID="MyWebTreeView2" EnableWordWrap="True" Width="100px" runat="server"/>
  </div>
  <h3>WebTreeView3</h3>
  <remotion:WebTreeView ID="MyWebTreeView3" runat="server"/>
  <h3>MyOrderedWebTreeView</h3>
  <remotion:WebTreeView ID="MyOrderedWebTreeView" EnableTopLevelGrouping="True" runat="server"/>
  <h3>MyUnorderedWebTreeView</h3>
  <remotion:WebTreeView ID="MyUnorderedWebTreeView" EnableTopLevelGrouping="False" runat="server"/>
  <h3>MyWebTreeViewWithCategories</h3>
  <remotion:WebTreeView ID="MyWebTreeViewWithCategories" runat="server"/>
  <h3>MyWebTreeViewWithoutCategories</h3>
  <remotion:WebTreeView ID="MyWebTreeViewWithoutCategories" runat="server"/>
  <h3>MyWebTreeViewWithUmlauts</h3>
  <remotion:WebTreeView ID="MyWebTreeViewWithUmlauts" runat="server"/>
</asp:Content>

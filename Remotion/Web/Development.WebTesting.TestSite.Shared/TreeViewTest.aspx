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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="TreeViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.TreeViewTest" %>
<%@ Import Namespace="System" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TreeView</h3>
  <asp:TreeView ID="MyTreeView" runat="server"/>
  <h3>TreeView with SingleQuote and DoubleQuotes in Childs Display Text</h3>
  <asp:TreeView ID="MyTreeViewWithSpecialChildren" runat="server"/>
  <h3>TreeView with SingleQuote in root node Display Text</h3>
  <asp:TreeView ID="TreeViewWithOnlyRootWithSingleQuote" runat="server" />  
  <h3>TreeView with Quotation Mark in root node Display Text</h3>
  <asp:TreeView ID="TreeViewWithOnlyRootWithDoubleQuote" runat="server"/>
</asp:Content>
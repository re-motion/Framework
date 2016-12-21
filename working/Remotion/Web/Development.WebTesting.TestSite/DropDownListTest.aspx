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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="DropDownListTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.DropDownListTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>DropDownList1</h3>
  <asp:DropDownList ID="MyDropDownList" AutoPostBack="true" Enabled="true" runat="server">
    <Items>
      <asp:ListItem Text="Item1" Value="Item1Value"/>
      <asp:ListItem Text="Item2" Value="Item2Value"/>
      <asp:ListItem Text="Item3" Value="Item3Value"/>
    </Items>
  </asp:DropDownList>
  <div id="scope">
    <h3>DropDownList2</h3>
    <asp:DropDownList ID="MyDropDownList2" AutoPostBack="true" Enabled="true" runat="server">
      <Items>
        <asp:ListItem Text="Item1" Value="Item1Value"/>
        <asp:ListItem Text="Item2" Value="Item2Value"/>
        <asp:ListItem Text="Item3" Value="Item3Value"/>
      </Items>
    </asp:DropDownList>
  </div>
</asp:Content>
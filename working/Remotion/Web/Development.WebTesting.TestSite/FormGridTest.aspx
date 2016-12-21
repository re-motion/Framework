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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="FormGridTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.FormGridTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <remotion:FormGridManager runat="server"></remotion:FormGridManager>
  <h3>FormGrid1</h3>
  <table ID="My1FormGrid" runat="server">
    <tr>
      <td colspan="2">MyFormGrid1</td>
    </tr>
    <tr>
      <td></td>
      <td>DoNotFindMe2</td>
    </tr>
    <tr>
      <td></td>
      <td>Content1</td>
    </tr>
  </table>
  <div id="scope">
    <h3>FormGrid2</h3>
    <table ID="My2FormGrid" runat="server">
      <tr>
        <td colspan="2">MyFormGrid2</td>
      </tr>
      <tr>
        <td></td>
        <td>DoNotFindMe1</td>
      </tr>
      <tr>
        <td></td>
        <td>Content2</td>
      </tr>
    </table>
  </div>
</asp:Content>
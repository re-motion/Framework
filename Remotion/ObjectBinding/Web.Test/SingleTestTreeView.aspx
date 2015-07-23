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
<%@ Page Language="c#" Codebehind="SingleTestTreeView.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleTestTreeView" MasterPageFile="~/StandardMode.Master" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" />
    <h1>
      SingleTest TreeView</h1>
    <table width="100%">
      <tr>
        <td valign="top" width="33%">
          <ros:PersonTreeView ID="PersonTreeView" runat="server" DataSourceControl="CurrentObject" CssClass="TreeBlock" EnableTopLevelExpander="False" EnableLookAheadEvaluation="True" />
          <asp:Button ID="RefreshPesonTreeViewButton" runat="server" Text="Refresh"></asp:Button>
        </td>
        <td valign="top" width="33%">
          <remotion:WebTreeView ID="WebTreeView" runat="server" CssClass="TreeBlock" Width="150px" EnableScrollBars="True" />
          <p>
            <asp:Button ID="PostBackButton" runat="server" Text="PostBack"></asp:Button></p>
          <p>
            <asp:Label ID="TreeViewLabel" runat="server" EnableViewState="False">#</asp:Label></p>
          <p>
            <asp:Button ID="Node332Button" runat="server" Text="Node 332"></asp:Button></p>
        </td>
        <td valign="top" width="33%">
          <asp:UpdatePanel ID="UpdatePanel" runat="server">
            <ContentTemplate>
              <ros:PersonTreeView ID="PersonTreeViewWithMenus" runat="server" DataSourceControl="CurrentObject" CssClass="TreeBlock" EnableTopLevelExpander="False" EnableLookAheadEvaluation="True" />
            </ContentTemplate>
          </asp:UpdatePanel>
        </td>
      </tr>
    </table>
    <remotion:FormGridManager ID="FormGridManager" runat="server" />
    <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
</asp:Content>

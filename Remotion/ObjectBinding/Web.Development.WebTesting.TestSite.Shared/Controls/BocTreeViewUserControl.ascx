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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocTreeViewUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocTreeViewUserControl" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="ros" Namespace="Remotion.ObjectBinding.Sample" Assembly="Remotion.ObjectBinding.Sample" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<div id="sizeLimiter" style="width: 900px">
  <table id="FormGrid" runat="server">
    <tr>
      <td></td>
      <td>
        <remotion:BocTreeView
          ID="Normal"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="True"
          runat="server">
        </remotion:BocTreeView>
      </td>
      <td>(normal)</td>
    </tr>
    <tr>
      <td></td>
      <td>
        <remotion:BocTreeView
          ID="NoTopLevelExpander"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          Enabled="True"
          EnableTopLevelExpander="False"
          EnableLookAheadEvaluation="True"
          runat="server"/>
      </td>
      <td>(no top level expander)</td>
    </tr>
    <tr>
      <td></td>
      <td>
        <remotion:BocTreeView
          ID="NoLookAheadEvaluation"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          Enabled="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="False"
          runat="server"/>
      </td>
      <td>(no look ahead evaluation)</td>
    </tr>
    <tr>
      <td>Person</td>
      <td>
        <remotion:BocTreeView
          ID="NoPropertyIdentifier"
          DataSourceControl="CurrentObject"
          ShowLines="True"
          Enabled="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="True"
          runat="server"/>
      </td>
      <td>(no property identifier)</td>
    </tr>
    <tr>
      <td></td>
      <td>
        <remotion:BocTreeView
          ID="ContextMenu_Delayed"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="True"
          ControlServicePath="BocTreeViewWebService.asmx"
          ControlServiceArguments="500"
          runat="server">
        </remotion:BocTreeView>
      </td>
      <td>(context menu with delay)</td>
    </tr>
    <tr>
      <td></td>
      <td>
        <remotion:BocTreeView
          ID="ContextMenu_DelayedLongerThanTimeout"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="True"
          ControlServicePath="BocTreeViewWebService.asmx"
          ControlServiceArguments="31000"
          runat="server">
        </remotion:BocTreeView>
      </td>
      <td>(context menu with timeout)</td>
    </tr>
    <tr>
      <td></td>
      <td>
        <remotion:BocTreeView
          ID="ContextMenu_Error"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="True"
          ControlServicePath="BocTreeViewWebService.asmx"
          ControlServiceArguments="error"
          runat="server">
        </remotion:BocTreeView>
      </td>
      <td>(context menu with error)</td>
    </tr>
    <tr>
      <td></td>
      <td>
        <ros:PersonTreeView
          ID="ContextMenu_Person"
          DataSourceControl="CurrentObject"
          PropertyIdentifier="Children"
          ShowLines="True"
          EnableTopLevelExpander="True"
          EnableLookAheadEvaluation="True"
          ControlServicePath="BocTreeViewWebService.asmx"
          ControlServiceArguments="error"
          runat="server">
        </ros:PersonTreeView>
      </td>
      <td>(person tree view)</td>
    </tr>
  </table>
</div>
<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 % 
 % This program is free software; you can redistribute it and/or modify
 % it under the terms of the GNU Affero General Public License version 3.0 
 % as published by the Free Software Foundation.
 % 
 % This program is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Affero General Public License for more details.
 % 
 % You should have received a copy of the GNU Affero General Public License
 % along with this program; if not, see http://www.gnu.org/licenses.
 % 
 % Additional permissions are listed in the file re-motion_exceptions.txt.
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUserControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditUserControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.User, Remotion.SecurityManager" />
<asp:ScriptManagerProxy runat="server" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="UserLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue runat="server" ID="UserNameField" DataSourceControl="CurrentObject" PropertyIdentifier="UserName"></remotion:BocTextValue>    
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="TitleField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Title">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="FirstNameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="FirstName">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="LastNameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="OwningGroupField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="OwningGroup">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="RolesList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Roles" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition ItemID="Group" PropertyPathIdentifier="Group" />
          <remotion:BocSimpleColumnDefinition ItemID="Position" PropertyPathIdentifier="Position" />
        </FixedColumns>
      </remotion:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="SubstitutedByList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="SubstitutedBy" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="SubstitutingUser" Width="30%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="SubstitutedRole" Width="30%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="BeginDate" Width="15%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EndDate" Width="15%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="IsEnabled" Width="10%" />
        </FixedColumns>
      </remotion:BocList>
    </td>
  </tr>
</table>

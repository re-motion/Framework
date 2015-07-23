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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Group, Remotion.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="GroupLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="ShortName" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="ShortName">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Name">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="GroupTypeField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="GroupType">
      <PersistedCommand>
        <remotion:BocCommand />
      </PersistedCommand>
    </remotion:BocAutoCompleteReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="ParentField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Parent">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocAutoCompleteReferenceValue>
      <asp:CustomValidator ID="ParentValidator" runat="server" OnServerValidate="ParentValidator_ServerValidate" ControlToValidate="ParentField" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="ChildrenList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Children" ReadOnly="True" Selection="Disabled" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </remotion:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="RolesList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Roles" Selection="Multiple" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition ItemID="User" PropertyPathIdentifier="User">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
          <remotion:BocSimpleColumnDefinition ItemID="Position" PropertyPathIdentifier="Position">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </remotion:BocList>
    </td>
  </tr>
</table>

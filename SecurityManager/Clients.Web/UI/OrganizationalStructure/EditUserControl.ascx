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
      <remotion:BocTextValue runat="server" ID="UserNameField" DataSourceControl="CurrentObject" EnableOptionalValidators="true" PropertyIdentifier="UserName"></remotion:BocTextValue>    
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="TitleField" runat="server" DataSourceControl="CurrentObject" EnableOptionalValidators="true"
        PropertyIdentifier="Title">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="FirstNameField" runat="server" DataSourceControl="CurrentObject" EnableOptionalValidators="true"
        PropertyIdentifier="FirstName">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="LastNameField" runat="server" DataSourceControl="CurrentObject" EnableOptionalValidators="true"
        PropertyIdentifier="LastName">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="OwningGroupField" runat="server" DataSourceControl="CurrentObject" EnableOptionalValidators="true"
        PropertyIdentifier="OwningGroup">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="RolesList" runat="server" DataSourceControl="CurrentObject" EnableOptionalValidators="true" PropertyIdentifier="Roles" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
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
      <remotion:BocList ID="SubstitutedByList" runat="server" DataSourceControl="CurrentObject" EnableOptionalValidators="true" PropertyIdentifier="SubstitutedBy" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition ItemID="SubstitutingUser" PropertyPathIdentifier="SubstitutingUser" Width="30%" />
          <remotion:BocSimpleColumnDefinition ItemID="SubstitutedRole" PropertyPathIdentifier="SubstitutedRole" Width="30%" />
          <remotion:BocSimpleColumnDefinition ItemID="BeginDate" PropertyPathIdentifier="BeginDate" Width="15%" />
          <remotion:BocSimpleColumnDefinition ItemID="EndDate" PropertyPathIdentifier="EndDate" Width="15%" />
          <remotion:BocSimpleColumnDefinition ItemID="IsEnabled" PropertyPathIdentifier="IsEnabled" Width="10%" />
        </FixedColumns>
      </remotion:BocList>
    </td>
  </tr>
</table>

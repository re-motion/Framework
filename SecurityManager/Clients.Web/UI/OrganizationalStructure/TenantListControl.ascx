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
 % 
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TenantListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.TenantListControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Tenant, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<div class="listControlContent">
  <div class="listControlList">
    <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
      <tr class="underlinedMarkerCellRow">
        <td class="formGridTitleCell" style="white-space: nowrap;">
          <remotion:SmartLabel runat="server" id="TenantListLabel" Text="###"/>
        </td>
        <td style="DISPLAY: none;WIDTH: 100%"></td>
      </tr>
      <tr>
        <td>
          <remotion:BocList ID="TenantList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="TenantList_ListItemCommandClick" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
            <FixedColumns>
              <remotion:BocSimpleColumnDefinition ItemID="DisplayName" PropertyPathIdentifier="DisplayName">
                <PersistedCommand>
                  <remotion:BocListItemCommand Type="Event" />
                </PersistedCommand>
              </remotion:BocSimpleColumnDefinition>
            </FixedColumns>
          </remotion:BocList>
        </td>
      </tr>
    </table>
  </div>
  <div class="listControlButtons">
    <remotion:WebButton ID="NewTenantButton" runat="server" OnClick="NewTenantButton_Click" />
  </div>
</div>

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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PositionListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.PositionListControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Position, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<div class="listControlContent">
  <div class="listControlList">
    <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
      <tr class="underlinedMarkerCellRow">
        <td class="formGridTitleCell" style="white-space: nowrap;">
          <remotion:SmartLabel runat="server" id="PositionListLabel" Text="###"/>
        </td>
        <td style="DISPLAY: none;WIDTH: 100%"></td>
      </tr>
      <tr>
        <td>
          <remotion:BocList ID="PositionList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="PositionList_ListItemCommandClick" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
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
    <remotion:WebButton ID="NewPositionButton" runat="server" OnClick="NewPositionButton_Click" />
  </div>
</div>

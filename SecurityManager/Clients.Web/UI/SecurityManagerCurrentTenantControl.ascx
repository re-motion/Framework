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
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerCurrentTenantControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.CurrentTenantControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>
 <div>
  <remotion:BocReferenceValue ID="CurrentUserField" runat="server" ReadOnly="True" style="width:auto">
    <PersistedCommand>
      <remotion:BocCommand />
    </PersistedCommand>
  </remotion:BocReferenceValue>
  &nbsp;
  <remotion:BocReferenceValue ID="CurrentSubstitutionField" runat="server" Required="False" OnSelectionChanged="CurrentSubstitutionField_SelectionChanged" OnCommandClick="CurrentSubstitutionField_CommandClick" >
    <PersistedCommand>
      <remotion:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>
<div>
  <remotion:BocReferenceValue ID="CurrentTenantField" runat="server" Required="True" OnSelectionChanged="CurrentTenantField_SelectionChanged" OnCommandClick="CurrentTenantField_CommandClick">
    <PersistedCommand>
      <remotion:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>

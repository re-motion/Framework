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
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerUserContextControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.SecurityManagerUserContextControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<div>
  <remotion:SmartLabel runat="server" ID="CurrentUserLabel" hidden="hidden" EnableViewState="False" ForControl="CurrentUserField" Text="###" />
  <remotion:BocReferenceValue ID="CurrentUserField" runat="server" ReadOnly="True" style="width:auto">
  </remotion:BocReferenceValue>
  &nbsp;
  <remotion:SmartLabel runat="server" ID="CurrentSubstitutionLabel" hidden="hidden" EnableViewState="False" ForControl="CurrentSubstitutionField" Text="###" />
  <remotion:BocReferenceValue ID="CurrentSubstitutionField" runat="server" Required="False" OnSelectionChanged="CurrentSubstitutionField_SelectionChanged">
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>
<div>
  <remotion:SmartLabel runat="server" ID="CurrentTenantLabel" hidden="hidden" EnableViewState="False" ForControl="CurrentTenantField" Text="###" />
  <remotion:BocReferenceValue ID="CurrentTenantField" runat="server" Required="True" OnSelectionChanged="CurrentTenantField_SelectionChanged">
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>

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
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStatelessAccessControlListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStatelessAccessControlListControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StatelessAccessControlList, Remotion.SecurityManager" />
<asp:ScriptManagerProxy runat="server" />

<table class="accessControlList">
  <tr>
    <td class="stateCombinationsButtons">
      <remotion:WebButton ID="DeleteAccessControlListButton" runat="server" OnClick="DeleteAccessControlListButton_Click" CausesValidation="false" RequiresSynchronousPostBack="true" />
    </td>
    <td class="accessControlEntriesContainer" rowspan="2">
      <asp:PlaceHolder id="AccessControlEntryControls" runat="server" />
   </td>
  </tr>
  <tr>
    <td class="stateCombinationsContainer">
    </td>
  </tr>
  <tr>
    <td class="stateCombinationsButtons">      
    </td>
    <td class="accessControlEntriesButtons">
      <remotion:WebButton ID="NewAccessControlEntryButton" runat="server" OnClick="NewAccessControlEntryButton_Click" CausesValidation="false" />
   </td>
  </tr>
</table>


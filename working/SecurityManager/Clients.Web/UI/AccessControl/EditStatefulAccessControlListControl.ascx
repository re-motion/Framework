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
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStatefulAccessControlListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStatefulAccessControlListControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StatefulAccessControlList, Remotion.SecurityManager" />
<asp:ScriptManagerProxy runat="server" />

<table class="accessControlList">
  <tr>
    <td class="stateCombinationsButtons">
      <remotion:WebButton ID="DeleteAccessControlListButton" runat="server" OnClick="DeleteAccessControlListButton_Click" CausesValidation="false" RequiresSynchronousPostBack="true" />
      <remotion:WebButton ID="NewStateCombinationButton" runat="server" OnClick="NewStateCombinationButton_Click" CausesValidation="false" />
    </td>
    <td class="accessControlEntriesContainer" rowspan="2">
      <asp:PlaceHolder id="AccessControlEntryControls" runat="server" />
   </td>
  </tr>
  <tr>
    <td class="stateCombinationsContainer">
      <asp:PlaceHolder id="StateCombinationControls" runat="server" />
      <asp:CustomValidator ID="MissingStateCombinationsValidator" runat="server" ErrorMessage="###" OnServerValidate="MissingStateCombinationsValidator_ServerValidate" />
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


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
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStateCombinationControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStateCombinationControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>
<div>
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StateCombination, Remotion.SecurityManager" />
<remotion:BindableObjectDataSourceControl ID="StateDefinitionDataSource" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.StateDefinition, Remotion.SecurityManager" />
<div id="StateDefinitionContainer" runat="server" style="white-space:nowrap;">
<remotion:BocReferenceValue id="StateDefinitionField" runat="server" DataSourceControl="StateDefinitionDataSource" Required="True" Width="10em" EnableOptionalValidators="true"/>
<remotion:WebButton ID="DeleteStateDefinitionButton" runat="server" OnClick="DeleteStateDefinitionButton_Click" CssClass="imageButton" />
</div>
<asp:CustomValidator ID="RequiredStateCombinationValidator" runat="server" ErrorMessage="###" OnServerValidate="RequiredStateCombinationValidator_ServerValidate" CssClass="permissionsEditorValidationMessage"/>
</div>

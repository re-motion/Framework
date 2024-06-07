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

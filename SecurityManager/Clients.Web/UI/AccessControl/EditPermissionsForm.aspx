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
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<asp:Content ID="ActualHeaderControlsPlaceHolder" runat="server" ContentPlaceHolderID="HeaderControlsPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObjectHeaderControls" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" Mode="Read" />
  <h1 ID="TitleLabel" runat="server" class="pageTitle">###</h1>
</asp:Content>
<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder" />
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <div class="mainContent">
    <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" />
    <asp:CustomValidator ID="DuplicateStateCombinationsValidator" runat="server" ErrorMessage="<%$ res:DuplicateStateCombinationsValidatorErrorMessage %>" OnServerValidate="DuplicateStateCombinationsValidator_ServerValidate" EnableClientScript="false" CssClass="permissionsEditorValidationMessage"/>
    <asp:PlaceHolder ID="AccessControlListsPlaceHolder" runat="server"/>
  </div>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <asp:UpdatePanel ID="BottomControlsUpdatePanel" runat="server" UpdateMode="Always" RenderMode="Inline">
    <ContentTemplate>
      <remotion:WebButton ID="SaveButton" runat="server" OnClick="SaveButton_Click" CausesValidation="false" RequiresSynchronousPostBack="true" />
      <remotion:WebButton ID="CancelButton" runat="server" OnClick="CancelButton_Click" CausesValidation="false" RequiresSynchronousPostBack="true" />
      <remotion:WebButton ID="NewStatefulAccessControlListButton" runat="server" OnClick="NewStatefulAccessControlListButton_Click" CausesValidation="False" RequiresSynchronousPostBack="true"/>
      <remotion:WebButton ID="NewStatelessAccessControlListButton" runat="server" OnClick="NewStatelessAccessControlListButton_Click" CausesValidation="False" RequiresSynchronousPostBack="true" />
    </ContentTemplate> 
  </asp:UpdatePanel>
</asp:Content>

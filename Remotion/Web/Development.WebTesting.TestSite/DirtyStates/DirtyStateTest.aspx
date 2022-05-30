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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="DirtyStateTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.DirtyStates.DirtyStateTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>Set Dirty State</h3>
      <remotion:WebButton ID="SetPageDirtyButton" Text="SetPageDirtyButton" runat="server" OnClick="SetPageDirtyButton_OnClick" />
      <remotion:WebButton ID="SetCurrentFunctionDirtyButton" Text="SetCurrentFunctionDirtyButton" runat="server" OnClick="SetCurrentFunctionDirtyButton_OnClick" />
      <remotion:WebButton ID="DisableDirtyStateOnCurrentPageButton" Text="DisableDirtyStateOnCurrentPageButton" runat="server" OnClick="DisableDirtyStateOnCurrentPageButton_OnClick" />
      <h3>Control Flow</h3>
      <remotion:WebButton ID="ExecuteSubFunctionButton" Text="ExecuteSubFunctionButton" runat="server" OnClick="ExecuteSubFunctionButton_OnClick" RequiresSynchronousPostBack="True" />
      <remotion:WebButton ID="ExecuteSubFunctionWithDisabledDirtyStateButton" Text="ExecuteSubFunctionWithDisabledDirtyStateButton" runat="server" OnClick="ExecuteSubFunctionWithDisabledDirtyStateButton_OnClick" RequiresSynchronousPostBack="True" />
      <remotion:WebButton ID="ExecuteNextStepButton" Text="ExecuteNextStepButton" runat="server" OnClick="ExecuteNextStepButton_OnClick" RequiresSynchronousPostBack="True" />
      <remotion:WebButton ID="CancelExecutingFunctionButton" Text="CancelExecutingFunctionButton" runat="server" OnClick="CancelExecutingFunctionButton_OnClick" RequiresSynchronousPostBack="True" />
      <h3>Support Elements</h3>
      <asp:TextBox runat="server" ID="TextField" />
    </ContentTemplate>
  </asp:UpdatePanel>

  <script type="text/javascript">
  function SmartPage_IsDirty(conditions)
  {
    return SmartPage_Context.Instance.IsDirty(conditions);
  }

  function SetPageDirtyOnClientSide()
  {
    var textField = document.getElementById("<%=TextField.ClientID%>");
    textField.dispatchEvent(new Event("change"));
  }
  </script>
</asp:Content>
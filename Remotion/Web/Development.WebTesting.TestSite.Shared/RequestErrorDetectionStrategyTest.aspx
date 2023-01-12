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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="RequestErrorDetectionStrategyTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.RequestErrorDetectionStrategyTest" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>Async Postback Button With Error</h3>
      <remotion:WebLinkButton ID="AsyncPostbackError" Text="Async Postback Button With Error" runat="server" OnClick="AsyncPostback_OnClick"/>
    </ContentTemplate>
  </asp:UpdatePanel>
  
  <h3>Synchronous Postback Button With Error</h3>
  <remotion:WebLinkButton ID="SyncPostbackError" Text="Synchronous Postback Button With Error" runat="server" OnClick="SyncPostbackError_OnClick"/>
  <h3>Synchronous Postback Button Error</h3>
  <remotion:WebLinkButton ID="SyncPostbackWithoutError" Text="Synchronous Postback Button Error" runat="server" OnClick="SyncPostbackWithoutError_OnClick"/>
  <h3>Synchronous Postback With Special Characters in Error MessageButton Error</h3>
  <remotion:WebLinkButton ID="SyncPostbackWithSpecialCharactersInErrorMessage" Text="Synchronous Postback With Special Characters in Error MessageButton Error" runat="server" OnClick="SyncPostbackWithSpecialCharactersInErrorMessage_OnClick"/>
</asp:Content>
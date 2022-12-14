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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="CompletionDetectionStrategyTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.CompletionDetectionStrategyTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>Delayed PostBack</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <remotion:WebButton ID="MyDelayedPostBack" Text="DelayedPostBack" runat="server"/>
      <h3>Delayed Reset</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <remotion:WebButton ID="MyDelayedReset" Text="DelayedReset" runat="server"/>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>
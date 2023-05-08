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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="ElementScopeTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.ElementScopeTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
<asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
    <h3>Disabled</h3>
  <button id="DisabledButton" type="button" disabled="disabled">Disabled button</button>
  <button id="NormalButton" type="button">Normal button</button>
    
  <h3>Toggle Visibility</h3>
  <asp:Button ID="ToggleVisibilityButton" Text="ToggleVisibilityButton" runat="server"/>
  <asp:LinkButton ID="VisibilityButton" Text="VisibilityButton" CommandName="VisibilityButton" PostBackUrl="ElementScopeTest.aspx" runat="server" Visible="True"/>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
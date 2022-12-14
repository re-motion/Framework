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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="LabelTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.LabelTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>Label1 - re-motion SmartLabel</h3>
  <remotion:SmartLabel ID="MySmartLabel" Text=" MySmartLabelContent " runat="server"/>
  <h3>Label2 - re-motion FormGridLabel</h3>
  <remotion:FormGridLabel ID="MyFormGridLabel" Text=" MyFormGridLabelContent " runat="server"/>
  <h3>Label3 - ASP.NET Label</h3>
  <asp:Label ID="MyAspLabel" Text=" MyAspLabelContent " runat="server"/>
  <h3>Label4 - HTML span</h3>
  <span id="MyHtmlLabel" runat="server"> MyHtmlLabelContent </span>
</asp:Content>
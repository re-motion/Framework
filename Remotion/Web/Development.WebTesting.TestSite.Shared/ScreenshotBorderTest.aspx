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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="ScreenshotBorderTest.aspx.cs" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <ContentTemplate>
      <div id="borderElementTop" style="position: absolute; width: 1px; height: 1px; top: 0; left: 50%; background-color: black"></div>
      <div id="borderElementRight" style="position: absolute; width: 1px; height: 1px; top: 50%; right: 0; background-color: black"></div>
      <div id="borderElementLeft" style="position: absolute; width: 1px; height: 1px; top: 50%; left: 0; background-color: black"></div>
      <div id="borderElementBottom" style="position: absolute; width: 1px; height: 1px; bottom: 0; right: 50%; background-color: black"></div>
    </ContentTemplate>
</asp:Content>
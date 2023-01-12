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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="CommandTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.CommandTest" %>
<%@ Register tagPrefix="testsite" namespace="Remotion.Web.Development.WebTesting.TestSite.Shared" assembly="Remotion.Web.Development.WebTesting.TestSite.Shared" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>Command1 - Event</h3>
      <testsite:TestCommand ID="Command1" ItemID="Command1ItemID" Text="Command1" CommandType="Event" runat="server"></testsite:TestCommand>
      <div id="scope">
        <h3>Command2 - Href</h3>
        <testsite:TestCommand ID="Command2" ItemID="Command2ItemID" Text="Command2" CommandType="Href" HrefCommandInfo-Href="CommandTest.wxe" runat="server"></testsite:TestCommand>
      </div>
      <h3>Command3 - None</h3>
      <testsite:TestCommand ID="TestCommand3" ItemID="Command3ItemID" Text="Command3" CommandType="None" runat="server"></testsite:TestCommand>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>
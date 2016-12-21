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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="ImageButtonTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.ImageButtonTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>ImageButton</h3>
      <asp:ImageButton ID="MyImageButton" CommandName="MyImageButtonCommand" ImageUrl="~/Images/SampleIcon.gif" PostBackUrl="ImageButtonTest.wxe" runat="server"/>
      <div id="scope">
        <h3>ImageButton2</h3>
        <asp:ImageButton ID="MyImageButton2" CommandName="MyImageButton2Command" ImageUrl="~/Images/SampleIcon.gif" PostBackUrl="ImageButtonTest.wxe" runat="server"/>
      </div>
      <h3>ImageButton3</h3>
      <asp:HyperLink ID="MyImageButton3" ImageUrl="~/Images/SampleIcon.gif" NavigateUrl="ImageButtonTest.wxe" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>
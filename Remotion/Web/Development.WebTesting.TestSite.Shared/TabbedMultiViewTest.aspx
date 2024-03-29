﻿<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="TabbedMultiViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.TabbedMultiViewTest" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TabbedMultiView</h3>
  <remotion:TabbedMultiView ID="MyTabbedMultiView" runat="server" Height="500">
    <TopControls>
      <span>TopControls</span>
    </TopControls>
    <Views>
      <remotion:TabView ID="Tab1" Title="Tab1Title" runat="server">
        <span>Content1</span>
      </remotion:TabView>
      <remotion:TabView ID="Tab2" Title="Tab2Title" runat="server">
        <span>Content2</span>
      </remotion:TabView>
      <remotion:TabView runat="server" ID="Tab3" Title="Tab3Title" Icon-Url="Image/SampleIcon.gif">
        <span>Content3</span>
      </remotion:TabView>
    </Views>
    <BottomControls>
      <span>BottomControls</span>
    </BottomControls>
  </remotion:TabbedMultiView>
  <span>DoNotFindMe</span>
</asp:Content>
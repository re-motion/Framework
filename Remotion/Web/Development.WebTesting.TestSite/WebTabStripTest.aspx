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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="WebTabStripTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.WebTabStripTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>WebTabStrip1</h3>
  <remotion:WebTabStrip ID="MyTabStrip1" style="width: 300px" runat="server">
    <Tabs>
      <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
      <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
      <remotion:WebTab ItemID="Tab3" Text="Tab3 disabled" IsDisabled="True"/>
    </Tabs>
  </remotion:WebTabStrip>
  <div id="scope">
    <h3>WebTabStrip2</h3>
    <remotion:WebTabStrip ID="MyTabStrip2" Width="100px" runat="server">
      <Tabs>
        <remotion:WebTab ItemID="Tab1" Icon-Url="~/Images/SampleIcon.gif" Text="This is a very long text that should wrap."/>
        <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
      </Tabs>
    </remotion:WebTabStrip>
  </div>
  <remotion:WebTabStrip ID="MyTabStripWithAccessKeys" runat="server">
    <Tabs>
      <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
      <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
      <remotion:WebTab ItemID="Tab3" Text="Tab3 disabled" IsDisabled="True"/>
      <remotion:WebTab ItemID="TabWithAccessKey" Text="Tab with access key" AccessKey="A"/>
      <remotion:WebTab ItemID="TabWithImplicitAccessKey" Text="Tab with implicit access &key"/>
      <remotion:WebTab ItemID="TabDisabledWithAccessKey" Text="Tab disabled with access key" IsDisabled="True" AccessKey="D"/>
    </Tabs>
  </remotion:WebTabStrip>
  <h3>WebTabStrip1</h3>
  <remotion:WebTabStrip ID="MyTabStripWithUmlaut" style="width: 300px" runat="server">
    <Tabs>
      <remotion:WebTab ItemID="Tab1" Text="(html)UmlautÖ"/>
    </Tabs>
  </remotion:WebTabStrip>
</asp:Content>

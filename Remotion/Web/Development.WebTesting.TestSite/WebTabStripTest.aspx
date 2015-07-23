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
  <remotion:WebTabStrip ID="MyTabStrip1" runat="server">
    <Tabs>
      <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
      <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
    </Tabs>
  </remotion:WebTabStrip>
  <div id="scope">
    <h3>WebTabStrip2</h3>
    <remotion:WebTabStrip ID="MyTabStrip2" runat="server">
      <Tabs>
        <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
        <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
      </Tabs>
    </remotion:WebTabStrip>
  </div>
</asp:Content>
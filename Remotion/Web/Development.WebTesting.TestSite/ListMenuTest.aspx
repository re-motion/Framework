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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="ListMenuTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.ListMenuTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>ListMenu1</h3>
      <remotion:ListMenu ID="MyListMenu" LineBreaks="BetweenGroups" Heading="Test List Menu" HeadingLevel="H4" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="ListMenuTest.wxe" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID3" Text="NoneItem">
            <PersistedCommand>
              <remotion:Command Type="None" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID4" Text="WxeFunctionItem">
            <PersistedCommand>
              <remotion:Command Type="WxeFunction" WxeFunctionCommand-MappingID="ListMenuTest" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif" Style="Icon"></remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID6" Text="DisabledItem" IsDisabled="True">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="ListMenuTest.wxe" />
            </PersistedCommand>
          </remotion:WebMenuItem>
        </MenuItems>
      </remotion:ListMenu>
      <div id="scope">
        <h3>ListMenu2</h3>
        <remotion:ListMenu ID="MyListMenu2" runat="server">
          <MenuItems>
            <remotion:WebMenuItem Category="Category1" Icon-Url="~/Images/SampleIcon.gif" ItemID="ItemID1" Text="This is a very long text that should wrap around into the next line and should be displayed to the right of the icon"/>
          </MenuItems>
        </remotion:ListMenu>
        <style>
          /* Limit the size of the list menu to test text wrapping */
          #body_MyListMenu2_0 { width: 100px }
        </style>
      </div>
      <h3>ListMenu3 - Disabled</h3>
      <remotion:ListMenu ID="MyListMenu_Disabled" runat="server" Enabled="False">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:ListMenu>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>

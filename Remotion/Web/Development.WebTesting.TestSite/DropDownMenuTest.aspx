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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="DropDownMenuTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.DropDownMenuTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>DropDownMenu1</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu" Mode="DropDownMenu" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
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
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif"></remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID6" Text="DisabledItem" IsDisabled="True">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
            </PersistedCommand>
          </remotion:WebMenuItem>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenuPrimary" Mode="DropDownMenu" ButtonType="Primary" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
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
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif"></remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID6" Text="DisabledItem" IsDisabled="True">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
            </PersistedCommand>
          </remotion:WebMenuItem>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenuSupplemental" Mode="DropDownMenu" ButtonType="Supplemental" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
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
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif"></remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID6" Text="DisabledItem" IsDisabled="True">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
            </PersistedCommand>
          </remotion:WebMenuItem>
        </MenuItems>
      </remotion:DropDownMenu>
      <div id="scope">
        <h3>DropDownMenu2</h3>
        <remotion:DropDownMenu ID="MyDropDownMenu2" Mode="DropDownMenu" TitleText="MyTitleText" runat="server">
          <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
          </MenuItems>
        </remotion:DropDownMenu>
        <remotion:DropDownMenu ID="MyDropDownMenu2Primary" Mode="DropDownMenu" ButtonType="Primary" TitleText="MyTitleText" runat="server">
          <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
          </MenuItems>
        </remotion:DropDownMenu>
        <remotion:DropDownMenu ID="MyDropDownMenu2Supplemental" Mode="DropDownMenu" ButtonType="Supplemental" TitleText="MyTitleText" runat="server">
          <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
          </MenuItems>
        </remotion:DropDownMenu>
      </div>
      <h3>DropDownMenu3</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_Disabled" Mode="DropDownMenu" TitleText="Text" Enabled="False" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
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
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif"></remotion:WebMenuItem>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_DisabledPrimary" Mode="DropDownMenu" ButtonType="Primary" TitleText="Text" Enabled="False" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
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
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif"></remotion:WebMenuItem>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_DisabledSupplemental" Mode="DropDownMenu" ButtonType="Supplemental" TitleText="Text" Enabled="False" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="EventItem">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="HrefItem">
            <PersistedCommand>
              <remotion:Command Type="Href" HrefCommand-Href="DropDownMenuTest.wxe" />
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
          <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Icon-Url="~/Images/SampleIcon.gif"></remotion:WebMenuItem>
        </MenuItems>
      </remotion:DropDownMenu>
      <h3>DropDownMenu (delayed open)</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_Delayed" Mode="DropDownMenu" TitleText="Delayed Open" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_DelayedPrimary" Mode="DropDownMenu" ButtonType="Primary" TitleText="Delayed Open" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_DelayedSupplemental" Mode="DropDownMenu" ButtonType="Supplemental" TitleText="Delayed Open" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <h3>DropDownMenu (delayed longer than timeout)</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_DelayedLongerThanTimeout" Mode="DropDownMenu" TitleText="Delayed Open" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_DelayedLongerThanTimeoutPrimary" Mode="DropDownMenu" ButtonType="Primary" TitleText="Delayed Open" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_DelayedLongerThanTimeoutSupplemental" Mode="DropDownMenu" ButtonType="Supplemental" TitleText="Delayed Open" runat="server">
        <MenuItems>
          <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <h3>DropDownMenu (error on open)</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_Error" Mode="DropDownMenu" TitleText="Delayed Open" runat="server">
        <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_ErrorPrimary" Mode="DropDownMenu" ButtonType="Primary" TitleText="Delayed Open" runat="server">
        <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <remotion:DropDownMenu ID="MyDropDownMenu_ErrorSupplemental" Mode="DropDownMenu" ButtonType="Supplemental" TitleText="Delayed Open" runat="server">
        <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Item"/>
        </MenuItems>
      </remotion:DropDownMenu>
      <h3>DropDownMenu (wrapping)</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_Wrapping" Mode="DropDownMenu" TitleText="Long text that should wrap correctly" Width="100px" runat="server">
        <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Very long text that is so big that the drop down menu overflows to the right out of the visible screen."/>
        </MenuItems>
      </remotion:DropDownMenu>
      <h3>DropDownMenu (hidden title)</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_HiddenTitle" Mode="DropDownMenu" TitleText="Title should not be displayed" ShowTitle="False" runat="server" Width="0">
        <MenuItems>
            <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="Very long text that is so big that the drop down menu overflows to the right out of the visible screen."/>
        </MenuItems>
      </remotion:DropDownMenu>
      <h3>DropDownMenu (many menu items)</h3>
      <remotion:DropDownMenu ID="MyDropDownMenu_ManyMenuItems" Mode="DropDownMenu" TitleText="Many menu items" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>
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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="ScreenshotTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.ScreenshotTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <%-- General tests --%>
      <div id="screenshotTarget" style="">
      </div>
      
      <table id="screenshotControlContainer" style="margin-bottom: 40px;">
        <tr>
          <td>
            <%-- DropDownList tests --%>
            <asp:DropDownList ID="MyDropDownList" Enabled="true" runat="server">
              <Items>
                <asp:ListItem Text=" " Value="Value1"/>
                <asp:ListItem Text=" " Value="Value2"/>
                <asp:ListItem Text=" " Value="Value3"/>
              </Items>
            </asp:DropDownList>
          </td>
          <td>
            <%-- DropDownMenu tests --%>
            <remotion:DropDownMenu ID="MyDropDownMenu" Mode="DropDownMenu" TitleText=" " style="width: 60px;" runat="server">
              <MenuItems>
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="  " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="  " Icon-Url="~/Images/SampleIcon.gif" /> <%-- space + non-breaking space --%>
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID3" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID4" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID5" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
              </MenuItems>
            </remotion:DropDownMenu>
          </td>
          <td>
            <%-- ListMenu tests --%>
            <remotion:ListMenu ID="MyListMenu" LineBreaks="BetweenGroups" runat="server">
              <MenuItems>
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID1" Text="  " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID2" Text="  " Icon-Url="~/Images/SampleIcon.gif" /> <%-- space + non-breaking space --%>
                <remotion:WebMenuItem Category="Category1" ItemID="ItemID3" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebMenuItem Category="Category2" ItemID="ItemID4" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebMenuItem Category="Category2" ItemID="ItemID5" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
              </MenuItems>
            </remotion:ListMenu>
          </td>
          <td>
            <%-- TabbedMenu tests --%>
            <remotion:TabbedMenu ID="MyTabbedMenu" StatusText="" runat="server">
              <Tabs>
                <remotion:MainMenuTab ItemID="ItemID1" Text="  " Icon-Url="~/Images/SampleIcon.gif">
                <SubMenuTabs>
                  <remotion:SubMenuTab ItemID="ItemID1" Text="  " Icon-Url="~/Images/SampleIcon.gif" />
                  <remotion:SubMenuTab ItemID="ItemID2" Text="  " Icon-Url="~/Images/SampleIcon.gif" /> <%-- space + non-breaking space --%>
                  <remotion:SubMenuTab ItemID="ItemID3" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                  <remotion:SubMenuTab ItemID="ItemID4" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                  <remotion:SubMenuTab ItemID="ItemID5" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                </SubMenuTabs>
                  </remotion:MainMenuTab>
                <remotion:MainMenuTab ItemID="ItemID2" Text="  " Icon-Url="~/Images/SampleIcon.gif" /> <%-- space + non-breaking space --%>
                <remotion:MainMenuTab ItemID="ItemID3" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:MainMenuTab ItemID="ItemID4" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:MainMenuTab ItemID="ItemID5" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
              </Tabs>
            </remotion:TabbedMenu>
          </td>
          <td>
            <%-- WebTabStrip tests --%>
            <remotion:WebTabStrip ID="MyTabStrip" runat="server">
              <Tabs>
                <remotion:WebTab ItemID="ItemID1" Text="  " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebTab ItemID="ItemID2" Text="  " Icon-Url="~/Images/SampleIcon.gif" /> <%-- space + non-breaking space --%>
                <remotion:WebTab ItemID="ItemID3" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebTab ItemID="ItemID4" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
                <remotion:WebTab ItemID="ItemID5" Text=" " Icon-Url="~/Images/SampleIcon.gif" />
              </Tabs>
            </remotion:WebTabStrip>
          </td>
        </tr>
      </table>
      <iframe id="frame" src="ScreenshotTestFrame.aspx" >
      </iframe>
      <style>
        #screenshotTarget
        {
          margin-top: 150px;
          margin-right: 140px;
          margin-bottom: 130px;
          margin-left: 120px; 
      
          width: 150px;
          height: 65px;

          background-color: black;
        }

        #frame
        {
          border: solid black 2px;
          margin: 4px;
          padding: 6px;

          width: 340px;
          height: 160px;
        }

        #screenshotControlContainer > tr > td
        {
          padding: 0;
        }
      </style>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>

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
<%@ Page Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="WebButtonTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.Shared.WebButtonTest" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>WebButton1</h3>
      <remotion:WebButton ID="MyWebButton1Sync" Text="SyncButton" CommandName="Sync" RequiresSynchronousPostBack="true" runat="server" />
      <remotion:WebButton ID="MyWebButtonPrimary1Sync" Text="SyncButton" ButtonType="Primary" CommandName="Sync" RequiresSynchronousPostBack="true" runat="server" />
      <remotion:WebButton ID="MyWebButtonSupplemental1Sync" Text="SyncButton" ButtonType="Supplemental" CommandName="Sync" RequiresSynchronousPostBack="true" runat="server" />
      <h3>WebButton2</h3>
      <remotion:WebButton ID="MyWebButton2Async" Text="AsyncButton" CommandName="Async" runat="server" />
      <remotion:WebButton ID="MyWebButtonPrimary2Async" Text="AsyncButton" ButtonType="Primary" CommandName="Async" runat="server" />
      <remotion:WebButton ID="MyWebButtonSupplemental2Async" Text="AsyncButton" ButtonType="Supplemental" CommandName="Async" runat="server" />
      <div id="scope">
        <h3>WebButton3</h3>
        <%-- ReSharper disable once Html.PathError --%>
        <remotion:WebButton ID="MyWebButton3Href" Text="HrefButton" PostBackUrl="~/WebButtonTest.wxe" runat="server" />
        <remotion:WebButton ID="MyWebButtonPrimary3Href" Text="HrefButton" ButtonType="Primary" PostBackUrl="~/WebButtonTest.wxe" runat="server" />
        <remotion:WebButton ID="MyWebButtonSupplemental3Href" Text="HrefButton" ButtonType="Supplemental" PostBackUrl="~/WebButtonTest.wxe" runat="server" />
      </div>
      <h3>Disabled WebButton</h3>
      <remotion:WebButton ID="MyDisabledWebButton" Text="DisabledWebButton" CommandName="Disabled" Enabled="False" runat="server"/>
      <remotion:WebButton ID="MyDisabledWebButtonPrimary" Text="DisabledWebButton" ButtonType="Primary" CommandName="Disabled" Enabled="False" runat="server"/>
      <remotion:WebButton ID="MyDisabledWebButtonSupplemental" Text="DisabledWebButton" ButtonType="Supplemental" CommandName="Disabled" Enabled="False" runat="server"/>
      <h3>WebButton with icon</h3>
      <remotion:WebButton ID="MyWebButtonWithIcon" Text="This text should wrap correctly because it is too long to fit" Width="100px" CommandName="Disabled" runat="server"/>
      <h3>WebButton with icon and projec relative path (~/...)</h3>
      <remotion:WebButton ID="MyWebButtonWithIconAndProjectRelativePath" Icon-Url="~/res/Remotion.Web.Development.WebTesting.TestSite.Shared/Image/SampleIcon.gif" Text="My icon Text" Width="100px" CommandName="Disabled" runat="server"/>
      <h3>WebButton UseLegacyButton="True"</h3>
      <remotion:WebButton ID="MyWebButtonWithUseLegacyButton" Text="LegacyButton" CommandName="Sync" RequiresSynchronousPostBack="true" UseLegacyButton="True" runat="server" />
      <h3>WebButton with Access Keys</h3>
      <remotion:WebButton ID="MyWebButtonWithAccessKey" Text="Button with access key" AccessKey="A" CommandName="Sync" RequiresSynchronousPostBack="true" runat="server" />
      <remotion:WebButton ID="MyWebButtonWithImplicitAccessKey" Text="Button with implicit access &key" CommandName="Sync" RequiresSynchronousPostBack="true" runat="server" />
      <h3>WebButton with Umlauts</h3>
      <remotion:WebButton ID="MyWebButtonWithUmlauts" Text="(html)UmlautÖ" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>

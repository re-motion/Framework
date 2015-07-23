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
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="AnchorTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.AnchorTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <script type="text/javascript">
        function myClickHandler(text) { $ ('#TestOutputLabel').text (text); }
      </script>
      <h3>HtmlAnchor1 - re-motion WebLinkButton</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <remotion:WebLinkButton ID="MyWebLinkButton" Text="MyWebLinkButton" CommandName="MyWebLinkButtonCommand" PostBackUrl="AnchorTest.wxe" runat="server"/>
      <h3>HtmlAnchor2 - re-motion SmartHyperLink</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <remotion:SmartHyperLink ID="MySmartHyperLink" Text="MySmartHyperLink" NavigateUrl="AnchorTest.wxe" runat="server"/>
      <h3>HtmlAnchor3 - ASP.NET LinkButton</h3>
      <asp:LinkButton ID="MyAspLinkButton" Text="MyAspLinkButton" CommandName="MyAspLinkButtonCommand" runat="server"/>
      <h3>HtmlAnchor4 - ASP.NET HyperLink</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <asp:HyperLink ID="MyAspHyperLink" Text="MyAspHyperLink" NavigateUrl="AnchorTest.wxe" runat="server" />
      <div id="scope">
        <h3>HtmlAnchor5 - HTML a</h3>
        <a id="MyHtmlAnchor" href="AnchorTest.wxe" runat="server">MyHtmlAnchor</a>
      </div>
      <h3>HtmlAnchor6 - HTML a with JavaScript action</h3>
      <a id="MyHtmlAnchorWithJavaScriptLink" href="#" onclick="javascript:myClickHandler('MyHtmlAnchorWithJavaScriptLink');" runat="server">MyHtmlAnchorWithJavaScriptLink</a>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>
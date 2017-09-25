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
<%@ Page Title="" Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="GenericTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.GenericTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>Initially hidden control</h3>
      <asp:Panel ID="PanelHiddenControl" runat="server" Visible="False">
        
      </asp:Panel>
      
      <h3>Initially shown control</h3>
      <asp:Panel ID="PanelVisibleControl" runat="server">
        
      </asp:Panel>
      
      <h3>Ambiguous controls</h3>
      <asp:Panel ID="PanelAmbiguousControl" runat="server">
        
      </asp:Panel>
      
      <h3>Frame test</h3>
      <iframe id="testFrame">
      </iframe>
    </ContentTemplate>
  </asp:UpdatePanel>
  <script>
    var target = document.getElementById("<%= PanelAmbiguousControl.ClientID %>");
    if (target)
    {
      var newTarget = target.cloneNode(true);
      newTarget.id += "2";
      target.parentNode.insertBefore (newTarget, target.nextSibling);
    }
  </script>
</asp:Content>
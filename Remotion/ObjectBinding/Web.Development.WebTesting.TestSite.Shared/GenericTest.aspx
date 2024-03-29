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
<%@ Page Title="" Language="C#" MasterPageFile="Layout.Master" AutoEventWireup="true" CodeBehind="GenericTest.aspx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.GenericTest" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Import Namespace="Remotion.ServiceLocation" %>
<%@ Import Namespace="Remotion.Web" %>
<%@ Import Namespace="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared" %>
<%@ Import Namespace="Remotion.Web.Development.WebTesting.TestSite.Infrastructure" %>
<asp:Content ContentPlaceHolderID="body" runat="server">

  <remotion:BindableObjectDataSourceControl ID="DataSource" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
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
      
      <h3>Disabled control</h3>
      <asp:Panel ID="PanelDisabledControl" runat="server">
        
      </asp:Panel>

      <h3>ReadOnly control</h3>
      <asp:Panel ID="PanelReadOnlyControl" runat="server">
        
      </asp:Panel>

      <h3>FormGrid control</h3>

      <h4>Standard FormGrid control</h4>
      <remotion:FormGridManager ID="FormGridManager" runat="server" />
      <table id="FormGrid" runat="server">
        <tr>
          <td></td>
          <td></td>
        </tr>
      </table>

      <h4>FormGrid control with readonly control</h4>
      <table id="ReadonlyControlFormGrid" runat="server">
        <tr>
          <td></td>
          <td></td>
        </tr>
      </table>

      <h4>One control over multiple rows inside a FormGrid control</h4>
      <table id="OneControlOverMultipleRowsFormGrid" runat="server">
        <tr>
          <td></td>
          <td></td>
        </tr>
        <tr>
          <td colspan="2"></td>
        </tr>
      </table>
      
      <h4>Form Grid Control with shifted Label and Control column </h4>
      <asp:PlaceHolder ID="ShiftedColumnsFormGridPlaceHolder" runat="server">
      </asp:PlaceHolder>

      <h4>Multiple controls in one row FormGrid control</h4>
      <asp:PlaceHolder ID="MultipleControlsFormGridPlaceHolder" runat="server">
      </asp:PlaceHolder>
      
      
      <h3>Validation</h3>
      <remotion:FormGridManager ID="FormGridManagerValidation" FormGridSuffix="Validation" runat="server"/>
      <table id="FormGridValidation" runat="server">
        <tr><td><span>Without Validator</span></td></tr>
        <tr>
          <td></td>
          <td></td>
        </tr>
        <tr><td><span>Custom Validator</span></td></tr>
        <tr>
          <td></td>
          <td></td>
        </tr>
        <tr><td><span>Multiple Validators</span></td></tr>
        <tr>
          <td></td>
          <td></td>
        </tr>
        <tr><td><span>Custom Validator + ReadOnly</span></td></tr>
        <tr>
          <td></td>
          <td></td>
        </tr>
      </table>
      <remotion:WebButton ID="ValidateButton" runat="server" Width="10em" Text="Validate"/>

      <h3>Frame test</h3>
      <iframe id="testFrame" src="<%= ResolveUrl(SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>().CreateResourceUrl (typeof (FrameTestFrame), TestResourceType.Root, "FrameTestFrame.aspx").GetUrl()) %>">
      </iframe>
    </ContentTemplate>
  </asp:UpdatePanel>
  <script>
    var target = document.getElementById("<%= PanelAmbiguousControl.ClientID %>");
    if (target)
    {
      var newTarget = target.cloneNode(true);
      newTarget.ID += "2";
      target.parentNode.insertBefore (newTarget, target.nextSibling);
    }
  </script>
</asp:Content>
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
<%@ Page Language="c#" Codebehind="Form.aspx.cs" AutoEventWireup="True" Inherits="OBWTest.IndividualControlTests.Form" 
  Title="IndividualControlTestForm" MasterPageFile="~/StandardMode.Master" %>
<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" AsyncPostBackTimeout="3600" />
    <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
    <remotion:SingleView ID="SingleView" runat="server">
      <TopControls>
        <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
        <asp:PlaceHolder ID="ButtonPlaceHolder" runat="server">
          <div>
            <remotion:WebButton ID="PostBackButton" runat="server" Text="Post Back"/>&nbsp;
            <remotion:WebButton ID="ValidateButton" runat="server" Width="10em" Text="Validate" />&nbsp;
            <remotion:WebButton ID="SaveButton" runat="server" Width="10em" Text="Save" />&nbsp;
            <remotion:WebButton ID="SaveAndRestartButton" runat="server" Width="10em" Text="Save &&amp;amp; Restart" />&nbsp;
            <remotion:WebButton ID="CancelButton" runat="server" Width="10em" Text="Cancel" />
          </div>
        </asp:PlaceHolder>
      </TopControls>
      
      <View>
        <remotion:WebUpdatePanel ID="UserControlUpdatePanel" runat="server" style="height: 100%">
          <contenttemplate>
            <asp:PlaceHolder ID="UserControlPlaceHolder" runat="server" />
          </contenttemplate>
        </remotion:WebUpdatePanel>
      </View>
      
      <BottomControls>
         <asp:UpdatePanel ID="StackUpdatePanel" runat="server" UpdateMode="Conditional">
          <contenttemplate>
            <asp:Literal ID="Stack" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
     </BottomControls>
    </remotion:SingleView>
</asp:Content>

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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="Remotion.ObjectBinding.Web.Test.Shared.ViewLayoutTests.Form" MasterPageFile="../StandardMode.Master" %>
<%@ Register tagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>

<asp:content contentplaceholderid="head" runat="server">
</asp:content>
<asp:content contentplaceholderid="body" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" AsyncPostBackTimeout="3600" />
    <remotion:SingleView ID="OuterSingleView" runat="server">
      <TopControls>
        <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
      </TopControls>
      
      <View>
      <div style="height: 100%; background-color: darkgrey">
        <div style="position: absolute; left:0; width: 33.4%; top: 0; height: 100%; background-color: red">
          <remotion:SingleView ID="InnerSingleView" runat="server">
            <TopControls>
            <div style="background: orange">
            Top Controls
            </div>
            </TopControls>
            
            <View>
            <div style="background: goldenrod; height: 100%">
            View
            </div>
            </View>
            
            <BottomControls>
            <div style="background: orange">
            Bottom Controls
            </div>
           </BottomControls>
          </remotion:SingleView>
        </div>
        
        <div style="position: absolute; right:0; width: 66.4%; top: 0; height: 100%; background-color: red">
          <remotion:TabbedMultiView ID="InnerMultiView" runat="server">
            <TopControls>
            <div style="background: cyan">
            Top Controls
            </div>
            </TopControls>
            
            <Views>
              <remotion:TabView Title="First Tab">
                <div style="background: lightskyblue; height: 100%">
                View
                </div>
              </remotion:TabView>
            </Views>
            
            <BottomControls>
            <div style="background: cyan">
            Bottom Controls
            </div>
           </BottomControls>
          </remotion:TabbedMultiView>
        </div>
      </div>
      </View>
      
      <BottomControls>
      <div style="background: lime">
      Bottom Controls
      </div>
     </BottomControls>
    </remotion:SingleView>
</asp:content>

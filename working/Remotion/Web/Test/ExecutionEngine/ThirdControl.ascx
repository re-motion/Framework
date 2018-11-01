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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThirdControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.ThirdControl" %>
<div style="background-color: #CCFFFF">
  Third Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteNextStepButton" runat="server" Text="Execute Next Step (Return)" OnClick="ExecuteNextStep_Click" />
    <remotion:WebButton ID="ExecuteFourthUserControlButton" runat="server" Text="Execute Fourth User Control" OnClick="ExecuteFourthUserControlButton_Click" /><br />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteThirdUserControlButton" runat="server" 
      Text="Throw invalid operation exception / Act as sink for previous button's event target" OnClick="ExecuteThirdUserControlButton_Click" />    
 </p>
</div>

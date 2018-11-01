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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecondControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.SecondControl" %>
<div style="background-color: #FFFFCC">
  Second Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteNextStepButton" runat="server" Text="Execute Next Step (Return)" OnClick="ExecuteNextStep_Click" />
    <remotion:WebButton ID="CancelButton" runat="server" Text="Cancel" OnClick="Cancel_Click" />
    <remotion:WebButton ID="ExecuteThirdUserControlButton" runat="server" Text="Execute Third User Control" OnClick="ExecuteThirdUserControlButton_Click" /><br />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" /><br />
    <remotion:ControlMock ID="SubControlWithState" runat="server" /><br />
    <asp:TextBox ID="SubControlWithFormElement" runat="server" EnableViewState="false" /><br />
    <remotion:WebButton ID="ExecuteSecondUserControlButton" runat="server" 
      Text="Throw invalid operation exception / Act as sink for previous button's event target" OnClick="ExecuteSecondUserControlButton_Click" />    
 </p>
</div>

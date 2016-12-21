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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FirstControl.ascx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.FirstControl" %>
<%@ Register TagPrefix="webTest" TagName="ControlWithAllDataTypes" Src="ControlWithAllDataTypes.ascx" %>

<div style="background-color:#ccffcc">
  <label>This is the first control.</label><BR />
  <label>Input parameter: </label><asp:Label id="GuidInLabel" runat="server" /><BR />
  <label>Output parameter: </label><asp:Label id="GuidOutLabel" runat="server" /><BR />
  <label>ClientTransaction: </label><asp:Label id="ClientTransactionLabel" runat="server" /><BR />
  <remotion:WebButton id="NonTransactionUserControlStepButton" runat="server" 
    Text="Call non-transacted control" 
    onclick="NonTransactionUserControlStepButton_Click" />
  <remotion:WebButton id="RootTransactionUserControlStepButton" runat="server" 
    Text="Call root-transacted control" 
    onclick="RootTransactionUserControlStepButton_Click" />
  <remotion:WebButton id="SubTransactionUserControlStepButton" runat="server" 
    Text="Call sub-transacted control" 
    onclick="SubTransactionUserControlStepButton_Click" />
  <br />
  <remotion:WebButton id="SaveButton" runat="server" 
    Text="Save" 
    onclick="SaveButton_Click" />
  <remotion:WebButton id="ReturnButton" runat="server" 
    Text="Return" 
    onclick="ReturnButton_Click" />
</div>
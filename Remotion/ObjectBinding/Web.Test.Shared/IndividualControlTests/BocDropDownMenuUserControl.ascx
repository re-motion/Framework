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
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BocDropDownMenuUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Test.Shared.IndividualControlTests.BocDropDownMenuUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
    <remotion:formgridmanager id=FormGridManager runat="server"/>
    <remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
    <remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
</div>
<table id=FormGrid runat="server">
<tr>
  <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></remotion:boctextvalue></td></tr>
<tr>
  <td></td>
  <td><remotion:BocDropDownMenu id=PartnerField runat="server" datasourcecontrol="CurrentObject" PropertyIdentifier="Partner" Width="20em" ControlServicePath="BocDropDownMenuWebService.asmx" ControlServiceArguments="ControlServiceArgs" /></td>
  <td>
    bound</td>
  <td style="WIDTH: 20%"></td></tr>
<tr>
  <td></td>
  <td><remotion:BocDropDownMenu id=UnboundField runat="server" Width="20em" ControlServicePath="BocDropDownMenuWebService.asmx" /></td>
  <td>
    unbound, value not set</td>
  <td style="WIDTH: 20%"></td></tr>
</table>
<p><asp:label id=MenuItemClickEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>

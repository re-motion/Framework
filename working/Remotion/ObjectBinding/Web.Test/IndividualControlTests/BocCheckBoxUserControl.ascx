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
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocCheckBoxUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocCheckBoxUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" id="NonVisualControls">
<remotion:formgridmanager id="FormGridManager" runat="server"/><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>
<table id="FormGrid" runat="server">
  <tr>
    <td colSpan="4"><remotion:boctextvalue id="FirstNameField" runat="server" datasourcecontrol="CurrentObject" ReadOnly="True" PropertyIdentifier="FirstName"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id="LastNameField" runat="server" datasourcecontrol="CurrentObject" ReadOnly="True" PropertyIdentifier="LastName"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="DeceasedField" runat="server" datasourcecontrol="CurrentObject" truedescription="ja" falsedescription="nein" propertyidentifier="Deceased" ShowDescription="True" AutoPostBack="True" ></remotion:boccheckbox></td>
    <td>bound, AutoPostBack, description=true</td>
    <td style="WIDTH: 20%"><asp:label id="DeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="ReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True" showdescription="True"></remotion:boccheckbox></td>
    <td>bound, read only, description= true</td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="UnboundDeceasedField" runat="server" Width="150px" AutoPostBack="True" ShowDescription="False"></remotion:boccheckbox></td>
    <td>unbound, value set</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="UnboundReadOnlyDeceasedField" runat="server" ReadOnly="True" Width="150px" height="24px"></remotion:boccheckbox></td>
    <td>unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="DisabledDeceasedField" runat="server" datasourcecontrol="CurrentObject" truedescription="ja" falsedescription="nein" propertyidentifier="Deceased" showdescription="True" enabled="false"></remotion:boccheckbox></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="DisabledReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True" showdescription="True" enabled="false"></remotion:boccheckbox></td>
    <td>disabled, bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="DisabledUnboundDeceasedField" runat="server" Width="150px" enabled="false"></remotion:boccheckbox></td>
    <td>disabled, unbound, value set</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boccheckbox id="DisabledUnboundReadOnlyDeceasedField" runat="server" ReadOnly="True" Width="150px" height="24px" enabled="false"></remotion:boccheckbox></td>
    <td>disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr></table>
<p>Deceased Field Checked Changed: <asp:label id="DeceasedFieldCheckedChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p>Unbound Deceased Field Checked Changed: <asp:label id="UnboundDeceasedFieldCheckedChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p><remotion:webbutton id="DeceasedTestSetNullButton" runat="server" width="220px" Text="Deceased Set Null"/><remotion:webbutton id="DeceasedTestToggleValueButton" runat="server" width="220px" Text="Deceased Toggle Value"/></p>
<p><remotion:webbutton id="ReadOnlyDeceasedTestSetNullButton" runat="server" width="220px" Text="Read Only Deceased Set Null"/><remotion:webbutton id="ReadOnlyDeceasedTestToggleValueButton" runat="server" width="220px" Text="Read Only Deceased Toggle Value"/></p>

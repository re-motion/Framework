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


<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocBooleanValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocBooleanValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id="DeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" FalseDescription="nope" NullDescription="häh?" TrueDescription="sicha" AutoPostBack="true" ></remotion:bocbooleanvalue></td>
    <td>bound, AutoPostBack</td>
    <td style="WIDTH: 20%"><asp:label id="DeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id="ReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True" showdescription="false"></remotion:bocbooleanvalue></td>
    <td>bound, read only, description=false</td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id=UnboundDeceasedField runat="server"  required="False" ></remotion:bocbooleanvalue></td>
    <td>unbound, value not set, required= false</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id=UnboundReadOnlyDeceasedField runat="server"  ReadOnly="True"></remotion:bocbooleanvalue></td>
    <td>unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id="DisabledDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" FalseDescription="nein" NullDescription="undefiniert" TrueDescription="ja" enabled=false></remotion:bocbooleanvalue></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id="DisabledReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True" enabled=false></remotion:bocbooleanvalue></td>
    <td>disabled, bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id=DisabledUnboundDeceasedField runat="server"  required="False" enabled=false></remotion:bocbooleanvalue></td>
    <td> disabled, unbound, value set, required= false</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id=DisabledUnboundReadOnlyDeceasedField runat="server"  ReadOnly="True" enabled=false></remotion:bocbooleanvalue></td>
    <td>disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr></table>
<p>Deceased Field Checked Changed: <asp:label id="DeceasedFieldCheckedChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p><remotion:webbutton id="DeceasedTestSetNullButton" runat="server" UseLegacyButton="true" Text="Deceased Set Null" width="220px"/><remotion:webbutton id="DeceasedTestToggleValueButton" runat="server" Text="Deceased Toggle Value" width="220px"/></p>
<p><remotion:webbutton id="ReadOnlyDeceasedTestSetNullButton" runat="server" Text="Read Only Deceased Set Null" width="220px"/><remotion:webbutton id="ReadOnlyDeceasedTestToggleValueButton" runat="server" Text="Read Only Deceased Toggle Value" width="220px"/></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/><remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>

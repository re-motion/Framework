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

<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocDateTimeValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocDateTimeValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
    <remotion:formgridmanager id=FormGridManager runat="server"/>
    <remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
    <remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
</div>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" ReadOnly="True" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" ReadOnly="True" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=BirthdayField runat="server" DateTextBoxStyle-AutoPostBack="true" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject" showseconds="False"></remotion:bocdatetimevalue></td>
          <td>
            bound</td>
          <td style="WIDTH: 20%"><asp:label id=BirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=ReadOnlyBirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject" readonly="True" showseconds="False"></remotion:bocdatetimevalue></td>
          <td>
            bound, read-only</td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="UnboundBirthdayFieldLabel" runat="server" ForControl="UnboundBirthdayField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id=UnboundBirthdayField runat="server" readonly="False" required="False" ShowSeconds="true"></remotion:bocdatetimevalue></td>
          <td>
            unbound, value not set, not required, with seconds</td>
          <td style="WIDTH: 20%"><asp:label id=UnboundBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="UnboundRequiredBirthdayFieldLabel" runat="server" ForControl="UnboundRequiredBirthdayField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id="UnboundRequiredBirthdayField" runat="server" required="True"></remotion:bocdatetimevalue></td>
          <td>
            unbound, value not set</td>
          <td style="WIDTH: 20%"><asp:label id="UnboundRequiredBirthdayFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="UnboundReadOnlyBirthdayFieldLabel" runat="server" ForControl="UnboundReadOnlyBirthdayField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id=UnboundReadOnlyBirthdayField runat="server" readonly="True"></remotion:bocdatetimevalue></td>
          <td>
            unbound, value set, read only</td>
          <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=DateOfDeathField runat="server" PropertyIdentifier="DateOfDeath" datasourcecontrol="CurrentObject" readonly="False"></remotion:bocdatetimevalue></td>
          <td>
            bound</td>
          <td style="WIDTH: 20%"><asp:label id=DateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=ReadOnlyDateOfDeathField runat="server" PropertyIdentifier="DateOfDeath" datasourcecontrol="CurrentObject" readonly="True"></remotion:bocdatetimevalue></td>
          <td>
            bound, read-only</td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyDateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="UnboundDateOfDeathFieldLabel" runat="server" ForControl="UnboundDateOfDeathField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id=UnboundDateOfDeathField runat="server" readonly="False" required="False"></remotion:bocdatetimevalue></td>
          <td>
            unbound, value not set, not required</td>
          <td style="WIDTH: 20%"><asp:label id=UnboundDateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="UnboundReadOnlyDateOfDeathFieldLabel" runat="server" ForControl="UnboundReadOnlyDateOfDeathField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id=UnboundReadOnlyDateOfDeathField runat="server" readonly="True"></remotion:bocdatetimevalue></td>
          <td>
            unbound, value set, read only</td>
          <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyDateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td>Today</td>
          <td><remotion:bocdatetimevalue id=DirectlySetBocDateTimeValueField runat="server" readonly="False" required="False" valuetype="Date"></remotion:bocdatetimevalue></td>
          <td>
            directly set, not required</td>
          <td style="WIDTH: 20%"><asp:label id=DirectlySetBocDateTimeValueFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td>Today</td>
          <td><remotion:bocdatetimevalue id=ReadOnlyDirectlySetBocDateTimeValueField runat="server" readonly="True" valuetype="Date"></remotion:bocdatetimevalue></td>
          <td>
            directly set, read only</td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=DisabledBirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject" showseconds="False" enabled=false></remotion:bocdatetimevalue></td>
          <td>
            disabled, bound</td>
          <td style="WIDTH: 20%"><asp:label id=DisabledBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=DisabledReadOnlyBirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject" readonly="True" showseconds="False" enabled=false></remotion:bocdatetimevalue></td>
          <td>
            disabled, bound, read-only</td>
          <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=CitizenshipField runat="server" PropertyIdentifier="DateOfCitizenship" datasourcecontrol="CurrentObject" readonly="False" showseconds="False" enabled=true></remotion:bocdatetimevalue></td>
          <td>
            date-only-type, bound</td>
          <td style="WIDTH: 20%"><asp:label id=CitizenshipFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td>
        </tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=ReadOnlyCitizenshipField runat="server" PropertyIdentifier="DateOfCitizenship" datasourcecontrol="CurrentObject" readonly="True" showseconds="False" enabled=true></remotion:bocdatetimevalue></td>
          <td>
            date-only-type, bound, read-only</td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyCitizenshipFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id=DisabledReadOnlyCitizenshipField runat="server" PropertyIdentifier="DateOfCitizenship" datasourcecontrol="CurrentObject" readonly="True" showseconds="False" enabled=false></remotion:bocdatetimevalue></td>
          <td>
            date-only-type, disabled, bound, read-only</td>
          <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyCitizenshipFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="UnboundCitizenshipFieldLabel" runat="server" ForControl="UnboundCitizenshipField" Text="Citizenship"/></td>
          <td><remotion:bocdatetimevalue id=UnboundCitizenshipField runat="server" readonly="False" required="False" ShowSeconds="false"></remotion:bocdatetimevalue></td>
          <td>
            date-only-type, unbound, value not set, not required</td>
          <td style="WIDTH: 20%"><asp:label id=UnboundCitizenshipFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="DisabledUnboundBirthdayFieldLabel" runat="server" ForControl="DisabledUnboundBirthdayField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id=DisabledUnboundBirthdayField runat="server" readonly="False" required="False" enabled=false></remotion:bocdatetimevalue></td>
          <td>
             disabled, unbound, value set, not required</td>
          <td style="WIDTH: 20%"><asp:label id=DisabledUnboundBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td><remotion:SmartLabel ID="DisabledUnboundReadOnlyBirthdayFieldLabel" runat="server" ForControl="DisabledUnboundReadOnlyBirthdayField" Text="Birthday"/></td>
          <td><remotion:bocdatetimevalue id=DisabledUnboundReadOnlyBirthdayField runat="server" readonly="True" enabled=false></remotion:bocdatetimevalue></td>
          <td>
            disabled, unbound, value set, read only</td>
          <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr></table>
      <p><remotion:webbutton id=BirthdayTestSetNullButton runat="server" width="220px" Text="Birthday Set Null"/><remotion:webbutton id=BirthdayTestSetNewValueButton runat="server" width="220px" Text="Birthday Set New Value"/></p>
      <p>Birthday Field Date Time Changed Label: <asp:label id=BirthdayFieldDateTimeChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
      <p><br /><remotion:webbutton id=ReadOnlyBirthdayTestSetNullButton runat="server" width="220px" Text="Read Only Birthday Set Null"/><remotion:webbutton id=ReadOnlyBirthdayTestSetNewValueButton runat="server" width="220px" Text="Read Only Birthday Set New Value"/></p>

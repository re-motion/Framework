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
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocAutoCompleteReferenceValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/>
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
<remotion:BindableObjectDataSourceControl id="PersonDataSource" runat="server" Type="Remotion.ObjectBinding.Sample::Person" Mode="Search"/>
<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
</div>

<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" readonly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName">
</remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" readonly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=PartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" ControlServiceArguments="ControlServiceArgs"
    TextBoxStyle-AutoPostBack="true" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Partner" CompletionSetCount="5" OnSelectionChanged="Control_SelectionChanged">
<optionsmenuitems>
<remotion:BocMenuItem Text="intern">
</remotion:BocMenuItem>
<remotion:BocMenuItem Text="extern">
</remotion:BocMenuItem>
</OptionsMenuItems>

<labelstyle cssclass="MyLabelClass">
</LabelStyle></remotion:bocAutoCompleteReferenceValue>
</td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id=PartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=ReadOnlyPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" readonly="True" datasourcecontrol="CurrentObject" propertyidentifier="Partner" >
    </remotion:bocAutoCompleteReferenceValue></td>
    <td>bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td><remotion:SmartLabel ID="UnboundPartnerFieldLabel" runat="server" ForControl="UnboundPartnerField" Text="Partner"/></td>
    <td><remotion:bocAutoCompleteReferenceValue id=UnboundPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" DataSourceControl="PersonDataSource" required="True" showoptionsmenu="False"  ValidSearchStringRegex=".{1}" IgnoreSearchStringForDropDownUponValidInput="True">
</remotion:bocAutoCompleteReferenceValue></td>
    <td>
       unbound, value not set</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td><remotion:SmartLabel ID="UnboundReadOnlyPartnerFieldLabel" runat="server" ForControl="UnboundReadOnlyPartnerField" Text="Partner"/></td>
    <td><remotion:bocAutoCompleteReferenceValue id=UnboundReadOnlyPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" readonly="True" enableicon="False" DataSourceControl="PersonDataSource">
</remotion:bocAutoCompleteReferenceValue></td>
    <td>
      unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Partner" enabled="false">
</remotion:bocAutoCompleteReferenceValue></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledReadOnlyPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" readonly="True" datasourcecontrol="CurrentObject" propertyidentifier="Partner" enabled="false">
</remotion:bocAutoCompleteReferenceValue></td>
    <td>disabled, bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td><remotion:SmartLabel ID="DisabledUnboundPartnerFieldLabel" runat="server" ForControl="DisabledUnboundPartnerField" Text="Partner"/></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledUnboundPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" required="True" enabled="false" DataSourceControl="PersonDataSource">
</remotion:bocAutoCompleteReferenceValue></td>
    <td>
       disabled, unbound, value set</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td><remotion:SmartLabel ID="DisabledUnboundReadOnlyPartnerFieldLabel" runat="server" ForControl="DisabledUnboundReadOnlyPartnerField" Text="Partner"/></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledUnboundReadOnlyPartnerField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" readonly="True" enableicon="False" enabled="false" DataSourceControl="PersonDataSource">
</remotion:bocAutoCompleteReferenceValue></td>
    <td>
      disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=FatherField runat="server" ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx" ControlServiceArguments="ControlServiceArgs"
    TextBoxStyle-AutoPostBack="true" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Father" CompletionSetCount="5" OnSelectionChanged="Control_SelectionChanged" />
    </td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id=FatherFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr> </table>
<p>Partner Command Click: <asp:label id="PartnerCommandClickLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p>Partner Selection Changed: <asp:label id=PartnerFieldSelectionChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
<p>Partner Menu Click: <asp:label id=PartnerFieldMenuClickEventArgsLabel runat="server" enableviewstate="False">#</asp:label></p>
<p><br /><remotion:webbutton id=PartnerTestSetNullButton runat="server" width="220px" Text="Partner Set Null"/><remotion:webbutton id=PartnerTestSetNewItemButton runat="server" width="220px" Text="Partner Set New Item"/></p>
<p><remotion:webbutton id=ReadOnlyPartnerTestSetNullButton runat="server" width="220px" Text="Read Only Partner Set Null"/><remotion:webbutton id=ReadOnlyPartnerTestSetNewItemButton runat="server" width="220px" Text="Read Only Partner Set New Item"/></p>

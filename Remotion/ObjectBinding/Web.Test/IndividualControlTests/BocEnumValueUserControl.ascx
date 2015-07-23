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


<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocEnumValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocEnumValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/>
<remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
<remotion:BindableObjectDataSourceControl id=EnumObject runat="server" Type="Remotion.ObjectBinding.Sample::ClassWithEnums"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" readonly="True"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=GenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" >
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" AutoPostBack="true" >
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      bound, radio buttons, AutoPostBack, required=true</td>
    <td style="WIDTH: 20%"><asp:label id=GenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=ReadOnlyGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" ReadOnly="True" Required="True" >
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" >
</listcontrolstyle>
</remotion:bocenumvalue></td>
    <td>
      bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=MarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" datasourcecontrol="CurrentObject" required="False">
<listcontrolstyle radiobuttonlistrepeatdirection="Horizontal"  AutoPostBack="true">
</ListControlStyle>
            </remotion:bocenumvalue></td>
    <td>
      bound, drop-down, AutoPostBack, required=false</td>
    <td style="WIDTH: 20%"><asp:label id=MarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=UnboundMarriageStatusField runat="server" >
<listcontrolstyle listboxrows="2" controltype="ListBox"  AutoPostBack="true" >
</ListControlStyle>
            </remotion:bocenumvalue></td>
    <td>
      <p>unbound, value not set, list-box, AutoPostBack, 
    required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=UnboundReadOnlyMarriageStatusField runat="server" ReadOnly="True">
              <listcontrolstyle radionbuttonlistrepeatlayout="Table" controltype="ListBox"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      unbound, AutoPostBack value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DeceasedAsEnumField runat="server" PropertyIdentifier="Deceased" datasourcecontrol="CurrentObject" required="False">
              <listcontrolstyle radiobuttonlistrepeatdirection="Horizontal"></ListControlStyle>
            </remotion:bocenumvalue></td>
    <td>deceased (bool) as enum</td>
    <td style="WIDTH: 20%"><asp:label id=DeceasedAsEnumFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" enabled=false>
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      disabled, bound, radio buttons, required=true</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledReadOnlyGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" ReadOnly="True" Required="True" enabled=false>
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      disabled, bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledMarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" datasourcecontrol="CurrentObject" required="False" enabled=false>
              <listcontrolstyle radionbuttonlistrepeatlayout="Table" radiobuttonlistrepeatdirection="Horizontal"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      disabled, bound, drop-down, required=false</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledUnboundMarriageStatusField runat="server" enabled=false>
              <listcontrolstyle radiobuttonlisttextalign="Right" listboxrows="2" radionbuttonlistrepeatlayout="Table"
                controltype="ListBox" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      <p> disabled, unbound, value&nbsp;set, list-box, 
    required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledUnboundReadOnlyMarriageStatusField runat="server" ReadOnly="True" enabled=false>
              <listcontrolstyle radionbuttonlistrepeatlayout="Table" controltype="ListBox"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>Instance Enum</td>
    <td><remotion:bocenumvalue id="InstanceEnumField" runat="server">
            </remotion:bocenumvalue></td>
    <td>
       unbound</td>
    <td style="WIDTH: 20%"><asp:label id="InstanceEnumFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumNotNullable</td>
    <td><remotion:bocenumvalue id="EnumNotNullableDropDownField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumNotNullable"></remotion:bocenumvalue></td>
    <td>
       bound, drop-down</td>
    <td style="WIDTH: 20%"><asp:label id="EnumNotNullableDropDownLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumNullable</td>
    <td><remotion:bocenumvalue id="EnumNullableDropDownField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumNullable"></remotion:bocenumvalue></td>
    <td>
       bound, drop-down</td>
    <td style="WIDTH: 20%"><asp:label id="EnumNullableDropDownLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumUndefined</td>
    <td><remotion:bocenumvalue id="EnumUndefinedDropDownField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumUndefined"></remotion:bocenumvalue></td>
    <td>
       bound, drop-down</td>
    <td style="WIDTH: 20%"><asp:label id="EnumUndefinedDropDownLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumNotNullable</td>
    <td><remotion:bocenumvalue id="EnumNotNullableRadioButtonField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumNotNullable" 
        style="font-weight: 700">
      <ListControlStyle ControlType="RadioButtonList" RadioButtonListRepeatDirection="Horizontal" />
      </remotion:bocenumvalue></td>
    <td>
       bound, radio buttons</td>
    <td style="WIDTH: 20%"><asp:label id="EnumNotNullableRadioButtonLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumNullable</td>
    <td><remotion:bocenumvalue id="EnumNullableRadioButtonField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumNullable">
      <ListControlStyle ControlType="RadioButtonList" RadioButtonListRepeatDirection="Horizontal" />
      </remotion:bocenumvalue></td>
    <td>
       bound, radio buttons</td>
    <td style="WIDTH: 20%"><asp:label id="EnumNullableRadioButtonLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumUndefined</td>
    <td><remotion:bocenumvalue id="EnumUndefinedRadioButtonField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumUndefined">
      <ListControlStyle ControlType="RadioButtonList" RadioButtonListRepeatDirection="Horizontal"  /></remotion:bocenumvalue></td>
    <td>
       bound, radio buttons</td>
    <td style="WIDTH: 20%"><asp:label id="EnumUndefinedRadioButtonLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumNotNullable</td>
    <td><remotion:bocenumvalue id="EnumNotNullableRadioButtonWithoutNullvalueField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumNotNullable" 
        style="font-weight: 700">
      <ListControlStyle ControlType="RadioButtonList" RadioButtonListRepeatDirection="Horizontal" RadioButtonListNullValueVisible="False" />
      </remotion:bocenumvalue></td>
    <td>
       bound, radio buttons, nullvalue not visible</td>
    <td style="WIDTH: 20%"><asp:label id="EnumNotNullableRadioButtonWithoutNullvalueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumNullable</td>
    <td><remotion:bocenumvalue id="EnumNullableRadioButtonWithoutNullvalueField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumNullable">
      <ListControlStyle ControlType="RadioButtonList" RadioButtonListRepeatDirection="Horizontal" RadioButtonListNullValueVisible="False" />
      </remotion:bocenumvalue></td>
    <td>
       bound, radio buttons, nullvalue not visible</td>
    <td style="WIDTH: 20%"><asp:label id="EnumNullableRadioButtonWithoutNullvalueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>EnumUndefined</td>
    <td><remotion:bocenumvalue id="EnumUndefinedRadioButtonWithoutNullvalueField" runat="server" DataSourceControl="EnumObject" PropertyIdentifier="EnumUndefined">
      <ListControlStyle ControlType="RadioButtonList" RadioButtonListRepeatDirection="Horizontal" RadioButtonListNullValueVisible="False"  /></remotion:bocenumvalue></td>
    <td>
       bound, radio buttons, nullvalue not visible</td>
    <td style="WIDTH: 20%"><asp:label id="EnumUndefinedRadioButtonWithoutNullvalueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
    </table>
<p><br />Gender Selection Changed: <asp:label id=GenderFieldSelectionChangedLabel runat="server" EnableViewState="False">#</asp:label></p>
<p><remotion:webbutton id=GenderTestSetNullButton runat="server" Text="Gender Set Null" width="165px"/><remotion:webbutton id=GenderTestSetDisabledGenderButton runat="server" Text="Gender Set Disabled Gender" width="165px"/><remotion:webbutton id=GenderTestSetMarriedButton runat="server" Text="Gender Set Married" width="165px"/></p>
<p><br /><remotion:webbutton id=ReadOnlyGenderTestSetNullButton runat="server" Text="Read Only Gender Set Null" width="220px"/><remotion:webbutton id=ReadOnlyGenderTestSetNewItemButton runat="server" Text="Read Only Gender Set Female" width="220px"/></p>

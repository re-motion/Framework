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

<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonDetailsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonDetailsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<table id="FormGrid" runat="server" style="MARGIN-TOP: 0%">
  <tr>
    <td></td>
    <td><remotion:boctextvalue id="LastNameField" required="true" runat="server" propertyidentifier="LastName" datasourcecontrol="CurrentObject">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id="FirstNameField" runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocdatetimevalue id="DateOfBirthField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="CurrentObject" >
<datetimetextboxstyle autopostback="True">
</DateTimeTextBoxStyle></remotion:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject" AutoPostBack="True"></remotion:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocdatetimevalue id="DateOfDeathField" runat="server" propertyidentifier="DateOfDeath" datasourcecontrol="CurrentObject" ></remotion:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id="MarriageStatusField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" >
<listcontrolstyle autopostback="True">
</ListControlStyle></remotion:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocreferencevalue id="PartnerField" runat="server" propertyidentifier="Partner" datasourcecontrol="CurrentObject" >
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<PersistedCommand>
<remotion:BocCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></remotion:BocCommand>
</PersistedCommand></remotion:bocreferencevalue></td></tr>
</table>
<table id="LabeltestFormGrid" runat="server">
  <tr>
    <td colspan="2">Label Test</td>
  </tr>
  <tr>
    <td><remotion:SmartLabel runat="server" ForControl="TextFieldWithSmartLabel" Text="&First Hotkey"/></td>
    <td><remotion:boctextvalue id="TextFieldWithSmartLabel" runat="server"></remotion:boctextvalue></td></tr>
  <tr>
    <td><remotion:FormGridLabel runat="server" ForControl="TextFieldWithFormGridLabel" Text="&Second Hotkey"/></td>
    <td><asp:TextBox id="TextFieldWithFormGridLabel" runat="server"></asp:TextBox></td></tr>
  <tr>
    <td><asp:Label runat="server" ForControl="TextFieldWithLabel" Text="&Third Hotkey"/></td>
    <td><asp:TextBox id="TextFieldWithLabel" runat="server"></asp:TextBox></td></tr>
</table>
<remotion:WebButton runat="server" ID="ShowExtraFormGridButton" OnClick="ShowExtraFormGridButton_Click" Text="Create FormGrid"/>
<asp:PlaceHolder runat="server" ID="ExtraFormGridPlaceHolder"></asp:PlaceHolder>
<p><remotion:formgridmanager id="FormGridManager" runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" /></p>

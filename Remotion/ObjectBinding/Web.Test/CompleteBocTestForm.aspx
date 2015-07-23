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


<%@ Page language="c#" Codebehind="CompleteBocTestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocForm" MasterPageFile="~/StandardMode.Master" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
<h1>CompleteBocTest: Form, No UserControl</h1>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" ReadOnly="True"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" ReadOnly="True"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=TextField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="FirstName" errormessage="Fehler">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocmultilinetextvalue id=MultilineTextField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="CV" DESIGNTIMEDRAGDROP="37" errormessage="Fehler">
<textboxstyle textmode="MultiLine" autopostback="True">
</TextBoxStyle></remotion:bocmultilinetextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocdatetimevalue id=DateTimeField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="DateOfBirth" errormessage="Fehler">
<datetextboxstyle autopostback="True">
</DateTextBoxStyle></remotion:bocdatetimevalue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><remotion:bocenumvalue id=EnumField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" errormessage="Fehler">
<listcontrolstyle autopostback="True">
</ListControlStyle></remotion:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocreferencevalue id=ReferenceField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Partner" NullItemErrorMessage="Fehler">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<PersistedCommand>
<remotion:BocCommand Type="None"></remotion:BocCommand>
</PersistedCommand></remotion:bocreferencevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id=BooleanField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" errormessage="Fehler" AutoPostBack="True"></remotion:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><remotion:boclist id=ListField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Jobs" showsortingorder="True" alwaysshowpageinfo="True">
<fixedcolumns>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Title">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns></remotion:boclist></td></tr></table>
<p><remotion:formgridmanager id=FormGridManager runat="server" visible="true"></remotion:formgridmanager>
<remotion:BindableObjectDataSourceControl  id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person" /></p>
<p><asp:button id=SaveButton runat="server" Text="Save" Width="80px"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></p>
</asp:Content>

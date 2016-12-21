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
<%@ Page language="c#" Codebehind="PersonDetailsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonDetailsForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>PersonDetails Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <remotion:htmlheadcontents runat="server" id="HtmlHeadContents"></remotion:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>PersonDetails Form</h1>
      <table id=FormGrid runat="server" width="100%">
        <tr>
          <td colSpan=2>Persondetails</td></tr>
        <tr>
          <td></td>
          <td><remotion:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" Width="100%" required="True">
<textboxstyle textmode="SingleLine">
</textBoxStyle></remotion:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" Width="100%" required="True">
<textboxstyle textmode="SingleLine">
</textBoxStyle></remotion:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocenumvalue id="GenderField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Gender" width="100%">
<listcontrolstyle>
</listControlStyle></remotion:bocenumvalue></td></tr>
        <tr>
          <td style="HEIGHT: 14px"></td>
          <td style="HEIGHT: 14px"><remotion:bocreferencevalue id="PartnerField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Partner" width="100%">

<dropdownliststyle autopostback="True">
</DropDownListStyle>

<PersistedCommand>
<remotion:BocCommand Type="Event"></remotion:BocCommand>
</PersistedCommand></remotion:bocreferencevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:BocTextValue id="ParterFirstNameField" runat="server" DataSourceControl="PartnerDataSource" propertyidentifier="FirstName" width="100%">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</remotion:BocTextValue></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocdatetimevalue id="BirthdayField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="DateOfBirth" width="100%"></remotion:bocdatetimevalue></td></tr>
        <tr>
          <td></td>
          <td><remotion:BocBooleanValue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject" width="100%"></remotion:BocBooleanValue></td></tr>
        <tr>
          <td></td>
          <td><remotion:BocMultilineTextValue id="CVField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" width="100%">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></remotion:BocMultilineTextValue></td></tr>
        <tr>
          <td style="HEIGHT: 17px"></td>
          <td style="HEIGHT: 17px"><remotion:BocList id="JobList" runat="server" PropertyIdentifier="Jobs" DataSourceControl="CurrentObject" ShowAvailableViewsList="false" ShowAllProperties="True">
</remotion:BocList></td></tr>
          </table>
      <p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><remotion:formgridmanager id=FormGridManager runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" /><remotion:BusinessObjectReferenceDataSourceControl id="PartnerDataSource" runat="server" PropertyIdentifier="Partner" DataSourceControl="CurrentObject"></remotion:BusinessObjectReferenceDataSourceControl></p></form>

  </body>
</html>

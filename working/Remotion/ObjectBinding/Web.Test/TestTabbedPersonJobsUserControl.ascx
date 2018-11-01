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
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonJobsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonJobsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>





<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td><remotion:bocmultilinetextvalue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject"></remotion:bocmultilinetextvalue></td>
  </tr>
  <tr>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td colspan="2"><remotion:boclist id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="CurrentObject" showsortingorder="True" alwaysshowpageinfo="True" selection="Multiple" >
<fixedcolumns>
<remotion:BocRowEditModeColumnDefinition SaveText="Save" CancelText="Cancel" EditText="Edit"></remotion:BocRowEditModeColumnDefinition>
<remotion:BocCommandColumnDefinition Text="Event">
<persistedcommand>
<remotion:BocListItemCommand Type="Event"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<remotion:BocListItemCommand Type="Href" HrefCommand-Href="Start.aspx"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
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
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EndDate">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns></remotion:boclist></td>
</tr>
</table>
<p><remotion:formgridmanager id="FormGridManager" runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"></remotion:BindableObjectDataSourceControl></p>

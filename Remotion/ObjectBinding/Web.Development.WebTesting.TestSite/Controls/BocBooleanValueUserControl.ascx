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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocBooleanValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocBooleanValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocBooleanValue ID="DeceasedField_Normal"
                                DataSourceControl="CurrentObject"
                                FalseDescription="Is_So_False"
                                NullDescription="Is_So_Null"
                                PropertyIdentifier="Deceased"
                                TrueDescription="Is_So_True"

                                AutoPostBack="true"
                                Enabled="true"
                                ReadOnly="false"
                                Required="true"
        
                                runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocBooleanValue ID="DeceasedField_ReadOnly"
                                DataSourceControl="CurrentObject"
                                FalseDescription="Is_So_False"
                                NullDescription="Is_So_Null"
                                PropertyIdentifier="Deceased"
                                TrueDescription="Is_So_True"

                                AutoPostBack="true"
                                Enabled="true"
                                ReadOnly="true"
                                Required="true"
        
                                runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocBooleanValue ID="DeceasedField_Disabled"
                                DataSourceControl="CurrentObject"
                                FalseDescription="Is_So_False"
                                NullDescription="Is_So_Null"
                                PropertyIdentifier="Deceased"
                                TrueDescription="Is_So_True"

                                AutoPostBack="true"
                                Enabled="false"
                                ReadOnly="false"
                                Required="true"
        
                                runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocBooleanValue ID="DeceasedField_NoAutoPostBack"
                                DataSourceControl="CurrentObject"
                                FalseDescription="Is_So_False"
                                NullDescription="Is_So_Null"
                                PropertyIdentifier="Deceased"
                                TrueDescription="Is_So_True"

                                AutoPostBack="false"
                                Enabled="true"
                                ReadOnly="false"
                                Required="true"
        
                                runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocBooleanValue ID="DeceasedField_TriState"
                                DataSourceControl="CurrentObject"
                                FalseDescription="Is_So_False"
                                NullDescription="Is_So_Null"
                                PropertyIdentifier="Deceased"
                                TrueDescription="Is_So_True"

                                AutoPostBack="true"
                                Enabled="true"
                                ReadOnly="false"
                                Required="false"
        
                                runat="server"/>
    </td>
    <td>(not required, tri-state)</td>
  </tr>
</table>
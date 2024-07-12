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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocDateTimeValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocDateTimeValueUserControl" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls.Validation" Assembly="Remotion.ObjectBinding.Web" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_Normal"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="false"
        
                                 ShowSeconds="false"
                                 ValueType="DateTime"

                                 runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_ReadOnly"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="true"
        
                                 ShowSeconds="false"
                                 ValueType="DateTime"

                                 runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_Disabled"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="false"
                                 ReadOnly="false"
        
                                 ShowSeconds="false"
                                 ValueType="DateTime"

                                 runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_NoAutoPostBack"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="false"
                                 Enabled="true"
                                 ReadOnly="false"
        
                                 ShowSeconds="false"
                                 ValueType="DateTime"

                                 runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_DateOnly"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="false"
        
                                 ShowSeconds="false"
                                 ValueType="Date"

                                 runat="server"/>
    </td>
    <td>(date-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_ReadOnlyDateOnly"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="true"
        
                                 ShowSeconds="false"
                                 ValueType="Date"

                                 runat="server"/>
    </td>
    <td>(read-only, date-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_WithSeconds"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfBirth"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="false"
        
                                 ShowSeconds="true"
                                 ValueType="DateTime"

                                 runat="server"/>
    </td>
    <td>(with seconds)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfCitizenship_DateOnlyType"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfCitizenship"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="false"

                                 runat="server"/>
    </td>
    <td>(DateOnly type)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfCitizenship_ReadOnlyDateOnlyType"
                                 DataSourceControl="CurrentObject"
                                 PropertyIdentifier="DateOfCitizenship"

                                 DateTimeTextBoxStyle-AutoPostBack="true"
                                 Enabled="true"
                                 ReadOnly="true"

                                 runat="server"/>
    </td>
    <td>(read-only, DateOnly type)</td>
  </tr>
</table>
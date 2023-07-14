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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocTextValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocTextValueUserControl" %>
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
      <remotion:BocTextValue
        ID="LastNameField_Normal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName"
        TextBoxStyle-AutoPostBack="true"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="LastNameField_ReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName"
        TextBoxStyle-AutoPostBack="true"
        ReadOnly="true"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="LastNameField_Disabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName"
        TextBoxStyle-AutoPostBack="true"
        Enabled="false"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="LastNameField_NoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName"
        TextBoxStyle-AutoPostBack="false"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="LastNameField_PasswordRenderMasked"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName"
        TextBoxStyle-AutoPostBack="true"
        TextBoxStyle-TextMode="PasswordRenderMasked"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(password, render masked)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="LastNameField_PasswordNoRender"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName"
        TextBoxStyle-AutoPostBack="true"
        TextBoxStyle-TextMode="PasswordNoRender"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(password, no render)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="CVString_Normal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CVString"
        TextBoxStyle-AutoPostBack="true"
        TextBoxStyle-TextMode="MultiLine"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="CVString_ReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CVString"
        TextBoxStyle-AutoPostBack="true"
        TextBoxStyle-TextMode="MultiLine"
        ReadOnly="true"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="CVString_Disabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CVString"
        TextBoxStyle-AutoPostBack="true"
        TextBoxStyle-TextMode="MultiLine"
        Enabled="false"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="CVString_NoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CVString"
        TextBoxStyle-AutoPostBack="false"
        TextBoxStyle-TextMode="MultiLine"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr>
    <tr>
      <td></td>
      <td>
        <remotion:BocTextValue
            ID="LastNameField_Normal_Required"
            DataSourceControl="CurrentObject"
            PropertyIdentifier="LastName"
            TextBoxStyle-AutoPostBack="true"
            ValueType="String"
            Required="true"
            EnableOptionalValidators="true"
            runat="server"/>
      </td>
      <td>(normal, required)</td>
    </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue
        ID="UnboundText"
        ValueType="String"
        runat="server"/>
    </td>
    <td>(unbound)</td>
  </tr>
</table>
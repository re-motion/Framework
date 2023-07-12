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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocEnumValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocEnumValueUserControl" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls.Validation" Assembly="Remotion.ObjectBinding.Web" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControl ID="NoObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" Mode="Search" />
<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListNormal"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="DropDownList"
                             ListControlStyle-DropDownListNullValueTextVisible="true"
                             ReadOnly="false"
                             Required="false"
        
                             runat="server"/>
    </td>
    <td>(DropDownList, normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListReadOnly"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="DropDownList"
                             ReadOnly="true"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(DropDownList, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListReadOnlyWithoutSelectedValue"
                             DataSourceControl="NoObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="DropDownList"
                             ReadOnly="true"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(DropDownList, read-only without selected value)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListDisabled"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="false"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="DropDownList"
                             ReadOnly="false"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(DropDownList, disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListNoAutoPostBack"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="false"
                             ListControlStyle-ControlType="DropDownList"
                             ReadOnly="false"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(DropDownList, no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxNormal"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             ListControlStyle-ListBoxRows="2"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="ListBox"
                             ReadOnly="false"
                             Required="false"
        
                             runat="server"/>
    </td>
    <td>(ListBox, normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxReadOnly"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             ListControlStyle-ListBoxRows="2"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="ListBox"
                             ReadOnly="true"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(ListBox, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxReadOnlyWithoutSelectedValue"
                             DataSourceControl="NoObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             ListControlStyle-ListBoxRows="2"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="ListBox"
                             ReadOnly="true"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(ListBox, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxDisabled"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             ListControlStyle-ListBoxRows="2"

                             Enabled="false"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="ListBox"
                             ReadOnly="false"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(ListBox, disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxNoAutoPostBack"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             ListControlStyle-ListBoxRows="2"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="false"
                             ListControlStyle-ControlType="ListBox"
                             ReadOnly="false"
                             Required="true"
        
                             runat="server"/>
    </td>
    <td>(ListBox, no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListNormal"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="false"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListReadOnly"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="true"
                             Required="true"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListReadOnlyWithoutSelectedValue"
                             DataSourceControl="NoObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="true"
                             Required="true"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListDisabled"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="false"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="true"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListNoAutoPostBack"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"
        
                             Enabled="true"
                             ListControlStyle-AutoPostBack="false"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="true"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListMultiColumn"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="false"
        
                             ListControlStyle-RadioButtonListRepeatColumns="2"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, multi-column)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListFlow"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="false"
        
                             ListControlStyle-RadioButtonListRepeatColumns="2"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="Flow"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, flow, multi-column)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListOrderedList"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="false"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="OrderedList"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, ordered list)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListUnorderedList"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="false"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Right"
                             ListControlStyle-RadionButtonListRepeatLayout="UnorderedList"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, unordered list)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListLabelLeft"
                             DataSourceControl="CurrentObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="false"
        
                             ListControlStyle-RadioButtonListRepeatColumns="1"
                             ListControlStyle-RadioButtonListTextAlign="Left"
                             ListControlStyle-RadionButtonListRepeatLayout="Table"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, label left)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListRequiredWithoutSelectedValue"
                             DataSourceControl="NoObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ReadOnly="false"
                             Required="true"
                             EnableOptionalValidators="true"
                             runat="server"/>
    </td>
    <td>(RadioButtonList, required without selected value)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListWithoutSelectedValueAndWithoutVisibleNullValue"
                             DataSourceControl="NoObject"
                             PropertyIdentifier="MarriageStatus"
                             UndefinedItemText="Is_So_Undefined"

                             Enabled="true"
                             ListControlStyle-AutoPostBack="true"
                             ListControlStyle-ControlType="RadioButtonList"
                             ListControlStyle-RadioButtonListNullValueVisible="false"
                             ReadOnly="false"
                             Required="false"
        
                             runat="server"/>
    </td>
    <td>(RadioButtonList, required without selected value)</td>
  </tr>
</table>
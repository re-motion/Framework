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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocReferenceValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocReferenceValueUserControl" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls.Validation" Assembly="Remotion.ObjectBinding.Web" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControl ID="NoObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" Mode="Search" />
<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
<div>
    <remotion:WebButton ID="PostbackButton" runat="server" Width="10em" Text="Postback" OnClick="PostbackButton_OnClick" />
    <remotion:WebButton ID="ResetBusinessObjectListButton" runat="server" Width="10em" Text="Reset Items" OnClick="ResetBusinessObjectListButton_OnClick" />
    <remotion:WebButton ID="SetEmptyBusinessObjectListButton" runat="server" Width="10em" Text="Set Items Empty" OnClick="SetEmptyBusinessObjectListButton_OnClick" />
</div>
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_Normal"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  ControlServicePath="BocReferenceValueWebService.asmx"
                                  runat="server">
        
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3" RequiredSelection="ExactlyOne">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocReferenceValue>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_ReadOnly"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="True"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  ControlServicePath="BocReferenceValueWebService.asmx"
                                  runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocReferenceValue>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_ReadOnlyWithoutSelectedValue"
                                  ReadOnly="True"
                                  DataSourceControl="NoObject"
                                  PropertyIdentifier="Partner"
                                  runat="server"/>
    </td>
    <td>(read-only without selected value)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_Disabled"
                                  DropDownListStyle-AutoPostBack="true"
                                  Enabled="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  ControlServicePath="BocReferenceValueWebService.asmx"
                                  runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocReferenceValue>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_NoAutoPostBack"
                                  DropDownListStyle-AutoPostBack="false"
                                  ReadOnly="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  ControlServicePath="BocReferenceValueWebService.asmx"
                                  runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocReferenceValue>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_NoCommandNoMenu"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  ControlServicePath="BocReferenceValueWebService.asmx"
                                  runat="server">
        
      </remotion:BocReferenceValue>
    </td>
    <td>(no command & no menu)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_Required"
                                  DropDownListStyle-AutoPostBack="true"
                                  Required="true"
                                  ReadOnly="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  EnableIcon="false"
                                  runat="server">
      </remotion:BocReferenceValue>
    </td>
    <td>(required, no icon)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_WithoutSelectedValue"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="False"
                                  DataSourceControl="NoObject"
                                  PropertyIdentifier="Partner"
                                  runat="server">
      </remotion:BocReferenceValue>
    </td>
    <td>(without selected value)</td>
  </tr>
<tr>
  <td></td>
  <td>
    <remotion:BocReferenceValue ID="PartnerField_WithoutSelectedValue_Required"
                                DropDownListStyle-AutoPostBack="true"
                                ReadOnly="False"
                                DataSourceControl="NoObject"
                                PropertyIdentifier="Partner"
                                
                                Required="true"
                                EnableOptionalValidators="true"
                                runat="server">
    </remotion:BocReferenceValue>
  </td>
  <td>(without selected value, required)</td>
</tr>
</table>
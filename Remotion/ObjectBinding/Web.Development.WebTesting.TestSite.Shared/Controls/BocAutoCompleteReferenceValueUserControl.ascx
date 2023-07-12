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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocAutoCompleteReferenceValueUserControl" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls.Validation" Assembly="Remotion.ObjectBinding.Web" %>
<%@ Import Namespace="System" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_Normal"
                                              TextBoxStyle-AutoPostBack="true"
                                              ReadOnly="False"
                                              DataSourceControl="CurrentObject"
                                              PropertyIdentifier="Partner"
                                              CompletionSetCount="5"
                                              ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"
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
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_ReadOnly"
                                              TextBoxStyle-AutoPostBack="true"
                                              ReadOnly="True"
                                              DataSourceControl="CurrentObject"
                                              PropertyIdentifier="Partner"
                                              CompletionSetCount="5"
                                              ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"
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
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_Disabled"
                                              TextBoxStyle-AutoPostBack="false"
                                              Enabled="False"
                                              DataSourceControl="CurrentObject"
                                              PropertyIdentifier="Partner"
                                              CompletionSetCount="5"
                                              ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"
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
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_NoAutoPostBack"
                                              TextBoxStyle-AutoPostBack="false"
                                              ReadOnly="False"
                                              DataSourceControl="CurrentObject"
                                              PropertyIdentifier="Partner"
                                              CompletionSetCount="10"
                                              ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"
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
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_NoCommandNoMenu"
                                              TextBoxStyle-AutoPostBack="false"
                                              ReadOnly="False"
                                              DataSourceControl="CurrentObject"
                                              PropertyIdentifier="Partner"
                                              CompletionSetCount="5"
                                              ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"
                                              runat="server">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(no command & no menu)</td>
  </tr>
<tr>
  <td></td>
  <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_NoCommandNoMenu_ReadOnly"
                                              TextBoxStyle-AutoPostBack="false"
                                              ReadOnly="True"
                                              DataSourceControl="CurrentObject"
                                              PropertyIdentifier="Partner"
                                              CompletionSetCount="5"
                                              ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"

                                              runat="server">
      </remotion:BocAutoCompleteReferenceValue>
  </td>
    <td>(read-only, no command & no menu)</td>
</tr>
<tr>
  <td></td>
  <td>
    <remotion:BocAutoCompleteReferenceValue ID="PartnerField_Normal_Required"
                                            TextBoxStyle-AutoPostBack="true"
                                            ReadOnly="False"
                                            DataSourceControl="CurrentObject"
                                            PropertyIdentifier="Partner"
                                            CompletionSetCount="5"
                                            ControlServicePath="BocAutoCompleteReferenceValueWebService.asmx"
                                            
                                            Required="true"
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
    </remotion:BocAutoCompleteReferenceValue>
  </td>
    <td>(normal, required)</td>
</tr>
</table>
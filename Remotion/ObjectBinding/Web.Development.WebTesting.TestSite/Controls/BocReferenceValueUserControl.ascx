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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocReferenceValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocReferenceValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_Normal"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

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
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_Normal_AlternativeRendering"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="False"
                                  HasValueEmbeddedInsideOptionsMenu="True"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

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
    <td>(normal, alternative rendering)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_ReadOnly"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="True"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

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
      <remotion:BocReferenceValue ID="PartnerField_ReadOnly_AlternativeRendering"
                                  DropDownListStyle-AutoPostBack="true"
                                  ReadOnly="True"
                                  HasValueEmbeddedInsideOptionsMenu="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

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
    <td>(read-only, alternative rendering)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="PartnerField_Disabled"
                                  DropDownListStyle-AutoPostBack="true"
                                  Enabled="False"
                                  DataSourceControl="CurrentObject"
                                  PropertyIdentifier="Partner"
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

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
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

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
                                  IconServicePath="IconService.asmx"
                                  runat="server">
        
      </remotion:BocReferenceValue>
    </td>
    <td>(no command & no menu)</td>
  </tr>
</table>
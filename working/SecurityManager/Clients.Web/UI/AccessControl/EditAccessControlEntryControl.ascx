<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 % 
 % This program is free software; you can redistribute it and/or modify
 % it under the terms of the GNU Affero General Public License version 3.0 
 % as published by the Free Software Foundation.
 % 
 % This program is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Affero General Public License for more details.
 % 
 % You should have received a copy of the GNU Affero General Public License
 % along with this program; if not, see http://www.gnu.org/licenses.
 % 
 % Additional permissions are listed in the file re-motion_exceptions.txt.
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditAccessControlEntryControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditAccessControlEntryControl" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.AccessControlEntry, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ShowHelpProviders="False" />
<asp:ScriptManagerProxy runat="server" />

<remotion:WebUpdatePanel ID="AceUpdatePanel" runat="server" UpdateMode="Conditional" RenderMode="Tbody">
  <ContentTemplate>
    <tr class="<%= CssClass %>">
      <td class="buttonCell">
        <remotion:WebButton ID="ToggleAccessControlEntryButton" runat="server" CssClass="imageButton" OnClick="ToggleAccessControlEntryButton_Click" CausesValidation="false"/>
      </td>
      <td>
        <remotion:WebButton ID="DeleteAccessControlEntryButton" runat="server" CssClass="imageButton" OnClick="DeleteAccessControlEntryButton_Click" CausesValidation="false"
          RequiresSynchronousPostBack="True" />
      </td>
      <td class="conditionCell">
        <asp:PlaceHolder ID="CollapsedTenantInformation" runat="server" />
      </td>
      <td class="conditionCell">
        <asp:PlaceHolder ID="CollapsedGroupInformation" runat="server" />
      </td>
      <td class="conditionCell">
        <asp:PlaceHolder ID="CollapsedUserInformation" runat="server" />
      </td>
      <td class="conditionCell">
        <asp:PlaceHolder ID="CollapsedAbstractRoleInformation" runat="server" />
      </td>
      <td class="buttonCell">
        <remotion:DropDownMenu ID="AllPermisionsMenu" runat="server" />
      </td>
      <asp:PlaceHolder ID="PermissionsPlaceHolder" runat="server" />
    </tr>

    <asp:PlaceHolder ID="DetailsView" runat="server">
      <tr class="<%= CssClass %>">
        <td id="DetailsCell" runat="server">
          <asp:UpdatePanel ID="DetailsViewUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
              <table id="FormGrid" runat="server" class="accessControlEntryExpanded">
                <tr>
                  <td colspan="2"></td>
                </tr>
                <tr>
                  <td><remotion:SmartLabel ID="TenantLabel" runat="server" ForControl="TenantConditionField"/></td>
                  <td>
                    <remotion:BocEnumValue ID="TenantConditionField" runat="server" PropertyIdentifier="TenantCondition" DataSourceControl="CurrentObject" OnSelectionChanged="TenantConditionField_SelectionChanged" Width="32%" >
                      <ListControlStyle AutoPostBack="True"/>
                    </remotion:BocEnumValue>
                    <remotion:BocAutoCompleteReferenceValue ID="SpecificTenantField" runat="server" PropertyIdentifier="SpecificTenant" DataSourceControl="CurrentObject" Required="True" OnSelectionChanged="SpecificTenantField_SelectionChanged" Width="32%">
                      <TextBoxStyle AutoPostBack="true" />
                    </remotion:BocAutoCompleteReferenceValue>
                    <remotion:BocEnumValue ID="TenantHierarchyConditionField" runat="server" PropertyIdentifier="TenantHierarchyCondition" DataSourceControl="CurrentObject" Required="true" Width="32%"/>
                  </td>
                </tr>
                <tr>
                  <td><remotion:SmartLabel ID="GroupConditionLabel" runat="server" ForControl="GroupConditionField"/></td>
                  <td>
                    <remotion:BocEnumValue ID="GroupConditionField" runat="server" PropertyIdentifier="GroupCondition" DataSourceControl="CurrentObject"  OnSelectionChanged="GroupConditionField_SelectionChanged" Width="32%">
                      <ListControlStyle AutoPostBack="True"/>
                    </remotion:BocEnumValue>
                    <remotion:BocAutoCompleteReferenceValue ID="SpecificGroupField" runat="server" PropertyIdentifier="SpecificGroup" DataSourceControl="CurrentObject" Required="true" Width="32%"/>
                    <remotion:BocEnumValue ID="GroupHierarchyConditionField" runat="server" PropertyIdentifier="GroupHierarchyCondition" DataSourceControl="CurrentObject" Required="true" Width="32%"/>
                    <remotion:BocAutoCompleteReferenceValue ID="SpecificGroupTypeField" runat="server" PropertyIdentifier="SpecificGroupType" DataSourceControl="CurrentObject" Required="true" Width="32%"/>
                  </td>      
                </tr>
                <tr>
                  <td><remotion:SmartLabel ID="UserConditionLabel" runat="server" ForControl="UserConditionField"/></td>
                  <td>
                    <remotion:BocEnumValue ID="UserConditionField" runat="server" PropertyIdentifier="UserCondition" DataSourceControl="CurrentObject"  OnSelectionChanged="UserConditionField_SelectionChanged" Width="32%">
                      <ListControlStyle AutoPostBack="True"/>
                    </remotion:BocEnumValue>
                    <remotion:BocAutoCompleteReferenceValue ID="SpecificUserField" runat="server" PropertyIdentifier="SpecificUser" DataSourceControl="CurrentObject" Required="true" Width="32%"/>
                    <remotion:BocAutoCompleteReferenceValue ID="SpecificPositionField" runat="server" PropertyIdentifier="SpecificPosition" DataSourceControl="CurrentObject" Required="true" Width="32%"/>
                  </td>
                </tr>
                <tr>
                  <td><remotion:SmartLabel ID="SpecificAbstractRoleLabel" runat="server" ForControl="SpecificAbstractRoleField"/></td>
                  <td>
                    <remotion:BocAutoCompleteReferenceValue ID="SpecificAbstractRoleField" runat="server" PropertyIdentifier="SpecificAbstractRole" DataSourceControl="CurrentObject" Width="32%">
                      <TextBoxStyle AutoPostBack="True" />
                    </remotion:BocAutoCompleteReferenceValue>
                  </td>
                </tr>
            </table>
            </ContentTemplate>
          </asp:UpdatePanel>
        </td>
      </tr>
    </asp:PlaceHolder>
  </ContentTemplate>
</remotion:WebUpdatePanel>

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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocListUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocListUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControl ID="EmptyObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Normal"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        AlwaysShowPageInfo="True"
        AvailableViewsListTitle="V_iews:"
        EnableEditModeValidator="true"
        EnableMultipleSorting="true"
        EnableSorting="true"
        Index="SortedOrder"
        IndexColumnTitle="I_ndex"
        IndexOffset="10"
        ListMenuLineBreaks="BetweenGroups"
        OptionsTitle="O_ptions:"
        PageSize="2"
        RowMenuDisplay="Manual"
        Selection="Multiple"
        ShowAllProperties="True"
        ShowAvailableViewsList="True"
        ShowEditModeRequiredMarkers="true"
        ShowEditModeValidationMarkers="true"
        ShowListMenu="true"
        ShowOptionsMenu="true"
        ShowSortingOrder="True"
        
        Width="100%"
        Height="10em"
        runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="Option command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="Option command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="Option command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="ListMenuCmd1" Text="LM cmd">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd2" Icon-Url="~/Images/SampleIcon.gif" Icon-AlternateText="SampleIcon" Icon-ToolTip="SampleIcon">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd3" Text="LM cmd 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" ColumnTitle="Command">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocCommandColumnDefinition>
          <remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="16px" ColumnTitle="Menu"/>
          <remotion:BocAllPropertiesPlaceholderColumnDefinition/>
          <remotion:BocSimpleColumnDefinition ColumnTitle="TitleWithCmd" PropertyPathIdentifier="Title">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_ReadOnly"
        ReadOnly="True"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        AlwaysShowPageInfo="True"
        AvailableViewsListTitle="V_iews:"
        EnableEditModeValidator="true"
        EnableMultipleSorting="true"
        EnableSorting="true"
        Index="SortedOrder"
        IndexColumnTitle="I_ndex"
        IndexOffset="10"
        ListMenuLineBreaks="BetweenGroups"
        OptionsTitle="O_ptions:"
        PageSize="2"
        RowMenuDisplay="Manual"
        Selection="Multiple"
        ShowAllProperties="True"
        ShowAvailableViewsList="True"
        ShowEditModeRequiredMarkers="true"
        ShowEditModeValidationMarkers="true"
        ShowListMenu="true"
        ShowOptionsMenu="true"
        ShowSortingOrder="True"
        
        Width="100%"
        Height="10em"
        runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="Option command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="Option command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="Option command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="ListMenuCmd1" Text="LM cmd">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd2" Icon-Url="~/Images/SampleIcon.gif" Icon-AlternateText="SampleIcon" Icon-ToolTip="SampleIcon">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd3" Text="LM cmd 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" ColumnTitle="Command">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocCommandColumnDefinition>
          <remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="16px" ColumnTitle="Menu"/>
          <remotion:BocAllPropertiesPlaceholderColumnDefinition/>
          <remotion:BocSimpleColumnDefinition ColumnTitle="TitleWithCmd" PropertyPathIdentifier="Title">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>  
    </td>
    <td>&nbsp; (read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Special"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        PageSize="2"
        ShowAllProperties="False"
        
        Width="100%"
        Height="10em"
        runat="server">
        <FixedColumns>
          <remotion:BocCompoundColumnDefinition ItemID="DateRange" ColumnTitle="Date range" FormatString="{0} until {1}">
            <PropertyPathBindings>
              <remotion:PropertyPathBinding PropertyPathIdentifier="StartDate"/>
              <remotion:PropertyPathBinding PropertyPathIdentifier="EndDate"/>
            </PropertyPathBindings>
          </remotion:BocCompoundColumnDefinition>
          <remotion:BocCustomColumnDefinition ItemID="CustomCell" ColumnTitle="Custom cell" CustomCellType="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite::Controls.TestBocListCustomCell" PropertyPathIdentifier="Title"/>
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (custom cells)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Empty"
        ReadOnly="False"
        DataSourceControl="EmptyObject"
        PropertyIdentifier="Jobs"
        
        EmptyListMessage="A wonderful empty list."
        ShowAllProperties="True"
        ShowEmptyListMessage="True"
        
        Width="100%"
        Height="10em"
        runat="server">
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (empty)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_NoFakeTableHeader"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        ShowAllProperties="True"
        runat="server">
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (no fake table header)</td>
  </tr>
</table>
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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocListUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls.BocListUserControl" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.Web.UI.Controls" assembly="Remotion.Web" %>
<%@ Register tagPrefix="remotion" namespace="Remotion.ObjectBinding.Web.UI.Controls" assembly="Remotion.ObjectBinding.Web" %>
<%@ Register tagPrefix="testsite" namespace="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls" assembly="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.ObjectBinding.Web.UI.Controls.Validation" Assembly="Remotion.ObjectBinding.Web" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControl ID="EmptyObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<remotion:BindableObjectDataSourceControlValidationResultDispatchingValidator ID="CurrentObjectValidationResultDispatchingValidator" ControlToValidate="CurrentObject" runat="server"/>
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
        
        Width="835px"
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
          <remotion:BocMenuItem ItemID="OptCmd3" Text="Option command 3" RequiredSelection="ExactlyOne">
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
          <remotion:BocMenuItem ItemID="ListMenuCmd2" Icon-Url="../Image/SampleIcon.gif" Icon-AlternateText="SampleIcon" Icon-ToolTip="SampleIcon" RequiredSelection="ExactlyOne">
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
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" Icon-Width="16px" Icon-Height="16px" Icon-Url="../Image/SampleIcon.gif" ColumnTitle="Command">
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
      <br/>
      <remotion:WebButton ID="DeleteSelectedRowTestCaseRowButton" Text="Delete Selected Row Test Case" OnClick="DeleteSelectedRowTestCaseRowButton_OnClick" runat="server"/>
    </td>
    <td>&nbsp; (normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_WithRadioButtons"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        Selection="SingleRadioButton"
        ShowAllProperties="True"
        
        Width="100%"
        Height="10em"
        runat="server">
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="ListMenuCmd1" Text="LM cmd">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (selection via radio buttons)</td>
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
          <remotion:BocMenuItem ItemID="ListMenuCmd2" Icon-Url="../Image/SampleIcon.gif" Icon-AlternateText="SampleIcon" Icon-ToolTip="SampleIcon">
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
          <remotion:BocCustomColumnDefinition ItemID="CustomCell" ColumnTitle="Custom cell" CustomCellType="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared::Controls.TestBocListCustomCell" PropertyPathIdentifier="Title"/>
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
        ID="ChildrenList_EmptyWithRowHeaders"
        ReadOnly="False"
        DataSourceControl="EmptyObject"
        PropertyIdentifier="Children"

        EmptyListMessage="An empty list can still identify its row headers to the web testing framework."
        ShowEmptyListMessage="True"

        Width="100%"
        Height="10em"
        runat="server">
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" ColumnTitle="Command">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocCommandColumnDefinition>
          <remotion:BocSimpleColumnDefinition ColumnTitle="LastName" PropertyPathIdentifier="LastName" IsRowHeader="True" />
          <remotion:BocSimpleColumnDefinition ColumnTitle="FirstName" PropertyPathIdentifier="FirstName" IsRowHeader="True" />
          <remotion:BocSimpleColumnDefinition ColumnTitle="Partner" PropertyPathIdentifier="Partner" />
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (empty with row headers)</td>
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
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" ColumnTitle="Edit" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (no fake table header)</td>
  </tr>
<tr>
    <td></td>
    <td>
        <testsite:TestBocListWithRowMenuItems
            ID="JobList_ColumnsWithoutDiagnosticMetadata"
            ReadOnly="False"
            DataSourceControl="CurrentObject"
            PropertyIdentifier="Jobs"
        
            ShowAllProperties="False"
        
            Width="100%"
            Height="10em"
            runat="server">
            <ListMenuItems>
                <remotion:BocMenuItem ItemID="ListMenuCmd1" Text="LM cmd">
                    <PersistedCommand>
                        <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
                    </PersistedCommand>
                </remotion:BocMenuItem>
            </ListMenuItems>
            <FixedColumns>
                <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="With'SingleQuote" SaveText="Save" CancelText="Cancel" Width="2em"  ColumnTitle="Edit"/>
                <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="With'SingleQuoteAndDouble&quot;Quote" ColumnTitle="Command">
                    <PersistedCommand>
                        <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
                    </PersistedCommand>
                </remotion:BocCommandColumnDefinition>
            </FixedColumns>
        </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (without Diagnostic Metadata, with characters requiring special handling in selector)</td>
</tr>
<tr>
  <td></td>
  <td>
    <testsite:TestBocListWithRowMenuItems
      ID="NestedPropertyPathIdentifier"
      DataSourceControl="CurrentObject"
      ShowAllProperties="False"
      PropertyIdentifier="Children"

      Width="100%"
      Height="13em"
      runat="server">
      <FixedColumns>
        <remotion:BocSimpleColumnDefinition ColumnTitle="Partner's FirstName" PropertyPathIdentifier="Partner.FirstName">
        </remotion:BocSimpleColumnDefinition>
      </FixedColumns>
    </testsite:TestBocListWithRowMenuItems>
  </td>
  <td>&nbsp; (Nested PropertyPathIdentifier)</td>
</tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Empty_WithoutPlaceholder"
        ReadOnly="False"
        DataSourceControl="EmptyObject"
        PropertyIdentifier="Jobs"

        ShowAllProperties="True"
        ShowEmptyListMessage="False"

        Width="100%"
        Height="10em"
        runat="server">
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (empty without placeholder)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_AlwaysInvalid"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        ShowAllProperties="True"
        ShowEmptyListMessage="true"
        Selection="Multiple"
        Width="100%"
        Height="10em"
        runat="server">
      </testsite:TestBocListWithRowMenuItems>
      <asp:CustomValidator ID="AlwaysInvalidValidator" ErrorMessage="Always Invalid" runat="server"/>
    </td>
    <td>&nbsp; (always invalid)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Empty_VariableColumns"
        ReadOnly="False"
        DataSourceControl="EmptyObject"
        PropertyIdentifier="Jobs"

        ShowAllProperties="False"
        ShowEmptyListMessage="False"

        Width="100%"
        Height="10em"
        runat="server">
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (empty with variable columns)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Empty_Umlauts"
        ReadOnly="False"
        DataSourceControl="EmptyObject"
        PropertyIdentifier="Jobs"
        Index="SortedOrder"
        IndexColumnTitle="(html)Indexübersicht"

        ShowAllProperties="True"
        ShowEmptyListMessage="True"

        Width="100%"
        Height="10em"
        runat="server">
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (empty with umlauts)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Validation"
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

        Width="835px"
        Height="10em"
        runat="server">
        <FixedColumns>
          <remotion:BocValidationErrorIndicatorColumnDefinition ColumnTitleStyle="Icon" />
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" Icon-Width="16px" Icon-Height="16px" Icon-Url="../Image/SampleIcon.gif" ColumnTitle="Command">
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
      <br/>
      <remotion:WebButton ID="ValidationTestCaseRowButton" Text="Validation scenario (row)" OnClick="ValidationTestCaseRow" runat="server"/>
      <remotion:WebButton ID="ValidationTestCaseCellButton" Text="Validation scenario (cell)" OnClick="ValidationTestCaseCell" runat="server"/>
      <asp:CheckBox ID="ValidationTestCaseStartDate" Text="Validation scenario (StartDate)" runat="server"/>
    </td>
    <td>&nbsp; (validation)</td>
  </tr>
</table>

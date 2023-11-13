// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.Services;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Remotion.Web.UI.Controls.PostBackTargets;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> 
  ///   This control can be used to display and edit a list of <see cref="IBusinessObject"/> instances.
  ///   The properties of the business objects are displayed in individual columns. 
  /// </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/Class/*' />
  // TODO: see "Doc\Bugs and ToDos.txt"
  [DefaultEvent("CommandClick")]
  [ToolboxItemFilter("System.Web.UI")]
  public partial class BocList :
      BusinessObjectBoundEditableWebControl,
      IBocList,
      IPostBackEventHandler,
      IPostBackDataHandler,
      IResourceDispatchTarget
  {
    #region Obsolete

    /// <summary> Gets or sets the offset between the items in the <c>menu block</c>. </summary>
    /// <remarks> The <see cref="MenuBlockOffset"/> is applied as a <c>margin</c> attribute. </remarks>
    [Obsolete("Style via CSS instead. (Version 3.0.0)", true)]
    public Unit MenuBlockItemOffset
    {
      get => throw new NotSupportedException("Style via CSS instead.");
      set => throw new NotSupportedException("Style via CSS instead.");
    }

    /// <summary> Gets or sets the offset between the table and the menu block. </summary>
    [Obsolete("Style via CSS instead. (Version 3.0.0)", true)]
    public Unit MenuBlockOffset
    {
      get => throw new NotSupportedException("Style via CSS instead.");
      set => throw new NotSupportedException("Style via CSS instead.");
    }

    /// <summary> Gets or sets the width reserved for the menu block. </summary>
    [Obsolete("Use " + nameof(MenuBlockMinWidth) + " and " + nameof(MenuBlockMaxWidth) + " instead. (Version 3.0.0)", true)]
    public Unit MenuBlockWidth
    {
      get => throw new NotSupportedException($"Use {nameof(MenuBlockMinWidth)} and {nameof(MenuBlockMaxWidth)} instead.");
      set => throw new NotSupportedException($"Use {nameof(MenuBlockMinWidth)} and {nameof(MenuBlockMaxWidth)} instead.");
    }

    [Obsolete("For DependDB only.", true)]
    private new BaseValidator[] CreateValidators ()
    {
      throw new NotImplementedException("For DependDB only.");
    }

    #endregion

    //  constants
    private const string c_currentPageControlName = "_Boc_CurrentPage";
    private const string c_availableViewsListIDSuffix = "_Boc_AvailableViewsList";
    private const string c_optionsMenuIDSuffix = "_Boc_OptionsMenu";
    private const string c_listMenuIDSuffix = "_Boc_ListMenu";
    private const string c_rowMenuIDPrefix = "_RowMenu_";

    /// <summary> Prefix applied to the post back argument of the event type column commands. </summary>
    private const string c_eventListItemCommandPrefix = "ListCommand=";

    /// <summary> Prefix applied to the post back argument of the custom columns. </summary>
    private const string c_customCellEventPrefix = "CustomCell=";

    private const string c_eventRowEditModePrefix = "RowEditMode=";
    private const string c_rowEditModeRequiredFieldIcon = "sprite.svg#RequiredField";
    private const string c_rowEditModeValidationErrorIcon = "sprite.svg#ValidationError";

    /// <summary> Prefix applied to the post back argument of the sort buttons. </summary>
    public const string SortCommandPrefix = "Sort=";

    /// <summary> The key identifying a fixed column resource entry. </summary>
    private const string c_resourceKeyFixedColumns = "FixedColumns";

    /// <summary> The key identifying a options menu item resource entry. </summary>
    private const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";

    /// <summary> The key identifying a list menu item resource entry. </summary>
    private const string c_resourceKeyListMenuItems = "ListMenuItems";

    private const string c_rowSelectorPostfix = "_RowSelector";
    private const string c_allRowsSelectorPostfix = "_AllRowsSelector";

    // types

    /// <summary> A list of control wide resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.TryGetString(string, out string)"/>.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocList")]
    public enum ResourceIdentifier
    {
      EmptyListMessage,
      OptionsTitle,
      ListMenuHeading,
      AvailableViewsListTitle,
      /// <summary>The tool tip text for the required icon.</summary>
      RequiredFieldTitle,
      /// <summary>The tool tip text for the validation icon.</summary>
      ValidationErrorInfoTitle,
      /// <summary> The alternate text for the sort ascending button. </summary>
      SortAscendingAlternateText,
      /// <summary> The alternate text for the sort descending button. </summary>
      SortDescendingAlternateText,
      RowEditModeErrorMessage,
      ListEditModeErrorMessage,
      RowEditModeEditAlternateText,
      RowEditModeSaveAlternateText,
      RowEditModeCancelAlternateText,
      SelectAllRowsLabelText,
      SelectRowLabelText,
      SelectionHeaderText,
      IndexColumnTitle,
      /// <summary> The menu title text used for an automatically generated row menu column. </summary>
      RowMenuTitle,
    }

    public enum RowEditModeCommand
    {
      Edit,
      Save,
      Cancel
    }

    private enum ValueMode
    {
      Interim,
      Complete
    }

    // static members
    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof(IBusinessObjectReferenceProperty) };

    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType!);

    private static readonly object s_menuItemClickEvent = new object();
    private static readonly object s_listItemCommandClickEvent = new object();
    private static readonly object s_customCellClickEvent = new object();

    private static readonly object s_sortingOrderChangingEvent = new object();
    private static readonly object s_sortingOrderChangedEvent = new object();

    private static readonly object s_dataRowRenderEvent = new object();

    private static readonly object s_editableRowChangesSavingEvent = new object();
    private static readonly object s_editableRowChangesSavedEvent = new object();
    private static readonly object s_editableRowChangesCancelingEvent = new object();
    private static readonly object s_editableRowChangesCanceledEvent = new object();


    // member fields

    private IRowIDProvider _rowIDProvider = new NullValueRowIDProvider();

    private readonly PlaceHolder _availableViewsListPlaceHolder;

    private WebString _availableViewsListTitle;

    /// <summary> The predefined column definition sets that the user can choose from at run-time. </summary>
    private readonly BocListViewCollection _availableViews;

    /// <summary> Determines whether to show the drop down list for selecting a view. </summary>
    private bool _showAvailableViewsList = true;

    /// <summary> The current <see cref="BocListView"/>. May be set at run time. </summary>
    private BocListView? _selectedView;

    /// <summary> 
    ///   The zero-based index of the <see cref="BocListView"/> selected from 
    ///   <see cref="AvailableViews"/>.
    /// </summary>
    private int? _selectedViewIndex;

    private bool _isSelectedViewIndexSet;

    /// <summary>
    /// The <see cref="IReadOnlyList{IBusinessObject}"/> displayed by the <see cref="BocList"/>. Can additionally implement <see cref="IList"/> for modification.
    /// </summary>
    private IReadOnlyList<IBusinessObject>? _value;

    /// <summary> The user independent column definitions. </summary>
    private readonly BocColumnDefinitionCollection _fixedColumns;

    /// <summary> 
    ///   Contains a <see cref="BocColumnDefinition"/> for each property of the bound 
    ///   <see cref="IBusinessObject"/>. 
    /// </summary>
    private BocColumnDefinition[]? _allPropertyColumns;

    /// <summary> Contains the <see cref="BocColumnDefinition"/> objects during the rendering phase. </summary>
    private BocColumnDefinition[]? _columnDefinitions;

    private bool _hasAppendedAllPropertyColumnDefinitions;


    /// <summary> Determines whether the options menu is shown. </summary>
    private bool _showOptionsMenu = true;

    /// <summary> Determines whether the list menu is shown. </summary>
    private bool _showListMenu = true;

    private RowMenuDisplay _rowMenuDisplay = RowMenuDisplay.Undefined;
    private WebString _optionsTitle;
    private WebString _listMenuHeading;
    private string[]? _hiddenMenuItems;
    private Unit _menuBlockOffset = Unit.Empty;
    private Unit _menuBlockItemOffset = Unit.Empty;
    private readonly DropDownMenu _optionsMenu;

    private readonly ListMenu _listMenu;

    /// <summary> Determines wheter an empty list will still render its headers and the additional column sets list. </summary>
    private bool _showEmptyListEditMode = true;

    private bool _showMenuForEmptyListEditMode = true;
    private bool _showEmptyListReadOnlyMode;
    private bool _showMenuForEmptyListReadOnlyMode;
    private WebString _emptyListMessage;
    private bool _showEmptyListMessage;

    /// <summary> Determines whether to generate columns for all properties. </summary>
    private bool _showAllProperties;

    /// <summary> Determines whether to show the icons for each entry in <see cref="Value"/>. </summary>
    private bool _enableIcon = true;

    /// <summary> Determines whether to show the sort buttons. </summary>
    private bool _enableSorting = true;

    /// <summary> Determines whether to show the sorting order after the sorting button. Undefined interpreted as True. </summary>
    private bool? _showSortingOrder;

    /// <summary> Undefined interpreted as True. </summary>
    private bool? _enableMultipleSorting;

    private BocListSortingOrderEntry[] _sortingOrder = new BocListSortingOrderEntry[0];

    private ReadOnlyCollection<BocListRow>? _indexedRowsSorted;
    private ReadOnlyCollection<SortedRow>? _currentPageRows;

    /// <summary> Determines whether to enable the selecting of the data rows. </summary>
    private RowSelection _selection = RowSelection.Undefined;

    /// <summary> Contains the checked state for each of the selector controls in the <see cref="BocList"/>. </summary>
    private HashSet<string> _selectorControlCheckedState = new HashSet<string>();

    private RowIndex _index = RowIndex.Undefined;
    private WebString _indexColumnTitle;
    private int? _indexOffset;

    /// <summary> Null, 0: show all objects, > 0: show n objects per page. </summary>
    private int? _pageSize;

    /// <summary>
    ///   Show page info ("page 1 of n") and links always (true),
    ///   or only if there is more than 1 page (false)
    /// </summary>
    private bool _alwaysShowPageInfo;

    /// <summary> The index of the current page. </summary>
    private int _currentPageIndex;

    /// <summary> The total number of pages required for paging through the entire list. </summary>
    private int _pageCount;
    private int? _newPageIndex;
    private int? _editedRowIndex;

    /// <summary> Determines whether the client script is enabled. </summary>
    private bool _enableClientScript = true;

    private readonly IEditModeController _editModeController;
    private EditableRowDataSourceFactory _editModeDataSourceFactory = new EditableRowDataSourceFactory();
    private EditableRowControlFactory _editModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();
    private bool _enableEditModeValidator = true;
    private bool _enableAutoFocusOnSwitchToEditMode = true;
    private bool _showEditModeRequiredMarkers = true;
    private bool _showEditModeValidationMarkers;
    private bool _disableEditModeValidationMessages;

    private PlainTextString _errorMessage;
    private bool? _isBrowserCapableOfSCripting;
    private ScalarLoadPostDataTarget? _currentPagePostBackTarget;

    private readonly IRenderingFeatures _renderingFeatures;
    private ReadOnlyCollection<BaseValidator>? _validators;

    private string? _controlServicePath;
    private string? _controlServiceArguments;

    private bool _hasPreRenderCompleted;

    // construction and disposing

    protected IWebServiceFactory WebServiceFactory { get; }

    public BocList ()
        : this(SafeServiceLocator.Current.GetInstance<IWebServiceFactory>())
    {
    }

    protected BocList ([JetBrains.Annotations.NotNull] IWebServiceFactory webServiceFactory)
    {
      ArgumentUtility.CheckNotNull("webServiceFactory", webServiceFactory);

      _availableViewsListPlaceHolder = new PlaceHolder();
      _editModeController = new EditModeController(new EditModeHost(this));
      _optionsMenu = new DropDownMenu(this);
      _listMenu = new ListMenu(this);
      _availableViews = new BocListViewCollection(this);
      _fixedColumns = new BocColumnDefinitionCollection(this);
      _fixedColumns.CollectionChanged += delegate { OnColumnsChanged(); };

      _renderingFeatures = ServiceLocator.GetInstance<IRenderingFeatures>();
      WebServiceFactory = webServiceFactory;
    }

    // methods and properties

    protected override void CreateChildControls ()
    {
      _optionsMenu.ID = ID + c_optionsMenuIDSuffix;
      _optionsMenu.EventCommandClick += MenuItemEventCommandClick;
      _optionsMenu.WxeFunctionCommandClick += MenuItemWxeFunctionCommandClick;
      Controls.Add(_optionsMenu);

      _listMenu.ID = ID + c_listMenuIDSuffix;
      _listMenu.EventCommandClick += MenuItemEventCommandClick;
      _listMenu.WxeFunctionCommandClick += MenuItemWxeFunctionCommandClick;
      Controls.Add(_listMenu);

      Controls.Add(_availableViewsListPlaceHolder);

      var availableViewsListPostBackTarget = new ScalarLoadPostDataTarget();
      availableViewsListPostBackTarget.ID = ID + c_availableViewsListIDSuffix;
      availableViewsListPostBackTarget.DataChanged += HandleSelectedViewChanged;
      _availableViewsListPlaceHolder.Controls.Add(availableViewsListPostBackTarget);

      _currentPagePostBackTarget = new ScalarLoadPostDataTarget();
      _currentPagePostBackTarget.ID = ID + c_currentPageControlName;
      _currentPagePostBackTarget.ClientIDMode = ClientIDMode.AutoID;
      _currentPagePostBackTarget.DataChanged += HandleCurrentPageChanged;
      Controls.Add(_currentPagePostBackTarget);

      _editModeController.ID = ID + "_EditModeController";
      Controls.Add((Control)_editModeController);

      CreateChildControlsForRowMenus();
      CreateChildControlsForCustomColumns();
    }

    /// <summary> Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls. </summary>
    /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      _availableViews.CollectionChanged += AvailableViews_CollectionChanged;
      Binding.BindingChanged += Binding_BindingChanged;

      Page!.RegisterRequiresPostBack(this);
      InitializeMenusItems();
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents(htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender, EditModeControlFactory);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (ControlExistedInPreviousRequest)
      {
        var columns = EnsureColumnsGot();
        EnsureEditModeRestored();
        EnsureRowMenusInitialized();
        EnsureCustomColumnsInitialized(columns);
      }
    }

    /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
    /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      RaisePostBackEvent(eventArgument);
    }

    /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
    protected virtual void RaisePostBackEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);

      eventArgument = eventArgument.Trim();
      if (eventArgument.StartsWith(c_eventListItemCommandPrefix))
        HandleListItemCommandEvent(eventArgument.Substring(c_eventListItemCommandPrefix.Length));
      else if (eventArgument.StartsWith(SortCommandPrefix))
        HandleResorting(eventArgument.Substring(SortCommandPrefix.Length));
      else if (eventArgument.StartsWith(c_customCellEventPrefix))
        HandleCustomCellEvent(eventArgument.Substring(c_customCellEventPrefix.Length));
      else if (eventArgument.StartsWith(c_eventRowEditModePrefix))
        HandleRowEditModeEvent(eventArgument.Substring(c_eventRowEditModePrefix.Length));
      else
        throw new ArgumentException("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
    }

    /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (IsLoadPostDataRequired())
        return LoadPostData(postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary>
    ///   Returns always <see langword="true"/>. 
    ///   Used to raise the post data changed event for getting the selected column definition set.
    /// </summary>
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (_editModeController.IsRowEditModeActive)
        return false;

      LoadSelectionPostData(postCollection);

      return false;
    }

    private void HandleSelectedViewChanged (object? sender, EventArgs e)
    {
      ArgumentUtility.CheckNotNull("sender", sender!);

      if (!IsLoadPostDataRequired())
        return;

      var value = ((ScalarLoadPostDataTarget)sender).Value;
      Assertion.IsNotNull(value, "sender.Value != null");
      SelectedViewIndex = int.Parse(value);
    }

    private void HandleCurrentPageChanged (object? sender, EventArgs e)
    {
      ArgumentUtility.CheckNotNull("sender", sender!);

      if (!IsLoadPostDataRequired())
        return;

      if (!IsPagingEnabled)
        return;

      var value = ((ScalarLoadPostDataTarget)sender).Value;
      Assertion.IsNotNull(value, "sender.Value != null");
      _newPageIndex = int.Parse(value);
    }

    private void LoadSelectionPostData (NameValueCollection postCollection)
    {
      _selectorControlCheckedState.Clear();

      string dataRowSelectorControlFilter = ((IBocList)this).GetSelectorControlName();
      var values = postCollection.GetValues(dataRowSelectorControlFilter);
      if (values == null)
        return;

      foreach (string rowID in values)
      {
        if ((_selection == RowSelection.SingleCheckBox || _selection == RowSelection.SingleRadioButton) && (_selectorControlCheckedState.Count == 1))
          break;

        _selectorControlCheckedState.Add(rowID);
      }
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
    }

    /// <summary> Handles post back events raised by a list item event. </summary>
    /// <param name="eventArgument"> &lt;column-index&gt;,&lt;row-ID&gt; </param>
    private void HandleListItemCommandEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);

      if (Value == null)
      {
        throw new InvalidOperationException(
            string.Format("The BocList '{0}' does not have a Value when attempting to handle the list item click event.", ID));
      }

      string[] eventArgumentParts = eventArgument.Split(new[] { ',' }, 2);

      //  First part: column index
      int columnIndex;
      eventArgumentParts[0] = eventArgumentParts[0].Trim();
      try
      {
        columnIndex = int.Parse(eventArgumentParts[0]);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(
            "First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>'.", ex);
      }

      BocColumnDefinition[] columns = EnsureColumnsGot();
      if (columnIndex >= columns.Length)
      {
        throw new ArgumentOutOfRangeException(
            "Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'",
            (Exception?)null);
      }

      BocCommandEnabledColumnDefinition column = (BocCommandEnabledColumnDefinition)columns[columnIndex];
      if (column.Command == null)
      {
        throw new ArgumentOutOfRangeException(
            string.Format("The BocList '{0}' does not have a command inside column {1}.", ID, columnIndex));
      }
      BocListItemCommand command = column.Command;

      //  Second part: list index
      BocListRow? row;
      try
      {
        row = RowIDProvider.GetRowFromItemRowID(Value, eventArgumentParts[1].Trim());
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(
            "Second part of argument 'eventArgument' does not match the expected format. Expected format: <column-index>,<row-ID>'.", ex);
      }

      if (row == null)
        return;

      switch (command.Type)
      {
        case CommandType.Event:
        {
          OnListItemCommandClick(column, row.Index, row.BusinessObject);
          break;
        }
        case CommandType.WxeFunction:
        {
          if (Page is IWxePage)
            command.ExecuteWxeFunction((IWxePage)Page, row.Index, row.BusinessObject);
          //else
          //  command.ExecuteWxeFunction (Page, row.Index, row.BusinessObject);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Handles post back events raised by a custom cell event. </summary>
    /// <param name="eventArgument"> &lt;column-index&gt;,&lt;row-ID&gt;[,&lt;customArgument&gt;] </param>
    private void HandleCustomCellEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);

      if (Value == null)
      {
        throw new InvalidOperationException(
            string.Format("The BocList '{0}' does not have a Value when attempting to handle the custom cell event.", ID));
      }

      string[] eventArgumentParts = eventArgument.Split(new[] { ',' }, 3);

      //  First part: column index
      int columnIndex;
      eventArgumentParts[0] = eventArgumentParts[0].Trim();
      try
      {
        columnIndex = int.Parse(eventArgumentParts[0]);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(
            "First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>[,<customArgument>]'.", ex);
      }

      BocColumnDefinition[] columns = EnsureColumnsGot();
      if (columnIndex >= columns.Length)
      {
        throw new ArgumentOutOfRangeException(
            "Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'",
            (Exception?)null);
      }

      //  Second part: list index
      BocListRow? row;
      try
      {
        row = RowIDProvider.GetRowFromItemRowID(Value, eventArgumentParts[1].Trim());
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(
            "Second part of argument 'eventArgument' does not match the expected format. Expected format: <column-index>,<row-ID>[,<customArgument>]'.",
            ex);
      }

      //  Thrid part, optional: customCellArgument
      string? customCellArgument = null;
      if (eventArgumentParts.Length == 3)
      {
        eventArgumentParts[2] = eventArgumentParts[2].Trim();
        customCellArgument = eventArgumentParts[2];
      }

      if (row == null)
        return;

      BocCustomColumnDefinition column = (BocCustomColumnDefinition)columns[columnIndex];
      OnCustomCellClick(column, row.BusinessObject, customCellArgument);
    }

    /// <summary> Handles post back events raised by an row edit mode event. </summary>
    /// <param name="eventArgument"> &lt;row-ID&gt;,&lt;command&gt; </param>
    private void HandleRowEditModeEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);

      if (Value == null)
      {
        throw new InvalidOperationException(
            string.Format(
                "The BocList '{0}' does not have a Value when attempting to handle the list item click event.", ID));
      }

      string[] eventArgumentParts = eventArgument.Split(new char[] { ',' }, 2);

      //  First part: list index
      BocListRow? row;
      try
      {
        row = RowIDProvider.GetRowFromItemRowID(Value, eventArgumentParts[0].Trim());
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(
            "First part of argument 'eventArgument' does not match the expected format. Expected format: <row-ID>,<command>'.",
            ex);
      }

      //  Second part: command
      RowEditModeCommand command;
      eventArgumentParts[1] = eventArgumentParts[1].Trim();
      try
      {
        command = (RowEditModeCommand)Enum.Parse(typeof(RowEditModeCommand), eventArgumentParts[1]);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(
            "Second part of argument 'eventArgument' must be an integer. Expected format: <list-index>,<command>'.", ex);
      }

      if (row == null)
        return;

      switch (command)
      {
        case RowEditModeCommand.Edit:
        {
          SwitchRowIntoEditMode(row.Index);
          break;
        }
        case RowEditModeCommand.Save:
        {
          EndRowEditMode(true);
          break;
        }
        case RowEditModeCommand.Cancel:
        {
          EndRowEditMode(false);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Fires the <see cref="ListItemCommandClick"/> event. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnListItemCommandClick/*' />
    protected virtual void OnListItemCommandClick (
        BocCommandEnabledColumnDefinition column,
        int listIndex,
        IBusinessObject businessObject)
    {
      if (column != null && column.Command != null)
      {
        column.Command.OnClick(column, listIndex, businessObject);
        BocListItemCommandClickEventHandler? commandClickHandler =
            (BocListItemCommandClickEventHandler?)Events[s_listItemCommandClickEvent];
        if (commandClickHandler != null)
        {
          BocListItemCommandClickEventArgs e =
              new BocListItemCommandClickEventArgs(column.Command, column, listIndex, businessObject);
          commandClickHandler(this, e);
        }
      }
    }

    protected virtual void OnCustomCellClick (
        BocCustomColumnDefinition column,
        IBusinessObject businessObject,
        string? argument)
    {
      BocCustomCellClickArguments args = new BocCustomCellClickArguments(this, businessObject, column);
      column.CustomCell.Click(args, argument);
      BocCustomCellClickEventHandler? clickHandler =
          (BocCustomCellClickEventHandler?)Events[s_customCellClickEvent];
      if (clickHandler != null)
      {
        BocCustomCellClickEventArgs e = new BocCustomCellClickEventArgs(column, businessObject, argument);
        clickHandler(this, e);
      }
    }

    /// <summary> Handles post back events raised by a sorting button. </summary>
    /// <param name="eventArgument"> &lt;column-index&gt; </param>
    private void HandleResorting (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);

      int columnIndex;
      try
      {
        if (eventArgument.Length == 0)
          throw new FormatException();
        columnIndex = int.Parse(eventArgument);
      }
      catch (FormatException)
      {
        throw new ArgumentException("Argument 'eventArgument' must be an integer.");
      }

      // Get columns from current life cycle. Once a sorting event was fired, no one will change the columns in this page life cycle.
      BocColumnDefinition[] columns = EnsureColumnsGot();

      if (columnIndex >= columns.Length)
      {
        throw new ArgumentOutOfRangeException(
            "eventArgument",
            eventArgument,
            "Column index was out of the range of valid values. Index must be less than the number of displayed columns.'");
      }
      var column = columns[columnIndex];
      if (!(column is IBocSortableColumnDefinition && ((IBocSortableColumnDefinition)column).IsSortable))
        throw new ArgumentOutOfRangeException("The BocList '" + ID + "' does not sortable column at index" + columnIndex + ".");

      var oldSortingOrder = GetSortingOrder();
      var workingSortingOrder = new List<BocListSortingOrderEntry>(oldSortingOrder);

      var oldSortingOrderEntry = workingSortingOrder.FirstOrDefault(entry => entry.Column == column) ?? BocListSortingOrderEntry.Empty;

      BocListSortingOrderEntry newSortingOrderEntry;
      //  Cycle: Ascending -> Descending -> None -> Ascending
      if (! oldSortingOrderEntry.IsEmpty)
      {
        workingSortingOrder.Remove(oldSortingOrderEntry);
        switch (oldSortingOrderEntry.Direction)
        {
          case SortingDirection.Ascending:
          {
            newSortingOrderEntry = new BocListSortingOrderEntry(oldSortingOrderEntry.Column!, SortingDirection.Descending);
            break;
          }
          case SortingDirection.Descending:
          {
            newSortingOrderEntry = BocListSortingOrderEntry.Empty;
            break;
          }
          case SortingDirection.None:
          {
            newSortingOrderEntry = new BocListSortingOrderEntry(oldSortingOrderEntry.Column!, SortingDirection.Ascending);
            break;
          }
          default:
          {
            throw new InvalidOperationException(string.Format("SortingDirection '{0}' is not valid.", oldSortingOrderEntry.Direction));
          }
        }
      }
      else
      {
        newSortingOrderEntry = new BocListSortingOrderEntry((IBocSortableColumnDefinition)column, SortingDirection.Ascending);
      }

      if (newSortingOrderEntry.IsEmpty)
      {
        if (workingSortingOrder.Count > 1 && ! IsMultipleSortingEnabled)
        {
          var entry = workingSortingOrder[0];
          workingSortingOrder.Clear();
          workingSortingOrder.Add(entry);
        }
      }
      else
      {
        if (! IsMultipleSortingEnabled)
          workingSortingOrder.Clear();
        workingSortingOrder.Add(newSortingOrderEntry);
      }

      var newSortingOrder = workingSortingOrder.ToArray();

      OnSortingOrderChanging(oldSortingOrder, newSortingOrder);
      _sortingOrder = workingSortingOrder.ToArray();
      OnSortingOrderChanged(oldSortingOrder, newSortingOrder);
      OnSortedRowsChanged();
    }

    protected virtual void OnSortingOrderChanging (
        BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
    {
      BocListSortingOrderChangeEventHandler? handler =
          (BocListSortingOrderChangeEventHandler?)Events[s_sortingOrderChangingEvent];
      if (handler != null)
      {
        BocListSortingOrderChangeEventArgs e =
            new BocListSortingOrderChangeEventArgs(oldSortingOrder, newSortingOrder);
        handler(this, e);
      }
    }

    protected virtual void OnSortingOrderChanged (
        BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
    {
      BocListSortingOrderChangeEventHandler? handler =
          (BocListSortingOrderChangeEventHandler?)Events[s_sortingOrderChangedEvent];
      if (handler != null)
      {
        BocListSortingOrderChangeEventArgs e =
            new BocListSortingOrderChangeEventArgs(oldSortingOrder, newSortingOrder);
        handler(this, e);
      }
    }

    /// <summary> Is raised when the sorting order of the <see cref="BocList"/> is about to change. </summary>
    /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
    [Category("Action")]
    [Description("Occurs when the sorting order of the BocList is about to change.")]
    public event BocListSortingOrderChangeEventHandler SortingOrderChanging
    {
      add { Events.AddHandler(s_sortingOrderChangingEvent, value); }
      remove { Events.RemoveHandler(s_sortingOrderChangingEvent, value); }
    }

    /// <summary> Is raised when the sorting order of the <see cref="BocList"/> has changed. </summary>
    /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
    [Category("Action")]
    [Description("Occurs when the sorting order of the BocList has to changed.")]
    public event BocListSortingOrderChangeEventHandler SortingOrderChanged
    {
      add { Events.AddHandler(s_sortingOrderChangedEvent, value); }
      remove { Events.RemoveHandler(s_sortingOrderChangedEvent, value); }
    }



    /// <summary>
    ///   Generates a <see cref="EditModeValidator"/>.
    /// </summary>
    /// <param name="isReadOnly">
    /// This flag is initialized with the value of <see cref="BusinessObjectBoundEditableWebControl.IsReadOnly"/>. 
    /// Implemantations should consider whether they require a validator also when the control is rendered as read-only.
    /// </param>
    /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.CreateValidators()">BusinessObjectBoundEditableWebControl.CreateValidators()</seealso>
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBocListValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();

      if (!ErrorMessage.IsEmpty)
        UpdateValidatorErrorMessages<EditModeValidator>(ErrorMessage);

      return _validators;
    }

    private void UpdateValidatorErrorMessages<T> (PlainTextString errorMessage) where T : BaseValidator
    {
      var validator = _validators.GetValidator<T>();
      if (validator != null)
        validator.ErrorMessage = errorMessage.GetValue();
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity (BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("columns", columns);

      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (ShowOptionsMenu)
          WcagHelper.Instance.HandleError(1, this, "ShowOptionsMenu");
        if (ShowListMenu)
          WcagHelper.Instance.HandleError(1, this, "ShowListMenu");
        if (ShowAvailableViewsList)
          WcagHelper.Instance.HandleError(1, this, "ShowAvailableViewsList");
        bool isPagingEnabled = _pageSize != null && _pageSize.Value != 0;
        if (isPagingEnabled)
          WcagHelper.Instance.HandleError(1, this, "PageSize");
        if (EnableSorting)
          WcagHelper.Instance.HandleWarning(1, this, "EnableSorting");
        if (RowMenuDisplay == RowMenuDisplay.Automatic)
          WcagHelper.Instance.HandleError(1, this, "RowMenuDisplay");

        for (int i = 0; i < columns.Length; i++)
        {
          if (columns[i] is BocRowEditModeColumnDefinition)
            WcagHelper.Instance.HandleError(1, this, string.Format("Columns[{0}]", i));

          BocCommandEnabledColumnDefinition? commandColumn = columns[i] as BocCommandEnabledColumnDefinition;
          if (commandColumn != null)
          {
            bool hasPostBackColumnCommand = commandColumn.Command != null
                                            && (commandColumn.Command.Type == CommandType.Event
                                                || commandColumn.Command.Type == CommandType.WxeFunction);
            if (hasPostBackColumnCommand)
              WcagHelper.Instance.HandleError(1, this, string.Format("Columns[{0}].Command", i));
          }

          if (columns[i] is BocDropDownMenuColumnDefinition)
            WcagHelper.Instance.HandleError(1, this, string.Format("Columns[{0}]", i));
        }
      }
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelDoubleARequired())
      {
        if (IsSelectionEnabled && ! IsIndexEnabled)
          WcagHelper.Instance.HandleError(2, this, "Selection");
      }
    }

    public override void PrepareValidation ()
    {
      base.PrepareValidation();

      _editModeController.PrepareValidation();
    }

    protected override void OnPreRender (EventArgs e)
    {
      _optionsMenu.Enabled = !_editModeController.IsRowEditModeActive;
      _listMenu.Enabled = !_editModeController.IsRowEditModeActive;

      BocColumnDefinition[] columns = EnsureColumnsGot();
      EnsureChildControls();

      base.OnPreRender(e);

      // Must be executed before CalculateCurrentPage
      if (_editModeController.IsRowEditModeActive)
        _editedRowIndex = _editModeController.GetEditedRow().Index;

      if (!IsPagingEnabled)
        _editedRowIndex = null;

      if (_editedRowIndex.HasValue)
      {
        var currentRow = EnsureBocListRowsForCurrentPageGot()
                             .FirstOrDefault(r => r.ValueRow.Index == _editedRowIndex.Value)
                         ??
                         EnsureSortedBocListRowsGot()
                             .Select((row, index) => new SortedRow(row, index))
                             .FirstOrDefault(r => r.ValueRow.Index == _editedRowIndex.Value);

        if (currentRow == null)
          _newPageIndex = null;
        else
          _newPageIndex = currentRow.SortedIndex / _pageSize!.Value;
      }

      CalculateCurrentPage(_newPageIndex);

      EnsureEditModeValidatorsRestored();

      LoadResources(GetResourceManager(), GlobalizationService);

      PreRenderMenuItems();
      PreRenderListItemCommands();

      EnsureRowMenusInitialized();
      PreRenderRowMenusItems();

      EnsureCustomColumnsInitialized(columns);
      PreRenderCustomColumns();

      _optionsMenu.GetSelectionCount = GetSelectionCountScript();

      CheckControlService();

      SetPreRenderComplete();
    }

    private void CheckControlService ()
    {
      if (string.IsNullOrEmpty(ControlServicePath))
        return;

      var virtualServicePath = VirtualPathUtility.GetVirtualPath(this, ControlServicePath);
      WebServiceFactory.CreateJsonService<IBocListWebService>(virtualServicePath);
    }

    /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    /// <summary>
    /// Sets the page index for <see cref="BocList"/> during the next render phase.
    /// </summary>
    /// <remarks>Note: The page index will be ignored if row edit mode is active since the <see cref="BocList"/> will always page to the edited row.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if paging is not enabled.</exception>
    protected void SetPageIndex (int pageIndex)
    {
      if (pageIndex < 0)
        throw new ArgumentOutOfRangeException("pageIndex", "The page index must not be less then zero.");

      if (!IsPagingEnabled)
        throw new InvalidOperationException(string.Format("The page index cannot be set on BoocList '{0}' unless paging is enabled.", ID));

      _newPageIndex = pageIndex;
    }

    private void CalculateCurrentPage (int? newPageIndex)
    {
      var oldPageIndex = _currentPageIndex;

      if (!IsPagingEnabled || Value == null)
      {
        _pageCount = 1;
        _currentPageIndex = 0;
      }
      else
      {
        Assertion.IsFalse(_editModeController.IsListEditModeActive, "ListEditMode cannot be enabled when paging is enabled and vice versa.");

        if (newPageIndex.HasValue)
          _currentPageIndex = newPageIndex.Value;

        _pageCount = (int)Math.Ceiling((double)Value.Count / _pageSize.Value);
        if (_currentPageIndex >= _pageCount)
          _currentPageIndex = _pageCount - 1;

        if (_currentPageIndex < 0)
          _currentPageIndex = 0;
      }

      if (_currentPageIndex != oldPageIndex)
        OnDisplayedRowsChanged();
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return new BocListConstraintVisitor(this);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      if (Page != null)
        Page.VerifyRenderingInServerForm(this);

      CreateAvailableViewsList();

      BocColumnDefinition[] renderColumns = EnsureColumnsGot();
      EvaluateWaiConformity(renderColumns);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer, GetColumnRenderers(renderColumns)));
    }

    protected virtual IBocListRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocListRenderer>();
    }

    protected virtual BocListRenderingContext CreateRenderingContext (HtmlTextWriter writer, BocColumnRenderer[] columnRenderers)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocListRenderingContext(Context, writer, this, CreateBusinessObjectWebServiceContext(), columnRenderers);
    }

    private BusinessObjectWebServiceContext CreateBusinessObjectWebServiceContext ()
    {
      return BusinessObjectWebServiceContext.Create(DataSource, Property, ControlServiceArguments);
    }

    public bool HasNavigator
    {
      get
      {
        bool hasNavigator = _alwaysShowPageInfo || _pageCount > 1;
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && _showEmptyListReadOnlyMode
                                || !isReadOnly && _showEmptyListEditMode;
        if (!HasValue && !showForEmptyList)
          hasNavigator = false;
        return hasNavigator;
      }
    }

    protected bool HasMenuBlock
    {
      get { return HasAvailableViewsList || HasOptionsMenu || HasListMenu; }
    }

    bool IBocList.HasMenuBlock
    {
      get { return HasMenuBlock; }
    }

    protected bool HasAvailableViewsList
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;

        if (! IsBrowserCapableOfScripting)
          return false;

        bool showAvailableViewsList = _showAvailableViewsList
                                      && _availableViews.Count > 1;
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && _showEmptyListReadOnlyMode
                                || ! isReadOnly && _showEmptyListEditMode;
        return showAvailableViewsList
               && (HasValue || showForEmptyList);
      }
    }

    protected bool IsBrowserCapableOfScripting
    {
      get
      {
        if (!_isBrowserCapableOfSCripting.HasValue)
        {
          var preRenderer = ServiceLocator.GetInstance<IClientScriptBehavior>();
          _isBrowserCapableOfSCripting = preRenderer.IsBrowserCapableOfScripting(Context, this);
        }
        return _isBrowserCapableOfSCripting.Value;
      }
    }

    bool IBocList.HasAvailableViewsList
    {
      get { return HasAvailableViewsList; }
    }

    protected bool HasOptionsMenu
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;

        if (! IsBrowserCapableOfScripting)
          return false;

        bool showOptionsMenu = ShowOptionsMenu
                               && OptionsMenuItems.Count > 0;
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && ShowMenuForEmptyListReadOnlyMode
                                || ! isReadOnly && ShowMenuForEmptyListEditMode;
        return showOptionsMenu
               && (HasValue || showForEmptyList);
      }
    }

    bool IBocList.HasOptionsMenu
    {
      get { return HasOptionsMenu; }
    }

    protected bool HasListMenu
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;

        if (! IsBrowserCapableOfScripting)
          return false;

        bool showListMenu = ShowListMenu
                            && ListMenuItems.Count > 0;
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && ShowMenuForEmptyListReadOnlyMode
                                || ! isReadOnly && ShowMenuForEmptyListEditMode;
        return showListMenu
               && (HasValue || showForEmptyList);
      }
    }

    bool IBocList.HasListMenu
    {
      get { return HasListMenu; }
    }

    private void CreateAvailableViewsList ()
    {
      var availableViewsList = new DropDownList();
      availableViewsList.ID = ID + c_availableViewsListIDSuffix;
      availableViewsList.EnableViewState = false;
      availableViewsList.AutoPostBack = true;
      _availableViewsListPlaceHolder.Controls.Clear();
      _availableViewsListPlaceHolder.Controls.Add(availableViewsList);

      Assertion.IsTrue(availableViewsList.Items.Count == 0, "availableViewsList should never have values after it is created.");

      if (_availableViews != null)
      {
        for (int i = 0; i < _availableViews.Count; i++)
        {
          BocListView columnDefinitionCollection = _availableViews[i];

          ListItem item = new ListItem(columnDefinitionCollection.Title, i.ToString());
          if (_renderingFeatures.EnableDiagnosticMetadata)
            item.Attributes[DiagnosticMetadataAttributes.ItemID] = columnDefinitionCollection.ItemID;
          availableViewsList.Items.Add(item);
          if (_selectedViewIndex != null && _selectedViewIndex == i)
            item.Selected = true;
        }
      }
    }

    /// <summary> Builds the input required marker. </summary>
    protected virtual Image GetRequiredMarker ()
    {
      Image requiredIcon = new Image();
      var themedResourceUrlResolver = ServiceLocator.GetInstance<IInfrastructureResourceUrlFactory>();
      requiredIcon.ImageUrl = themedResourceUrlResolver.CreateThemedResourceUrl(ResourceType.Image, c_rowEditModeRequiredFieldIcon).GetUrl();

      IResourceManager resourceManager = GetResourceManager();
      requiredIcon.AlternateText = "*";
      requiredIcon.ToolTip = resourceManager.GetString(ResourceIdentifier.RequiredFieldTitle);
      requiredIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      requiredIcon.CssClass = "validationRequiredMarker";
      return requiredIcon;
    }

    /// <summary> Builds the validation error marker. </summary>
    protected virtual Control GetValidationErrorMarker ()
    {
      Image validationErrorIcon = new Image();
      var urlFactory = ServiceLocator.GetInstance<IInfrastructureResourceUrlFactory>();
      validationErrorIcon.ImageUrl = urlFactory.CreateThemedResourceUrl(ResourceType.Image, c_rowEditModeValidationErrorIcon).GetUrl();

      IResourceManager resourceManager = GetResourceManager();
      validationErrorIcon.AlternateText = "!";
      validationErrorIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      var validationErrorMarker = new HtmlGenericControl("span");
      validationErrorMarker.Controls.Add(validationErrorIcon);
      validationErrorMarker.Attributes["class"] = "validationErrorMarker";
      validationErrorMarker.Attributes["title"] = resourceManager.GetString(ResourceIdentifier.ValidationErrorInfoTitle);

      return validationErrorMarker;
    }

    protected virtual void OnDataRowRendering (BocListDataRowRenderEventArgs e)
    {
      BocListDataRowRenderEventHandler? handler = (BocListDataRowRenderEventHandler?)Events[s_dataRowRenderEvent];
      if (handler != null)
        handler(this, e);
    }

    void IBocList.OnDataRowRendering (BocListDataRowRenderEventArgs e)
    {
      OnDataRowRendering(e);
    }

    private string GetListItemCommandArgument (int columnIndex, BocListRow row)
    {
      return c_eventListItemCommandPrefix + columnIndex + "," + RowIDProvider.GetItemRowID(row);
    }

    string IBocList.GetListItemCommandArgument (int columnIndex, BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);
      return GetListItemCommandArgument(columnIndex, row);
    }

    string IBocList.GetRowEditCommandArgument (BocListRow row, RowEditModeCommand command)
    {
      return c_eventRowEditModePrefix + RowIDProvider.GetItemRowID(row) + "," + command;
    }

    string IBocList.GetCustomCellPostBackClientEvent (int columnIndex, BocListRow row, string customCellArgument)
    {
      ArgumentUtility.CheckNotNull("row", row);

      if (_editModeController.IsRowEditModeActive)
        return "return false;";
      string postBackArgument = FormatCustomCellPostBackArgument(columnIndex, row, customCellArgument);
      return Page!.ClientScript.GetPostBackEventReference(this, postBackArgument) + ";";
    }

    void IBocList.RegisterCustomCellForSynchronousPostBack (int columnIndex, BocListRow row, string customCellArgument)
    {
      ArgumentUtility.CheckNotNull("row", row);

      if (!ControlHelper.IsNestedInUpdatePanel(this))
        return;

      var smartPage = Page as ISmartPage;
      if (smartPage == null)
      {
        throw new InvalidOperationException(
            string.Format(
                "BocList '{0}', column '{1}': Registering a custom column for a synchronous post back is only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                ID,
                columnIndex));
      }

      string postBackArgument = FormatCustomCellPostBackArgument(columnIndex, row, customCellArgument);
      smartPage.RegisterCommandForSynchronousPostBack(this, postBackArgument);
    }

    private string FormatCustomCellPostBackArgument (int columnIndex, BocListRow row, string customCellArgument)
    {
      if (customCellArgument == null)
        return c_customCellEventPrefix + columnIndex + "," + RowIDProvider.GetItemRowID(row);
      else
        return c_customCellEventPrefix + columnIndex + "," + RowIDProvider.GetItemRowID(row) + "," + customCellArgument;
    }


    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      _selectedViewIndex = (int?)values[1];
      _availableViewsListPlaceHolder.Controls.Cast<ScalarLoadPostDataTarget>().Single().Value = (string?)values[2];
      _currentPageIndex = (int)values[3]!;
      _sortingOrder = (BocListSortingOrderEntry[])values[4]!;
      _selectorControlCheckedState = (HashSet<string>)values[5]!;
      _rowIDProvider = (IRowIDProvider)values[6]!;

      Assertion.IsNotNull(_currentPagePostBackTarget, "_currentPagePostBackTarget must not be null.");

      _currentPagePostBackTarget.Value = _currentPageIndex.ToString(CultureInfo.InvariantCulture);
    }

    protected override object SaveControlState ()
    {
      object?[] values = new object?[7];

      values[0] = base.SaveControlState();
      values[1] = _selectedViewIndex;
      values[2] = _availableViewsListPlaceHolder.Controls.Cast<ScalarLoadPostDataTarget>().Single().Value;
      values[3] = _currentPageIndex;
      values[4] = GetSortingOrder();
      values[5] = _selectorControlCheckedState;
      values[6] = _rowIDProvider;

      return values;
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (Property == null)
        return;

      if (DataSource == null)
        return;

      IReadOnlyList<IBusinessObject>? valueAsList;

      if (DataSource.BusinessObject != null)
      {
        var value = DataSource.BusinessObject.GetProperty(Property);
        if (value == null)
          valueAsList = null;
        else if (value is IReadOnlyList<IBusinessObject>)
          valueAsList = (IReadOnlyList<IBusinessObject>)value;
        else if (value is IList)
          valueAsList = new BusinessObjectListAdapter<IBusinessObject>((IList)value);
        else
          throw new InvalidCastException(string.Format("Cannot cast '{0}' to type IReadOnlyList<IBusinessObject> or IList.", value.GetType()));
      }
      else
      {
        valueAsList = null;
      }

      LoadValueInternal(valueAsList, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The <see cref="IReadOnlyList{IBusinessObject}"/> of objects to load, or <see langword="null"/>.
    /// </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadUnboundValue/*' />
    public void LoadUnboundValue (IReadOnlyList<IBusinessObject>? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value">
    ///   The <see cref="IList"/> of objects to load, or <see langword="null"/>.
    /// </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadUnboundValue/*' />
    public void LoadUnboundValueAsList (IList value, bool interim)
    {
      IReadOnlyList<IBusinessObject>? valueAsList;

      if (value == null)
        valueAsList = null;
      else if (value is IReadOnlyList<IBusinessObject>)
        valueAsList = (IReadOnlyList<IBusinessObject>)value;
      else
        valueAsList = new BusinessObjectListAdapter<IBusinessObject>(value);

      LoadValueInternal(valueAsList, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="O:Remotion.ObjectBinding.Web.UI.Controls.BocList.LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (IReadOnlyList<IBusinessObject>? value, bool interim)
    {
      if (! interim)
      {
        if (_editModeController.IsRowEditModeActive)
          EndRowEditMode(false);
        else if (_editModeController.IsListEditModeActive)
          EndListEditMode(false);
      }

      if (interim)
      {
        SetValue(value, ValueMode.Interim);
      }
      else
      {
        SetValue(value, ValueMode.Complete);
        IsDirty = false;
        InitializeRowIDProvider();
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadValue/*' />
    public override bool SaveValue (bool interim)
    {
      if (Property == null)
        return false;

      if (DataSource == null)
        return false;

      if (!interim)
      {
        bool isValid = Validate();
        if (!isValid)
          return false;

        if (_editModeController.IsRowEditModeActive)
        {
          EndRowEditMode(true);
          if (_editModeController.IsRowEditModeActive)
            return false;
        }
        else if (_editModeController.IsListEditModeActive)
        {
          EndListEditMode(true);
          if (_editModeController.IsListEditModeActive)
            return false;
        }
      }

      if (!IsDirty)
        return true;

      if (SaveValueToDomainModel())
      {
        if (!interim)
          IsDirty = false;
        return true;
      }
      return false;
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    IResourceManager IControlWithResourceManager.GetResourceManager ()
    {
      return GetResourceManager();
    }

    /// <summary> Handles refreshing the bound control. </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
    private void Binding_BindingChanged (object? sender, EventArgs e)
    {
      _allPropertyColumns = null;
    }

    protected virtual void InitializeMenusItems ()
    {
    }

    protected virtual void PreRenderMenuItems ()
    {
      if (_hiddenMenuItems == null)
        return;

      BocDropDownMenu.HideMenuItems(ListMenuItems, _hiddenMenuItems);
      BocDropDownMenu.HideMenuItems(OptionsMenuItems, _hiddenMenuItems);
    }

    private BocColumnRenderer[] GetColumnRenderers (BocColumnDefinition[] columns)
    {
      var columnRendererBuilder = new BocColumnRendererArrayBuilder(columns, ServiceLocator, WcagHelper.Instance);
      columnRendererBuilder.IsListReadOnly = IsReadOnly;
      columnRendererBuilder.EnableIcon = EnableIcon;
      columnRendererBuilder.IsListEditModeActive = _editModeController.IsListEditModeActive;
      columnRendererBuilder.IsBrowserCapableOfScripting = IsBrowserCapableOfScripting;
      columnRendererBuilder.IsClientSideSortingEnabled = IsClientSideSortingEnabled;
      columnRendererBuilder.HasSortingKeys = HasSortingKeys;
      columnRendererBuilder.IsIndexEnabled = IsIndexEnabled;
      columnRendererBuilder.IsSelectionEnabled = IsSelectionEnabled;
      columnRendererBuilder.SortingOrder = GetSortingOrder();

      return columnRendererBuilder.CreateColumnRenderers();
    }

    private BocColumnDefinition[] GetAllPropertyColumns ()
    {
      if (_allPropertyColumns != null)
        return _allPropertyColumns;

      bool isBusinessObjectWithIdentity;
      IBusinessObjectProperty[] properties;
      if (DataSource == null)
      {
        properties = new IBusinessObjectProperty[0];
        isBusinessObjectWithIdentity = false;
      }
      else if (Property == null)
      {
        Assertion.IsNotNull(DataSource.BusinessObjectClass, "DataSource.BusinessObjectClass must not be null.");
        properties = DataSource.BusinessObjectClass.GetPropertyDefinitions();
        isBusinessObjectWithIdentity = DataSource.BusinessObjectClass is IBusinessObjectClassWithIdentity;
      }
      else
      {
        properties = Property.ReferenceClass.GetPropertyDefinitions();
        isBusinessObjectWithIdentity = Property.ReferenceClass is IBusinessObjectClassWithIdentity;
      }

      var allPropertyColumns = new List<BocColumnDefinition>(properties.Length);
      for (int i = 0; i < properties.Length; i++)
      {
        IBusinessObjectProperty property = properties[i];
        BocSimpleColumnDefinition column = new BocSimpleColumnDefinition();
        column.ItemID = property.Identifier;
        column.ColumnTitle = WebString.CreateFromText(property.DisplayName);
        column.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new[] { property }));
        column.OwnerControl = this;
        if (isBusinessObjectWithIdentity && property.Identifier == nameof(IBusinessObjectWithIdentity.DisplayName))
        {
          allPropertyColumns.Insert(0, column);
          column.IsRowHeader = true;
        }
        else
        {
          allPropertyColumns.Add(column);
        }
      }

      _allPropertyColumns = allPropertyColumns.ToArray();
      return _allPropertyColumns;
    }

    private void PreRenderListItemCommands ()
    {
      if (!HasValue)
        return;

      BocColumnDefinition[] columns = EnsureColumnsGot();
      var commandColumns =
          columns.Select((column, index) => new { Column = column as BocCommandEnabledColumnDefinition, Index = index })
                 .Where(d => d.Column != null && d.Column.Command != null)
                 .ToArray();

      foreach (var commandColumn in commandColumns)
      {
        foreach (var row in EnsureBocListRowsForCurrentPageGot())
        {
          commandColumn.Column!.Command!.RegisterForSynchronousPostBackOnDemand(
              this,
              GetListItemCommandArgument(commandColumn.Index, row.ValueRow),
              string.Format("BocList '{0}', Column '{1}'", ID, commandColumn.Column.ItemID));
        }
      }
    }

    private BocColumnDefinition[] EnsureColumnsGot ()
    {
      if (_columnDefinitions == null)
        _columnDefinitions = GetColumnsInternal();
      return _columnDefinitions;
    }

    /// <summary>
    ///   Compiles the <see cref="BocColumnDefinition"/> objects from the <see cref="FixedColumns"/>,
    ///   the <see cref="_allPropertyColumns"/> and the <see cref="SelectedView"/>
    ///   into a single array.
    /// </summary>
    /// <returns> An array of <see cref="BocColumnDefinition"/> objects. </returns>
    private BocColumnDefinition[] GetColumnsInternal ()
    {
      _hasAppendedAllPropertyColumnDefinitions = false;

      List<BocColumnDefinition> columnDefinitionList = new List<BocColumnDefinition>();

      AppendFixedColumns(columnDefinitionList);
      if (_showAllProperties)
        EnsureAllPropertyColumnsDefinitionsAppended(null, columnDefinitionList);
      AppendRowMenuColumn(columnDefinitionList);
      AppendSelectedViewColumns(columnDefinitionList);

      var columnDefinitions = GetColumns(columnDefinitionList.ToArray());

      CheckRowMenuColumns(columnDefinitions);

      return columnDefinitions;
    }

    private void AppendFixedColumns (List<BocColumnDefinition> columnDefinitionList)
    {
      foreach (BocColumnDefinition columnDefinition in _fixedColumns)
      {
        if (columnDefinition is BocAllPropertiesPlaceholderColumnDefinition)
        {
          EnsureAllPropertyColumnsDefinitionsAppended(
              (BocAllPropertiesPlaceholderColumnDefinition)columnDefinition, columnDefinitionList);
        }
        else
          columnDefinitionList.Add(columnDefinition);
      }
    }

    private void AppendRowMenuColumn (List<BocColumnDefinition> columnDefinitionList)
    {
      BocDropDownMenuColumnDefinition? dropDownMenuColumn = GetRowMenuColumn();
      if (dropDownMenuColumn != null)
        columnDefinitionList.Add(dropDownMenuColumn);
    }

    private void AppendSelectedViewColumns (List<BocColumnDefinition> columnDefinitionList)
    {
      EnsureSelectedViewIndexSet();
      if (_selectedView == null)
        return;

      foreach (BocColumnDefinition columnDefinition in _selectedView.ColumnDefinitions)
      {
        if (columnDefinition is BocAllPropertiesPlaceholderColumnDefinition)
        {
          EnsureAllPropertyColumnsDefinitionsAppended(
              (BocAllPropertiesPlaceholderColumnDefinition)columnDefinition, columnDefinitionList);
        }
        else
          columnDefinitionList.Add(columnDefinition);
      }
    }

    private void EnsureAllPropertyColumnsDefinitionsAppended (
        BocAllPropertiesPlaceholderColumnDefinition? placeholderColumnDefinition, List<BocColumnDefinition> columnDefinitionList)
    {
      if (_hasAppendedAllPropertyColumnDefinitions)
        return;

      BocColumnDefinition[] allPropertyColumnDefinitions = GetAllPropertyColumns();
      Unit width = Unit.Empty;
      string cssClass = string.Empty;

      if (placeholderColumnDefinition != null)
      {
        if (! placeholderColumnDefinition.Width.IsEmpty)
        {
          double value = placeholderColumnDefinition.Width.Value / allPropertyColumnDefinitions.Length;
          value = Math.Round(value, 1);
          width = new Unit(value, placeholderColumnDefinition.Width.Type);
        }
        cssClass = placeholderColumnDefinition.CssClass;
      }

      foreach (BocColumnDefinition columnDefinition in allPropertyColumnDefinitions)
      {
        columnDefinition.CssClass = cssClass;
        columnDefinition.Width = width;
      }

      columnDefinitionList.AddRange(allPropertyColumnDefinitions);
      _hasAppendedAllPropertyColumnDefinitions = true;
    }

    /// <summary>
    ///   Override this method to modify the column definitions displayed in the <see cref="BocList"/> in the
    ///   current page life cycle.
    /// </summary>
    /// <remarks>
    ///   This call can happen more than once in the control's life cycle, passing different 
    ///   arrays in <paramref name="columnDefinitions" />. It is therefor important to not cache the return value
    ///   in the override of <see cref="GetColumns"/>.
    /// </remarks>
    /// <param name="columnDefinitions"> 
    ///   The <see cref="BocColumnDefinition"/> array containing the columns defined by the <see cref="BocList"/>. 
    /// </param>
    /// <returns> The <see cref="BocColumnDefinition"/> array. </returns>
    protected virtual BocColumnDefinition[] GetColumns (BocColumnDefinition[] columnDefinitions)
    {
      return columnDefinitions;
    }

    /// <summary>
    ///   Gets a flag set <see langword="true"/> if the <see cref="Value"/> is sorted before it is displayed.
    /// </summary>
    [Browsable(false)]
    public bool HasSortingKeys
    {
      get { return _sortingOrder.Any(entry => !entry.IsEmpty); }
    }

    /// <summary> Sets the sorting order for the <see cref="BocList"/>. </summary>
    /// <remarks>
    ///   <para>
    ///     It is recommended to only set the sorting order when the <see cref="BocList"/> is initialized for the first 
    ///     time. During subsequent postbacks, setting the sorting order before the post back events of the 
    ///     <see cref="BocList"/> have been handled, will undo the user's chosen sorting order.
    ///   </para><para>
    ///     Does not raise the <see cref="SortingOrderChanging"/> and <see cref="SortingOrderChanged"/>.
    ///   </para><para>
    ///     Use <see cref="ClearSortingOrder"/> if you need to clear the sorting order.
    ///   </para>
    /// </remarks>
    /// <param name="newSortingOrder"> 
    ///   The new sorting order of the <see cref="BocList"/>. Must not be <see langword="null"/> or contain 
    ///   <see langword="null"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">EnableMultipleSorting == False &amp;&amp; sortingOrder.Length > 1</exception>
    public void SetSortingOrder (params BocListSortingOrderEntry[] newSortingOrder)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("newSortingOrder", newSortingOrder);

      if (! IsMultipleSortingEnabled && newSortingOrder.Length > 1)
        throw new InvalidOperationException(string.Format("Attempted to set multiple sorting keys on BocList '{0}' but EnableMultipleSorting is False.", ID));
      else
        _sortingOrder = newSortingOrder.ToArray();

      OnSortedRowsChanged();
    }

    /// <summary> Clears the sorting order for the <see cref="BocList"/>. </summary>
    /// <remarks>
    ///   Does not raise the <see cref="SortingOrderChanging"/> and <see cref="SortingOrderChanged"/>.
    /// </remarks>
    public void ClearSortingOrder ()
    {
      _sortingOrder = new BocListSortingOrderEntry[0];

      OnSortedRowsChanged();
    }

    /// <summary>
    ///   Gets the sorting order for the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    ///   If the list also contains a collection of available views, then this method shoud only be called after the 
    ///   <see cref="AvailableViews"/> have been set. Otherwise the result can vary from a wrong sorting order to an 
    ///   <see cref="IndexOutOfRangeException"/>.
    /// </remarks>
    public BocListSortingOrderEntry[] GetSortingOrder ()
    {
      var columns = EnsureColumnsGot();

      Func<BocListSortingOrderEntry, BocListSortingOrderEntry> entryProcessor =
          entry =>
          {
            if (entry.Column == null)
            {
              var newEntry = new BocListSortingOrderEntry((IBocSortableColumnDefinition)columns[entry.ColumnIndex], entry.Direction);
              newEntry.SetColumnIndex(entry.ColumnIndex);
              return newEntry;
            }
            else
            {
              var newIndex = Array.IndexOf(columns, entry.Column);
              if (newIndex == entry.ColumnIndex)
              {
                return entry;
              }
              else if (newIndex >= 0)
              {
                entry.SetColumnIndex(newIndex);
                return entry;
              }
              else
              {
                var newEntry = new BocListSortingOrderEntry((IBocSortableColumnDefinition)columns[entry.ColumnIndex], entry.Direction);
                newEntry.SetColumnIndex(entry.ColumnIndex);
                return newEntry;
              }
            }
          };

      _sortingOrder = _sortingOrder.Where(entry => !entry.IsEmpty).Select(entryProcessor).ToArray();

      return _sortingOrder.ToArray();
    }

    /// <summary>
    ///   Sorts the <see cref="Value"/>'s <see cref="IBusinessObject"/> instances using the sorting keys
    ///   and returns the sorted <see cref="IBusinessObject"/> instances as a new array. The original values remain
    ///   untouched.
    /// </summary>
    /// <returns> 
    ///   An <see cref="IBusinessObject"/> array sorted by the sorting keys or <see langword="null"/> if the list is
    ///   not sorted.
    /// </returns>
    public IBusinessObject[]? GetSortedRows ()
    {
      if (! HasSortingKeys)
        return null;

      var sortedRows = EnsureSortedBocListRowsGot();

      return sortedRows.Select(r => r.BusinessObject).ToArray();
    }

    protected ReadOnlyCollection<BocListRow> EnsureSortedBocListRowsGot ()
    {
      if (_indexedRowsSorted == null)
        return _indexedRowsSorted = GetSortedBocListRows().ToList().AsReadOnly();
      return _indexedRowsSorted;
    }

    protected IEnumerable<BocListRow> GetSortedBocListRows ()
    {
      if (!HasValue)
        return new BocListRow[0];

      var rows = Value.Cast<IBusinessObject>().Select((row, rowIndex) => new BocListRow(rowIndex, row));

      return SortBocListRows(rows, GetSortingOrder());
    }

    protected virtual IEnumerable<BocListRow> SortBocListRows (IEnumerable<BocListRow> rows, BocListSortingOrderEntry[] sortingOrder)
    {
      ArgumentUtility.CheckNotNull("rows", rows);
      ArgumentUtility.CheckNotNull("sortingOrder", sortingOrder);

      return rows.OrderBy(sortingOrder);
    }

    protected ReadOnlyCollection<SortedRow> EnsureBocListRowsForCurrentPageGot ()
    {
      if (_currentPageRows == null)
        _currentPageRows = GetBocListRowsForCurrentPage().ToList().AsReadOnly();
      return _currentPageRows;
    }

    protected IEnumerable<SortedRow> GetBocListRowsForCurrentPage ()
    {
      var result = EnsureSortedBocListRowsGot().Select((row, index) => new SortedRow(row, index));

      if (IsPagingEnabled)
      {
        // ReSharper disable PossibleInvalidOperationException
        int pageSize = PageSize.Value;
        // ReSharper restore PossibleInvalidOperationException
        result = result.Skip(_currentPageIndex * pageSize).Take(pageSize);
      }

      return result;
    }

    BocListRowRenderingContext[] IBocList.GetRowsToRender ()
    {
      return EnsureBocListRowsForCurrentPageGot()
          .Select(
              data => new BocListRowRenderingContext(
                          data.ValueRow,
                          data.SortedIndex,
                          _selectorControlCheckedState.Contains(RowIDProvider.GetItemRowID(data.ValueRow))))
          .ToArray();
    }

    /// <summary>
    ///   Removes the columns provided by <see cref="SelectedView"/> from the <see cref="_sortingOrder"/> list.
    /// </summary>
    private void RemoveDynamicColumnsFromSortingOrder ()
    {
      if (HasSortingKeys)
      {
        var staticColumns = new HashSet<BocColumnDefinition?>(_fixedColumns.Cast<BocColumnDefinition>().Concat(GetAllPropertyColumns()));
        var oldSortingOrder = GetSortingOrder();
        var oldCount = oldSortingOrder.Length;
        _sortingOrder = oldSortingOrder.Where(entry => staticColumns.Contains((BocColumnDefinition?)entry.Column)).ToArray();
      }
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary<string, WebString> values)
    {
      ArgumentUtility.CheckNotNull("values", values);
      Dispatch(values);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    protected virtual void Dispatch (IDictionary<string, WebString> values)
    {
      var fixedColumnValues = new Dictionary<string, IDictionary<string, WebString>>();
      var optionsMenuItemValues = new Dictionary<string, IDictionary<string, WebString>>();
      var listMenuItemValues = new Dictionary<string, IDictionary<string, WebString>>();
      var propertyValues = new Dictionary<string, WebString>();

      //  Parse the values

      foreach (var entry in values)
      {
        string key = entry.Key;
        string[] keyParts = key.Split(new[] { ':' }, 3);

        //  Is a property/value entry?
        if (keyParts.Length == 1)
        {
          string property = keyParts[0];
          propertyValues.Add(property, entry.Value);
        }
            //  Is collection entry?
        else if (keyParts.Length == 3)
        {
          //  Compound key: "collectionID:elementID:property"
          string collectionID = keyParts[0];
          string elementID = keyParts[1];
          string property = keyParts[2];

          IDictionary<string,IDictionary<string,WebString>>? currentCollection = null;

          //  Switch to the right collection
          switch (collectionID)
          {
            case c_resourceKeyFixedColumns:
            {
              currentCollection = fixedColumnValues;
              break;
            }
            case c_resourceKeyOptionsMenuItems:
            {
              currentCollection = optionsMenuItemValues;
              break;
            }
            case c_resourceKeyListMenuItems:
            {
              currentCollection = listMenuItemValues;
              break;
            }
            default:
            {
              //  Invalid collection property
              s_log.Debug(
                  "BocList '" + ID + "' in naming container '" + NamingContainer.GetType().GetFullNameSafe() + "' on page '" + Page
                  + "' does not contain a collection property named '" + collectionID + "'.");
              break;
            }
          }

          //  Add the property/value pair to the collection
          if (currentCollection != null)
          {
            //  Get the dictonary for the current element
            //  If no dictonary exists, create it and insert it into the elements hashtable.
            if (!currentCollection.TryGetValue(elementID, out var elementValues))
            {
              elementValues = new Dictionary<string, WebString>();
              currentCollection[elementID] = elementValues;
            }

            //  Insert the argument and resource's value into the dictonary for the specified element.
            elementValues.Add(property, entry.Value);
          }
        }
        else
        {
          //  Not supported format or invalid property
          s_log.Debug(
              "BocList '" + ID + "' in naming container '" + NamingContainer.GetType().GetFullNameSafe() + "' on page '" + Page
              + "' received a resource with an invalid or unknown key '" + key
              + "'. Required format: 'property' or 'collectionID:elementID:property'.");
        }
      }

      //  Dispatch simple properties
      ResourceDispatcher.DispatchGeneric(this, propertyValues);

      //  Dispatch to collections
      _fixedColumns.Dispatch(fixedColumnValues, this, "FixedColumns");
      OptionsMenuItems.Dispatch(optionsMenuItemValues, this, "OptionsMenuItems");
      ListMenuItems.Dispatch(listMenuItemValues, this, "ListMenuItems");
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);

      string? key;
      key = ResourceManagerUtility.GetGlobalResourceKey(IndexColumnTitle.GetValue());
      if (! string.IsNullOrEmpty(key))
        IndexColumnTitle = resourceManager.GetWebString(key, IndexColumnTitle.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(EmptyListMessage.GetValue());
      if (! string.IsNullOrEmpty(key))
        EmptyListMessage = resourceManager.GetWebString(key, EmptyListMessage.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(OptionsTitle.GetValue());
      if (! string.IsNullOrEmpty(key))
        OptionsTitle = resourceManager.GetWebString(key, OptionsTitle.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(AvailableViewsListTitle.GetValue());
      if (! string.IsNullOrEmpty(key))
        AvailableViewsListTitle = resourceManager.GetWebString(key, AvailableViewsListTitle.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(ErrorMessage.GetValue());
      if (! string.IsNullOrEmpty(key))
        ErrorMessage = resourceManager.GetText(key);

      _fixedColumns.LoadResources(resourceManager, globalizationService);
      OptionsMenuItems.LoadResources(resourceManager, globalizationService);
      ListMenuItems.LoadResources(resourceManager, globalizationService);
    }

    /// <summary> Is raised when a data row is rendered. </summary>
    [Category("Action")]
    [Description("Occurs when a data row is rendered.")]
    public event BocListDataRowRenderEventHandler DataRowRender
    {
      add { Events.AddHandler(s_dataRowRenderEvent, value); }
      remove { Events.RemoveHandler(s_dataRowRenderEvent, value); }
    }

    /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
    /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectReferenceProperty? Property
    {
      get { return (IBusinessObjectReferenceProperty?)base.Property; }
      set { base.Property = ArgumentUtility.CheckType<IBusinessObjectReferenceProperty>("value", value); }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> An object implementing <see cref="IList"/>. </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    [Browsable(false)]
    public new IReadOnlyList<IBusinessObject>? Value
    {
      get { return GetValue(); }
      set
      {
        SetValue(value, ValueMode.Complete);
        IsDirty = true;
        InitializeRowIDProvider();
      }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> A list of <see cref="IBusinessObject"/> implementations or <see langword="null"/>. </value>
    [Browsable(false)]
    public IList? ValueAsList
    {
      get
      {
        var value = Value;

        if (value == null)
          return null;
        else if (value is BusinessObjectListAdapter<IBusinessObject>)
          return ((BusinessObjectListAdapter<IBusinessObject>)value).WrappedList;
        else if (value is IList)
          return (IList)value;
        else
          throw new InvalidOperationException("The value only implements the IReadOnlyList<IBusinessObject> interface. Use the Value property to access the value.");
      }
      set
      {
        if (value == null)
          Value = null;
        else if (value is IReadOnlyList<IBusinessObject>)
          Value = (IReadOnlyList<IBusinessObject>)value;
        else
          Value = new BusinessObjectListAdapter<IBusinessObject>(value);
      }
    }

    /// <summary>
    /// Gets the value from the backing field.
    /// </summary>
    private IReadOnlyList<IBusinessObject>? GetValue ()
    {
      return _value;
    }

    /// <summary>
    /// Sets the value from the backing field.
    /// </summary>
    /// <remarks>
    /// <para>Setting the value via this method does not affect the control's dirty state.</para>
    /// </remarks>
    private void SetValue (IReadOnlyList<IBusinessObject>? value, ValueMode mode)
    {
      _value = value;
      OnSortedRowsChanged();

      if (mode == ValueMode.Complete)
      {
        _currentPageIndex = 0;
        OnDisplayedRowsChanged();
      }
    }

    /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
    /// <value> The value must be of type <see cref="IList"/>. </value>
    protected sealed override object? ValueImplementation
    {
      get
      {
        var value = Value;
        if (value is BusinessObjectListAdapter<IBusinessObject>)
          return ((BusinessObjectListAdapter<IBusinessObject>)value).WrappedList;
        else
          return value;
      }
      set
      {
        if (value == null)
        {
          Value = null;
        }
        else if (value is IReadOnlyList<IBusinessObject>)
        {
          Value = (IReadOnlyList<IBusinessObject>)value;
        }
        else if (value is IList)
        {
          Value = new BusinessObjectListAdapter<IBusinessObject>((IList)value);
        }
        else
        {
          throw new ArgumentException(
              string.Format(
                  "Parameter type '{0}' is not supported. Parameters must implement interface IReadOnlyList<IBusinessObject> or IList.",
                  value.GetType()),
              "value");
        }
      }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocList"/> contains a value. </summary>
    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(true, nameof(Value))]
    public override bool HasValue
    {
      get { return _value != null && _value.Count > 0; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="false"/>. </value>
    public override bool UseLabel
    {
      get { return false; }
    }

    /// <summary> Gets or sets the dirty flag. </summary>
    /// <value> 
    ///   Evaluates <see langword="true"/> if either the <see cref="BocList"/> or one of the edit mode controls is 
    ///   dirty.
    /// </value>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.IsDirty">BusinessObjectBoundEditableWebControl.IsDirty</seealso>
    public override bool IsDirty
    {
      get
      {
        if (base.IsDirty)
          return true;

        return _editModeController.IsDirty();
      }
      set { base.IsDirty = value; }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   Returns the <see cref="Control.ClientID"/> values of the edit mode controls for the row currently being edited.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      if (IsReadOnly)
        return new string[0];
      else
        return _editModeController.GetTrackedClientIDs();
    }

    /// <summary> The <see cref="BocList"/> supports properties of type <see cref="IBusinessObjectReferenceProperty"/>. </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary> The <see cref="BocList"/> supports only list properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return isList;
    }

    /// <summary> Gets the user independent column definitions. </summary>
    /// <remarks> Behavior undefined if set after initialization phase or changed between postbacks. </remarks>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    //  Default category
    [Description("The user independent column definitions.")]
    [DefaultValue((string?)null)]
    public BocColumnDefinitionCollection FixedColumns
    {
      get { return _fixedColumns; }
    }

    //  No designer support intended
    /// <summary> Gets the predefined column definition sets that the user can choose from at run-time. </summary>
    //  [PersistenceMode(PersistenceMode.InnerProperty)]
    //  [ListBindable (false)]
    //  //  Default category
    //  [Description ("The predefined column definition sets that the user can choose from at run-time.")]
    //  [DefaultValue ((string) null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BocListViewCollection AvailableViews
    {
      get { return _availableViews; }
    }

    /// <summary>
    ///   Gets or sets the selected <see cref="BocListView"/> used to
    ///   supplement the <see cref="FixedColumns"/>.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BocListView? SelectedView
    {
      get
      {
        EnsureSelectedViewIndexSet();
        return _selectedView;
      }
      set
      {
        bool hasChanged = _selectedView != value;
        _selectedView = value;
        ArgumentUtility.CheckNotNullOrEmpty("AvailableViews", _availableViews);
        _selectedViewIndex = null;

        if (_selectedView != null)
        {
          for (int i = 0; i < _availableViews.Count; i++)
          {
            if (_availableViews[i] == _selectedView)
            {
              _selectedViewIndex = i;
              break;
            }
          }

          if (_selectedViewIndex == null)
            throw new ArgumentOutOfRangeException("value");
        }

        if (hasChanged)
        {
          RemoveDynamicColumnsFromSortingOrder();
          OnColumnsChanged();
        }
      }
    }

    private void EnsureSelectedViewIndexSet ()
    {
      if (_isSelectedViewIndexSet)
        return;
      if (_selectedViewIndex == null)
        SelectedViewIndex = _selectedViewIndex;
      else if (_availableViews.Count == 0)
        SelectedViewIndex = null;
      else if (_selectedViewIndex.Value >= _availableViews.Count)
        SelectedViewIndex = _availableViews.Count - 1;
      else
        SelectedViewIndex = _selectedViewIndex;
      _isSelectedViewIndexSet = true;
    }

    /// <summary>
    ///   Gets or sets the index of the selected <see cref="BocListView"/> used to
    ///   supplement the <see cref="FixedColumns"/>.
    /// </summary>
    private int? SelectedViewIndex
    {
      get { return _selectedViewIndex; }
      set
      {
        if (value != null
            && (value.Value < 0 || value.Value >= _availableViews.Count))
          throw new ArgumentOutOfRangeException("value");

        if ((_editModeController.IsRowEditModeActive || _editModeController.IsListEditModeActive)
            && _isSelectedViewIndexSet
            && _selectedViewIndex != value)
        {
          throw new InvalidOperationException(
              string.Format("The selected column definition set cannot be changed while the BocList '{0}' is in row edit mode.", ID));
        }

        bool hasIndexChanged = _selectedViewIndex != value;
        _selectedViewIndex = value;

        _selectedView = null;
        if (_selectedViewIndex != null)
        {
          int selectedIndex = _selectedViewIndex.Value;
          if (selectedIndex < _availableViews.Count)
            _selectedView = _availableViews[selectedIndex];
        }

        if (hasIndexChanged)
        {
          RemoveDynamicColumnsFromSortingOrder();
          OnColumnsChanged();
        }
      }
    }

    private void AvailableViews_CollectionChanged (object? sender, CollectionChangeEventArgs e)
    {
      if (_selectedViewIndex == null
          && _availableViews.Count > 0)
        _selectedViewIndex = 0;
      else if (_selectedViewIndex >= _availableViews.Count)
      {
        if (_availableViews.Count > 0)
          _selectedViewIndex = _availableViews.Count - 1;
        else
          _selectedViewIndex = null;
      }
    }

    public void ClearSelectedRows ()
    {
      _selectorControlCheckedState.Clear();
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
    /// <returns> An array of <see cref="IBusinessObject"/> objects. </returns>
    public IBusinessObject[] GetSelectedBusinessObjects ()
    {
      return GetSelectedRowsInternal().Select(r => r.BusinessObject).ToArray();
    }

    /// <summary> Gets indices for the rows selected in the <see cref="BocList"/>. </summary>
    /// <returns> An array of <see cref="int"/> values. </returns>
    public int[] GetSelectedRows ()
    {
      return GetSelectedRowsInternal().Select(r => r.Index).ToArray();
    }

    private IEnumerable<BocListRow> GetSelectedRowsInternal ()
    {
      if (Value == null)
        return Enumerable.Empty<BocListRow>();

      return _selectorControlCheckedState
          .Select(rowID => RowIDProvider.GetRowFromItemRowID(Value, rowID)!)
          .Where(r => r != null)
          .OrderBy(r => r.Index);
    }

    /// <summary> Sets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
    /// <param name="selectedObjects"> An <see cref="IReadOnlyList{IBusinessObject}"/> of objects. </param>>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if the number of rows do not match the <see cref="Selection"/> mode 
    ///   or the <see cref="Value"/> is <see langword="null"/>.
    /// </exception>
    public void SetSelectedBusinessObjects (IReadOnlyList<IBusinessObject> selectedObjects)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("selectedObjects", selectedObjects);

      if (Value == null)
        throw new InvalidOperationException(string.Format("The BocList '{0}' does not have a Value.", ID));

      var selectedRows = ListUtility.IndicesOf(Value, selectedObjects);
      SetSelectedRows(selectedRows.ToArray());
    }


    /// <summary> Sets indices for the rows selected in the <see cref="BocList"/>. </summary>
    /// <param name="selectedRows"> An array of <see cref="int"/> values. </param>
    /// <exception cref="InvalidOperationException"> Thrown if the number of rows do not match the <see cref="Selection"/> mode.</exception>
    public void SetSelectedRows (int[] selectedRows)
    {
      if (Value == null)
        throw new InvalidOperationException(string.Format("The BocList '{0}' does not have a Value.", ID));

      foreach (var rowIndex in selectedRows)
      {
        if (rowIndex < 0)
          throw new ArgumentException("Negative row-indices are not supported for selection.", "selectedRows");

        if (rowIndex >= Value.Count)
        {
          throw new InvalidOperationException(
              string.Format(
                  "The Value of the BocList '{0}' only contains {1} rows but an attempt was made to select row #{2}.",
                  ID,
                  Value.Count,
                  rowIndex));
        }
      }

      SetSelectedRows(selectedRows.Select(rowIndex => new BocListRow(rowIndex, Value[rowIndex])).ToArray());
    }

    private void SetSelectedRows (BocListRow[] selectedRows)
    {
      if ((_selection == RowSelection.Undefined || _selection == RowSelection.Disabled)
          && selectedRows.Length > 0)
      {
        throw new InvalidOperationException(string.Format("Cannot select rows if the BocList '{0}' is set to RowSelection.Disabled.", ID));
      }

      if ((_selection == RowSelection.SingleCheckBox
           || _selection == RowSelection.SingleRadioButton)
          && selectedRows.Length > 1)
      {
        throw new InvalidOperationException(string.Format("Cannot select more than one row if the BocList '{0}' is set to RowSelection.Single.", ID));
      }

      _selectorControlCheckedState.Clear();
      foreach (var row in selectedRows)
        _selectorControlCheckedState.Add(RowIDProvider.GetItemRowID(row));
    }

    /// <summary>
    /// Synchronizes the <see cref="Value"/> collection with the rows rendered on the page.
    /// </summary>
    /// <remarks>
    /// Synchronization is needed when changes are made to the <see cref="Value"/> collection via a domain operation after <see cref="LoadValue"/> has been called.
    /// When chaning the <see cref="Value"/> collection via APIs exposed by the <see cref="BocList"/> itself (e.g. <see cref="AddRow"/>), explicit
    /// synchronization is not required.
    /// </remarks>
    public void SynchronizeRows ()
    {
      OnSortedRowsChanged();
      ((EditModeController)_editModeController).SynchronizeEditModeControls(EnsureColumnsGot());
    }

    /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void AddRows (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);

      _editModeController.AddRows(businessObjects, EnsureColumnsGot());
    }

    /// <summary> Adds the <paramref name="businessObject"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public int AddRow (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      return _editModeController.AddRow(businessObject, EnsureColumnsGot());
    }

    /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void RemoveRows (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);

      _editModeController.RemoveRows(businessObjects);
    }

    /// <summary> Removes the <paramref name="businessObject"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void RemoveRow (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      _editModeController.RemoveRow(businessObject);
    }

    /// <summary> 
    ///   Removes the <see cref="IBusinessObject"/> at the specifed <paramref name="index"/> from the 
    ///   <see cref="Value"/> collection. 
    /// </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void RemoveRow (int index)
    {
      if (Value == null)
        return;
      if (index > Value.Count)
        throw new ArgumentOutOfRangeException("index");

      RemoveRow(Value[index]);
    }

    private BocListRow[] AddRowsImplementation (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);

      IList? valueAsList;
      try
      {
        valueAsList = ValueAsList;
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException(
            "The BocList is bound to a collection that does not implement the IList interface. "
            + "Add and remove rows is not supported for collections that do not implement the IList interface.",
            ex);
      }

      var newValue = ListUtility.AddRange(valueAsList, businessObjects, Property, false, true);

      if (newValue == null)
      {
        Value = null;
        return new BocListRow[0];
      }
      else
      {
        IReadOnlyList<IBusinessObject> newValueAsReadOnlyList;
        if (newValue is IReadOnlyList<IBusinessObject>)
          newValueAsReadOnlyList = (IReadOnlyList<IBusinessObject>)newValue;
        else
          newValueAsReadOnlyList = new BusinessObjectListAdapter<IBusinessObject>(newValue);

        SetValue(newValueAsReadOnlyList, ValueMode.Complete);
        IsDirty = true;

        var rows = ListUtility.IndicesOf(newValueAsReadOnlyList, businessObjects).ToArray();
        foreach (var row in rows.OrderBy(r => r.Index))
          RowIDProvider.AddRow(row);

        return rows;
      }
    }

    private BocListRow[] RemoveRowsImplementation (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);

      IList? valueAsList;
      try
      {
        valueAsList = ValueAsList;
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException(
            "The BocList is bound to a collection that does not implement the IList interface. "
            + "Add and remove rows is not supported for collections that do not implement the IList interface.",
            ex);
      }

      if (valueAsList == null)
        return new BocListRow[0];

      var rows = ListUtility.IndicesOf(valueAsList.Cast<IBusinessObject>(), businessObjects).ToArray();
      var newValue = ListUtility.Remove(valueAsList, rows.Select(r => r.BusinessObject).ToArray(), Property, false);

      if (newValue == null)
      {
        Value = null;
        return rows;
      }
      else
      {
        IReadOnlyList<IBusinessObject> newValueAsReadOnlyList;
        if (newValue is IReadOnlyList<IBusinessObject>)
          newValueAsReadOnlyList = (IReadOnlyList<IBusinessObject>)newValue;
        else
          newValueAsReadOnlyList = new BusinessObjectListAdapter<IBusinessObject>(newValue);

        SetValue(newValueAsReadOnlyList, ValueMode.Complete);
        IsDirty = true;

        foreach (var row in rows.OrderByDescending(r => r.Index))
          RowIDProvider.RemoveRow(row);

        return rows;
      }
    }

    /// <summary>
    ///   Saves changes to previous edited row and starts editing for the specified row.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     Once the list is in edit mode, it is important not to change to index of the edited 
    ///     <see cref="IBusinessObject"/> in <see cref="Value"/>. Otherwise the wrong object would be edited.
    ///     Use <see cref="IsRowEditModeActive"/> to programatically check whether it is save to insert a row.
    ///     It is always save to add a row using the <see cref="AddRow"/> and <see cref="AddRows"/> methods.
    ///   </para><para>
    ///     While the list is in row edit mode, all commands and menus for this list are disabled with the exception
    ///     of those rendered in the <see cref="BocRowEditModeColumnDefinition"/> column.
    ///   </para>
    /// </remarks>
    /// <param name="index"> The index of the row to be edited. </param>
    public void SwitchRowIntoEditMode (int index)
    {
      _editModeController.SwitchRowIntoEditMode(index, EnsureColumnsGot());
      OnStateOfDisplayedRowsChanged();
    }

    /// <summary>
    ///   Saves changes to the edited row rows and (re-)starts editing for the entire list.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     Once the list is in edit mode, it is important not to change to order of the objects in <see cref="Value"/>. 
    ///     Otherwise the wrong objects would be edited. Use <see cref="IsListEditModeActive"/> to programatically check 
    ///     whether it is save to insert a row.
    ///   </para>
    /// </remarks>
    public void SwitchListIntoEditMode ()
    {
      if (IsPagingEnabled)
      {
        throw new InvalidOperationException(
            string.Format(
                "Cannot switch BocList '{0}' in to List Edit Mode: Paging Enabled.", ID));
      }

      _editModeController.SwitchListIntoEditMode(EnsureColumnsGot());
      OnStateOfDisplayedRowsChanged();
    }

    /// <summary> 
    ///   Ends the current edit mode, appends the <paramref name="businessObject"/> to the list and switches the new
    ///   row into edit mode.
    /// </summary>
    /// <remarks>
    ///   If already in row edit mode and the previous row cannot be saved, the new row will not be added to the list.
    /// </remarks>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> to add. Must not be <see langword="null"/>. </param>
    public bool AddAndEditRow (IBusinessObject businessObject)
    {
      return _editModeController.AddAndEditRow(businessObject, EnsureColumnsGot());
    }

    /// <summary>
    ///   Ends the current edit mode and optionally validates and saves the changes made during edit mode.
    /// </summary>
    /// <remarks> 
    ///   If <paramref name="saveChanges"/> is <see langword="true"/>, the edit mode will only be ended once the 
    ///   validation has been successful.
    /// </remarks>
    /// <param name="saveChanges"> 
    ///   <see langword="true"/> to validate and save the changes, <see langword="false"/> to discard them.
    /// </param>
    public void EndRowEditMode (bool saveChanges)
    {
      _editModeController.EndRowEditMode(saveChanges, EnsureColumnsGot());
    }

    /// <summary>
    ///   Ends the current edit mode and optionally validates and saves the changes made during edit mode.
    /// </summary>
    /// <remarks> 
    ///   If <paramref name="saveChanges"/> is <see langword="true"/>, the edit mode will only be ended once the 
    ///   validation has been successful.
    /// </remarks>
    /// <param name="saveChanges"> 
    ///   <see langword="true"/> to validate and save the changes, <see langword="false"/> to discard them.
    /// </param>
    public void EndListEditMode (bool saveChanges)
    {
      _editModeController.EndListEditMode(saveChanges, EnsureColumnsGot());
    }

    private void EnsureEditModeRestored ()
    {
      _editModeController.EnsureEditModeRestored(EnsureColumnsGot());
    }

    private void EnsureEditModeValidatorsRestored ()
    {
      _editModeController.EnsureValidatorsRestored();
    }

    private void EndRowEditModeCleanUp (int modifiedRowIndex)
    {
      if (! IsReadOnly)
      {
        OnSortedRowsChanged();
        _editedRowIndex = modifiedRowIndex;
      }
    }

    private void EndListEditModeCleanUp ()
    {
      if (! IsReadOnly)
        OnSortedRowsChanged();
    }

    /// <summary> Explicitly validates the changes made to the edit mode. </summary>
    /// <returns> <see langword="true"/> if the rows contain only valid values. </returns>
    public bool ValidateEditableRows ()
    {
      return _editModeController.Validate();
    }

    private EditModeValidator? GetEditModeValidator ()
    {
      return _validators.GetValidator<EditModeValidator>();
    }

    private void SetFocusImplementation (IFocusableControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      var focusID = control.FocusID;
      if (string.IsNullOrEmpty(focusID))
        return;

      Page!.SetFocus(focusID);
    }

    /// <summary>
    /// Gets the <see cref="BocListRow"/> currently being edited in row-edit-mode.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="BocList"/> is not currently in row-edit-mode or the <see cref="Value"/> has not yet been set.
    /// </exception>
    public BocListRow GetEditedRow ()
    {
      return _editModeController.GetEditedRow();
    }

    /// <summary> Gets a flag that determines wheter the <see cref="BocList"/> is n row edit mode. </summary>
    /// <remarks>
    ///   Queried where the rendering depends on whether the list is in edit mode. 
    ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
    /// </remarks>
    [Browsable(false)]
    public bool IsRowEditModeActive
    {
      get { return _editModeController.IsRowEditModeActive; }
    }

    /// <summary> Gets a flag that determines wheter the <see cref="BocList"/> is n row edit mode. </summary>
    /// <remarks>
    ///   Queried where the rendering depends on whether the list is in edit mode. 
    ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
    /// </remarks>
    [Browsable(false)]
    public bool IsListEditModeActive
    {
      get { return _editModeController.IsListEditModeActive; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to show the asterisks in the title row for columns having 
    ///   edit mode controls.
    /// </summary>
    [Description("Set false to hide the asterisks in the title row for columns having edit mode control.")]
    [Category("Edit Mode")]
    [DefaultValue(true)]
    public bool ShowEditModeRequiredMarkers
    {
      get { return _showEditModeRequiredMarkers; }
      set { _showEditModeRequiredMarkers = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to show an exclamation mark in front of each control with 
    ///   an validation error.
    /// </summary>
    [Description("Set true to show an exclamation mark in front of each control with an validation error.")]
    [Category("Edit Mode")]
    [DefaultValue(false)]
    public bool ShowEditModeValidationMarkers
    {
      get { return _showEditModeValidationMarkers; }
      set { _showEditModeValidationMarkers = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to render validation messages and client side validators.
    /// </summary>
    [Description("Set true to prevent the validation messages from being rendered. This also disables any client side validation in the edited row.")
    ]
    [Category("Edit Mode")]
    [DefaultValue(false)]
    public bool DisableEditModeValidationMessages
    {
      get { return _disableEditModeValidationMessages; }
      set { _disableEditModeValidationMessages = value; }
    }

    /// <summary> Gets or sets a flag that enables the <see cref="EditModeValidator"/>. </summary>
    /// <remarks> 
    ///   <see langword="false"/> to prevent the <see cref="EditModeValidator"/> from being created by
    ///   <see cref="CreateValidators(bool)"/>.
    /// </remarks>
    [Description("Enables the EditModeValidator.")]
    [Category("Edit Mode")]
    [DefaultValue(true)]
    public bool EnableEditModeValidator
    {
      get { return _enableEditModeValidator; }
      set { _enableEditModeValidator = value; }
    }

    /// <summary> Gets or sets a flag for automatically setting the focus to the first editable control when the list is switched into edit mode. </summary>
    /// <remarks> 
    ///   <see langword="false"/> to prevent the focus from getting set by <see cref="SwitchRowIntoEditMode"/> or <see cref="SwitchListIntoEditMode"/>.
    /// </remarks>
    [Description("Enables automatically setting the focus when switching to edit mode.")]
    [Category("Edit Mode")]
    [DefaultValue(true)]
    public bool EnableAutoFocusOnSwitchToEditMode
    {
      get { return _enableAutoFocusOnSwitchToEditMode; }
      set { _enableAutoFocusOnSwitchToEditMode = value; }
    }

    /// <summary> Is raised before the changes to the editable row are saved. </summary>
    [Category("Action")]
    [Description("Is raised before the changes to the editable row are saved.")]
    public event BocListEditableRowChangesEventHandler EditableRowChangesSaving
    {
      add { Events.AddHandler(s_editableRowChangesSavingEvent, value); }
      remove { Events.RemoveHandler(s_editableRowChangesSavingEvent, value); }
    }

    /// <summary> Is raised after the changes to the editable row have been saved. </summary>
    [Category("Action")]
    [Description("Is raised after the changes to the editable row have been saved.")]
    public event BocListItemEventHandler EditableRowChangesSaved
    {
      add { Events.AddHandler(s_editableRowChangesSavedEvent, value); }
      remove { Events.RemoveHandler(s_editableRowChangesSavedEvent, value); }
    }

    /// <summary> Is raised before the changes to the editable row are canceled. </summary>
    [Category("Action")]
    [Description("Is raised before the changes to the editable row are canceled.")]
    public event BocListEditableRowChangesEventHandler EditableRowChangesCanceling
    {
      add { Events.AddHandler(s_editableRowChangesCancelingEvent, value); }
      remove { Events.RemoveHandler(s_editableRowChangesCancelingEvent, value); }
    }

    /// <summary> Is raised after the changes to the editable row have been canceled. </summary>
    [Category("Action")]
    [Description("Is raised after the changes to the editable row have been canceled.")]
    public event BocListItemEventHandler EditableRowChangesCanceled
    {
      add { Events.AddHandler(s_editableRowChangesCanceledEvent, value); }
      remove { Events.RemoveHandler(s_editableRowChangesCanceledEvent, value); }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="EditableRowDataSourceFactory"/> used to create the data souce for the edit mode
    ///   controls.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EditableRowDataSourceFactory EditModeDataSourceFactory
    {
      get { return _editModeDataSourceFactory; }
      set
      {
        ArgumentUtility.CheckNotNull("value", value);
        _editModeDataSourceFactory = value;
      }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="EditableRowControlFactory"/> used to create the controls for the edit mode.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public EditableRowControlFactory EditModeControlFactory
    {
      get { return _editModeControlFactory; }
      set
      {
        ArgumentUtility.CheckNotNull("value", value);
        _editModeControlFactory = value;
      }
    }

    protected virtual void OnEditableRowChangesSaving (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNull("dataSource", dataSource);
      ArgumentUtility.CheckNotNull("controls", controls);

      BocListEditableRowChangesEventHandler? handler =
          (BocListEditableRowChangesEventHandler?)Events[s_editableRowChangesSavingEvent];
      if (handler != null)
      {
        BocListEditableRowChangesEventArgs e =
            new BocListEditableRowChangesEventArgs(index, businessObject, dataSource, controls);
        handler(this, e);
      }
    }

    protected virtual void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      BocListItemEventHandler? handler = (BocListItemEventHandler?)Events[s_editableRowChangesSavedEvent];
      if (handler != null)
      {
        BocListItemEventArgs e = new BocListItemEventArgs(index, businessObject);
        handler(this, e);
      }
    }

    protected virtual void OnEditableRowChangesCanceling (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNull("dataSource", dataSource);
      ArgumentUtility.CheckNotNull("controls", controls);

      BocListEditableRowChangesEventHandler? handler =
          (BocListEditableRowChangesEventHandler?)Events[s_editableRowChangesCancelingEvent];
      if (handler != null)
      {
        BocListEditableRowChangesEventArgs e =
            new BocListEditableRowChangesEventArgs(index, businessObject, dataSource, controls);
        handler(this, e);
      }
    }

    protected virtual void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      BocListItemEventHandler? handler = (BocListItemEventHandler?)Events[s_editableRowChangesCanceledEvent];
      if (handler != null)
      {
        BocListItemEventArgs e = new BocListItemEventArgs(index, businessObject);
        handler(this, e);
      }
    }

    /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    protected virtual void InsertBusinessObjects (IBusinessObject[] businessObjects)
    {
      AddRows(businessObjects);
    }

    /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    protected virtual void RemoveBusinessObjects (IBusinessObject[] businessObjects)
    {
      RemoveRows(businessObjects);
    }

    private void MenuItemEventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      OnMenuItemEventCommandClick(e.Item);
    }

    /// <summary> Fires the <see cref="MenuItemClick"/> event. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnMenuItemEventCommandClick/*' />
    protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
    {
      ArgumentUtility.CheckNotNull("menuItem", menuItem);

      // Just pro forma. MenuBase already fired Command.Click before click-handler is invoked.
      // OnClick only fires once because of a guard-condition.
      if (menuItem.Command != null)
        menuItem.Command.OnClick();

      if (menuItem is BocMenuItem && menuItem.Command is BocMenuItemCommand)
        ((BocMenuItemCommand)menuItem.Command).OnClick((BocMenuItem)menuItem);

      WebMenuItemClickEventHandler? menuItemClickHandler = (WebMenuItemClickEventHandler?)Events[s_menuItemClickEvent];
      if (menuItemClickHandler != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs(menuItem);
        menuItemClickHandler(this, e);
      }
    }

    private void MenuItemWxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      OnMenuItemWxeFunctionCommandClick(e.Item);
    }

    /// <summary> Handles the click to a WXE function command. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnMenuItemWxeFunctionCommandClick/*' />
    protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
    {
      ArgumentUtility.CheckNotNull("menuItem", menuItem);

      if (menuItem.Command == null)
        return;

      if (menuItem is BocMenuItem)
      {
        BocMenuItemCommand command = (BocMenuItemCommand)menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction((IWxePage)Page, GetSelectedRows(), GetSelectedBusinessObjects());
        //else
        //  command.ExecuteWxeFunction (Page, GetSelectedRows(), GetSelectedBusinessObjects());
      }
      else
      {
        Command command = menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction((IWxePage)Page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }

    bool IBocMenuItemContainer.IsReadOnly
    {
      get { return IsReadOnly; }
    }

    bool IBocMenuItemContainer.IsSelectionEnabled
    {
      get { return IsSelectionEnabled; }
    }

    IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects ()
    {
      return GetSelectedBusinessObjects();
    }

    void IBocMenuItemContainer.InsertBusinessObjects (IBusinessObject[] businessObjects)
    {
      InsertBusinessObjects(businessObjects);
    }

    void IBocMenuItemContainer.RemoveBusinessObjects (IBusinessObject[] businessObjects)
    {
      RemoveBusinessObjects(businessObjects);
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets  (read-only mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    [Category("Appearance")]
    [Description("Determines whether the list headers and the additional column sets will be rendered if no data is provided (read-only mode only).")
    ]
    [DefaultValue(false)]
    public virtual bool ShowEmptyListReadOnlyMode
    {
      get { return _showEmptyListReadOnlyMode; }
      set { _showEmptyListReadOnlyMode = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets (edit mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    [Category("Appearance")]
    [Description("Determines whether the list headers and the additional column sets will be rendered if no data is provided (edit mode only).")]
    [DefaultValue(true)]
    public virtual bool ShowEmptyListEditMode
    {
      get { return _showEmptyListEditMode; }
      set { _showEmptyListEditMode = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus. 
    ///   (read-only mode only).
    /// </summary>
    /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
    [Category("Menu")]
    [Description("Determines whether the options and list menus will be rendered if no data is provided (read-only mode only).")]
    [DefaultValue(false)]
    public virtual bool ShowMenuForEmptyListReadOnlyMode
    {
      get { return _showMenuForEmptyListReadOnlyMode; }
      set { _showMenuForEmptyListReadOnlyMode = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus
    ///   (edit mode only).
    /// </summary>
    /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
    [Category("Menu")]
    [Description("Determines whether the options and list menus will be rendered if no data is provided (edit mode only).")]
    [DefaultValue(true)]
    public virtual bool ShowMenuForEmptyListEditMode
    {
      get { return _showMenuForEmptyListEditMode; }
      set { _showMenuForEmptyListEditMode = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that indicates whether the control automatically generates a column 
    ///   for each property of the bound object.
    /// </summary>
    /// <value> <see langword="true"/> show all properties of the bound business object. </value>
    [Category("Appearance")]
    [Description("Indicates whether the control automatically generates a column for each property of the bound object.")]
    [DefaultValue(false)]
    public virtual bool ShowAllProperties
    {
      get { return _showAllProperties; }
      set { _showAllProperties = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that indicates whether to display an icon in front of the first value 
    ///   column.
    /// </summary>
    /// <value> <see langword="true"/> to enable the icon. </value>
    [Category("Appearance")]
    [Description("Enables the icon in front of the first value column.")]
    [DefaultValue(true)]
    public virtual bool EnableIcon
    {
      get { return _enableIcon; }
      set { _enableIcon = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to to enable cleint side sorting.
    /// </summary>
    /// <value> <see langword="true"/> to enable the sorting buttons. </value>
    [Category("Behavior")]
    [Description("Enables the sorting button in front of each value column's header.")]
    [DefaultValue(true)]
    public virtual bool EnableSorting
    {
      get { return _enableSorting; }
      set { _enableSorting = value; }
    }

    protected bool IsClientSideSortingEnabled
    {
      get { return EnableSorting; }
    }

    bool IBocList.IsClientSideSortingEnabled
    {
      get { return IsClientSideSortingEnabled; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the sorting order index 
    ///   after each sorting button.
    /// </summary>
    /// <remarks> 
    ///   Only displays the index if more than one column is included in the sorting.
    /// </remarks>
    /// <value> 
    ///   <see langword="NaBooleanEnum.True"/> to show the sorting order index after the button. 
    ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
    /// </value>
    [Category("Appearance")]
    [Description("Enables the sorting order display after each sorting button. Undefined is interpreted as true.")]
    [DefaultValue(typeof(bool?), "")]
    public virtual bool? ShowSortingOrder
    {
      get { return _showSortingOrder; }
      set { _showSortingOrder = value; }
    }

    protected virtual bool IsShowSortingOrderEnabled
    {
      get { return ShowSortingOrder != false; }
    }

    bool IBocList.IsShowSortingOrderEnabled
    {
      get { return IsShowSortingOrderEnabled; }
    }

    [Category("Behavior")]
    [Description("Enables sorting by multiple columns. Undefined is interpreted as true.")]
    [DefaultValue(typeof(bool?), "")]
    public virtual bool? EnableMultipleSorting
    {
      get { return _enableMultipleSorting; }
      set
      {
        _enableMultipleSorting = value;
        if (!IsMultipleSortingEnabled)
        {
          var oldCount = _sortingOrder.Length;
          _sortingOrder = _sortingOrder.Where(o => !o.IsEmpty).Take(1).ToArray();
          if (_sortingOrder.Length != oldCount)
            OnSortedRowsChanged();
        }
      }
    }

    protected virtual bool IsMultipleSortingEnabled
    {
      get { return EnableMultipleSorting != false; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the options menu.
    /// </summary>
    /// <value> <see langword="true"/> to show the options menu. </value>
    [Category("Menu")]
    [Description("Enables the options menu.")]
    [DefaultValue(true)]
    public virtual bool ShowOptionsMenu
    {
      get { return _showOptionsMenu; }
      set { _showOptionsMenu = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the list menu.
    /// </summary>
    /// <value> <see langword="true"/> to show the list menu. </value>
    [Category("Menu")]
    [Description("Enables the list menu.")]
    [DefaultValue(true)]
    public virtual bool ShowListMenu
    {
      get { return _showListMenu; }
      set { _showListMenu = value; }
    }

    /// <summary> Gets or sets a value that determines if the row menu is being displayed. </summary>
    /// <value> <see cref="Controls.RowMenuDisplay.Undefined"/> is interpreted as <see cref="Controls.RowMenuDisplay.Disabled"/>. </value>
    [Category("Menu")]
    [Description("Enables the row menu. Undefined is interpreted as Disabled.")]
    [DefaultValue(RowMenuDisplay.Undefined)]
    public RowMenuDisplay RowMenuDisplay
    {
      get { return _rowMenuDisplay; }
      set { _rowMenuDisplay = value; }
    }

    /// <summary>
    ///   Gets or sets a value that indicating the row selection mode.
    /// </summary>
    /// <remarks> 
    ///   If row selection is enabled, the control displays a checkbox in front of each row
    ///   and highlights selected data rows.
    /// </remarks>
    [Category("Behavior")]
    [Description("Indicates whether row selection is enabled.")]
    [DefaultValue(RowSelection.Undefined)]
    public virtual RowSelection Selection
    {
      get { return _selection; }
      set { _selection = value; }
    }

    protected bool IsSelectionEnabled
    {
      get { return _selection != RowSelection.Undefined && _selection != RowSelection.Disabled; }
    }

    /// <summary> Gets or sets a value that indicating the row index is enabled. </summary>
    /// <value> 
    ///   <see langword="RowIndex.InitialOrder"/> to show the of the initial (unsorted) list and
    ///   <see langword="RowIndex.SortedOrder"/> to show the index based on the current sorting order. 
    ///   Defaults to <see cref="RowIndex.Undefined"/>, which is interpreted as <see langword="RowIndex.Disabled"/>.
    /// </value>
    /// <remarks> If row selection is enabled, the control displays an index in front of each row. </remarks>
    [Category("Appearance")]
    [Description("Indicates whether the row index is enabled. Undefined is interpreted as Disabled.")]
    [DefaultValue(RowIndex.Undefined)]
    public virtual RowIndex Index
    {
      get { return _index; }
      set { _index = value; }
    }

    protected bool IsIndexEnabled
    {
      get { return _index != RowIndex.Undefined && _index != RowIndex.Disabled; }
    }

    bool IBocList.IsIndexEnabled
    {
      get { return IsIndexEnabled; }
    }

    /// <summary> Gets or sets the offset for the rendered index. </summary>
    /// <value> Defaults to <see langword="null"/>. </value>
    [Category("Appearance")]
    [Description("The offset for the rendered index.")]
    [DefaultValue(typeof(int?), "")]
    public int? IndexOffset
    {
      get { return _indexOffset; }
      set { _indexOffset = value; }
    }


    /// <summary> Gets or sets the text that is displayed in the index column's title row. </summary>
    [Category("Appearance")]
    [Description("The text that is displayed in the index column's title row.")]
    [DefaultValue(typeof(WebString), "")]
    public WebString IndexColumnTitle
    {
      get { return _indexColumnTitle; }
      set { _indexColumnTitle = value; }
    }

    protected bool AreDataRowsClickSensitive ()
    {
      return HasClientScript
             && ! WcagHelper.Instance.IsWaiConformanceLevelARequired()
             && IsBrowserCapableOfScripting;
    }

    bool IBocList.AreDataRowsClickSensitive ()
    {
      return AreDataRowsClickSensitive();
    }

    /// <summary> The number of rows displayed per page. </summary>
    /// <value> 
    ///   An integer greater than zero to limit the number of rows per page to the specified value,
    ///   or zero, less than zero or <see langword="null"/> to show all rows.
    /// </value>
    [Category("Appearance")]
    [Description("The number of rows displayed per page. Set PageSize to null/0 to show all rows.")]
    [DefaultValue(typeof(int?), "")]
    public virtual int? PageSize
    {
      get { return _pageSize; }
      set
      {
        if (value == null || value.Value < 0)
          _pageSize = null;
        else if (value == 0)
          _pageSize = 0;
        else if (_editModeController.IsListEditModeActive)
          throw new InvalidOperationException("Paging cannot be enabled (i.e. the PageSize cannot be set) when ListEditMode is active.");
        else
          _pageSize = value;
      }
    }

    [MemberNotNullWhen(true, nameof(_pageSize))]
    [MemberNotNullWhen(true, nameof(PageSize))]
    protected bool IsPagingEnabled
    {
      get { return ! WcagHelper.Instance.IsWaiConformanceLevelARequired() && _pageSize != null && _pageSize.Value != 0; }
    }

    bool IBocList.IsPagingEnabled
    {
      get { return IsPagingEnabled; }
    }

    /// <summary>
    ///   Gets or sets a flag that indicates whether to the show the page count even when there 
    ///   is just one page.
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to force showing the page info, even if the rows fit onto a single 
    ///   page.
    /// </value>
    [Category("Behavior")]
    [Description("Indicates whether to the show the page count even when there is just one page.")]
    [DefaultValue(false)]
    public bool AlwaysShowPageInfo
    {
      get { return _alwaysShowPageInfo; }
      set { _alwaysShowPageInfo = value; }
    }

    /// <summary> Gets or sets the text rendered if the list is empty. </summary>
    [Category("Appearance")]
    [Description("The text if the list is empty.")]
    [DefaultValue(typeof(WebString), "")]
    public WebString EmptyListMessage
    {
      get { return _emptyListMessage; }
      set { _emptyListMessage = value; }
    }

    /// <summary> Gets or sets a flag whether to render the <see cref="EmptyListMessage"/>. </summary>
    [Category("Appearance")]
    [Description("A flag that determines whether the EmpryListMessage is rendered.")]
    [DefaultValue(false)]
    public bool ShowEmptyListMessage
    {
      get { return _showEmptyListMessage; }
      set { _showEmptyListMessage = value; }
    }

    /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
    /// <remarks> Effects only advanced scripts used for selcting data rows. </remarks>
    /// <value> <see langref="true"/> to enable the client script. </value>
    [Category("Behavior")]
    [Description(" True to enable the client script for BocList features. ")]
    [DefaultValue(true)]
    public bool EnableClientScript
    {
      get { return _enableClientScript; }
      set { _enableClientScript = value; }
    }

    /// <summary> Is raised when a column type <see cref="BocCustomColumnDefinition"/> is clicked on. </summary>
    [Category("Action")]
    [Description("Occurs when a custom column is clicked on.")]
    public event BocCustomCellClickEventHandler CustomCellClick
    {
      add { Events.AddHandler(s_customCellClickEvent, value); }
      remove { Events.RemoveHandler(s_customCellClickEvent, value); }
    }

    /// <summary> Is raised when a column with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category("Action")]
    [Description("Occurs when a column with a command of type Event is clicked inside an column.")]
    public event BocListItemCommandClickEventHandler ListItemCommandClick
    {
      add { Events.AddHandler(s_listItemCommandClickEvent, value); }
      remove { Events.RemoveHandler(s_listItemCommandClickEvent, value); }
    }

    /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category("Action")]
    [Description("Is raised when a menu item with a command of type Event is clicked.")]
    public event WebMenuItemClickEventHandler MenuItemClick
    {
      add { Events.AddHandler(s_menuItemClickEvent, value); }
      remove { Events.RemoveHandler(s_menuItemClickEvent, value); }
    }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s options menu. </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [Category("Menu")]
    [Description("The menu items displayed by options menu.")]
    [DefaultValue((string?)null)]
    public WebMenuItemCollection OptionsMenuItems
    {
      get { return _optionsMenu.MenuItems; }
    }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s menu area. </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [Category("Menu")]
    [Description("The menu items displayed in the list's menu area.")]
    [DefaultValue((string?)null)]
    public WebMenuItemCollection ListMenuItems
    {
      get { return _listMenu.MenuItems; }
    }

    /// <inheritdoc />
    [Category("Menu")]
    [Description("The minimum width reserved for the menu block.")]
    [DefaultValue(typeof(Unit), "")]
    public Unit MenuBlockMinWidth { get; set; }

    /// <inheritdoc />
    [Category("Menu")]
    [Description("The maximum width reserved for the menu block.")]
    [DefaultValue(typeof(Unit), "")]
    public Unit MenuBlockMaxWidth { get; set; }

    /// <summary> Gets or sets the list of menu items to be hidden. </summary>
    /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
    [Category("Menu")]
    [Description("The list of menu items to be hidden, identified by their ItemIDs.")]
    [DefaultValue((string?)null)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] HiddenMenuItems
    {
      get
      {
        if (_hiddenMenuItems == null)
          return new string[0];
        return _hiddenMenuItems;
      }
      set { _hiddenMenuItems = value; }
    }

    /// <summary>
    ///   Gets or sets a value that indicates whether the control displays a drop down list 
    ///   containing the available column definition sets.
    /// </summary>
    [Category("Menu")]
    [Description("Indicates whether the control displays a drop down list containing the available views.")]
    [DefaultValue(true)]
    public bool ShowAvailableViewsList
    {
      get { return _showAvailableViewsList; }
      set { _showAvailableViewsList = value; }
    }

    /// <summary> Gets or sets the text that is rendered as a title for the drop list of additional columns. </summary>
    [Category("Menu")]
    [Description("The text that is rendered as a title for the list of available views.")]
    [DefaultValue(typeof(WebString), "")]
    public WebString AvailableViewsListTitle
    {
      get { return _availableViewsListTitle; }
      set { _availableViewsListTitle = value; }
    }

    /// <summary> Gets or sets the text that is rendered as a label for the <c>options menu</c>. </summary>
    [Category("Menu")]
    [Description("The text that is rendered as a label for the options menu.")]
    [DefaultValue(typeof(WebString), "")]
    public WebString OptionsTitle
    {
      get { return _optionsTitle; }
      set { _optionsTitle = value; }
    }

    /// <summary> Gets or sets the rendering option for the <c>list menu</c>. </summary>
    [Category("Menu")]
    [Description("Defines how the items will be rendered.")]
    [DefaultValue(ListMenuLineBreaks.All)]
    public ListMenuLineBreaks ListMenuLineBreaks
    {
      get { return _listMenu.LineBreaks; }
      set { _listMenu.LineBreaks = value; }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("Validation message displayed if there is an error.")]
    [Category("Validator")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;
        UpdateValidatorErrorMessages<EditModeValidator>(_errorMessage);
      }
    }

    [Category("Behavior")]
    [DefaultValue("")]
    public string? ControlServicePath
    {
      get { return _controlServicePath; }
      set { _controlServicePath = value ?? string.Empty; }
    }

    [Category("Behavior")]
    [DefaultValue("")]
    [Description("Additional arguments passed to the control service.")]
    public string? ControlServiceArguments
    {
      get { return _controlServiceArguments; }
      set { _controlServiceArguments = StringUtility.EmptyToNull(value); }
    }

    bool IBocList.HasClientScript
    {
      get { return HasClientScript; }
    }

    protected bool HasClientScript
    {
      get { return EnableClientScript; }
    }

    DropDownList? IBocList.GetAvailableViewsList ()
    {
      return _availableViewsListPlaceHolder.Controls.OfType<DropDownList>().SingleOrDefault();
    }

    IDropDownMenu IBocList.OptionsMenu
    {
      get
      {
        if (OptionsTitle.IsEmpty)
          _optionsMenu.TitleText = GetResourceManager().GetText(ResourceIdentifier.OptionsTitle);
        else
          _optionsMenu.TitleText = OptionsTitle;

        _optionsMenu.Enabled = !_editModeController.IsRowEditModeActive;
        _optionsMenu.IsReadOnly = IsReadOnly;

        return _optionsMenu;
      }
    }

    /// <inheritdoc />
    [Category("Menu")]
    [Description("The text that is rendered as a label for the menu.")]
    [DefaultValue(typeof(WebString), "")]
    public WebString ListMenuHeading
    {
      get => _listMenuHeading;
      set => _listMenuHeading = value;
    }

    /// <inheritdoc />
    [Category("Menu")]
    [Description("The heading level applied to the ListMenuHeading. Leave empty to use a SPAN instead of a Heading-element.")]
    [DefaultValue(typeof(HeadingLevel?), "")]
    public HeadingLevel? ListMenuHeadingLevel
    {
      get => _listMenu.HeadingLevel;
      set => _listMenu.HeadingLevel = value;
    }

    IListMenu IBocList.ListMenu
    {
      get
      {
        if (ListMenuHeading.IsEmpty)
          _listMenu.Heading = GetResourceManager().GetText(ResourceIdentifier.ListMenuHeading);
        else
          _listMenu.Heading = ListMenuHeading;

        return _listMenu;
      }
    }

    IEditModeController IBocList.EditModeController
    {
      get { return _editModeController; }
    }

    ReadOnlyCollection<DropDownMenu> IBocList.RowMenus
    {
      get { return new ReadOnlyCollection<DropDownMenu>(_rowMenus.ToArray()); }
    }

    ReadOnlyDictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]> IBocList.CustomColumns
    {
      get { return new ReadOnlyDictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]>(_customColumnControls); }
    }

    IEnumerable<PlainTextString> IBocList.GetValidationErrors ()
    {
      return GetRegisteredValidators()
          .Where(v => !v.IsValid)
          .Select(v => v.ErrorMessage)
          .Select(PlainTextString.CreateFromText)
          .Distinct();
    }

    IEnumerable<string> IControlWithLabel.GetLabelIDs ()
    {
      return GetLabelIDs();
    }

    public string GetSelectorControlValue (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);

      return RowIDProvider.GetItemRowID(row);
    }

    string IBocList.GetSelectorControlName ()
    {
      return ClientID + c_rowSelectorPostfix;
    }

    string IBocList.GetSelectAllControlName ()
    {
      return ClientID + c_allRowsSelectorPostfix;
    }

    string IBocList.GetSelectionChangedHandlerScript ()
    {
      return GetSelectionChangedHandlerScript();
    }

    /// <summary>
    /// Returns a an anonymous Javascript function with the signature <c>function (bocList) {...}</c>. 
    /// It is invoked when the <see cref="BocList"/> is initialized on the client and when the selected rows change.
    /// </summary>
    /// <remarks>
    /// One way to extend the Javascript is by wrapping the function returned by the base-call, e.g.
    /// <code>
    ///protected override string GetSelectionChangedHandlerScript ()
    ///{
    ///  var baseScript = base.GetSelectionChangedHandlerScript ();
    ///  var extensionScript = "alert (bocList.id);";
    ///  return string.Format ("function (bocList, isInitializing) {{ var base = {0}; base (bocList, isInitializing); {1} }}", baseScript, extensionScript);
    ///}
    /// </code>
    /// </remarks>
    protected virtual string GetSelectionChangedHandlerScript ()
    {
      if (!HasListMenu)
        return "function(){{}}";

      Assertion.IsTrue(_listMenu.Visible, "BocList '{0}': The ListMenu must remain visible if BocList.HasListMenu is evaluates 'true'.", ID);

      return string.Format("function(bocList, isInitializing) {{ {0} }}", _listMenu.GetUpdateScriptReference(GetSelectionCountScript()));
    }

    [PublicAPI]
    protected string GetSelectionCountScript ()
    {
      return "function() { return BocList.GetSelectionCount ('" + ClientID + "'); }";
    }

    private IRowIDProvider RowIDProvider
    {
      get { return _rowIDProvider; }
    }

    private void InitializeRowIDProvider ()
    {
      if (Value == null)
        _rowIDProvider = new NullValueRowIDProvider();
      else if (GetBusinessObjectClass() is IBusinessObjectClassWithIdentity)
        _rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      else
        _rowIDProvider = new IndexBasedRowIDProvider(Value);
    }

    protected IBusinessObjectClass? GetBusinessObjectClass ()
    {
      if (Property != null)
        return Property.ReferenceClass;
      else if (DataSource != null)
        return DataSource.BusinessObjectClass;
      return null;
    }

    private void OnColumnsChanged ()
    {
      CheckPreRenderHasNotCompleted();

      _columnDefinitions = null;
      OnSortedRowsChanged();
    }

    private void OnSortedRowsChanged ()
    {
      CheckPreRenderHasNotCompleted();

      _indexedRowsSorted = null;
      _currentPageRows = null;
      OnStateOfDisplayedRowsChanged();
    }

    private void OnDisplayedRowsChanged ()
    {
      CheckPreRenderHasNotCompleted();

      ClearSelectedRows();
      _currentPageRows = null;
      OnStateOfDisplayedRowsChanged();
    }

    private void OnStateOfDisplayedRowsChanged ()
    {
      CheckPreRenderHasNotCompleted();

      ResetCustomColumns();
      ResetRowMenus();
    }

    private void SetPreRenderComplete ()
    {
      _hasPreRenderCompleted = true;
    }

    private void CheckPreRenderHasNotCompleted ()
    {
      if (_hasPreRenderCompleted)
      {
        throw new InvalidOperationException(
            string.Format("Cannot perform the requested operation on BocList '{0}' because the PreRender phase has already completed.", ID));
      }
    }

    protected int CurrentPageIndex
    {
      get { return _currentPageIndex; }
    }

    int IBocList.CurrentPageIndex
    {
      get { return CurrentPageIndex; }
    }

    int IBocList.PageCount
    {
      get { return _pageCount; }
    }

    string IBocList.GetCurrentPageControlName ()
    {
      Assertion.DebugIsNotNull(_currentPagePostBackTarget, "_currentPagePostBackTarget must not be null.");

      return _currentPagePostBackTarget.UniqueID;
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "BocList"; }
    }
  }

  public enum RowSelection
  {
    Undefined = -1,
    Disabled = 0,
    SingleCheckBox = 1,
    SingleRadioButton = 2,
    Multiple = 3
  }

  public enum RowIndex
  {
    Undefined,
    Disabled,
    InitialOrder,
    SortedOrder
  }

  public enum RowMenuDisplay
  {
    Undefined,
    /// <summary> No menus will be shown, even if a <see cref="BocDropDownMenuColumnDefinition"/> has been created. </summary>
    Disabled,
    /// <summary> The developer must manually provide a <see cref="BocDropDownMenuColumnDefinition"/>. </summary>
    Manual,
    /// <summary> The <see cref="BocList"/> will automatically create a <see cref="BocDropDownMenuColumnDefinition"/>. </summary>
    Automatic
  }
}

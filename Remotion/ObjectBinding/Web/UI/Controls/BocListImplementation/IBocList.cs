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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.ListMenuImplementation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  public interface IBocList
      : IBusinessObjectBoundEditableWebControl,
        IBocRenderableControl,
        IBocMenuItemContainer,
        IControlWithResourceManager

  {
    new bool IsReadOnly { get; }

    new IReadOnlyList<IBusinessObject>? Value { get; }

    bool HasNavigator { get; }

    /// <summary>
    ///   Gets a flag set <see langword="true"/> if the <see cref="IBusinessObjectBoundControl.Value"/> is sorted before it is displayed.
    /// </summary>
    bool HasSortingKeys { get; }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets  (read-only mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    bool ShowEmptyListReadOnlyMode { get; }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets (edit mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    bool ShowEmptyListEditMode { get; }

    /// <summary> Gets or sets a value that determines if the row menu is being displayed. </summary>
    /// <value> <see cref="Controls.RowMenuDisplay.Undefined"/> is interpreted as <see cref="Controls.RowMenuDisplay.Disabled"/>. </value>
    RowMenuDisplay RowMenuDisplay { get; }

    bool IsIndexEnabled { get; }

    /// <summary> Gets or sets a value that indicating the row index is enabled. </summary>
    /// <value> 
    ///   <see langword="RowIndex.InitialOrder"/> to show the of the initial (unsorted) list and
    ///   <see langword="RowIndex.SortedOrder"/> to show the index based on the current sorting order. 
    ///   Defaults to <see cref="RowIndex.Undefined"/>, which is interpreted as <see langword="RowIndex.Disabled"/>.
    /// </value>
    /// <remarks> If row selection is enabled, the control displays an index in front of each row. </remarks>
    RowIndex Index { get; }

    /// <summary> Gets or sets the offset for the rendered index. </summary>
    /// <value> Defaults to <see langword="null"/>. </value>
    int? IndexOffset { get; }

    /// <summary> Gets or sets the text that is displayed in the index column's title row. </summary>
    WebString IndexColumnTitle { get; }

    /// <summary> Gets or sets the text rendered if the list is empty. </summary>
    WebString EmptyListMessage { get; }

    /// <summary> Gets or sets a flag whether to render the <see cref="BocList.EmptyListMessage"/>. </summary>
    bool ShowEmptyListMessage { get; }

    /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
    /// <remarks> Effects only advanced scripts used for selcting data rows. </remarks>
    /// <value> <see langref="true"/> to enable the client script. </value>
    bool EnableClientScript { get; }

    /// <summary>
    /// Gets or sets the minimum width reserved for the menu block.
    /// </summary>
    Unit MenuBlockMinWidth { get; }

    /// <summary>
    /// Gets or sets the maximum width reserved for the menu block.
    /// </summary>
    Unit MenuBlockMaxWidth { get; }

    /// <summary> Gets or sets the text that is rendered as a title for the drop list of additional columns. </summary>
    WebString AvailableViewsListTitle { get; }

    /// <summary> Gets or sets the text that is rendered as a label for the <c>options menu</c>. </summary>
    WebString OptionsTitle { get; }

    /// <summary>
    /// Gets the heading used to label the list menu.
    /// </summary>
    WebString ListMenuHeading { get; }

    /// <summary>
    /// Gets the heading level used to render the list menu heading. <see langword="null"/> renders a <c>span</c>.
    /// </summary>
    HeadingLevel? ListMenuHeadingLevel { get; }

    bool HasClientScript { get; }
    DropDownList? GetAvailableViewsList ();
    IDropDownMenu OptionsMenu { get; }

    IEditModeController EditModeController { get; }
    ReadOnlyCollection<DropDownMenu> RowMenus { get; }
    ReadOnlyDictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]> CustomColumns { get; }
    bool HasListMenu { get; }
    bool IsClientSideSortingEnabled { get; }
    bool HasOptionsMenu { get; }
    bool HasAvailableViewsList { get; }
    bool HasMenuBlock { get; }
    bool IsShowSortingOrderEnabled { get; }
    IListMenu ListMenu { get; }

    /// <summary>
    ///   Obtains a reference to a client-side script function that causes, when invoked, a server post back to the form.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     If the <see cref="BocList"/> is in row edit mode, <c>return false;</c> will be returned to prevent actions on 
    ///     this list.
    ///   </para><para>
    ///     Insert the return value in the rendered onClick event.
    ///   </para>
    /// </remarks>
    /// <param name="columnIndex"> The index of the column for which the post back function should be created. </param>
    /// <param name="row"> The <see cref="BocListRow"/> for which the post back function should be created. </param>
    /// <param name="customCellArgument"> 
    ///   The argument to be passed to the <see cref="BocCustomColumnDefinitionCell"/>'s <c>OnClick</c> method.
    ///   Can be <see langword="null"/>.
    /// </param>
    /// <returns></returns>
    string GetCustomCellPostBackClientEvent (int columnIndex, BocListRow row, string customCellArgument);

    /// <summary>
    ///   Registers the command used in <see cref="BocCustomColumnDefinition"/> for a synchronous post back.
    /// </summary>
    /// <param name="columnIndex"> The index of the column for which the synchronous post back shoud be registered. </param>
    /// <param name="row"> The <see cref="BocListRow"/> for which the synchronous post back shoud be registered. </param>
    /// <param name="customCellArgument"> 
    ///   The argument for which the synchronous post back shoud be registered.
    ///   Can be <see langword="null"/>.
    /// </param>
    void RegisterCustomCellForSynchronousPostBack (int columnIndex, BocListRow row, string customCellArgument);

    string GetListItemCommandArgument (int columnIndex, BocListRow row);
    string GetRowEditCommandArgument (BocListRow row, BocList.RowEditModeCommand command);
    BocListRowRenderingContext[] GetRowsToRender ();
    void OnDataRowRendering (BocListDataRowRenderEventArgs args);

    RowSelection Selection { get; }
    bool AreDataRowsClickSensitive ();
    string GetSelectorControlName ();
    string GetSelectorControlValue (BocListRow row);
    string GetSelectAllControlName ();
    string GetSelectionChangedHandlerScript ();

    bool IsPagingEnabled { get; }
    int PageCount { get; }
    int? PageSize { get; }
    int CurrentPageIndex { get; }
    string GetCurrentPageControlName ();

    bool IsInlineValidationDisplayEnabled { get; }

    void BuildErrorMessages ();

    /// <summary>
    /// Gets the <see cref="ValidationFailureRepository"/> belonging to this <see cref="BocList"/>.
    /// </summary>
    IBocListValidationFailureRepository ValidationFailureRepository { get; }

    /// <summary>
    /// Returns the <see cref="BocColumnDefinition"/>s displayed in the <see cref="IBocList"/>.
    /// </summary>
    IReadOnlyList<BocColumnDefinition> GetColumnDefinitions ();

    /// <summary>
    /// Gets the list of validation errors for this control.
    /// </summary>
    IEnumerable<PlainTextString> GetValidationErrors ();

    /// <summary> Gets the <see cref="IBusinessObject"/>s relevant for validation. This list can be a superset of the <see cref="Value" /> property.</summary>
    IReadOnlyList<IBusinessObject>? GetBusinessObjectsForValidation ();

    string? ControlServicePath { get; }
  }
}

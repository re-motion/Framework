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
using System.Linq;
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocColumnRendererArrayBuilder"/> is responsible for creating a collection of <see cref="BocColumnRenderer"/>s for the given
  /// <see cref="BocColumnDefinition"/>s. It exposes several properties which must be set to ensure that the right column renderers
  /// are created and the correct sorting information gets prepared.
  /// </summary>
  public class BocColumnRendererArrayBuilder
  {
    private readonly IReadOnlyList<BocColumnDefinition> _columnDefinitions;
    private readonly IServiceLocator _serviceLocator;
    private readonly WcagHelper _wcagHelper;

    public BocColumnRendererArrayBuilder (IReadOnlyList<BocColumnDefinition> columnDefinitions, IServiceLocator serviceLocator, WcagHelper wcagHelper)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnDefinitions", columnDefinitions);
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);
      ArgumentUtility.CheckNotNull("wcagHelper", wcagHelper);

      _columnDefinitions = columnDefinitions;
      _serviceLocator = serviceLocator;
      _wcagHelper = wcagHelper;
    }

    public bool EnableIcon { get; set; }
    public bool IsListReadOnly { get; set; }
    public bool IsListEditModeActive { get; set; }
    public bool IsBrowserCapableOfScripting { get; set; }
    public bool IsClientSideSortingEnabled { get; set; }
    public bool HasSortingKeys { get; set; }
    public bool IsIndexEnabled { get; set; }
    public bool IsSelectionEnabled { get; set; }
    public ICollection<BocListSortingOrderEntry>? SortingOrder { get; set; }

    public BocColumnRenderer[] CreateColumnRenderers ()
    {
      var sortingDirections = new Dictionary<int, SortingDirection>();
      var sortingOrder = new List<int>();

      PrepareSorting(sortingDirections, sortingOrder);

      var bocColumnRenderers = new List<BocColumnRenderer>(_columnDefinitions.Count);
      var visibleColumnIndex = GetInitialVisibleColumnIndex();

      BocColumnDefinition? iconColumn = null;
      if (EnableIcon)
      {
        iconColumn = _columnDefinitions.FirstOrDefault(cd => cd is IBocColumnDefinitionWithRowIconSupport { RowIconMode: RowIconMode.Preferred })
                     ?? _columnDefinitions.FirstOrDefault(cd => cd is IBocColumnDefinitionWithRowIconSupport { RowIconMode: RowIconMode.Automatic });
      }

      for (int columnIndex = 0; columnIndex < _columnDefinitions.Count; columnIndex++)
      {
        var columnDefinition = _columnDefinitions[columnIndex];
        var isRowHeader = (columnDefinition as IBocColumnDefinitionWithRowHeaderSupport)?.IsRowHeader ?? false;
        var showIcon = ReferenceEquals(columnDefinition, iconColumn);

        var sortingDirection = SortingDirection.None;
        if (sortingDirections.ContainsKey(columnIndex))
          sortingDirection = sortingDirections[columnIndex];
        var orderIndex = sortingOrder.IndexOf(columnIndex);

        if (IsColumnVisible(columnDefinition))
        {
          var columnRenderer = columnDefinition.GetRenderer(_serviceLocator);
          bocColumnRenderers.Add(
              new BocColumnRenderer(
                  columnRenderer,
                  columnDefinition,
                  columnIndex,
                  visibleColumnIndex,
                  isRowHeader: isRowHeader,
                  showIcon: showIcon,
                  sortingDirection,
                  orderIndex));

          visibleColumnIndex++;
        }
        else
        {
          bocColumnRenderers.Add(
              new BocColumnRenderer(
                  new NullColumnRenderer(),
                  columnDefinition,
                  columnIndex,
                  -1,
                  isRowHeader: isRowHeader,
                  showIcon: false,
                  sortingDirection,
                  orderIndex));
        }
      }
      return bocColumnRenderers.ToArray();
    }

    private void PrepareSorting (IDictionary<int, SortingDirection> sortingDirections, IList<int> sortingOrder)
    {
      if (IsClientSideSortingEnabled || HasSortingKeys)
      {
        Assertion.IsNotNull(SortingOrder, "SortingOrder must not be null.");

        foreach (var entry in SortingOrder)
        {
          var columnIndex = _columnDefinitions.IndexOf((BocColumnDefinition?)entry.Column);
          if (entry.Direction != SortingDirection.None)
          {
            sortingDirections.Add(columnIndex, entry.Direction);
            sortingOrder.Add(columnIndex);
          }
        }
      }
    }

    private int GetInitialVisibleColumnIndex ()
    {
      var visibleColumnIndex = 0;

      if (IsIndexEnabled)
        ++visibleColumnIndex;
      if (IsSelectionEnabled)
        ++visibleColumnIndex;

      return visibleColumnIndex;
    }

    private bool IsColumnVisible (BocColumnDefinition column)
    {
      ArgumentUtility.CheckNotNull("column", column);

      var columnAsCommandColumn = column as BocCommandColumnDefinition;
      if (columnAsCommandColumn != null && columnAsCommandColumn.Command != null)
      {
        if (!IsColumnVisibleForBocCommandColumnDefinition(columnAsCommandColumn))
          return false;
      }

      var columnAsRowEditModeColumn = column as BocRowEditModeColumnDefinition;
      if (columnAsRowEditModeColumn != null)
      {
        if (!IsColumnVisibleForBocRowEditModeColumnDefinition(columnAsRowEditModeColumn))
          return false;
      }

      var columnAsDropDownMenuColumn = column as BocDropDownMenuColumnDefinition;
      if (columnAsDropDownMenuColumn != null)
      {
        if (!IsColumnVisibleForBocDropDownMenuColumnDefinition(columnAsDropDownMenuColumn))
          return false;
      }

      var columnAsValidationErrorIndicatorColumn = column as BocValidationErrorIndicatorColumnDefinition;
      if (columnAsValidationErrorIndicatorColumn != null)
      {
        if (!IsColumnVisibleForBocValidationErrorIndicatorColumnDefinition(columnAsValidationErrorIndicatorColumn))
          return false;
      }

      return true;
    }

    private bool IsColumnVisibleForBocValidationErrorIndicatorColumnDefinition (BocValidationErrorIndicatorColumnDefinition columnAsValidationErrorIndicatorColumn)
    {
      var bocList = (IBocList?)columnAsValidationErrorIndicatorColumn.OwnerControl;
      if (bocList == null)
        return true;

      var repository = bocList.ValidationFailureRepository;
      return columnAsValidationErrorIndicatorColumn.Visibility switch
      {
        BocValidationErrorIndicatorColumnDefinitionVisibility.Always => true,
        BocValidationErrorIndicatorColumnDefinitionVisibility.AnyValidationFailure => (repository.GetListFailureCount() + repository.GetRowAndCellFailureCount()) > 0,
        BocValidationErrorIndicatorColumnDefinitionVisibility.AnyRowOrCellValidationFailure => repository.GetRowAndCellFailureCount() > 0,
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    private bool IsColumnVisibleForBocCommandColumnDefinition (BocCommandColumnDefinition columnAsCommandColumn)
    {
      if (_wcagHelper.IsWaiConformanceLevelARequired()
          && columnAsCommandColumn.Command != null
          && (columnAsCommandColumn.Command.Type == CommandType.Event || columnAsCommandColumn.Command.Type == CommandType.WxeFunction))
        return false;
      return true;
    }

    private bool IsColumnVisibleForBocRowEditModeColumnDefinition (BocRowEditModeColumnDefinition column)
    {
      if (_wcagHelper.IsWaiConformanceLevelARequired())
        return false;
      if (column.Show == BocRowEditColumnDefinitionShow.EditMode && IsListReadOnly)
        return false;
      if (IsListEditModeActive)
        return false;
      return true;
    }

    private bool IsColumnVisibleForBocDropDownMenuColumnDefinition (BocDropDownMenuColumnDefinition column)
    {
      if (_wcagHelper.IsWaiConformanceLevelARequired())
        return false;
      return IsBrowserCapableOfScripting;
    }

  }
}

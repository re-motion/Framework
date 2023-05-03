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
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  /// <summary>
  /// Repository which contains all <see cref="BusinessObjectValidationFailure"/> belonging to a BocList.
  /// </summary>
  public class BocListValidationFailureRepository : IBocListValidationFailureRepository
  {
    private class HandleableFailure
    {
      public BusinessObjectValidationFailure Failure { get; }

      public bool Handled { get; private set; }

      public HandleableFailure (BusinessObjectValidationFailure failure)
      {
        Failure = failure;
        Handled = false;
      }

      public void MarkAsHandled ()
      {
        Handled = true;
      }
    }

    private class RowFailureContainer
    {
      private readonly List<HandleableFailure> _rowFailures = new();
      private readonly Dictionary<IBocColumnDefinitionWithValidationSupport, List<HandleableFailure>> _cellFailures = new();

      public void AddRowFailures (IEnumerable<BusinessObjectValidationFailure> failures)
      {
        _rowFailures.AddRange(failures.Select(e => new HandleableFailure(e)));
      }

      public void AddCellFailures (IBocColumnDefinitionWithValidationSupport columnDefinition, IEnumerable<BusinessObjectValidationFailure> failures)
      {
        List<HandleableFailure>? cellFailureForRow;
        if (!_cellFailures.TryGetValue(columnDefinition, out cellFailureForRow))
        {
          cellFailureForRow = new List<HandleableFailure>();
          _cellFailures.Add(columnDefinition, cellFailureForRow);
        }

        cellFailureForRow.AddRange(failures.Select(e => new HandleableFailure(e)));
      }

      public IEnumerable<HandleableFailure> GetValidationFailuresForDataRow ()
      {
        return _rowFailures;
      }

      public IEnumerable<HandleableFailure> GetValidationFailuresForDataCell (IBocColumnDefinitionWithValidationSupport columnDefinition)
      {
        return _cellFailures.GetValueOrDefault(columnDefinition, new List<HandleableFailure>());
      }

      public bool HasValidationFailures ()
      {
        return _rowFailures is { Count: > 0 } || _cellFailures.Count > 0 && _cellFailures.Values.SelectMany(e => e).Any();
      }

      public IEnumerable<HandleableFailure> GetAllCellFailuresForRow ()
      {
        return _cellFailures.SelectMany(c => c.Value).ToList();
      }
    }

    private readonly List<HandleableFailure> _listValidationFailures = new();
    private readonly Dictionary<IBusinessObject, RowFailureContainer> _rowValidationFailures = new();

    public BocListValidationFailureRepository ()
    {
    }

    public void AddValidationFailuresForBocList (IEnumerable<BusinessObjectValidationFailure> validationFailures)
    {
      ArgumentUtility.CheckNotNull(nameof(validationFailures), validationFailures);

      _listValidationFailures.AddRange(validationFailures.Select(e => new HandleableFailure(e)));
    }

    public void AddValidationFailuresForDataRow (IBusinessObject rowObject, IEnumerable<BusinessObjectValidationFailure> validationFailures)
    {
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);
      ArgumentUtility.CheckNotNull(nameof(validationFailures), validationFailures);

      var failureContainer = GetOrCreateRowFailureContainer(rowObject);
      failureContainer.AddRowFailures(validationFailures);
    }

    public void AddValidationFailuresForDataCell (
        IBusinessObject rowObject,
        IBocColumnDefinitionWithValidationSupport columnDefinition,
        IEnumerable<BusinessObjectValidationFailure> validationFailures)
    {
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);
      ArgumentUtility.CheckNotNull(nameof(columnDefinition), columnDefinition);
      ArgumentUtility.CheckNotNull(nameof(validationFailures), validationFailures);

      var failureContainer = GetOrCreateRowFailureContainer(rowObject);
      failureContainer.AddCellFailures(columnDefinition, validationFailures);
    }


    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForBocList (bool markAsHandled)
    {
      var failures = _listValidationFailures.Where(e => !e.Handled);

      if (markAsHandled)
        failures = failures.ApplySideEffect(f => f.MarkAsHandled());

      return failures.Select(f => f.Failure).ToList();
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells (bool markAsHandled)
    {
      var failures = _listValidationFailures
          .Concat(
              _rowValidationFailures.Values.SelectMany(
                  row => row
                      .GetValidationFailuresForDataRow()
                      .Concat(row.GetAllCellFailuresForRow())))
          .Where(f => !f.Handled);

      if (markAsHandled)
        failures = failures.ApplySideEffect(f => f.MarkAsHandled());

      return failures.Select(f => f.Failure).ToList();
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForDataRow (IBusinessObject rowObject, bool markAsHandled)
    {
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);

      var failureContainer = _rowValidationFailures.GetValueOrDefault(rowObject);
      if (failureContainer == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var failures = failureContainer.GetValidationFailuresForDataRow().Where(f => !f.Handled);

      if (markAsHandled)
        failures = failures.ApplySideEffect(f => f.MarkAsHandled());

      return failures.Select(f => f.Failure).ToList();
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForDataRowAndContainingDataCells (IBusinessObject rowObject, bool markAsHandled)
    {
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);

      var failureContainer = _rowValidationFailures.GetValueOrDefault(rowObject);
      if (failureContainer == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var failures = failureContainer
          .GetValidationFailuresForDataRow()
          .Concat(failureContainer.GetAllCellFailuresForRow())
          .Where(f => !f.Handled);

      if (markAsHandled)
        failures = failures.ApplySideEffect(f => f.MarkAsHandled());

      return failures.Select(f => f.Failure).ToList();
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForDataCell (
        IBusinessObject rowObject,
        IBocColumnDefinitionWithValidationSupport columnDefinition,
        bool markAsHandled)
    {
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);
      ArgumentUtility.CheckNotNull(nameof(columnDefinition), columnDefinition);

      var failureContainer = _rowValidationFailures.GetValueOrDefault(rowObject);
      if (failureContainer == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var cellFailures = failureContainer.GetValidationFailuresForDataCell(columnDefinition).Where(f => !f.Handled);

      if (markAsHandled)
        cellFailures = cellFailures.ApplySideEffect(f => f.MarkAsHandled());

      return cellFailures.Select(f => f.Failure).ToList();
    }

    public bool HasValidationFailuresForDataRow (IBusinessObject rowObject)
    {
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);

      return _rowValidationFailures.GetValueOrDefault(rowObject)?.HasValidationFailures() ?? false;
    }

    public void ClearAllValidationFailures ()
    {
      _rowValidationFailures.Clear();
      _listValidationFailures.Clear();
    }

    private RowFailureContainer GetOrCreateRowFailureContainer (IBusinessObject rowObject)
    {
      if (_rowValidationFailures.TryGetValue(rowObject, out var existingFailureContainer))
        return existingFailureContainer;

      var newFailureContainer = new RowFailureContainer();
      _rowValidationFailures.Add(rowObject, newFailureContainer);

      return newFailureContainer;
    }
  }
}

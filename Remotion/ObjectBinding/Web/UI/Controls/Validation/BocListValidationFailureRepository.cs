using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.ObjectBinding.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public class BocListValidationFailureRepository
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
      private List<HandleableFailure>? _rowFailures;
      private Dictionary<BocColumnDefinition, List<HandleableFailure>>? _columnFailures;

      public IBusinessObject RowObject { get; }

      public RowFailureContainer (IBusinessObject rowObject)
      {
        RowObject = rowObject;
      }

      public void AddRowFailures (IEnumerable<BusinessObjectValidationFailure> failures)
      {
        _rowFailures ??= new List<HandleableFailure>();
        _rowFailures.AddRange(failures.Select(e => new HandleableFailure(e)));
      }

      public void AddColumnFailures (BocColumnDefinition columnDefinition, IEnumerable<BusinessObjectValidationFailure> failures)
      {
        var columnFailures = GetOrCreateListOfColumnFailures(columnDefinition);
        columnFailures.AddRange(failures.Select(e => new HandleableFailure(e)));
      }

      public void GetAllFailures (ICollection<BusinessObjectValidationFailure> failures, bool markAsHandled)
      {
        GetRowFailures(failures, markAsHandled);

        if (_columnFailures != null)
        {
          foreach (var handleableRowFailure in _columnFailures.Values.SelectMany(e => e).Where(e => !e.Handled))
          {
            failures.Add(handleableRowFailure.Failure);
            if (markAsHandled)
              handleableRowFailure.MarkAsHandled();
          }
        }
      }

      public void GetRowFailures (ICollection<BusinessObjectValidationFailure> failures, bool markAsHandled)
      {
        if (_rowFailures == null)
          return;

        foreach (var handleableRowFailure in _rowFailures.Where(e => !e.Handled))
        {
          failures.Add(handleableRowFailure.Failure);
          if (markAsHandled)
            handleableRowFailure.MarkAsHandled();
        }
      }

      public void GetColumnFailures (ICollection<BusinessObjectValidationFailure> failures, BocColumnDefinition columnDefinition, bool markAsHandled)
      {
        if (_columnFailures == null)
          return;

        if (!_columnFailures.TryGetValue(columnDefinition, out var handleableFailures))
          return;

        foreach (var handleableRowFailure in handleableFailures.Where(e => !e.Handled))
        {
          failures.Add(handleableRowFailure.Failure);
          if (markAsHandled)
            handleableRowFailure.MarkAsHandled();
        }
      }

      private List<HandleableFailure> GetOrCreateListOfColumnFailures (BocColumnDefinition columnDefinition)
      {
        _columnFailures ??= new Dictionary<BocColumnDefinition, List<HandleableFailure>>();
        if (_columnFailures.TryGetValue(columnDefinition, out var existingFailureList))
          return existingFailureList;

        var newFailureList = new List<HandleableFailure>();
        _columnFailures[columnDefinition] = newFailureList;
        return newFailureList;
      }

      public bool HasValidationFailures ()
      {
        return _rowFailures is { Count: > 0 } || (_columnFailures != null && _columnFailures.Values.SelectMany(e => e).Any());
      }
    }

    private List<HandleableFailure>? _listValidationFailures;
    private Dictionary<IBusinessObject, RowFailureContainer>? _rowValidationFailures;

    public BocListValidationFailureRepository ()
    {
    }

    public void AddListValidationFailures (IEnumerable<BusinessObjectValidationFailure> validationFailures)
    {
      _listValidationFailures ??= new List<HandleableFailure>();
      _listValidationFailures.AddRange(validationFailures.Select(e => new HandleableFailure(e)));
    }

    public void AddRowValidationFailures (IBusinessObject rowObject, IEnumerable<BusinessObjectValidationFailure> validationFailures)
    {
      var failureContainer = GetOrCreateRowFailureContainer(rowObject);
      failureContainer.AddRowFailures(validationFailures);
    }

    public void AddRowValidationFailures (IBusinessObject rowObject, BocColumnDefinition columnDefinition, IEnumerable<BusinessObjectValidationFailure> validationFailures)
    {
      var failures = GetOrCreateRowFailureContainer(rowObject);
      failures.AddColumnFailures(columnDefinition, validationFailures);
    }

    public void Clear ()
    {
      _listValidationFailures = null;
      _rowValidationFailures = null;
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetListValidationFailures (bool markAsHandled)
    {
      if (_listValidationFailures == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var listFailures = new List<BusinessObjectValidationFailure>();
      foreach (var handleableRowFailure in _listValidationFailures.Where(e => !e.Handled))
      {
        listFailures.Add(handleableRowFailure.Failure);
        if (markAsHandled)
          handleableRowFailure.MarkAsHandled();
      }

      return listFailures;
    }

    public bool HasValidationFailures (IBusinessObject rowObject)
    {
      return GetRowFailureContainer(rowObject)?.HasValidationFailures() ?? false;
    }

  public IReadOnlyCollection<BusinessObjectValidationFailure> GetAllValidationFailures (IBusinessObject rowObject, bool markAsHandled)
    {
      var failureContainer = GetRowFailureContainer(rowObject);
      if (failureContainer == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var failures = new List<BusinessObjectValidationFailure>();
      failureContainer.GetAllFailures(failures, markAsHandled);

      return failures;
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetRowValidationFailures (IBusinessObject rowObject, bool markAsHandled)
    {
      var failureContainer = GetRowFailureContainer(rowObject);
      if (failureContainer == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var failures = new List<BusinessObjectValidationFailure>();
      failureContainer.GetRowFailures(failures, markAsHandled);

      return failures;
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetCellValidationFailures (IBusinessObject rowObject, BocColumnDefinition columnDefinition, bool markAsHandled)
    {
      var failureContainer = GetRowFailureContainer(rowObject);
      if (failureContainer == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var failures = new List<BusinessObjectValidationFailure>();
      failureContainer.GetColumnFailures(failures, columnDefinition, markAsHandled);

      return failures;
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailures ()
    {
      var failures = new List<BusinessObjectValidationFailure>();

      if (_listValidationFailures != null)
        failures.AddRange(_listValidationFailures.Where(e => !e.Handled).Select(e => e.Failure));

      if (_rowValidationFailures != null)
      {
        foreach (var failureContainer in _rowValidationFailures.Values)
          failureContainer.GetAllFailures(failures, false);
      }

      return failures;
    }

    private RowFailureContainer? GetRowFailureContainer (IBusinessObject rowObject)
    {
      if (_rowValidationFailures == null)
        return null;

      return _rowValidationFailures.TryGetValue(rowObject, out var failureContainer)
          ? failureContainer
          : null;
    }

    private RowFailureContainer GetOrCreateRowFailureContainer (IBusinessObject rowObject)
    {
      _rowValidationFailures ??= new Dictionary<IBusinessObject, RowFailureContainer>();
      if (_rowValidationFailures.TryGetValue(rowObject, out var existingFailureContainer))
        return existingFailureContainer;

      var newFailureContainer = new RowFailureContainer(rowObject);
      _rowValidationFailures.Add(rowObject, newFailureContainer);

      return newFailureContainer;
    }
  }
}

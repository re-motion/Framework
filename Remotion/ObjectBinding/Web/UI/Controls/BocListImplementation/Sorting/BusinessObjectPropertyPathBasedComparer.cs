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
using Remotion.Collections;
using Remotion.Logging;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  using CacheValue = Tuple<object?, DoubleCheckedLockingContainer<string>>;

  public sealed class BusinessObjectPropertyPathBasedComparer : IComparer<BocListRow>
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(BusinessObjectPropertyPathBasedComparer));

    private readonly IBusinessObjectPropertyPath _propertyPath;
    private readonly Dictionary<BocListRow, CacheValue> _cache = new Dictionary<BocListRow, CacheValue>();

    public BusinessObjectPropertyPathBasedComparer (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      _propertyPath = propertyPath;
    }

    public IBusinessObjectPropertyPath PropertyPath
    {
      get { return _propertyPath; }
    }

    public int Compare (BocListRow? rowA, BocListRow? rowB)
    {
      ArgumentUtility.CheckNotNull("rowA", rowA!);
      ArgumentUtility.CheckNotNull("rowB", rowB!);

      object? valueA = GetPropertyPathValueFromCache(rowA);
      object? valueB = GetPropertyPathValueFromCache(rowB);

      if (valueA == null && valueB == null)
        return 0;
      if (valueA == null)
        return -1;
      if (valueB == null)
        return 1;

      IList? listA = valueA as IList;
      IList? listB = valueB as IList;
      if (listA != null && listB != null)
      {
        if (listA.Count == 0 && listB.Count == 0)
          return 0;
        if (listA.Count == 0)
          return -1;
        if (listB.Count == 0)
          return 1;
        valueA = listA[0];
        valueB = listB[0];
      }

      if (valueA is IComparable && valueB is IComparable)
        return CompareComparableValues(valueA, valueB);
      else
        return CompareStringValues(rowA, rowB);
    }

    private static int CompareComparableValues (object valueA, object valueB)
    {
      return Comparer.Default.Compare(valueA, valueB);
    }

    private int CompareStringValues (BocListRow? rowA, BocListRow? rowB)
    {
      ArgumentUtility.CheckNotNull("rowA", rowA!);
      ArgumentUtility.CheckNotNull("rowB", rowB!);

      var valueA = GetPropertyPathStringValueFromCache(rowA);
      var valueB = GetPropertyPathStringValueFromCache(rowB);

      return string.Compare(valueA, valueB, StringComparison.CurrentCultureIgnoreCase);
    }

    private object? GetPropertyPathValueFromCache (BocListRow row)
    {
      return _cache.GetOrCreateValue(row, GetPropertyPathResult).Item1;
    }

    private string GetPropertyPathStringValueFromCache (BocListRow row)
    {
      return _cache.GetOrCreateValue(row, GetPropertyPathResult).Item2.Value;
    }

    private CacheValue GetPropertyPathResult (BocListRow row)
    {
      IBusinessObjectPropertyPathResult result;
      try
      {
        result = _propertyPath.GetResult(
            row.BusinessObject,
            BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
            BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat(
            e, "Exception thrown while evaluating the result for property path '{0}' in row {1} of BocList.", _propertyPath.Identifier, row.Index);
        return Tuple.Create((object?)null, new DoubleCheckedLockingContainer<string>(() => null!));
      }

      return Tuple.Create(
          GetResultValue(result, row),
          new DoubleCheckedLockingContainer<string>(() => GetResultString(result, row)));
    }

    private object? GetResultValue (IBusinessObjectPropertyPathResult result, BocListRow row)
    {
      try
      {
        return result.GetValue();
      }
      catch (Exception e)
      {
        s_log.ErrorFormat(
            e, "Exception thrown while reading the value for property path '{0}' in row {1} of BocList.", _propertyPath.Identifier, row.Index);
        return null;
      }
    }

    private string GetResultString (IBusinessObjectPropertyPathResult result, BocListRow row)
    {
      try
      {
        return result.GetString(null);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat(
            e, "Exception thrown while reading string value for property path '{0}' in row {1} of BocList.", _propertyPath.Identifier, row.Index);
        return string.Empty;
      }
    }
  }
}

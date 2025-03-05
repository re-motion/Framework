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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting;

public static class TestContextExtensions
{
  public static IReadOnlyCollection<T> GetCollection<T> (this ITestContext testContext, string propertyKey)
  {
    ArgumentUtility.CheckNotNull(nameof(testContext), testContext);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyKey), propertyKey);

    if (!testContext.Properties.TryGetValue(propertyKey, out var value))
      return Array.Empty<T>();

    if (value is not IReadOnlyCollection<T> collection)
    {
      throw new InvalidCastException(
          $"Value for property '{propertyKey}' is of type unexpected type {value.GetType()}, which is incompatible with {typeof(IReadOnlyCollection<T>)}");
    }

    return collection;
  }

  public static T GetValueOrDefault<T> (this ITestContext testContext, string propertyKey, T defaultValue)
      where T : notnull
  {
    ArgumentUtility.CheckNotNull(nameof(testContext), testContext);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyKey), propertyKey);
    ArgumentUtility.CheckNotNull(nameof(defaultValue), defaultValue);

    if (!testContext.Properties.TryGetValue(propertyKey, out var value))
      return defaultValue;

    if (value is null)
      throw new InvalidOperationException($"Value for property '{propertyKey}' is null.");

    if (value is not T typedValue)
      throw new InvalidCastException($"Value for property '{propertyKey}' is of type unexpected type {value.GetType()} instead of {typeof(T)}");

    return typedValue;
  }
}

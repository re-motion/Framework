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
using System.Reflection;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting;

/// <summary>
/// Base class for <see cref="Attribute"/>s that modify web tests independent of any specific testing framework.
/// </summary>
/// <remarks>
/// Derived classes are used to populate the <see cref="ITestContext"/>'s <see cref="ITestContext.Properties"/>.
/// </remarks>
public abstract class WebTestAttribute : Attribute
{
  /// <summary>
  /// Derived classes add entries to the given <paramref name="properties"/> according to their specific purpose.
  /// </summary>
  public abstract void ApplyValue ([NotNull] IDictionary<string, object> properties);

  /// <summary>
  /// Creates, populates, and returns an <see cref="IReadOnlyDictionary{TKey,TValue}"/> that maps a property key to a value governed by the derived class.
  /// </summary>
  /// <remarks>The resulting <see cref="IReadOnlyDictionary{TKey,TValue}"/> is used for the <see cref="ITestContext"/>'s <see cref="ITestContext.Properties"/>.</remarks>
  public static IReadOnlyDictionary<string, object> CreatePropertiesFromAttributes ([NotNull] MethodInfo testMethod)
  {
    ArgumentNullException.ThrowIfNull(testMethod);

    var testAttributes = testMethod.GetCustomAttributes<WebTestAttribute>(true);
    var classAttributes = testMethod.DeclaringType?.GetCustomAttributes<WebTestAttribute>(true) ?? Array.Empty<WebTestAttribute>();
    var assemblyAttributes = testMethod.DeclaringType?.Assembly.GetCustomAttributes<WebTestAttribute>() ?? Array.Empty<WebTestAttribute>();

    var properties = new Dictionary<string, object>();
    foreach (var attribute in assemblyAttributes.Concat(classAttributes).Concat(testAttributes))
    {
      attribute.ApplyValue(properties);
    }

    return properties;
  }
}

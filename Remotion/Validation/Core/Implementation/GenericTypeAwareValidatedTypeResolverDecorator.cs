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
using System.Collections.Concurrent;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements <see cref="IValidatedTypeResolver"/> and resolves the validated Type via <see cref="IValidationRuleCollector{TValidatedType}"/>.
  /// </summary>
  [ImplementationFor(typeof(IValidatedTypeResolver), Position = 0, RegistrationType = RegistrationType.Decorator)]
  public class GenericTypeAwareValidatedTypeResolverDecorator : IValidatedTypeResolver
  {
    private static readonly ConcurrentDictionary<Type, (bool CanAscribeTo, Type? ItemType)> s_genericValidationRuleCollectorTypeCache = new();
    private readonly IValidatedTypeResolver _validatedTypeResolver;

    public GenericTypeAwareValidatedTypeResolverDecorator (IValidatedTypeResolver validatedTypeResolver)
    {
      ArgumentUtility.CheckNotNull("validatedTypeResolver", validatedTypeResolver);

      _validatedTypeResolver = validatedTypeResolver;
    }

    public IValidatedTypeResolver InnerResolver
    {
      get { return _validatedTypeResolver; }
    }

    public Type? GetValidatedType (Type collectorType)
    {
      ArgumentUtility.CheckNotNull("collectorType", collectorType);

      var itemType = s_genericValidationRuleCollectorTypeCache.GetOrAdd(
              collectorType,
              static t =>
              {
                var canAscribeTo = typeof(IValidationRuleCollector).IsAssignableFrom(t) && t.CanAscribeTo(typeof(IValidationRuleCollector<>));
                return ValueTuple.Create(
                    canAscribeTo,
                    canAscribeTo
                        ? t.GetAscribedGenericArguments(typeof(IValidationRuleCollector<>))[0]
                        : null);
              })
          .ItemType;

      if (itemType != null)
        return itemType;

      return _validatedTypeResolver.GetValidatedType(collectorType);
    }
  }
}

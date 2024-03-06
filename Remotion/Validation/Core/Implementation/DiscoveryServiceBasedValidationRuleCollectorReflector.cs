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
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Attributes;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements the <see cref="IValidationRuleCollectorReflector"/> and uses the <see cref="ITypeDiscoveryService"/> to find all implementations
  /// of the <see cref="IValidationRuleCollector"/> interface. The <see cref="IValidatedTypeResolver"/> is the used to associate the 
  /// collector types to the validated type. 
  /// </summary>
  [ImplementationFor(typeof(IValidationRuleCollectorReflector), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class DiscoveryServiceBasedValidationRuleCollectorReflector : IValidationRuleCollectorReflector
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly Lazy<ILookup<Type, Type>> _validationCollectors;
    private readonly IValidatedTypeResolver _validatedTypeResolver;

    public DiscoveryServiceBasedValidationRuleCollectorReflector (
        ITypeDiscoveryService typeDiscoveryService,
        IValidatedTypeResolver validatedTypeResolver)
    {
      ArgumentUtility.CheckNotNull("typeDiscoveryService", typeDiscoveryService);
      ArgumentUtility.CheckNotNull("validatedTypeResolver", validatedTypeResolver);

      _typeDiscoveryService = typeDiscoveryService;
      _validatedTypeResolver = validatedTypeResolver;
      _validationCollectors = new Lazy<ILookup<Type, Type>>(GetValidationCollectors, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public IValidatedTypeResolver ValidatedTypeResolver
    {
      get { return _validatedTypeResolver; }
    }

    public IEnumerable<Type> GetCollectorsForType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _validationCollectors.Value[type];
    }

    private ILookup<Type, Type> GetValidationCollectors ()
    {
      return _typeDiscoveryService.GetTypes(typeof(IValidationRuleCollector), excludeGlobalTypes: false).Cast<Type>()
          .Where(IsRelevant)
          .ToLookup(GetValidatedType, collectorType => collectorType);
    }

    private Type GetValidatedType (Type collectorType)
    {
      var type = _validatedTypeResolver.GetValidatedType(collectorType);
      if (type == null)
        throw new InvalidOperationException(string.Format("No validated type could be resolved for collector '{0}'.", collectorType.GetFullNameSafe()));
      return type;
    }

    private bool IsRelevant (Type collectorType)
    {
      return !(collectorType.IsAbstract
               || collectorType.IsInterface
               || collectorType.IsGenericTypeDefinition
               || collectorType.IsDefined(typeof(ApplyProgrammaticallyAttribute), false));
    }
  }
}

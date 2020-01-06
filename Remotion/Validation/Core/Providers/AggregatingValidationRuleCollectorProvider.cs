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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Use this class to retrieve the combined <see cref="IValidationRuleCollector"/>s for a <see cref="Type"/> 
  /// provided by the individual <see cref="IValidationRuleCollectorProvider"/>s registered with the application's IoC container.
  /// </summary>
  [ImplementationFor (typeof (IValidationRuleCollectorProvider), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class AggregatingValidationRuleCollectorProvider : IValidationRuleCollectorProvider
  {
    private readonly IInvolvedTypeProvider _involvedTypeProvider;
    private readonly IReadOnlyList<IValidationRuleCollectorProvider> _validationCollectorProviders;

    public AggregatingValidationRuleCollectorProvider (
        IInvolvedTypeProvider involvedTypeProvider,
        IEnumerable<IValidationRuleCollectorProvider> validationCollectorProviders)
    {
      ArgumentUtility.CheckNotNull ("involvedTypeProvider", involvedTypeProvider);
      ArgumentUtility.CheckNotNull ("validationCollectorProviders", validationCollectorProviders);

      _involvedTypeProvider = involvedTypeProvider;
      _validationCollectorProviders = validationCollectorProviders.ToList();
    }

    public IInvolvedTypeProvider InvolvedTypeProvider
    {
      get { return _involvedTypeProvider; }
    }

    public IReadOnlyList<IValidationRuleCollectorProvider> ValidationCollectorProviders
    {
      get { return _validationCollectorProviders; }
    }

    public IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> GetValidationRuleCollectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return GetTypeGroups (types)
          .Aggregate (
              Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>(),
              (validationCollectors, typeGroup) => validationCollectors.Concat (GetValidationRuleCollectorsForTypeGroup (typeGroup)));
    }

    private IEnumerable<IEnumerable<Type>> GetTypeGroups (IEnumerable<Type> types)
    {
      return types.SelectMany (t => _involvedTypeProvider.GetTypes (t));
    }

    private IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> GetValidationRuleCollectorsForTypeGroup (IEnumerable<Type> typeGroup)
    {
      var evaluatedTypeGroup = typeGroup.ToArray();
      return _validationCollectorProviders.SelectMany (p => p.GetValidationRuleCollectors (evaluatedTypeGroup));
    }
  }
}
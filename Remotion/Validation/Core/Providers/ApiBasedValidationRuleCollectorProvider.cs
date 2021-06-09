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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Use this class to retrieve the <see cref="IValidationRuleCollector{TValidatedType}"/>s for a <see cref="Type"/> based on reflection metadata.
  /// </summary>
  [ImplementationFor (typeof (IValidationRuleCollectorProvider), Lifetime = LifetimeKind.Singleton, Position = 2, RegistrationType = RegistrationType.Multiple)]
  public class ApiBasedValidationRuleCollectorProvider : IValidationRuleCollectorProvider
  {
    private readonly IValidationRuleCollectorReflector _validationRuleCollectorReflector;

    public ApiBasedValidationRuleCollectorProvider (IValidationRuleCollectorReflector validationRuleCollectorReflector)
    {
      ArgumentUtility.CheckNotNull ("validationRuleCollectorReflector", validationRuleCollectorReflector);

      _validationRuleCollectorReflector = validationRuleCollectorReflector;
    }

    public IValidationRuleCollectorReflector ValidationRuleCollectorReflector
    {
      get { return _validationRuleCollectorReflector; }
    }

    public IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> GetValidationRuleCollectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var result = types
          .SelectMany (_validationRuleCollectorReflector.GetCollectorsForType)
          .Select (c => new ValidationRuleCollectorInfo (
              (IValidationRuleCollector) Assertion.IsNotNull (Activator.CreateInstance (c), "Could not create an instance of {0}.", c.GetFullNameSafe()),
              GetType()))
          .ToArray();
      return result.Any() ? new[] { result } : Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>();
    }
  }
}
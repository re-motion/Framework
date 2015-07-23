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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="IValidationTypeFilter"/>-instances. When calling <see cref="IsValidatableType"/>, all combined 
  /// <see cref="IValidationTypeFilter"/> instances must confirm that a <see cref="Type"/> can be used 
  /// as the <see cref="IComponentValidationCollector.ValidatedType"/> of a <see cref="IComponentValidationCollector"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IValidationTypeFilter), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundValidationTypeFilter : IValidationTypeFilter
  {
    private readonly ReadOnlyCollection<IValidationTypeFilter> _validationTypeFilters;

    public CompoundValidationTypeFilter (IEnumerable<IValidationTypeFilter> validationTypeFilters)
    {
      ArgumentUtility.CheckNotNull ("validationTypeFilters", validationTypeFilters);

      _validationTypeFilters = validationTypeFilters.ToList().AsReadOnly();
    }

    public ReadOnlyCollection<IValidationTypeFilter> ValidationTypeFilters
    {
      get { return _validationTypeFilters; }
    }

    public bool IsValidatableType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _validationTypeFilters.All (f => f.IsValidatableType (type));
    }
  }
}
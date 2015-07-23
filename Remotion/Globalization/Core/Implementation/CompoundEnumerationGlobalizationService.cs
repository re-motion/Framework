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

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="IEnumerationGlobalizationService"/>-instances and 
  /// delegates to them to retrieve localized name for a specified <see cref="Enum"/> value.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IEnumerationGlobalizationService), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompoundEnumerationGlobalizationService : IEnumerationGlobalizationService
  {
    private readonly IReadOnlyCollection<IEnumerationGlobalizationService> _enumerationGlobalizationServices;

    /// <summary>
    ///   Combines several <see cref="IEnumerationGlobalizationService"/>-instances to a single <see cref="CompoundEnumerationGlobalizationService"/>.
    /// </summary>
    /// <param name="enumerationGlobalizationServices"> The <see cref="IEnumerationGlobalizationService"/>s, starting with the least specific.</param>
    public CompoundEnumerationGlobalizationService (IEnumerable<IEnumerationGlobalizationService> enumerationGlobalizationServices)
    {
      ArgumentUtility.CheckNotNull ("enumerationGlobalizationServices", enumerationGlobalizationServices);

      _enumerationGlobalizationServices = enumerationGlobalizationServices.ToArray();
    }

    public IEnumerable<IEnumerationGlobalizationService> EnumerationGlobalizationServices
    {
      get { return _enumerationGlobalizationServices; }
    }

    public bool TryGetEnumerationValueDisplayName (Enum value, out string result)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      foreach (var service in _enumerationGlobalizationServices)
      {
        if (service.TryGetEnumerationValueDisplayName (value, out result))
          return true;
      }

      result = null;
      return false;
    }
  }
}
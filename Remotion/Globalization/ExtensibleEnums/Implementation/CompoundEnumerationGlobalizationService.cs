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
using Remotion.ExtensibleEnums;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.ExtensibleEnums.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="IExtensibleEnumGlobalizationService"/>-instances and 
  /// delegates to them to retrieve localized name for a specified <see cref="IExtensibleEnum"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IExtensibleEnumGlobalizationService), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompoundExtensibleEnumGlobalizationService : IExtensibleEnumGlobalizationService
  {
    private readonly IReadOnlyCollection<IExtensibleEnumGlobalizationService> _extensibleEnumGlobalizationServices;

    /// <summary>
    ///   Combines several <see cref="IExtensibleEnumGlobalizationService"/>-instances to a single <see cref="CompoundExtensibleEnumGlobalizationService"/>.
    /// </summary>
    /// <param name="extensibleEnumGlobalizationServices"> The <see cref="IExtensibleEnumGlobalizationService"/>s, starting with the least specific.</param>
    public CompoundExtensibleEnumGlobalizationService (IEnumerable<IExtensibleEnumGlobalizationService> extensibleEnumGlobalizationServices)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumGlobalizationServices", extensibleEnumGlobalizationServices);

      _extensibleEnumGlobalizationServices = extensibleEnumGlobalizationServices.ToArray();
    }

    public IEnumerable<IExtensibleEnumGlobalizationService> ExtensibleEnumGlobalizationServices
    {
      get { return _extensibleEnumGlobalizationServices; }
    }

    public bool TryGetExtensibleEnumValueDisplayName (IExtensibleEnum value, out string result)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      foreach (var service in _extensibleEnumGlobalizationServices)
      {
        if (service.TryGetExtensibleEnumValueDisplayName (value, out result))
          return true;
      }

      result = null;
      return false;
    }
  }
}
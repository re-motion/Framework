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

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="IGlobalizationService"/>-instances and 
  /// delegates to it to retrieve an <see cref="IResourceManager"/> for a specified type.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IGlobalizationService), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompoundGlobalizationService : IGlobalizationService
  {
    private readonly IReadOnlyList<IGlobalizationService> _globalizationServices;

    /// <summary>
    ///   Combines several <see cref="IGlobalizationService"/>-instances to a single <see cref="CompoundGlobalizationService"/>.
    /// </summary>
    /// <param name="globalizationServices"> The <see cref="IGlobalizationService"/>s, starting with the least specific.</param>
    public CompoundGlobalizationService (IEnumerable<IGlobalizationService> globalizationServices)
    {
      ArgumentUtility.CheckNotNull ("globalizationServices", globalizationServices);

      _globalizationServices = globalizationServices.Reverse().ToList().AsReadOnly();
    }

    public IReadOnlyList<IGlobalizationService> GlobalizationServices
    {
      get { return _globalizationServices; }
    }

    public IResourceManager GetResourceManager (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

      return new ResourceManagerSet (_globalizationServices.Select (s => s.GetResourceManager (typeInformation)));
    }
  }
}
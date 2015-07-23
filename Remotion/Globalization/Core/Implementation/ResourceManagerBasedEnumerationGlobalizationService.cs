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
using System.Reflection;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of enumeration objects.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IEnumerationGlobalizationService), Lifetime = LifetimeKind.Singleton, 
      Position = Position, RegistrationType = RegistrationType.Multiple)]
  public sealed class ResourceManagerBasedEnumerationGlobalizationService : IEnumerationGlobalizationService
  {
    public const int Position = MultiLingualNameBasedEnumerationGlobalizationService.Position - 1;

    private readonly IGlobalizationService _globalizationService;
    private readonly IMemberInformationNameResolver _memberInformationNameResolver;

    public ResourceManagerBasedEnumerationGlobalizationService (
        IGlobalizationService globalizationService,
        IMemberInformationNameResolver memberInformationNameResolver)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      ArgumentUtility.CheckNotNull ("memberInformationNameResolver", memberInformationNameResolver);
      
      _globalizationService = globalizationService;
      _memberInformationNameResolver = memberInformationNameResolver;
    }

    public bool TryGetEnumerationValueDisplayName (Enum value, out string result)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      var resourceManager = _globalizationService.GetResourceManager (value.GetType());
      return resourceManager.TryGetString (_memberInformationNameResolver.GetEnumName (value), out result);
    }
  }
}
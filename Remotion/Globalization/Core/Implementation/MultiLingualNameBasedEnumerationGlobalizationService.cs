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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of <see cref="Enum"/> values based on the <see cref="MultiLingualNameAttribute"/> 
  /// applied to the respective <see cref="Enum"/> value.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IEnumerationGlobalizationService), Lifetime = LifetimeKind.Singleton,
      Position = Position, RegistrationType = RegistrationType.Multiple)]
  public sealed class MultiLingualNameBasedEnumerationGlobalizationService : IEnumerationGlobalizationService
  {
    public const int Position = 0;

    private class LocalizedNameForEnumerationProvider : LocalizedNameProviderBase<Enum>
    {
      protected override IEnumerable<MultiLingualNameAttribute> GetCustomAttributes (Enum value)
      {
        ArgumentUtility.CheckNotNull ("value", value);

        var field = value.GetType().GetField (value.ToString(), BindingFlags.Static | BindingFlags.Public);
        if (field == null)
          return Enumerable.Empty<MultiLingualNameAttribute>();
        return AttributeUtility.GetCustomAttributes<MultiLingualNameAttribute> (field, false);
      }

      protected override Assembly GetAssembly (Enum reflectionObject)
      {
        ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

        return reflectionObject.GetType().Assembly;
      }

      protected override string GetContextForExceptionMessage (Enum value)
      {
        ArgumentUtility.CheckNotNull ("value", value);

        return string.Format ("The enum value '{0}' declared on type '{1}'", value, value.GetType());
      }
    }

    private readonly LocalizedNameForEnumerationProvider _localizedNameForEnumerationProvider = new LocalizedNameForEnumerationProvider();

    public MultiLingualNameBasedEnumerationGlobalizationService ()
    {
    }

    public bool TryGetEnumerationValueDisplayName (Enum value, out string result)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      return _localizedNameForEnumerationProvider.TryGetLocalizedNameForCurrentUICulture (value, out result);
    }
  }
}
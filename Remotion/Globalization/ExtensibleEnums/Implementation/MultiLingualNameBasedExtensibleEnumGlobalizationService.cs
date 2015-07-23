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
using System.Reflection;
using Remotion.ExtensibleEnums;
using Remotion.FunctionalProgramming;
using Remotion.Globalization.Implementation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.ExtensibleEnums.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of <see cref="IExtensibleEnum"/> values based on the <see cref="MultiLingualNameAttribute"/> 
  /// applied to the respective <see cref="IExtensibleEnum"/> value.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IExtensibleEnumGlobalizationService), Lifetime = LifetimeKind.Singleton,
      Position = Position, RegistrationType = RegistrationType.Multiple)]
  public sealed class MultiLingualNameBasedExtensibleEnumGlobalizationService : IExtensibleEnumGlobalizationService
  {
    public const int Position = 0;

    private class LocalizedNameForEnumerationProvider : LocalizedNameProviderBase<MethodInfo>
    {
      protected override IEnumerable<MultiLingualNameAttribute> GetCustomAttributes (MethodInfo definingMethod)
      {
        ArgumentUtility.CheckNotNull ("definingMethod", definingMethod);

        return definingMethod.GetCustomAttributes<MultiLingualNameAttribute>(false);
      }

      protected override Assembly GetAssembly (MethodInfo definingMethod)
      {
        ArgumentUtility.CheckNotNull ("definingMethod", definingMethod);

        return Maybe.ForValue (definingMethod.DeclaringType).Select (t => t.Assembly).ValueOrDefault();
      }

      protected override string GetContextForExceptionMessage (MethodInfo definingMethod)
      {
        ArgumentUtility.CheckNotNull ("definingMethod", definingMethod);

        return string.Format ("The extensible enum value '{0}' declared on type '{1}'", definingMethod.Name, definingMethod.DeclaringType);
      }
    }

    private readonly LocalizedNameForEnumerationProvider _localizedNameForEnumerationProvider = new LocalizedNameForEnumerationProvider();

    public MultiLingualNameBasedExtensibleEnumGlobalizationService ()
    {
    }

    public bool TryGetExtensibleEnumValueDisplayName (IExtensibleEnum value, out string result)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      var extensibleEnumInfo = value.GetValueInfo();
      Assertion.IsNotNull (extensibleEnumInfo, "No value info found for extensible enum '{0}'.", value.ID);

      var definingMethod = extensibleEnumInfo.DefiningMethod;
      Assertion.IsNotNull (definingMethod, "No defining method found for extensible enum '{0}'.", value.ID);

      return _localizedNameForEnumerationProvider.TryGetLocalizedNameForCurrentUICulture (definingMethod, out result);
    }
  }
}
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
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Encapsulates the rules governing mixin inheritance for target types.
  /// </summary>
  public class DefaultMixinInheritancePolicy : IMixinInheritancePolicy
  {
    public static readonly DefaultMixinInheritancePolicy Instance = new DefaultMixinInheritancePolicy();

    private DefaultMixinInheritancePolicy ()
    {
    }

    /// <summary>
    /// Gets the types this <paramref name="targetType"/> inherits mixins from. A target type inherits mixins from its generic type definition,
    /// its base class, and its interfaces.
    /// </summary>
    /// <param name="targetType">The type whose "base" types should be retrieved.</param>
    /// <returns>The types from which the given <paramref name="targetType"/> inherits its mixins.</returns>
    public IEnumerable<Type> GetTypesToInheritFrom (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      if (targetType.IsGenericType && !targetType.IsGenericTypeDefinition)
        yield return targetType.GetGenericTypeDefinition ();
      if (targetType.BaseType != null)
        yield return targetType.BaseType;
      foreach (Type interfaceType in targetType.GetInterfaces ())
        yield return interfaceType;
    }

    /// <summary>
    /// Gets the class contexts this <paramref name="targetType"/> inherits mixins from. A target type inherits mixins from its generic type 
    /// definition, its base class, and its interfaces.
    /// </summary>
    /// <param name="targetType">The type whose "base" <see cref="ClassContext"/> instances should be retrieved.</param>
    /// <param name="classContextRetriever">A function returning the <see cref="ClassContext"/> for a given type.</param>
    /// <returns><see cref="ClassContext"/> objects for the types from which the given <paramref name="targetType"/> inherits its mixins.</returns>
    public IEnumerable<ClassContext> GetClassContextsToInheritFrom (Type targetType, Func<Type, ClassContext> classContextRetriever)
    {
      return GetTypesToInheritFrom (targetType).Select (classContextRetriever);
    }
  }
}

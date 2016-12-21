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
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  /// <summary>
  /// Finds the requirements imposed by a given <see cref="MixinDefinition"/> on its <see cref="TargetClassDefinition"/> by analyzing the types
  /// passed as generic arguments of the <see cref="Mixin{TTarget}"/> and <see cref="Mixin{TTarget,TNext}"/> base classes.
  /// </summary>
  /// <remarks>
  /// A <see cref="MixinDefinition"/> can either pass an ordinary type (<c>Mixin&lt;IRequirements&gt;</c>) or one of its own generic parameters 
  /// (<c>Mixin&lt;T&gt;</c>) to its to its <see cref="Mixin{TTarget}"/> or <see cref="Mixin{TTarget,TNext}"/> base classes. This class detects these
  /// two scenarios and gathers concrete requirement information from either the type itself or its generic parameter constraints.
  /// </remarks>
  public class RequirementsAnalyzer
  {
    private readonly MixinGenericArgumentFinder _genericArgumentFinder;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequirementsAnalyzer"/> class.
    /// </summary>
    /// <param name="genericArgumentFinder">The <see cref="MixinGenericArgumentFinder"/> used to determine the generic argument defining the
    /// requirements to be analyzed.</param>
    public RequirementsAnalyzer (MixinGenericArgumentFinder genericArgumentFinder)
    {
      ArgumentUtility.CheckNotNull ("genericArgumentFinder", genericArgumentFinder);
      _genericArgumentFinder = genericArgumentFinder;
    }

    public Type[] GetRequirements (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      var genericArgument = _genericArgumentFinder.FindGenericArgument (mixinType);
      if (genericArgument != null)
        return GetRequirementsForType (genericArgument).Distinct().ToArray();
      else
        return Type.EmptyTypes;
    }

    // The generic arguments used for MixinBase<,> are bound to either to real types or to new type parameters
    // The real types are directly taken as required interfaces; the type parameters have constraints which are taken as required interfaces
    private IEnumerable<Type> GetRequirementsForType (Type mixinBaseGenericArgument)
    {
      ArgumentUtility.CheckNotNull ("mixinBaseGenericArgument", mixinBaseGenericArgument);

      if (mixinBaseGenericArgument.IsGenericParameter)
      {
        return from constraint in mixinBaseGenericArgument.GetGenericParameterConstraints ()
               from requirement in GetRequirementsForType (constraint)
               select requirement;
      }
      else
      {
        if (mixinBaseGenericArgument.IsInterface) // if this is an interface, add all inherited interfaces as requirements as well
          return new[] { mixinBaseGenericArgument }.Concat (mixinBaseGenericArgument.GetInterfaces());
        else
          return new[] { mixinBaseGenericArgument };
      }
    }
  }
}

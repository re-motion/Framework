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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Performs common reflection operations on mixins and mixed types. The results of these operations are not cached.
  /// </summary>
  public class MixinReflector
  {
    public enum InitializationMode { Construction, Deserialization }

    public static Type GetMixinBaseType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      Type currentType = mixinType;

      while (currentType != null
          && !ReflectionUtility.IsEqualOrInstantiationOf (currentType, typeof (Mixin<>))
          && !ReflectionUtility.IsEqualOrInstantiationOf (currentType, typeof (Mixin<,>)))
      {
        currentType = currentType.BaseType;
      }

      return currentType;
    }

    public static PropertyInfo GetTargetProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("Target", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetNextProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("Next", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static Type GetNextCallProxyType (object mixinTargetInstance)
    {
      ArgumentUtility.CheckNotNull ("mixinTargetInstance", mixinTargetInstance);
      var castTarget = mixinTargetInstance as IMixinTarget;
      if (castTarget == null)
      {
        string message = string.Format ("The given object of type {0} is not a mixin target.", mixinTargetInstance.GetType().FullName);
        throw new ArgumentException (message, "mixinTargetInstance");
      }

      Assertion.IsNotNull (castTarget.FirstNextCallProxy);
      Type NextCallProxyType = castTarget.FirstNextCallProxy.GetType();
      return NextCallProxyType;
    }

    /// <summary>
    /// Returns the types of the mixins that were used to generate <paramref name="concreteMixedType"/>. The mixins are ordered and open generic 
    /// mixins are closed exactly as used by the code generation (and as defined by <see cref="TargetClassDefinition.Mixins"/>.
    /// </summary>
    /// <param name="concreteMixedType">The concrete mixed type whose mixins should be retrieved.</param>
    /// <returns>An ordered array of mixin types that directly corresponds to the mixins held by instances of the mixed type.</returns>
    public static Type[] GetOrderedMixinTypesFromConcreteType (Type concreteMixedType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixedType", concreteMixedType);

      var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (concreteMixedType, true);
      if (attribute == null)
        return null;
      else
        return attribute.OrderedMixinTypes;
    }
  }
}

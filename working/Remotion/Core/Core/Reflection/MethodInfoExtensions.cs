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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines extension methods for working with <see cref="MethodInfo"/>.
  /// </summary>
  public static class MethodInfoExtensions
  {
    /// <summary>
    /// Returns the <see cref="Type"/> where the method was initially declared.
    /// </summary>
    /// <param name="methodInfo">The method whose type should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The <see cref="Type"/> where the method was declared for the first time.</returns>
    public static Type GetOriginalDeclaringType (this MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      return methodInfo.GetBaseDefinition ().DeclaringType;
    }

    /// <summary>
    /// Finds the property declaration corresponding to this <see cref="MethodInfo"/> on the given <see cref="Type"/> and it's base types.
    /// </summary>
    /// <returns>
    /// Returns the <see cref="PropertyInfo"/> of the declared property, or <see langword="null" /> if no corresponding property was found.
    /// </returns>
    public static PropertyInfo FindDeclaringProperty (this MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      // Note: We scan the hierarchy ourselves because private (eg., explicit) property implementations in base types are ignored by GetProperties
      // We use AreEqualMethodsWithoutReflectedType because our algorithm manually iterates over the base type hierarchy, so the accesor's
      // ReflectedType will be the declaring type, whereas _methodInfo might have a different ReflectedType.
      // AreEqualMethodsWithoutReflectedType can't deal with closed generic methods, but property accessors aren't generic anyway.

      return (from t in methodInfo.DeclaringType.CreateSequence (t => t.BaseType)
        from pi in t.GetProperties (BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
        from accessor in pi.GetAccessors (true)
        where MemberInfoEqualityComparer<MethodInfo>.Instance.Equals (methodInfo, accessor)
        select pi).FirstOrDefault();
    }
  }
}
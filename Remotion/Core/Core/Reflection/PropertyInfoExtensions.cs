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
  /// Defines extension methods for working with <see cref="PropertyInfo"/>.
  /// </summary>
  public static class PropertyInfoExtensions
  {
    /// <summary>
    /// <see cref="PropertyInfo"/> object for the property on the direct or indirect base class in which the property represented by this instance was first declared.
    /// </summary>
    /// <returns>A <see cref="PropertyInfo"/> object for the first implementation of this method.</returns>
    public static PropertyInfo GetBaseDefinition (this PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var declaringType = propertyInfo.DeclaringType;
      if (declaringType == null)
        return propertyInfo;

      MethodInfo[] accessors = propertyInfo.GetAccessors (true);
      if (accessors.Length == 0)
      {
        throw new ArgumentException (
            String.Format ("The property does not define any accessors.\r\n  Type: {0}, property: {1}", declaringType, propertyInfo.Name),
            "propertyInfo");
      }

      var originalDeclaringType = GetOriginalDeclaringType (propertyInfo);

      if (originalDeclaringType == null)
        return propertyInfo;

      if (declaringType == originalDeclaringType)
        return propertyInfo;

      var accessorBaseDefinitions = accessors.Select (a => a.GetBaseDefinition()).ToArray();

      var baseDefinition = Enumerable.Where (originalDeclaringType
              .GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), p => p.Name == propertyInfo.Name)
          .Where (p => p.GetIndexParameters().Length == propertyInfo.GetIndexParameters().Length)
          .Where (p => p.GetAccessors (true).All (a => accessorBaseDefinitions.Contains (a, MemberInfoEqualityComparer<MethodInfo>.Instance)))
          .SingleOrDefault (
              () => new AmbiguousMatchException (
                  String.Format (
                      "The property '{0}' declared on derived type '{1}' resolves to more than one possible base definition on type '{2}'.",
                      propertyInfo.Name,
                      declaringType,
                      originalDeclaringType)));

      if (baseDefinition == null)
      {
        throw new MissingMemberException (
            String.Format (
                "The property '{0}' declared on derived type '{1}' could not be resolved for base type '{2}'.",
                propertyInfo.Name,
                declaringType,
                originalDeclaringType));
      }

      return baseDefinition;
    }

    /// <summary>
    /// Returns the <see cref="Type"/> where the property was initially decelared.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The <see cref="Type"/> where the property was declared for the first time.</returns>
    public static Type GetOriginalDeclaringType (this PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      MethodInfo[] accessors = propertyInfo.GetAccessors (true);
      if (accessors.Length == 0)
      {
        throw new ArgumentException (
            String.Format ("The property does not define any accessors.\r\n  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name),
            "propertyInfo");
      }

      return accessors[0].GetOriginalDeclaringType();
    }

    /// <summary>
    /// Determines whether the given <see cref="PropertyInfo"/> is the original base declaration.
    /// </summary>
    /// <param name="propertyInfo">The property info to check.</param>
    /// <returns>
    ///   <see langword="true"/> if the <paramref name="propertyInfo"/> is the first declaration of the property; <see langword="false"/> if it is an 
    ///   overrride.
    /// </returns>
    public static bool IsOriginalDeclaration (this PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Type originalDeclaringType = GetOriginalDeclaringType (propertyInfo);
      return propertyInfo.DeclaringType == originalDeclaringType;
    }

    /// <summary>
    /// Guesses whether the given property is an explicit interface implementation by checking whether it has got private virtual final accessors.
    /// This can be used as a heuristic to find explicit interface properties without having to check InterfaceMaps for every interface on
    /// info.DeclaringType. With C# and VB.NET, the heuristic should always be right.
    /// </summary>
    /// <param name="info">The property to check.</param>
    /// <returns>True, if the property is very likely an explicit interface implementation (at least in C# and VB.NET code); otherwise, false.</returns>
    public static bool GuessIsExplicitInterfaceProperty (this PropertyInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      return info.GetAccessors (true).Any (accessor => accessor.IsPrivate && accessor.IsVirtual && accessor.IsFinal);
    }
  }
}
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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Reflection;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  [Obsolete ("Use TypeExtensions, PropertyInfoExtensions, and MethodInfoExtensions instead. (Version 1.15.17.0)")]
  public static class ReflectionUtility
  {
    [Obsolete ("This method has been replaced by AttributeUtilities.GetCustomAttriubte. (Version 1.9.2)", true)]
    public static object GetSingleAttribute (MemberInfo member, Type attributeType, bool inherit, bool throwExceptionIfNotPresent)
    {
      throw new NotImplementedException ("Obsolete. Use AttributeUtilities.GetCustomAttriubte instead.");
    }

    [Obsolete ("This method has been replaced by MemberInfoFromExpressionUtility.GetMember. (Version 1.13.148)", true)]
    public static MemberInfo GetMemberFromExpression<TSourceObject, TResult> (Expression<Func<TSourceObject, TResult>> memberAccessExpression)
    {
      throw new NotImplementedException ("Obsolete. Use MemberInfoFromExpressionUtility.GetMember instead.");
    }

    /// <summary>
    /// Evaluates whether the <paramref name="type"/> can be ascribed to the <paramref name="ascribeeType"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <param name="ascribeeType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is the <paramref name="ascribeeType"/> or its instantiation, 
    /// its subclass or the implementation of an interface in case the <paramref name="ascribeeType"/> is an interface.
    /// </returns>
    [Obsolete ("Use Remotion.Reflection.TypeExtensions.CanAscribeTo (type, ascribeeType) instead. (Version 1.15.17.0)")]
    public static bool CanAscribe (Type type, Type ascribeeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ascribeeType", ascribeeType);

      return type.CanAscribeTo (ascribeeType);
    }

    /// <summary>
    /// Returns the type arguments for the ascribed <paramref name="ascribeeType"/> as inherited or implemented by a given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <param name="ascribeeType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>A <see cref="Type"/> array containing the generic arguments of the <paramref name="ascribeeType"/> as it is inherited or implemented
    /// by <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the <paramref name="type"/> is not the <paramref name="ascribeeType"/> or its instantiation, its subclass or the implementation
    /// of an interface in case the <paramref name="ascribeeType"/> is an interface.
    /// </exception>
    /// <exception cref="AmbiguousMatchException">
    /// Thrown if the <paramref name="type"/> is an interface and implements the interface <paramref name="ascribeeType"/> or its instantiations
    /// more than once.
    /// </exception>
    [Obsolete ("Use Remotion.Reflection.TypeExtensions.GetAscribedGenericArguments (type, ascribeeType) instead. (Version 1.15.17.0)")]
    public static Type[] GetAscribedGenericArguments (Type type, Type ascribeeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ascribeeType", ascribeeType);

      return type.GetAscribedGenericArguments (ascribeeType);
    }
  }
}
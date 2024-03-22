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

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines extension methods for working with <see cref="Type"/>.
  /// </summary>
  static partial class TypeExtensions
  {
    public static string GetAssemblyQualifiedNameSafe (this Type type)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
    }

    public static string GetAssemblyQualifiedNameChecked (this Type type)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return type.AssemblyQualifiedName
             ?? throw new InvalidOperationException(string.Format("Type '{0}' does not have an assembly qualified name.", type.FullName ?? type.Name));
    }

    public static string GetFullNameSafe (this Type type)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return type.FullName ?? type.Name;
    }

    public static string GetFullNameChecked (this Type type)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return type.FullName ?? throw new InvalidOperationException(string.Format("Type '{0}' does not have a full name.", type.Name));
    }

    public static string GetNamespaceSafe (this Type type)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return type.Namespace ?? "<undefined>";
    }

    public static string GetNamespaceChecked (this Type type)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return type.Namespace ?? throw new InvalidOperationException(string.Format("Type '{0}' does not have a namespace.", type.Name));
    }
  }
}

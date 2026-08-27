// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
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
      var fullName = type.FullName;
      if (fullName == null && type.IsConstructedGenericType)
      {
        // Constructed generic types like `List<int>` have a full name so the only way get here
        // is through an open constructed type like `List<T>`, which does not have a full name.
        // We instead use `.ToString()` here to create a fake full name that still carries the
        // generic type information with it -> e.g. `System.Collection.Generics.List`1[T]`
        // Otherwise, there is no difference to an open unconstructed generic (e.g. `List<>`).
        return type.ToString();
      }

      // ReSharper disable once ConstantNullCoalescingCondition
      return fullName ?? throw new InvalidOperationException(string.Format("Type '{0}' does not have a full name.", type.Name));
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

// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides a <see cref="Null"/> property that can be assigned arbitrary values, and a type <see cref="T"/> to be used as a dummy generic argument.
  /// </summary>
  public static partial class Dev
  {
    /// <summary>
    /// Defines a dummy type that can be used as a generic argument.
    /// </summary>
    public class T
    {
    }

    /// <summary>
    /// Use this in unit tests where you need to assign a value to
    /// something (e.g., for syntactic reasons, or to remove unused variable warnings), but don't care about the result of the assignment.
    /// </summary>
    public static object? Null
    {
      get { return null; }
      // ReSharper disable ValueParameterNotUsed
      set { }
      // ReSharper restore ValueParameterNotUsed
    }
  }

  /// <summary>
  /// Provides a <see cref="Dummy"/> field that can be used as a ref or out parameter, and a typed <see cref="Null"/> property that can be assigned 
  /// arbitrary values and always returns the default value for <typeparamref name="T"/>.
  /// </summary>
  public static class Dev<T>
  {
    /// <summary>
    /// Use this in unit tests where you need a ref or out parameter but but don't care about the result of the assignment.
    /// Never rely on the value of the <see cref="Dummy"/> field, it will be changed by other tests.
    /// </summary>
    public static T Dummy = default(T)!;

    /// <summary>
    /// Use this in unit tests where you need to assign a value to
    /// something (e.g., for syntactic reasons, or to remove unused variable warnings), but don't care about the result of the assignment.
    /// </summary>
    public static T Null
    {
      get { return default(T)!; }
      set { Dev.Null = value; }
    }
  }
}

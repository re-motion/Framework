// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Diagnostics;
using System.Reflection;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides useful extension methods on <see cref="object"/> to increase clarity in unit tests.
  /// </summary>
  public static partial class ObjectExtensions
  {
    [DebuggerStepThrough]
    public static T As<T> (this object obj)
    {
      return (T)obj;
    }

    [DebuggerStepThrough]
    public static T Invoke<T> (this object target, string method, params object?[]? args)
    {
      return (T)PrivateInvoke.InvokeNonPublicMethod(target, method, args)!;
    }

    [DebuggerStepThrough]
    public static object? Invoke (this object target, string method, params object?[]? args)
    {
      return PrivateInvoke.InvokeNonPublicMethod(target, method, args);
    }

    [DebuggerStepThrough]
    public static object? Invoke (this object target, MethodInfo method, params object?[]? args)
    {
      return method.Invoke(target, args);
    }

    [DebuggerStepThrough]
    public static T Invoke<T> (this object target, MethodInfo method, params object?[]? args)
    {
      return (T)method.Invoke(target, args)!;
    }
  }
}

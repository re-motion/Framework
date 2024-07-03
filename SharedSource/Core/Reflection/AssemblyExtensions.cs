// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Reflection;

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines extension methods for working with <see cref="Assembly"/>.
  /// </summary>
  static partial class AssemblyExtensions
  {
    public static string GetFullNameSafe (this Assembly assembly)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return assembly.FullName ?? "<undefined>";
    }

    public static string GetFullNameChecked (this Assembly assembly)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return assembly.FullName ?? throw new InvalidOperationException("Assembly name is undefined.");
    }
  }
}

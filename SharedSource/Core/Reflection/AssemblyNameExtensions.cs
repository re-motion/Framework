// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Reflection;

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines extension methods for working with <see cref="AssemblyName"/>.
  /// </summary>
  static partial class AssemblyNameExtensions
  {
    public static string GetNameSafe (this AssemblyName assemblyName)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return assemblyName.Name ?? "<undefined>";
    }

    public static string GetNameChecked (this AssemblyName assemblyName)
    {
      // ReSharper disable once ConstantNullCoalescingCondition
      return assemblyName.Name ?? throw new InvalidOperationException("Assembly name is undefined.");
    }
  }
}

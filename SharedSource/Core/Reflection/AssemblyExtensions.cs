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

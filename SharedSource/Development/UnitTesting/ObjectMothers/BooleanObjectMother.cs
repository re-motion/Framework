// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.ObjectMothers
{
  /// <summary>
  /// Provides boolean values for unit tests.
  /// </summary>
  static partial class BooleanObjectMother
  {
    private static readonly Random s_random = new Random();

    /// <summary>
    /// Gets a random <see cref="bool"/> value. This is used by unit tests when they need code to work with arbitrary boolean values. Rather than
    /// duplicating the test, once for <see langword="true" /> and once for <see langword="false" />, the test is written once and is executed 
    /// with both <see langword="true" /> and <see langword="false" /> values chosen at random.
    /// </summary>
    /// <returns>A random <see cref="bool"/> value.</returns>
    public static bool GetRandomBoolean ()
    {
      return s_random.Next(2) == 1;
    }

    /// <summary>
    /// Gets a random <see cref="bool"/> value or <see langword="null"/>. This is used by unit tests when they need code to work with arbitrary
    /// nullable boolean values. Rather than
    /// duplicating the test, once for <see langword="true" /> and once for <see langword="false" />, the test is written once and is executed 
    /// with both <see langword="true" /> and <see langword="false" /> values chosen at random.
    /// </summary>
    /// <returns>A random <see cref="bool"/> value.</returns>
    public static bool? GetRandomNullableBoolean ()
    {
      return s_random.Next(3) switch
      {
          1 => true,
          2 => false,
          _ => null
      };
    }
  }
}

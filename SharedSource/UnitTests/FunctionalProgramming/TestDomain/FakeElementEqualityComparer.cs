// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.FunctionalProgramming.TestDomain
{
  internal class FakeElementEqualityComparer : IEqualityComparer<Element>
  {
    private readonly Func<Element?, Element?, bool> _equals;

    public FakeElementEqualityComparer (Func<Element?, Element?, bool> equals)
    {
      _equals = @equals;
    }

    public bool Equals (Element? x, Element? y)
    {
      return _equals(x, y);
    }

    public int GetHashCode (Element obj)
    {
      return 0;
    }
  }
}

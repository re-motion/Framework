// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0

//
using System;
using NUnit.Framework;
using Remotion.Utilities;

#nullable enable
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class ReferenceEqualityComparerTest
  {
    private ReferenceEqualityComparer<object> _comparer = default!;
    private object _obj1 = default!;
    private object _obj2 = default!;

    [SetUp]
    public void SetUp ()
    {
      _comparer = ReferenceEqualityComparer<object>.Instance;
      _obj1 = new Object();
      _obj2 = new Object();
    }

    [Test]
    public void Equal_True ()
    {
      Assert.That(_comparer.Equals(_obj1, _obj1), Is.True);
    }

    [Test]
    public void Equal_False ()
    {
      Assert.That(_comparer.Equals(_obj1, _obj2), Is.False);
    }

    [Test]
    public void GetHashcode ()
    {
      Assert.That(_comparer.GetHashCode(_obj1), Is.EqualTo(_comparer.GetHashCode(_obj1)));
    }
  }
}

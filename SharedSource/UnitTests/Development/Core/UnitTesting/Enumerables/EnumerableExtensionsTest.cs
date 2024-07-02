// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.FunctionalProgramming;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting.Enumerables
{
  [TestFixture]
  public class EnumerableExtensionsTest
  {
    [Test]
    public void AsOneTime ()
    {
      var source = new[] { 1, 2, 3 };

      OneTimeEnumerable<int> result = source.AsOneTime();

      Assert.That(result, Is.EqualTo(source));
    }

    [Test]
    public void ForceEnumeration ()
    {
      var wasCalled = false;
      var source = new[] { 7 }.ApplySideEffect(x => wasCalled = true);

      Assert.That(wasCalled, Is.False);
      var result = source.ForceEnumeration();
      Assert.That(wasCalled, Is.True);

      Assert.That(result, Is.EqualTo(source));
    }
  }
}

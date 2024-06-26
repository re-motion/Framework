// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
#if !DEBUG
  [Ignore("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckTypeIsAssignableFrom
  {
    [Test]
    public void Fail ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckTypeIsAssignableFrom("arg", typeof(object), typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' is a 'System.Object', which cannot be assigned to type 'System.String'.", "arg"));
    }

    [Test]
    public void Succeed_Null ()
    {
      ArgumentUtility.DebugCheckTypeIsAssignableFrom("arg", null, typeof(object));
    }

    [Test]
    public void Succeed ()
    {
      ArgumentUtility.DebugCheckTypeIsAssignableFrom("arg", typeof(string), typeof(object));
    }
  }
}

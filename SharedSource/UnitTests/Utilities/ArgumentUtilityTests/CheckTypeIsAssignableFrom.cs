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
  [TestFixture]
  public class CheckTypeIsAssignableFrom
  {
    [Test]
    public void Fail ()
    {
      Assert.That(
          () => ArgumentUtility.CheckTypeIsAssignableFrom("arg", typeof(object), typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' is a 'System.Object', which cannot be assigned to type 'System.String'.", "arg"));
    }

    [Test]
    public void Succeed_Null ()
    {
      Type? result = ArgumentUtility.CheckTypeIsAssignableFrom("arg", null, typeof(object));
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Succeed ()
    {
      Type result = ArgumentUtility.CheckTypeIsAssignableFrom("arg", typeof(string), typeof(object));
      Assert.That(result, Is.SameAs(typeof(string)));
    }
  }
}

// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullAndTypeIsAssignableFrom
  {
    [Test]
    public void Fail_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("arg", null, typeof(string)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_Type ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("arg", typeof(object), typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' is a 'System.Object', which cannot be assigned to type 'System.String'.", "arg"));
    }

    [Test]
    public void Succeed ()
    {
      Type result = ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("arg", typeof(string), typeof(object));
      Assert.That(result, Is.SameAs(typeof(string)));
    }
  }
}

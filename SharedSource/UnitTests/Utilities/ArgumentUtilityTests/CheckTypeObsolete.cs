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
  public class CheckType3
  {
    [Test]
    public void Fail_Type ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType("arg", 13, typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }

    [Test]
    public void Succeed_ValueType ()
    {
      Assert.That(ArgumentUtility.CheckType("arg", (object)1, typeof(int)), Is.EqualTo(1));
    }

    [Test]
    public void Succeed_NullableValueTypeNull ()
    {
      Assert.That(ArgumentUtility.CheckType("arg", (object?)null, typeof(int?)), Is.EqualTo(null));
    }

    [Test]
    public void Fail_ValueTypeNull ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType("arg", (object?)null, typeof(int)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type '<null>' when type 'System.Int32' was expected.", "arg"));
    }

    [Test]
    public void Fail_ValueType ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType("arg", (object)DateTime.MinValue, typeof(int)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.DateTime' when type 'System.Int32' was expected.", "arg"));
    }

    [Test]
    public void Succeed_ReferenceTypeNull ()
    {
      Assert.That(ArgumentUtility.CheckType("arg", (object?)null, typeof(string)), Is.EqualTo(null));
    }

    [Test]
    public void Succeed_NotNull ()
    {
      Assert.That(ArgumentUtility.CheckType("arg", "test", typeof(string)), Is.EqualTo("test"));
    }
  }
}

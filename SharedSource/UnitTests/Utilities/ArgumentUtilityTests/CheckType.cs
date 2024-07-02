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
  public class CheckType
  {
    [Test]
    public void Fail_Type ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<string>("arg", 13),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }

    [Test]
    public void Fail_ValueType ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<int>("arg", (object?)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Null ()
    {
      string result = ArgumentUtility.CheckType<string>("arg", null);
      Assert.That(result, Is.EqualTo(null));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckType<string>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Succeed_BaseType ()
    {
      string result = (string)ArgumentUtility.CheckType<object>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }
  }
}

// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNull
  {
    [Test]
    public void Nullable_Fail ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNull("arg", (int?)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Nullable_Succeed ()
    {
      int? result = ArgumentUtility.CheckNotNull("arg", (int?)1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Value_Succeed ()
    {
      int result = ArgumentUtility.CheckNotNull("arg", (int)1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Reference_Fail ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNull("arg", (string)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Reference_Succeed ()
    {
      string result = ArgumentUtility.CheckNotNull("arg", string.Empty);
      Assert.That(result, Is.SameAs(string.Empty));
    }
  }
}

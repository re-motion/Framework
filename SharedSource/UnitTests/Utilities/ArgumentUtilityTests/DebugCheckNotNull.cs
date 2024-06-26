// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
#if !DEBUG
  [Ignore("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckNotNull
  {
    [Test]
    public void Nullable_Fail ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNull("arg", (int?)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Nullable_Succeed ()
    {
      ArgumentUtility.DebugCheckNotNull("arg", (int?)1);
    }

    [Test]
    public void Value_Succeed ()
    {
      ArgumentUtility.DebugCheckNotNull("arg", (int)1);
    }

    [Test]
    public void Reference_Fail ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNull("arg", (string)null),
          Throws.InstanceOf<ArgumentNullException>());
    }
  }
}

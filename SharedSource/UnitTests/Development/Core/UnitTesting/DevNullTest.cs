// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class DevNullTest
  {
    [Test] public void GetNull ()
    {
      Assert.That(Dev.Null, Is.Null);
    }

    [Test] public void SetNull ()
    {
      Dev.Null = new object();
      Assert.That(Dev.Null, Is.Null);
    }
  }
}

// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting.Threading;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.Moq.UnitTesting.Threading
{
  [TestFixture]
  public class LockTestHelperTest
  {
    private object _lockObject;

    [SetUp]
    public void SetUp ()
    {
      _lockObject = new object();
    }

    [TearDown]
    public void TearDown ()
    {
      Assert.That(Monitor.TryEnter(_lockObject), Is.True, "Lock should have been released.");
    }

    [Test]
    public void CheckLockIsHeld ()
    {
      lock (_lockObject)
        LockTestHelper.CheckLockIsHeld(_lockObject);
    }

    [Test]
    public void CheckLockIsHeld_Throws ()
    {
      Assert.That(
          () => LockTestHelper.CheckLockIsHeld(_lockObject),
          Throws.InstanceOf<AssertionException>()
              .With.Message.Contains("Parallel thread should have been blocked."));
    }

    [Test]
    public void CheckLockIsNotHeld ()
    {
      LockTestHelper.CheckLockIsNotHeld(_lockObject);
    }

    [Test]
    public void CheckLockIsNotHeld_Throws ()
    {
      lock (_lockObject)
      {
        Assert.That(
            () => LockTestHelper.CheckLockIsNotHeld(_lockObject),
            Throws.InstanceOf<AssertionException>()
                .With.Message.Contains("Parallel thread should NOT have been blocked."));
      }
    }

    [Test]
    public void CouldAcquireLockFromOtherThread_True ()
    {
      Assert.That(LockTestHelper.CouldAcquireLockFromOtherThread(_lockObject), Is.True);
    }

    [Test]
    public void CouldAcquireLockFromOtherThread_False ()
    {
      lock (_lockObject)
        Assert.That(LockTestHelper.CouldAcquireLockFromOtherThread(_lockObject), Is.False);
    }
  }
}

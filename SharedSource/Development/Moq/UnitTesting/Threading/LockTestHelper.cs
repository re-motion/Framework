// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.Moq.UnitTesting.Threading
{
  /// <summary>
  /// A helper class for testing locking code.
  /// </summary>
  static partial class LockTestHelper
  {
    public static void CheckLockIsHeld (object lockObject)
    {
      ArgumentUtility.CheckNotNull("lockObject", lockObject);

      var lockAcquired = CouldAcquireLockFromOtherThread(lockObject);
      Assert.That(lockAcquired, Is.False, "Parallel thread should have been blocked.");
    }

    public static void CheckLockIsNotHeld (object lockObject)
    {
      ArgumentUtility.CheckNotNull("lockObject", lockObject);

      var lockAcquired = CouldAcquireLockFromOtherThread(lockObject);
      Assert.That(lockAcquired, Is.True, "Parallel thread should NOT have been blocked.");
    }

    public static bool CouldAcquireLockFromOtherThread (object lockObject)
    {
      ArgumentUtility.CheckNotNull("lockObject", lockObject);

      var lockAcquired = false;
      ThreadRunner.Run(
          () =>
          {
            lockAcquired = Monitor.TryEnter(lockObject);
            if (lockAcquired)
              Monitor.Exit(lockObject);
          });

      return lockAcquired;
    }
  }
}

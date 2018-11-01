// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.RhinoMocks.UnitTesting.Threading
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
      Assert.That (Monitor.TryEnter (_lockObject), Is.True, "Lock should have been released.");
    }

    [Test]
    public void CheckLockIsHeld ()
    {
      lock (_lockObject)
        LockTestHelper.CheckLockIsHeld (_lockObject);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), MatchType = MessageMatch.Contains,
        ExpectedMessage = "Parallel thread should have been blocked.")]
    public void CheckLockIsHeld_Throws ()
    {
      LockTestHelper.CheckLockIsHeld (_lockObject);
    }

    [Test]
    public void CheckLockIsNotHeld ()
    {
      LockTestHelper.CheckLockIsNotHeld (_lockObject);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), MatchType = MessageMatch.Contains,
        ExpectedMessage = "Parallel thread should NOT have been blocked.")]
    public void CheckLockIsNotHeld_Throws ()
    {
      lock (_lockObject)
        LockTestHelper.CheckLockIsNotHeld (_lockObject);
    }

    [Test]
    public void CouldAcquireLockFromOtherThread_True ()
    {
      Assert.That (LockTestHelper.CouldAcquireLockFromOtherThread (_lockObject), Is.True);
    }

    [Test]
    public void CouldAcquireLockFromOtherThread_False ()
    {
      lock (_lockObject)
        Assert.That (LockTestHelper.CouldAcquireLockFromOtherThread (_lockObject), Is.False);
    }
  }
}
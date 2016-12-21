// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Sandboxing;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class TestResultTest
  {
    private MethodInfo _methodInfo;
    private Exception _exception;

    [SetUp]
    public void SetUp ()
    {
      _methodInfo = typeof (DummyTest1).GetMethod ("Test1");
      _exception = new Exception ("Test");
    }

    [Test]
    public void CreateSucceeded ()
    {
      var result = TestResult.CreateSucceeded (_methodInfo);
      
      Assert.That (result.MethodInfo, Is.SameAs (_methodInfo));
      Assert.That (result.Status, Is.EqualTo (SandboxTestStatus.Succeeded));
      Assert.That (result.Exception, Is.Null);
    }

    [Test]
    public void CreateIgnored ()
    {
      var result = TestResult.CreateIgnored (_methodInfo);

      Assert.That (result.MethodInfo, Is.SameAs (_methodInfo));
      Assert.That (result.Status, Is.EqualTo (SandboxTestStatus.Ignored));
      Assert.That (result.Exception, Is.Null);
    }

    [Test]
    public void CreateFailed ()
    {
      var result = TestResult.CreateFailed (_methodInfo, _exception);

      Assert.That (result.MethodInfo, Is.SameAs (_methodInfo));
      Assert.That (result.Status, Is.EqualTo (SandboxTestStatus.Failed));
      Assert.That (result.Exception, Is.SameAs (_exception));
    }

    [Test]
    public void CreateFailedInSetUp ()
    {
      var result = TestResult.CreateFailedInSetUp (_methodInfo, _exception);

      Assert.That (result.MethodInfo, Is.SameAs (_methodInfo));
      Assert.That (result.Status, Is.EqualTo (SandboxTestStatus.FailedInSetUp));
      Assert.That (result.Exception, Is.SameAs (_exception));
    }

    [Test]
    public void CreateFailedInTearDown ()
    {
      var result = TestResult.CreateFailedInTearDown (_methodInfo, _exception);

      Assert.That (result.MethodInfo, Is.SameAs (_methodInfo));
      Assert.That (result.Status, Is.EqualTo (SandboxTestStatus.FailedInTearDown));
      Assert.That (result.Exception, Is.SameAs (_exception));
    }
    

    [Test]
    public void EnsureNotFailed_Succeeded ()
    {
      var result = TestResult.CreateSucceeded (_methodInfo);
      result.EnsureNotFailed();
    }

    [Test]
    public void EnsureNotFailed_Ignored ()
    {
      var result = TestResult.CreateIgnored (_methodInfo);
      result.EnsureNotFailed ();
    }

    [Test]
    [ExpectedException (typeof (TestFailedException), ExpectedMessage =
        "Test 'Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing.DummyTest1.Test1' failed. Status: Failed.")]
    public void EnsureNotFailed_Failed ()
    {
      var result = TestResult.CreateFailed (_methodInfo, _exception);
      result.EnsureNotFailed ();
    }

    [Test]
    [ExpectedException (typeof (TestFailedException), ExpectedMessage =
        "Test 'Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing.DummyTest1.Test1' failed. Status: FailedInSetUp.")]
    public void EnsureNotFailed_FailedInSetUp ()
    {
      var result = TestResult.CreateFailedInSetUp (_methodInfo, _exception);
      result.EnsureNotFailed ();
    }

    [Test]
    [ExpectedException (typeof (TestFailedException), ExpectedMessage =
        "Test 'Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing.DummyTest1.Test1' failed. Status: FailedInTearDown.")]
    public void EnsureNotFailed_FailedInTearDown ()
    {
      var result = TestResult.CreateFailedInTearDown (_methodInfo, _exception);
      result.EnsureNotFailed ();
    }
  }
}
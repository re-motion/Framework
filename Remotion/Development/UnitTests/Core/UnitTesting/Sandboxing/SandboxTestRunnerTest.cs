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
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Sandboxing;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class SandboxTestRunnerTest
  {
    private SandboxTestRunner _sandboxTestRunner;
    private Type[] _testFixtureTypes;

    [SetUp]
    public void SetUp ()
    {
      _sandboxTestRunner = new SandboxTestRunner();
      _testFixtureTypes = new[] { typeof (DummyTest1) };
    }

    [Test]
    public void RunTestsInSandbox ()
    {
      var permissions = PermissionSets.GetMediumTrust (AppDomain.CurrentDomain.BaseDirectory, Environment.MachineName);
      var testResults =
          SandboxTestRunner.RunTestFixturesInSandbox (_testFixtureTypes, permissions, null).SelectMany (
              r => r.TestResults).Where (r => r.Status != SandboxTestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (3));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestFixtures ()
    {
      var testResults =
          _sandboxTestRunner.RunTestFixtures (_testFixtureTypes).SelectMany (r => r.TestResults).Where (r => r.Status != SandboxTestStatus.Ignored);

      Assert.That (testResults.Count(), Is.EqualTo (3));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed();
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RunTestFixtures_ArgumentIsNull_ThrowsException ()
    {
      _sandboxTestRunner.RunTestFixtures (null);
    }

    [Test]
    public void RunTestFixture_WithSetupAndTearDownMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest1)).TestResults.Where (r => r.Status != SandboxTestStatus.Ignored);

      Assert.That (testResults.Count(), Is.EqualTo (3));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed();
    }

    [Test]
    public void RunTestFixture_WithoutSetupAndTearDownMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest2)).TestResults.Where (r => r.Status != SandboxTestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (1));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestFixture_WithoutTearDownMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest3)).TestResults.Where (r => r.Status != SandboxTestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (2));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestFixture_WithoutSetupMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest4)).TestResults.Where (r => r.Status != SandboxTestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (2));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RunTestFixture_ArgumentIsNull_ThrowsException ()
    {
      _sandboxTestRunner.RunTestFixture (null);
    }

    [Test]
    public void RunTestMethod_IgnoredTestMethod ()
    {
      var instance = new DummyTest5();
      var testMethod = typeof (DummyTest5).GetMethod ("TestIgnored");

      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, null);
      testResult.EnsureNotFailed();
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.Ignored));
    }

    [Test]
    public void RunTestMethod_IgnoredTestFixture ()
    {
      var instance = new DummyTestIgnored ();
      var testMethod = typeof (DummyTestIgnored).GetMethod ("TestIgnored");

      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, null);
      testResult.EnsureNotFailed ();
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.Ignored));
    }

    [Test]
    public void RunTestMethod_SetupFailed ()
    {
      var instance = new DummyTest5 ();
      var testMethod = typeof (DummyTest5).GetMethod ("TestSucceeded");
      var setupMethod = typeof (DummyTest5).GetMethod ("TestThrowsException");

      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, setupMethod, null);
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.FailedInSetUp));
    }

    [Test]
    public void RunTestMethod_TearDownFailed ()
    {
      var instance = new DummyTest5 ();
      var testMethod = typeof (DummyTest5).GetMethod ("TestSucceeded");
      var tearDownMethod = typeof (DummyTest5).GetMethod ("TestThrowsException");

      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, tearDownMethod);
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.FailedInTearDown));
    }

    [Test]
    public void RunTestMethod_ExpectedExceptionSucceded ()
    {
      var instance = new DummyTest5 ();
      var testMethod = typeof (DummyTest5).GetMethod ("TestExpectedExceptionSucceeded");
      
      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, null);
      testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestMethod_ExpectedExceptionFailed ()
    {
      var instance = new DummyTest5 ();
      var testMethod = typeof (DummyTest5).GetMethod ("TestExpectedExceptionFailed");

      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, null);
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.Failed));
    }

    [Test]
    public void RunTestMethod_TestSucceded ()
    {
      var instance = new DummyTest5 ();
      var testMethod = typeof (DummyTest5).GetMethod ("TestSucceeded");
      
      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, null);
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.Succeeded));
    }

    [Test]
    public void RunTestMethod_TestFailed()
    {
      var instance = new DummyTest5 ();
      var testMethod = typeof (DummyTest5).GetMethod ("TestFailed");

      var testResult = _sandboxTestRunner.RunTestMethod (instance, testMethod, null, null);
      Assert.That (testResult.Status, Is.EqualTo (SandboxTestStatus.Failed));
    }
  
  }
}
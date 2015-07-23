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
using Remotion.Reflection;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.NullSecurityClientTests
{
  [TestFixture]
  public class HasMethodAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private MethodInfo _methodInfo;
    private IMethodInformation _methodInformation;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
      _methodInfo = typeof (SecurableObject).GetMethod ("Show");
      _methodInformation = MockRepository.GenerateStub<IMethodInformation>();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, "Show");

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessGranted_WithMethodInfo ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessGranted_WithMethodInformation ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, "Show");
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithMethodInfo ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithMethodInformation ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInformation);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (null), "Show");
    
      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithMethodInfo ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (null), _methodInfo);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithMethodInformation ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (null), _methodInformation);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }
  }
}

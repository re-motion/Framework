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
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.NullSecurityClientTests
{
  [TestFixture]
  public class HasMethodAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private MethodInfo _methodInfo;
    private Mock<IMethodInformation> _methodInformation;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
      _methodInfo = typeof(SecurableObject).GetMethod("Show");
      _methodInformation = new Mock<IMethodInformation>();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      bool hasAccess = _securityClient.HasMethodAccess(_testHelper.SecurableObject, "Show");

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessGranted_WithMethodInfo ()
    {
      bool hasAccess = _securityClient.HasMethodAccess(_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessGranted_WithMethodInformation ()
    {
      bool hasAccess = _securityClient.HasMethodAccess(_testHelper.SecurableObject, _methodInformation.Object);

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasMethodAccess(_testHelper.SecurableObject, "Show");
      }

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithMethodInfo ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasMethodAccess(_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithMethodInformation ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasMethodAccess(_testHelper.SecurableObject, _methodInformation.Object);
      }

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      bool hasAccess = _securityClient.HasMethodAccess(new SecurableObject(null), "Show");

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithMethodInfo ()
    {
      bool hasAccess = _securityClient.HasMethodAccess(new SecurableObject(null), _methodInfo);

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithMethodInformation ()
    {
      bool hasAccess = _securityClient.HasMethodAccess(new SecurableObject(null), _methodInformation.Object);

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }
  }
}

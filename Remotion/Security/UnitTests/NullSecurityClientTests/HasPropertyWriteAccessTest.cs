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
  public class HasPropertyWriteAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private PropertyInfo _propertyInfo;
    private Mock<IPropertyInformation> _propertyInformation;
    private Mock<IMethodInformation> _methodInformation;

    [SetUp]
    public void SetUp()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatefulSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
      _propertyInfo = typeof (SecurableObject).GetProperty ("IsVisible");
      _propertyInformation = new Mock<IPropertyInformation>();
      _methodInformation = new Mock<IMethodInformation>();
      _propertyInformation.Setup (mock => mock.GetSetMethod (true)).Returns (_methodInformation.Object).Verifiable();
    }

    [Test]
    public void Test_AccessGranted()
    {
      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "IsVisible");

      Assert.That (hasAccess, Is.True);
      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_AccessGranted_WithPropertyInfo ()
    {
      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation.Object);

      Assert.That (hasAccess, Is.True);
      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessGranted_WithPropertyInformation ()
    {
      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation.Object);

      Assert.That (hasAccess, Is.True);
      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "IsVisible");
      }

      _testHelper.VerifyAll();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithPropertyInfo ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation.Object);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithPropertyInformation ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation.Object);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull()
    {
      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (null), "IsVisible");

      _testHelper.VerifyAll();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithPropertyInfo ()
    {
      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (null), _methodInformation.Object);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithPropertyInformation ()
    {
      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (null), _methodInformation.Object);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }
  }
}

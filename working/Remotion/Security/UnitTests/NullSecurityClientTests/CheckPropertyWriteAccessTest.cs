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
  public class CheckPropertyWriteAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private PropertyInfo _propertyInfo;
    private IPropertyInformation _propertyInformation;
    private IMethodInformation _methodInformation;

    [SetUp]
    public void SetUp()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatefulSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
      _propertyInfo = typeof (SecurableObject).GetProperty ("IsVisible");
      _propertyInformation = MockRepository.GenerateStub<IPropertyInformation>();
      _methodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _propertyInformation.Expect (mock => mock.GetSetMethod (true)).Return (_methodInformation);
    }

    [Test]
    public void Test_AccessGranted()
    {
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "IsVisible");

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_AccessGranted_WithPropertyInfo ()
    {
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessGranted_WithPropertyInformation ()
    {
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
      _testHelper.ReplayAll();

      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "IsVisible");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithPropertyInfo ()
    {
      _testHelper.ReplayAll ();

      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithPropertyInformation ()
    {
      _testHelper.ReplayAll ();

      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, _methodInformation);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull()
    {
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (null), "IsVisible");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithPropertyInfo ()
    {
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (null), _methodInformation);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithPropertyInformation ()
    {
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (null), _methodInformation);

      _testHelper.VerifyAll ();
    }
  }
}

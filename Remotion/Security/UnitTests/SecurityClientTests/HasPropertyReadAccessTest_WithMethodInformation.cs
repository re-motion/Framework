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
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasPropertyReadAccessTest_WithMethodInformation
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private IMethodInformation _methodInformation;
    
    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper ();
      _securityClient = _testHelper.CreateSecurityClient ();
      _methodInformation = MockRepository.GenerateMock<IMethodInformation>();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Read, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessDenied_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Read, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation);
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      
      _testHelper.ReplayAll ();

      Assert.That (
          () => _securityClient.HasPropertyReadAccess (new SecurableObject (null), _methodInformation),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("The securableObject did not return an IObjectSecurityStrategy."));
      
      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, (Enum[]) null);
      _testHelper.ReplayAll ();

      Assert.That (
          () => _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null."));
      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, (Enum[]) null);
      _testHelper.ReplayAll ();

      using (SecurityFreeSection.Activate())
      {
        Assert.That (
            () =>  _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, _methodInformation),
            Throws.InvalidOperationException
                .With.Message.EqualTo ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null."));
      }
    
      _testHelper.VerifyAll();
    }
  }
}
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
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasPropertyWriteAccessTest_WithMethodInfo
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private PropertyInfo _propertyInfo;
    private Mock<IMethodInformation> _methodInformation;
    private MethodInfo _methodInfo;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper ();
      _securityClient = _testHelper.CreateSecurityClient ();
      _propertyInfo = typeof (SecurableObject).GetProperty ("IsVisible");
      _methodInformation = new Mock<IMethodInformation>();
      _methodInfo = _propertyInfo.GetGetMethod ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object, TestAccessTypes.Second);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.Second, true);

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo);

      Assert.That (hasAccess, Is.True);
      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object, TestAccessTypes.Second);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.Second, false);

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object, TestAccessTypes.First);

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, true);

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessDenied_WithDefaultAccessType ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, false);

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object);

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo);
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
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object, TestAccessTypes.First);

      Assert.That (
          () => _securityClient.HasPropertyWriteAccess (new SecurableObject (null), _methodInfo),
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
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object, null);

      Assert.That (
          () => _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null."));
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException_WithPropertyInfo ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation.Object, null);

      using (SecurityFreeSection.Activate())
      {
        Assert.That (
            () => _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, _methodInfo),
            Throws.InvalidOperationException
                .With.Message.EqualTo ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null."));
      }

      _testHelper.VerifyAll();
    }
  }
}
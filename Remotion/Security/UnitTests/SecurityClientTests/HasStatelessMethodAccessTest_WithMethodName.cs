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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasStatelessMethodAccessTest_WithMethodName
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private Mock<IMethodInformation> _methodInformation;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper();
      _securityClient = _testHelper.CreateSecurityClient();
      _methodInformation = new Mock<IMethodInformation>();
      _methodInformation.Setup(n => n.Name).Returns("InstanceMethod");
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("InstanceMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object, TestAccessTypes.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess(TestAccessTypes.First, true);

      bool hasAccess = _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "InstanceMethod");

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("InstanceMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object, TestAccessTypes.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess(TestAccessTypes.First, false);

      bool hasAccess = _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "InstanceMethod");

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.False);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("InstanceMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object, TestAccessTypes.First);

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "InstanceMethod");
      }

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void Test_WithoutRequiredPermissions_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("InstanceMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object);

      Assert.That(
          () => _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "InstanceMethod"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The member 'InstanceMethod' does not define required permissions.", "requiredAccessTypeEnums"));
      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithoutRequiredPermissionsAndWithinSecurityFreeSection_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("InstanceMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object);

      using (SecurityFreeSection.Activate())
      {
        Assert.That(
            () => _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "InstanceMethod"),
            Throws.ArgumentException
                .With.ArgumentExceptionMessageEqualTo("The member 'InstanceMethod' does not define required permissions.", "requiredAccessTypeEnums"));
      }

      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("StaticMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object,  (Enum[])null);

      Assert.That(
          () => _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "StaticMethod"),
          Throws.InvalidOperationException
              .With.Message.EqualTo("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null."));
      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation("StaticMethod", MemberAffiliation.Instance, _methodInformation.Object);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions(_methodInformation.Object, (Enum[])null);

      using (SecurityFreeSection.Activate())
      {
        Assert.That(
            () => _securityClient.HasStatelessMethodAccess(typeof(SecurableObject), "StaticMethod"),
            Throws.InvalidOperationException
                .With.Message.EqualTo("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null."));
      }

      _testHelper.VerifyAll();
    }

  }
}

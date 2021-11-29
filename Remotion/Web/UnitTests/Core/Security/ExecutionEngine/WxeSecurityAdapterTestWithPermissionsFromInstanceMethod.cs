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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithPermissionsFromInstanceMethod
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private Mock<IObjectSecurityStrategy> _mockObjectSecurityStrategy;
    private Mock<IFunctionalSecurityStrategy> _mockFunctionalSecurityStrategy;
    private Mock<ISecurityProvider> _mockSecurityProvider;
    private Mock<IPrincipalProvider> _mockPrincipalProvider;
    private Mock<ISecurityPrincipal> _stubUser;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public WxeSecurityAdapterTestWithPermissionsFromInstanceMethod ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WxeSecurityAdapter();

      _mockSecurityProvider = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _mockSecurityProvider.Setup(_ => _.IsNull).Returns(false).Verifiable();
      _stubUser = new Mock<ISecurityPrincipal>();
      _stubUser.Setup(_ => _.User).Returns("user");
      _mockPrincipalProvider = new Mock<IPrincipalProvider>(MockBehavior.Strict);
      _mockPrincipalProvider.Setup(_ => _.GetPrincipal()).Returns(_stubUser.Object).Verifiable();

      _mockObjectSecurityStrategy = new Mock<IObjectSecurityStrategy>(MockBehavior.Strict);
      _mockFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy>(MockBehavior.Strict);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _mockSecurityProvider.Object);
      serviceLocator.RegisterSingle(() => _mockPrincipalProvider.Object);
      serviceLocator.RegisterSingle(() => _mockFunctionalSecurityStrategy.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void CheckAccess_AccessGranted ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, true);
      SecurableObject thisObject = new SecurableObject(_mockObjectSecurityStrategy.Object);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod(thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      _securityAdapter.CheckAccess(function);

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
    }

    [Test]
    public void CheckAccess_AccessDenied ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, false);
      SecurableObject thisObject = new SecurableObject(_mockObjectSecurityStrategy.Object);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod(thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
      Assert.That(
          () => _securityAdapter.CheckAccess(function),
          Throws.InstanceOf<PermissionDeniedException>());
    }

    [Test]
    public void CheckAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      SecurableObject thisObject = new SecurableObject(_mockObjectSecurityStrategy.Object);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod(thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      using (SecurityFreeSection.Activate())
      {
        _securityAdapter.CheckAccess(function);
      }

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _stubUser.Verify();
      _mockPrincipalProvider.Verify(_ => _.GetPrincipal(), Times.Never);
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, true);
      SecurableObject thisObject = new SecurableObject(_mockObjectSecurityStrategy.Object);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod(thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      bool hasAccess = _securityAdapter.HasAccess(function);

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccess_AccessDenied ()
    {
      ExpectObjectSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, false);
      SecurableObject thisObject = new SecurableObject(_mockObjectSecurityStrategy.Object);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod(thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      bool hasAccess = _securityAdapter.HasAccess(function);

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.False);
    }

    [Test]
    public void HasAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      SecurableObject thisObject = new SecurableObject(_mockObjectSecurityStrategy.Object);
      TestFunctionWithPermissionsFromInstanceMethod function = new TestFunctionWithPermissionsFromInstanceMethod(thisObject);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasAccess(function);
      }

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _stubUser.Verify();
      _mockPrincipalProvider.Verify(_ => _.GetPrincipal(), Times.Never);
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, true);

      bool hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithPermissionsFromInstanceMethod));

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Read, false);

      bool hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithPermissionsFromInstanceMethod));

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.False);
    }

    [Test]
    public void HasStatelessAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithPermissionsFromInstanceMethod));
      }

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _stubUser.Verify();
      _mockPrincipalProvider.Verify(_ => _.GetPrincipal(), Times.Never);
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    private void ExpectObjectSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      _mockObjectSecurityStrategy
          .Setup(
              _ => _.HasAccess(
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  new[] { AccessType.Get(accessTypeEnum) }))
          .Returns(returnValue)
          .Verifiable();
    }

    private void ExpectFunctionalSecurityStrategyHasAccessForSecurableObject (Enum accessTypeEnum, bool returnValue)
    {
      _mockFunctionalSecurityStrategy
          .Setup(
              _ => _.HasAccess(
                  typeof(SecurableObject),
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  new[] { AccessType.Get(accessTypeEnum) }))
          .Returns(returnValue)
          .Verifiable();
    }
  }
}

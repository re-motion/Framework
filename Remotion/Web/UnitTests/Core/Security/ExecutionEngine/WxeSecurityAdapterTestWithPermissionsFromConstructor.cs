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
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithPermissionsFromConstructor
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private Mock<IFunctionalSecurityStrategy> _mockFunctionalSecurityStrategy;
    private Mock<ISecurityProvider> _mockSecurityProvider;
    private Mock<IPrincipalProvider> _mockPrincipalProvider;
    private Mock<ISecurityPrincipal> _stubUser;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public WxeSecurityAdapterTestWithPermissionsFromConstructor ()
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
      _mockFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy>(MockBehavior.Strict);

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
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
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Create, true);

      _securityAdapter.CheckAccess(new TestFunctionWithPermissionsFromConstructor());

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
    }

    [Test]
    public void CheckAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Create, false);
      Assert.That(
          () => _securityAdapter.CheckAccess(new TestFunctionWithPermissionsFromConstructor()),
          Throws.InstanceOf<PermissionDeniedException>());
    }

    [Test]
    public void CheckAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      using (SecurityFreeSection.Activate())
      {
        _securityAdapter.CheckAccess(new TestFunctionWithPermissionsFromConstructor());
      }

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _stubUser.Verify();
      _mockPrincipalProvider.Verify(_ => _.GetPrincipal(), Times.Never);
      _mockFunctionalSecurityStrategy.Verify();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Create, true);

      bool hasAccess = _securityAdapter.HasAccess(new TestFunctionWithPermissionsFromConstructor());

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Create, false);

      bool hasAccess = _securityAdapter.HasAccess(new TestFunctionWithPermissionsFromConstructor());

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.False);
    }

    [Test]
    public void HasAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasAccess(new TestFunctionWithPermissionsFromConstructor());
      }

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _stubUser.Verify();
      _mockPrincipalProvider.Verify(_ => _.GetPrincipal(), Times.Never);
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Create, true);

      bool hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithPermissionsFromConstructor));

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessDenied ()
    {
      ExpectFunctionalSecurityStrategyHasAccessForSecurableObject(GeneralAccessTypes.Create, false);

      bool hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithPermissionsFromConstructor));

      _mockSecurityProvider.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.False);
    }

    [Test]
    public void HasStatelessAccess_WithinSecurityFreeSection_AccessGranted ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithPermissionsFromConstructor));
      }

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _stubUser.Verify();
      _mockPrincipalProvider.Verify(_ => _.GetPrincipal(), Times.Never);
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
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

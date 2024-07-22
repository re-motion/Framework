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

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithoutPermissionAttribute
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private Mock<IFunctionalSecurityStrategy> _mockFunctionalSecurityStrategy;
    private Mock<ISecurityProvider> _mockSecurityProvider;
    private Mock<IPrincipalProvider> _mockPrincipalProvider;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public WxeSecurityAdapterTestWithoutPermissionAttribute ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WxeSecurityAdapter();

      _mockSecurityProvider = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _mockSecurityProvider.Setup(_ => _.IsNull).Returns(false).Verifiable();
      _mockPrincipalProvider = new Mock<IPrincipalProvider>(MockBehavior.Strict);
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
      _securityAdapter.CheckAccess(new TestFunctionWithoutPermissions());

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      bool hasAccess = _securityAdapter.HasAccess(new TestFunctionWithoutPermissions());

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      bool hasAccess = _securityAdapter.HasStatelessAccess(typeof(TestFunctionWithoutPermissions));

      _mockSecurityProvider.Verify(_ => _.IsNull, Times.Never);
      _mockPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      Assert.That(hasAccess, Is.True);
    }
  }
}

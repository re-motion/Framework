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
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CreateSecurityClientFromConfiguration
  {
    private Mock<ISecurityProvider> _stubSecurityProvider;
    private Mock<IPrincipalProvider> _stubPrincipalProvider;
    private Mock<IPermissionProvider> _stubPermissionProvider;
    private Mock<IMemberResolver> _stubMemberResolver;
    private Mock<IFunctionalSecurityStrategy> _stubFunctionalSecurityStrategy;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      _stubSecurityProvider = new Mock<ISecurityProvider>();
      _stubPrincipalProvider = new Mock<IPrincipalProvider>();
      _stubPermissionProvider = new Mock<IPermissionProvider>();
      _stubMemberResolver = new Mock<IMemberResolver>();
      _stubFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _stubSecurityProvider.Object);
      serviceLocator.RegisterSingle(() => _stubPrincipalProvider.Object);
      serviceLocator.RegisterSingle(() => _stubPermissionProvider.Object);
      serviceLocator.RegisterSingle(() => _stubMemberResolver.Object);
      serviceLocator.RegisterSingle(() => _stubFunctionalSecurityStrategy.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void Create_ReturnsSecurityClientWithMembersFromServiceLocator ()
    {
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();

      Assert.That(securityClient, Is.InstanceOf(typeof(SecurityClient)));
      Assert.That(securityClient.SecurityProvider , Is.SameAs(_stubSecurityProvider.Object));
      Assert.That(securityClient.PrincipalProvider , Is.SameAs(_stubPrincipalProvider.Object));
      Assert.That(securityClient.PermissionProvider , Is.SameAs(_stubPermissionProvider.Object));
      Assert.That(securityClient.MemberResolver , Is.SameAs(_stubMemberResolver.Object));
      Assert.That(securityClient.FunctionalSecurityStrategy , Is.SameAs(_stubFunctionalSecurityStrategy.Object));
    }

    [Test]
    public void Create_WithNullSecurityProvider_ReturnsNullObject ()
    {
      _stubSecurityProvider.Setup(_ => _.IsNull).Returns(true);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();

      Assert.That(securityClient, Is.SameAs(SecurityClient.Null));
    }
  }
}

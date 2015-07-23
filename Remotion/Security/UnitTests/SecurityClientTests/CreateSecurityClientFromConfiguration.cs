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
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CreateSecurityClientFromConfiguration
  {
    private ISecurityProvider _stubSecurityProvider;
    private IPrincipalProvider _stubPrincipalProvider;
    private IPermissionProvider _stubPermissionProvider;
    private IMemberResolver _stubMemberResolver;
    private IFunctionalSecurityStrategy _stubFunctionalSecurityStrategy;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      _stubSecurityProvider = MockRepository.GenerateStub<ISecurityProvider>();
      _stubPrincipalProvider = MockRepository.GenerateStub<IPrincipalProvider>();
      _stubPermissionProvider = MockRepository.GenerateStub<IPermissionProvider>();
      _stubMemberResolver = MockRepository.GenerateStub<IMemberResolver>();
      _stubFunctionalSecurityStrategy = MockRepository.GenerateStub<IFunctionalSecurityStrategy>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _stubSecurityProvider);
      serviceLocator.RegisterSingle (() => _stubPrincipalProvider);
      serviceLocator.RegisterSingle (() => _stubPermissionProvider);
      serviceLocator.RegisterSingle (() => _stubMemberResolver);
      serviceLocator.RegisterSingle (() => _stubFunctionalSecurityStrategy);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void Create_WithEnabledAccessChecks_ReturnsSecurityClientWithMembersFromServiceLocator ()
    {
      Assert.That (SecurityConfiguration.Current.DisableAccessChecks, Is.False);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();

      Assert.IsInstanceOf (typeof (SecurityClient), securityClient);
      Assert.That (securityClient.SecurityProvider , Is.SameAs (_stubSecurityProvider));
      Assert.That (securityClient.PrincipalProvider , Is.SameAs (_stubPrincipalProvider));
      Assert.That (securityClient.PermissionProvider , Is.SameAs (_stubPermissionProvider));
      Assert.That (securityClient.MemberResolver , Is.SameAs (_stubMemberResolver));
      Assert.That (securityClient.FunctionalSecurityStrategy , Is.SameAs (_stubFunctionalSecurityStrategy));
    }

    [Test]
    public void Create_WithDisabledAccessChecks_ReturnsNullObject ()
    {
      bool backupValue = SecurityConfiguration.Current.DisableAccessChecks;
      try
      {
        SecurityConfiguration.Current.DisableAccessChecks = true;

        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();

        Assert.That (securityClient, Is.SameAs (SecurityClient.Null));
      }
      finally
      {
        SecurityConfiguration.Current.DisableAccessChecks = backupValue;
      }
    }

    [Test]
    public void Create_WithNullSecurityProvider_ReturnsNullObject ()
    {
      Assert.That (SecurityConfiguration.Current.DisableAccessChecks, Is.False);
      _stubSecurityProvider.Stub (_ => _.IsNull).Return (true);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();

      Assert.That (securityClient, Is.SameAs (SecurityClient.Null));
    }
  }
}
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
using Remotion.Collections.Caching;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.ObjectSecurityStrategyTests
{
  [TestFixture]
  public class HasAccessWithCaching_ObjectSecurityStrategyTest
  {
    private Mock<ISecurityProvider> _securityProviderMock;
    private Mock<ISecurityContextFactory> _securityContextFactoryStub;
    private Mock<ISecurityPrincipal> _principalStub;
    private SecurityContext _context;
    private InvalidationToken _invalidationToken;
    private IObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderMock = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      _principalStub = new Mock<ISecurityPrincipal>();
      _principalStub.Setup(_ => _.User).Returns("user");
      _context = SecurityContext.Create(typeof(SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);
      _securityContextFactoryStub.Setup(_ => _.CreateSecurityContext()).Returns(_context);

      _invalidationToken = InvalidationToken.Create();
      _strategy = ObjectSecurityStrategy.Create(_securityContextFactoryStub.Object, _invalidationToken);
    }

    [Test]
    public void HasAccess_WithAccessGranted_OnlyRequestsAccessTypesOnce_ReturnsTrue ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(new[]{AccessType.Get(GeneralAccessTypes.Read)})
          .Verifiable();

      bool hasAccessOnFirstCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccessOnFirstCall, Is.True);

      bool hasAccessOnSecondCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccessOnSecondCall, Is.True);

      _securityProviderMock.Verify(_ => _.GetAccess(_context, _principalStub.Object), Times.Once());
    }

    [Test]
    public void HasAccess_WithAccessDenied_OnlyRequestsAccessTypesOnce_ReturnsFalse ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(new[]{AccessType.Get(GeneralAccessTypes.Create)})
          .Verifiable();

      bool hasAccessOnFirstCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccessOnFirstCall, Is.False);

      bool hasAccessOnSecondCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccessOnSecondCall, Is.False);

      _securityProviderMock.Verify(_ => _.GetAccess(_context, _principalStub.Object), Times.Once());
    }

    [Test]
    public void HasAccess_WithAccessGranted_AndWithAccessDenied_OnlyRequestsAccessTypesOnce_ReturnsBooleanBasedOnRequestedAccess ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(new[]{AccessType.Get(GeneralAccessTypes.Create)})
          .Verifiable();

      bool hasAccessOnFirstCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccessOnFirstCall, Is.False);

      bool hasAccessOnSecondCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Create) });

      Assert.That(hasAccessOnSecondCall, Is.True);

      _securityProviderMock.Verify(_ => _.GetAccess(_context, _principalStub.Object), Times.Once());
    }

    [Test]
    public void HasAccess_WithCacheInvalidation_RequestsNewAccessTypes_ReturnsBooleanBasedOnRequestedAccess ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(new[]{AccessType.Get(GeneralAccessTypes.Read)})
          .Verifiable();

      bool hasAccessOnFirstCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccessOnFirstCall, Is.True);
      _securityProviderMock.Verify(_ => _.GetAccess(_context, _principalStub.Object), Times.Once());

      _invalidationToken.Invalidate();

      _securityProviderMock.Reset();
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(new[]{AccessType.Get(GeneralAccessTypes.Create)})
          .Verifiable();

      bool hasAccessOnSecondCall = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Create) });

      Assert.That(hasAccessOnSecondCall, Is.True);
      _securityProviderMock.Verify(_ => _.GetAccess(_context, _principalStub.Object), Times.Once());
    }

    [Test]
    public void CreateWithCustomCache_UsesCache ()
    {
      var cache = new Cache<ISecurityPrincipal, AccessType[]>();
      var strategy = ObjectSecurityStrategy.CreateWithCustomCache(_securityContextFactoryStub.Object, cache);

      _securityProviderMock
          .Setup(_ => _.GetAccess(It.IsAny<ISecurityContext>(), It.IsAny<ISecurityPrincipal>()))
          .Throws(new InvalidOperationException("Should not be called."))
          .Verifiable();

      cache.GetOrCreateValue(_principalStub.Object, key => new[] { AccessType.Get(GeneralAccessTypes.Read) });

      bool hasAccess = strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[] { AccessType.Get(GeneralAccessTypes.Read) });

      Assert.That(hasAccess, Is.True);
    }
  }
}

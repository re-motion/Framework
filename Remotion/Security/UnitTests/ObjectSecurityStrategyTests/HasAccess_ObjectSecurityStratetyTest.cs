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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.ObjectSecurityStrategyTests
{
  [TestFixture]
  public class HasAccess_ObjectSecurityStratetyTest
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
    public void HasAccess_WithRequiredAccessTypesMatchingAllowedAccessTypes_ReturnsTrue ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Create),
                  AccessType.Get(GeneralAccessTypes.Delete),
                  AccessType.Get(GeneralAccessTypes.Read)
              })
          .Verifiable();

      bool hasAccess = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[]
          {
              AccessType.Get(GeneralAccessTypes.Delete),
              AccessType.Get(GeneralAccessTypes.Create)
          });

      Assert.That(hasAccess, Is.True);
      _securityProviderMock.Verify();
    }

    [Test]
    public void HasAccess_WithoutRequiredAccessTypesMatchingAllowedAccessTypes_ReturnsFalse ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Create),
                  AccessType.Get(GeneralAccessTypes.Delete),
              })
          .Verifiable();
      bool hasAccess = _strategy.HasAccess(
          _securityProviderMock.Object,
          _principalStub.Object,
          new[]
          {
              AccessType.Get(GeneralAccessTypes.Create),
              AccessType.Get(GeneralAccessTypes.Delete),
              AccessType.Get(GeneralAccessTypes.Read)
          });

      Assert.That(hasAccess, Is.False);
      _securityProviderMock.Verify();
    }

    [Test]
    public void HasAccess_WithAllowedAccessTypesAreNull_ThrowsInvalidOperationException ()
    {
      _securityProviderMock.Setup(_ => _.GetAccess(_context, _principalStub.Object)).Returns((AccessType[])null).Verifiable();

      Assert.That(
          () => _strategy.HasAccess(
              _securityProviderMock.Object,
              _principalStub.Object,
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Find)
              }),
          Throws.InvalidOperationException.With.Message.EqualTo("GetAccess evaluated and returned null."));

      _securityProviderMock.Verify();
    }

    [Test]
    public void HasAccess_UsesSecurityFreeSectionWhileWhenCreatingSecurityContext ()
    {
      _securityContextFactoryStub.Reset();
      _securityContextFactoryStub
          .Setup(_ => _.CreateSecurityContext())
          .Returns(
              () =>
              {
                Assert.That(SecurityFreeSection.IsActive);
                return _context;
              });

      _securityProviderMock.Setup(_ => _.GetAccess(_context, _principalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Edit) }).Verifiable();

      bool hasAccess = _strategy.HasAccess(_securityProviderMock.Object, _principalStub.Object, new[] { AccessType.Get(GeneralAccessTypes.Edit) });

      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccess_WithRequiredAccessTypesEmpty_ThrowsArgumentException ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(_context, _principalStub.Object))
          .Returns(
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Read)
              })
          .Verifiable();

      Assert.That(
          () => _strategy.HasAccess(_securityProviderMock.Object, _principalStub.Object, new AccessType[0]),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Parameter 'requiredAccessTypes' cannot be empty.", "requiredAccessTypes"));
    }
  }
}

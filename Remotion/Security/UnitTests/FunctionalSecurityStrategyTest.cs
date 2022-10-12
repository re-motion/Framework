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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class FunctionalSecurityStrategyTest
  {
    private Mock<ISecurityProvider> _securityProviderMock;
    private Mock<ISecurityPrincipal> _principalStub;
    private IFunctionalSecurityStrategy _strategy;
    private Type _securableType;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderMock = new Mock<ISecurityProvider>(MockBehavior.Strict);

      _principalStub = new Mock<ISecurityPrincipal>();
      _principalStub.Setup(_ => _.User).Returns("user");

      _securableType = typeof(SecurableObject);

      _strategy = new FunctionalSecurityStrategy();
    }

    [Test]
    public void HasAccess_WithRequiredAccessTypesMatchingAllowedAccessTypes_ReturnsTrue ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(SecurityContext.CreateStateless(_securableType), _principalStub.Object))
          .Returns(
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Create),
                  AccessType.Get(GeneralAccessTypes.Delete),
                  AccessType.Get(GeneralAccessTypes.Read)
              })
          .Verifiable();

      bool hasAccess = _strategy.HasAccess(
          _securableType,
          _securityProviderMock.Object,
          _principalStub.Object,
          new[]
          {
              AccessType.Get(GeneralAccessTypes.Delete),
              AccessType.Get(GeneralAccessTypes.Create)
          });

      Assert.That(hasAccess, Is.EqualTo(true));
      _securityProviderMock.Verify();
    }

    [Test]
    public void HasAccess_WithoutRequiredAccessTypesMatchingAllowedAccessTypes_ReturnsFalse ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(SecurityContext.CreateStateless(_securableType), _principalStub.Object))
          .Returns(
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Delete),
                  AccessType.Get(GeneralAccessTypes.Read)
              })
          .Verifiable();

      bool hasAccess = _strategy.HasAccess(
          _securableType,
          _securityProviderMock.Object,
          _principalStub.Object,
          new[]
          {
              AccessType.Get(GeneralAccessTypes.Create),
              AccessType.Get(GeneralAccessTypes.Delete),
              AccessType.Get(GeneralAccessTypes.Read)
          });

      Assert.That(hasAccess, Is.EqualTo(false));
      _securityProviderMock.Verify();
    }

    [Test]
    public void HasAccess_WithAllowedAccessTypesAreNull_ThrowsInvalidOperationException ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(SecurityContext.CreateStateless(_securableType), _principalStub.Object))
          .Returns((AccessType[])null)
          .Verifiable();

      Assert.That(
          () => _strategy.HasAccess(
              _securableType,
              _securityProviderMock.Object,
              _principalStub.Object,
              new[] { AccessType.Get(GeneralAccessTypes.Find) }),
          Throws.InvalidOperationException.With.Message.EqualTo("GetAccess evaluated and returned null."));

      _securityProviderMock.Verify();
    }

    [Test]
    public void HasAccess_WithRequiredAccessTypesEmpty_ThrowsArgumentException ()
    {
      _securityProviderMock
          .Setup(_ => _.GetAccess(SecurityContext.CreateStateless(_securableType), _principalStub.Object))
          .Returns(
              new[]
              {
                  AccessType.Get(GeneralAccessTypes.Read)
              })
          .Verifiable();

      Assert.That(
          () => _strategy.HasAccess(_securableType, _securityProviderMock.Object, _principalStub.Object, new AccessType[0]),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Parameter 'requiredAccessTypes' cannot be empty.", "requiredAccessTypes"));
    }

    [Test]
    public void IsNull_ReturnsFalse ()
    {
      Assert.That(_strategy.IsNull, Is.False);
    }
  }
}

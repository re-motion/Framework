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
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecoratorTest
  {
    private Mock<ISecurityProvider> _securityProviderStub;
    private Mock<ISecurityPrincipal> _principalStub;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderStub = new Mock<ISecurityProvider>();

      _principalStub = new Mock<ISecurityPrincipal>();
      _principalStub.Setup(_ => _.User).Returns("user");
    }

    [Test]
    public void HasAccess_DelegatesToDecoratedStrategy_ReturnsResult ()
    {
      var objectSecurityStrategyStub = new Mock<IObjectSecurityStrategy>();
      var guard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator(objectSecurityStrategyStub.Object);
      var accessTypes = new[] { AccessType.Get(GeneralAccessTypes.Find) };

      bool expectedResult = BooleanObjectMother.GetRandomBoolean();
      bool securityStrategyWasCalled = false;
      objectSecurityStrategyStub
          .Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypes))
          .Returns(expectedResult)
          .Callback((ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) => { securityStrategyWasCalled = true; });

      var result = guard.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypes);

      Assert.That(result, Is.EqualTo(expectedResult));
      Assert.That(securityStrategyWasCalled, Is.True);
    }

    [Test]
    public void HasAccess_WithReEntrancyOnSameGuard_ThrowsInvalidOperationException ()
    {
      var objectSecurityStrategyStub = new Mock<IObjectSecurityStrategy>();
      var guard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator(objectSecurityStrategyStub.Object);
      var accessTypesOnFirstCall = new[] { AccessType.Get(GeneralAccessTypes.Find) };

      bool isExceptionThrownBySecondHasAccess = false;
      objectSecurityStrategyStub
          .Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnFirstCall))
          .Returns(false)
          .Callback(
              (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) =>
              {
                var exception = Assert.Throws<InvalidOperationException>(
                    () => guard.HasAccess(_securityProviderStub.Object, _principalStub.Object, new[] { AccessType.Get(GeneralAccessTypes.Read) }));
                isExceptionThrownBySecondHasAccess = true;
                throw exception;
              });

      Assert.That(
          () => guard.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnFirstCall),
          Throws.InvalidOperationException
              .With.Message.StartsWith(
                  "Multiple reentrancies on InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator.HasAccess(...) are not allowed as they can indicate a possible infinite recursion."));

      Assert.That(isExceptionThrownBySecondHasAccess, Is.True);
    }

    [Test]
    public void HasAccess_WithReEntrancyOnDifferentGuard_ReturnsResult ()
    {
      var firstObjectSecurityStrategyStub = new Mock<IObjectSecurityStrategy>();
      var firstGuard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator(firstObjectSecurityStrategyStub.Object);
      var accessTypesOnFirstCall = new[] { AccessType.Get(GeneralAccessTypes.Find) };
      bool expectedResultOnFirstCall = BooleanObjectMother.GetRandomBoolean();

      bool secondCallWasPerformed = false;
      firstObjectSecurityStrategyStub
          .Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnFirstCall))
          .Returns(expectedResultOnFirstCall)
          .Callback(
              (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) =>
              {
                var secondObjectSecurityStrategyStub = new Mock<IObjectSecurityStrategy>();
                var secondGuard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator(secondObjectSecurityStrategyStub.Object);
                var accessTypesOnSecondCall = new[] { AccessType.Get(GeneralAccessTypes.Read) };
                bool expectedResultOnSecondCall = BooleanObjectMother.GetRandomBoolean();

                secondObjectSecurityStrategyStub
                    .Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnSecondCall))
                    .Returns(expectedResultOnSecondCall);

                Assert.That(
                    secondGuard.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnSecondCall),
                    Is.EqualTo(expectedResultOnSecondCall));

                secondCallWasPerformed = true;
              });

      Assert.That(
          firstGuard.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnFirstCall),
          Is.EqualTo(expectedResultOnFirstCall));

      Assert.That(secondCallWasPerformed, Is.True);
    }

    [Test]
    public void HasAccess_WithExceptionDuringDecoratedCall_ResetsReentrancyForSubsequentCalls ()
    {
      var objectSecurityStrategyStub = new Mock<IObjectSecurityStrategy>();
      var guard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator(objectSecurityStrategyStub.Object);

      var accessTypesOnFirstCall = new[] { AccessType.Get(GeneralAccessTypes.Find) };
      var exception = new Exception();
      objectSecurityStrategyStub.Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnFirstCall)).Throws(exception);

      var accessTypesOnSecondCall = new[] { AccessType.Get(GeneralAccessTypes.Read) };
      bool expectedResult = BooleanObjectMother.GetRandomBoolean();
      objectSecurityStrategyStub.Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnSecondCall)).Returns(expectedResult);

      Assert.That(() => guard.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnFirstCall), Throws.Exception.SameAs(exception));
      Assert.That(guard.HasAccess(_securityProviderStub.Object, _principalStub.Object, accessTypesOnSecondCall), Is.EqualTo(expectedResult));
    }
  }
}
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
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecoratorTest
  {
    private ISecurityProvider _securityProviderStub;
    private ISecurityPrincipal _principalStub;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      _principalStub.Stub (_ => _.User).Return ("user");
    }

    [Test]
    public void HasAccess_DelegatesToDecoratedStrategy_ReturnsResult ()
    {
      var objectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy>();
      var guard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator (objectSecurityStrategyStub);
      var accessTypes = new[] { AccessType.Get (GeneralAccessTypes.Find) };

      bool expectedResult = BooleanObjectMother.GetRandomBoolean();
      bool securityStrategyWasCalled = false;
      objectSecurityStrategyStub
          .Stub (_ => _.HasAccess (_securityProviderStub, _principalStub, accessTypes))
          .Return (expectedResult)
          .WhenCalled (mi => { securityStrategyWasCalled = true; });

      var result = guard.HasAccess (_securityProviderStub, _principalStub, accessTypes);

      Assert.That (result, Is.EqualTo (expectedResult));
      Assert.That (securityStrategyWasCalled, Is.True);
    }

    [Test]
    public void HasAccess_WithReEntrancyOnSameGuard_ThrowsInvalidOperationException ()
    {
      var objectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy>();
      var guard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator (objectSecurityStrategyStub);
      var accessTypesOnFirstCall = new[] { AccessType.Get (GeneralAccessTypes.Find) };

      bool isExceptionThrownBySecondHasAccess = false;
      objectSecurityStrategyStub
          .Stub (_ => _.HasAccess (_securityProviderStub, _principalStub, accessTypesOnFirstCall))
          .Return (false)
          .WhenCalled (
              mi =>
              {
                var exception = Assert.Throws<InvalidOperationException> (
                    () => guard.HasAccess (_securityProviderStub, _principalStub, new[] { AccessType.Get (GeneralAccessTypes.Read) }));
                isExceptionThrownBySecondHasAccess = true;
                throw exception;
              });

      Assert.That (
          () => guard.HasAccess (_securityProviderStub, _principalStub, accessTypesOnFirstCall),
          Throws.InvalidOperationException
              .With.Message.StartsWith (
                  "Multiple reentrancies on InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator.HasAccess(...) are not allowed as they can indicate a possible infinite recursion."));

      Assert.That (isExceptionThrownBySecondHasAccess, Is.True);
    }

    [Test]
    public void HasAccess_WithReEntrancyOnDifferentGuard_ReturnsResult ()
    {
      var firstObjectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy>();
      var firstGuard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator (firstObjectSecurityStrategyStub);
      var accessTypesOnFirstCall = new[] { AccessType.Get (GeneralAccessTypes.Find) };
      bool expectedResultOnFirstCall = BooleanObjectMother.GetRandomBoolean();

      bool secondCallWasPerformed = false;
      firstObjectSecurityStrategyStub
          .Stub (_ => _.HasAccess (_securityProviderStub, _principalStub, accessTypesOnFirstCall))
          .Return (expectedResultOnFirstCall)
          .WhenCalled (
              mi =>
              {
                var secondObjectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy>();
                var secondGuard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator (secondObjectSecurityStrategyStub);
                var accessTypesOnSecondCall = new[] { AccessType.Get (GeneralAccessTypes.Read) };
                bool expectedResultOnSecondCall = BooleanObjectMother.GetRandomBoolean();

                secondObjectSecurityStrategyStub
                    .Stub (_ => _.HasAccess (_securityProviderStub, _principalStub, accessTypesOnSecondCall))
                    .Return (expectedResultOnSecondCall);

                Assert.That (
                    secondGuard.HasAccess (_securityProviderStub, _principalStub, accessTypesOnSecondCall),
                    Is.EqualTo (expectedResultOnSecondCall));

                secondCallWasPerformed = true;
              });

      Assert.That (
          firstGuard.HasAccess (_securityProviderStub, _principalStub, accessTypesOnFirstCall),
          Is.EqualTo (expectedResultOnFirstCall));

      Assert.That (secondCallWasPerformed, Is.True);
    }

    [Test]
    public void HasAccess_WithExceptionDuringDecoratedCall_ResetsReentrancyForSubsequentCalls ()
    {
      var objectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy>();
      var guard = new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator (objectSecurityStrategyStub);

      var accessTypesOnFirstCall = new[] { AccessType.Get (GeneralAccessTypes.Find) };
      var exception = new Exception();
      objectSecurityStrategyStub.Stub (_ => _.HasAccess (_securityProviderStub, _principalStub, accessTypesOnFirstCall)).Throw (exception);

      var accessTypesOnSecondCall = new[] { AccessType.Get (GeneralAccessTypes.Read) };
      bool expectedResult = BooleanObjectMother.GetRandomBoolean();
      objectSecurityStrategyStub.Stub (_ => _.HasAccess (_securityProviderStub, _principalStub, accessTypesOnSecondCall)).Return (expectedResult);

      Assert.That (() => guard.HasAccess (_securityProviderStub, _principalStub, accessTypesOnFirstCall), Throws.Exception.SameAs (exception));
      Assert.That (guard.HasAccess (_securityProviderStub, _principalStub, accessTypesOnSecondCall), Is.EqualTo (expectedResult));
    }
  }
}
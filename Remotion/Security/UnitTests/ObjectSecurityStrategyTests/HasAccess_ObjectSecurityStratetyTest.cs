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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.ObjectSecurityStrategyTests
{
  [TestFixture]
  public class HasAccess_ObjectSecurityStratetyTest
  {
    private ISecurityProvider _securityProviderMock;
    private ISecurityContextFactory _securityContextFactoryStub;
    private ISecurityPrincipal _principalStub;
    private SecurityContext _context;
    private InvalidationToken _invalidationToken;
    private IObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderMock = MockRepository.GenerateStrictMock<ISecurityProvider>();
      _securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      _principalStub.Stub (_ => _.User).Return ("user");
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);
      _securityContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      _invalidationToken = InvalidationToken.Create();
      _strategy = ObjectSecurityStrategy.Create (_securityContextFactoryStub, _invalidationToken);
    }

    [Test]
    public void HasAccess_WithRequiredAccessTypesMatchingAllowedAccessTypes_ReturnsTrue ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Create),
                  AccessType.Get (GeneralAccessTypes.Delete),
                  AccessType.Get (GeneralAccessTypes.Read)
              });

      bool hasAccess = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Create)
          });

      Assert.That (hasAccess, Is.True);
      _securityProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_WithoutRequiredAccessTypesMatchingAllowedAccessTypes_ReturnsFalse ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Create),
                  AccessType.Get (GeneralAccessTypes.Delete),
              });
      bool hasAccess = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          });

      Assert.That (hasAccess, Is.False);
      _securityProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_WithAllowedAccessTypesAreNull_ThrowsInvalidOperationException ()
    {
      _securityProviderMock.Expect (_ => _.GetAccess (_context, _principalStub)).Return (null);

      Assert.That (
          () => _strategy.HasAccess (
              _securityProviderMock,
              _principalStub,
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Find)
              }),
          Throws.InvalidOperationException.With.Message.EqualTo ("GetAccess evaluated and returned null."));

      _securityProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_UsesSecurityFreeSectionWhileWhenCreatingSecurityContext ()
    {
      _securityContextFactoryStub.BackToRecord();
      _securityContextFactoryStub
          .Stub (_ => _.CreateSecurityContext())
          .Return (_context)
          .WhenCalled (
              mi =>
              {
                Assert.That (SecurityFreeSection.IsActive);
                mi.ReturnValue = _context;
              });
      _securityContextFactoryStub.Replay();

      _securityProviderMock.Expect (_ => _.GetAccess (_context, _principalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Edit) });

      bool hasAccess = _strategy.HasAccess (_securityProviderMock, _principalStub, new[] { AccessType.Get (GeneralAccessTypes.Edit) });

      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void HasAccess_WithRequiredAccessTypesEmpty_ThrowsArgumentException ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Read)
              });

      Assert.That (
          () => _strategy.HasAccess (_securityProviderMock, _principalStub, new AccessType[0]),
          Throws.ArgumentException.With.Message.EqualTo ("Parameter 'requiredAccessTypes' cannot be empty.\r\nParameter name: requiredAccessTypes"));
    }
  }
}
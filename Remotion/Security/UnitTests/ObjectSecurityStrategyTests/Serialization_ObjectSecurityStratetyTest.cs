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
using Remotion.Development.UnitTesting;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.ObjectSecurityStrategyTests
{
  [TestFixture]
  public class Serialization_ObjectSecurityStratetyTest
  {
    [Serializable]
    private class SerializableSecurityContextFactory : ISecurityContextFactory
    {
      private readonly ISecurityContext _securityContext;

      public SerializableSecurityContextFactory (ISecurityContext securityContext)
      {
        _securityContext = securityContext;
      }

      public ISecurityContext CreateSecurityContext ()
      {
        return _securityContext;
      }
    }

    private SecurityContext _context;
    private InvalidationToken _invalidationToken;
    private IObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);

      _invalidationToken = InvalidationToken.Create();
      _strategy = ObjectSecurityStrategy.Create (new SerializableSecurityContextFactory (_context), _invalidationToken);
    }

    [Test]
    public void Serialization ()
    {
      var securityProviderMock = MockRepository.GenerateStrictMock<ISecurityProvider>();
      securityProviderMock
          .Expect (_ => _.GetAccess (_context, new SecurityPrincipal ("foo", null, null, null)))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) })
          .Repeat.Once();

      bool hasAccess = _strategy.HasAccess (
          securityProviderMock,
          new SecurityPrincipal ("foo", null, null, null),
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccess, Is.True);

      var deserializedStrategy = Serializer.SerializeAndDeserialize (_strategy);
      Assert.That (deserializedStrategy, Is.Not.SameAs (_strategy));

      bool hasAccessAfterDeserialization = _strategy.HasAccess (
          MockRepository.GenerateStrictMock<ISecurityProvider>(),
          new SecurityPrincipal ("foo", null, null, null),
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessAfterDeserialization, Is.True);
    }
  }
}
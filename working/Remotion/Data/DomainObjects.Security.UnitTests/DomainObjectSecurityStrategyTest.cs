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
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Security.UnitTests
{
  [TestFixture]
  public class DomainObjectSecurityStrategyDecoratorTest : TestBase
  {
    private MockRepository _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityProvider _stubSecurityProvider;
    private IDomainObjectSecurityContextFactory _stubContextFactory;
    private ISecurityPrincipal _stubUser;
    private AccessType[] _accessTypeResult;

    public override void SetUp ()
    {
      base.SetUp();

      _mocks = new MockRepository();
      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy>();
      _stubSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      _stubContextFactory = _mocks.StrictMock<IDomainObjectSecurityContextFactory>();

      _stubUser = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_stubUser.User).Return ("user");
      _accessTypeResult = new[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };
    }

    [Test]
    public void Initialize ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.New);
      Assert.That (strategy.InnerStrategy, Is.SameAs (_mockObjectSecurityStrategy));
      Assert.That (strategy.SecurityContextFactory, Is.SameAs (_stubContextFactory));
      Assert.That (strategy.RequiredSecurityForStates, Is.EqualTo (RequiredSecurityForStates.New));
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.None);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockObjectSecurityStrategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_StateIsNew ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.None);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_StateIsDeleted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.None);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_SecurityRequiredForNew ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.New);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockObjectSecurityStrategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_SecurityRequiredForDeleted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.Deleted);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_mockObjectSecurityStrategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_SecurityRequiredForNewAndDeleted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (
          _mockObjectSecurityStrategy,
          _stubContextFactory,
          RequiredSecurityForStates.NewAndDeleted);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_mockObjectSecurityStrategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithAccessGrantedAndStateIsDiscarded ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.None);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator (_mockObjectSecurityStrategy, _stubContextFactory, RequiredSecurityForStates.None);
      using (_mocks.Ordered())
      {
        Expect.Call (_stubContextFactory.IsInvalid).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockObjectSecurityStrategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult)).Return (false);
      }
      _mocks.ReplayAll();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Serializable]
    private class SerializableFactory : IDomainObjectSecurityContextFactory
    {
      public bool IsInvalid
      {
        get { return false; }
      }

      public bool IsNew
      {
        get { return false; }
      }

      public bool IsDeleted
      {
        get { return false; }
      }

      public ISecurityContext CreateSecurityContext ()
      {
        throw new NotImplementedException();
      }
    }

    [Serializable]
    private class SerializableObjectSecurityStrategy : IObjectSecurityStrategy
    {
      public bool HasAccess (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
      {
        return true;
      }
    }

    [Test]
    public void Serialize ()
    {
      IDomainObjectSecurityContextFactory factory = new SerializableFactory();
      IObjectSecurityStrategy objectSecurityStrategy = new SerializableObjectSecurityStrategy();

      var strategy = new DomainObjectSecurityStrategyDecorator (objectSecurityStrategy, factory, RequiredSecurityForStates.NewAndDeleted);
      var deserializedStrategy = Serializer.SerializeAndDeserialize (strategy);

      Assert.That (deserializedStrategy, Is.Not.SameAs (strategy));
      Assert.That (deserializedStrategy.RequiredSecurityForStates, Is.EqualTo (RequiredSecurityForStates.NewAndDeleted));
      Assert.That (deserializedStrategy.SecurityContextFactory, Is.Not.SameAs (factory));
      Assert.IsInstanceOf (typeof (SerializableFactory), deserializedStrategy.SecurityContextFactory);
      Assert.That (deserializedStrategy.InnerStrategy, Is.Not.SameAs (objectSecurityStrategy));
      Assert.IsInstanceOf (typeof (SerializableObjectSecurityStrategy), deserializedStrategy.InnerStrategy);
    }
  }
}
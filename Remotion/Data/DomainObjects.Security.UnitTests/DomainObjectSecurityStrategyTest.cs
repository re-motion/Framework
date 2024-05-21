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
using Remotion.Development.UnitTesting;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Security.UnitTests
{
  [TestFixture]
  public class DomainObjectSecurityStrategyDecoratorTest : TestBase
  {
    private Mock<IObjectSecurityStrategy> _mockObjectSecurityStrategy;
    private Mock<ISecurityProvider> _stubSecurityProvider;
    private Mock<IDomainObjectSecurityContextFactory> _stubContextFactory;
    private Mock<ISecurityPrincipal> _stubUser;
    private AccessType[] _accessTypeResult;

    public override void SetUp ()
    {
      base.SetUp();

      _mockObjectSecurityStrategy = new Mock<IObjectSecurityStrategy>(MockBehavior.Strict);
      _stubSecurityProvider = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _stubContextFactory = new Mock<IDomainObjectSecurityContextFactory>(MockBehavior.Strict);

      _stubUser = new Mock<ISecurityPrincipal>();
      _stubUser.Setup(_ => _.User).Returns("user");
      _accessTypeResult = new[] { AccessType.Get(GeneralAccessTypes.Read), AccessType.Get(GeneralAccessTypes.Edit) };
    }

    [Test]
    public void Initialize ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.New);
      Assert.That(strategy.InnerStrategy, Is.SameAs(_mockObjectSecurityStrategy.Object));
      Assert.That(strategy.SecurityContextFactory, Is.SameAs(_stubContextFactory.Object));
      Assert.That(strategy.RequiredSecurityForStates, Is.EqualTo(RequiredSecurityForStates.New));
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.None);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsNew).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsDeleted).Returns(false).Verifiable();
      _mockObjectSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult))
          .Returns(true)
          .Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_StateIsNew ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.None);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsNew).Returns(true).Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_StateIsDeleted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.None);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsNew).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsDeleted).Returns(true).Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_SecurityRequiredForNew ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.New);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsDeleted).Returns(false).Verifiable();
      _mockObjectSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult))
          .Returns(true)
          .Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_SecurityRequiredForDeleted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.Deleted);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsNew).Returns(false).Verifiable();
      _mockObjectSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult))
          .Returns(true)
          .Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_SecurityRequiredForNewAndDeleted ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(
          _mockObjectSecurityStrategy.Object,
          _stubContextFactory.Object,
          RequiredSecurityForStates.NewAndDeleted);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _mockObjectSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult))
          .Returns(true)
          .Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_WithAccessGrantedAndStateIsDiscarded ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.None);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(true).Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      var strategy = new DomainObjectSecurityStrategyDecorator(_mockObjectSecurityStrategy.Object, _stubContextFactory.Object, RequiredSecurityForStates.None);
      var sequence = new VerifiableSequence();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsInvalid).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsNew).Returns(false).Verifiable();
      _stubContextFactory.InVerifiableSequence(sequence).Setup(_ => _.IsDeleted).Returns(false).Verifiable();
      _mockObjectSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult))
          .Returns(false)
          .Verifiable();

      bool hasAccess = strategy.HasAccess(_stubSecurityProvider.Object, _stubUser.Object, _accessTypeResult);

      _mockObjectSecurityStrategy.Verify();
      _stubSecurityProvider.Verify();
      _stubContextFactory.Verify();
      sequence.Verify();
      Assert.That(hasAccess, Is.EqualTo(false));
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

      var strategy = new DomainObjectSecurityStrategyDecorator(objectSecurityStrategy, factory, RequiredSecurityForStates.NewAndDeleted);
      var deserializedStrategy = Serializer.SerializeAndDeserialize(strategy);

      Assert.That(deserializedStrategy, Is.Not.SameAs(strategy));
      Assert.That(deserializedStrategy.RequiredSecurityForStates, Is.EqualTo(RequiredSecurityForStates.NewAndDeleted));
      Assert.That(deserializedStrategy.SecurityContextFactory, Is.Not.SameAs(factory));
      Assert.That(deserializedStrategy.SecurityContextFactory, Is.InstanceOf(typeof(SerializableFactory)));
      Assert.That(deserializedStrategy.InnerStrategy, Is.Not.SameAs(objectSecurityStrategy));
      Assert.That(deserializedStrategy.InnerStrategy, Is.InstanceOf(typeof(SerializableObjectSecurityStrategy)));
    }
  }
}

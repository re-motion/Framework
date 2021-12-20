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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class InitializedEventDomainObjectTransactionContextDecoratorTest
  {
    [Test]
    public void AllowedMembers ()
    {
      var expected = ClientTransaction.CreateRootTransaction();

      var contextMock = new Mock<IDomainObjectTransactionContext>();
      contextMock.Setup(mock => mock.ClientTransaction).Returns(expected).Verifiable();

      var ctx = new InitializedEventDomainObjectTransactionContextDecorator(contextMock.Object);

      var result = ctx.ClientTransaction;

      contextMock.Verify();
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ForbiddenMembers_State ()
    {
      var contextMock = new Mock<IDomainObjectTransactionContext>();
      var ctx = new InitializedEventDomainObjectTransactionContextDecorator(contextMock.Object);

      Assert.That(
          () => ctx.State,
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the OnReferenceInitializing event is executing, this member cannot be used."));

      contextMock.VerifyGet(_ => _.State, Times.Never());
    }

    [Test]
    public void ForbiddenMembers_Timestamp ()
    {
      var contextMock = new Mock<IDomainObjectTransactionContext>();
      var ctx = new InitializedEventDomainObjectTransactionContextDecorator(contextMock.Object);

      Assert.That(
          () => ctx.Timestamp,
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the OnReferenceInitializing event is executing, this member cannot be used."));

      contextMock.VerifyGet(_ => _.Timestamp, Times.Never());
    }

    [Test]
    public void ForbiddenMembers_RegisterForCommit ()
    {
      var contextMock = new Mock<IDomainObjectTransactionContext>();
      var ctx = new InitializedEventDomainObjectTransactionContextDecorator(contextMock.Object);

      Assert.That(
          () => ctx.RegisterForCommit(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the OnReferenceInitializing event is executing, this member cannot be used."));

      contextMock.Verify(_ => _.RegisterForCommit(), Times.Never());
    }

    [Test]
    public void ForbiddenMembers_EnsureDataAvailable ()
    {
      var contextMock = new Mock<IDomainObjectTransactionContext>();
      var ctx = new InitializedEventDomainObjectTransactionContextDecorator(contextMock.Object);

      Assert.That(
          () => ctx.EnsureDataAvailable(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the OnReferenceInitializing event is executing, this member cannot be used."));

      contextMock.Verify(_ => _.EnsureDataAvailable(), Times.Never());
    }

    [Test]
    public void ForbiddenMembers_TryEnsureDataAvailable ()
    {
      var contextMock = new Mock<IDomainObjectTransactionContext>();
      var ctx = new InitializedEventDomainObjectTransactionContextDecorator(contextMock.Object);

      Assert.That(
          () => ctx.TryEnsureDataAvailable(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("While the OnReferenceInitializing event is executing, this member cannot be used."));

      contextMock.Verify(_ => _.TryEnsureDataAvailable(), Times.Never());
    }
  }
}

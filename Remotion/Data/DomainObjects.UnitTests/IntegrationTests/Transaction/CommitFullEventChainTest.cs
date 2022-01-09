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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class CommitFullEventChainTest : CommitRollbackFullEventChainTestBase
  {
    [Test]
    public void FullEventChain ()
    {
      var sequence = new MockSequence();
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null),
              (DeletedObject, DeletedObjectEventReceiverMock, null)
          });

      ExpectCommitValidateEvents(sequence, new[] { ChangedObject, NewObject, DeletedObject });

      ExpectCommittedEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null)
          });

      Assert.That(ClientTransaction.Current, Is.Not.SameAs(Transaction));

      Transaction.Commit();

      Assert.That(ClientTransaction.Current, Is.Not.SameAs(Transaction));
      VerifyAll();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InExtension ()
    {
      var sequence = new MockSequence();
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null),
              (DeletedObject, DeletedObjectEventReceiverMock, null)
          },
          extensionCallback: _ =>
          {
            // This triggers one additional run
            Transaction.ExecuteInScope(() => UnchangedObject.RegisterForCommit());
          });

      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (UnchangedObject, UnchangedObjectEventReceiverMock, null)
          },
          extensionCallback: _ =>
          {
            // This does not trigger an additional run because the object is no longer new to the commit set
            Transaction.ExecuteInScope(() => UnchangedObject.RegisterForCommit());
          });

      ExpectCommitValidateEvents(sequence, new[] { ChangedObject, NewObject, DeletedObject, UnchangedObject });

      ExpectCommittedEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null),
              (UnchangedObject, UnchangedObjectEventReceiverMock, null)
          });

      Transaction.Commit();

      VerifyAll();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InTransaction ()
    {
      var sequence = new MockSequence();
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null),
              (DeletedObject, DeletedObjectEventReceiverMock, null)
          },
          transactionCallback: _ =>
          {
            // This triggers one additional run
            Transaction.ExecuteInScope(() => UnchangedObject.RegisterForCommit());
          });

      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (UnchangedObject, UnchangedObjectEventReceiverMock, null)
          },
          transactionCallback: _ =>
          {
            // This does not trigger an additional run because the object is no longer new to the commit set
            Transaction.ExecuteInScope(() => UnchangedObject.RegisterForCommit());
          });

      ExpectCommitValidateEvents(sequence, new[] { ChangedObject, NewObject, DeletedObject, UnchangedObject });

      ExpectCommittedEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null),
              (UnchangedObject, UnchangedObjectEventReceiverMock, null)
          });

      Transaction.Commit();

      VerifyAll();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InDomainObject ()
    {
      var sequence = new MockSequence();
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, _ =>
              {
                // This triggers one additional run
                Transaction.ExecuteInScope(() => UnchangedObject.RegisterForCommit());
              }),
              (DeletedObject, DeletedObjectEventReceiverMock, null)
          });

      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (UnchangedObject, UnchangedObjectEventReceiverMock, _ =>
              {
                // This does not trigger an additional run because the object is no longer new to the commit set
                Transaction.ExecuteInScope(() => UnchangedObject.RegisterForCommit());
              })
          });

      ExpectCommitValidateEvents(sequence, new[] { ChangedObject, NewObject, DeletedObject, UnchangedObject });

      ExpectCommittedEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null),
              (UnchangedObject, UnchangedObjectEventReceiverMock, null)
          });

      Transaction.Commit();

      VerifyAll();
    }
  }
}

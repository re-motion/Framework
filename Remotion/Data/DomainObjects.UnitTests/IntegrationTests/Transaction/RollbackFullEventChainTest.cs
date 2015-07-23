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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackFullEventChainTest : CommitRollbackFullEventChainTestBase
  {
    [Test]
    public void FullEventChain ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectRollingBackEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock));

        ExpectRolledBackEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (Transaction));

      Transaction.Rollback ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (Transaction));
      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InExtension ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectRollingBackEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            .ExtensionOptions
          // This triggers one additional run
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectRollingBackEventsWithCustomOptions (
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock))
            .ExtensionOptions
          // This does not trigger an additional run because the object is no longer new to the Rollback set
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectRolledBackEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Rollback ();

      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InTransaction ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectRollingBackEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            .TransactionOptions
          // This triggers one additional run
            .WhenCalled (mi => RegisterForCommitWithDisabledListener (UnchangedObject));

        ExpectRollingBackEventsWithCustomOptions (
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock))
            .TransactionOptions
          // This does not trigger an additional run because the object is no longer new to the Rollback set
            .WhenCalled (mi => RegisterForCommitWithDisabledListener (UnchangedObject));

        ExpectRolledBackEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Rollback ();

      MockRepository.VerifyAll ();
    }

     [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InDomainObject ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectRollingBackEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            .DomainObjectOptions[1]
          // This triggers one additional run
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectRollingBackEventsWithCustomOptions (
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock))
            .DomainObjectOptions[0]
          // This does not trigger an additional run because the object is no longer new to the Rollback set
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectRolledBackEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Rollback ();

      MockRepository.VerifyAll ();
    }
  }
}
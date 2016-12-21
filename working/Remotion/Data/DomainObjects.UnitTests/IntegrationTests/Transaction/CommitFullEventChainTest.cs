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
  public class CommitFullEventChainTest : CommitRollbackFullEventChainTestBase
  {
    [Test]
    public void FullEventChain ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (Transaction));

      Transaction.Commit ();
      
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (Transaction));
      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InExtension ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            // This triggers one additional run
            .ExtensionOptions
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectCommittingEventsWithCustomOptions (Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock))
            // This does not trigger an additional run because the object is no longer new to the commit set
            .ExtensionOptions
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject, UnchangedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InTransaction ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
          // This triggers one additional run
            .TransactionOptions
            .WhenCalled (mi => RegisterForCommitWithDisabledListener(UnchangedObject));

        ExpectCommittingEventsWithCustomOptions (Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock))
          // This does not trigger an additional run because the object is no longer new to the commit set
            .TransactionOptions
            .WhenCalled (mi => RegisterForCommitWithDisabledListener(UnchangedObject));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject, UnchangedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject_InDomainObject ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
          // This triggers one additional run
            .DomainObjectOptions[1]
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectCommittingEventsWithCustomOptions (Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock))
          // This does not trigger an additional run because the object is no longer new to the commit set
            .DomainObjectOptions[0]
            .WhenCalled (mi => Transaction.ExecuteInScope (() => UnchangedObject.RegisterForCommit()));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject, UnchangedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }
  }
}
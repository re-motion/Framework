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
  public class RegisterForAdditionalCommittingEventsTest : CommitRollbackFullEventChainTestBase
  {
    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents_InExtension ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
             // This triggers _one_ (not two) additional run for _changedObject
            .ExtensionOptions.WhenCalled (mi => Transaction.ExecuteInScope (() =>
            {
              ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (ChangedObject);
              ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (ChangedObject);
            }));

        ExpectCommittingEventsWithCustomOptions (Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock))
            .ExtensionOptions
            // This triggers one additional run for _newObject
            .WhenCalled (
                mi => Transaction.ExecuteInScope (() => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (NewObject)));

        ExpectCommittingEventsWithCustomOptions (Tuple.Create (NewObject, NewObjectEventReceiverMock))
            .ExtensionOptions
            // This triggers one additional run for _newObject
            .WhenCalled (
                mi => Transaction.ExecuteInScope (() => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (NewObject)));

        // No more additional runs
        ExpectCommittingEvents (Tuple.Create (NewObject, NewObjectEventReceiverMock));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents_InClientTransaction ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            .TransactionOptions
          // This triggers an additional run for _changedObject
            .WhenCalled (
                mi =>
                {
                  var args = ((ClientTransactionCommittingEventArgs) mi.Arguments[1]);
                  Transaction.ExecuteInScope (() => args.EventRegistrar.RegisterForAdditionalCommittingEvents (ChangedObject));
                });

        // No more additional runs
        ExpectCommittingEvents (Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents_InDomainObject ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            .DomainObjectOptions[1]
          // This triggers an additional run for _deletedObject
            .WhenCalled (
                mi =>
                {
                  var args = ((DomainObjectCommittingEventArgs) mi.Arguments[1]);
                  Transaction.ExecuteInScope (() => args.EventRegistrar.RegisterForAdditionalCommittingEvents (DeletedObject));
                });

        // No more additional runs
        ExpectCommittingEvents (Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject);

        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }
    
    [Test]
    public void FullEventChain_WithReiterationDueToAddedObjectAndRegisterForAdditionalCommittingEvents ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEventsWithCustomOptions (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            .ExtensionOptions
            // This triggers _one_ (not two) additional run for _unchangedObject
            .WhenCalled (
                mi => Transaction.ExecuteInScope (
                    () =>
                    {
                      Assert.That (
                          () => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (UnchangedObject),
                          Throws.ArgumentException.With.Message.EqualTo (
                              string.Format (
                                  "The given DomainObject '{0}' cannot be registered due to its state (Unchanged). "
                                  + "Only objects that are part of the commit set can be registered. Use RegisterForCommit to add an unchanged "
                                  + "object to the commit set.\r\nParameter name: domainObjects",
                                  UnchangedObject.ID)));
                      UnchangedObject.RegisterForCommit();
                      ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (UnchangedObject);
                    }));

        // No more additional runs
        ExpectCommittingEvents (Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
        
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
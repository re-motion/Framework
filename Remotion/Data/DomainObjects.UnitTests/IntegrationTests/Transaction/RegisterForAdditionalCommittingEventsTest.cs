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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RegisterForAdditionalCommittingEventsTest : CommitRollbackFullEventChainTestBase
  {
    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents_InExtension ()
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
          extensionCallback: invocation =>
          {
            var committingEventRegistrar = (ICommittingEventRegistrar)invocation.Arguments[2];

            // This triggers _one_ (not two) additional run for _changedObject
            Transaction.ExecuteInScope(
                () =>
                {
                  committingEventRegistrar.RegisterForAdditionalCommittingEvents(ChangedObject);
                  committingEventRegistrar.RegisterForAdditionalCommittingEvents(ChangedObject);
                });
          });

      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null)
          },
          extensionCallback: invocation =>
          {
            var committingEventRegistrar = (ICommittingEventRegistrar)invocation.Arguments[2];

            // This triggers one additional run for _newObject
            Transaction.ExecuteInScope(() => committingEventRegistrar.RegisterForAdditionalCommittingEvents(NewObject));
          });

      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (NewObject, NewObjectEventReceiverMock, null)
          },
          extensionCallback: invocation =>
          {
            var committingEventRegistrar = (ICommittingEventRegistrar)invocation.Arguments[2];

            // This triggers one additional run for _newObject
            Transaction.ExecuteInScope(() => committingEventRegistrar.RegisterForAdditionalCommittingEvents(NewObject));
          });


      // No more additional runs
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (NewObject, NewObjectEventReceiverMock, null)
          });

      ExpectCommitValidateEvents(sequence, new[] { ChangedObject, NewObject, DeletedObject });

      ExpectCommittedEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null)
          });

      Transaction.Commit();

      VerifyAll();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents_InClientTransaction ()
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
          transactionCallback: invocation =>
          {
            var args = (ClientTransactionCommittingEventArgs)invocation.Arguments[1];

            // This triggers an additional run for _changedObject
            Transaction.ExecuteInScope(() => args.EventRegistrar.RegisterForAdditionalCommittingEvents(ChangedObject));
          });

      // No more additional runs
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null)
          });

      ExpectCommitValidateEvents(sequence, new[] { ChangedObject, NewObject, DeletedObject });

      ExpectCommittedEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, null)
          });

      Transaction.Commit();

      VerifyAll();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents_InDomainObject ()
    {
      var sequence = new MockSequence();
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (ChangedObject, ChangedObjectEventReceiverMock, null),
              (NewObject, NewObjectEventReceiverMock, invocation =>
              {
                var args = (DomainObjectCommittingEventArgs)invocation.Arguments[1];

                // This triggers an additional run for _deletedObject
                Transaction.ExecuteInScope(() => args.EventRegistrar.RegisterForAdditionalCommittingEvents(DeletedObject));
              }),
              (DeletedObject, DeletedObjectEventReceiverMock, null)
          });

      // No more additional runs
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
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

      Transaction.Commit();

      VerifyAll();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObjectAndRegisterForAdditionalCommittingEvents ()
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
          extensionCallback: invocation =>
          {
            var committingEventRegistrar = (ICommittingEventRegistrar)invocation.Arguments[2];

            // This triggers _one_ (not two) additional run for _unchangedObject
            Transaction.ExecuteInScope(
                () =>
                {
                  Assert.That(
                      () => committingEventRegistrar.RegisterForAdditionalCommittingEvents(UnchangedObject),
                      Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
                          string.Format(
                              "The given DomainObject '{0}' cannot be registered due to its DomainObjectState (Unchanged). "
                              + "Only objects that are part of the commit set can be registered. Use RegisterForCommit to add an unchanged "
                              + "object to the commit set.",
                              UnchangedObject.ID),
                          "domainObjects"));
                  UnchangedObject.RegisterForCommit();
                  committingEventRegistrar.RegisterForAdditionalCommittingEvents(UnchangedObject);
                });
          });

      // No more additional runs
      ExpectCommittingEvents(
          sequence,
          new (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[]
          {
              (UnchangedObject, UnchangedObjectEventReceiverMock, null)
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

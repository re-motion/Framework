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
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.FunctionalProgramming;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class RegisterForCommitTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject_NoOp ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      Assert.That (domainObject.State, Is.EqualTo (StateType.New));

      domainObject.RegisterForCommit();

      Assert.That (domainObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (domainObject);
    }

    [Test]
    public void ChangedObject_RemembersRegistration_EvenWhenChangedBack ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      ++domainObject.Int32Property;
      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));

      domainObject.RegisterForCommit ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (domainObject);

      --domainObject.Int32Property;
      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void UnchangedObject ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));

      domainObject.RegisterForCommit ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (domainObject);

      ++domainObject.Int32Property;
      --domainObject.Int32Property;

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void DeletedObject ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      domainObject.Delete();
      Assert.That (domainObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (() => domainObject.RegisterForCommit (), Throws.Nothing);

      Assert.That (domainObject.State, Is.EqualTo (StateType.Deleted));
      CheckNotMarkedAsChanged (domainObject);
    }

    [Test]
    public void InvalidObject ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      domainObject.Delete ();
      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));

      Assert.That (() => domainObject.RegisterForCommit (), Throws.TypeOf<ObjectInvalidException> ());

      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));

      ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, domainObject.ID);

      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void NotLoadedYetObject_LoadedToUnchanged ()
    {
      var domainObject = (ClassWithAllDataTypes) LifetimeService.GetObjectReference (TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));

      domainObject.RegisterForCommit();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
      CheckMarkedAsChanged (domainObject);
    }

    [Test]
    public void NotLoadedYetObject_LoadedToChanged ()
    {
      var domainObject = (ClassWithAllDataTypes) LifetimeService.GetObjectReference (TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      domainObject.ProtectedLoaded += (sender, args) => ++domainObject.Int32Property;
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));

      domainObject.RegisterForCommit ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
      CheckMarkedAsChanged (domainObject);
    }

    [Test]
    public void CommitRoot ()
    {
      var newObject = ClassWithAllDataTypes.NewObject ();
      newObject.DateTimeProperty = new DateTime (2012, 12, 12);
      newObject.DateProperty = new DateTime (2012, 12, 12);
      Assert.That (newObject.State, Is.EqualTo (StateType.New));

      var changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      ++changedObject.Int32Property;
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));

      var unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ();
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));

      newObject.RegisterForCommit();
      changedObject.RegisterForCommit();
      unchangedObject.RegisterForCommit();

      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (unchangedObject);

      var objectEventReceiverMock = MockRepository.GenerateMock<DomainObjectMockEventReceiver> (newObject);
      var transactionEventReceiverMock = MockRepository.GenerateMock<ClientTransactionMockEventReceiver> (TestableClientTransaction);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);
      
      SetDatabaseModifyable ();
      CommitTransactionAndCheckTimestamps (newObject, changedObject, unchangedObject);

      listenerMock.AssertWasCalled (
          mock => mock.TransactionCommitting (
              Arg.Is (TestableClientTransaction),
              Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { newObject, changedObject, unchangedObject }),
              Arg<ICommittingEventRegistrar>.Is.Anything));
      listenerMock.AssertWasCalled (
          mock => mock.TransactionCommitValidate (
              Arg.Is (TestableClientTransaction),
              Arg<ReadOnlyCollection<PersistableData>>.Matches (
                  x => x.Select (d => d.DomainObject).SetEquals (new[] { newObject, changedObject, unchangedObject }))));
      objectEventReceiverMock.AssertWasCalled (mock => mock.Committing ());
      objectEventReceiverMock.AssertWasCalled (mock => mock.Committed ());
      transactionEventReceiverMock.AssertWasCalled (mock => mock.Committing (newObject, changedObject, unchangedObject));
      transactionEventReceiverMock.AssertWasCalled (mock => mock.Committed (newObject, changedObject, unchangedObject));

      Assert.That (newObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (unchangedObject);
    }

    [Test]
    public void CommitRoot_RegisterForUnchanged_LeadsToConcurrencyCheck ()
    {
      var unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ();
      unchangedObject.RegisterForCommit ();

      SetDatabaseModifyable();
      ModifyAndCommitInOtherTransaction (unchangedObject.ID);

      Assert.That (() => TestableClientTransaction.Commit(), Throws.TypeOf<ConcurrencyViolationException>());
    }

    [Test]
    public void CommitSub ()
    {
      ClassWithAllDataTypes newObject;
      ClassWithAllDataTypes changedObject;
      ClassWithAllDataTypes unchangedObject;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newObject = ClassWithAllDataTypes.NewObject();
        Assert.That (newObject.State, Is.EqualTo (StateType.New));

        changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
        ++changedObject.Int32Property;
        Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));

        unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ();
        Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));

        newObject.RegisterForCommit();
        changedObject.RegisterForCommit();
        unchangedObject.RegisterForCommit();

        Assert.That (newObject.State, Is.EqualTo (StateType.New));
        CheckNotMarkedAsChanged (newObject);
        Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
        CheckMarkedAsChanged (changedObject);
        Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
        CheckMarkedAsChanged (unchangedObject);

        var objectEventReceiverMock = MockRepository.GenerateMock<DomainObjectMockEventReceiver> (newObject);
        var transactionEventReceiverMock = MockRepository.GenerateMock<ClientTransactionMockEventReceiver> (ClientTransaction.Current);
        var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (ClientTransaction.Current);
        try
        {
          ClientTransaction.Current.Commit();
        }
        finally
        {
          ClientTransactionTestHelper.RemoveListener (ClientTransaction.Current, listenerMock);
        }

        listenerMock.AssertWasCalled (
                      mock => mock.TransactionCommitting (
                          Arg.Is (ClientTransaction.Current),
                          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { newObject, changedObject, unchangedObject }),
                          Arg<ICommittingEventRegistrar>.Is.Anything));
        listenerMock.AssertWasCalled (
            mock => mock.TransactionCommitValidate (
                Arg.Is (ClientTransaction.Current),
                Arg<ReadOnlyCollection<PersistableData>>.Matches (
                    x => x.Select (d => d.DomainObject).SetEquals (new[] { newObject, changedObject, unchangedObject }))));
        objectEventReceiverMock.AssertWasCalled (mock => mock.Committing ());
        objectEventReceiverMock.AssertWasCalled (mock => mock.Committed ());
        transactionEventReceiverMock.AssertWasCalled (mock => mock.Committing (newObject, changedObject, unchangedObject));
        transactionEventReceiverMock.AssertWasCalled (mock => mock.Committed (newObject, changedObject, unchangedObject));

        Assert.That (newObject.State, Is.EqualTo (StateType.Unchanged));
        CheckNotMarkedAsChanged (newObject);
        Assert.That (changedObject.State, Is.EqualTo (StateType.Unchanged));
        CheckNotMarkedAsChanged (changedObject);
        Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
        CheckNotMarkedAsChanged (unchangedObject);
      }

      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (unchangedObject);
    }

    [Test]
    public void CommitSub_Nested ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      CheckNotMarkedAsChanged (domainObject);
      Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        CheckNotMarkedAsChanged (domainObject);
        Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          domainObject.RegisterForCommit();

          CheckMarkedAsChanged (domainObject);
          Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));

          using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
          {
            CheckNotMarkedAsChanged (domainObject);
            Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));

            ++domainObject.Int32Property;

            CheckNotMarkedAsChanged (domainObject);
            Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));

            ClientTransaction.Current.Commit ();

            CheckNotMarkedAsChanged (domainObject);
            Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));
          }
          CheckMarkedAsChanged (domainObject);

          ClientTransaction.Current.Commit ();

          CheckNotMarkedAsChanged (domainObject);
          Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));
        }

        CheckMarkedAsChanged (domainObject);
        Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));

        ClientTransaction.Current.Commit ();

        CheckNotMarkedAsChanged (domainObject);
        Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));
      }

      CheckMarkedAsChanged (domainObject);
      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void CommitSub_ObjectNewInParent ()
    {
      var newObject = ClassWithAllDataTypes.NewObject();
      CheckNotMarkedAsChanged (newObject);
      Assert.That (newObject.State, Is.EqualTo (StateType.New));

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        CheckNotMarkedAsChanged (newObject);
        Assert.That (newObject.State, Is.EqualTo (StateType.Unchanged));

        newObject.RegisterForCommit();

        CheckMarkedAsChanged (newObject);
        Assert.That (newObject.State, Is.EqualTo (StateType.Changed));

        ClientTransaction.Current.Commit ();

        CheckNotMarkedAsChanged (newObject);
        Assert.That (newObject.State, Is.EqualTo (StateType.Unchanged));
      }

      CheckNotMarkedAsChanged (newObject);
      Assert.That (newObject.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void Rollback ()
    {
      var newObject = ClassWithAllDataTypes.NewObject ();
      Assert.That (newObject.State, Is.EqualTo (StateType.New));

      var changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      ++changedObject.Int32Property;
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));

      var unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ();
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));

      newObject.RegisterForCommit ();
      changedObject.RegisterForCommit ();
      unchangedObject.RegisterForCommit ();

      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (unchangedObject);

      var objectEventReceiverMock = MockRepository.GenerateMock<DomainObjectMockEventReceiver> (unchangedObject);
      var transactionEventReceiverMock = MockRepository.GenerateMock<ClientTransactionMockEventReceiver> (TestableClientTransaction);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);

      TestableClientTransaction.Rollback();

      listenerMock.AssertWasCalled (
          mock => mock.TransactionRollingBack (
            Arg.Is (TestableClientTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { newObject, changedObject, unchangedObject })));
      objectEventReceiverMock.AssertWasCalled (mock => mock.RollingBack ());
      objectEventReceiverMock.AssertWasCalled (mock => mock.RolledBack ());
      transactionEventReceiverMock.AssertWasCalled (mock => mock.RollingBack (newObject, changedObject, unchangedObject));
      transactionEventReceiverMock.AssertWasCalled (mock => mock.RolledBack (changedObject, unchangedObject));

      Assert.That (newObject.State, Is.EqualTo (StateType.Invalid));
      Assert.That (changedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (unchangedObject);
    }

    private void CommitTransactionAndCheckTimestamps (params DomainObject[] domainObjects)
    {
      var timestampsBefore = domainObjects.Select (obj => obj.Timestamp).ToArray();
      TestableClientTransaction.Commit();
      var timestampsAfter = domainObjects.Select (obj => obj.Timestamp).ToArray ();
      Assert.That (timestampsBefore, Is.Not.EqualTo (timestampsAfter));
    }

    private void ModifyAndCommitInOtherTransaction (ObjectID objectID)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = objectID.GetObject<ClassWithAllDataTypes> ();
        ++domainObject.Int32Property;
        ClientTransaction.Current.Commit();
      }
    }
    
    private void CheckMarkedAsChanged (ClassWithAllDataTypes domainObject)
    {
      Assert.That (domainObject.InternalDataContainer.HasBeenMarkedChanged, Is.True);
    }

    private void CheckNotMarkedAsChanged (ClassWithAllDataTypes domainObject)
    {
      Assert.That (domainObject.InternalDataContainer.HasBeenMarkedChanged, Is.False);
    }
  }
}
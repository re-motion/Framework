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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class RegisterForCommitTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject_NoOp ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      Assert.That(domainObject.State.IsNew, Is.True);

      domainObject.RegisterForCommit();

      Assert.That(domainObject.State.IsNew, Is.True);
      CheckNotMarkedAsChanged(domainObject);
    }

    [Test]
    public void ChangedObject_RemembersRegistration_EvenWhenChangedBack ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      ++domainObject.Int32Property;
      Assert.That(domainObject.State.IsChanged, Is.True);

      domainObject.RegisterForCommit();

      Assert.That(domainObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(domainObject);

      --domainObject.Int32Property;
      Assert.That(domainObject.State.IsChanged, Is.True);
    }

    [Test]
    public void UnchangedObject ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      Assert.That(domainObject.State.IsUnchanged, Is.True);

      domainObject.RegisterForCommit();

      Assert.That(domainObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(domainObject);

      ++domainObject.Int32Property;
      --domainObject.Int32Property;

      Assert.That(domainObject.State.IsChanged, Is.True);
    }

    [Test]
    public void DeletedObject ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      domainObject.Delete();
      Assert.That(domainObject.State.IsDeleted, Is.True);

      Assert.That(() => domainObject.RegisterForCommit(), Throws.Nothing);

      Assert.That(domainObject.State.IsDeleted, Is.True);
      CheckNotMarkedAsChanged(domainObject);
    }

    [Test]
    public void InvalidObject ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      domainObject.Delete();
      Assert.That(domainObject.State.IsInvalid, Is.True);

      Assert.That(() => domainObject.RegisterForCommit(), Throws.TypeOf<ObjectInvalidException>());

      Assert.That(domainObject.State.IsInvalid, Is.True);

      ResurrectionService.ResurrectInvalidObject(TestableClientTransaction, domainObject.ID);

      Assert.That(domainObject.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void NotLoadedYetObject_LoadedToUnchanged ()
    {
      var domainObject = (ClassWithAllDataTypes)LifetimeService.GetObjectReference(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
      Assert.That(domainObject.State.IsNotLoadedYet, Is.True);

      domainObject.RegisterForCommit();

      Assert.That(domainObject.State.IsChanged, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
      CheckMarkedAsChanged(domainObject);
    }

    [Test]
    public void NotLoadedYetObject_LoadedToChanged ()
    {
      var domainObject = (ClassWithAllDataTypes)LifetimeService.GetObjectReference(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      domainObject.ProtectedLoaded += (sender, args) => ++domainObject.Int32Property;
      Assert.That(TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
      Assert.That(domainObject.State.IsNotLoadedYet, Is.True);

      domainObject.RegisterForCommit();

      Assert.That(domainObject.State.IsChanged, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
      CheckMarkedAsChanged(domainObject);
    }

    [Test]
    public void CommitRoot ()
    {
      var newObject = ClassWithAllDataTypes.NewObject();
      newObject.PopulateMandatoryProperties();
      Assert.That(newObject.State.IsNew, Is.True);

      var changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      ++changedObject.Int32Property;
      changedObject.TransactionOnlyStringProperty = "TransactionOnly"; // the default value is null, which is invalid for this property during commit
      changedObject.TransactionOnlyBinaryProperty = new byte[] { 08, 15 }; // the default value is null, which is invalid for this property during commit
      Assert.That(changedObject.State.IsChanged, Is.True);

      var unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
      unchangedObject.TransactionOnlyStringProperty = "TransactionOnly"; // the default value is null, which is invalid for this property during commit
      unchangedObject.TransactionOnlyBinaryProperty = new byte[] { 47, 11 }; // the default value is null, which is invalid for this property during commit
      unchangedObject.InternalDataContainer.CommitState(); // we want to test an unchanged object, so the previous change must be committed
      Assert.That(unchangedObject.State.IsUnchanged, Is.True);

      newObject.RegisterForCommit();
      changedObject.RegisterForCommit();
      unchangedObject.RegisterForCommit();

      Assert.That(newObject.State.IsNew, Is.True);
      CheckNotMarkedAsChanged(newObject);
      Assert.That(changedObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(changedObject);
      Assert.That(unchangedObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(unchangedObject);

      var objectEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Default, newObject);
      var transactionEventReceiverMock = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Default, TestableClientTransaction);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      CommitTransactionAndCheckTimestamps(newObject, changedObject, unchangedObject);

      listenerMock.Verify(
          mock => mock.TransactionCommitting(
              TestableClientTransaction,
              It.Is<ReadOnlyCollection<DomainObject>>(x => x.SetEquals(new[] { newObject, changedObject, unchangedObject })),
              It.IsAny<ICommittingEventRegistrar>()),
          Times.AtLeastOnce());
      listenerMock.Verify(
          mock => mock.TransactionCommitValidate(
              TestableClientTransaction,
              It.Is<ReadOnlyCollection<PersistableData>>(x => x.Select(d => d.DomainObject).SetEquals(new[] { newObject, changedObject, unchangedObject }))),
          Times.AtLeastOnce());
      objectEventReceiverMock.Verify(mock => mock.Committing(newObject, It.IsAny<DomainObjectCommittingEventArgs>()), Times.AtLeastOnce());
      objectEventReceiverMock.Verify(mock => mock.Committed(newObject, It.IsAny<EventArgs>()), Times.AtLeastOnce());
      transactionEventReceiverMock
          .VerifyCommitting(TestableClientTransaction, new[] { newObject, changedObject, unchangedObject }, Times.AtLeastOnce());
      transactionEventReceiverMock
          .VerifyCommitted(TestableClientTransaction, new[] { newObject, changedObject, unchangedObject }, Times.AtLeastOnce());

      Assert.That(newObject.State.IsUnchanged, Is.True);
      CheckNotMarkedAsChanged(newObject);
      Assert.That(changedObject.State.IsUnchanged, Is.True);
      CheckNotMarkedAsChanged(changedObject);
      Assert.That(unchangedObject.State.IsUnchanged, Is.True);
      CheckNotMarkedAsChanged(unchangedObject);
    }

    [Test]
    public void CommitRoot_RegisterForUnchanged_LeadsToConcurrencyCheck ()
    {
      var unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
      unchangedObject.TransactionOnlyStringProperty = "TransactionOnly"; // the default value is null, which is invalid for this property during commit
      unchangedObject.TransactionOnlyBinaryProperty = new byte[] { 47, 11 }; // the default value is null, which is invalid for this property during commit
      unchangedObject.InternalDataContainer.CommitState(); // we want to test an unchanged object, so the previous change must be committed
      unchangedObject.RegisterForCommit();

      ModifyAndCommitInOtherTransaction(unchangedObject.ID);

      Assert.That(() => TestableClientTransaction.Commit(), Throws.TypeOf<ConcurrencyViolationException>());
    }

    [Test]
    public void CommitSub ()
    {
      ClassWithAllDataTypes newObject;
      ClassWithAllDataTypes changedObject;
      ClassWithAllDataTypes unchangedObject;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        newObject = ClassWithAllDataTypes.NewObject();
        Assert.That(newObject.State.IsNew, Is.True);

        changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
        ++changedObject.Int32Property;
        Assert.That(changedObject.State.IsChanged, Is.True);

        unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
        Assert.That(unchangedObject.State.IsUnchanged, Is.True);

        newObject.RegisterForCommit();
        changedObject.RegisterForCommit();
        unchangedObject.RegisterForCommit();

        Assert.That(newObject.State.IsNew, Is.True);
        CheckNotMarkedAsChanged(newObject);
        Assert.That(changedObject.State.IsChanged, Is.True);
        CheckMarkedAsChanged(changedObject);
        Assert.That(unchangedObject.State.IsChanged, Is.True);
        CheckMarkedAsChanged(unchangedObject);

        var objectEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Default, newObject);
        var transactionEventReceiverMock = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Default, ClientTransaction.Current);
        var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(ClientTransaction.Current);
        try
        {
          ClientTransaction.Current.Commit();
        }
        finally
        {
          ClientTransactionTestHelper.RemoveListener(ClientTransaction.Current, listenerMock.Object);
        }

        listenerMock.Verify(
            mock => mock.TransactionCommitting(
                ClientTransaction.Current,
                It.Is<ReadOnlyCollection<DomainObject>>(x => x.SetEquals(new[] { newObject, changedObject, unchangedObject })),
                It.IsAny<ICommittingEventRegistrar>()),
            Times.AtLeastOnce());
        listenerMock.Verify(
            mock => mock.TransactionCommitValidate(
                ClientTransaction.Current,
                It.Is<ReadOnlyCollection<PersistableData>>(x => x.Select(d => d.DomainObject).SetEquals(new[] { newObject, changedObject, unchangedObject }))),
            Times.AtLeastOnce());
        objectEventReceiverMock.Verify(mock => mock.Committing(newObject, It.IsAny<DomainObjectCommittingEventArgs>()), Times.AtLeastOnce());
        objectEventReceiverMock.Verify(mock => mock.Committed(newObject, It.IsAny<EventArgs>()), Times.AtLeastOnce());
        transactionEventReceiverMock
            .VerifyCommitting(ClientTransaction.Current, new[] { newObject, changedObject, unchangedObject }, Times.AtLeastOnce());
        transactionEventReceiverMock
            .VerifyCommitted(ClientTransaction.Current, new[] { newObject, changedObject, unchangedObject }, Times.AtLeastOnce());

        Assert.That(newObject.State.IsUnchanged, Is.True);
        CheckNotMarkedAsChanged(newObject);
        Assert.That(changedObject.State.IsUnchanged, Is.True);
        CheckNotMarkedAsChanged(changedObject);
        Assert.That(unchangedObject.State.IsUnchanged, Is.True);
        CheckNotMarkedAsChanged(unchangedObject);
      }

      Assert.That(newObject.State.IsNew, Is.True);
      CheckNotMarkedAsChanged(newObject);
      Assert.That(changedObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(changedObject);
      Assert.That(unchangedObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(unchangedObject);
    }

    [Test]
    public void CommitSub_Nested ()
    {
      var domainObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      CheckNotMarkedAsChanged(domainObject);
      Assert.That(domainObject.State.IsUnchanged, Is.True);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        CheckNotMarkedAsChanged(domainObject);
        Assert.That(domainObject.State.IsUnchanged, Is.True);

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          domainObject.RegisterForCommit();

          CheckMarkedAsChanged(domainObject);
          Assert.That(domainObject.State.IsChanged, Is.True);

          using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
          {
            CheckNotMarkedAsChanged(domainObject);
            Assert.That(domainObject.State.IsUnchanged, Is.True);

            ++domainObject.Int32Property;

            CheckNotMarkedAsChanged(domainObject);
            Assert.That(domainObject.State.IsChanged, Is.True);

            ClientTransaction.Current.Commit();

            CheckNotMarkedAsChanged(domainObject);
            Assert.That(domainObject.State.IsUnchanged, Is.True);
          }
          CheckMarkedAsChanged(domainObject);

          ClientTransaction.Current.Commit();

          CheckNotMarkedAsChanged(domainObject);
          Assert.That(domainObject.State.IsUnchanged, Is.True);
        }

        CheckMarkedAsChanged(domainObject);
        Assert.That(domainObject.State.IsChanged, Is.True);

        ClientTransaction.Current.Commit();

        CheckNotMarkedAsChanged(domainObject);
        Assert.That(domainObject.State.IsUnchanged, Is.True);
      }

      CheckMarkedAsChanged(domainObject);
      Assert.That(domainObject.State.IsChanged, Is.True);
    }

    [Test]
    public void CommitSub_ObjectNewInParent ()
    {
      var newObject = ClassWithAllDataTypes.NewObject();
      CheckNotMarkedAsChanged(newObject);
      Assert.That(newObject.State.IsNew, Is.True);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        CheckNotMarkedAsChanged(newObject);
        Assert.That(newObject.State.IsUnchanged, Is.True);

        newObject.RegisterForCommit();

        CheckMarkedAsChanged(newObject);
        Assert.That(newObject.State.IsChanged, Is.True);

        ClientTransaction.Current.Commit();

        CheckNotMarkedAsChanged(newObject);
        Assert.That(newObject.State.IsUnchanged, Is.True);
      }

      CheckNotMarkedAsChanged(newObject);
      Assert.That(newObject.State.IsNew, Is.True);
    }

    [Test]
    public void Rollback ()
    {
      var newObject = ClassWithAllDataTypes.NewObject();
      Assert.That(newObject.State.IsNew, Is.True);

      var changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      ++changedObject.Int32Property;
      Assert.That(changedObject.State.IsChanged, Is.True);

      var unchangedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
      Assert.That(unchangedObject.State.IsUnchanged, Is.True);

      newObject.RegisterForCommit();
      changedObject.RegisterForCommit();
      unchangedObject.RegisterForCommit();

      Assert.That(newObject.State.IsNew, Is.True);
      CheckNotMarkedAsChanged(newObject);
      Assert.That(changedObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(changedObject);
      Assert.That(unchangedObject.State.IsChanged, Is.True);
      CheckMarkedAsChanged(unchangedObject);

      var objectEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Default, unchangedObject);
      var transactionEventReceiverMock = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Default, TestableClientTransaction);
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      TestableClientTransaction.Rollback();

      listenerMock.Verify(
          mock => mock.TransactionRollingBack(
              TestableClientTransaction,
              It.Is<ReadOnlyCollection<DomainObject>>(x => x.SetEquals(new[] { newObject, changedObject, unchangedObject }))),
          Times.AtLeastOnce());
      objectEventReceiverMock.Verify(mock => mock.RollingBack(unchangedObject, It.IsAny<EventArgs>()), Times.AtLeastOnce());
      objectEventReceiverMock.Verify(mock => mock.RolledBack(unchangedObject, It.IsAny<EventArgs>()), Times.AtLeastOnce());
      transactionEventReceiverMock.VerifyRollingBack(TestableClientTransaction, new[]{newObject, changedObject, unchangedObject}, Times.AtLeastOnce());
      transactionEventReceiverMock.VerifyRolledBack(TestableClientTransaction, new []{changedObject, unchangedObject}, Times.AtLeastOnce());

      Assert.That(newObject.State.IsInvalid, Is.True);
      Assert.That(changedObject.State.IsUnchanged, Is.True);
      CheckNotMarkedAsChanged(changedObject);
      Assert.That(unchangedObject.State.IsUnchanged, Is.True);
      CheckNotMarkedAsChanged(unchangedObject);
    }

    private void CommitTransactionAndCheckTimestamps (params DomainObject[] domainObjects)
    {
      var timestampsBefore = domainObjects.Select(obj => obj.Timestamp).ToArray();
      TestableClientTransaction.Commit();
      var timestampsAfter = domainObjects.Select(obj => obj.Timestamp).ToArray();
      Assert.That(timestampsBefore, Is.Not.EqualTo(timestampsAfter));
    }

    private void ModifyAndCommitInOtherTransaction (ObjectID objectID)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = objectID.GetObject<ClassWithAllDataTypes>();
        ++domainObject.Int32Property;
        domainObject.TransactionOnlyStringProperty = "TransactionOnly"; // the default value is null, which is invalid for this property during commit
        domainObject.TransactionOnlyBinaryProperty = new byte[] { 47, 11 }; // the default value is null, which is invalid for this property during commit
        ClientTransaction.Current.Commit();
      }
    }

    private void CheckMarkedAsChanged (ClassWithAllDataTypes domainObject)
    {
      Assert.That(domainObject.InternalDataContainer.HasBeenMarkedChanged, Is.True);
    }

    private void CheckNotMarkedAsChanged (ClassWithAllDataTypes domainObject)
    {
      Assert.That(domainObject.InternalDataContainer.HasBeenMarkedChanged, Is.False);
    }
  }
}

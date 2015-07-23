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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class TransactionHierarchyManagerTest : StandardMappingTest
  {
    private ClientTransaction _thisTransaction;
    private IClientTransactionEventSink _thisEventSinkWithStrictMock;

    private ClientTransaction _parentTransaction;
    private ITransactionHierarchyManager _parentHierarchyManagerStrictMock;
    private IClientTransactionEventSink _parentEventSinkWithStrictMock;

    private TransactionHierarchyManager _manager;
    private TransactionHierarchyManager _managerWithoutParent;
    private IClientTransactionHierarchy _hierarchyStrictMock;

    public override void SetUp ()
    {
      base.SetUp();

      _thisTransaction = ClientTransactionObjectMother.Create ();
      _thisEventSinkWithStrictMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();
      _hierarchyStrictMock = MockRepository.GenerateStrictMock<IClientTransactionHierarchy>();

      _parentTransaction = ClientTransactionObjectMother.Create ();
      _parentHierarchyManagerStrictMock = MockRepository.GenerateStrictMock<ITransactionHierarchyManager>();
      _parentHierarchyManagerStrictMock.Stub (stub => stub.TransactionHierarchy).Return (_hierarchyStrictMock);
      _parentEventSinkWithStrictMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();

      _manager = new TransactionHierarchyManager (
          _thisTransaction, _thisEventSinkWithStrictMock, _parentTransaction, _parentHierarchyManagerStrictMock, _parentEventSinkWithStrictMock);
      _managerWithoutParent = new TransactionHierarchyManager (_thisTransaction, _thisEventSinkWithStrictMock);
    }

    [Test]
    public void Initialization_WithParent ()
    {
      Assert.That (_manager.ThisTransaction, Is.SameAs (_thisTransaction));
      Assert.That (_manager.ThisEventSink, Is.SameAs (_thisEventSinkWithStrictMock));
      Assert.That (_manager.ParentTransaction, Is.SameAs (_parentTransaction));
      Assert.That (_manager.ParentHierarchyManager, Is.SameAs (_parentHierarchyManagerStrictMock));
      Assert.That (_manager.ParentEventSink, Is.SameAs (_parentEventSinkWithStrictMock));
      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.TransactionHierarchy, Is.SameAs (_hierarchyStrictMock));
    }

    [Test]
    public void Initialization_WithoutParent ()
    {
      Assert.That (_managerWithoutParent.ThisTransaction, Is.SameAs (_thisTransaction));
      Assert.That (_managerWithoutParent.ThisEventSink, Is.SameAs (_thisEventSinkWithStrictMock));
      Assert.That (_managerWithoutParent.ParentTransaction, Is.Null);
      Assert.That (_managerWithoutParent.ParentHierarchyManager, Is.Null);
      Assert.That (_managerWithoutParent.ParentEventSink, Is.Null);
      Assert.That (_managerWithoutParent.IsWriteable, Is.True);
      Assert.That (_managerWithoutParent.SubTransaction, Is.Null);
      Assert.That (
          _managerWithoutParent.TransactionHierarchy,
          Is.TypeOf<ClientTransactionHierarchy>().With.Property<ClientTransactionHierarchy> (h => h.RootTransaction).SameAs (_thisTransaction));
    }

    [Test]
    public void InstallListeners ()
    {
      var eventBrokerMock = MockRepository.GenerateStrictMock<IClientTransactionEventBroker>();
      eventBrokerMock.Expect (mock => mock.AddListener (Arg<ReadOnlyClientTransactionListener>.Is.TypeOf));
      eventBrokerMock.Expect (mock => mock.AddListener (Arg<NewObjectHierarchyInvalidationClientTransactionListener>.Is.TypeOf));

      _manager.InstallListeners (eventBrokerMock);
    }

    [Test]
    public void OnBeforeTransactionInitialize ()
    {
      _parentEventSinkWithStrictMock.Expect (mock => mock.RaiseSubTransactionInitializeEvent ( _thisTransaction));
      ClientTransactionTestHelper.SetIsWriteable (_parentTransaction, false); // required by assertion in ReadOnlyClientTransactionListener

      _manager.OnBeforeTransactionInitialize();

      _parentEventSinkWithStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void OnBeforeTransactionInitialize_NullParent ()
    {
      Assert.That (() => _managerWithoutParent.OnBeforeTransactionInitialize(), Throws.Nothing);
    }

    [Test]
    public void OnTransactionDiscard ()
    {
      _parentHierarchyManagerStrictMock.Expect (mock => mock.RemoveSubTransaction());

      _manager.OnTransactionDiscard ();

      _parentHierarchyManagerStrictMock.VerifyAllExpectations ();
    }

    [Test]
    public void OnTransactionDiscard_NullParent ()
    {
      Assert.That (() => _managerWithoutParent.OnTransactionDiscard (), Throws.Nothing);
    }

    [Test]
    public void OnTransactionDiscard_WithSubTransaction ()
    {
      FakeManagerWithSubtransaction (_manager);
      ClientTransaction fakeSubTransaction = _manager.SubTransaction;

      Assert.That (_manager.SubTransaction, Is.SameAs (fakeSubTransaction));
      Assert.That (fakeSubTransaction.IsDiscarded, Is.False);

      _parentHierarchyManagerStrictMock
          .Expect (mock => mock.RemoveSubTransaction())
          .WhenCalled (
              mi =>
              Assert.That (
                  fakeSubTransaction.IsDiscarded,
                  Is.True,
                  "Subtransaction should be discarded before this transaction is removed from the parent tx, so that the hierarchy is still intact "
                  + "within SubTransaction's Discard listener."));

      _manager.OnTransactionDiscard();

      Assert.That (fakeSubTransaction.IsDiscarded, Is.True);
    }

    [Test]
    public void OnBeforeObjectRegistration_WithoutParent ()
    {
      Assert.That (_managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);

      _managerWithoutParent.OnBeforeObjectRegistration (Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));

      Assert.That (
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, 
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));

      _managerWithoutParent.OnBeforeObjectRegistration (Array.AsReadOnly (new[] { DomainObjectIDs.Order4 }));

      Assert.That (
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));
    }

    [Test]
    public void OnBeforeObjectRegistration_WithParent ()
    {
      Assert.That (_manager.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);

      _parentHierarchyManagerStrictMock
          .Expect (mock => mock.OnBeforeSubTransactionObjectRegistration (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }))
          .WhenCalled (mi => Assert.That (_manager.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty));

      _manager.OnBeforeObjectRegistration (Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));

      _parentHierarchyManagerStrictMock.VerifyAllExpectations();
      Assert.That (
          _manager.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));
    }

    [Test]
    public void OnAfterObjectRegistration ()
    {
      _managerWithoutParent.OnBeforeObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));
      Assert.That (
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));

      _managerWithoutParent.OnAfterObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));

      Assert.That (
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order4 }));

      _managerWithoutParent.OnAfterObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order4 }));

      Assert.That (_managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);
    }

    [Test]
    public void OnBeforeSubTransactionObjectRegistration_NoConflicts ()
    {
      Assert.That (
          () =>_manager.OnBeforeSubTransactionObjectRegistration (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }),
          Throws.Nothing);
    }

    [Test]
    public void OnBeforeSubTransactionObjectRegistration_Conflicts ()
    {
      _managerWithoutParent.OnBeforeObjectRegistration (
          Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));

      Assert.That (
          () => _managerWithoutParent.OnBeforeSubTransactionObjectRegistration (
              new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order5 }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'."));
    }

    [Test]
    public void CreateSubTransaction ()
    {
      var counter = new OrderedExpectationCounter();
      bool subTransactionCreatingCalled = false;
      _thisEventSinkWithStrictMock.Expect (mock => mock.RaiseSubTransactionCreatingEvent ())
          .WhenCalledOrdered (counter, mi =>
          {
            Assert.That (_manager.IsWriteable, Is.True);
            subTransactionCreatingCalled = true;
          });

      ClientTransaction fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent (_thisTransaction);
      Func<ClientTransaction, ClientTransaction> factory = tx =>
      {
        Assert.That (tx, Is.SameAs (_thisTransaction));
        Assert.That (subTransactionCreatingCalled, Is.True);
        Assert.That (_manager.IsWriteable, Is.False, "IsWriteable needs to be set before the factory is called.");
        ClientTransactionTestHelper.SetIsWriteable (_thisTransaction, false); // required by assertion in ReadOnlyClientTransactionListener
        return fakeSubTransaction;
      };

      _hierarchyStrictMock.Expect (mock => mock.AppendLeafTransaction (fakeSubTransaction)).Ordered (counter);
      _thisEventSinkWithStrictMock.Expect (mock => mock.RaiseSubTransactionCreatedEvent (fakeSubTransaction)).Ordered (counter);

      var result = _manager.CreateSubTransaction (factory);

      Assert.That (result, Is.Not.Null.And.SameAs (fakeSubTransaction));
      Assert.That (_manager.SubTransaction, Is.SameAs (fakeSubTransaction));
      Assert.That (_manager.IsWriteable, Is.False);

      _hierarchyStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateSubTransaction_InvalidFactory ()
    {
      _thisEventSinkWithStrictMock.Expect (mock => mock.RaiseSubTransactionCreatingEvent ());

      var fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent (null);
      Func<ClientTransaction, ClientTransaction> factory = tx => fakeSubTransaction;

      Assert.That (
          () => _manager.CreateSubTransaction (factory), 
          Throws.InvalidOperationException.With.Message.EqualTo ("The given factory did not create a sub-transaction for this transaction."));

      _thisEventSinkWithStrictMock.AssertWasNotCalled (mock => mock.RaiseSubTransactionCreatedEvent ( Arg<ClientTransaction>.Is.Anything));
      _thisEventSinkWithStrictMock.VerifyAllExpectations();

      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.IsWriteable, Is.True);
    }

    [Test]
    public void CreateSubTransaction_ThrowingFactory ()
    {
      _thisEventSinkWithStrictMock.Expect (mock => mock.RaiseSubTransactionCreatingEvent ());

      var exception = new Exception();
      Func<ClientTransaction, ClientTransaction> factory = tx => { throw exception; };

      Assert.That (
          () => _manager.CreateSubTransaction (factory),
          Throws.Exception.SameAs (exception));

      _thisEventSinkWithStrictMock.AssertWasNotCalled (mock => mock.RaiseSubTransactionCreatedEvent ( Arg<ClientTransaction>.Is.Anything));
      _thisEventSinkWithStrictMock.VerifyAllExpectations();

      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.IsWriteable, Is.True);
    }

    [Test]
    public void CreateSubTransaction_BeginEventAbortsOperation ()
    {
      var exception = new Exception ();
      _thisEventSinkWithStrictMock.Expect (mock => mock.RaiseSubTransactionCreatingEvent ()).Throw (exception);

      Func<ClientTransaction, ClientTransaction> factory = tx => { Assert.Fail ("Must not be called."); return null; };

      Assert.That (
          () => _manager.CreateSubTransaction (factory),
          Throws.Exception.SameAs (exception));

      _thisEventSinkWithStrictMock.AssertWasNotCalled (mock => mock.RaiseSubTransactionCreatedEvent ( Arg<ClientTransaction>.Is.Anything));
      _thisEventSinkWithStrictMock.VerifyAllExpectations();

      Assert.That (_manager.SubTransaction, Is.Null);
      Assert.That (_manager.IsWriteable, Is.True);
    }

    [Test]
    public void RemoveSubTransaction_NoSubtransaction ()
    {
      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
      _hierarchyStrictMock.Stub (stub => stub.LeafTransaction).Return (_thisTransaction);

      _manager.RemoveSubTransaction();

      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void RemoveSubTransaction_WithSubtransaction ()
    {
      FakeManagerWithSubtransaction (_manager);

      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      _hierarchyStrictMock.Stub (stub => stub.LeafTransaction).Return (_manager.SubTransaction);
      _hierarchyStrictMock.Expect (mock => mock.RemoveLeafTransaction());

      _manager.RemoveSubTransaction ();

      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
      _hierarchyStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void Unlock_Writeable ()
    {
      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);

      Assert.That (
          () => _manager.Unlock(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              _thisTransaction + " cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed "
              + "while its parent transaction is engaged in an infrastructure operation. During such an operation, the subtransaction cannot be used."));

      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void Unlock_ReadOnly ()
    {
      FakeManagerWithSubtransaction (_manager);
      
      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      var result = _manager.Unlock();

      Assert.That (result, Is.Not.Null);
      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose();

      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose ();

      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);
    }

    [Test]
    public void UnlockIfRequired_Writeable ()
    {
      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);

      var result = _manager.UnlockIfRequired();
      Assert.That (result, Is.Null);

      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void UnlockIfRequired_ReadOnly ()
    {
      FakeManagerWithSubtransaction (_manager);

      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      var result = _manager.UnlockIfRequired ();

      Assert.That (result, Is.Not.Null);
      Assert.That (_manager.IsWriteable, Is.True);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose ();

      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);

      result.Dispose ();

      Assert.That (_manager.IsWriteable, Is.False);
      Assert.That (_manager.SubTransaction, Is.Not.Null);
    }

    [Test]
    public void Serializable ()
    {
      var instance = new TransactionHierarchyManager (
          ClientTransactionObjectMother.Create(),
          new SerializableClientTransactionEventSinkFake(),
          ClientTransactionObjectMother.Create (), 
          new SerializableTransactionHierarchyManagerFake(),
          new SerializableClientTransactionEventSinkFake());
      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.IsWriteable, Is.True);
      Assert.That (deserializedInstance.SubTransaction, Is.Null);
      Assert.That (deserializedInstance.ThisTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.ParentTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.ParentHierarchyManager, Is.Not.Null);
    }

    private void FakeManagerWithSubtransaction (TransactionHierarchyManager transactionHierarchyManager)
    {
      TransactionHierarchyManagerTestHelper.SetIsWriteable (transactionHierarchyManager, false);
      var fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent (transactionHierarchyManager.ThisTransaction);
      TransactionHierarchyManagerTestHelper.SetSubtransaction (transactionHierarchyManager, fakeSubTransaction);
    }
  }
}
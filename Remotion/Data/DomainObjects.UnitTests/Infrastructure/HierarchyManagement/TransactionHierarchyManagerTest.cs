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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class TransactionHierarchyManagerTest : StandardMappingTest
  {
    private ClientTransaction _thisTransaction;
    private Mock<IClientTransactionEventSink> _thisEventSinkWithStrictMock;

    private ClientTransaction _parentTransaction;
    private Mock<ITransactionHierarchyManager> _parentHierarchyManagerStrictMock;
    private Mock<IClientTransactionEventSink> _parentEventSinkWithStrictMock;

    private TransactionHierarchyManager _manager;
    private TransactionHierarchyManager _managerWithoutParent;
    private Mock<IClientTransactionHierarchy> _hierarchyStrictMock;

    public override void SetUp ()
    {
      base.SetUp();

      _thisTransaction = ClientTransactionObjectMother.Create();
      _thisEventSinkWithStrictMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
      _hierarchyStrictMock = new Mock<IClientTransactionHierarchy>(MockBehavior.Strict);

      _parentTransaction = ClientTransactionObjectMother.Create();
      _parentHierarchyManagerStrictMock = new Mock<ITransactionHierarchyManager>(MockBehavior.Strict);
      _parentHierarchyManagerStrictMock.Setup(stub => stub.TransactionHierarchy).Returns(_hierarchyStrictMock.Object);
      _parentEventSinkWithStrictMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _manager = new TransactionHierarchyManager(
          _thisTransaction, _thisEventSinkWithStrictMock.Object, _parentTransaction, _parentHierarchyManagerStrictMock.Object, _parentEventSinkWithStrictMock.Object);
      _managerWithoutParent = new TransactionHierarchyManager(_thisTransaction, _thisEventSinkWithStrictMock.Object);
    }

    [Test]
    public void Initialization_WithParent ()
    {
      Assert.That(_manager.ThisTransaction, Is.SameAs(_thisTransaction));
      Assert.That(_manager.ThisEventSink, Is.SameAs(_thisEventSinkWithStrictMock.Object));
      Assert.That(_manager.ParentTransaction, Is.SameAs(_parentTransaction));
      Assert.That(_manager.ParentHierarchyManager, Is.SameAs(_parentHierarchyManagerStrictMock.Object));
      Assert.That(_manager.ParentEventSink, Is.SameAs(_parentEventSinkWithStrictMock.Object));
      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);
      Assert.That(_manager.TransactionHierarchy, Is.SameAs(_hierarchyStrictMock.Object));
    }

    [Test]
    public void Initialization_WithoutParent ()
    {
      Assert.That(_managerWithoutParent.ThisTransaction, Is.SameAs(_thisTransaction));
      Assert.That(_managerWithoutParent.ThisEventSink, Is.SameAs(_thisEventSinkWithStrictMock.Object));
      Assert.That(_managerWithoutParent.ParentTransaction, Is.Null);
      Assert.That(_managerWithoutParent.ParentHierarchyManager, Is.Null);
      Assert.That(_managerWithoutParent.ParentEventSink, Is.Null);
      Assert.That(_managerWithoutParent.IsWriteable, Is.True);
      Assert.That(_managerWithoutParent.SubTransaction, Is.Null);
      Assert.That(
          _managerWithoutParent.TransactionHierarchy,
          Is.TypeOf<ClientTransactionHierarchy>().With.Property<ClientTransactionHierarchy>(h => h.RootTransaction).SameAs(_thisTransaction));
    }

    [Test]
    public void InstallListeners ()
    {
      var eventBrokerMock = new Mock<IClientTransactionEventBroker>(MockBehavior.Strict);
      eventBrokerMock.Setup(mock => mock.AddListener(It.IsNotNull<ReadOnlyClientTransactionListener>())).Verifiable();
      eventBrokerMock.Setup(mock => mock.AddListener(It.IsNotNull<NewObjectHierarchyInvalidationClientTransactionListener>())).Verifiable();

      _manager.InstallListeners(eventBrokerMock.Object);
    }

    [Test]
    public void OnBeforeTransactionInitialize ()
    {
      _parentEventSinkWithStrictMock.Setup(mock => mock.RaiseSubTransactionInitializeEvent(_thisTransaction)).Verifiable();
      ClientTransactionTestHelper.SetIsWriteable(_parentTransaction, false); // required by assertion in ReadOnlyClientTransactionListener

      _manager.OnBeforeTransactionInitialize();

      _parentEventSinkWithStrictMock.Verify();
    }

    [Test]
    public void OnBeforeTransactionInitialize_NullParent ()
    {
      Assert.That(() => _managerWithoutParent.OnBeforeTransactionInitialize(), Throws.Nothing);
    }

    [Test]
    public void OnTransactionDiscard ()
    {
      _parentHierarchyManagerStrictMock.Setup(mock => mock.RemoveSubTransaction()).Verifiable();

      _manager.OnTransactionDiscard();

      _parentHierarchyManagerStrictMock.Verify();
    }

    [Test]
    public void OnTransactionDiscard_NullParent ()
    {
      Assert.That(() => _managerWithoutParent.OnTransactionDiscard(), Throws.Nothing);
    }

    [Test]
    public void OnTransactionDiscard_WithSubTransaction ()
    {
      FakeManagerWithSubtransaction(_manager);
      ClientTransaction fakeSubTransaction = _manager.SubTransaction;

      Assert.That(_manager.SubTransaction, Is.SameAs(fakeSubTransaction));
      Assert.That(fakeSubTransaction.IsDiscarded, Is.False);

      _parentHierarchyManagerStrictMock
          .Setup(mock => mock.RemoveSubTransaction())
          .Callback(
              () =>
              Assert.That(
                  fakeSubTransaction.IsDiscarded,
                  Is.True,
                  "Subtransaction should be discarded before this transaction is removed from the parent tx, so that the hierarchy is still intact "
                  + "within SubTransaction's Discard listener."))
          .Verifiable();

      _manager.OnTransactionDiscard();

      Assert.That(fakeSubTransaction.IsDiscarded, Is.True);
    }

    [Test]
    public void OnBeforeObjectRegistration_WithoutParent ()
    {
      Assert.That(_managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);

      _managerWithoutParent.OnBeforeObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      Assert.That(
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));

      _managerWithoutParent.OnBeforeObjectRegistration(new[] { DomainObjectIDs.Order4 });

      Assert.That(
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));
    }

    [Test]
    public void OnBeforeObjectRegistration_WithParent ()
    {
      Assert.That(_manager.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);

      _parentHierarchyManagerStrictMock
          .Setup(mock => mock.OnBeforeSubTransactionObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }))
          .Callback((IReadOnlyList<ObjectID> _) => Assert.That(_manager.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty))
          .Verifiable();

      _manager.OnBeforeObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      _parentHierarchyManagerStrictMock.Verify();
      Assert.That(
          _manager.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));
    }

    [Test]
    public void OnAfterObjectRegistration ()
    {
      _managerWithoutParent.OnBeforeObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 });
      Assert.That(
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));

      _managerWithoutParent.OnAfterObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      Assert.That(
          _managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order4 }));

      _managerWithoutParent.OnAfterObjectRegistration(new[] { DomainObjectIDs.Order4 });

      Assert.That(_managerWithoutParent.ReadOnlyClientTransactionListener.CurrentlyLoadingObjectIDs, Is.Empty);
    }

    [Test]
    public void OnBeforeSubTransactionObjectRegistration_NoConflicts ()
    {
      Assert.That(
          () =>_manager.OnBeforeSubTransactionObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }),
          Throws.Nothing);
    }

    [Test]
    public void OnBeforeSubTransactionObjectRegistration_Conflicts ()
    {
      _managerWithoutParent.OnBeforeObjectRegistration(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 });

      Assert.That(
          () => _managerWithoutParent.OnBeforeSubTransactionObjectRegistration(
              new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order5 }),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'."));
    }

    [Test]
    public void CreateSubTransaction ()
    {
      var sequence = new VerifiableSequence();
      bool subTransactionCreatingCalled = false;
      _thisEventSinkWithStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RaiseSubTransactionCreatingEvent())
          .Callback(
              () =>
              {
                Assert.That(_manager.IsWriteable, Is.True);
                subTransactionCreatingCalled = true;
              })
          .Verifiable();

      ClientTransaction fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent(_thisTransaction);
      Func<ClientTransaction, ClientTransaction> factory = tx =>
      {
        Assert.That(tx, Is.SameAs(_thisTransaction));
        Assert.That(subTransactionCreatingCalled, Is.True);
        Assert.That(_manager.IsWriteable, Is.False, "IsWriteable needs to be set before the factory is called.");
        ClientTransactionTestHelper.SetIsWriteable(_thisTransaction, false); // required by assertion in ReadOnlyClientTransactionListener
        return fakeSubTransaction;
      };
      _hierarchyStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.AppendLeafTransaction(fakeSubTransaction))
          .Verifiable();
      _thisEventSinkWithStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RaiseSubTransactionCreatedEvent(fakeSubTransaction))
          .Verifiable();

      var result = _manager.CreateSubTransaction(factory);

      Assert.That(result, Is.Not.Null.And.SameAs(fakeSubTransaction));
      Assert.That(_manager.SubTransaction, Is.SameAs(fakeSubTransaction));
      Assert.That(_manager.IsWriteable, Is.False);

      _hierarchyStrictMock.Verify();
      _thisEventSinkWithStrictMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CreateSubTransaction_InvalidFactory ()
    {
      _thisEventSinkWithStrictMock.Setup(mock => mock.RaiseSubTransactionCreatingEvent()).Verifiable();

      var fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent(null);
      Func<ClientTransaction, ClientTransaction> factory = tx => fakeSubTransaction;

      Assert.That(
          () => _manager.CreateSubTransaction(factory),
          Throws.InvalidOperationException.With.Message.EqualTo("The given factory did not create a sub-transaction for this transaction."));

      _thisEventSinkWithStrictMock.Verify(mock => mock.RaiseSubTransactionCreatedEvent(It.IsAny<ClientTransaction>()), Times.Never());
      _thisEventSinkWithStrictMock.Verify();

      Assert.That(_manager.SubTransaction, Is.Null);
      Assert.That(_manager.IsWriteable, Is.True);
    }

    [Test]
    public void CreateSubTransaction_ThrowingFactory ()
    {
      _thisEventSinkWithStrictMock.Setup(mock => mock.RaiseSubTransactionCreatingEvent()).Verifiable();

      var exception = new Exception();
      Func<ClientTransaction, ClientTransaction> factory = tx => { throw exception; };

      Assert.That(
          () => _manager.CreateSubTransaction(factory),
          Throws.Exception.SameAs(exception));

      _thisEventSinkWithStrictMock.Verify(mock => mock.RaiseSubTransactionCreatedEvent(It.IsAny<ClientTransaction>()), Times.Never());
      _thisEventSinkWithStrictMock.Verify();

      Assert.That(_manager.SubTransaction, Is.Null);
      Assert.That(_manager.IsWriteable, Is.True);
    }

    [Test]
    public void CreateSubTransaction_BeginEventAbortsOperation ()
    {
      var exception = new Exception();
      _thisEventSinkWithStrictMock.Setup(mock => mock.RaiseSubTransactionCreatingEvent()).Throws(exception).Verifiable();

      Func<ClientTransaction, ClientTransaction> factory = tx => { Assert.Fail("Must not be called."); return null; };

      Assert.That(
          () => _manager.CreateSubTransaction(factory),
          Throws.Exception.SameAs(exception));

      _thisEventSinkWithStrictMock.Verify(mock => mock.RaiseSubTransactionCreatedEvent(It.IsAny<ClientTransaction>()), Times.Never());
      _thisEventSinkWithStrictMock.Verify();

      Assert.That(_manager.SubTransaction, Is.Null);
      Assert.That(_manager.IsWriteable, Is.True);
    }

    [Test]
    public void RemoveSubTransaction_NoSubtransaction ()
    {
      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);
      _hierarchyStrictMock.Setup(stub => stub.LeafTransaction).Returns(_thisTransaction);

      _manager.RemoveSubTransaction();

      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void RemoveSubTransaction_WithSubtransaction ()
    {
      FakeManagerWithSubtransaction(_manager);

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      _hierarchyStrictMock.Setup(stub => stub.LeafTransaction).Returns(_manager.SubTransaction);
      _hierarchyStrictMock.Setup(mock => mock.RemoveLeafTransaction()).Verifiable();

      _manager.RemoveSubTransaction();

      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);
      _hierarchyStrictMock.Verify();
    }

    [Test]
    public void Unlock_Writeable ()
    {
      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);

      Assert.That(
          () => _manager.Unlock(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              _thisTransaction + " cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed "
              + "while its parent transaction is engaged in an infrastructure operation. During such an operation, the subtransaction cannot be used."));

      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void Unlock_ReadOnly ()
    {
      FakeManagerWithSubtransaction(_manager);

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      var result = _manager.Unlock();

      Assert.That(result, Is.Not.Null);
      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      result.Dispose();

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      result.Dispose();

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);
    }

    [Test]
    public void UnlockIfRequired_Writeable ()
    {
      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);

      var result = _manager.UnlockIfRequired();
      Assert.That(result, Is.Null);

      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Null);
    }

    [Test]
    public void UnlockIfRequired_ReadOnly ()
    {
      FakeManagerWithSubtransaction(_manager);

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      var result = _manager.UnlockIfRequired();

      Assert.That(result, Is.Not.Null);
      Assert.That(_manager.IsWriteable, Is.True);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      result.Dispose();

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);

      result.Dispose();

      Assert.That(_manager.IsWriteable, Is.False);
      Assert.That(_manager.SubTransaction, Is.Not.Null);
    }

    private void FakeManagerWithSubtransaction (TransactionHierarchyManager transactionHierarchyManager)
    {
      TransactionHierarchyManagerTestHelper.SetIsWriteable(transactionHierarchyManager, false);
      var fakeSubTransaction = ClientTransactionObjectMother.CreateWithParent(transactionHierarchyManager.ThisTransaction);
      TransactionHierarchyManagerTestHelper.SetSubtransaction(transactionHierarchyManager, fakeSubTransaction);
    }
  }
}

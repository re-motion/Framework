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
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  public class CommitRollbackFullEventChainTestBase : StandardMappingTest
  {
    private ClientTransaction _transaction;

    private DomainObject _unchangedObject;
    private DomainObject _changedObject;
    private DomainObject _newObject;
    private DomainObject _deletedObject;

    private Mock<IClientTransactionListener> _listenerMock;
    private Mock<ClientTransactionExtensionBase> _extensionMock;
    private Mock<IClientTransactionMockEventReceiver> _transactionMockEventReceiver;
    private Mock<IDomainObjectMockEventReceiver> _changedObjectEventReceiverMock;
    private Mock<IDomainObjectMockEventReceiver> _newObjectEventReceiverMock;
    private Mock<IDomainObjectMockEventReceiver> _deletedObjectEventReceiverMock;
    private Mock<IDomainObjectMockEventReceiver> _unchangedObjectEventReceiverMock;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction().CreateSubTransaction();

      _unchangedObject = GetUnchangedObject();
      _changedObject = GetChangedObject();
      _newObject = GetNewObject();
      _deletedObject = GetDeletedObject();

      // Listener is a dynamic mock so that we don't have to expect all the internal events of committing
      _listenerMock = new Mock<IClientTransactionListener>();
      _extensionMock = new Mock<ClientTransactionExtensionBase>(MockBehavior.Strict, "test");
      _transactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, _transaction);

      _changedObjectEventReceiverMock = CreateDomainObjectMockEventReceiver(_changedObject);
      _newObjectEventReceiverMock = CreateDomainObjectMockEventReceiver(_newObject);
      _deletedObjectEventReceiverMock = CreateDomainObjectMockEventReceiver(_deletedObject);
      _unchangedObjectEventReceiverMock = CreateDomainObjectMockEventReceiver(_unchangedObject);

      ClientTransactionTestHelper.AddListener(_transaction, _listenerMock.Object);
      _transaction.Extensions.Add(_extensionMock.Object);
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public DomainObject UnchangedObject
    {
      get { return _unchangedObject; }
    }

    public DomainObject ChangedObject
    {
      get { return _changedObject; }
    }

    public DomainObject NewObject
    {
      get { return _newObject; }
    }

    public DomainObject DeletedObject
    {
      get { return _deletedObject; }
    }

    public Mock<IClientTransactionListener> ListenerMock
    {
      get { return _listenerMock; }
    }

    public Mock<ClientTransactionExtensionBase> ExtensionMock
    {
      get { return _extensionMock; }
    }

    public Mock<IClientTransactionMockEventReceiver> TransactionMockEventReceiver
    {
      get { return _transactionMockEventReceiver; }
    }

    public Mock<IDomainObjectMockEventReceiver> ChangedObjectEventReceiverMock
    {
      get { return _changedObjectEventReceiverMock; }
    }

    public Mock<IDomainObjectMockEventReceiver> NewObjectEventReceiverMock
    {
      get { return _newObjectEventReceiverMock; }
    }

    public Mock<IDomainObjectMockEventReceiver> DeletedObjectEventReceiverMock
    {
      get { return _deletedObjectEventReceiverMock; }
    }

    public Mock<IDomainObjectMockEventReceiver> UnchangedObjectEventReceiverMock
    {
      get { return _unchangedObjectEventReceiverMock; }
    }

    protected void VerifyAll ()
    {
      ListenerMock.Verify();
      ExtensionMock.Verify();
      ChangedObjectEventReceiverMock.Verify();
      NewObjectEventReceiverMock.Verify();
      UnchangedObjectEventReceiverMock.Verify();
      DeletedObjectEventReceiverMock.Verify();
    }

    private protected void ExpectCommittingEvents (
        VerifiableSequence sequence,
        (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[] domainObjectsAndMocks,
        Action<IInvocation> extensionCallback = null,
        Action<IInvocation> transactionCallback = null,
        bool suppressVerificationForListenerMock = false,
        bool suppressVerificationForExtensionMock = false)
    {
      var domainObjects = domainObjectsAndMocks.Select(t => t.DomainObject).ToArray();

      var listenerMockSetup = ListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.TransactionCommitting(
                  _transaction,
                  It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects)),
                  It.IsNotNull<CommittingEventRegistrar>()));
      if (!suppressVerificationForListenerMock)
          listenerMockSetup.Verifiable();

      var extensionMockSetup = ExtensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.Committing(
                  _transaction,
                  It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects)),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Callback((IInvocation invocation) => extensionCallback?.Invoke(invocation));
      if (!suppressVerificationForExtensionMock)
        extensionMockSetup.Verifiable();

      TransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupCommitting(_transaction, domainObjects)
          .Callback(
              (IInvocation invocation) =>
              {
                Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                transactionCallback?.Invoke(invocation);
              })
          .Verifiable();


      foreach (var (_, mock, callback) in domainObjectsAndMocks)
      {
        mock
            .Setup(_ => _.Committing(It.IsAny<object>(), It.IsAny<DomainObjectCommittingEventArgs>()))
            .Callback(
                (IInvocation invocation) =>
                {
                  Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                  callback?.Invoke(invocation);
                })
            .Verifiable();
      }
    }

    private protected void ExpectCommittedEvents (
        VerifiableSequence sequence,
        (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[] domainObjectsAndMocks,
        Action<IInvocation> extensionCallback = null,
        Action<IInvocation> transactionCallback = null)
    {
      foreach (var (_, mock, callback) in domainObjectsAndMocks)
      {
        mock
            .Setup(_ => _.Committed(It.IsAny<object>(), It.IsAny<EventArgs>()))
            .Callback(
                (IInvocation invocation) =>
                {
                  Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                  callback?.Invoke(invocation);
                })
            .Verifiable();
      }

      var domainObjects = domainObjectsAndMocks.Select(t => t.DomainObject).ToArray();
      TransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupCommitted(_transaction, domainObjects)
          .Callback(
              (IInvocation invocation) =>
              {
                Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                transactionCallback?.Invoke(invocation);
              })
          .Verifiable();

      ExtensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committed(_transaction, It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Callback((IInvocation invocation) => extensionCallback?.Invoke(invocation))
          .Verifiable();

      ListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionCommitted(_transaction, It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Verifiable();
    }

    private protected void ExpectCommitValidateEvents (VerifiableSequence sequence, DomainObject[] domainObjects)
    {
      ListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionCommitValidate(_transaction, It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(domainObjects))))
          .Callback((ClientTransaction _, IReadOnlyList<PersistableData> _) => Assert.That(_transaction.HasChanged(), Is.True, "CommitValidate: last event before actual commit."))
          .Verifiable();

      ExtensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CommitValidate(_transaction, It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(domainObjects))))
          .Verifiable();
    }

    private protected void ExpectRollingBackEvents (
        VerifiableSequence sequence,
        (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[] domainObjectsAndMocks,
        Action<IInvocation> extensionCallback = null,
        Action<IInvocation> transactionCallback = null)
    {
      var domainObjects = domainObjectsAndMocks.Select(t => t.DomainObject).ToArray();
      ListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionRollingBack(Transaction, It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Verifiable();

      ExtensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RollingBack(Transaction, It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Callback((IInvocation invocation) => extensionCallback?.Invoke(invocation))
          .Verifiable();

      TransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(Transaction, domainObjects)
          .Callback(
              (IInvocation invocation) =>
              {
                Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                transactionCallback?.Invoke(invocation);
              })
          .Verifiable();

      foreach (var (_, mock, callback) in domainObjectsAndMocks)
      {
        mock
            .Setup(_ => _.RollingBack(It.IsAny<object>(), It.IsAny<EventArgs>()))
            .Callback(
                (IInvocation invocation) =>
                {
                  Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                  callback?.Invoke(invocation);
                })
            .Verifiable();
      }
    }

    private protected void ExpectRolledBackEvents (
        VerifiableSequence sequence,
        (DomainObject DomainObject, Mock<IDomainObjectMockEventReceiver> Mock, Action<IInvocation> Callback)[] domainObjectsAndMocks,
        Action<IInvocation> extensionCallback = null,
        Action<IInvocation> transactionCallback = null)
    {
      foreach (var (_, mock, callback) in domainObjectsAndMocks)
      {
        mock
            .Setup(_ => _.RolledBack(It.IsAny<object>(), It.IsAny<EventArgs>()))
            .Callback(
                (IInvocation invocation) =>
                {
                  Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                  callback?.Invoke(invocation);
                })
            .Verifiable();
      }

      var domainObjects = domainObjectsAndMocks.Select(t => t.DomainObject).ToArray();

      TransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(Transaction, domainObjects)
          .Callback(
              (IInvocation invocation) =>
              {
                Assert.That(ClientTransaction.Current, Is.SameAs(Transaction));
                transactionCallback?.Invoke(invocation);
              })
          .Verifiable();

      ExtensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RolledBack(Transaction, It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Callback((IInvocation invocation) => extensionCallback?.Invoke(invocation))
          .Verifiable();

      ListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionRolledBack(Transaction, It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Verifiable();
    }

    private DomainObject GetDeletedObject ()
    {
      return _transaction.ExecuteInScope(
          () =>
          {
            var instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
            instance.Delete();
            return instance;
          });
    }

    private DomainObject GetNewObject ()
    {
      return _transaction.ExecuteInScope(() => ClassWithAllDataTypes.NewObject());
    }

    private DomainObject GetChangedObject ()
    {
      return _transaction.ExecuteInScope(
          () =>
          {
            var instance = DomainObjectIDs.Order1.GetObject<Order>();
            instance.RegisterForCommit();
            return instance;
          });
    }

    private DomainObject GetUnchangedObject ()
    {
      return _transaction.ExecuteInScope(() => DomainObjectIDs.Customer1.GetObject<Customer>());
    }

    private Mock<IDomainObjectMockEventReceiver> CreateDomainObjectMockEventReceiver (DomainObject changedObject)
    {
      return DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, changedObject);
    }
  }
}

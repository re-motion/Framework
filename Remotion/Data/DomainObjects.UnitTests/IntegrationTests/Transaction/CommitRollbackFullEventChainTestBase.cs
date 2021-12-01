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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  public class CommitRollbackFullEventChainTestBase : StandardMappingTest
  {
    protected struct AllMethodOptions
    {
      private readonly IMethodOptions<RhinoMocksExtensions.VoidType> _extensionOptions;
      private readonly IMethodOptions<RhinoMocksExtensions.VoidType> _transactionOptions;
      private readonly IMethodOptions<RhinoMocksExtensions.VoidType>[] _domainObjectOptions;

      public AllMethodOptions (
          IMethodOptions<RhinoMocksExtensions.VoidType> extensionOptions,
          IMethodOptions<RhinoMocksExtensions.VoidType> transactionOptions,
          IMethodOptions<RhinoMocksExtensions.VoidType>[] domainObjectOptions)
      {
        _extensionOptions = extensionOptions;
        _transactionOptions = transactionOptions;
        _domainObjectOptions = domainObjectOptions;
      }

      public IMethodOptions<RhinoMocksExtensions.VoidType> ExtensionOptions
      {
        get { return _extensionOptions; }
      }

      public IMethodOptions<RhinoMocksExtensions.VoidType> TransactionOptions
      {
        get { return _transactionOptions; }
      }

      public IMethodOptions<RhinoMocksExtensions.VoidType>[] DomainObjectOptions
      {
        get { return _domainObjectOptions; }
      }
    }

    private ClientTransaction _transaction;

    private DomainObject _unchangedObject;
    private DomainObject _changedObject;
    private DomainObject _newObject;
    private DomainObject _deletedObject;

    private Mock<IClientTransactionListener> _listenerMock;
    private Mock<IClientTransactionExtension> _extensionMock;
    private Mock<ClientTransactionMockEventReceiver> _transactionMockEventReceiver;
    private DomainObjectMockEventReceiver _changedObjectEventReceiverMock;
    private DomainObjectMockEventReceiver _newObjectEventReceiverMock;
    private DomainObjectMockEventReceiver _deletedObjectEventReceiverMock;
    private DomainObjectMockEventReceiver _unchangedObjectEventReceiverMock;

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
      _extensionMock = new Mock<ClientTransactionExtensionBase> (MockBehavior.Strict, "test");
      _transactionMockEventReceiver = new Mock<ClientTransactionMockEventReceiver> (MockBehavior.Strict, _transaction);

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

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    public IClientTransactionListener ListenerMock
    {
      get { return _listenerMock.Object; }
    }

    public IClientTransactionExtension ExtensionMock
    {
      get { return _extensionMock.Object; }
    }

    public ClientTransactionMockEventReceiver TransactionMockEventReceiver
    {
      get { return _transactionMockEventReceiver.Object; }
    }

    public DomainObjectMockEventReceiver ChangedObjectEventReceiverMock
    {
      get { return _changedObjectEventReceiverMock; }
    }

    public DomainObjectMockEventReceiver NewObjectEventReceiverMock
    {
      get { return _newObjectEventReceiverMock; }
    }

    public DomainObjectMockEventReceiver DeletedObjectEventReceiverMock
    {
      get { return _deletedObjectEventReceiverMock; }
    }

    public DomainObjectMockEventReceiver UnchangedObjectEventReceiverMock
    {
      get { return _unchangedObjectEventReceiverMock; }
    }

    protected void ExpectCommittingEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var methodOptions = ExpectCommittingEventsWithCustomOptions(domainObjectsAndMocks);

      methodOptions.TransactionOptions.WithCurrentTransaction(_transaction);

      foreach (var domainObjectOption in methodOptions.DomainObjectOptions)
        domainObjectOption.WithCurrentTransaction(_transaction);
    }

    protected AllMethodOptions ExpectCommittingEventsWithCustomOptions (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var sequence = new MockSequence();
      var domainObjects = domainObjectsAndMocks.Select(t => t.Item1).ToArray();
      ListenerMock
            .Setup(mock => mock.TransactionCommitting(_transaction, ArgIsDomainObjectSet(domainObjects), Arg<CommittingEventRegistrar>.Is.TypeOf))
            .Verifiable();
      var extensionOptions = ExtensionMock
            .Setup(mock => mock.Committing(_transaction, ArgIsDomainObjectSet(domainObjects), Arg<CommittingEventRegistrar>.Is.TypeOf));
      var transactionOptions = TransactionMockEventReceiver.Setup(mock => mock.Committing(domainObjects));
      IMethodOptions<RhinoMocksExtensions.VoidType>[] domainObjectOptions;
      using (_mockRepository.Unordered())
        {
          domainObjectOptions = domainObjectsAndMocks.Select(t => t.Item2.Setup(mock => mock.Committing())).ToArray();
        }
      return new AllMethodOptions(extensionOptions, transactionOptions, domainObjectOptions);
    }

    protected void ExpectCommittedEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var methodOptions = ExpectCommittedEventsWithCustomOptions(domainObjectsAndMocks);

      methodOptions.TransactionOptions.WithCurrentTransaction(_transaction);

      foreach (var domainObjectOption in methodOptions.DomainObjectOptions)
        domainObjectOption.WithCurrentTransaction(_transaction);
    }

    protected AllMethodOptions ExpectCommittedEventsWithCustomOptions (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var sequence = new MockSequence();
      var domainObjectOptions = new List<IMethodOptions<RhinoMocksExtensions.VoidType>>(domainObjectsAndMocks.Length);
      using (_mockRepository.Unordered())
        {
            domainObjectOptions.AddRange(domainObjectsAndMocks.Select(t => t.Item2.Setup(mock => mock.Committed())));
        }
      var domainObjects = domainObjectsAndMocks.Select(t => t.Item1).ToArray();
      var transactionOptions = TransactionMockEventReceiver.Setup(mock => mock.Committed(domainObjects));
      var extensionOptions = ExtensionMock.Setup(mock => mock.Committed(_transaction, ArgIsDomainObjectSet(domainObjects)));
      ListenerMock.Setup (mock => mock.TransactionCommitted (_transaction, ArgIsDomainObjectSet (domainObjects))).Verifiable();
      return new AllMethodOptions(extensionOptions, transactionOptions, domainObjectOptions.ToArray());
    }

    protected void ExpectCommitValidateEvents (params DomainObject[] domainObjects)
    {
      var sequence = new MockSequence();
      ListenerMock
            .Setup(mock => mock.TransactionCommitValidate(_transaction, ArgIsPersistableDataSet(domainObjects)))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData) => Assert.That(_transaction.HasChanged(), Is.True, "CommitValidate: last event before actual commit."))
            .Verifiable();
      ExtensionMock
            .Setup(mock => mock.CommitValidate(_transaction, ArgIsPersistableDataSet(domainObjects)))
            .Verifiable();
    }

    protected void ExpectRollingBackEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var methodOptions = ExpectRollingBackEventsWithCustomOptions(domainObjectsAndMocks);

      methodOptions.TransactionOptions.WithCurrentTransaction(_transaction);
      foreach (var domainObjectOption in methodOptions.DomainObjectOptions)
        domainObjectOption.WithCurrentTransaction(_transaction);
    }

    protected AllMethodOptions ExpectRollingBackEventsWithCustomOptions (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var sequence = new MockSequence();
      var domainObjects = domainObjectsAndMocks.Select(t => t.Item1).ToArray();
      _listenerMock
            .InSequence (sequence)
            .Setup(mock => mock.TransactionRollingBack(Transaction, ArgIsDomainObjectSet(domainObjects)))
            .Verifiable();
      var extensionOptions = _extensionMock
            .Setup(mock => mock.RollingBack(Transaction, ArgIsDomainObjectSet(domainObjects)));
      var transactionOptions = TransactionMockEventReceiver
            .Setup(mock => mock.RollingBack(domainObjects))
            .WithCurrentTransaction(Transaction);
      IMethodOptions<RhinoMocksExtensions.VoidType>[] domainObjectOptions;
      using (_mockRepository.Unordered())
        {
          domainObjectOptions = domainObjectsAndMocks.Select(t => t.Item2.Setup(mock => mock.RollingBack())).ToArray();
        }
      return new AllMethodOptions(extensionOptions, transactionOptions, domainObjectOptions);
    }

    protected void ExpectRolledBackEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var methodOptions = ExpectRolledBackEventsWithCustomOptions(domainObjectsAndMocks);

      methodOptions.TransactionOptions.WithCurrentTransaction(_transaction);

      foreach (var domainObjectOption in methodOptions.DomainObjectOptions)
        domainObjectOption.WithCurrentTransaction(_transaction);
    }

    protected AllMethodOptions ExpectRolledBackEventsWithCustomOptions (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      var sequence = new MockSequence();
      IMethodOptions<RhinoMocksExtensions.VoidType>[] domainObjectOptions;
      using (_mockRepository.Unordered())
        {
          domainObjectOptions = domainObjectsAndMocks.Select(t => t.Item2.Setup(mock => mock.RolledBack())).ToArray();
        }
      var domainObjects = domainObjectsAndMocks.Select(t => t.Item1).ToArray();
      var transactionOptions = TransactionMockEventReceiver
            .Setup(mock => mock.RolledBack(domainObjects))
            .WithCurrentTransaction(Transaction);
      var extensionOptions = _extensionMock
            .Setup(mock => mock.RolledBack(Transaction, ArgIsDomainObjectSet(domainObjects)));
      _listenerMock
            .InSequence (sequence)
            .Setup(mock => mock.TransactionRolledBack(Transaction, ArgIsDomainObjectSet(domainObjects)))
            .Verifiable();
      return new AllMethodOptions(extensionOptions, transactionOptions, domainObjectOptions);
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

    protected ReadOnlyCollection<DomainObject> ArgIsDomainObjectSet (params DomainObject[] domainObjects)
    {
      return Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent(domainObjects);
    }

    private ReadOnlyCollection<PersistableData> ArgIsPersistableDataSet (params DomainObject[] domainObjecs)
    {
      return Arg<ReadOnlyCollection<PersistableData>>.Matches(c => c.Select(d => d.DomainObject).SetEquals(domainObjecs));
    }

    private DomainObjectMockEventReceiver CreateDomainObjectMockEventReceiver (DomainObject changedObject)
    {
      return new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, changedObject).Object;
    }

    protected void RegisterForCommitWithDisabledListener (DomainObject domainObject)
    {
      // WORKAROUND: Remove listener before calling RegisterForCommit to avoid the event triggering mocked methods.
      // (Yes, the listener is a dynamic mock, but due to a bug in Rhino.Mocks, triggering an unexpected mocked method method will destroy the
      // MockRepository's replay state...)
      ClientTransactionTestHelper.RemoveListener(Transaction, ListenerMock);
      Transaction.ExecuteInScope(domainObject.RegisterForCommit);
      ClientTransactionTestHelper.AddListener(Transaction, ListenerMock);
    }
  }
}

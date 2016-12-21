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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  public abstract class CollectionEndPointModificationCommandTestBase : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private CollectionEndPoint _collectionEndPoint;
    private IDomainObjectCollectionData _collectionDataMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private Customer _domainObject;
    private DomainObjectCollectionMockEventReceiver _collectionMockEventReceiver;
    private RelationEndPointID _relationEndPointID;
    private Order _order1;
    private Order _order2;
    private IClientTransactionEventSink _transactionEventSinkMock;

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public CollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public IDomainObjectCollectionData CollectionDataMock
    {
      get { return _collectionDataMock; }
    }

    public IRelationEndPointProvider EndPointProviderStub
    {
      get { return _endPointProviderStub; }
    }

    public Customer DomainObject
    {
      get { return _domainObject; }
    }

    public DomainObjectCollectionMockEventReceiver CollectionMockEventReceiver
    {
      get { return _collectionMockEventReceiver; }
    }

    public RelationEndPointID RelationEndPointID
    {
      get { return _relationEndPointID; }
    }

    public IClientTransactionEventSink TransactionEventSinkMock
    {
      get { return _transactionEventSinkMock; }
    }

    protected IDataManager DataManager
    {
      get { return ClientTransactionTestHelper.GetIDataManager (Transaction); }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction();

      _domainObject = DomainObjectIDs.Customer1.GetObject<Customer> (_transaction);

      _order1 = DomainObjectIDs.Order1.GetObject<Order> (_transaction);
      _order2 = DomainObjectIDs.Order2.GetObject<Order> (_transaction);

      _relationEndPointID = RelationEndPointID.Create(DomainObject.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      _collectionEndPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _relationEndPointID, new[] { _order1, _order2 }, _transaction);
      _collectionMockEventReceiver = MockRepository.GenerateStrictMock<DomainObjectCollectionMockEventReceiver> (_collectionEndPoint.Collection);

      _collectionDataMock = new MockRepository ().StrictMock<IDomainObjectCollectionData> ();
      CollectionDataMock.Replay ();

      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _transactionEventSinkMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();
    }
  }
}

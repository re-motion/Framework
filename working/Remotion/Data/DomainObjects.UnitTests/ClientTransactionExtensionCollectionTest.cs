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
using System.Diagnostics;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionExtensionCollectionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ClientTransactionExtensionCollection _collection;
    private ClientTransactionExtensionCollection _collectionWithExtensions;
    private IClientTransactionExtension _extension1;
    private IClientTransactionExtension _extension2;

    private Order _order;
    private PropertyDefinition _propertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _collection = new ClientTransactionExtensionCollection ("key");
      _extension1 = _mockRepository.StrictMock<IClientTransactionExtension> ();
      _extension2 = _mockRepository.StrictMock<IClientTransactionExtension> ();

      _extension1.Stub (stub => stub.Key).Return ("Name1");
      _extension2.Stub (stub => stub.Key).Return ("Name2");
      _mockRepository.ReplayAll();

      _collectionWithExtensions = new ClientTransactionExtensionCollection ("key");
      _collectionWithExtensions.Add (_extension1);
      _collectionWithExtensions.Add (_extension2);

      // _mockRepository.BackToRecordAll();

      _order = Order.NewObject ();
      _propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");
    }

    [Test]
    public void Initialization ()
    {
      var collection = new ClientTransactionExtensionCollection ("abc");

      Assert.That (((IClientTransactionExtension) collection).Key, Is.EqualTo ("abc"));
    }

    [Test]
    public void Add ()
    {
      Assert.That (_collection.Count, Is.EqualTo (0));

      _collection.Add (_extension1);

      Assert.That (_collection.Count, Is.EqualTo (1));
    }

    [Test]
    public void Insert ()
    {
      _collection.Add (_extension1);
      Assert.That (_collection.Count, Is.EqualTo (1));
      Assert.That (_collection[0], Is.SameAs (_extension1));

      _collection.Insert (0, _extension2);
      Assert.That (_collection.Count, Is.EqualTo (2));
      Assert.That (_collection[0], Is.SameAs (_extension2));
      Assert.That (_collection[1], Is.SameAs (_extension1));
    }

    [Test]
    public void Remove ()
    {
      _collection.Add (_extension1);
      Assert.That (_collection.Count, Is.EqualTo (1));
      _collection.Remove (_extension1.Key);
      Assert.That (_collection.Count, Is.EqualTo (0));
      _collection.Remove (_extension1.Key);
      //expectation: no exception
    }

    [Test]
    public void Indexer ()
    {
      _collection.Add (_extension1);
      _collection.Add (_extension2);
      Assert.That (_collection[0], Is.SameAs (_extension1));
      Assert.That (_collection[1], Is.SameAs (_extension2));
    }

    [Test]
    public void IndexerWithName ()
    {
      _collection.Add (_extension1);
      _collection.Add (_extension2);
      Assert.That (_collection[_extension1.Key], Is.SameAs (_extension1));
      Assert.That (_collection[_extension2.Key], Is.SameAs (_extension2));
    }

    [Test]
    public void IndexOf ()
    {
      _collection.Add (_extension1);

      Assert.That (_collection.IndexOf (_extension1.Key), Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An extension with key 'Name1' is already part of the collection.")]
    public void AddWithDuplicateKey ()
    {
      _collection.Add (_extension1);

      var extensionWithSameKey = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionWithSameKey.Stub (stub => stub.Key).Return (_extension1.Key);

      _collection.Add (extensionWithSameKey);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An extension with key 'Name1' is already part of the collection.")]
    public void InsertWithDuplicateName ()
    {
      _collection.Insert (0, _extension1);

      var extensionWithSameKey = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionWithSameKey.Stub (stub => stub.Key).Return (_extension1.Key);

      _collection.Insert (0, extensionWithSameKey);
    }

    [Test]
    public void TransactionInitialize ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.TransactionInitialize (TestableClientTransaction));
        _extension2.Expect (mock => mock.TransactionInitialize (TestableClientTransaction));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.TransactionInitialize (TestableClientTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionDiscard ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.TransactionDiscard (TestableClientTransaction));
        _extension2.Expect (mock => mock.TransactionDiscard (TestableClientTransaction));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.TransactionDiscard (TestableClientTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactionCreating ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.SubTransactionCreating (TestableClientTransaction));
        _extension2.Expect (mock => mock.SubTransactionCreating (TestableClientTransaction));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.SubTransactionCreating (TestableClientTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactionInitialize ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.SubTransactionInitialize (TestableClientTransaction, subTransaction));
        _extension2.Expect (mock => mock.SubTransactionInitialize (TestableClientTransaction, subTransaction));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.SubTransactionInitialize (TestableClientTransaction, subTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactionCreated ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.SubTransactionCreated (TestableClientTransaction, subTransaction));
        _extension2.Expect (mock => mock.SubTransactionCreated (TestableClientTransaction, subTransaction));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.SubTransactionCreated (TestableClientTransaction, subTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChanging ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.PropertyValueChanging (TestableClientTransaction, _order, _propertyDefinition, 0, 1));
        _extension2.Expect (mock => mock.PropertyValueChanging (TestableClientTransaction, _order, _propertyDefinition, 0, 1));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueChanging (TestableClientTransaction, _order, _propertyDefinition, 0, 1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChanged ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.PropertyValueChanged (TestableClientTransaction, _order, _propertyDefinition, 0, 1));
        _extension2.Expect (mock => mock.PropertyValueChanged (TestableClientTransaction, _order, _propertyDefinition, 0, 1));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueChanged (TestableClientTransaction, _order, _propertyDefinition, 0, 1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyReading ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.PropertyValueReading (TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original));
        _extension2.Expect (mock => mock.PropertyValueReading (TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueReading (TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [Explicit ("Performance test")]
    public void PropertyReading_Perf ()
    {
      var coll = new ClientTransactionExtensionCollection ("key");

      Stopwatch sw = Stopwatch.StartNew ();
      for (int i = 0; i < 100000; ++i)
        coll.PropertyValueReading (TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original);
      sw.Stop ();
      Console.WriteLine (sw.Elapsed);
      Console.WriteLine (sw.ElapsedMilliseconds / 100000.0);
    }

    [Test]
    public void PropertyRead ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.PropertyValueRead (TestableClientTransaction, _order, _propertyDefinition, 0, ValueAccess.Original));
        _extension2.Expect (mock => mock.PropertyValueRead (TestableClientTransaction, _order, _propertyDefinition, 0, ValueAccess.Original));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueRead (TestableClientTransaction, _order, _propertyDefinition, 0, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChanging ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      OrderTicket newOrderTicket = OrderTicket.NewObject ();

      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RelationChanging (TestableClientTransaction, _order, relationEndPointDefinition, orderTicket, newOrderTicket));
        _extension2.Expect (mock => mock.RelationChanging (TestableClientTransaction, _order, relationEndPointDefinition, orderTicket, newOrderTicket));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationChanging (TestableClientTransaction, _order, relationEndPointDefinition, orderTicket, newOrderTicket);
      
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChanged ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      OrderTicket newOrderTicket = OrderTicket.NewObject ();
      
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition> ();

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RelationChanged (TestableClientTransaction, _order, relationEndPointDefinition, orderTicket, newOrderTicket));
        _extension2.Expect (mock => mock.RelationChanged (TestableClientTransaction, _order, relationEndPointDefinition, orderTicket, newOrderTicket));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationChanged (TestableClientTransaction, _order, relationEndPointDefinition, orderTicket, newOrderTicket);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.NewObjectCreating (TestableClientTransaction, typeof(Order)));
        _extension2.Expect (mock => mock.NewObjectCreating (TestableClientTransaction, typeof (Order)));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.NewObjectCreating (TestableClientTransaction, typeof (Order));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleting ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectDeleting (TestableClientTransaction, _order));
        _extension2.Expect (mock => mock.ObjectDeleting (TestableClientTransaction, _order));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectDeleting (TestableClientTransaction, _order);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleted ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectDeleted (TestableClientTransaction, _order));
        _extension2.Expect (mock => mock.ObjectDeleted (TestableClientTransaction, _order));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectDeleted (TestableClientTransaction, _order);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Committing ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      var eventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar> ();
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.Committing (TestableClientTransaction, data, eventRegistrar));
        _extension2.Expect (mock => mock.Committing (TestableClientTransaction, data, eventRegistrar));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.Committing (TestableClientTransaction, data, eventRegistrar);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitValidate ()
    {
      var data = new ReadOnlyCollection<PersistableData> (new PersistableData[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.CommitValidate (TestableClientTransaction, data));
        _extension2.Expect (mock => mock.CommitValidate (TestableClientTransaction, data));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.CommitValidate (TestableClientTransaction, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Committed ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.Committed (TestableClientTransaction, data));
        _extension2.Expect (mock => mock.Committed (TestableClientTransaction, data));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.Committed (TestableClientTransaction, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RollingBack ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RollingBack (TestableClientTransaction, data));
        _extension2.Expect (mock => mock.RollingBack (TestableClientTransaction, data));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RollingBack (TestableClientTransaction, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RolledBack ()
    {
      var data = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RolledBack (TestableClientTransaction, data));
        _extension2.Expect (mock => mock.RolledBack (TestableClientTransaction, data));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RolledBack (TestableClientTransaction, data);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var loadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { _order });

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsLoaded (TestableClientTransaction, loadedDomainObjects));
        _extension2.Expect (mock => mock.ObjectsLoaded (TestableClientTransaction, loadedDomainObjects));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsLoaded (TestableClientTransaction, loadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoading ()
    {
      var objectIDs = new List<ObjectID> { _order.ID }.AsReadOnly ();

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsLoading (TestableClientTransaction, objectIDs));
        _extension2.Expect (mock => mock.ObjectsLoading (TestableClientTransaction, objectIDs));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsLoading (TestableClientTransaction, objectIDs);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      var unloadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { _order });

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsUnloaded (TestableClientTransaction, unloadedDomainObjects));
        _extension2.Expect (mock => mock.ObjectsUnloaded (TestableClientTransaction, unloadedDomainObjects));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsUnloaded (TestableClientTransaction, unloadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsUnloading ()
    {
      var unloadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { _order });

      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.ObjectsUnloading (TestableClientTransaction, unloadedDomainObjects));
        _extension2.Expect (mock => mock.ObjectsUnloading (TestableClientTransaction, unloadedDomainObjects));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsUnloading (TestableClientTransaction, unloadedDomainObjects);

      _mockRepository.VerifyAll ();
    }


    [Test]
    public void FilterQueryResult ()
    {
      var queryStub = QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ());

      var originalResult = new QueryResult<Order> (queryStub, new Order[0]);
      var newResult1 = new QueryResult<Order> (queryStub, new[] { DomainObjectIDs.Order1.GetObject<Order> () });
      var newResult2 = new QueryResult<Order> (queryStub, new[] { DomainObjectIDs.Order2.GetObject<Order> () });

      _extension1.Expect (mock => mock.FilterQueryResult (TestableClientTransaction, originalResult)).Return (newResult1);
      _extension2.Expect (mock => mock.FilterQueryResult (TestableClientTransaction, newResult1)).Return (newResult2);

      var finalResult = _collectionWithExtensions.FilterQueryResult (TestableClientTransaction, originalResult);
      Assert.That (finalResult, Is.SameAs (newResult2));

      _extension1.VerifyAllExpectations ();
      _extension2.VerifyAllExpectations ();
    }

    [Test]
    public void RelationReading ()
    {
      IRelationEndPointDefinition endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RelationReading (TestableClientTransaction, _order, endPointDefinition, ValueAccess.Current));
        _extension2.Expect (mock => mock.RelationReading (TestableClientTransaction, _order, endPointDefinition, ValueAccess.Current));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationReading (TestableClientTransaction, _order, endPointDefinition, ValueAccess.Current);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadWithOneToOneRelation ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      IRelationEndPointDefinition endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RelationRead (TestableClientTransaction, _order, endPointDefinition, orderTicket, ValueAccess.Original));
        _extension2.Expect (mock => mock.RelationRead (TestableClientTransaction, _order, endPointDefinition, orderTicket, ValueAccess.Original));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationRead (TestableClientTransaction, _order, endPointDefinition, orderTicket, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadWithOneToManyRelation ()
    {
      var orderItems = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (_order.OrderItems);
      IRelationEndPointDefinition endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      using (_mockRepository.Ordered ())
      {
        _extension1.Expect (mock => mock.RelationRead (TestableClientTransaction, _order, endPointDefinition, orderItems, ValueAccess.Original));
        _extension2.Expect (mock => mock.RelationRead (TestableClientTransaction, _order, endPointDefinition, orderItems, ValueAccess.Original));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationRead (TestableClientTransaction, _order, endPointDefinition, orderItems, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }
  }
}

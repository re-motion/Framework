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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionExtensionCollectionTest : ClientTransactionBaseTest
  {
    private ClientTransactionExtensionCollection _collection;
    private ClientTransactionExtensionCollection _collectionWithExtensions;
    private Mock<IClientTransactionExtension> _extension1;
    private Mock<IClientTransactionExtension> _extension2;

    private Order _order;
    private PropertyDefinition _propertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _collection = new ClientTransactionExtensionCollection("key");
      _extension1 = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      _extension2 = new Mock<IClientTransactionExtension>(MockBehavior.Strict);

      _extension1.Setup(stub => stub.Key).Returns("Name1");
      _extension2.Setup(stub => stub.Key).Returns("Name2");

      _collectionWithExtensions = new ClientTransactionExtensionCollection("key");
      _collectionWithExtensions.Add(_extension1.Object);
      _collectionWithExtensions.Add(_extension2.Object);

      // _mockRepository.BackToRecordAll();

      _order = Order.NewObject();
      _propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");
    }

    [Test]
    public void Initialization ()
    {
      var collection = new ClientTransactionExtensionCollection("abc");

      Assert.That(((IClientTransactionExtension)collection).Key, Is.EqualTo("abc"));
    }

    [Test]
    public void Add ()
    {
      Assert.That(_collection.Count, Is.EqualTo(0));

      _collection.Add(_extension1.Object);

      Assert.That(_collection.Count, Is.EqualTo(1));
    }

    [Test]
    public void Insert ()
    {
      _collection.Add(_extension1.Object);
      Assert.That(_collection.Count, Is.EqualTo(1));
      Assert.That(_collection[0], Is.SameAs(_extension1.Object));

      _collection.Insert(0, _extension2.Object);
      Assert.That(_collection.Count, Is.EqualTo(2));
      Assert.That(_collection[0], Is.SameAs(_extension2.Object));
      Assert.That(_collection[1], Is.SameAs(_extension1.Object));
    }

    [Test]
    public void Remove ()
    {
      _collection.Add(_extension1.Object);
      Assert.That(_collection.Count, Is.EqualTo(1));
      _collection.Remove(_extension1.Object.Key);
      Assert.That(_collection.Count, Is.EqualTo(0));
      _collection.Remove(_extension1.Object.Key);
      //expectation: no exception
    }

    [Test]
    public void Indexer ()
    {
      _collection.Add(_extension1.Object);
      _collection.Add(_extension2.Object);
      Assert.That(_collection[0], Is.SameAs(_extension1.Object));
      Assert.That(_collection[1], Is.SameAs(_extension2.Object));
    }

    [Test]
    public void IndexerWithName ()
    {
      _collection.Add(_extension1.Object);
      _collection.Add(_extension2.Object);
      Assert.That(_collection[_extension1.Object.Key], Is.SameAs(_extension1.Object));
      Assert.That(_collection[_extension2.Object.Key], Is.SameAs(_extension2.Object));
    }

    [Test]
    public void IndexOf ()
    {
      _collection.Add(_extension1.Object);

      Assert.That(_collection.IndexOf(_extension1.Object.Key), Is.EqualTo(0));
    }

    [Test]
    public void AddWithDuplicateKey ()
    {
      _collection.Add(_extension1.Object);

      var extensionWithSameKey = new Mock<IClientTransactionExtension>();
      extensionWithSameKey.Setup(stub => stub.Key).Returns(_extension1.Object.Key);
      Assert.That(
          () => _collection.Add(extensionWithSameKey.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "An extension with key 'Name1' is already part of the collection."));
    }

    [Test]
    public void InsertWithDuplicateName ()
    {
      _collection.Insert(0, _extension1.Object);

      var extensionWithSameKey = new Mock<IClientTransactionExtension>();
      extensionWithSameKey.Setup(stub => stub.Key).Returns(_extension1.Object.Key);
      Assert.That(
          () => _collection.Insert(0, extensionWithSameKey.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "An extension with key 'Name1' is already part of the collection."));
    }

    [Test]
    public void TransactionInitialize ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.TransactionInitialize(TestableClientTransaction)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.TransactionInitialize(TestableClientTransaction)).Verifiable();

      _collectionWithExtensions.TransactionInitialize(TestableClientTransaction);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void TransactionDiscard ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDiscard(TestableClientTransaction)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDiscard(TestableClientTransaction)).Verifiable();

      _collectionWithExtensions.TransactionDiscard(TestableClientTransaction);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void SubTransactionCreating ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionCreating(TestableClientTransaction)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionCreating(TestableClientTransaction)).Verifiable();

      _collectionWithExtensions.SubTransactionCreating(TestableClientTransaction);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void SubTransactionInitialize ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();

      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionInitialize(TestableClientTransaction, subTransaction)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionInitialize(TestableClientTransaction, subTransaction)).Verifiable();

      _collectionWithExtensions.SubTransactionInitialize(TestableClientTransaction, subTransaction);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void SubTransactionCreated ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();

      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionCreated(TestableClientTransaction, subTransaction)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionCreated(TestableClientTransaction, subTransaction)).Verifiable();

      _collectionWithExtensions.SubTransactionCreated(TestableClientTransaction, subTransaction);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyChanging ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.PropertyValueChanging(TestableClientTransaction, _order, _propertyDefinition, 0, 1)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.PropertyValueChanging(TestableClientTransaction, _order, _propertyDefinition, 0, 1)).Verifiable();

      _collectionWithExtensions.PropertyValueChanging(TestableClientTransaction, _order, _propertyDefinition, 0, 1);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyChanged ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.PropertyValueChanged(TestableClientTransaction, _order, _propertyDefinition, 0, 1)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.PropertyValueChanged(TestableClientTransaction, _order, _propertyDefinition, 0, 1)).Verifiable();

      _collectionWithExtensions.PropertyValueChanged(TestableClientTransaction, _order, _propertyDefinition, 0, 1);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyReading ()
    {
      var sequence = new VerifiableSequence();
      _extension1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original))
          .Verifiable();
      _extension2
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original))
          .Verifiable();

      _collectionWithExtensions.PropertyValueReading(TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    [Explicit("Performance test")]
    public void PropertyReading_Perf ()
    {
      var coll = new ClientTransactionExtensionCollection("key");

      Stopwatch sw = Stopwatch.StartNew();
      for (int i = 0; i < 100000; ++i)
        coll.PropertyValueReading(TestableClientTransaction, _order, _propertyDefinition, ValueAccess.Original);
      sw.Stop();
      Console.WriteLine(sw.Elapsed);
      Console.WriteLine(sw.ElapsedMilliseconds / 100000.0);
    }

    [Test]
    public void PropertyRead ()
    {
      var sequence = new VerifiableSequence();
      _extension1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(TestableClientTransaction, _order, _propertyDefinition, 0, ValueAccess.Original))
          .Verifiable();
      _extension2
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(TestableClientTransaction, _order, _propertyDefinition, 0, ValueAccess.Original))
          .Verifiable();

      _collectionWithExtensions.PropertyValueRead(TestableClientTransaction, _order, _propertyDefinition, 0, ValueAccess.Original);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationChanging ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      OrderTicket newOrderTicket = OrderTicket.NewObject();

      var relationEndPointDefinition = new Mock<IRelationEndPointDefinition>();

      var sequence = new VerifiableSequence();

      _extension1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, _order, relationEndPointDefinition.Object, orderTicket, newOrderTicket))
          .Verifiable();

      _extension2
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, _order, relationEndPointDefinition.Object, orderTicket, newOrderTicket))
          .Verifiable();

      _collectionWithExtensions.RelationChanging(TestableClientTransaction, _order, relationEndPointDefinition.Object, orderTicket, newOrderTicket);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationChanged ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      OrderTicket newOrderTicket = OrderTicket.NewObject();

      var relationEndPointDefinition = new Mock<IRelationEndPointDefinition>();

      var sequence = new VerifiableSequence();

      _extension1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, _order, relationEndPointDefinition.Object, orderTicket, newOrderTicket))
          .Verifiable();

      _extension2
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, _order, relationEndPointDefinition.Object, orderTicket, newOrderTicket))
          .Verifiable();

      _collectionWithExtensions.RelationChanged(TestableClientTransaction, _order, relationEndPointDefinition.Object, orderTicket, newOrderTicket);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void NewObjectCreating ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.NewObjectCreating(TestableClientTransaction, typeof(Order))).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.NewObjectCreating(TestableClientTransaction, typeof(Order))).Verifiable();

      _collectionWithExtensions.NewObjectCreating(TestableClientTransaction, typeof(Order));

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDeleting ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(TestableClientTransaction, _order)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(TestableClientTransaction, _order)).Verifiable();

      _collectionWithExtensions.ObjectDeleting(TestableClientTransaction, _order);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDeleted ()
    {
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(TestableClientTransaction, _order)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(TestableClientTransaction, _order)).Verifiable();

      _collectionWithExtensions.ObjectDeleted(TestableClientTransaction, _order);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void Committing ()
    {
      var data = new ReadOnlyCollection<DomainObject>(new DomainObject[0]);
      var eventRegistrar = new Mock<ICommittingEventRegistrar>();
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.Committing(TestableClientTransaction, data, eventRegistrar.Object)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.Committing(TestableClientTransaction, data, eventRegistrar.Object)).Verifiable();

      _collectionWithExtensions.Committing(TestableClientTransaction, data, eventRegistrar.Object);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitValidate ()
    {
      var data = new ReadOnlyCollection<PersistableData>(new PersistableData[0]);
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.CommitValidate(TestableClientTransaction, data)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.CommitValidate(TestableClientTransaction, data)).Verifiable();

      _collectionWithExtensions.CommitValidate(TestableClientTransaction, data);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void Committed ()
    {
      var data = new ReadOnlyCollection<DomainObject>(new DomainObject[0]);
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.Committed(TestableClientTransaction, data)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.Committed(TestableClientTransaction, data)).Verifiable();

      _collectionWithExtensions.Committed(TestableClientTransaction, data);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void RollingBack ()
    {
      var data = new ReadOnlyCollection<DomainObject>(new DomainObject[0]);
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.RollingBack(TestableClientTransaction, data)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.RollingBack(TestableClientTransaction, data)).Verifiable();

      _collectionWithExtensions.RollingBack(TestableClientTransaction, data);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void RolledBack ()
    {
      var data = new ReadOnlyCollection<DomainObject>(new DomainObject[0]);
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.RolledBack(TestableClientTransaction, data)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.RolledBack(TestableClientTransaction, data)).Verifiable();

      _collectionWithExtensions.RolledBack(TestableClientTransaction, data);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var loadedDomainObjects = new ReadOnlyCollection<DomainObject>(new[] { _order });

      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoaded(TestableClientTransaction, loadedDomainObjects)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoaded(TestableClientTransaction, loadedDomainObjects)).Verifiable();

      _collectionWithExtensions.ObjectsLoaded(TestableClientTransaction, loadedDomainObjects);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsLoading ()
    {
      var objectIDs = new List<ObjectID> { _order.ID }.AsReadOnly();

      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoading(TestableClientTransaction, objectIDs)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoading(TestableClientTransaction, objectIDs)).Verifiable();

      _collectionWithExtensions.ObjectsLoading(TestableClientTransaction, objectIDs);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      var unloadedDomainObjects = new ReadOnlyCollection<DomainObject>(new[] { _order });

      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsUnloaded(TestableClientTransaction, unloadedDomainObjects)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsUnloaded(TestableClientTransaction, unloadedDomainObjects)).Verifiable();

      _collectionWithExtensions.ObjectsUnloaded(TestableClientTransaction, unloadedDomainObjects);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsUnloading ()
    {
      var unloadedDomainObjects = new ReadOnlyCollection<DomainObject>(new[] { _order });

      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsUnloading(TestableClientTransaction, unloadedDomainObjects)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsUnloading(TestableClientTransaction, unloadedDomainObjects)).Verifiable();

      _collectionWithExtensions.ObjectsUnloading(TestableClientTransaction, unloadedDomainObjects);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }


    [Test]
    public void FilterQueryResult ()
    {
      var queryStub = QueryFactory.CreateQuery(TestQueryFactory.CreateOrderQueryWithCustomCollectionType(StorageSettings));

      var originalResult = new QueryResult<Order>(queryStub, new Order[0]);
      var newResult1 = new QueryResult<Order>(queryStub, new[] { DomainObjectIDs.Order1.GetObject<Order>() });
      var newResult2 = new QueryResult<Order>(queryStub, new[] { DomainObjectIDs.Order2.GetObject<Order>() });

      _extension1.Setup(mock => mock.FilterQueryResult(TestableClientTransaction, originalResult)).Returns(newResult1).Verifiable();
      _extension2.Setup(mock => mock.FilterQueryResult(TestableClientTransaction, newResult1)).Returns(newResult2).Verifiable();

      var finalResult = _collectionWithExtensions.FilterQueryResult(TestableClientTransaction, originalResult);
      Assert.That(finalResult, Is.SameAs(newResult2));

      _extension1.Verify();
      _extension2.Verify();
    }

    [Test]
    public void RelationReading ()
    {
      IRelationEndPointDefinition endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");
      var sequence = new VerifiableSequence();
      _extension1.InVerifiableSequence(sequence).Setup(mock => mock.RelationReading(TestableClientTransaction, _order, endPointDefinition, ValueAccess.Current)).Verifiable();
      _extension2.InVerifiableSequence(sequence).Setup(mock => mock.RelationReading(TestableClientTransaction, _order, endPointDefinition, ValueAccess.Current)).Verifiable();

      _collectionWithExtensions.RelationReading(TestableClientTransaction, _order, endPointDefinition, ValueAccess.Current);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationReadWithOneToOneRelation ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      IRelationEndPointDefinition endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
      var sequence = new VerifiableSequence();
      _extension1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(TestableClientTransaction, _order, endPointDefinition, orderTicket, ValueAccess.Original))
          .Verifiable();
      _extension2
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(TestableClientTransaction, _order, endPointDefinition, orderTicket, ValueAccess.Original))
          .Verifiable();

      _collectionWithExtensions.RelationRead(TestableClientTransaction, _order, endPointDefinition, orderTicket, ValueAccess.Original);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationReadWithOneToManyRelation ()
    {
      var orderItems = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(_order.OrderItems);
      IRelationEndPointDefinition endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");
      var sequence = new VerifiableSequence();
      _extension1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(TestableClientTransaction, _order, endPointDefinition, orderItems, ValueAccess.Original))
          .Verifiable();
      _extension2
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(TestableClientTransaction, _order, endPointDefinition, orderItems, ValueAccess.Original))
          .Verifiable();

      _collectionWithExtensions.RelationRead(TestableClientTransaction, _order, endPointDefinition, orderItems, ValueAccess.Original);

      _extension1.Verify();
      _extension2.Verify();
      sequence.Verify();
    }
  }
}

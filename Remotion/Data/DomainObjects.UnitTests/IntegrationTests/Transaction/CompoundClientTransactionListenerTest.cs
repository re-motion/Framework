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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class CompoundClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private Mock<IClientTransactionListener> _listener1;
    private Mock<IClientTransactionListener> _listener2;
    private CompoundClientTransactionListener _compoundListener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _listener1 = new Mock<IClientTransactionListener>(MockBehavior.Strict);
      _listener2 = new Mock<IClientTransactionListener>(MockBehavior.Strict);

      _compoundListener = new CompoundClientTransactionListener();
    }

    [Test]
    public void AddListener ()
    {
      Assert.That(_compoundListener.Listeners, Is.Empty);

      _compoundListener.AddListener(_listener1.Object);

      Assert.That(_compoundListener.Listeners, Is.EqualTo(new[] { _listener1.Object }));
    }

    [Test]
    public void RemoveListener ()
    {
      _compoundListener.AddListener(_listener1.Object);
      Assert.That(_compoundListener.Listeners, Is.EqualTo(new[] { _listener1.Object }));

      _compoundListener.RemoveListener(_listener1.Object);

      Assert.That(_compoundListener.Listeners, Is.Empty);
    }

    [Test]
    public void RemoveListener_NotFound ()
    {
      Assert.That(_compoundListener.Listeners, Is.Empty);

      _compoundListener.RemoveListener(_listener1.Object);

      Assert.That(_compoundListener.Listeners, Is.Empty);
    }

    [Test]
    public void AggregatedClientsAreNotified ()
    {
      _compoundListener.AddListener(_listener1.Object);
      _compoundListener.AddListener(_listener2.Object);

      var order = Order.NewObject();
      var order3 = Order.NewObject();
      var domainObjects = new ReadOnlyCollection<DomainObject>(new DomainObject[0]);
      var persistableData = new ReadOnlyCollection<PersistableData>(new PersistableData[0]);
      var relatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(new DomainObjectCollection());
      var clientTransaction2 = ClientTransaction.CreateRootTransaction();

      var realtionEndPointDefinitionMock = new Mock<IRelationEndPointDefinition>();

      CheckNotification(listener => listener.TransactionInitialize(TestableClientTransaction));
      CheckNotification(listener => listener.TransactionDiscard(TestableClientTransaction));

      CheckNotification(listener => listener.SubTransactionCreating(TestableClientTransaction));
      CheckNotification(listener => listener.SubTransactionInitialize(TestableClientTransaction, clientTransaction2));
      CheckNotification(listener => listener.SubTransactionCreated(TestableClientTransaction, clientTransaction2));

      CheckNotification(listener => listener.NewObjectCreating(TestableClientTransaction, typeof(string)));

      CheckNotification(listener => listener.ObjectsLoading(TestableClientTransaction, new ReadOnlyCollection<ObjectID>(new ObjectID[0])));
      CheckNotification(listener => listener.ObjectsLoaded(TestableClientTransaction, domainObjects));
      CheckNotification(listener => listener.ObjectsNotFound(TestableClientTransaction, new ReadOnlyCollection<ObjectID>(new ObjectID[0])));

      CheckNotification(listener => listener.ObjectsUnloading(TestableClientTransaction, domainObjects));
      CheckNotification(listener => listener.ObjectsUnloaded(TestableClientTransaction, domainObjects));

      CheckNotification(listener => listener.ObjectDeleting(TestableClientTransaction, order));
      CheckNotification(listener => listener.ObjectDeleted(TestableClientTransaction, order));

      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      CheckNotification(listener => listener.PropertyValueReading(
          TestableClientTransaction,
          order,
          propertyDefinition,
          ValueAccess.Original));
      CheckNotification(listener => listener.PropertyValueRead(
        TestableClientTransaction,
        order,
        propertyDefinition,
        "Foo",
        ValueAccess.Original));

      CheckNotification(listener => listener.PropertyValueChanging(
          TestableClientTransaction,
          order,
          propertyDefinition,
          "Foo",
          "Bar"));
      CheckNotification(listener => listener.PropertyValueChanged(
          TestableClientTransaction,
          order,
          propertyDefinition,
          "Foo",
          "Bar"));

      CheckNotification(listener => listener.RelationRead(TestableClientTransaction, order, realtionEndPointDefinitionMock.Object, order, ValueAccess.Original));
      CheckNotification(listener => listener.RelationRead(TestableClientTransaction, order, realtionEndPointDefinitionMock.Object, relatedObjects, ValueAccess.Original));
      CheckNotification(listener => listener.RelationReading(TestableClientTransaction, order, realtionEndPointDefinitionMock.Object, ValueAccess.Current));

      CheckNotification(listener => listener.RelationChanging(TestableClientTransaction, order, realtionEndPointDefinitionMock.Object, order, order3));
      CheckNotification(listener => listener.RelationChanged(TestableClientTransaction, order, realtionEndPointDefinitionMock.Object, order, order3));

      var eventRegistrar = new Mock<ICommittingEventRegistrar>();
      CheckNotification(listener => listener.TransactionCommitting(TestableClientTransaction, domainObjects, eventRegistrar.Object));
      CheckNotification(listener => listener.TransactionCommitValidate(TestableClientTransaction, persistableData));
      CheckNotification(listener => listener.TransactionCommitted(TestableClientTransaction, domainObjects));
      CheckNotification(listener => listener.TransactionRollingBack(TestableClientTransaction, domainObjects));
      CheckNotification(listener => listener.TransactionRolledBack(TestableClientTransaction, domainObjects));

      var id = RelationEndPointID.Create(order.ID, typeof(Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint(id, null);

      CheckNotification(listener => listener.RelationEndPointMapRegistering(TestableClientTransaction, endPoint));

      CheckNotification(listener => listener.RelationEndPointMapUnregistering(TestableClientTransaction, endPoint.ID));
      CheckNotification(listener => listener.RelationEndPointBecomingIncomplete(TestableClientTransaction, endPoint.ID));

      CheckNotification(listener => listener.ObjectMarkedInvalid(TestableClientTransaction, order));
      CheckNotification(listener => listener.ObjectMarkedNotInvalid(TestableClientTransaction, order));

      CheckNotification(listener => listener.DataContainerMapRegistering(TestableClientTransaction, order.InternalDataContainer));
      CheckNotification(listener => listener.DataContainerMapUnregistering(TestableClientTransaction, order.InternalDataContainer));

      CheckNotification(listener => listener.DataContainerStateUpdated(TestableClientTransaction, order.InternalDataContainer, new DataContainerState.Builder().SetDeleted().Value));
      CheckNotification(listener => listener.VirtualRelationEndPointStateUpdated(TestableClientTransaction, endPoint.ID, true));
    }

    [Test]
    public void FilterQueryResult ()
    {
      _compoundListener.AddListener(_listener1.Object);
      _compoundListener.AddListener(_listener2.Object);

      var listenerMock1 = new Mock<IClientTransactionListener>();
      var listenerMock2 = new Mock<IClientTransactionListener>();

      var compoundListener = new CompoundClientTransactionListener();
      compoundListener.AddListener(listenerMock1.Object);
      compoundListener.AddListener(listenerMock2.Object);

      var queryStub = new Mock<IQuery>();
      var originalResult = new QueryResult<Order>(queryStub.Object, new Order[0]);
      var newResult1 = new QueryResult<Order>(queryStub.Object, new[] { DomainObjectIDs.Order1.GetObject<Order>()});
      var newResult2 = new QueryResult<Order>(queryStub.Object, new[] { DomainObjectIDs.Order3.GetObject<Order>()});

      listenerMock1.Setup(mock => mock.FilterQueryResult(TestableClientTransaction, originalResult)).Returns(newResult1).Verifiable();
      listenerMock2.Setup(mock => mock.FilterQueryResult(TestableClientTransaction, newResult1)).Returns(newResult2).Verifiable();

      var finalResult = compoundListener.FilterQueryResult(TestableClientTransaction, originalResult);

      Assert.That(finalResult, Is.SameAs(newResult2));
    }

    [Test]
    public void FilterCustomQueryResult ()
    {
      _compoundListener.AddListener(_listener1.Object);
      _compoundListener.AddListener(_listener2.Object);

      var queryStub = new Mock<IQuery>();

      var listenerMock1 = new Mock<IClientTransactionListener>();
      var listenerMock2 = new Mock<IClientTransactionListener>();

      var compoundListener = new CompoundClientTransactionListener();
      compoundListener.AddListener(listenerMock1.Object);
      compoundListener.AddListener(listenerMock2.Object);

      var originalResult = new object[0];
      var newResult1 = new[] { new object() };
      var newResult2 = new[] { new object() };

      listenerMock1.Setup(mock => mock.FilterCustomQueryResult(TestableClientTransaction, queryStub.Object, originalResult)).Returns(newResult1).Verifiable();
      listenerMock2.Setup(mock => mock.FilterCustomQueryResult(TestableClientTransaction, queryStub.Object, newResult1)).Returns(newResult2).Verifiable();

      var finalResult = compoundListener.FilterCustomQueryResult(TestableClientTransaction, queryStub.Object, originalResult);

      Assert.That(finalResult, Is.SameAs(newResult2));
    }

    private void CheckNotification (Expression<Action<IClientTransactionListener>> notificationCall)
    {
      var sequence = new VerifiableSequence();
      _listener1.InVerifiableSequence(sequence).Setup(notificationCall).Verifiable();
      _listener2.InVerifiableSequence(sequence).Setup(notificationCall).Verifiable();

      var compiledNotificationCall = notificationCall.Compile();
      compiledNotificationCall(_compoundListener);

      _listener1.Verify();
      _listener2.Verify();
      sequence.Verify();
    }

  }
}

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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class NullClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private IClientTransactionListener _listener;
    private DataContainer _dataContainer;
    private Client _domainObject;
    private PropertyDefinition _propertyDefinition;
    private IRelationEndPoint _relationEndPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _listener = NullClientTransactionListener.Instance;
      _domainObject = DomainObjectMother.CreateObjectInTransaction<Client>(TestableClientTransaction);
      _dataContainer = _domainObject.GetInternalDataContainerForTransaction(TestableClientTransaction);
      _propertyDefinition = GetPropertyDefinition(typeof(Client), "ParentClient");
      _relationEndPoint = TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_dataContainer.AssociatedRelationEndPointIDs[0]);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(_listener.IsNull, Is.True);
    }

    [Test]
    public void RootTransactionCreating ()
    {
      _listener.TransactionInitialize(TestableClientTransaction);
    }

    [Test]
    public void TransactionDiscard ()
    {
      _listener.TransactionDiscard(TestableClientTransaction);
    }

    [Test]
    public void SubTransactionCreating ()
    {
      _listener.SubTransactionCreating(TestableClientTransaction);
    }

    [Test]
    public void SubTransactionCreated ()
    {
      _listener.SubTransactionCreated(TestableClientTransaction, TestableClientTransaction);
    }

    [Test]
    public void NewObjectCreating ()
    {
      _listener.NewObjectCreating(TestableClientTransaction, _domainObject.GetPublicDomainObjectType());
    }

    [Test]
    public void ObjectsLoading ()
    {
      _listener.ObjectsLoading(TestableClientTransaction, new ReadOnlyCollection<ObjectID>(new ObjectID[0]));
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _listener.ObjectsLoaded(TestableClientTransaction, new ReadOnlyCollection<DomainObject>(new DomainObject[0]));
    }

    [Test]
    public void ObjectsUnloading ()
    {
      _listener.ObjectsUnloading(TestableClientTransaction, new ReadOnlyCollection<DomainObject>(new DomainObject[0]));
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      _listener.ObjectsUnloaded(TestableClientTransaction, new ReadOnlyCollection<DomainObject>(new DomainObject[0]));
    }

    [Test]
    public void ObjectDeleting ()
    {
      _listener.ObjectDeleting(TestableClientTransaction, _domainObject);
    }

    [Test]
    public void ObjectDeleted ()
    {
      _listener.ObjectDeleted(TestableClientTransaction, _domainObject);
    }

    [Test]
    public void PropertyValueReading ()
    {
      _listener.PropertyValueReading(TestableClientTransaction, _domainObject, _propertyDefinition, ValueAccess.Current);
    }

    [Test]
    public void PropertyValueRead ()
    {
      _listener.PropertyValueRead(TestableClientTransaction, _domainObject, _propertyDefinition, 0, ValueAccess.Current);
    }

    [Test]
    public void PropertyValueChanging ()
    {
      _listener.PropertyValueChanging(TestableClientTransaction, _domainObject, _propertyDefinition, 0, 1);
    }

    [Test]
    public void PropertyValueChanged ()
    {
      _listener.PropertyValueChanged(TestableClientTransaction, _domainObject, _propertyDefinition, 0, 1);
    }

    [Test]
    public void RelationReading ()
    {
      _listener.RelationReading(TestableClientTransaction, _domainObject, _relationEndPoint.Definition, ValueAccess.Current);
    }

    [Test]
    public void RelationRead_SingleValue ()
    {
      _listener.RelationRead(TestableClientTransaction, _domainObject, _relationEndPoint.Definition, _domainObject, ValueAccess.Current);
    }

    [Test]
    public void RelationRead_MultiValue ()
    {
      _listener.RelationRead(
          TestableClientTransaction,
          _domainObject,
          _relationEndPoint.Definition,
          new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(new DomainObjectCollection()),
          ValueAccess.Current);
    }

    [Test]
    public void RelationChanging ()
    {
      _listener.RelationChanging(TestableClientTransaction, _domainObject, _relationEndPoint.Definition, _domainObject, _domainObject);
    }

    [Test]
    public void RelationChanged ()
    {
      _listener.RelationChanged(TestableClientTransaction, _domainObject, _relationEndPoint.Definition, _domainObject, _domainObject);
    }

    [Test]
    public void FilterQueryResult ()
    {
      var querResult = new QueryResult<DomainObject>(new Mock<IQuery>().Object, new DomainObject[0]);
      Assert.That(_listener.FilterQueryResult(TestableClientTransaction, querResult), Is.SameAs(querResult));
    }

    [Test]
    public void FilterCustomQueryResult ()
    {
      var querResult = new List<object>();
      Assert.That(_listener.FilterCustomQueryResult(TestableClientTransaction, new Mock<IQuery>().Object, querResult), Is.SameAs(querResult));
    }

    [Test]
    public void TransactionCommitting ()
    {
      _listener.TransactionCommitting(
          TestableClientTransaction,
          new ReadOnlyCollection<DomainObject>(new DomainObject[0]),
          new Mock<ICommittingEventRegistrar>().Object);
    }

    [Test]
    public void TransactionCommitted ()
    {
      _listener.TransactionCommitted(TestableClientTransaction, new ReadOnlyCollection<DomainObject>(new DomainObject[0]));
    }

    [Test]
    public void TransactionRollingBack ()
    {
      _listener.TransactionRollingBack(TestableClientTransaction, new ReadOnlyCollection<DomainObject>(new DomainObject[0]));
    }

    [Test]
    public void TransactionRolledBack ()
    {
      _listener.TransactionRolledBack(TestableClientTransaction, new ReadOnlyCollection<DomainObject>(new DomainObject[0]));
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      _listener.RelationEndPointMapRegistering(TestableClientTransaction, _relationEndPoint);
    }

    [Test]
    public void RelationEndPointMapUnregistering ()
    {
      _listener.RelationEndPointMapUnregistering(TestableClientTransaction, _relationEndPoint.ID);
    }

    [Test]
    public void RelationEndPointBecomingIncomplete ()
    {
      _listener.RelationEndPointBecomingIncomplete(TestableClientTransaction, _relationEndPoint.ID);
    }

    [Test]
    public void DataManagerMarkingObjectDiscarded ()
    {
      _listener.ObjectMarkedInvalid(TestableClientTransaction, _domainObject);
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      _listener.DataContainerMapRegistering(TestableClientTransaction, _dataContainer);
    }

    [Test]
    public void DataContainerMapUnregistering ()
    {
      _listener.DataContainerMapUnregistering(TestableClientTransaction, _dataContainer);
    }
  }
}
